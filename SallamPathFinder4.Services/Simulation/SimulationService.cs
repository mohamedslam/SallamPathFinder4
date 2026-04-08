#region File Header
/// <summary>
/// File: SimulationService.cs
/// Description: Service for robot simulation along paths with obstacle detection
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.ML.Training;
#endregion

namespace SallamPathFinder4.Services.Simulation
{
    public sealed class SimulationService : ISimulationService, IDisposable
    {
        #region Constants
        private const double DEFAULT_STEP_DELAY_SECONDS = 1.0;
        private const double SQRT2 = 1.4142135623730951;
        #endregion

        #region Private Fields
        private readonly MapGrid _mapGrid;
        private readonly List<DynamicObstacle> _dynamicObstacles;
        private readonly Random _random;
        private readonly object _obstacleLock = new object();

        private IReadOnlyList<PathNode> _currentPath;
        private int _currentStep;
        private Timer _simulationTimer;
        private bool _isRunning;
        private bool _isPaused;
        private double _stepDelaySeconds;

        private double _robotViewAngle = 180.0;
        private int _detectionRangeCells = 2;
        private bool _enableDetection = true;
        private Point _lastRobotPosition;
        private float _lastRobotAngle;

        private bool _isDisposed;
        private List<Point> _goalPositions;
        private bool[] _reachedGoals;
        private readonly SynchronizationContext _uiContext;

        // Special cells tracking
        private HashSet<Point> _collisionCells;
        private HashSet<Point> _scannedCells;

        // ML Data Collection
        private ObstacleDataCollector _dataCollector;
        private bool _collectTrainingData;

        // Performance tracking
        private double _totalWeightedTime;
        private double _totalDistance;
        private double _baseSpeed;
        private int _collisionCount;
        private int _invalidMoveCount;
        private DoorGroupManager _doorGroupManager;
        private bool _isWaitingForDoor;
        private Point _waitingDoorCell;
        #endregion

        #region Constructor
        public SimulationService(MapGrid grid, List<DynamicObstacle> obstacles, bool collectTrainingData = false)
        {
            _mapGrid = grid ?? throw new ArgumentNullException(nameof(grid));
            _dynamicObstacles = obstacles ?? new List<DynamicObstacle>();
            _random = new Random();
            _currentPath = new List<PathNode>();
            _currentStep = 0;
            _isRunning = false;
            _isPaused = false;
            _stepDelaySeconds = DEFAULT_STEP_DELAY_SECONDS;
            _lastRobotPosition = new Point(0, 0);
            _lastRobotAngle = 0;
            _collisionCells = new HashSet<Point>();
            _scannedCells = new HashSet<Point>();
            _dataCollector = new ObstacleDataCollector();
            _collectTrainingData = collectTrainingData;
            _baseSpeed = 10.0; // Default base speed cm/s
            _totalWeightedTime = 0;
            _totalDistance = 0;
            _collisionCount = 0;
            _invalidMoveCount = 0;

            _uiContext = SynchronizationContext.Current ?? new SynchronizationContext();
            _doorGroupManager = new DoorGroupManager(grid);
            _doorGroupManager.FindDoorGroups();
            _doorGroupManager.DoorStateChanged += OnDoorStateChanged;
            _isWaitingForDoor = false;
        }
        #endregion

        #region Properties
        public Point CurrentRobotPosition => _lastRobotPosition;
        public float CurrentRobotAngle => _lastRobotAngle;
        public bool IsRunning => _isRunning;
        public bool IsPaused => _isPaused;
        public IReadOnlyCollection<Point> CollisionCells => _collisionCells;
        public IReadOnlyCollection<Point> ScannedCells => _scannedCells;
        public int CollisionCount => _collisionCount;
        public int InvalidMoveCount => _invalidMoveCount;
        public double AverageActualSpeed => _totalDistance > 0 && _totalWeightedTime > 0 ? _totalDistance / _totalWeightedTime : _baseSpeed;
        public double TotalDistance => _totalDistance;
        public double TotalWeightedTime => _totalWeightedTime;
        public ObstacleDataCollector DataCollector => _dataCollector;
        #endregion

        #region Events
        public event Action<Point, float> RobotMoved;
        public event Action<ObstacleData, Point> ObstacleCollision;
        public event Action<Point, ObstacleType, double> ObstacleDetected;
        public event Action<double> BatteryChanged;
        public event Action BatteryEmpty;
        public event Action<int> GoalReached;
        #endregion

        #region Public Methods - Control
        private CancellationTokenSource _cts;

        public void Start(IReadOnlyList<PathNode> path)
        {
            if (path == null || path.Count == 0)
                throw new ArgumentException("Path cannot be null or empty", nameof(path));

            Stop();

            _currentPath = path;
            _currentStep = 0;
            _isRunning = true;
            _isPaused = false;
            _cts = new CancellationTokenSource();
            _collisionCells.Clear();
            _scannedCells.Clear();
            _collisionCount = 0;
            _invalidMoveCount = 0;
            _totalWeightedTime = 0;
            _totalDistance = 0;

            if (_currentPath.Count > 0)
            {
                _lastRobotPosition = new Point(_currentPath[0].X, _currentPath[0].Y);
            }

            Task.Run(() => SimulationLoop(_cts.Token));
        }

        public void Stop()
        {
            _isRunning = false;
            _isPaused = false;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _currentStep = 0;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        private async Task SimulationLoop(CancellationToken token)
        {
            System.Diagnostics.Debug.WriteLine("SimulationLoop started");
            DateTime lastObstacleUpdate = DateTime.UtcNow;

            while (_isRunning && !token.IsCancellationRequested)
            {
                if (!_isPaused && !_isWaitingForDoor)
                {
                    // Update dynamic obstacles
                    DateTime now = DateTime.UtcNow;
                    double deltaTime = (now - lastObstacleUpdate).TotalSeconds;
                    lastObstacleUpdate = now;

                    if (deltaTime > 0)
                    {
                        UpdateDynamicObstacles(deltaTime);
                    }

                    await Task.Delay((int)(_stepDelaySeconds * 1000), token);

                    if (!_isRunning || token.IsCancellationRequested) break;

                    if (_currentStep >= _currentPath.Count - 1)
                    {
                        Stop();
                        System.Diagnostics.Debug.WriteLine("Simulation loop ended - path completed");
                        break;
                    }

                    var from = _currentPath[_currentStep];
                    var to = _currentPath[_currentStep + 1];

                    // Check if next cell is a closed door
                    if (IsDoorBlocked(new Point(to.X, to.Y)))
                    {
                        _isWaitingForDoor = true;
                        _waitingDoorCell = new Point(to.X, to.Y);
                        System.Diagnostics.Debug.WriteLine($"Waiting for door at ({to.X},{to.Y}) to open");

                        // Raise event to update UI status
                        _uiContext.Post(_ => RobotMoved?.Invoke(_lastRobotPosition, _lastRobotAngle), null);
                        continue;
                    }

                    float angle = (float)(Math.Atan2(to.Y - from.Y, to.X - from.X) * 180 / Math.PI);

                    _lastRobotPosition = new Point(to.X, to.Y);
                    _lastRobotAngle = angle;

                    // Check if reached a goal point
                    CheckAndRaiseGoalReached(_lastRobotPosition);

                    CheckForObstaclesInRange(_lastRobotPosition, angle);
                    CheckForCollision(_lastRobotPosition);

                    // Raise RobotMoved event on UI thread
                    var handler = RobotMoved;
                    if (handler != null)
                    {
                        var pos = _lastRobotPosition;
                        var ang = _lastRobotAngle;
                        _uiContext.Post(_ => handler(pos, ang), null);
                    }

                    _currentStep++;
                }
                // Inside the while loop, replace the waiting logic
                else if (_isWaitingForDoor)
                {
                    // Check if the door is now open
                    var cell = _mapGrid[_waitingDoorCell.X, _waitingDoorCell.Y];
                    if (cell.IsDoorOpen)
                    {
                        _isWaitingForDoor = false;
                        _waitingDoorCell = Point.Empty;
                        continue;  // Resume movement immediately
                    }
                    await Task.Delay(100, token);
                }
                else
                {
                    // Wait while paused
                    await Task.Delay(50, token);
                }
            }

            System.Diagnostics.Debug.WriteLine("Simulation loop ended");
        }

        public void MoveRobotManually(RobotCommand command, int stepSize = 1, float rotationAngle = 15f)
        {
            if (!_mapGrid.IsValidCoordinate(_lastRobotPosition.X, _lastRobotPosition.Y))
                return;

            Point newPosition = _lastRobotPosition;
            float newAngle = _lastRobotAngle;

            switch (command)
            {
                case RobotCommand.Forward:
                    newPosition = new Point(
                        _lastRobotPosition.X + (int)(stepSize * Math.Cos(_lastRobotAngle * Math.PI / 180)),
                        _lastRobotPosition.Y + (int)(stepSize * Math.Sin(_lastRobotAngle * Math.PI / 180)));
                    break;

                case RobotCommand.Backward:
                    newPosition = new Point(
                        _lastRobotPosition.X - (int)(stepSize * Math.Cos(_lastRobotAngle * Math.PI / 180)),
                        _lastRobotPosition.Y - (int)(stepSize * Math.Sin(_lastRobotAngle * Math.PI / 180)));
                    break;

                case RobotCommand.TurnLeft:
                    newAngle = _lastRobotAngle - rotationAngle;
                    break;

                case RobotCommand.TurnRight:
                    newAngle = _lastRobotAngle + rotationAngle;
                    break;

                default:
                    return;
            }

            if (_mapGrid.IsValidCoordinate(newPosition.X, newPosition.Y))
            {
                var cell = _mapGrid[newPosition.X, newPosition.Y];
                if (cell.IsWalkable && cell.OccupyingObstacle == null)
                {
                    _lastRobotPosition = newPosition;
                    _lastRobotAngle = newAngle;
                    RobotMoved?.Invoke(_lastRobotPosition, _lastRobotAngle);
                }
                else
                {
                    _invalidMoveCount++;
                }
            }
        }

        public void ClearSpecialCells()
        {
            _collisionCells.Clear();
            _scannedCells.Clear();
            _collisionCount = 0;
            _invalidMoveCount = 0;
        }
        #endregion

        #region Public Methods - Detection
        public void SetDetectionParameters(double viewAngleDegrees, int rangeCells, bool enabled)
        {
            _robotViewAngle = Math.Max(45, Math.Min(360, viewAngleDegrees));
            _detectionRangeCells = Math.Max(1, Math.Min(10, rangeCells));
            _enableDetection = enabled;
        }

        public List<Point> GetDetectionZoneCells(Point robotPos, float robotAngle)
        {
            var cells = new List<Point>();
            if (!_enableDetection) return cells;

            for (int dx = -_detectionRangeCells; dx <= _detectionRangeCells; dx++)
            {
                for (int dy = -_detectionRangeCells; dy <= _detectionRangeCells; dy++)
                {
                    int nx = robotPos.X + dx;
                    int ny = robotPos.Y + dy;

                    if (!_mapGrid.IsValidCoordinate(nx, ny)) continue;

                    double distance = Math.Sqrt(dx * dx + dy * dy);
                    if (distance > _detectionRangeCells) continue;

                    if (IsWithinFieldOfView(robotPos, robotAngle, new Point(nx, ny)))
                    {
                        cells.Add(new Point(nx, ny));
                    }
                }
            }
            return cells;
        }
        #endregion

        #region Private Methods
        private void CheckAndRaiseGoalReached(Point position)
        {
            if (_goalPositions == null) return;

            for (int i = 0; i < _goalPositions.Count; i++)
            {
                if (_goalPositions[i].X == position.X && _goalPositions[i].Y == position.Y)
                {
                    if (!_reachedGoals[i])
                    {
                        _reachedGoals[i] = true;
                        GoalReached?.Invoke(i);
                    }
                    break;
                }
            }
        }

        public void SetGoals(List<Point> goals)
        {
            _goalPositions = goals ?? new List<Point>();
            _reachedGoals = new bool[_goalPositions.Count];
        }

        private void CheckForObstaclesInRange(Point robotPos, float robotAngle)
        {
            if (!_enableDetection) return;

            List<DynamicObstacle> obstaclesCopy;
            lock (_obstacleLock)
            {
                obstaclesCopy = _dynamicObstacles.ToList();
            }

            foreach (var obstacle in obstaclesCopy)
            {
                int dx = obstacle.Location.X - robotPos.X;
                int dy = obstacle.Location.Y - robotPos.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance <= _detectionRangeCells)
                {
                    if (IsWithinFieldOfView(robotPos, robotAngle, obstacle.Location))
                    {
                        ObstacleDetected?.Invoke(obstacle.Location, obstacle.Type, distance);
                    }
                }
            }
        }

        private void CheckForCollision(Point robotPos)
        {
            DynamicObstacle obstacle = null;
            lock (_obstacleLock)
            {
                obstacle = _dynamicObstacles.FirstOrDefault(o => o.Location.X == robotPos.X && o.Location.Y == robotPos.Y);
            }

            if (obstacle != null)
            {
                _collisionCells.Add(robotPos);
                _collisionCount++;

                var obstacleData = new ObstacleData
                {
                    Type = obstacle.Type,
                    Location = obstacle.Location,
                    Timestamp = DateTime.UtcNow,
                    Distance = 0
                };

                ObstacleCollision?.Invoke(obstacleData, robotPos);
                ObstacleDetected?.Invoke(obstacle.Location, obstacle.Type, 0);
            }
        }

        private bool IsWithinFieldOfView(Point robotPos, float robotAngle, Point targetPos)
        {
            int dx = targetPos.X - robotPos.X;
            int dy = targetPos.Y - robotPos.Y;

            double targetAngle = Math.Atan2(dy, dx) * 180 / Math.PI;
            double angleDiff = Math.Abs(targetAngle - robotAngle);
            angleDiff = Math.Min(360 - angleDiff, angleDiff);

            return angleDiff <= (_robotViewAngle / 2);
        }
        #endregion

        #region Private Methods - Dynamic Obstacles
        private void UpdateDynamicObstacles(double deltaTimeSeconds)
        {
            if (deltaTimeSeconds <= 0) return;

            List<DynamicObstacle> obstaclesCopy;
            lock (_obstacleLock)
            {
                obstaclesCopy = _dynamicObstacles.ToList();
            }

            foreach (var obstacle in obstaclesCopy)
            {
                Point previousPosition = obstacle.Location;
                MoveObstacleRandomly(obstacle, deltaTimeSeconds);

                if (_collectTrainingData)
                {
                    _dataCollector.RecordMovement(obstacle, previousPosition, deltaTimeSeconds);
                }
            }
        }

        private void MoveObstacleRandomly(DynamicObstacle obstacle, double deltaTimeSeconds)
        {
            if (obstacle.Speed <= 0) return;

            double steps = obstacle.Speed * deltaTimeSeconds;
            int stepsInt = (int)Math.Floor(steps);

            if (stepsInt <= 0) return;

            for (int i = 0; i < stepsInt; i++)
            {
                int oldX = obstacle.Location.X;
                int oldY = obstacle.Location.Y;

                int dir = _random.Next(4);
                int newX = oldX;
                int newY = oldY;

                switch (dir)
                {
                    case 0: newY--; break;
                    case 1: newX++; break;
                    case 2: newY++; break;
                    case 3: newX--; break;
                }

                if (!_mapGrid.IsValidCoordinate(newX, newY))
                    continue;

                lock (_obstacleLock)
                {
                    var targetCell = _mapGrid[newX, newY];

                    if (!targetCell.IsWalkable)
                        continue;

                    if (targetCell.OccupyingObstacle != null)
                        continue;

                    var oldCell = _mapGrid[oldX, oldY];
                    oldCell.OccupyingObstacle = null;
                    oldCell.IsWalkable = true;

                    obstacle.UpdatePosition(new Point(newX, newY));

                    targetCell.OccupyingObstacle = obstacle;
                    targetCell.IsWalkable = false;
                }
            }
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (!_isDisposed)
            {
                Stop();
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private Methods - Doors
        /// <summary>
        /// Handles door state change events from DoorGroupManager
        /// </summary>
        private void OnDoorStateChanged(DoorGroup group, bool isOpen)
        {
            // Update all cells in the door group
            foreach (var cell in group.Cells)
            {
                var gridCell = _mapGrid[cell.X, cell.Y];
                gridCell.IsDoorOpen = isOpen;
                _mapGrid.UpdateCellProperties(cell.X, cell.Y);
            }

            // If door opened and robot was waiting for this door
            if (isOpen && _isWaitingForDoor)
            {
                var waitingGroup = _doorGroupManager.GetDoorGroupAt(_waitingDoorCell);

                if (waitingGroup == group)
                {
                    _isWaitingForDoor = false;
                    _waitingDoorCell = Point.Empty;

                    // Trigger UI update to resume simulation
                    _uiContext.Post(_ => RobotMoved?.Invoke(_lastRobotPosition, _lastRobotAngle), null);
                }
            }
        }

        /// <summary>
        /// Checks if a cell is a blocked door
        /// </summary>
        private bool IsDoorBlocked(Point position)
        {
            var cell = _mapGrid[position.X, position.Y];
            if (cell.ElementType != MapElementType.Door)
                return false;

            return !cell.IsDoorOpen;
        }

        /// <summary>
        /// Starts the door group manager for random door opening/closing
        /// </summary>
        public void StartDoorManager()
        {
            _doorGroupManager?.Start();
            System.Diagnostics.Debug.WriteLine("DoorGroupManager started");
        }

        /// <summary>
        /// Stops the door group manager
        /// </summary>
        public void StopDoorManager()
        {
            _doorGroupManager?.Stop();
            System.Diagnostics.Debug.WriteLine("DoorGroupManager stopped");
        }

        public int GetDoorGroupsCount()
        {
            return _doorGroupManager?.DoorGroups.Count ?? 0;
        }
        public void ReinitializeDoorGroups()
        {
            StopDoorManager();
            _doorGroupManager?.Dispose();
            _doorGroupManager = new DoorGroupManager(_mapGrid);
            _doorGroupManager.FindDoorGroups();
            _doorGroupManager.DoorStateChanged += OnDoorStateChanged;
            System.Diagnostics.Debug.WriteLine($"Reinitialized door groups: Found {_doorGroupManager.DoorGroups.Count} groups");
        }
        #endregion
    }
}
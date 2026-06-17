#region File Header
/// <summary>
/// File: SimulationService.cs
/// Description: Service for robot simulation along paths with obstacle detection
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.Core.Models.Sensors;
using SallamPathFinder4.ML.Training;
using System.Drawing; 
#endregion

namespace SallamPathFinder4.Services.Simulation
{
    public sealed class SimulationService : ISimulationService, IDisposable
    {
        #region Constants
        private const double DEFAULT_STEP_DELAY_SECONDS = 1.0;
        private const double SQRT2 = 1.4142135623730951;
        private const int DEFAULT_DETECTION_RANGE = 2;
        private const double BATTERY_CHECK_INTERVAL_SECONDS = 1.0;  // كل ثانية
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
        private double _currentSpeed = 10.0;
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
        private bool _isChargingMode;
        private Point _chargingTargetPoint;
        private List<PathNode> _originalPath;
        private int _originalPathIndex;
        private List<Point> _parkingPoints;
        private double _lastBatteryCheckTime; 
        private CancellationTokenSource _cts;
        private double _robotWidthCm = 50;      // عرض الروبوت بالسنتيمتر
        private double _robotLengthCm = 50;     // طول الروبوت بالسنتيمتر
        private double _cellSizeCm = 10.0;  // Default cell size in cm

        #endregion
        
        #region Private Fields - Multi-Sensor Detection
        private CameraHandler _cameraHandler;
        private List<DetectedObstacle> _activeDetections;
        private Dictionary<string, DateTime> _lastDetectionTime;
        private readonly object _detectionLock = new object();
        // NEW: Current robot definition for sensor access
        private RobotDefinition _currentRobot;
        #endregion
        
        #region Private Fields - Obstacle Detection System
        private ObstacleClassifier _obstacleClassifier;
        private DecisionEngine _decisionEngine;
        private LearningMemory _learningMemory;
        private ObstacleLogCollection _obstacleLog;
        private List<DetectedObstacle> _activeObstacles;
        private readonly object _obstacleDetectionLock = new object();
        private bool _learningEnabled = true;
        private bool _exportObstacleLogEnabled = true;
        private string _obstacleLogFilePath = "ObstacleLog.csv";
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

            // NEW: Initialize multi-sensor detection
            _cameraHandler = new CameraHandler();
            _activeDetections = new List<DetectedObstacle>();
            _lastDetectionTime = new Dictionary<string, DateTime>();

            // Subscribe to camera identification event
            _cameraHandler.ObstacleIdentified += OnObstacleIdentifiedByCamera;

            _uiContext = SynchronizationContext.Current ?? new SynchronizationContext();
            _doorGroupManager = new DoorGroupManager(grid);
            _doorGroupManager.FindDoorGroups();
            _doorGroupManager.DoorStateChanged += OnDoorStateChanged;
            _isWaitingForDoor = false;

            // NEW: Initialize obstacle detection system
            _obstacleClassifier = new ObstacleClassifier();
            _decisionEngine = new DecisionEngine();
            _learningMemory = new LearningMemory();
            _obstacleLog = new ObstacleLogCollection();
            _activeObstacles = new List<DetectedObstacle>();

            // Subscribe to decision engine events
            _decisionEngine.DecisionMade += OnDecisionMade;
            _decisionEngine.ReplanningNeeded += OnReplanningNeeded;
            _decisionEngine.RecordForLearning += OnRecordForLearning;

            // Subscribe to classifier events
            _obstacleClassifier.ObstacleClassified += OnObstacleClassified;

            // Load learning memory from disk
            _ = _learningMemory.LoadAsync();

            System.Diagnostics.Debug.WriteLine("[SimulationService] Obstacle detection system initialized");
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
        public bool FastModeForExperiments { get; set; } = false;

        #region Public Properties - Charging
        /// <summary>
        /// Indicates whether robot is currently in charging mode (going to/from charging station)
        /// </summary>
        public bool IsChargingMode => _isChargingMode;

        /// <summary>
        /// Gets or sets the parking points list for charging calculation
        /// </summary>
        public List<Point> ParkingPoints
        {
            get => _parkingPoints;
            set => _parkingPoints = value ?? new List<Point>();
        }
        #endregion
        #endregion

        #region Events
        public event Action<Point, float, double> RobotMoved;
        public event Action<ObstacleData, Point> ObstacleCollision;
        public event Action<Point, ObstacleType, double> ObstacleDetected;
        public event Action<double> BatteryChanged;
        public event Action BatteryEmpty;
        public event Action<int> GoalReached;
        #region Events - Charging
        /// <summary>
        /// Event raised when robot needs to go to charging station
        /// </summary>
        public event Action<Point, int> ChargingNeeded;  // Point = nearest parking, int = charging time seconds
        #endregion
        #endregion

        #region Public Methods - Control

        /// <summary>
        /// Sets the robot speed from settings
        /// </summary>
        /// <param name="speedCmPerSec">Speed in centimeters per second</param>
        public void SetRobotSpeedFromSettings(double speedCmPerSec)
        {
            _currentSpeed = Math.Max(0.1, Math.Min(100, speedCmPerSec));

            // Calculate step time based on speed
            double cellSizeCm = 10.0;  // Default cell size
            double stepTimeSeconds = cellSizeCm / _currentSpeed;

            _stepDelaySeconds = Math.Max(0.05, Math.Min(2.0, stepTimeSeconds));

            System.Diagnostics.Debug.WriteLine($"[SimulationService] Speed={_currentSpeed:F1} cm/s, StepDelay={_stepDelaySeconds:F3}s");
        }

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
                        
                        // NEW: Check all sensors for obstacles
                        await CheckAllSensorsAsync(_lastRobotPosition, _lastRobotAngle);
                    }

                    await Task.Delay((int)(_stepDelaySeconds * 1000), token);

                    if (!_isRunning || token.IsCancellationRequested) break;

                    if (_currentStep >= _currentPath.Count - 1)
                    {
                        Stop(); 
                        break;
                    }

                    var from = _currentPath[_currentStep];
                    var to = _currentPath[_currentStep + 1];

                    // Check if next cell is a closed door
                    if (IsDoorBlocked(new Point(to.X, to.Y)))
                    {
                        _isWaitingForDoor = true;
                        _waitingDoorCell = new Point(to.X, to.Y); 

                        // Raise event to update UI status
                        _uiContext.Post(_ => RobotMoved?.Invoke(_lastRobotPosition, _lastRobotAngle, _currentSpeed), null);
                        continue;
                    }

                    float angle = (float)(Math.Atan2(to.Y - from.Y, to.X - from.X) * 180 / Math.PI);

                    _lastRobotPosition = new Point(to.X, to.Y);
                    _lastRobotAngle = angle;

                    // Check if reached a goal point
                    CheckAndRaiseGoalReached(_lastRobotPosition);

                    //CheckForObstaclesInRange(_lastRobotPosition, angle);
                    CheckForCollision(_lastRobotPosition);

                    // Raise RobotMoved event on UI thread
                    var handler = RobotMoved;
                    if (handler != null)
                    {
                        var pos = _lastRobotPosition;
                        var ang = _lastRobotAngle;
                        var speed=  _currentSpeed;
                        _uiContext.Post(_ => handler(pos, ang,speed), null);
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
        }

        public void MoveRobotManually(RobotCommand command, int stepSize = 1, float rotationAngle = 15f)
        {
            if (!_mapGrid.IsValidCoordinate(_lastRobotPosition.X, _lastRobotPosition.Y))
                return;

            Point newPosition = _lastRobotPosition;
            float newAngle = _lastRobotAngle;

            // Handle different command types
            switch (command.ID)
            {
                case CommandID.Forward:
                    int steps = command.Parameters.Count > 0 ? (int)command.Parameters[0] : stepSize;
                    newPosition = new Point(
                        _lastRobotPosition.X + (int)(steps * Math.Cos(_lastRobotAngle * Math.PI / 180)),
                        _lastRobotPosition.Y + (int)(steps * Math.Sin(_lastRobotAngle * Math.PI / 180)));
                    break;

                case CommandID.Backward:
                    steps = command.Parameters.Count > 0 ? (int)command.Parameters[0] : stepSize;
                    newPosition = new Point(
                        _lastRobotPosition.X - (int)(steps * Math.Cos(_lastRobotAngle * Math.PI / 180)),
                        _lastRobotPosition.Y - (int)(steps * Math.Sin(_lastRobotAngle * Math.PI / 180)));
                    break;

                case CommandID.TurnLeftTank:
                    newAngle = _lastRobotAngle - rotationAngle;
                    break;

                case CommandID.TurnRightTank:
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
                    RobotMoved?.Invoke(_lastRobotPosition, _lastRobotAngle, _currentSpeed);
                }
                else
                {
                    _invalidMoveCount++;
                }
            }
        }


        /// <summary>
        /// Sets the robot dimensions for collision detection
        /// </summary>
        /// <param name="widthCm">Robot width in centimeters</param>
        /// <param name="lengthCm">Robot length in centimeters</param>
        public void SetRobotDimensions(double widthCm, double lengthCm)
        {
            _robotWidthCm = Math.Max(10, Math.Min(200, widthCm));
            _robotLengthCm = Math.Max(10, Math.Min(200, lengthCm));

            System.Diagnostics.Debug.WriteLine($"[SimulationService] Robot dimensions updated: W={_robotWidthCm:F1}cm, L={_robotLengthCm:F1}cm");
        }

        public void SetCurrentRobot(RobotDefinition robot)
        {
            _currentRobot = robot;
            System.Diagnostics.Debug.WriteLine($"[SimulationService] Current robot set: {_currentRobot?.RobotName}");
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
        /// <summary>
        /// Sets the detection parameters for obstacle sensing
        /// </summary>
        /// <param name="viewAngleDegrees">Field of view in degrees (45-360)</param>
        /// <param name="rangeCells">Detection range in cells (1-10)</param>
        /// <param name="enabled">Enable or disable detection</param>
        public void SetDetectionParameters(double viewAngleDegrees, int rangeCells, bool enabled)
        {
            _robotViewAngle = Math.Max(45, Math.Min(360, viewAngleDegrees));
            _detectionRangeCells = Math.Max(1, Math.Min(10, rangeCells));
            _enableDetection = enabled;

            System.Diagnostics.Debug.WriteLine($"[SimulationService] Detection params: Angle={_robotViewAngle:F0}°, Range={_detectionRangeCells} cells, Enabled={enabled}");
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

        #region Public Methods - Charging

        /// <summary>
        /// Sets the original path and current position for later resumption after charging
        /// </summary>
        public void SetOriginalPath(IReadOnlyList<PathNode> path, int currentStep)
        {
            _originalPath = path?.ToList();
            _originalPathIndex = currentStep;
        }

        /// <summary>
        /// Finds the nearest parking point to the current robot position
        /// </summary>
        public Point FindNearestParking()
        {
            if (_parkingPoints == null || _parkingPoints.Count == 0)
            {
                return Point.Empty;
            }

            Point currentPos = this.CurrentRobotPosition;
            Point nearest = _parkingPoints
                .OrderBy(p => Math.Abs(p.X - currentPos.X) + Math.Abs(p.Y - currentPos.Y))
                .FirstOrDefault();

            System.Diagnostics.Debug.WriteLine($"[SimulationService] Nearest parking: ({nearest.X},{nearest.Y}) from ({currentPos.X},{currentPos.Y})");

            return nearest;
        }

        /// <summary>
        /// Calculates Manhattan distance to nearest parking
        /// </summary>
        public int GetDistanceToNearestParking()
        {
            Point nearest = FindNearestParking();
            if (nearest == Point.Empty)
            {
                return int.MaxValue;
            }

            Point currentPos = this.CurrentRobotPosition;
            return Math.Abs(currentPos.X - nearest.X) + Math.Abs(currentPos.Y - nearest.Y);
        }

        /// <summary>
        /// Resumes the original path after charging is complete
        /// </summary>
        public void ResumeOriginalPath()
        {
            if (_originalPath != null && _originalPathIndex < _originalPath.Count)
            {
                var remainingPath = _originalPath.Skip(_originalPathIndex).ToList();
                this.Start(remainingPath);
                _isChargingMode = false;

                System.Diagnostics.Debug.WriteLine($"[SimulationService] Resumed original path from step {_originalPathIndex}, remaining {remainingPath.Count} cells");
            }
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

        //private void CheckForObstaclesInRange(Point robotPos, float robotAngle)
        //{
        //    if (!_enableDetection) return;

        //    List<DynamicObstacle> obstaclesCopy;
        //    lock (_obstacleLock)
        //    {
        //        obstaclesCopy = _dynamicObstacles.ToList();
        //    }

        //    foreach (var obstacle in obstaclesCopy)
        //    {
        //        int dx = obstacle.Location.X - robotPos.X;
        //        int dy = obstacle.Location.Y - robotPos.Y;
        //        double distance = Math.Sqrt(dx * dx + dy * dy);

        //        if (distance <= _detectionRangeCells)
        //        {
        //            if (IsWithinFieldOfView(robotPos, robotAngle, obstacle.Location))
        //            {
        //                ObstacleDetected?.Invoke(obstacle.Location, obstacle.Type, distance);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Checks for collision with obstacles using robot dimensions
        /// </summary>
        private void CheckForCollision(Point robotPos)
        {
            // Calculate how many cells the robot occupies based on its dimensions
            int cellSizeCm = 10; // Default cell size in cm
            int widthCells = Math.Max(1, (int)Math.Ceiling(_robotWidthCm / cellSizeCm));
            int lengthCells = Math.Max(1, (int)Math.Ceiling(_robotLengthCm / cellSizeCm));

            int radiusX = (widthCells - 1) / 2;
            int radiusY = (lengthCells - 1) / 2;

            // Check all cells that the robot occupies
            for (int dx = -radiusX; dx <= radiusX; dx++)
            {
                for (int dy = -radiusY; dy <= radiusY; dy++)
                {
                    int checkX = robotPos.X + dx;
                    int checkY = robotPos.Y + dy;

                    if (!_mapGrid.IsValidCoordinate(checkX, checkY))
                        continue;

                    // Check for static obstacles (walls, windows, closed doors)
                    var cell = _mapGrid[checkX, checkY];
                    if (!cell.IsWalkable)
                    {
                        _collisionCells.Add(new Point(checkX, checkY));
                        _collisionCount++;

                        var obstacleData = new ObstacleData
                        {
                            Type = ObstacleType.Equipment, // Default type for static obstacles
                            Location = new Point(checkX, checkY),
                            Timestamp = DateTime.UtcNow,
                            Distance = 0
                        };

                        ObstacleCollision?.Invoke(obstacleData, robotPos);
                        ObstacleDetected?.Invoke(new Point(checkX, checkY), ObstacleType.Equipment, 0);
                        return; // Collision detected, stop checking
                    }

                    // Check for dynamic obstacles
                    DynamicObstacle obstacle = null;
                    lock (_obstacleLock)
                    {
                        obstacle = _dynamicObstacles.FirstOrDefault(o => o.Location.X == checkX && o.Location.Y == checkY);
                    }

                    if (obstacle != null)
                    {
                        _collisionCells.Add(new Point(checkX, checkY));
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
                        return; // Collision detected, stop checking
                    }
                }
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

        /// <summary>
        /// Sets the cell size in centimeters for accurate collision detection
        /// </summary>
        /// <param name="cellSizeCm">Cell size in centimeters</param>
        public void SetCellSizeCm(double cellSizeCm)
        {
            _cellSizeCm = Math.Max(1.0, Math.Min(50.0, cellSizeCm));
            System.Diagnostics.Debug.WriteLine($"[SimulationService] Cell size set to {_cellSizeCm:F1} cm");
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
            double speedMultiplier = FastModeForExperiments ? 5.0 : 1.0;  // 5x أسرع
            double effectiveSpeed = obstacle.Speed * speedMultiplier; 
 
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

        #region Private Methods - Charging Helpers

        /// <summary>
        /// Calculates average surface weight around a path to a target
        /// Note: This is a simplified estimation. For production, calculate from actual path.
        /// </summary>
        private double EstimateAverageSurfaceWeightToTarget(Point target)
        {
            // Simplified estimation - assumes average surface weight of 50%
            // In production, you would calculate based on actual path
            return 50.0;
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
                    _uiContext.Post(_ => RobotMoved?.Invoke(_lastRobotPosition, _lastRobotAngle, _currentSpeed), null);
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

        #region Private Methods - Thread Safety
        //private void ExecuteOnUIThread(Action action)
        //{
        //    if (mainForm.InvokeRequired)
        //    {
        //        mainForm.Invoke(action);
        //    }
        //    else
        //    {
        //        action();
        //    }
        //}
        #endregion

        #region Public Methods - Multi-Sensor Detection

        /// <summary>
        /// Checks all sensors for obstacle detection
        /// Each sensor works independently within its own FOV and range
        /// </summary>
        public async Task<List<DetectedObstacle>> CheckAllSensorsAsync(Point robotPos, float robotAngle)
        {
            if (!_enableDetection || _currentRobot?.Sensors == null || _currentRobot.Sensors.Count == 0)
                return new List<DetectedObstacle>();

            var newDetections = new List<DetectedObstacle>();

            foreach (var sensor in _currentRobot.Sensors)
            {
                if (!sensor.IsEnabled) continue;

                // Perform detection with this sensor
                var detection = await DetectWithSensorAsync(sensor, robotPos, robotAngle);

                if (detection != null && detection.ObstacleDetected)
                {
                    newDetections.Add(detection.DetectedObstacle);

                    // Trigger camera for high-confidence identification
                    if (detection.TriggerCamera && detection.DetectedObstacle.ObstacleType == ObstacleType.Equipment)
                    {
                        await _cameraHandler.CaptureAndIdentifyAsync(detection.DetectedObstacle, robotPos, robotAngle);
                    }
                }
            }

            // Merge with existing detections (persistence tracking)
            lock (_detectionLock)
            {
                MergeDetections(newDetections);
            }

            // Determine behavior based on closest threat
            var closestThreat = GetClosestThreat();
            if (closestThreat != null)
            {
                var behavior = DetermineBehaviorFromObstacle(closestThreat, _currentSpeed);
                await HandleObstacleBehavior(behavior, closestThreat);
            }

            return _activeDetections.ToList();
        }

        /// <summary>
        /// Performs detection using a specific sensor with its own properties
        /// </summary>
        private async Task<DetectionResult> DetectWithSensorAsync(SimpleSensor sensor, Point robotPos, float robotAngle)
        {
            var result = new DetectionResult
            {
                SensorId = sensor.SensorId,
                SensorType = sensor.SensorType,
                Timestamp = DateTime.Now
            };

            // Calculate effective angle (robot angle + sensor mount angle)
            double effectiveAngle = robotAngle + sensor.MountAngle;
            double halfFOV = sensor.FieldOfView / 2.0;
            double startAngle = effectiveAngle - halfFOV;
            double endAngle = effectiveAngle + halfFOV;

            // Calculate sensor world position based on its position on robot
            double radAngle = robotAngle * Math.PI / 180.0;
            double sensorOffsetX = (sensor.PositionX / 100.0) * 10;  // Convert percentage to cm (assuming robot width ~10cm per cell)
            double sensorOffsetY = (sensor.PositionY / 100.0) * 10;

            double sensorWorldX = robotPos.X + sensorOffsetX * Math.Cos(radAngle) - sensorOffsetY * Math.Sin(radAngle);
            double sensorWorldY = robotPos.Y + sensorOffsetX * Math.Sin(radAngle) + sensorOffsetY * Math.Cos(radAngle);

            double maxRangeCells = sensor.MaxRange / 10.0;  // Convert cm to cells

            // Scan within sensor's range and FOV
            for (double distance = 0; distance <= maxRangeCells; distance += 0.5)
            {
                for (double angle = startAngle; angle <= endAngle; angle += sensor.FieldOfView / 10.0)
                {
                    double rad = angle * Math.PI / 180.0;
                    int checkX = (int)(sensorWorldX + distance * Math.Cos(rad));
                    int checkY = (int)(sensorWorldY + distance * Math.Sin(rad));

                    if (!_mapGrid.IsValidCoordinate(checkX, checkY)) continue;

                    var cell = _mapGrid[checkX, checkY];

                    // Check for static obstacles
                    if (!cell.IsWalkable)
                    {
                        result.ObstacleDetected = true;
                        result.Distance = distance * 10;  // Convert back to cm
                        result.Angle = angle;
                        result.ObstaclePosition = new Point(checkX, checkY);
                        result.ObstacleType = cell.ElementType.ToString();
                        result.DetectedObstacle = new DetectedObstacle(
                            sensor.SensorId, sensor.SensorType,
                            new Point(checkX, checkY), result.Distance, result.Angle);
                        result.TriggerCamera = (sensor.SensorType == "Camera");

                        return result;
                    }

                    // Check for dynamic obstacles
                    DynamicObstacle obstacle = null;
                    lock (_obstacleLock)
                    {
                        obstacle = _dynamicObstacles.FirstOrDefault(o => o.Location.X == checkX && o.Location.Y == checkY);
                    }

                    if (obstacle != null)
                    {
                        result.ObstacleDetected = true;
                        result.Distance = distance * 10;
                        result.Angle = angle;
                        result.ObstaclePosition = new Point(checkX, checkY);
                        result.ObstacleType = obstacle.Type.ToString();
                        result.DetectedObstacle = new DetectedObstacle(
                            sensor.SensorId, sensor.SensorType,
                            new Point(checkX, checkY), result.Distance, result.Angle);
                        result.DetectedObstacle.ObstacleType = obstacle.Type;
                        result.TriggerCamera = (sensor.SensorType == "Camera");

                        return result;
                    }
                }
            }

            result.ObstacleDetected = false;
            return result;
        }

        /// <summary>
        /// Merges new detections with existing ones for persistence tracking
        /// </summary>
        private void MergeDetections(List<DetectedObstacle> newDetections)
        {
            // Remove expired detections
            _activeDetections.RemoveAll(d => d.IsExpired);

            foreach (var newDetection in newDetections)
            {
                var existing = _activeDetections.FirstOrDefault(d =>
                    Math.Abs(d.Location.X - newDetection.Location.X) <= 1 &&
                    Math.Abs(d.Location.Y - newDetection.Location.Y) <= 1);

                if (existing != null)
                {
                    // Update existing detection
                    existing.Update();
                    existing.DistanceCm = newDetection.DistanceCm;
                    existing.LastDetectionTime = DateTime.UtcNow;
                }
                else
                {
                    // Add new detection
                    _activeDetections.Add(newDetection);
                }
            }
        }

        /// <summary>
        /// Gets the closest threat (obstacle that requires immediate attention)
        /// </summary>
        private DetectedObstacle GetClosestThreat()
        {
            return _activeDetections
                .Where(d => d.IsConfirmed && d.DistanceCm < 100)  // Within 1 meter
                .OrderBy(d => d.DistanceCm)
                .FirstOrDefault();
        }

        /// <summary>
        /// Determines behavior based on detected obstacle and current speed
        /// </summary>
        private AvoidanceBehavior DetermineBehaviorFromObstacle(DetectedObstacle obstacle, double currentSpeed)
        {
            double distance = obstacle.DistanceCm;

            // Distance-based decision with speed consideration
            double safeStoppingDistance = currentSpeed * 0.5;  // 0.5 seconds reaction time

            if (distance < safeStoppingDistance + 10)
                return AvoidanceBehavior.EmergencyStop;

            if (distance < safeStoppingDistance + 20)
                return AvoidanceBehavior.Stop;

            if (distance < 50)
                return AvoidanceBehavior.ReplanPermanent;

            if (distance < 80)
                return AvoidanceBehavior.ReplanTemporary;

            if (distance < 120)
                return AvoidanceBehavior.SlowDown;

            return AvoidanceBehavior.None;
        }

        /// <summary>
        /// Handles the robot behavior based on detected obstacle
        /// </summary>
        private async Task HandleObstacleBehavior(AvoidanceBehavior behavior, DetectedObstacle obstacle)
        {
            switch (behavior)
            {
                case AvoidanceBehavior.EmergencyStop:
                    System.Diagnostics.Debug.WriteLine($"[OBSTACLE] EMERGENCY STOP! {obstacle.ObstacleType} at {obstacle.DistanceCm:F0}cm");
                    Stop();
                    break;

                case AvoidanceBehavior.Stop:
                    System.Diagnostics.Debug.WriteLine($"[OBSTACLE] STOP - Waiting for {obstacle.ObstacleType} at {obstacle.DistanceCm:F0}cm");
                    Pause();
                    // Wait and check again after 1 second
                    await Task.Delay(1000);
                    Resume();
                    break;

                case AvoidanceBehavior.ReplanPermanent:
                    System.Diagnostics.Debug.WriteLine($"[OBSTACLE] PERMANENT REPLAN - {obstacle.ObstacleType} blocked path");
                    // Trigger path recalculation
                    OnReplanningNeeded?.Invoke(obstacle.Location);
                    break;

                case AvoidanceBehavior.ReplanTemporary:
                    System.Diagnostics.Debug.WriteLine($"[OBSTACLE] TEMPORARY AVOIDANCE - {obstacle.ObstacleType} at {obstacle.DistanceCm:F0}cm");
                    // Slow down and try to go around
                    _currentSpeed *= 0.5;
                    break;

                case AvoidanceBehavior.SlowDown:
                    System.Diagnostics.Debug.WriteLine($"[OBSTACLE] SLOW DOWN - {obstacle.ObstacleType} detected at {obstacle.DistanceCm:F0}cm");
                    _currentSpeed = Math.Max(5, _currentSpeed * 0.7);
                    break;

                default:
                    // Resume normal speed
                    if (_currentSpeed < _baseSpeed)
                        _currentSpeed = Math.Min(_baseSpeed, _currentSpeed * 1.1);
                    break;
            }
        }

        /// <summary>
        /// Event raised when replanning is needed
        /// </summary>
        public event Action<Point> OnReplanningNeeded;

        /// <summary>
        /// Callback when camera identifies an obstacle
        /// </summary>
        private void OnObstacleIdentifiedByCamera(DetectedObstacle obstacle, Image capturedImage)
        {
            System.Diagnostics.Debug.WriteLine($"[CAMERA] Identified: {obstacle.ObstacleType} at ({obstacle.Location.X},{obstacle.Location.Y}) with {obstacle.Confidence:P0} confidence");

            // Update the detection with identified type
            lock (_detectionLock)
            {
                var existing = _activeDetections.FirstOrDefault(d => d.DetectionId == obstacle.DetectionId);
                if (existing != null)
                {
                    existing.ObstacleType = obstacle.ObstacleType;
                    existing.Confidence = obstacle.Confidence;
                }
            }

            // Record for learning memory
            RecordObstacleForLearning(obstacle);
        }

        /// <summary>
        /// Records obstacle for SPPA-DL learning memory
        /// </summary>
        private void RecordObstacleForLearning(DetectedObstacle obstacle)
        {
            // This will be used by SPPA_DLFinder for learning
            // For now, just log
            System.Diagnostics.Debug.WriteLine($"[LEARNING] Recording obstacle at ({obstacle.Location.X},{obstacle.Location.Y}) type: {obstacle.ObstacleType}");
        }

        #endregion

        #region Public Methods - Obstacle Detection
 
        /// <summary>
        /// Exports obstacle log to file
        /// </summary>
        public async Task ExportObstacleLogAsync(string filePath = null)
        {
            string path = filePath ?? _obstacleLogFilePath;
            await _obstacleLog.ExportToCsvFileAsync(path);
            System.Diagnostics.Debug.WriteLine($"[SimulationService] Obstacle log exported to {path}");
        }

        /// <summary>
        /// Exports learning memory to file
        /// </summary>
        public async Task ExportLearningMemoryAsync(string filePath = null)
        {
            await _learningMemory.SaveAsync();
            System.Diagnostics.Debug.WriteLine($"[SimulationService] Learning memory saved");
        }

        /// <summary>
        /// Gets obstacle log statistics
        /// </summary>
        public ObstacleLogStatistics GetObstacleLogStatistics()
        {
            return _obstacleLog.GetStatistics();
        }

        /// <summary>
        /// Gets learning memory statistics
        /// </summary>
        public LearningMemory.LearningStatistics GetLearningStatistics()
        {
            return _learningMemory.GetStatistics();
        } 

        /// <summary>
        /// Gets active wait states from decision engine
        /// </summary>
        public List<ObstacleWaitState> GetActiveWaits()
        {
            return _decisionEngine.GetActiveWaits();
        }

        #endregion

        #region Private Methods - Multi-Sensor Detection

        /// <summary>
        /// Checks all sensors for obstacles and updates system state
        /// </summary>
        private async Task CheckAllSensorsForObstaclesAsync(Point robotPos, float robotAngle)
        {
            if (_currentRobot?.Sensors == null || _currentRobot.Sensors.Count == 0)
                return;

            var newDetections = new List<DetectedObstacle>();

            foreach (var sensor in _currentRobot.Sensors)
            {
                if (!sensor.IsEnabled) continue;

                // Perform detection - returns null if no obstacle, otherwise returns DetectedObstacle
                var detection = await PerformSensorDetectionAsync(sensor, robotPos, robotAngle);

                if (detection != null)  // Obstacle detected
                {
                    newDetections.Add(detection);
                }
            }

            // Update active obstacles
            lock (_obstacleDetectionLock)
            {
                MergeDetections(newDetections);
            }

            // Classify and make decisions for new obstacles
            if (newDetections.Count > 0)
            {
                await ProcessNewDetectionsAsync(newDetections, robotPos);
            }
        }

        /// <summary>
        /// Performs detection using a specific sensor with its exact FOV parameters
        /// </summary>
        private async Task<DetectedObstacle> PerformSensorDetectionAsync(SimpleSensor sensor, Point robotPos, float robotAngle)
        {
            // Calculate sensor world position based on robot position and sensor offset
            double robotRadAngle = robotAngle * Math.PI / 180.0;

            // Convert sensor position from percentage (-50..50) to cm offset
            double sensorOffsetXCm = (sensor.PositionX / 100.0) * _robotWidthCm;
            double sensorOffsetYCm = (sensor.PositionY / 100.0) * _robotLengthCm;

            // Convert cm offset to cell offset (assuming 10cm per cell)
            double sensorOffsetX = sensorOffsetXCm / 10.0;
            double sensorOffsetY = sensorOffsetYCm / 10.0;

            // Calculate absolute sensor position in world coordinates
            double sensorWorldX = robotPos.X + sensorOffsetX * Math.Cos(robotRadAngle) - sensorOffsetY * Math.Sin(robotRadAngle);
            double sensorWorldY = robotPos.Y + sensorOffsetX * Math.Sin(robotRadAngle) + sensorOffsetY * Math.Cos(robotRadAngle);

            // FOV parameters (same as used for drawing)
            double effectiveAngle = robotAngle + sensor.MountAngle;
            double halfFOV = sensor.FieldOfView / 2.0;
            double startAngle = effectiveAngle - halfFOV;
            double endAngle = effectiveAngle + halfFOV;

            // Range in cells (convert from cm to cells, assuming 10cm per cell)
            double maxRangeCells = sensor.MaxRange / 10.0;
            double stepSize = 0.3;  // Small step for accurate detection
            double angleStep = sensor.FieldOfView / 20.0;  // Good angular resolution

            // Scan within sensor's FOV
            for (double distance = 0; distance <= maxRangeCells; distance += stepSize)
            {
                for (double angle = startAngle; angle <= endAngle; angle += angleStep)
                {
                    double rad = angle * Math.PI / 180.0;
                    int checkX = (int)(sensorWorldX + distance * Math.Cos(rad));
                    int checkY = (int)(sensorWorldY + distance * Math.Sin(rad));

                    if (!_mapGrid.IsValidCoordinate(checkX, checkY)) continue;

                    var cell = _mapGrid[checkX, checkY];

                    // Check for static obstacle
                    if (!cell.IsWalkable)
                    {
                        return new DetectedObstacle(sensor.SensorId, sensor.SensorType,
                            new Point(checkX, checkY), distance * 10, angle)
                        {
                            ObstacleType = ObstacleType.Equipment,
                            Confidence = 0.9,
                            PersistenceCount = 1
                        };
                    }

                    // Check for dynamic obstacle
                    DynamicObstacle dynamicObstacle = null;
                    lock (_obstacleLock)
                    {
                        dynamicObstacle = _dynamicObstacles.FirstOrDefault(o => o.Location.X == checkX && o.Location.Y == checkY);
                    }

                    if (dynamicObstacle != null)
                    {
                        return new DetectedObstacle(sensor.SensorId, sensor.SensorType,
                            new Point(checkX, checkY), distance * 10, angle)
                        {
                            ObstacleType = dynamicObstacle.Type,
                            Confidence = 0.85,
                            PersistenceCount = 1,
                            IsMoving = true,
                            MovementSpeed = dynamicObstacle.Speed * 10
                        };
                    }
                }
            }

            return null;  // No obstacle detected
        }

        /// <summary>
        /// Processes new detections: classify and make decisions
        /// </summary>
        private async Task ProcessNewDetectionsAsync(List<DetectedObstacle> detections, Point robotPos)
        {
            var classifiedObstacles = new List<ClassificationResult>();

            foreach (var detection in detections)
            {
                // Update movement history
                _obstacleClassifier.UpdateMovementHistory(
                    $"{detection.Location.X}_{detection.Location.Y}",
                    detection.Location);

                // Classify the obstacle
                var classification = await _obstacleClassifier.ClassifyAsync(detection, robotPos, _lastRobotAngle);
                classifiedObstacles.Add(classification);

                // Update detection with classification
                detection.ObstacleType = classification.Type;
                detection.Confidence = classification.Confidence;
                detection.IsMoving = classification.IsMoving;
                detection.MovementSpeed = classification.EstimatedSpeedCmS;
                detection.MovementDirection = classification.EstimatedDirection;

                // Record in learning memory
                if (_learningEnabled)
                {
                    _learningMemory.RecordDetectionWithConfidence(
                        detection.Location.X, detection.Location.Y,
                        detection.ObstacleType, detection.Confidence);
                }

                // Add to log
                var logEntry = new ObstacleLogEntry(detection, "Detected", _currentSpeed, robotPos);
                _obstacleLog.Add(logEntry);
            }

            // Make decision based on all obstacles
            var decision = await _decisionEngine.MakeDecisionAsync(detections, _currentSpeed, robotPos);

            // Execute decision
            await ExecuteDecisionAsync(decision, detections);
        }

        /// <summary>
        /// Executes the decision made by the decision engine
        /// </summary>
        private async Task ExecuteDecisionAsync(DecisionResult decision, List<DetectedObstacle> obstacles)
        {
            switch (decision.Behavior)
            {
                case AvoidanceBehavior.EmergencyStop:
                    System.Diagnostics.Debug.WriteLine($"[OBSTACLE] Emergency stop! {decision.Reason}");
                    Stop();
                    break;

                case AvoidanceBehavior.Stop:
                    System.Diagnostics.Debug.WriteLine($"[OBSTACLE] Stopping: {decision.Reason}");
                    Pause();
                    // Wait for the specified time
                    await Task.Delay((int)(decision.WaitTimeSeconds * 1000));
                    Resume();
                    break;

                case AvoidanceBehavior.SlowDown:
                    System.Diagnostics.Debug.WriteLine($"[OBSTACLE] Slowing down: {decision.Reason}");
                    _currentSpeed = Math.Max(5, _currentSpeed * 0.5);
                    break;

                case AvoidanceBehavior.ReplanTemporary:
                case AvoidanceBehavior.ReplanPermanent:
                    System.Diagnostics.Debug.WriteLine($"[OBSTACLE] Replanning: {decision.Reason}");
                    if (obstacles.Count > 0)
                    {
                        OnReplanningNeeded?.Invoke(obstacles.First().Location);
                    }
                    break;
            }
        }
        #endregion

        #region Private Methods - Event Handlers

        private void OnDecisionMade(DecisionResult decision, DetectedObstacle obstacle)
        {
            System.Diagnostics.Debug.WriteLine($"[DecisionEngine] {decision.Behavior} - {decision.Reason}");

            // Update log entry with decision
            var logEntry = new ObstacleLogEntry(obstacle, decision.Behavior.ToString(), _currentSpeed, _lastRobotPosition);
            logEntry.WasAvoided = decision.Behavior != AvoidanceBehavior.None;
            _obstacleLog.Add(logEntry);
        }
 
        private void OnRecordForLearning(DetectedObstacle obstacle)
        {
            if (_learningEnabled)
            {
                _learningMemory.RecordDetection(obstacle.Location.X, obstacle.Location.Y, obstacle.ObstacleType);
                System.Diagnostics.Debug.WriteLine($"[LearningMemory] Recorded obstacle at ({obstacle.Location.X},{obstacle.Location.Y})");
            }
        }

        private void OnObstacleClassified(ClassificationResult result, Point location)
        {
            System.Diagnostics.Debug.WriteLine($"[ObstacleClassifier] {result.Type} at ({location.X},{location.Y}) with {result.Confidence:P0} confidence");
        }

        #endregion

        #region Public Methods - Obstacle Detection

        /// <summary>
        /// Sets whether learning is enabled
        /// </summary>
        public void SetLearningEnabled(bool enabled)
        {
            _learningEnabled = enabled;
            if (_decisionEngine != null)
                _decisionEngine.LearningEnabled = enabled;
            if (_learningMemory != null)
                _learningMemory.LearningEnabled = enabled;
            System.Diagnostics.Debug.WriteLine($"[SimulationService] Learning enabled: {enabled}");
        }

        /// <summary>
        /// Sets wait times for a specific obstacle type
        /// </summary>
        public void SetObstacleWaitTime(ObstacleType type, double waitTimeSeconds, double maxWaitTimeSeconds)
        {
            if (_decisionEngine != null)
                _decisionEngine.SetWaitTime(type, waitTimeSeconds, maxWaitTimeSeconds);
        }

        /// <summary>
        /// Sets safe and critical distances for obstacle detection
        /// </summary>
        public void SetSafetyDistances(double safeDistanceCm, double criticalDistanceCm)
        {
            if (_decisionEngine != null)
            {
                _decisionEngine.SafeDistanceCm = safeDistanceCm;
                _decisionEngine.CriticalDistanceCm = criticalDistanceCm;
            }
            System.Diagnostics.Debug.WriteLine($"[SimulationService] Safety distances: Safe={safeDistanceCm}cm, Critical={criticalDistanceCm}cm");
        } 

        /// <summary>
        /// Clears learning memory
        /// </summary>
        public void ClearLearningMemory()
        {
            if (_learningMemory != null)
                _learningMemory.Clear();
            System.Diagnostics.Debug.WriteLine("[SimulationService] Learning memory cleared");
        }

        /// <summary>
        /// Clears obstacle log
        /// </summary>
        public void ClearObstacleLog()
        {
            if (_obstacleLog != null)
                _obstacleLog.Clear();
        }



        #endregion

        #region Public Methods - Get Active Obstacles

        /// <summary>
        /// Gets the list of currently active detected obstacles
        /// </summary>
        public IReadOnlyList<DetectedObstacle> GetActiveObstacles()
        {
            lock (_obstacleDetectionLock)
            {
                return _activeObstacles?.ToList().AsReadOnly() ?? new List<DetectedObstacle>().AsReadOnly();
            }
        }

        /// <summary>
        /// Gets hotspot risk levels for all cells from learning memory
        /// </summary>
        public Dictionary<Point, double> GetHotspotRiskLevels()
        {
            var riskLevels = new Dictionary<Point, double>();

            if (_learningMemory != null)
            {
                var records = _learningMemory.GetAllRecords();
                foreach (var record in records)
                {
                    if (record.RiskLevel > 30)  // Only show cells with risk > 30%
                    {
                        riskLevels[new Point(record.X, record.Y)] = record.RiskLevel;
                    }
                }
            }

            return riskLevels;
        }

        /// <summary>
        /// Gets the current wait state (if robot is waiting for an obstacle)
        /// </summary>
        public ObstacleWaitState GetCurrentWaitState()
        {
            var waits = _decisionEngine?.GetActiveWaits();
            return waits?.FirstOrDefault();
        }

        #endregion
    }
}
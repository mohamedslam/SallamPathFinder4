#region File Header
/// <summary>
/// File: MainViewModel.cs
/// Description: Main ViewModel for the environment form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Experiments;
using SallamPathFinder4.Core.Models.Goals;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.Services.Battery;
using SallamPathFinder4.Services.Pathfinding;
using SallamPathFinder4.Services.Simulation;
using SallamPathFinder4.WinForms.Controls;
using SallamPathFinder4.WinForms.Forms;
using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentViewer;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace SallamPathFinder4.WinForms.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        #region Constants
        private const int SEARCH_TIMEOUT_MS = 30000;
        private const int SEGMENT_TIMEOUT_MS = 30000;
        #endregion

        #region Private Fields
        private readonly IPathfindingService _pathfindingService;
        private readonly ISimulationService _simulationService;
        private readonly IBatteryService _batteryService;
        private readonly IFileService _fileService;
        private readonly IExperimentService _experimentService;
        private readonly MapGrid _mapGrid;
        private readonly MapControl _mapControl;

        private bool _isSearching;
        private bool _isSimulating;
        private bool _isPaused;
        private bool _hasPath;
        private PathResult _currentPathResult;
        private ExperimentData _lastExperimentData;
        private bool _isWaitingForBatteryReplacement;
        private List<ColoredPath> _coloredSegments;
        private object _batteryLock = new object();
        private readonly frmEnvironment mainForm; 
        private List<bool> _visitedGoals;
        private List<Point> _traveledPath;
        private CancellationTokenSource _searchCts;
        #endregion

        #region Constructor
        public MainViewModel(
            IPathfindingService pathfindingService,
            ISimulationService simulationService,
            IBatteryService batteryService,
            IFileService fileService,
            IExperimentService experimentService,
            MapGrid mapGrid,
            MapControl mapControl,
            frmEnvironment form)
        {
            _pathfindingService = pathfindingService;
            _simulationService = simulationService;
            _batteryService = batteryService;
            _fileService = fileService;
            _experimentService = experimentService;
            _mapGrid = mapGrid;
            _mapControl = mapControl;

            RobotState = new ObservableRobotState();
            Goals = new ObservableCollection<GoalPoint>();
            ParkingPoints = new ObservableCollection<ParkingPoint>();
            ObstacleLog = new ObservableCollection<CollisionRecord>();

            SelectedAlgorithm = AlgorithmType.AStar;
            SelectedMetric = DistanceMetric.Manhattan;
            AllowDiagonals = true;
            HeavyDiagonals = false;
            HeuristicWeight = 2;
            SearchLimit = 10000;  // بدلاً من 50000
            mainForm = form;
            _simulationService.RobotMoved += OnRobotMoved;
            _simulationService.ObstacleCollision += OnObstacleCollision;
            _batteryService.BatteryChanged += OnBatteryChanged;
            _simulationService.BatteryEmpty += OnBatteryEmpty;
            _simulationService.GoalReached += OnGoalReached;
        }
        #endregion

        #region Public Methods - Special Cells
        /// <summary>
        /// Updates the map control with special cells (Collision, Scanned, Invalid Path)
        /// </summary>
        public void UpdateSpecialCells()
        {
            if (_simulationService is SimulationService simService)
            {
                _mapControl.CollisionCells = new HashSet<Point>(simService.CollisionCells);
                _mapControl.ScannedCells = new HashSet<Point>(simService.ScannedCells);
            }

            if (_currentPathResult != null && _currentPathResult.Path != null && _currentPathResult.Path.Count > 0)
            {
                _mapControl.StartPoint = new Point(_currentPathResult.Path[0].X, _currentPathResult.Path[0].Y);
            }

            // Collect invalid path cells from all algorithms
            var invalidCells = new HashSet<Point>();
            // This would need to be populated from the algorithm used
            // For now, we can leave it empty or collect from the last used algorithm

            _mapControl.InvalidPathCells = invalidCells;
            _mapControl.Invalidate();
        }
        #endregion

        #region Properties - Collections
        public ObservableRobotState RobotState { get; }
        public ObservableCollection<GoalPoint> Goals { get; }
        public ObservableCollection<ParkingPoint> ParkingPoints { get; }
        public ObservableCollection<CollisionRecord> ObstacleLog { get; }
        #endregion

        #region Properties - Algorithm Settings
        public AlgorithmType SelectedAlgorithm { get; set; }
        public DistanceMetric SelectedMetric { get; set; }
        public bool AllowDiagonals { get; set; }
        public bool HeavyDiagonals { get; set; }
        public int HeuristicWeight { get; set; }
        public int SearchLimit { get; set; }
        #endregion

        #region Properties - Status
        public bool IsSearching
        {
            get => _isSearching;
            set { _isSearching = value; OnPropertyChanged(); }
        }

        public bool IsSimulating
        {
            get => _isSimulating;
            set { _isSimulating = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasPath)); }
        }

        public bool IsPaused
        {
            get => _isPaused;
            set { _isPaused = value; OnPropertyChanged(); }
        }

        public bool HasPath
        {
            get => _hasPath && _currentPathResult?.Success == true;
            set { _hasPath = value; OnPropertyChanged(); }
        }

        public bool HasGoals => Goals.Count > 0;
        public PathResult CurrentPathResult => _currentPathResult;
        public ExperimentData LastExperimentData
        {
            get => _lastExperimentData;
            set { _lastExperimentData = value; OnPropertyChanged(); }
        }
        #endregion

        #region Nested Class - Pathfinding Result
        private sealed class PathfindingInternalResult
        {
            public bool Success { get; set; }
            public List<ColoredPath> Segments { get; set; }
            public List<PathNode> Path { get; set; }
            public double Time { get; set; }
            public string Error { get; set; }
        }
        #endregion

        #region Public Methods - Pathfinding
        /// <summary>
        /// Finds the optimal path through all goal points using selected algorithm
        /// </summary>
        public async Task FindPathAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== FindPathAsync STARTED ===");
            System.Diagnostics.Debug.WriteLine($"Goals count: {Goals.Count}");

            if (Goals.Count == 0)
            {
                MessageBox.Show("Please add at least one goal point first.", "No Goals");
                System.Diagnostics.Debug.WriteLine("FindPathAsync EXIT - No goals");
                return;
            }

            // Cancel any ongoing search
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();

            IsSearching = true;
            System.Diagnostics.Debug.WriteLine("IsSearching = true");

            try
            {
                Point start = RobotState.Position;
                var goalsList = Goals.ToList();

                System.Diagnostics.Debug.WriteLine($"Start position: ({start.X},{start.Y})");
                System.Diagnostics.Debug.WriteLine($"Goals list count: {goalsList.Count}");

                for (int i = 0; i < goalsList.Count; i++)
                {
                    System.Diagnostics.Debug.WriteLine($"Goal {i}: ({goalsList[i].Location.X},{goalsList[i].Location.Y})");
                }

                // Run pathfinding in background thread
                var searchTask = Task.Run(() => FindPathInternal(start, goalsList, _searchCts.Token), _searchCts.Token);

                // Wait for completion with timeout
                var completedTask = await Task.WhenAny(searchTask, Task.Delay(SEARCH_TIMEOUT_MS));

                if (completedTask != searchTask)
                {
                    _searchCts.Cancel();
                    MessageBox.Show(
                        "Pathfinding timeout. The map may be too complex or no path exists.\n\n" +
                        "Try reducing map size, adding more open space, or lowering Search Limit.",
                        "Search Timeout",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    IsSearching = false;
                    return;
                }

                var result = await searchTask;

                if (!result.Success)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: {result.Error}");
                    MessageBox.Show(result.Error, "Path Not Found");
                    IsSearching = false;
                    return;
                }

                // Apply results on UI thread
                _coloredSegments = result.Segments;
                _currentPathResult = new PathResult(result.Path, result.Time, 0);
                HasPath = true;
                _visitedGoals = new List<bool>(new bool[goalsList.Count]);
                _traveledPath = new List<Point>();

                // Draw colored paths on map
                _mapControl.DrawColoredPaths(result.Segments);

                LastExperimentData = CreateExperimentData(_currentPathResult);
                await _experimentService.LogExperimentAsync(LastExperimentData);

                System.Diagnostics.Debug.WriteLine("=== FindPathAsync COMPLETED SUCCESSFULLY ===");
                System.Diagnostics.Debug.WriteLine($"Total path length: {result.Path.Count} cells");
                System.Diagnostics.Debug.WriteLine($"Total computation time: {result.Time * 1000:F2} ms");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("FindPathAsync CANCELLED");
                MessageBox.Show("Pathfinding was cancelled.", "Search Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== FindPathAsync EXCEPTION: {ex.Message} ===");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                MessageBox.Show($"Error finding path: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsSearching = false;
                System.Diagnostics.Debug.WriteLine("FindPathAsync FINISHED");
            }
        }

        /// <summary>
        /// Internal pathfinding logic running on background thread
        /// </summary>
        private PathfindingInternalResult FindPathInternal(Point start, List<GoalPoint> goalsList, CancellationToken token)
        {
            var coloredSegments = new List<ColoredPath>();
            var fullPath = new List<PathNode>();
            Point currentPos = start;
            double totalTime = 0;

            for (int i = 0; i < goalsList.Count; i++)
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                System.Diagnostics.Debug.WriteLine($"Processing goal {i + 1}/{goalsList.Count} at ({goalsList[i].Location.X},{goalsList[i].Location.Y})");

                var finder = new AlgorithmFactory(_mapGrid).Create(SelectedAlgorithm);

                if (finder == null)
                {
                    return new PathfindingInternalResult
                    {
                        Success = false,
                        Error = $"Algorithm {SelectedAlgorithm} not available"
                    };
                }

                finder.AllowDiagonals = AllowDiagonals;
                finder.HeavyDiagonals = HeavyDiagonals;
                finder.HeuristicWeight = HeuristicWeight;
                finder.SearchLimit = SearchLimit;
                finder.Metric = SelectedMetric;

                System.Diagnostics.Debug.WriteLine($"Starting FindPath from ({currentPos.X},{currentPos.Y}) to goal {i + 1}");
                System.Diagnostics.Debug.WriteLine($"Parameters: AllowDiagonals={AllowDiagonals}, HeavyDiagonals={HeavyDiagonals}, HeuristicWeight={HeuristicWeight}, SearchLimit={SearchLimit}, Metric={SelectedMetric}");

                PathResult result = null;

                // Run each segment with a timeout using a separate thread
                var segmentTask = Task.Run(() => finder.FindPath(currentPos, goalsList[i].Location));
                bool completed = segmentTask.Wait(SEGMENT_TIMEOUT_MS);

                if (!completed)
                {
                    System.Diagnostics.Debug.WriteLine($"TIMEOUT on goal {i + 1}");
                    return new PathfindingInternalResult
                    {
                        Success = false,
                        Error = $"Timeout finding path to goal {i + 1}.\n\nTry increasing Search Limit or simplifying the map."
                    };
                }

                result = segmentTask.Result;

                System.Diagnostics.Debug.WriteLine($"FindPath result for goal {i + 1}: Success={result.Success}, PathLength={result.PathLength}, NodesExplored={result.NodesExplored}, ComputationTime={result.ComputationTimeSeconds}s");

                if (!result.Success)
                {
                    return new PathfindingInternalResult
                    {
                        Success = false,
                        Error = $"No path to goal {i + 1}\n\n{result.ErrorMessage ?? "Unknown reason"}"
                    };
                }

                // Color the path segment with goal color (transparent 180)
                Color segmentColor = Color.FromArgb(180, goalsList[i].Color);
                coloredSegments.Add(new ColoredPath(result.Path.ToList(), segmentColor, false));

                if (fullPath.Count == 0)
                    fullPath.AddRange(result.Path);
                else
                    fullPath.AddRange(result.Path.Skip(1));

                totalTime += result.ComputationTimeSeconds;
                currentPos = goalsList[i].Location;

                System.Diagnostics.Debug.WriteLine($"Goal {i + 1} completed. Total path so far: {fullPath.Count} cells");
            }

            // Return path to nearest parking (dashed green line)
            if (ParkingPoints.Count > 0 && goalsList.Count > 0)
            {
                var lastGoal = goalsList.Last().Location;
                var nearestParking = ParkingPoints
                    .OrderBy(p => Math.Abs(p.Location.X - lastGoal.X) + Math.Abs(p.Location.Y - lastGoal.Y))
                    .FirstOrDefault();

                if (nearestParking != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Finding return path to parking at ({nearestParking.Location.X},{nearestParking.Location.Y})");

                    var returnFinder = new AlgorithmFactory(_mapGrid).Create(SelectedAlgorithm);

                    if (returnFinder != null)
                    {
                        returnFinder.AllowDiagonals = AllowDiagonals;

                        var returnTask = Task.Run(() => returnFinder.FindPath(lastGoal, nearestParking.Location));
                        bool completed = returnTask.Wait(SEGMENT_TIMEOUT_MS);

                        if (completed)
                        {
                            var returnResult = returnTask.Result;
                            if (returnResult.Success && returnResult.Path.Count > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"Return path found: Length={returnResult.PathLength}");
                                Color returnPathColor = Color.FromArgb(200, Color.Green);
                                coloredSegments.Add(new ColoredPath(returnResult.Path.ToList(), returnPathColor, true));
                                fullPath.AddRange(returnResult.Path.Skip(1));
                                totalTime += returnResult.ComputationTimeSeconds;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Return path NOT found");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Return path TIMEOUT");
                        }
                    }
                }
            }

            return new PathfindingInternalResult
            {
                Success = true,
                Segments = coloredSegments,
                Path = fullPath,
                Time = totalTime,
                Error = null
            };
        }
        #endregion

        #region Public Methods - Simulation
        public void StartSimulation()
        {

            System.Diagnostics.Debug.WriteLine($"StartSimulation: _simulationService type = {_simulationService?.GetType()}");

            if (_simulationService is SimulationService simSvc)
            {
                System.Diagnostics.Debug.WriteLine($"StartSimulation: DoorGroups count before start = {simSvc.GetDoorGroupsCount()}");
                simSvc.StartDoorManager();
            }
            if (_currentPathResult?.Path == null || _currentPathResult.Path.Count == 0)
                return;

            var goalsList = Goals.Select(g => g.Location).ToList();
            _simulationService.SetGoals(goalsList);
            System.Diagnostics.Debug.WriteLine($"StartSimulation: SetGoals called with {goalsList.Count} goals"); 

            if (RobotState.BatteryLevel <= 0)
            {
                var result = MessageBox.Show(
                    "🔋 Battery is empty!\n\nDo you want to replace the battery?",
                    "Battery Empty", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    _batteryService.SetCharge(100);
                    RobotState.BatteryLevel = 100;
                }
                else
                {
                    return;
                }
            }

            _simulationService.Start(_currentPathResult.Path.ToList());
            IsSimulating = true;
            IsPaused = false;
        }

        public void PauseSimulation()
        {
            _simulationService.Pause();
            IsPaused = true;
            IsSimulating = true;
        }

        public void StopSimulation()
        {
            _simulationService.Stop();
            IsSimulating = false;
            IsPaused = false;
        }

        public void ResumeSimulation()
        {
            _simulationService.Resume();
            IsPaused = false;
            System.Diagnostics.Debug.WriteLine("Resume called - simulation continuing");
        }

        public void TogglePause()
        {
            if (IsPaused)
            {
                var goalsList = Goals.Select(g => g.Location).ToList();
                _simulationService.SetGoals(goalsList);

                _simulationService.Resume();
                IsPaused = false;
            }
            else
            {
                _simulationService.Pause();
                IsPaused = true;
            }
        }
        #endregion

        #region Public Methods - File Operations

        public async Task SaveMapAsync(string filePath)
        {
            var data = new MapData
            {
                GridWidth = _mapGrid.Width,
                GridHeight = _mapGrid.Height,
                CellSizePixels = _mapControl.CellSize,
                ScaleCmPerCell = _mapControl.ScaleCmPerCell,
                RobotPosition = RobotState.Position,
                RobotAngle = RobotState.Angle,
                BatteryLevel = RobotState.BatteryLevel
            };

            // Save surface weights
            data.SurfaceWeights = new byte[_mapGrid.Width, _mapGrid.Height];
            data.ElementTypes = new MapElementType[_mapGrid.Width, _mapGrid.Height];
            data.DoorStates = new bool[_mapGrid.Width, _mapGrid.Height];
            data.RampDifficulties = new byte[_mapGrid.Width, _mapGrid.Height];

            for (int x = 0; x < _mapGrid.Width; x++)
            {
                for (int y = 0; y < _mapGrid.Height; y++)
                {
                    data.SurfaceWeights[x, y] = _mapGrid[x, y].SurfaceWeight;
                    data.ElementTypes[x, y] = _mapGrid[x, y].ElementType;
                    data.DoorStates[x, y] = _mapGrid[x, y].IsDoorOpen;
                    data.RampDifficulties[x, y] = _mapGrid[x, y].RampDifficulty;
                }
            }

            // Save goals
            data.Goals = new List<GoalData>();
            foreach (var goal in Goals)
            {
                data.Goals.Add(new GoalData
                {
                    Number = goal.Number,
                    Location = goal.Location,
                    ColorArgb = goal.Color.ToArgb()
                });
            }

            // Save parking points
            data.ParkingPoints = new List<ParkingData>();
            foreach (var parking in ParkingPoints)
            {
                data.ParkingPoints.Add(new ParkingData
                {
                    Number = parking.Number,
                    Location = parking.Location
                });
            }

            // Save dynamic obstacles
            data.DynamicObstacles = new List<MapObstacleData>();
            foreach (var obstacle in _mapControl.DynamicObstacles)
            {
                data.DynamicObstacles.Add(new MapObstacleData
                {
                    Type = obstacle.Type,
                    Location = obstacle.Location
                });
            }

            await _fileService.SaveMapAsync(filePath, data);
        }
        public async Task LoadMapAsync(string filePath)
        {
            var mapData = await _fileService.LoadMapAsync(filePath);
            if (mapData != null)
            {
                // Resize grid
                _mapGrid.Resize(mapData.GridWidth, mapData.GridHeight);

                // Load grid data
                for (int x = 0; x < mapData.GridWidth; x++)
                {
                    for (int y = 0; y < mapData.GridHeight; y++)
                    {
                        if (mapData.SurfaceWeights != null && x < mapData.SurfaceWeights.GetLength(0) && y < mapData.SurfaceWeights.GetLength(1))
                            _mapGrid[x, y].SurfaceWeight = mapData.SurfaceWeights[x, y];
                        if (mapData.ElementTypes != null && x < mapData.ElementTypes.GetLength(0) && y < mapData.ElementTypes.GetLength(1))
                            _mapGrid[x, y].ElementType = mapData.ElementTypes[x, y];
                        if (mapData.DoorStates != null && x < mapData.DoorStates.GetLength(0) && y < mapData.DoorStates.GetLength(1))
                            _mapGrid[x, y].IsDoorOpen = mapData.DoorStates[x, y];
                        if (mapData.RampDifficulties != null && x < mapData.RampDifficulties.GetLength(0) && y < mapData.RampDifficulties.GetLength(1))
                            _mapGrid[x, y].RampDifficulty = mapData.RampDifficulties[x, y];
                    }
                }

                _mapGrid.UpdateAllCellProperties();

                // Restore robot state
                RobotState.Position = mapData.RobotPosition;
                RobotState.Angle = mapData.RobotAngle;
                RobotState.BatteryLevel = mapData.BatteryLevel;

                // ========== Restore Goals ==========
                Goals.Clear();
                if (mapData.Goals != null)
                {
                    foreach (var goalData in mapData.Goals)
                    {
                        var goal = new GoalPoint(
                            goalData.Number,
                            goalData.Location,
                            Color.FromArgb(goalData.ColorArgb)
                        );
                        Goals.Add(goal);
                        _mapGrid[goalData.Location.X, goalData.Location.Y].ElementType = MapElementType.GoalPoint;
                    }
                }

                // ========== Restore Parking Points ==========
                ParkingPoints.Clear();
                if (mapData.ParkingPoints != null)
                {
                    foreach (var parkingData in mapData.ParkingPoints)
                    {
                        var parking = new ParkingPoint(parkingData.Number, parkingData.Location);
                        ParkingPoints.Add(parking);
                        _mapGrid[parkingData.Location.X, parkingData.Location.Y].ElementType = MapElementType.ParkingPoint;
                    }
                }

                // ========== Restore Dynamic Obstacles ==========
                _mapControl.DynamicObstacles.Clear();
                if (mapData.DynamicObstacles != null)
                {
                    foreach (var obsData in mapData.DynamicObstacles)
                    {
                        var obstacle = new DynamicObstacle(obsData.Type, obsData.Location);
                        _mapControl.DynamicObstacles.Add(obstacle);
                        _mapGrid[obsData.Location.X, obsData.Location.Y].OccupyingObstacle = obstacle;
                        _mapGrid[obsData.Location.X, obsData.Location.Y].IsWalkable = false;
                    }
                }

                // Update map control
                _mapControl.CellSize = mapData.CellSizePixels;
                _mapControl.ScaleCmPerCell = mapData.ScaleCmPerCell;
                _mapControl.Goals = Goals.ToList();
                _mapControl.ParkingPoints = ParkingPoints.ToList();

                // Reset start points and add current robot position
                _mapControl.ResetStartPoints();
                _mapControl.AddStartPoint(RobotState.Position);

                _mapControl.Invalidate();

                // Notify UI
                OnPropertyChanged(nameof(HasGoals));

                System.Diagnostics.Debug.WriteLine($"Map loaded: {Goals.Count} goals, {ParkingPoints.Count} parking points, {_mapControl.DynamicObstacles.Count} dynamic obstacles");
            }
        }

        #endregion

        #region Public Methods - Test
        public async Task<List<AlgorithmTestResult>> TestAllAlgorithmsAsync(Point start, Point end)
        {
            var results = new List<AlgorithmTestResult>();
            var algorithmTypes = new[]
            {
                AlgorithmType.AStar,
                AlgorithmType.SPPA,
                AlgorithmType.SPPA_DL,
                AlgorithmType.ACO,
                AlgorithmType.DStar,
                AlgorithmType.KNN,
                AlgorithmType.BruteForce
            };

            foreach (var type in algorithmTypes)
            {
                var result = await TestAlgorithmAsync(type, start, end);
                results.Add(result);
                await Task.Delay(100);
            }

            return results;
        }

        public async Task<AlgorithmTestResult> TestAlgorithmAsync(AlgorithmType type, Point start, Point end)
        {
            var result = new AlgorithmTestResult();
            result.AlgorithmName = type.ToString();
            result.StartPosition = start;
            result.EndPosition = end;

            try
            {
                var finder = new AlgorithmFactory(_mapGrid).Create(type);
                if (finder == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Algorithm {type} not implemented";
                    return result;
                }

                finder.Metric = SelectedMetric;
                finder.AllowDiagonals = AllowDiagonals;
                finder.HeavyDiagonals = HeavyDiagonals;
                finder.HeuristicWeight = HeuristicWeight;
                finder.SearchLimit = SearchLimit;

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var pathResult = await Task.Run(() => finder.FindPath(start, end));
                stopwatch.Stop();

                result.Success = pathResult.Success;
                result.PathLength = pathResult.PathLength;
                result.ComputationTimeMs = stopwatch.Elapsed.TotalMilliseconds;
                result.NodesExplored = pathResult.NodesExplored;

                if (pathResult.Success && pathResult.Path != null)
                {
                    result.Path = pathResult.Path.Select(p => new Point(p.X, p.Y)).ToList();
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
        #endregion

        #region Public Methods - SPPA-DL
        public async Task InitializeSPPA_DLAsync(RobotSettings settings)
        {
            await Task.CompletedTask;
        }

        public List<Point> GetDetectionZoneCells()
        {
            if (_simulationService is SimulationService simService)
            {
                return simService.GetDetectionZoneCells(RobotState.Position, RobotState.Angle);
            }
            return new List<Point>();
        }

        public bool IsDynamicLearningEnabled => false;
        #endregion

        #region Public Methods - UI
        public void ShowExperimentViewer()
        {
            var viewer = new frmExperimentViewer(_experimentService);
            viewer.ShowDialog();
        }

        public void RefreshHasGoals()
        {
            OnPropertyChanged(nameof(HasGoals));
        }
        #endregion

        #region Public Methods - Battery
        public void ConsumeBattery(double distance)
        {
            double surfaceWeight = 50;
            double speed = RobotState.Speed;
            _batteryService.Consume(distance, surfaceWeight, speed);
        }
        #endregion

        #region Private Methods - Event Handlers
        private void OnRobotMoved(Point position, float angle)
        {
            RobotState.Position = position;
            RobotState.Angle = angle;
            ConsumeBattery(1.0);

            _mapControl.UpdateRobotPosition(position, angle);

            if (_traveledPath != null)
            {
                _traveledPath.Add(position);
                DrawTraveledPath();
            }
        }

        private void DrawTraveledPath()
        {
            if (_traveledPath == null || _traveledPath.Count < 2) return;

            var traveledNodes = _traveledPath.Select(p => new PathNode(p.X, p.Y)).ToList();
            var greenColor = Color.FromArgb(200, Color.Green);
            var greenPath = new ColoredPath(traveledNodes, greenColor, true);

            var allPaths = new List<ColoredPath>();

            if (_coloredSegments != null)
            {
                foreach (var segment in _coloredSegments)
                {
                    if (segment != null)
                    {
                        allPaths.Add(segment);
                    }
                }
            }

            allPaths.Add(greenPath);

            _mapControl.DrawColoredPaths(allPaths);
        }

        private void OnObstacleCollision(ObstacleData obstacle, Point position)
        {
            var record = new CollisionRecord
            {
                ObstacleType = obstacle.Type,
                Location = obstacle.Location,
                RobotPosition = position,
                Timestamp = obstacle.Timestamp,
                StrategyUsed = "Avoidance attempted",
                Success = false,
                ResolutionTime = 0
            };

            ObstacleLog.Add(record);
            _batteryService.Consume(5.0, 100, RobotState.Speed);
        }

        private void OnGoalReached(int goalIndex)
        {
            System.Diagnostics.Debug.WriteLine($"OnGoalReached called for goal {goalIndex}");

            if (goalIndex < 0 || goalIndex >= Goals.Count) return;

            if (_visitedGoals != null && !_visitedGoals[goalIndex])
            {
                _visitedGoals[goalIndex] = true;
                System.Diagnostics.Debug.WriteLine($"Goal {goalIndex} marked as visited");

                if (_traveledPath != null && _traveledPath.Count > 0)
                {
                    DrawTraveledPath();
                }
            }
        }

        private void RemovePathSegmentForGoal(int goalIndex)
        {
            if (_coloredSegments != null && goalIndex < _coloredSegments.Count)
            {
                _coloredSegments[goalIndex] = null;
                var activeSegments = _coloredSegments.Where(s => s != null).ToList();
                _mapControl.DrawColoredPaths(activeSegments);
            }
        }

        private ExperimentData CreateExperimentData(PathResult result)
        {
            return new ExperimentData
            {
                AlgorithmName = SelectedAlgorithm.ToString(),
                DistanceMetric = SelectedMetric.ToString(),
                SearchTimeMs = result.ComputationTimeSeconds * 1000,
                PathLengthCells = result.PathLength,
                GoalCount = Goals.Count,
                ParkingCount = ParkingPoints.Count,
                Success = result.Success,
                RobotSpeedCms = RobotState.Speed,
                HeuristicWeight = HeuristicWeight,
                Diagonals = AllowDiagonals
            };
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Battery Event Handlers
        private async void OnBatteryEmpty()
        {
            if (_isWaitingForBatteryReplacement) return;

            _isWaitingForBatteryReplacement = true;

            _simulationService.Pause();
            IsSimulating = false;

            await Task.Delay(100);

            var result = MessageBox.Show(
                "🔋 Battery is empty!\n\n" +
                $"Robot stopped at cell ({RobotState.Position.X}, {RobotState.Position.Y}).\n\n" +
                "Do you want to replace the battery and continue?",
                "Battery Empty",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _simulationService.Stop();

                _batteryService.SetCharge(100);
                RobotState.BatteryLevel = 100;

                await Task.Delay(100);

                if (_currentPathResult != null && _currentPathResult.Path != null)
                {
                    var currentPos = RobotState.Position;
                    var remainingPath = _currentPathResult.Path
                        .SkipWhile(p => p.X != currentPos.X || p.Y != currentPos.Y)
                        .ToList();

                    if (remainingPath.Count > 0)
                    {
                        _simulationService.Start(remainingPath);
                        IsSimulating = true;
                    }
                }
            }
            else
            {
                _simulationService.Stop();
                IsSimulating = false;
            }

            _isWaitingForBatteryReplacement = false;
        }

        private void OnBatteryChanged(double level)
        {
            RobotState.BatteryLevel = level;

            if (IsSimulating && level <= 0)
            {
                lock (_batteryLock)
                {
                    if (_isWaitingForBatteryReplacement) return;
                    _isWaitingForBatteryReplacement = true;
                }

                _simulationService.Pause();
                IsSimulating = false;

                ShowBatteryReplacementDialog();
            }
        }

        private void ShowBatteryReplacementDialog()
        {
            if (mainForm != null && mainForm.InvokeRequired)
            {
                mainForm.Invoke(new Action(ShowBatteryReplacementDialog));
                return;
            }

            var result = MessageBox.Show(
                "🔋 Battery is empty!\n\n" +
                $"Robot stopped at cell ({RobotState.Position.X}, {RobotState.Position.Y}).\n\n" +
                "Do you want to replace the battery and continue?",
                "Battery Empty",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _simulationService.Stop();
                System.Threading.Thread.Sleep(50);

                _batteryService.SetCharge(100);
                RobotState.BatteryLevel = 100;

                if (_currentPathResult != null && _currentPathResult.Path != null)
                {
                    var currentPos = RobotState.Position;
                    var remainingPath = _currentPathResult.Path
                        .SkipWhile(p => p.X != currentPos.X || p.Y != currentPos.Y)
                        .ToList();

                    if (remainingPath.Count > 0)
                    {
                        _simulationService.Start(remainingPath);
                        IsSimulating = true;
                    }
                }
            }
            else
            {
                _simulationService.Stop();
                IsSimulating = false;
            }

            lock (_batteryLock)
            {
                _isWaitingForBatteryReplacement = false;
            }
        }

        public void SetBatteryLevel(double level)
        {
            var clampedLevel = Math.Max(0, Math.Min(100, level));
            RobotState.BatteryLevel = clampedLevel;

            if (_batteryService is BatteryService batteryService)
            {
                batteryService.SetCharge(clampedLevel);
            }
        }
        #endregion
    }

    #region AlgorithmTestResult Class
    public class AlgorithmTestResult
    {
        public string AlgorithmName { get; set; }
        public Point StartPosition { get; set; }
        public Point EndPosition { get; set; }
        public bool Success { get; set; }
        public int PathLength { get; set; }
        public double ComputationTimeMs { get; set; }
        public int NodesExplored { get; set; }
        public string ErrorMessage { get; set; }
        public List<Point> Path { get; set; }

        public override string ToString()
        {
            if (Success)
            {
                return $"[OK] {AlgorithmName}: Length={PathLength}, Time={ComputationTimeMs:F2}ms";
            }
            return $"[FAIL] {AlgorithmName}: {ErrorMessage}";
        }
    }
    #endregion
}
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
        private const double BATTERY_CHECK_INTERVAL_SECONDS = 2.0;  // فحص البطارية كل 2 ثانية
        private const double AVERAGE_SURFACE_WEIGHT_ESTIMATE = 50.0;  // تقدير متوسط وزن السطح
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
            #region Private Fields - Dynamic Charging
                private System.Windows.Forms.Timer _batteryCheckTimer;
                private bool _isChargingInProgress;
                private Point _chargingParkingPoint;
                private List<PathNode> _originalFullPath;
                private int _originalPathStepWhenChargingStarted;
                private List<Point> _cachedParkingPoints;
                private DateTime _chargingStartTime;
                private bool _isWaitingForCharging;
            #endregion
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

        #region Public Properties - Dynamic Charging
        public bool IsDynamicChargingEnabled { get; private set; }
        public int ChargingTimeSeconds { get; private set; }
        public double SafetyMarginPercent { get; private set; }
        public bool IsChargingInProgress => _isChargingInProgress;

        #endregion

        #region Public Properties - Charging & Time Statistics

        /// <summary>
        /// Total charging units as decimal number (e.g., 2.3 means 2 full charges + 30% of third)
        /// </summary>
        public double TotalChargingUnits => _totalBatteryConsumed / 100.0;

        /// <summary>
        /// Total charging time in seconds
        /// </summary>
        public double TotalChargingTimeSeconds => _totalChargingTimeSeconds;

        /// <summary>
        /// Total travel time (actual movement) in seconds
        /// </summary>
        public double TotalTravelTimeSeconds => _totalTravelTimeSeconds;

        /// <summary>
        /// Total overhead time (exiting and re-entering path) in seconds
        /// </summary>
        public double TotalOverheadTimeSeconds => _totalOverheadTimeSeconds;

        /// <summary>
        /// Total time (travel + charging + overhead) in seconds
        /// </summary>
        public double TotalTimeSeconds => _totalTravelTimeSeconds + _totalChargingTimeSeconds + _totalOverheadTimeSeconds;

        /// <summary>
        /// Formatted battery and time statistics text for status bar
        /// </summary>
        public string BatteryAndTimeStatsText
        {
            get
            {
                var batteryService = new BatteryService();
                string batteryText = batteryService.FormatBatteryWithBatteries(RobotState.BatteryLevel, 3.0);
                string usageText = $"Used: {_totalBatteryConsumed:F1}% ({TotalChargingUnits:F1} charges)";
                string timeText = $"Time: {TotalTimeSeconds:F0}s";
                return $"🔋 {batteryText} | 📊 {usageText} | ⏱️ {timeText}";
            }
        }

        #endregion

        #region Public Properties - Start Point

        /// <summary>
        /// Gets the robot start point from map control
        /// </summary>
        public Point RobotStartPoint
        {
            get => _mapControl.RobotStartPoint;
            set => _mapControl.RobotStartPoint = value;
        }

        /// <summary>
        /// Indicates whether a custom start point is set
        /// </summary>
        public bool HasCustomStartPoint => _mapControl.HasCustomStartPoint;

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
                Point start = HasCustomStartPoint ? RobotStartPoint : RobotState.Position;
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

                var finder = new AlgorithmFactory(_mapGrid).Create(SelectedAlgorithm, SelectedMetric);

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

                    var returnFinder =  new AlgorithmFactory(_mapGrid).Create(SelectedAlgorithm, SelectedMetric); 

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

            // ========== NEW: Reset charging statistics at simulation start ==========
            ResetChargingStatistics();

            // ========== NEW: Record initial battery and cell size ==========
            _initialBatteryPercent = this.RobotState.BatteryLevel;
            _cellSizeCm = _mapControl.ScaleCmPerCell;

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Simulation started with {_initialBatteryPercent:F1}% battery, cell size={_cellSizeCm}cm");

            // Load charging settings and start monitoring
            LoadChargingSettings();
            UpdateParkingPointsForCharging();

            if (this.IsDynamicChargingEnabled)
            {
                StartBatteryMonitoring();
            }

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
            StopBatteryMonitoring();
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
                var finder = new AlgorithmFactory(_mapGrid).Create(SelectedAlgorithm, SelectedMetric);
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
        /// <summary>
        /// Consumes battery based on distance traveled
        /// Also updates travel time statistics
        /// </summary>
        /// <param name="distance">Distance traveled in cells</param>
        public void ConsumeBattery(double distance)
        {
            double surfaceWeight = 50;
            double speed = RobotState.Speed;

            // Store old charge before consumption
            double oldCharge = RobotState.BatteryLevel;

            // Consume battery
            _batteryService.Consume(distance, surfaceWeight, speed);

            // Calculate consumed amount
            double newCharge = RobotState.BatteryLevel;
            double consumed = oldCharge - newCharge;

            if (consumed > 0)
            {
                // Update total battery consumed
                _totalBatteryConsumed += consumed;

                // Calculate and update travel time (distance in cm / speed)
                double distanceCm = distance * _cellSizeCm;
                double timeSeconds = distanceCm / speed;
                _totalTravelTimeSeconds += timeSeconds;

                System.Diagnostics.Debug.WriteLine($"[MainViewModel] Battery consumed: {consumed:F1}%, " +
                    $"Total: {_totalBatteryConsumed:F1}%, Travel time: {timeSeconds:F2}s");

                // Update UI
                UpdateBatteryAndTimeStats();
            }
        }
        #endregion

        #region Public Methods - Reset Statistics 

        /// <summary>
        /// Resets all charging and time statistics
        /// Call this at the beginning of each new simulation
        /// </summary>
        public void ResetChargingStatistics()
        {
            _totalBatteryConsumed = 0;
            _totalChargingTimeSeconds = 0;
            _totalTravelTimeSeconds = 0;
            _totalOverheadTimeSeconds = 0;

            System.Diagnostics.Debug.WriteLine("[MainViewModel] Charging statistics reset");

            // Notify UI of property changes
            OnPropertyChanged(nameof(BatteryAndTimeStatsText));
            OnPropertyChanged(nameof(TotalChargingUnits));
            OnPropertyChanged(nameof(TotalChargingTimeSeconds));
            OnPropertyChanged(nameof(TotalTravelTimeSeconds));
            OnPropertyChanged(nameof(TotalOverheadTimeSeconds));
            OnPropertyChanged(nameof(TotalTimeSeconds));
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
        private void OnBatteryEmpty()
        {
            // إذا كان الشحن الديناميكي مفعلاً، لا تظهر رسالة استبدال البطارية
            if (this.IsDynamicChargingEnabled)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] Battery empty but dynamic charging is enabled - should auto-charge");

                // تأكد من بدء عملية الشحن إذا لم تكن قيد التقدم
                if (!_isChargingInProgress && !_isWaitingForCharging)
                {
                    InitiateChargingProcess();
                }
                return;
            }

            // الوضع اليدوي: رسالة استبدال البطارية
            if (_isWaitingForBatteryReplacement) return;

            _isWaitingForBatteryReplacement = true;

            _simulationService.Pause();
            IsSimulating = false;

            var result = MessageBox.Show(
                "🔋 Battery is empty!\n\nDo you want to replace the battery?",
                "Battery Empty",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _simulationService.Stop();
                _batteryService.SetCharge(100);
                RobotState.BatteryLevel = 100;

                if (_currentPathResult != null && _currentPathResult.Path != null)
                {
                    var remainingPath = _currentPathResult.Path
                        .SkipWhile(p => p.X != RobotState.Position.X || p.Y != RobotState.Position.Y)
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

            // إذا كان الشحن الديناميكي مفعلاً، لا تظهر رسالة استبدال البطارية
            if (this.IsDynamicChargingEnabled)
            {
                // دع نظام الشحن التلقائي يتولى الأمر
                return;
            }

            // الوضع اليدوي فقط
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

        #region Private Methods - Charging Settings

        /// <summary>
        /// Loads dynamic charging settings from robot panel
        /// </summary>
        private void LoadChargingSettings()
        {
            if (mainForm?.robotPanel != null)
            {
                this.IsDynamicChargingEnabled = mainForm.robotPanel.IsDynamicChargingEnabled;
                this.ChargingTimeSeconds = mainForm.robotPanel.ChargingTimeSeconds;
                this.SafetyMarginPercent = mainForm.robotPanel.SafetyMarginPercent;

                System.Diagnostics.Debug.WriteLine($"[MainViewModel] Charging Settings: Enabled={this.IsDynamicChargingEnabled}, " +
                    $"Time={this.ChargingTimeSeconds}s, Safety={this.SafetyMarginPercent}%");
            }
        }

        /// <summary>
        /// Updates parking points list for charging calculation
        /// </summary>
        private void UpdateParkingPointsForCharging()
        {
            _cachedParkingPoints = this.ParkingPoints?.Select(p => p.Location).ToList() ?? new List<Point>();

            if (_simulationService != null && _simulationService is SimulationService simSvc)
            {
                simSvc.ParkingPoints = _cachedParkingPoints;
            }

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Parking points updated: {_cachedParkingPoints.Count} points");

            // طباعة النقاط للتأكد
            foreach (var p in _cachedParkingPoints)
            {
                System.Diagnostics.Debug.WriteLine($"  - Parking: ({p.X},{p.Y})");
            }
        }

        #endregion

        #region Private Methods - Battery Monitoring

        /// <summary>
        /// Starts the battery monitoring timer for dynamic charging
        /// </summary>
        private void StartBatteryMonitoring()
        {
            if (_batteryCheckTimer != null)
            {
                _batteryCheckTimer.Dispose();
            }

            _batteryCheckTimer = new System.Windows.Forms.Timer();
            _batteryCheckTimer.Interval = (int)(BATTERY_CHECK_INTERVAL_SECONDS * 1000);
            _batteryCheckTimer.Tick += OnBatteryCheckTimerTick;
            _batteryCheckTimer.Start();

            System.Diagnostics.Debug.WriteLine("[MainViewModel] Battery monitoring started");
        }

        /// <summary>
        /// Stops the battery monitoring timer
        /// </summary>
        private void StopBatteryMonitoring()
        {
            if (_batteryCheckTimer != null)
            {
                _batteryCheckTimer.Stop();
                _batteryCheckTimer.Dispose();
                _batteryCheckTimer = null;
            }

            System.Diagnostics.Debug.WriteLine("[MainViewModel] Battery monitoring stopped");
        }

        /// <summary>
        /// Timer tick event - checks if battery needs charging
        /// </summary>
        private void OnBatteryCheckTimerTick(object sender, EventArgs e)
        {
            // Only check if:
            // 1. Dynamic charging is enabled
            // 2. Simulation is running
            // 3. Not already in charging mode
            // 4. Not waiting for charging to complete
            // 5. Has a valid path
            // 6. Has parking points available

            if (!this.IsDynamicChargingEnabled)
            {
                return;
            }

            if (!this.IsSimulating)
            {
                return;
            }

            if (_isChargingInProgress)
            {
                return;
            }

            if (_isWaitingForCharging)
            {
                return;
            }

            if (_currentPathResult?.Path == null || _currentPathResult.Path.Count == 0)
            {
                return;
            }

            if (_cachedParkingPoints == null || _cachedParkingPoints.Count == 0)
            {
                return;
            }

            CheckBatteryAndChargeIfNeeded();
        }

        #endregion

        #region Private Methods - Charging Decision

        /// <summary>
        /// Checks if battery is sufficient to continue or needs charging
        /// </summary>
        private void CheckBatteryAndChargeIfNeeded()
        {
            System.Diagnostics.Debug.WriteLine($"[MainViewModel] CheckBatteryAndChargeIfNeeded - " +
                $"Current={this.RobotState.BatteryLevel}%, " +
                $"InProgress={_isChargingInProgress}, Waiting={_isWaitingForCharging}");

            // Skip if charging already in progress
            if (_isChargingInProgress || _isWaitingForCharging)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] SKIP - charging already in progress");
                return;
            }

            // Skip if dynamic charging is not enabled
            if (!this.IsDynamicChargingEnabled)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] SKIP - dynamic charging disabled");
                return;
            }

            // Skip if simulation is not running
            if (!this.IsSimulating)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] SKIP - simulation not running");
                return;
            }

            // Skip if no parking points
            if (_cachedParkingPoints == null || _cachedParkingPoints.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] SKIP - no parking points");
                return;
            }

            double currentBattery = this.RobotState.BatteryLevel;

            // Calculate distance to nearest parking
            int distanceToParking = GetDistanceToNearestParking();

            // SPECIAL CASE: If robot is already on a parking point
            if (distanceToParking == 0)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] Robot is already on a parking point");

                // Only charge if battery is critically low (below 20%)
                if (currentBattery <= 20.0)
                {
                    System.Diagnostics.Debug.WriteLine($"[MainViewModel] Battery low ({currentBattery}%) while on parking - charging");
                    InitiateChargingProcess();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[MainViewModel] Battery OK ({currentBattery}%) - no need to charge");
                }
                return;
            }

            if (distanceToParking == int.MaxValue)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] No parking points available for charging");
                return;
            }

            // Calculate battery needed to reach parking
            double neededBattery = _batteryService.CalculateBatteryNeededForDistance(
                distanceToParking,
                AVERAGE_SURFACE_WEIGHT_ESTIMATE,
                this.RobotState.Speed);

            double requiredBattery = neededBattery + this.SafetyMarginPercent;

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Battery Check: Current={currentBattery:F1}%, " +
                $"Needed={neededBattery:F1}%, Required={requiredBattery:F1}%, " +
                $"Distance={distanceToParking} cells");

            // Check if battery is too low to continue safely
            if (currentBattery <= requiredBattery)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] Battery LOW - initiating charging process");
                InitiateChargingProcess();
            }
        }

        /// <summary>
        /// Gets Manhattan distance to nearest parking point
        /// </summary>
        private int GetDistanceToNearestParking()
        {
            if (_cachedParkingPoints == null || _cachedParkingPoints.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] GetDistanceToNearestParking: No parking points available");
                return int.MaxValue;
            }

            Point currentPos = this.RobotState.Position;

            int minDistance = int.MaxValue;
            Point nearestParking = Point.Empty;

            foreach (var parking in _cachedParkingPoints)
            {
                int distance = Math.Abs(currentPos.X - parking.X) + Math.Abs(currentPos.Y - parking.Y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestParking = parking;
                }
            }

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] GetDistanceToNearestParking: Current=({currentPos.X},{currentPos.Y}), " +
                $"Nearest=({nearestParking.X},{nearestParking.Y}), Distance={minDistance}");

            return minDistance;
        }

        /// <summary>
        /// Gets the nearest parking point coordinates
        /// </summary>
        private Point GetNearestParkingPoint()
        {
            Point currentPos = this.RobotState.Position;
            return _cachedParkingPoints
                .OrderBy(p => Math.Abs(p.X - currentPos.X) + Math.Abs(p.Y - currentPos.Y))
                .FirstOrDefault();
        }

        #endregion

        #region Private Methods - Charging Process

        /// <summary>
        /// Initiates the charging process: robot goes to parking, charges, then returns
        /// </summary>
        private async void InitiateChargingProcess()
        {
            System.Diagnostics.Debug.WriteLine($"[MainViewModel] InitiateChargingProcess - START");

            if (_isChargingInProgress || _isWaitingForCharging)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] InitiateChargingProcess - already in progress, EXIT");
                return;
            }

            _isChargingInProgress = true;
            _chargingParkingPoint = GetNearestParkingPoint();

            if (_chargingParkingPoint == Point.Empty)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] No parking point found for charging");
                _isChargingInProgress = false;
                return;
            }

            // Save current path progress to resume later
            if (_currentPathResult != null && _currentPathResult.Path != null)
            {
                _originalFullPath = _currentPathResult.Path.ToList();
                _originalPathStepWhenChargingStarted = GetCurrentPathIndex();

                System.Diagnostics.Debug.WriteLine($"[MainViewModel] Original path saved: {_originalFullPath.Count} cells, current index: {_originalPathStepWhenChargingStarted}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] WARNING: No original path to save!");
                _isChargingInProgress = false;
                return;
            }

            // ========== NEW: Calculate overhead time for exiting to parking ==========
            Point currentPos = this.RobotState.Position;
            int distanceToParking = Math.Abs(currentPos.X - _chargingParkingPoint.X) +
                                    Math.Abs(currentPos.Y - _chargingParkingPoint.Y);
            double overheadTimeToParking = (distanceToParking * _cellSizeCm) / this.RobotState.Speed;
            _totalOverheadTimeSeconds += overheadTimeToParking;

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Overhead time to parking: {overheadTimeToParking:F2}s (distance={distanceToParking} cells)");

            UpdateBatteryAndTimeStats();

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] INITIATING CHARGING PROCESS");
            System.Diagnostics.Debug.WriteLine($"  - Parking Point: ({_chargingParkingPoint.X},{_chargingParkingPoint.Y})");
            System.Diagnostics.Debug.WriteLine($"  - Charging Time: {ChargingTimeSeconds} seconds");
            System.Diagnostics.Debug.WriteLine($"  - Current Position: ({this.RobotState.Position.X},{this.RobotState.Position.Y})");

            // Pause current simulation
            _simulationService.Pause();

            // Create charging path from current position to parking
            var chargingPath = await CreateChargingPathAsync(this.RobotState.Position, _chargingParkingPoint);

            if (chargingPath == null || chargingPath.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] Failed to create charging path");
                _isChargingInProgress = false;
                _simulationService.Resume();
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Charging path created: {chargingPath.Count} cells");

            // Draw charging path in LightBlue
            var coloredChargingPath = new ColoredPath(chargingPath, Color.LightBlue, PathType.Charging, 4, false);
            var allPaths = new List<ColoredPath> { coloredChargingPath };

            // Also show original return path (green, thinner)
            if (_originalFullPath != null && _originalFullPath.Count > 0)
            {
                var returnPath = CreateReturnPathToParking();
                if (returnPath != null && returnPath.Count > 0)
                {
                    var greenPath = new ColoredPath(returnPath, Color.Green, PathType.Return, 2, false);
                    allPaths.Add(greenPath);
                }
            }

            _mapControl.DrawColoredPaths(allPaths);

            // Start simulation on charging path
            _simulationService.Start(chargingPath);
            _isWaitingForCharging = true;

            // Update UI
            ExecuteOnUIThread(() =>
            {
                if (mainForm?.lblStatus != null)
                {
                    mainForm.lblStatus.Text = $"🔋 Battery low! Going to charging station at ({_chargingParkingPoint.X},{_chargingParkingPoint.Y})";
                }
            });

            // Wait for robot to reach parking
            await WaitForRobotToReachParking();
        }
        /// <summary>
        /// Gets the current index in the original path
        /// </summary>
        private int GetCurrentPathIndex()
        {
            if (_originalFullPath == null)
            {
                return 0;
            }

            Point currentPos = this.RobotState.Position;

            for (int i = 0; i < _originalFullPath.Count; i++)
            {
                if (_originalFullPath[i].X == currentPos.X && _originalFullPath[i].Y == currentPos.Y)
                {
                    return i;
                }
            }

            // If not found, return closest index
            int closestIndex = 0;
            double minDistance = double.MaxValue;

            for (int i = 0; i < _originalFullPath.Count; i++)
            {
                double distance = Math.Abs(_originalFullPath[i].X - currentPos.X) +
                                  Math.Abs(_originalFullPath[i].Y - currentPos.Y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Current position ({currentPos.X},{currentPos.Y}) not found in path, using closest index {closestIndex}");

            return closestIndex;
        }
        /// <summary>
        /// Creates a path from current position to parking point
        /// </summary>
        private async Task<List<PathNode>> CreateChargingPathAsync(Point from, Point to)
        {
            var finder = new AlgorithmFactory(_mapGrid).Create(SelectedAlgorithm, SelectedMetric);

            if (finder == null)
            {
                return null;
            }

            finder.AllowDiagonals = true;
            finder.Metric = DistanceMetric.Euclidean;

            var result = await Task.Run(() => finder.FindPath(from, to));

            if (result.Success && result.Path != null)
            {
                System.Diagnostics.Debug.WriteLine($"[MainViewModel] Charging path created: {result.Path.Count} cells");
                return result.Path.ToList();
            }

            return null;
        }

        /// <summary>
        /// Creates the return path from last goal to parking (green, thinner)
        /// </summary>
        private List<PathNode> CreateReturnPathToParking()
        {
            if (_originalFullPath == null || _originalFullPath.Count == 0)
            {
                return null;
            }

            // Find the last point before charging started that is near a goal
            // Simplified: use current position as start for return path display
            Point currentPos = this.RobotState.Position;
            Point parkingPoint = _chargingParkingPoint;

            // Create a direct Manhattan path for display
            var path = new List<PathNode>();
            path.Add(new PathNode(currentPos.X, currentPos.Y));

            // Simple straight line approximation for display
            int dx = Math.Sign(parkingPoint.X - currentPos.X);
            int dy = Math.Sign(parkingPoint.Y - currentPos.Y);

            int x = currentPos.X;
            int y = currentPos.Y;

            while (x != parkingPoint.X || y != parkingPoint.Y)
            {
                if (x != parkingPoint.X) x += dx;
                if (y != parkingPoint.Y) y += dy;
                path.Add(new PathNode(x, y));
            }

            return path;
        }

        /// <summary>
        /// Waits for robot to reach the parking point
        /// </summary>
        private async Task WaitForRobotToReachParking()
        {
            System.Diagnostics.Debug.WriteLine("[MainViewModel] Waiting for robot to reach parking point");

            while (_isWaitingForCharging && _simulationService.IsRunning)
            {
                if (this.RobotState.Position.X == _chargingParkingPoint.X &&
                    this.RobotState.Position.Y == _chargingParkingPoint.Y)
                {
                    System.Diagnostics.Debug.WriteLine("[MainViewModel] Robot reached parking point - starting charging");
                    await StartCharging();
                    return;
                }

                await Task.Delay(500);
            }
        }
        #endregion

        #region Private Methods - Charging Execution

        /// <summary>
        /// Starts the charging process at parking point
        /// </summary>
        private async Task StartCharging()
        {
            _simulationService.Pause();
            _isWaitingForCharging = false;

            int totalSeconds = this.ChargingTimeSeconds;

            // ========== NEW: Update charging statistics ==========
            _totalChargingTimeSeconds += totalSeconds;

            // Calculate and add charged amount to total consumed
            double chargedAmount = 100 - this.RobotState.BatteryLevel;
            if (chargedAmount > 0)
            {
                _totalBatteryConsumed += chargedAmount;
            }

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Charging: +{chargedAmount:F1}%, " +
                $"Total consumed: {_totalBatteryConsumed:F1}%, Charging time: {totalSeconds}s");

            UpdateBatteryAndTimeStats();

            // Start countdown timer on UI
            ExecuteOnUIThread(() =>
            {
                if (mainForm?.robotPanel != null)
                {
                    mainForm.robotPanel.StartChargingCountdown(totalSeconds);
                }
                if (mainForm?.lblStatus != null)
                {
                    mainForm.lblStatus.Text = $"🔋 Charging... {totalSeconds} seconds remaining";
                }
            });

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Charging started for {totalSeconds} seconds");

            // Wait for charging time
            await Task.Delay(totalSeconds * 1000);

            // Stop countdown timer
            ExecuteOnUIThread(() =>
            {
                if (mainForm?.robotPanel != null)
                {
                    mainForm.robotPanel.StopChargingCountdown();
                }
                if (mainForm?.lblStatus != null)
                {
                    mainForm.lblStatus.Text = $"✅ Charging complete! Resuming path...";
                }
            });

            // Set battery to full
            _batteryService.SetFullCharge();
            this.RobotState.BatteryLevel = 100;

            System.Diagnostics.Debug.WriteLine("[MainViewModel] Charging complete, battery at 100%");

            UpdateBatteryAndTimeStats();

            // Resume original path
            await ResumeOriginalPathAfterCharging();
        }

        #region Private Methods - Resume After Charging

        /// <summary>
        /// Resumes the original path after charging is complete
        /// </summary>
        private async Task ResumeOriginalPathAfterCharging()
        {
            System.Diagnostics.Debug.WriteLine("[MainViewModel] ResumeOriginalPathAfterCharging - START");

            _isChargingInProgress = false;
            _isWaitingForCharging = false;

            if (_originalFullPath == null || _originalPathStepWhenChargingStarted >= _originalFullPath.Count)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] No original path to resume - simulation may be complete");
                return;
            }

            // Get the point where robot should resume
            Point resumePoint = new Point(
                _originalFullPath[_originalPathStepWhenChargingStarted].X,
                _originalFullPath[_originalPathStepWhenChargingStarted].Y
            );

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Resuming from point ({resumePoint.X},{resumePoint.Y})");

            // ========== NEW: Calculate overhead time for returning to path ==========
            int distanceToResume = Math.Abs(_chargingParkingPoint.X - resumePoint.X) +
                                   Math.Abs(_chargingParkingPoint.Y - resumePoint.Y);
            double overheadTimeToResume = (distanceToResume * _cellSizeCm) / this.RobotState.Speed;
            _totalOverheadTimeSeconds += overheadTimeToResume;

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Overhead time to resume: {overheadTimeToResume:F2}s (distance={distanceToResume} cells)");

            UpdateBatteryAndTimeStats();

            // Create return path from parking to resume point
            var returnPath = await CreateChargingPathAsync(_chargingParkingPoint, resumePoint);

            if (returnPath == null || returnPath.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] Failed to create return path");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Return path created: {returnPath.Count} cells");

            // Draw return path in LightBlue
            var coloredReturnPath = new ColoredPath(returnPath, Color.LightBlue, PathType.Charging, 4, false);
            _mapControl.DrawColoredPaths(new List<ColoredPath> { coloredReturnPath });

            // Start simulation on return path
            _simulationService.Stop();
            _simulationService.Start(returnPath);

            // Wait for robot to reach resume point
            await WaitForRobotToReachResumePoint(resumePoint);
        }
        #endregion
        #region Private Methods - Find Optimal Resume Point (Fully Optimized)

        /// <summary>
        /// Finds the optimal resume point on the original path after charging
        /// Prioritizes unvisited goals and prevents skipping them
        /// </summary>
        private async Task<Point> FindOptimalResumePointOptimized(Point currentChargingPosition, List<PathNode> originalPath, int originalIndex, Point nextGoal)
        {
            System.Diagnostics.Debug.WriteLine($"[MainViewModel] FindOptimalResumePointOptimized - START");
            System.Diagnostics.Debug.WriteLine($"  - Charging Position: ({currentChargingPosition.X},{currentChargingPosition.Y})");
            System.Diagnostics.Debug.WriteLine($"  - Original Index: {originalIndex}");
            System.Diagnostics.Debug.WriteLine($"  - Next Goal: ({nextGoal.X},{nextGoal.Y})");

            // البحث في نطاق ±10 حول نقطة التوقف، مع تفضيل التقدم للأمام
            int searchBackward = 5;   // يمكن التراجع قليلاً
            int searchForward = 15;    // نفضل التقدم للأمام
            int startIndex = Math.Max(0, originalIndex - searchBackward);
            int endIndex = Math.Min(originalPath.Count - 1, originalIndex + searchForward);

            System.Diagnostics.Debug.WriteLine($"  - Search range: {startIndex} to {endIndex} ({endIndex - startIndex + 1} candidates)");

            var candidates = new List<CandidateInfo>();

            // ========== تجميع المرشحين مع حساب الدرجات ==========
            for (int i = startIndex; i <= endIndex; i++)
            {
                Point candidatePoint = new Point(originalPath[i].X, originalPath[i].Y);

                // التحقق الأساسي من صحة الخلية
                if (!_mapGrid.IsValidCoordinate(candidatePoint.X, candidatePoint.Y))
                    continue;

                if (!_mapGrid[candidatePoint.X, candidatePoint.Y].IsWalkable)
                    continue;

                // التحقق من العقبات
                var cell = _mapGrid[candidatePoint.X, candidatePoint.Y];
                bool isDangerous = (cell.ElementType == MapElementType.Door && !cell.IsDoorOpen) ||
                                   (cell.OccupyingObstacle != null);

                if (isDangerous)
                    continue;

                // ========== التحقق من وجود هدف غير مزور ==========
                bool isUnvisitedGoal = false;
                GoalPoint unvisitedGoal = null;

                foreach (var goal in this.Goals)
                {
                    if (!goal.IsVisited && goal.Location.X == candidatePoint.X && goal.Location.Y == candidatePoint.Y)
                    {
                        isUnvisitedGoal = true;
                        unvisitedGoal = goal;
                        break;
                    }
                }

                // ========== حساب المعايير ==========

                // 1. المسافة إلى نقطة الشحن
                int distanceFromCharger = Math.Abs(candidatePoint.X - currentChargingPosition.X) +
                                          Math.Abs(candidatePoint.Y - currentChargingPosition.Y);

                // 2. التقدم نحو الهدف (نفضل التقدم ونتجنب التراجع)
                int progressForward = i - originalIndex;

                // 3. المسافة إلى الهدف التالي
                double distanceToNextGoal = (nextGoal != Point.Empty)
                    ? Math.Abs(candidatePoint.X - nextGoal.X) + Math.Abs(candidatePoint.Y - nextGoal.Y)
                    : 0;

                // 4. وزن الخلية
                double cellCost = cell.SurfaceWeight / 100.0;

                // 5. درجة الخطورة (الخلايا القريبة من العقبات)
                double dangerScore = 0;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = candidatePoint.X + dx;
                        int ny = candidatePoint.Y + dy;
                        if (_mapGrid.IsValidCoordinate(nx, ny) && _mapGrid[nx, ny].OccupyingObstacle != null)
                        {
                            dangerScore += 0.2;
                        }
                    }
                }

                // ========== حساب الدرجة النهائية ==========
                double score = 0;

                // وزن المسافة من الشاحن (15%)
                score += distanceFromCharger * 0.15;

                // وزن التقدم للأمام (25%)
                if (progressForward > 0)
                {
                    score -= progressForward * 3.0;  // مكافأة للتقدم
                }
                else if (progressForward < 0)
                {
                    score += Math.Abs(progressForward) * 8.0;  // عقوبة كبيرة للتراجع
                }

                // وزن المسافة إلى الهدف التالي (20%)
                score += distanceToNextGoal * 0.20;

                // وزن تكلفة الخلية (10%)
                score += cellCost * 10.0;

                // وزن درجة الخطورة (10%)
                score += dangerScore * 10.0;

                // ========== أفضلية خاصة للأهداف غير المزورة ==========
                if (isUnvisitedGoal)
                {
                    // أفضلية كبيرة جداً للأهداف غير المزورة
                    score -= 200;
                    System.Diagnostics.Debug.WriteLine($"  - Candidate ({candidatePoint.X},{candidatePoint.Y}) is UNVISITED GOAL! Score bonus applied. Final score: {score:F2}");
                }

                System.Diagnostics.Debug.WriteLine($"  - Candidate ({candidatePoint.X},{candidatePoint.Y}): " +
                    $"Dist={distanceFromCharger}, Progress={progressForward}, " +
                    $"ToGoal={distanceToNextGoal:F0}, CellCost={cellCost:F2}, " +
                    $"Danger={dangerScore:F2}, IsGoal={isUnvisitedGoal}, Score={score:F2}");

                candidates.Add(new CandidateInfo
                {
                    Index = i,
                    Point = candidatePoint,
                    Score = score,
                    DistanceFromCharger = distanceFromCharger,
                    ProgressForward = progressForward,
                    DistanceToNextGoal = distanceToNextGoal,
                    IsUnvisitedGoal = isUnvisitedGoal,
                    GoalNumber = unvisitedGoal?.Number ?? 0
                });
            }

            if (candidates.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[MainViewModel] No candidates found!");
                return Point.Empty;
            }

            // ترتيب حسب الدرجة (الأقل أولاً)
            candidates = candidates.OrderBy(c => c.Score).ToList();

            // طباعة أفضل 5 مرشحين
            System.Diagnostics.Debug.WriteLine("  --- Top 5 Candidates ---");
            for (int j = 0; j < Math.Min(5, candidates.Count); j++)
            {
                var c = candidates[j];
                string goalMarker = c.IsUnvisitedGoal ? " [UNVISITED GOAL!]" : "";
                System.Diagnostics.Debug.WriteLine($"    #{j + 1}: ({c.Point.X},{c.Point.Y}) - Score={c.Score:F2}, " +
                    $"Progress={c.ProgressForward}, ToGoal={c.DistanceToNextGoal:F0}{goalMarker}");
            }

            // ========== البحث عن مسار صالح ==========
            // نبدأ بأفضل المرشحين (حتى 5 مرشحين)
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                for (int j = 0; j < Math.Min(5, candidates.Count); j++)
                {
                    var candidate = candidates[j];

                    if (cts.Token.IsCancellationRequested)
                        break;

                    try
                    {
                        var pathTask = CreateChargingPathAsync(currentChargingPosition, candidate.Point);
                        var completedTask = await Task.WhenAny(pathTask, Task.Delay(1500, cts.Token));

                        if (completedTask != pathTask)
                        {
                            System.Diagnostics.Debug.WriteLine($"  - Candidate ({candidate.Point.X},{candidate.Point.Y}): Timeout");
                            continue;
                        }

                        var tempPath = await pathTask;

                        if (tempPath != null && tempPath.Count > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"  - Candidate ({candidate.Point.X},{candidate.Point.Y}): ✓ VALID (path={tempPath.Count} cells)");

                            // إذا كان المرشح هو هدف غير مزور، سجل ذلك
                            if (candidate.IsUnvisitedGoal)
                            {
                                System.Diagnostics.Debug.WriteLine($"  - *** This candidate is an UNVISITED GOAL (Goal {candidate.GoalNumber}) - will be visited after charging ***");
                            }

                            return candidate.Point;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"  - Candidate ({candidate.Point.X},{candidate.Point.Y}): No valid path");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - Candidate ({candidate.Point.X},{candidate.Point.Y}): Error - {ex.Message}");
                    }
                }
            }

            // إذا فشل الجميع، أرجع أفضل مرشح من حيث الدرجة فقط
            var bestCandidate = candidates.FirstOrDefault();
            if (bestCandidate != null)
            {
                System.Diagnostics.Debug.WriteLine($"  - Returning best candidate by score: ({bestCandidate.Point.X},{bestCandidate.Point.Y})");
                return bestCandidate.Point;
            }

            System.Diagnostics.Debug.WriteLine("[MainViewModel] No valid candidate found");
            return Point.Empty;
        }

        /// <summary>
        /// Helper class for candidate information (extended)
        /// </summary>
        private class CandidateInfo
        {
            public int Index { get; set; }
            public Point Point { get; set; }
            public double Score { get; set; }
            public int DistanceFromCharger { get; set; }
            public int ProgressForward { get; set; }
            public double DistanceToNextGoal { get; set; }
            public bool IsUnvisitedGoal { get; set; }
            public int GoalNumber { get; set; }
        }

        #endregion
        private void RestartBatteryMonitoring()
        {
            // Stop existing timer
            if (_batteryCheckTimer != null)
            {
                _batteryCheckTimer.Stop();
                _batteryCheckTimer.Dispose();
                _batteryCheckTimer = null;
            }

            // Create and start new timer
            _batteryCheckTimer = new System.Windows.Forms. Timer();
            _batteryCheckTimer.Interval = (int)(BATTERY_CHECK_INTERVAL_SECONDS * 1000);
            _batteryCheckTimer.Tick += OnBatteryCheckTimerTick;
            _batteryCheckTimer.Start();

            System.Diagnostics.Debug.WriteLine("[MainViewModel] Battery monitoring RESTARTED");
        }
        #region Private Methods - Charging Resume

        /// <summary>
        /// Waits for robot to reach the resume point on the original path after charging
        /// </summary>
        /// <param name="resumePoint">The point where robot should resume original path</param>
        private async Task WaitForRobotToReachResumePoint(Point resumePoint)
        {
            System.Diagnostics.Debug.WriteLine($"[MainViewModel] WAITING for resume point ({resumePoint.X},{resumePoint.Y})");
            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Original path has {_originalFullPath?.Count ?? 0} cells, current index = {_originalPathStepWhenChargingStarted}");

            while (_simulationService.IsRunning)
            {
                Point currentPos = this.RobotState.Position;

                if (currentPos.X == resumePoint.X && currentPos.Y == resumePoint.Y)
                {
                    System.Diagnostics.Debug.WriteLine("[MainViewModel] REACHED resume point!");

                    // Get remaining path from original path
                    if (_originalFullPath == null || _originalPathStepWhenChargingStarted >= _originalFullPath.Count)
                    {
                        System.Diagnostics.Debug.WriteLine("[MainViewModel] No remaining path - simulation complete");
                        _isChargingInProgress = false;
                        _isWaitingForCharging = false;

                        ExecuteOnUIThread(() =>
                        {
                            if (mainForm?.lblStatus != null)
                            {
                                mainForm.lblStatus.Text = "✅ All goals completed! Simulation finished.";
                            }
                        });
                        return;
                    }

                    var remainingPath = _originalFullPath.Skip(_originalPathStepWhenChargingStarted).ToList();

                    if (remainingPath == null || remainingPath.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("[MainViewModel] Remaining path is empty - simulation complete");
                        _isChargingInProgress = false;
                        _isWaitingForCharging = false;

                        ExecuteOnUIThread(() =>
                        {
                            if (mainForm?.lblStatus != null)
                            {
                                mainForm.lblStatus.Text = "✅ All goals completed! Simulation finished.";
                            }
                        });
                        return;
                    }

                    // Stop current simulation
                    _simulationService.Stop();

                    // Start remaining path
                    _simulationService.Start(remainingPath);

                    // Redraw original path in gold
                    _mapControl.DrawPath(_originalFullPath, Color.Gold);

                    System.Diagnostics.Debug.WriteLine($"[MainViewModel] Remaining path resumed: {remainingPath.Count} cells");

                    // ========== إعادة تعيين المتغيرات ==========
                    _isChargingInProgress = false;
                    _isWaitingForCharging = false;

                    // Restart battery monitoring for future charging needs
                    if (this.IsDynamicChargingEnabled)
                    {
                        RestartBatteryMonitoring();
                        System.Diagnostics.Debug.WriteLine("[MainViewModel] Battery monitoring RESTARTED after charging");
                    }

                    // Update UI
                    ExecuteOnUIThread(() =>
                    {
                        if (mainForm?.lblStatus != null)
                        {
                            mainForm.lblStatus.Text = "✅ Charging complete! Path resumed.";
                        }
                    });

                    return;
                }

                await Task.Delay(100);
            }

            // If simulation stopped unexpectedly
            _isChargingInProgress = false;
            _isWaitingForCharging = false;

            System.Diagnostics.Debug.WriteLine("[MainViewModel] EXITED WaitForRobotToReachResumePoint - simulation stopped");
        }

        #endregion
        #endregion

        #region Private Methods - Thread Safety
        private void ExecuteOnUIThread(Action action)
        {
            if (mainForm.InvokeRequired)
            {
                mainForm.Invoke(action);
            }
            else
            {
                action();
            }
        }
        #endregion
 
        #region Public Methods - Dynamic Charging Settings

        /// <summary>
        /// Updates dynamic charging settings from UI
        /// Call this method when settings change in RobotPanel
        /// </summary>
        /// <param name="isEnabled">Whether dynamic charging is enabled</param>
        /// <param name="chargingTimeSeconds">Charging time in seconds</param>
        /// <param name="safetyMarginPercent">Safety margin percentage (5-20%)</param>
        public void UpdateChargingSettings(bool isEnabled, int chargingTimeSeconds, double safetyMarginPercent)
        {
            this.IsDynamicChargingEnabled = isEnabled;
            this.ChargingTimeSeconds = chargingTimeSeconds;
            this.SafetyMarginPercent = safetyMarginPercent;

            System.Diagnostics.Debug.WriteLine($"[MainViewModel] Charging Settings Updated: Enabled={isEnabled}, " +
                $"Time={chargingTimeSeconds}s, Safety={safetyMarginPercent}%");

            if (isEnabled && this.IsSimulating)
            {
                StartBatteryMonitoring();
            }
            else if (!isEnabled)
            {
                StopBatteryMonitoring();
                _isChargingInProgress = false;
                _isWaitingForCharging = false;
            }
        }

        /// <summary>
        /// Gets whether dynamic charging is enabled
        /// </summary>
        public bool GetDynamicChargingEnabled()
        {
            return this.IsDynamicChargingEnabled;
        }

        /// <summary>
        /// Gets the charging time in seconds
        /// </summary>
        public int GetChargingTimeSeconds()
        {
            return this.ChargingTimeSeconds;
        }

        /// <summary>
        /// Gets the safety margin percentage
        /// </summary>
        public double GetSafetyMarginPercent()
        {
            return this.SafetyMarginPercent;
        }

        #endregion

        #region Private Fields - Charging & Time Statistics

        private double _totalBatteryConsumed;
        private double _totalChargingTimeSeconds;
        private double _totalTravelTimeSeconds;
        private double _totalOverheadTimeSeconds;
        private double _initialBatteryPercent;
        private double _cellSizeCm;

        #endregion

        #region Private Methods - Update Statistics

        /// <summary>
        /// Updates the battery and time statistics display in the UI
        /// </summary>
        private void UpdateBatteryAndTimeStats()
        {
            // Notify property changed for all statistics
            OnPropertyChanged(nameof(BatteryAndTimeStatsText));
            OnPropertyChanged(nameof(TotalChargingUnits));
            OnPropertyChanged(nameof(TotalChargingTimeSeconds));
            OnPropertyChanged(nameof(TotalTravelTimeSeconds));
            OnPropertyChanged(nameof(TotalOverheadTimeSeconds));
            OnPropertyChanged(nameof(TotalTimeSeconds));

            // Update UI directly
            ExecuteOnUIThread(() =>
            {
                if (mainForm?.lblBattery != null)
                {
                    mainForm.lblBattery.Text = this.BatteryAndTimeStatsText;
                }
            });
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
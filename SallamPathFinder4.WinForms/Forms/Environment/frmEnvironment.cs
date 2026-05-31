#region File Header
/// <summary>
/// File: frmEnvironment.cs
/// Description: Main environment form for robot pathfinding simulation
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Goals;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.Core.Models.Sensors;
using SallamPathFinder4.Services.Simulation;
using SallamPathFinder4.WinForms.Container;
using SallamPathFinder4.WinForms.Controls;
using SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard;
using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentBrowser;
using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentDesigner;
using SallamPathFinder4.WinForms.Forms.RobotDesigner;
using SallamPathFinder4.WinForms.Forms.Settings.frmMapSettings;
using SallamPathFinder4.WinForms.Forms.Settings.frmObstacleSettings;
using SallamPathFinder4.WinForms.Helpers;
using SallamPathFinder4.WinForms.Panels;
using SallamPathFinder4.WinForms.ViewModels; 
#endregion

namespace SallamPathFinder4.WinForms.Forms
{
    public partial class frmEnvironment : Form
    {
        #region Constants
        private const int DETECTION_ZONE_INTERVAL_MS = 100;
        private const int DEFAULT_GRID_WIDTH = 100;
        private const int DEFAULT_GRID_HEIGHT = 100; 

        // Keyboard shortcuts
        private const Keys SHORTCUT_NEW_MAP = Keys.Control | Keys.N;
        private const Keys SHORTCUT_OPEN_MAP = Keys.Control | Keys.O;
        private const Keys SHORTCUT_SAVE_MAP = Keys.Control | Keys.S;
        private const Keys SHORTCUT_FIND_PATH = Keys.Control | Keys.F;
        private const Keys SHORTCUT_NEW_EXPERIMENT = Keys.Control | Keys.Shift | Keys.N;
        private const Keys SHORTCUT_SET_START_POINT = Keys.Control | Keys.Shift | Keys.S;
        private const Keys SHORTCUT_ORDER_GOALS = Keys.Control | Keys.Shift | Keys.G;
        private const Keys SHORTCUT_START_SIMULATION = Keys.F5;
        private const Keys SHORTCUT_PAUSE_SIMULATION = Keys.F6;
        private const Keys SHORTCUT_STOP_SIMULATION = Keys.F7;
        private GifRecorder _gifRecorder;
        private string _recordingOutputPath;
        #endregion

        #region Private Fields
        private MainViewModel _viewModel;
        private System.Windows.Forms.Timer _detectionZoneTimer;
        private bool _isAddingGoal;
        private bool _isAddingParking;
        private GoalPoint _movingGoal;
        private ParkingPoint _movingParking;
        private bool _isMovingGoal;
        private bool _isMovingParking;
        private Random _random;
        private ISimulationService _simulationService;
            #region Private Fields - Dynamic Charging
            private bool _isDynamicChargingEnabled;
            private int _chargingTimeSeconds;
            private double _safetyMarginPercent;
            #endregion 
            #region Private Fields - Start Point
            private bool _isSettingStartPoint;
            #endregion
            #region Private Fields - Goals Ordering
            private bool _orderGoalsByDistance;
            private List<GoalPoint> _originalGoalsOrder;
            #endregion

            #region Private Fields - Robot Management
            private RobotDefinition _currentRobot;
            private string _currentRobotPath;
        #endregion
        #endregion

        #region Constructor
        public frmEnvironment()
        {
            try
            {
                CreateMenuAndToolbar();
                InitializeComponent();
                InitializeCustomComponents();

                // مؤقتاً: لا تحمل روبوت تلقائياً
                // LoadDefaultRobot();
                _currentRobot = new RobotDefinition();
                _currentRobot.RobotName = "Default Robot"; 
                 
                System.Diagnostics.Debug.WriteLine("frmEnvironment initialized successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing environment: {ex.Message}\n\n{ex.StackTrace}",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Initialization
        private void InitializeCustomComponents()
        {
            _random = new Random();

            ConfigureForm();
            InitializeMapGrid();
            InitializeServicesAndViewModel();
            InitializePanels();
            WireEvents();
            BindViewModel();
            StartDetectionZoneUpdater();
        }

        private void ConfigureForm()
        {
            this.Text = "SallamPathFinder 4: Cognitive Mobility Robot";
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;
        }

        private void InitializeMapGrid()
        {
            var mapGrid = new MapGrid(DEFAULT_GRID_WIDTH, DEFAULT_GRID_HEIGHT);
            mapGrid[10, 10].ElementType = MapElementType.StartPoint;
            mapGrid.UpdateAllCellProperties();
            mapControl.MapGrid = mapGrid;
            mapControl.RobotPosition = new Point(10, 10);
            mapControl.RobotAngle = 180;
        }

        private void InitializeServicesAndViewModel()
        {
            var mapGrid = mapControl.MapGrid;
            var pathfindingService = ServiceContainer.CreatePathfindingService(mapGrid);
            var batteryService = ServiceContainer.Resolve<IBatteryService>();
            var simulationService = ServiceContainer.CreateSimulationService(mapGrid, mapControl.DynamicObstacles);
            var fileService = ServiceContainer.Resolve<IFileService>();
            var experimentService = ServiceContainer.Resolve<IExperimentService>();

            _simulationService = simulationService;

            _viewModel = new MainViewModel(
                pathfindingService, simulationService, batteryService,
                fileService, experimentService, mapGrid, mapControl, this);

            if (_viewModel.Goals != null && _viewModel.Goals.Count > 0)
            {
                var goalsList = _viewModel.Goals.Select(g => g.Location).ToList();
                simulationService.SetGoals(goalsList);
            }
            else
            {
                simulationService.SetGoals(new List<Point>());
            }

            _viewModel.RobotState.Position = mapControl.RobotPosition;
            _viewModel.RobotState.Angle = mapControl.RobotAngle;
        }

        private void InitializePanels()
        {
            if (robotPanel == null) robotPanel = new RobotPanel();
            if (algorithmSettingsPanel == null) algorithmSettingsPanel = new AlgorithmSettingsPanel();
            if (goalsPanel == null) goalsPanel = new GoalsPanel(_viewModel.Goals);
            if (parkingPanel == null) parkingPanel = new ParkingPanel(_viewModel.ParkingPoints);
            if (pathDisplayPanel == null) pathDisplayPanel = new PathDisplayPanel();
            if (obstacleLogPanel == null) obstacleLogPanel = new ObstacleLogPanel(_viewModel.ObstacleLog);

            var algoLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(5)
            };
            algoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            algoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            algoLayout.Controls.Add(robotPanel, 0, 0);
            algoLayout.Controls.Add(algorithmSettingsPanel, 0, 1);
            tabAlgorithmRobot.Controls.Clear();
            tabAlgorithmRobot.Controls.Add(algoLayout);

            var goalsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(5)
            };
            goalsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            goalsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            goalsLayout.Controls.Add(goalsPanel, 0, 0);
            goalsLayout.Controls.Add(parkingPanel, 0, 1);
            tabGoalsParking.Controls.Clear();
            tabGoalsParking.Controls.Add(goalsLayout);

            tabPathResults.Controls.Clear();
            tabPathResults.Controls.Add(pathDisplayPanel);

            tabObstacleLog.Controls.Clear();
            tabObstacleLog.Controls.Add(obstacleLogPanel);
        }
        #endregion

        #region Event Wiring
        private void WireEvents()
        {
            if (goalsPanel != null)
            {
                goalsPanel.GoalAddRequested += (s, loc) => StartAddingGoal();
                goalsPanel.GoalRemoveRequested += (s, goal) => RemoveGoal(goal);
                goalsPanel.GoalMoveRequested += (s, goal) => StartMovingGoal(goal);
            }

            if (parkingPanel != null)
            {
                parkingPanel.ParkingAddRequested += (s, loc) => StartAddingParking();
                parkingPanel.ParkingRemoveRequested += (s, parking) => RemoveParking(parking);
                parkingPanel.ParkingMoveRequested += (s, parking) => StartMovingParking(parking);
            }

            if (robotPanel != null)
            {
                robotPanel.SimulateClick += (s, e) => _viewModel.StartSimulation();
                robotPanel.PauseClick += (s, e) => _viewModel.TogglePause();
                robotPanel.StopClick += (s, e) => _viewModel.StopSimulation();
                robotPanel.SpeedChanged += OnRobotSpeedChanged;
                robotPanel.RobotDimensionsChanged += OnRobotDimensionsChanged;
            }

            if (algorithmSettingsPanel != null)
            {
                algorithmSettingsPanel.FindPathRequested += (s, e) => _viewModel.FindPathAsync();
                algorithmSettingsPanel.SettingsChanged += (s, e) => UpdateAlgorithmSettings();
                algorithmSettingsPanel.PauseRequested += (s, e) => _viewModel?.PauseSearch();
                algorithmSettingsPanel.StopRequested += (s, e) => _viewModel?.StopSearch(); 
                algorithmSettingsPanel.ResumeRequested += (s, e) => _viewModel?.ResumeSearch();
            }
            // Dynamic Charging Events
            WireChargingEvents();

            mapControl.MouseClick += MapControl_MouseClick;
            mapControl.MouseMove += MapControl_MouseMove;
            mapControl.ViewChanged += (s, e) =>
            {
                // حساب المنطقة المرئية من الخريطة
                float scaledCellSize = mapControl.CellSize * mapControl.ZoomLevel;
                if (scaledCellSize <= 0) return;

                // 🔴 استخدم ViewOffset بدلاً من _viewOffset
                PointF viewOffset = mapControl.ViewOffset;

                int visibleStartX = (int)Math.Max(0, -viewOffset.X / scaledCellSize);
                int visibleEndX = (int)Math.Min(mapControl.MapGrid.Width - 1,
                    visibleStartX + (mapControl.Width / scaledCellSize) + 2);

                int visibleStartY = (int)Math.Max(0, -viewOffset.Y / scaledCellSize);
                int visibleEndY = (int)Math.Min(mapControl.MapGrid.Height - 1,
                    visibleStartY + (mapControl.Height / scaledCellSize) + 2);

                // تحديث المسطرة الأفقية
                rulerTop.UpdateVisibleRange(visibleStartX, visibleEndX, mapControl.ZoomLevel);

                // تحديث المسطرة الرأسية
                rulerLeft.UpdateVisibleRange(visibleStartY, visibleEndY, mapControl.ZoomLevel);
            };
            robotPanel.BatteryLevelChanged += (s, e) => UpdateBatteryFromPanel();

            // ربط حدث تغيير إعدادات التصور
            algorithmSettingsPanel.VisualizationSettingsChanged += (s, e) =>
            {
                if (_viewModel != null)
                {
                    _viewModel.EnableVisualization = algorithmSettingsPanel.IsVisualizationEnabled;
                    _viewModel.VisualizationSpeedDelayMs = algorithmSettingsPanel.GetSpeedDelayMs();
                }
            };

            // تعيين القيم الأولية
            _viewModel.EnableVisualization = algorithmSettingsPanel.IsVisualizationEnabled;
            _viewModel.VisualizationSpeedDelayMs = algorithmSettingsPanel.GetSpeedDelayMs();

            algorithmSettingsPanel.StartRecordingRequested += OnStartRecording;
            algorithmSettingsPanel.StopRecordingRequested += OnStopRecording;
        }

        private void UpdateAlgorithmSettings()
        {
            if (_viewModel == null || algorithmSettingsPanel == null) return;

            _viewModel.SelectedAlgorithm = algorithmSettingsPanel.CurrentAlgorithm;
            _viewModel.SelectedMetric = algorithmSettingsPanel.SelectedMetric;  // ← NEW
            _viewModel.AllowDiagonals = algorithmSettingsPanel.AllowDiagonals;
            _viewModel.HeavyDiagonals = algorithmSettingsPanel.HeavyDiagonals;
            _viewModel.HeuristicWeight = algorithmSettingsPanel.HeuristicWeight;
            _viewModel.SearchLimit = algorithmSettingsPanel.SearchLimit;
        }

        #region Event Wiring - Dynamic Charging

        /// <summary>
        /// Wires up dynamic charging events from RobotPanel
        /// </summary>
        private void WireChargingEvents()
        {
            if (robotPanel == null) return;

            robotPanel.ChargingSettingsChanged += OnChargingSettingsChanged;

            // ربط حدث اكتمال الشحن
            robotPanel.ChargingCompleted += OnChargingCompleted;
        }
        private void OnChargingCompleted()
        {
            // هذا الحدث قد لا يكون ضرورياً لأن MainViewModel يتعامل مع الشحن مباشرة
            System.Diagnostics.Debug.WriteLine("[frmEnvironment] Charging completed event received");
        }
        /// <summary>
        /// Handles charging settings changes from RobotPanel
        /// </summary>
        private void OnChargingSettingsChanged(object sender, EventArgs e)
        {
            if (robotPanel == null) return;

            _isDynamicChargingEnabled = robotPanel.IsDynamicChargingEnabled;
            _chargingTimeSeconds = robotPanel.ChargingTimeSeconds;
            _safetyMarginPercent = robotPanel.SafetyMarginPercent;

            // Update ViewModel
            _viewModel?.UpdateChargingSettings(_isDynamicChargingEnabled, _chargingTimeSeconds, _safetyMarginPercent);

            // Update status display
            if (_isDynamicChargingEnabled)
            {
                TimeSpan ts = TimeSpan.FromSeconds(_chargingTimeSeconds);
                lblStatus.Text = $"🔋 Dynamic Charging ENABLED | Charge Time: {ts:hh\\:mm\\:ss} | Safety Margin: {_safetyMarginPercent}%";
            }
            else
            {
                lblStatus.Text = "🔋 Dynamic Charging DISABLED (Manual battery replacement mode)";
            }

            System.Diagnostics.Debug.WriteLine($"[frmEnvironment] Charging Settings: Enabled={_isDynamicChargingEnabled}, " +
                $"Time={_chargingTimeSeconds}s, Safety={_safetyMarginPercent}%");
        }

        #endregion

        #endregion

        #region ViewModel Binding
        private void BindViewModel()
        {
            if (_viewModel == null) return;

            _viewModel.RobotState.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.RobotState.Position) ||
                    e.PropertyName == nameof(_viewModel.RobotState.Angle))
                {
                    UpdateRobotPositionDisplay();
                }
                if (e.PropertyName == nameof(_viewModel.RobotState.BatteryLevel))
                {
                    UpdateBatteryDisplay();
                }
            };

            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.HasPath) && _viewModel.HasPath)
                {
                    DisplayPath();
                }
                if (e.PropertyName == nameof(_viewModel.IsSearching))
                {
                    lblStatus.Text = _viewModel.IsSearching ? "🟡 Searching..." : "🟢 Ready";
                }
                if (e.PropertyName == nameof(_viewModel.IsSimulating))
                {
                    lblStatus.Text = _viewModel.IsSimulating ? "🔵 Simulating..." : "🟢 Ready";
                    robotPanel?.SetButtonStates(_viewModel.IsSimulating, _viewModel.IsPaused);
                }
            };
        }

        private void UpdateRobotPositionDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateRobotPositionDisplay));
                return;
            }
            lblRobotPos.Text = $"Robot: ({_viewModel.RobotState.Position.X},{_viewModel.RobotState.Position.Y}) {_viewModel.RobotState.Angle:F0}° | Speed: {_viewModel.RobotState.Speed:F1} cm/s";
            mapControl.UpdateRobot(_viewModel.RobotState.Speed, _viewModel.RobotState.Position, _viewModel.RobotState.Angle);
        }

        /// <summary>
        /// Updates the battery display in the status bar
        /// </summary>
        private void UpdateBatteryDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateBatteryDisplay));
                return;
            }

            if (lblBattery != null && _viewModel != null)
            {
                // Use the new formatted text from ViewModel
                lblBattery.Text = _viewModel.BatteryAndTimeStatsText;
            }

            robotPanel?.UpdateBattery(_viewModel?.RobotState.BatteryLevel ?? 100);
        }

        private void DisplayPath()
        {
            var path = _viewModel.CurrentPathResult?.Path;
            if (path == null || path.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("DisplayPath: No path to display");
                return;
            }

            pathDisplayPanel?.ClearPath();
            int step = 1;
            foreach (var node in path)
            {
                pathDisplayPanel?.AddPathStep(step++, node.X, node.Y, "Main", Color.Gold);

            }
            pathDisplayPanel?.UpdateStats(path.Count, _viewModel.CurrentPathResult.ComputationTimeSeconds * 1000, path.Count * 10.0);
            mapControl.DrawPath(path.ToList(), Color.Gold);
            lblStatus.Text = $"🟢 Path found! Length: {path.Count} cells";
        }
        #endregion

        #region Goals and Parking Methods
        private void StartAddingGoal()
        {
            System.Diagnostics.Debug.WriteLine("=== StartAddingGoal CALLED ===");

            CancelCurrentDrawMode();
            _isAddingGoal = true;
            System.Diagnostics.Debug.WriteLine($"_isAddingGoal = {_isAddingGoal}");

            mapControl.Cursor = Cursors.Cross;
            lblStatus.Text = "🟡 Click on map to add Goal point (Press ESC to cancel)";
        }

        private void StartAddingParking()
        {
            CancelCurrentDrawMode();
            _isAddingParking = true;
            mapControl.Cursor = Cursors.Cross;
            lblStatus.Text = "🟡 Click on map to add Parking point (Press ESC to cancel)";
        }

        private void StartMovingGoal(GoalPoint goal)
        {
            _isMovingGoal = true;
            _movingGoal = goal;
            mapControl.Cursor = Cursors.Cross;
            lblStatus.Text = $"🟡 Click on map to move {goal.Name}";
        }

        private void StartMovingParking(ParkingPoint parking)
        {
            _isMovingParking = true;
            _movingParking = parking;
            mapControl.Cursor = Cursors.Cross;
            lblStatus.Text = $"🟡 Click on map to move {parking.Name}";
        }

        private void RemoveGoal(GoalPoint goal)
        {
            mapControl.RemoveGoalAt(goal.Location);
            RefreshGoalsList();
            lblStatus.Text = $"❌ Goal {goal.Name} removed";
        }

        private void RemoveParking(ParkingPoint parking)
        {
            mapControl.RemoveParkingAt(parking.Location);
            RefreshParkingList();
            lblStatus.Text = $"❌ Parking {parking.Name} removed";
        }

        private void RefreshGoalsList()
        {
            if (_viewModel == null || mapControl == null) return;

            // مسح الأهداف الموجودة في ViewModel
            _viewModel.Goals.Clear();

            // إضافة جميع الأهداف من mapControl إلى ViewModel
            foreach (var goal in mapControl.Goals)
            {
                if (goal != null)
                {
                    _viewModel.Goals.Add(goal);
                }
            }

            // تحديث واجهة المستخدم
            goalsPanel?.RefreshList();
            _viewModel.RefreshHasGoals();

            // إعادة رسم الخريطة
            mapControl.Invalidate();
        }

        private void RefreshParkingList()
        {
            _viewModel.ParkingPoints.Clear();
            foreach (var parking in mapControl.ParkingPoints)
                _viewModel.ParkingPoints.Add(parking);
            parkingPanel?.RefreshList();
        }

        private void UpdateBatteryFromPanel()
        {
            if (_viewModel != null && robotPanel != null)
            {
                double newBatteryLevel = robotPanel.SetBatteryLevel;
                _viewModel.RobotState.BatteryLevel = newBatteryLevel;
                _viewModel.SetBatteryLevel(newBatteryLevel);
            }
        }
        #endregion

        #region Map Event Handlers
        private void MapControl_MouseClick(object sender, MouseEventArgs e)
        {
 
            var cell = mapControl.GetGridCellAtPoint(e.Location);
            if (!mapControl.MapGrid.IsValidCoordinate(cell.X, cell.Y)) {
                System.Diagnostics.Debug.WriteLine($"Invalid cell: ({cell.X},{cell.Y})");

                return;
            }
        

            // ========== Handle set start point mode ==========
            if (_isSettingStartPoint)
            {
                mapControl.SetCurrentStartPoint(cell);
                mapControl.RobotPosition = cell;

                if (_viewModel != null)
                {
                    _viewModel.RobotState.Position = cell;
                }

                _isSettingStartPoint = false;
                mapControl.Cursor = Cursors.Default;
                lblStatus.Text = $"🟢 Start point set to ({cell.X},{cell.Y})";
                mapControl.Invalidate();
                return;
            }
            
            if (!mapControl.MapGrid.IsValidCoordinate(cell.X, cell.Y)) return;

            if (_isAddingGoal)
            {
                System.Diagnostics.Debug.WriteLine($"Adding goal at ({cell.X},{cell.Y})");

                var color = Color.FromArgb(_random.Next(100, 255), _random.Next(100, 255), _random.Next(100, 255));
                mapControl.AddGoalAt(cell, color);
                System.Diagnostics.Debug.WriteLine($"AddGoalAt completed");
                RefreshGoalsList();
                System.Diagnostics.Debug.WriteLine($"RefreshGoalsList completed, Goals count = {_viewModel?.Goals?.Count ?? 0}");
                _isAddingGoal = false;
                mapControl.Cursor = Cursors.Default;
                mapControl.CurrentDrawMode = MapControl.DrawMode.None;
                mapControl.Invalidate();
                lblStatus.Text = "🟢 Ready";
                return;
            }

            if (_isAddingParking)
            {
                mapControl.AddParkingAt(cell);
                RefreshParkingList();
                _isAddingParking = false;
                mapControl.Cursor = Cursors.Default;
                mapControl.CurrentDrawMode = MapControl.DrawMode.None;
                lblStatus.Text = "🟢 Ready";
                return;
            }

            if (_isMovingGoal && _movingGoal != null)
            {
                mapControl.RemoveGoalAt(_movingGoal.Location);
                _movingGoal.Location = cell;
                mapControl.AddGoalAt(cell, _movingGoal.Color);
                RefreshGoalsList();
                _isMovingGoal = false;
                _movingGoal = null;
                mapControl.Cursor = Cursors.Default;
                mapControl.CurrentDrawMode = MapControl.DrawMode.None;
                lblStatus.Text = "🟢 Ready";
                return;
            }

            if (_isMovingParking && _movingParking != null)
            {
                mapControl.RemoveParkingAt(_movingParking.Location);
                _movingParking.Location = cell;
                mapControl.AddParkingAt(cell);
                RefreshParkingList();
                _isMovingParking = false;
                _movingParking = null;
                mapControl.Cursor = Cursors.Default;
                mapControl.CurrentDrawMode = MapControl.DrawMode.None;
                lblStatus.Text = "🟢 Ready";
                return;
            }

            if (mapControl.CurrentDrawMode == MapControl.DrawMode.SetDynamicObstacle)
            {
                mapControl.CurrentDrawMode = MapControl.DrawMode.None;
                mapControl.Cursor = Cursors.Default;
                lblStatus.Text = "🟢 Dynamic obstacle added. Ready";
                return;
            }

            lblCellPos.Text = $"Cell: ({cell.X},{cell.Y})";
            lblStatus.Text = $"Cell ({cell.X},{cell.Y}) | Walkable: {mapControl.MapGrid[cell.X, cell.Y].IsWalkable}";
        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            var cell = mapControl.GetGridCellAtPoint(e.Location);
            if (mapControl.MapGrid.IsValidCoordinate(cell.X, cell.Y))
            {
                lblMousePos.Text = $"Mouse: ({e.X},{e.Y})";
                lblCellPos.Text = $"Cell: ({cell.X},{cell.Y})";
                double realX = cell.X * mapControl.ScaleCmPerCell;
                double realY = cell.Y * mapControl.ScaleCmPerCell;
                lblRealPos.Text = $"Real: ({realX:F1}cm, {realY:F1}cm)";
            }
        }
        #endregion

        #region Map Operations
        private void NewMap()
        {
            mapControl.ClearPaths();
            mapControl.ClearGoals();
            mapControl.ClearParkingPoints();
            mapControl.ResetStartPoints();
            mapControl.AddStartPoint(new Point(10, 10));

            if (_simulationService is SimulationService simSvc)
            {
                simSvc.ReinitializeDoorGroups();
            }

            pathDisplayPanel?.ClearPath();
            lblStatus.Text = "🟢 New map created";
        }

        private async void OpenMap()
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Sallam Map (*.smap)|*.smap";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                await _viewModel.LoadMapAsync(ofd.FileName);

                if (_simulationService is SimulationService simSvc)
                {
                    simSvc.ReinitializeDoorGroups();
                }

                RefreshGoalsList();
                RefreshParkingList();
                lblStatus.Text = $"✅ Map loaded: {System.IO.Path.GetFileName(ofd.FileName)}";
            }
        }

        private async void SaveMap()
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "Sallam Map (*.smap)|*.smap";
            sfd.DefaultExt = "smap";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                await _viewModel.SaveMapAsync(sfd.FileName);
                lblStatus.Text = $"💾 Map saved: {System.IO.Path.GetFileName(sfd.FileName)}";
            }
        }

        private void ResetView()
        {
            mapControl.ZoomLevel = 1.0f;
            mapControl.CellSize = 30;
            lblStatus.Text = "View reset";
        }

        private void ToggleGrid()
        {
            mapControl.ShowGrid = !mapControl.ShowGrid;
            showGridItem.Checked = mapControl.ShowGrid;
        }

        private void ToggleCoordinates()
        {
            mapControl.ShowCoordinates = !mapControl.ShowCoordinates;
            showCoordsItem.Checked = mapControl.ShowCoordinates;
        }
        #endregion

        #region Detection Zone
        private void StartDetectionZoneUpdater()
        {
            _detectionZoneTimer = new System.Windows.Forms.Timer();
            _detectionZoneTimer.Interval = DETECTION_ZONE_INTERVAL_MS;
            _detectionZoneTimer.Tick += (s, e) => UpdateDetectionZone();
            _detectionZoneTimer.Start();
        }

        private void UpdateDetectionZone()
        {
            if (mapControl == null || _viewModel == null) return;
            var zoneCells = _viewModel.GetDetectionZoneCells();
            mapControl.UpdateDetectionZone(zoneCells);
        }

        private void StopDetectionZoneUpdater()
        {
            _detectionZoneTimer?.Stop();
            _detectionZoneTimer?.Dispose();
        }
        #endregion

        #region Test Methods
        private async Task TestAllAlgorithms()
        {
            var start = mapControl.RobotPosition;
            var end = _viewModel.Goals.Count > 0 ? _viewModel.Goals[0].Location : new Point(50, 50);

            lblStatus.Text = "🧪 Testing all algorithms...";
            Application.DoEvents();

            var results = await _viewModel.TestAllAlgorithmsAsync(start, end);

            var resultMessage = "===== ALGORITHM TEST RESULTS =====\n\n";
            foreach (var result in results)
                resultMessage += result.ToString() + "\n";

            var successful = results.Where(r => r.Success).ToList();
            resultMessage += $"\n--- SUMMARY ---\n";
            resultMessage += $"Successful: {successful.Count}/{results.Count}\n";

            if (successful.Any())
            {
                var fastest = successful.OrderBy(r => r.ComputationTimeMs).First();
                var shortest = successful.OrderBy(r => r.PathLength).First();
                resultMessage += $"Fastest: {fastest.AlgorithmName} ({fastest.ComputationTimeMs:F2}ms)\n";
                resultMessage += $"Shortest Path: {shortest.AlgorithmName} ({shortest.PathLength} cells)";
            }

            MessageBox.Show(resultMessage, "Algorithm Test Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
            lblStatus.Text = "✅ Algorithm testing completed";
        }

        private async Task TestSingleAlgorithm(AlgorithmType type)
        {
            var start = mapControl.RobotPosition;
            var end = _viewModel.Goals.Count > 0 ? _viewModel.Goals[0].Location : new Point(50, 50);

            lblStatus.Text = $"🧪 Testing {type}...";
            Application.DoEvents();

            var result = await _viewModel.TestAlgorithmAsync(type, start, end);

            var message = $"===== {type} TEST RESULTS =====\n\n";
            message += $"Start: ({start.X},{start.Y})\n";
            message += $"End: ({end.X},{end.Y})\n";
            message += $"Success: {(result.Success ? "YES" : "NO")}\n";

            if (result.Success)
            {
                message += $"Path Length: {result.PathLength} cells\n";
                message += $"Computation Time: {result.ComputationTimeMs:F2} ms\n";
                message += $"Nodes Explored: {result.NodesExplored}\n";
            }
            else
            {
                message += $"Error: {result.ErrorMessage}\n";
            }

            MessageBox.Show(message, $"{type} Test Results", MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            lblStatus.Text = $"✅ {type} testing completed";
        }

        private void ClearTestResults()
        {
            mapControl.ClearPaths();
            pathDisplayPanel?.ClearPath();
            lblStatus.Text = "Test results cleared";
        }
        #endregion

        #region Dialog Methods
        private void ShowRobotDashboard()
        {
            var dashboard = new frmRobotDashboard();
            dashboard.Show();
        }

        private void ShowExperimentDesigner()
        {
            var designer = new frmExperimentDesigner(mapControl.MapGrid, mapControl, _viewModel);
            designer.ShowDialog();
        }

        private void ShowMapSettings()
        {
            double currentScale = mapControl.ScaleCmPerCell;
            var settingsForm = new frmMapSettings(mapControl.MapGrid, mapControl.CellSize, currentScale);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                mapControl.CellSize = settingsForm.CellSize;
                mapControl.ScaleCmPerCell = settingsForm.Scale;
                mapControl.Invalidate();
                mapControl.Refresh();

                if (rulerTop != null)
                {
                    rulerTop.CellSize = settingsForm.CellSize;
                    rulerTop.Scale = (float)settingsForm.Scale;
                    rulerTop.Invalidate();
                }
                if (rulerLeft != null)
                {
                    rulerLeft.CellSize = settingsForm.CellSize;
                    rulerLeft.Scale = (float)settingsForm.Scale;
                    rulerLeft.Invalidate();
                }

                lblStatus.Text = $"Map updated: Cell size = {settingsForm.CellSize}px, Scale = {settingsForm.Scale}cm/cell";
            }
        }

        private void ShowObstacleSettings()
        {
            var settingsService = ServiceContainer.Resolve<IObstacleSettingsService>();
            var viewModel = new ObstacleSettingsViewModel(settingsService);
            var settingsForm = new frmObstacleSettings(viewModel);
            settingsForm.ShowDialog();
        }
        #endregion

        #region Goals Ordering Methods

        /// <summary>
        /// Toggles ordering of goals by distance from start point
        /// </summary>
        private void ToggleOrderGoalsByDistance()
        {
            if (_viewModel == null || _viewModel.Goals == null)
            {
                lblStatus.Text = "⚠️ no goals to order it ";
                return;
            }

            _orderGoalsByDistance = !_orderGoalsByDistance;

            if (_orderGoalsByDistance)
            {
                OrderGoalsByDistance();
                lblStatus.Text = "🟢 Goals ordered by distance from start point";
                System.Diagnostics.Debug.WriteLine("[GoalsOrder] Order goals by distance: ENABLED");
            }
            else
            {
                RestoreOriginalGoalsOrder();
                lblStatus.Text = "🟢 Goals restored to original order";
                System.Diagnostics.Debug.WriteLine("[GoalsOrder] Order goals by distance: DISABLED");
            }
        }

        /// <summary>
        /// Orders goals by distance from current robot start point using selected distance metric
        /// </summary>
        private void OrderGoalsByDistance()
        {
            if (_viewModel?.Goals == null || _viewModel.Goals.Count == 0) return;

            Point startPoint = mapControl.HasCustomStartPoint
                ? mapControl.RobotStartPoint
                : mapControl.RobotPosition;

            DistanceMetric selectedMetric = algorithmSettingsPanel?.SelectedMetric ?? DistanceMetric.Manhattan;

            if (_originalGoalsOrder == null || _originalGoalsOrder.Count == 0)
            {
                _originalGoalsOrder = _viewModel.Goals.ToList();
            }

            var orderedGoals = _viewModel.Goals
                .OrderBy(g => CalculateDistance(startPoint, g.Location, selectedMetric))
                .ToList();

            _viewModel.Goals.Clear();
            foreach (var goal in orderedGoals) _viewModel.Goals.Add(goal);

            mapControl.Goals = _viewModel.Goals.ToList();
            goalsPanel?.RefreshList();
            _viewModel.RefreshHasGoals();
            _viewModel.ClearCachedPath();

            mapControl.ClearPaths();
            pathDisplayPanel?.ClearPath();
            _viewModel.FindPathAsync();
        }

        /// <summary>
        /// Calculates distance between two points using the specified metric
        /// </summary>
        private double CalculateDistance(Point a, Point b, DistanceMetric metric)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);

            return metric switch
            {
                DistanceMetric.Manhattan => dx + dy,
                DistanceMetric.Euclidean => Math.Sqrt(dx * dx + dy * dy),
                DistanceMetric.MaxDXDY => Math.Max(dx, dy),
                DistanceMetric.DiagonalShortcut => (2 * Math.Min(dx, dy)) + Math.Abs(dx - dy),
                DistanceMetric.EuclideanNoSQR => dx * dx + dy * dy,
                _ => dx + dy // Default to Manhattan
            };
        }

        /// <summary>
        /// Restores original goals order
        /// </summary>
        /// <summary>
        /// Restores original goals order
        /// </summary>
        private void RestoreOriginalGoalsOrder()
        {
            if (_viewModel?.Goals == null || _originalGoalsOrder == null || _originalGoalsOrder.Count == 0)
            {
                return;
            }

            _viewModel.Goals.Clear();
            foreach (var goal in _originalGoalsOrder)
            {
                _viewModel.Goals.Add(goal);
            }

            RefreshGoalsList();
            System.Diagnostics.Debug.WriteLine("[GoalsOrder] Original order restored");

            // Recalculate path with original goal order
            if (_viewModel.HasPath || _viewModel.CurrentPathResult != null)
            {
                System.Diagnostics.Debug.WriteLine("[GoalsOrder] Recalculating path with original goal order...");
                mapControl.ClearPaths();
                pathDisplayPanel?.ClearPath();
                _viewModel.FindPathAsync();
            }
        }

        /// <summary>
        /// Checks if goals are currently ordered by distance
        /// </summary>
        private bool IsGoalsOrderedByDistance => _orderGoalsByDistance;

        #endregion

        #region Form Events
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopDetectionZoneUpdater();
            base.OnFormClosing(e);
        }

        #region Form Events

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // ESC - Cancel current operation
            if (keyData == Keys.Escape)
            {
                CancelCurrentDrawMode();
                return true;
            }

            // Manual robot control
            switch (keyData)
            {
                // حركة أمامية وخلفية
                case Keys.W:
                case Keys.S:
                // دوران حول عجلة (Tank)
                // دوران يسار
                case Keys.A:
                    mapControl.RotateRobot(-30f);
                    _viewModel.RobotState.Angle = mapControl.RobotAngle;
                    UpdateRobotPositionDisplay();
                    return true;

                // دوران يمين
                case Keys.D:
                    mapControl.RotateRobot(30f);
                    _viewModel.RobotState.Angle = mapControl.RobotAngle;
                    UpdateRobotPositionDisplay();
                    return true;
                // دوران حول مركز (Pivot)
                case Keys.Q:
                case Keys.E:
                // انزلاق جانبي
                case Keys.Z:
                case Keys.C:
                    double currentSpeed = _viewModel?.RobotState.Speed ?? 10.0;
                    mapControl.MoveRobotManually(keyData, currentSpeed);

                    if (_viewModel != null)
                    {
                        _viewModel.RobotState.Position = mapControl.RobotPosition;
                        _viewModel.RobotState.Angle = mapControl.RobotAngle;
                    }

                    UpdateRobotPositionDisplay();
                    return true;
            }

            // ========== KEYBOARD SHORTCUTS ==========

            // File operations
            if (keyData == SHORTCUT_NEW_MAP)
            {
                NewMap();
                return true;
            }

            if (keyData == SHORTCUT_OPEN_MAP)
            {
                OpenMap();
                return true;
            }

            if (keyData == SHORTCUT_SAVE_MAP)
            {
                SaveMap();
                return true;
            }

            // Pathfinding
            if (keyData == SHORTCUT_FIND_PATH)
            {
                _viewModel?.FindPathAsync();
                return true;
            }

            // New experiment (clear everything)
            if (keyData == SHORTCUT_NEW_EXPERIMENT)
            {
                NewExperiment();
                return true;
            }

            // Set start point
            if (keyData == SHORTCUT_SET_START_POINT)
            {
                StartSetStartPointMode();
                return true;
            }

            // Order goals by distance
            if (keyData == SHORTCUT_ORDER_GOALS)
            {
                ToggleOrderGoalsByDistance(); 
                return true;
            }

            // Simulation control
            if (keyData == SHORTCUT_START_SIMULATION)
            {
                _viewModel?.StartSimulation();
                return true;
            }

            if (keyData == SHORTCUT_PAUSE_SIMULATION)
            {
                _viewModel?.TogglePause();
                return true;
            }

            if (keyData == SHORTCUT_STOP_SIMULATION)
            {
                _viewModel?.StopSimulation();
                return true;
            }

            // Algorithm selection (Ctrl+1 to Ctrl+7)
            if (keyData == (Keys.Control | Keys.D1))
            {
                SelectAlgorithm(0); // A*
                return true;
            }

            if (keyData == (Keys.Control | Keys.D2))
            {
                SelectAlgorithm(1); // SPPA
                return true;
            }

            if (keyData == (Keys.Control | Keys.D3))
            {
                SelectAlgorithm(2); // SPPA-DL
                return true;
            }

            if (keyData == (Keys.Control | Keys.D4))
            {
                SelectAlgorithm(3); // ACO
                return true;
            }

            if (keyData == (Keys.Control | Keys.D5))
            {
                SelectAlgorithm(4); // D*
                return true;
            }

            if (keyData == (Keys.Control | Keys.D6))
            {
                SelectAlgorithm(5); // KNN
                return true;
            }

            if (keyData == (Keys.Control | Keys.D7))
            {
                SelectAlgorithm(6); // Brute Force
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #region Shortcut Methods

        /// <summary>
        /// Creates a new experiment (clears all paths, resets battery, clears goals and parking)
        /// </summary>
        private void NewExperiment()
        {
            var result = MessageBox.Show(
                "Start a new experiment?\n\nThis will clear all paths, reset battery, and clear all goals and parking points.",
                "New Experiment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            // Clear all paths from map
            mapControl.ClearPaths();

            // Clear all goals
            mapControl.ClearGoals();
            _viewModel?.Goals?.Clear();

            // Clear all parking points
            mapControl.ClearParkingPoints();
            _viewModel?.ParkingPoints?.Clear();

            // Reset start points
            mapControl.ResetStartPoints();
            mapControl.AddStartPoint(new Point(10, 10));

            // Reset robot position
            mapControl.RobotPosition = new Point(10, 10);
            mapControl.RobotAngle = 0;

            // Reset battery to 100%
            _viewModel?.SetBatteryLevel(100);
            _viewModel?.ResetChargingStatistics();

            // Clear path display panel
            pathDisplayPanel?.ClearPath();

            // Reset status
            lblStatus.Text = "🟢 New experiment started. Ready.";

            // Refresh UI
            RefreshGoalsList();
            RefreshParkingList();
            mapControl.Invalidate();

            System.Diagnostics.Debug.WriteLine("[Shortcut] New experiment started");
        }
         
        /// <summary>
        /// Selects algorithm by index
        /// </summary>
        /// <summary>
        /// Selects algorithm by index
        /// </summary>
        /// <summary>
        /// Selects algorithm by index
        /// </summary>
        private void SelectAlgorithm(int algorithmIndex)
        {
            if (algorithmSettingsPanel != null)
            {
                algorithmSettingsPanel.SetAlgorithmByIndex(algorithmIndex);
                System.Diagnostics.Debug.WriteLine($"[Shortcut] Algorithm selected: index {algorithmIndex}");
            }
        }
         

        #endregion
        #endregion

        private void CancelCurrentDrawMode()
        {
            _isAddingGoal = false;
            _isAddingParking = false;
            _isMovingGoal = false;
            _isMovingParking = false;
            _movingGoal = null;
            _movingParking = null;

            mapControl.CurrentDrawMode = MapControl.DrawMode.None;
            mapControl.Cursor = Cursors.Default;
            lblStatus.Text = "🟢 Operation cancelled. Ready";
        }
        #endregion

        #region Obstacle Menu Methods
        private void SetStaticElement(MapElementType element)
        {
            CancelCurrentDrawMode();
            mapControl.CurrentDrawMode = MapControl.DrawMode.SetElement;
            mapControl.CurrentElement = element;
            lblStatus.Text = $"🟡 Click on map to add {element} (Press ESC to cancel)";
        }

        private void SetDynamicObstacleType(ObstacleType type)
        {
            CancelCurrentDrawMode();
            mapControl.CurrentDrawMode = MapControl.DrawMode.SetDynamicObstacle;
            mapControl.CurrentObstacleType = type;
            lblStatus.Text = $"🟡 Click on map to add {type} obstacle (Press ESC to cancel)";
        }

        private void SetSurfaceWeight(byte weight)
        {
            CancelCurrentDrawMode();
            mapControl.CurrentDrawMode = MapControl.DrawMode.SetWeight;
            mapControl.CurrentWeight = weight;
            lblStatus.Text = $"📊 Click on map to set {weight}% surface weight (Press ESC to cancel)";
        }

        private void ClearAllObstacles()
        {
            mapControl.DynamicObstacles.Clear();
            if (mapControl.MapGrid != null)
            {
                for (int x = 0; x < mapControl.MapGrid.Width; x++)
                {
                    for (int y = 0; y < mapControl.MapGrid.Height; y++)
                    {
                        var cell = mapControl.MapGrid[x, y];
                        cell.OccupyingObstacle = null;
                        cell.IsWalkable = true;
                        if (cell.ElementType == MapElementType.Empty)
                        {
                            cell.IsWalkable = true;
                        }
                    }
                }
                mapControl.MapGrid.UpdateAllCellProperties();
            }
            mapControl.Invalidate();
            lblStatus.Text = "All obstacles cleared";
        }

        private void ShowDashboard()
        {
            var dashboard = new frmRobotDashboard();
            dashboard.Show();
        }
        #endregion

        #region Start Point Methods

        /// <summary>
        /// Starts set start point mode (user clicks on map to set robot start position)
        /// </summary>
        private void StartSetStartPointMode()
        {
            CancelCurrentDrawMode();
            _isSettingStartPoint = true;
            mapControl.Cursor = Cursors.Cross;
            lblStatus.Text = "🟡 Click on map to set robot start point (Press ESC to cancel)";
            System.Diagnostics.Debug.WriteLine("[StartPoint] Set start point mode activated");
        }

        /// <summary>
        /// Resets start point to default (10,10)
        /// </summary>
        private void ResetStartPoint()
        {
            var result = MessageBox.Show(
                "Reset start point to default (10,10)?",
                "Reset Start Point",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Point defaultStart = new Point(10, 10);
                mapControl.SetCurrentStartPoint(defaultStart);
                mapControl.RobotPosition = defaultStart;

                if (_viewModel != null)
                {
                    _viewModel.RobotState.Position = defaultStart;
                }

                lblStatus.Text = $"🟢 Start point reset to ({defaultStart.X},{defaultStart.Y})";
                mapControl.Invalidate();
            }
        }

        #endregion

        #region Dialog Methods
        /// <summary>
        /// Shows the experiment browser form
        /// </summary>
        private void ShowExperimentBrowser()
        {
            try
            {
                var browser = new frmExperimentBrowser();
                browser.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Experiment Browser: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void OnRobotSpeedChanged(double newSpeed)
        {
            _viewModel.RobotState.Speed = newSpeed;

            if (_simulationService is SimulationService simSvc)
            {
                simSvc.SetRobotSpeedFromSettings(newSpeed); 
            }
        }
        private void OnRobotDimensionsChanged(int widthCm, int lengthCm, int heightCm)
        {
            mapControl.SetRobotDimensions(widthCm, lengthCm, heightCm);

            System.Diagnostics.Debug.WriteLine($"[Robot] Dimensions: W={widthCm}cm, L={lengthCm}cm, H={heightCm}cm");
        }

        private void OnStartRecording(object sender, EventArgs e)
        {
            string recordingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recordings");
            if (!Directory.Exists(recordingsPath))
                Directory.CreateDirectory(recordingsPath);

            string fileName = $"Search_{DateTime.Now:yyyyMMdd_HHmmss}.gif";
            _recordingOutputPath = Path.Combine(recordingsPath, fileName);

            _gifRecorder = new GifRecorder(fps: 10);
            _gifRecorder.FrameCaptured += (frameCount) =>
            {
                algorithmSettingsPanel.UpdateRecordingStatus(true, frameCount);
            };

            // 🔴 تصحيح: معامل واحد فقط (filePath)
            _gifRecorder.RecordingCompleted += (filePath) =>
            {
                algorithmSettingsPanel.UpdateRecordingStatus(false, _gifRecorder.FrameCount);
                MessageBox.Show($"تم حفظ الفيديو في:\n{filePath}", "تسجيل الفيديو",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            _gifRecorder.StartRecording(mapControl, _recordingOutputPath);
            algorithmSettingsPanel.UpdateRecordingStatus(true, 0);
        }

        private void OnStopRecording(object sender, EventArgs e)
        {
            _gifRecorder?.StopRecording();
            _gifRecorder?.Dispose();
            _gifRecorder = null;
        }

        #region Private Methods - Robot Management
        private void LoadDefaultRobot()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadDefaultRobot START");

                // إنشاء روبوت افتراضي مباشرة بدون تحميل من ملف
                CreateDefaultRobot();

                System.Diagnostics.Debug.WriteLine("Applying selected robot...");
                ApplySelectedRobot();

                System.Diagnostics.Debug.WriteLine("LoadDefaultRobot END");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in LoadDefaultRobot: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void CreateDefaultRobot()
        {
            _currentRobot = new RobotDefinition
            {
                RobotId = "robot_default_001",
                RobotName = "Default Robot",
                RobotType = RobotType.Wheeled,
                Description = "Default wheeled robot"
            };

            _currentRobot.Appearance.Width = 50;
            _currentRobot.Appearance.Height = 30;
            _currentRobot.Appearance.Length = 60;
            _currentRobot.Appearance.Color = "#3498db";

            _currentRobot.Kinematics.MaxForwardSpeed = 1.5;
            _currentRobot.Kinematics.MaxReverseSpeed = 0.8;
            _currentRobot.Kinematics.MaxTurnRate = 90;
            _currentRobot.Kinematics.MinTurnRadius = 30;

            // إزالة الحساسات القديمة - نستخدم SimpleSensor الآن
            _currentRobot.Sensors.Clear();

            // إضافة حساسات افتراضية باستخدام SimpleSensor
            _currentRobot.Sensors.Add(new SimpleSensor
            {
                SensorId = Guid.NewGuid().ToString(),
                SensorName = "Front Ultrasonic",
                SensorType = "Ultrasonic",
                Position = new Point(25, 0),
                MountAngle = 0
            });

            _currentRobot.Sensors.Add(new SimpleSensor
            {
                SensorId = Guid.NewGuid().ToString(),
                SensorName = "Front Bumper",
                SensorType = "Proximity",
                Position = new Point(20, -15),
                MountAngle = 0
            });
            if (mapControl != null)
                mapControl.CurrentRobot = _currentRobot;
             
        }

        private void UpdateRobotUI()
        {
            if (_currentRobot == null) return;

            // ========== 1. تحديث MapControl ==========
            mapControl.CurrentRobot = _currentRobot;

            // ========== 2. تحديث ViewModel ==========
            _viewModel?.SetCurrentRobot(_currentRobot);

            // ========== 3. تحديث شريط الحالة ==========
            UpdateRobotStatusDisplay();

            // ========== 4. تحديث RobotPanel ==========
            if (robotPanel != null)
            {
                try
                {
                    // تحديث السرعة
                    var nudSpeed = robotPanel.Controls.Find("_nudSpeed", true).FirstOrDefault() as NumericUpDown;
                    if (nudSpeed != null)
                    {
                        nudSpeed.Value = (decimal)(_currentRobot.Kinematics.MaxForwardSpeed * 100);
                    }

                    // تحديث العرض (Width)
                    var nudWidth = robotPanel.Controls.Find("_nudWidth", true).FirstOrDefault() as NumericUpDown;
                    if (nudWidth != null)
                    {
                        nudWidth.Value = (decimal)_currentRobot.Appearance.Width;
                    }

                    // تحديث الطول (Length)
                    var nudLength = robotPanel.Controls.Find("_nudLength", true).FirstOrDefault() as NumericUpDown;
                    if (nudLength != null)
                    {
                        nudLength.Value = (decimal)_currentRobot.Appearance.Length;
                    }

                    robotPanel.UpdateDimensionsExternally(
                        (int)_currentRobot.Appearance.Width,
                        (int)_currentRobot.Appearance.Length,
                        (int)_currentRobot.Appearance.Height    );
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error updating RobotPanel: {ex.Message}");
                }
            }
        }
        private void UpdateRobotStatusDisplay()
        {
            if (_currentRobot == null) return;

            if (InvokeRequired)
            {
                Invoke(new Action(UpdateRobotStatusDisplay));
                return;
            }

            // تحديث شريط الحالة بمعلومات الروبوت
            lblStatus.Text = $"Robot: {_currentRobot.RobotName} | Type: {_currentRobot.RobotType} | Speed: {_currentRobot.Kinematics.MaxForwardSpeed:F1} m/s | Sensors: {_currentRobot.Sensors.Count}";
        }

        private void OpenRobotSelector()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("OpenRobotSelector START");

                string robotsDir = Path.Combine(Application.StartupPath, "Robots");
                if (!Directory.Exists(robotsDir))
                {
                    Directory.CreateDirectory(robotsDir);
                }

                var selector = new frmRobotSelector();

                if (selector.ShowDialog() == DialogResult.OK && selector.SelectedRobot != null)
                {
                    _currentRobot = selector.SelectedRobot;
                    ApplySelectedRobot();

                    if (lblStatus != null)
                    {
                        lblStatus.Text = $"Robot '{_currentRobot.RobotName}' loaded - Custom drawing enabled";
                    }

                    MessageBox.Show($"Robot '{_currentRobot.RobotName}' selected successfully.",
                        "Robot Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                System.Diagnostics.Debug.WriteLine("OpenRobotSelector END");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in OpenRobotSelector: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Error opening robot selector: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void EditCurrentRobot()
        {
            if (_currentRobot != null)
            {
                using (var designer = new frmRobotDesigner(_currentRobot))
                {
                    if (designer.ShowDialog() == DialogResult.OK)
                    {
                        UpdateRobotUI();
                    }
                }
            }
            else
            {
                MessageBox.Show("No robot selected. Please select a robot first.", "No Robot",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowRobotInfo()
        {
            if (_currentRobot == null)
            {
                MessageBox.Show("No robot selected.", "Robot Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string info = $"═══════════════════════════════════════════════════════════\n" +
                         $"                    ROBOT INFORMATION\n" +
                         $"═══════════════════════════════════════════════════════════\n\n" +
                         $"Name:           {_currentRobot.RobotName}\n" +
                         $"Type:           {_currentRobot.RobotType}\n" +
                         $"ID:             {_currentRobot.RobotId}\n" +
                         $"Description:    {_currentRobot.Description}\n\n" +
                         $"Dimensions:     {_currentRobot.Appearance.Width:F0} x {_currentRobot.Appearance.Height:F0} x {_currentRobot.Appearance.Length:F0} cm\n" +
                         $"Max Speed:      {_currentRobot.Kinematics.MaxForwardSpeed:F1} m/s\n" +
                         $"Turn Rate:      {_currentRobot.Kinematics.MaxTurnRate:F0}°/s\n" +
                         $"Turn Radius:    {_currentRobot.Kinematics.MinTurnRadius:F0} cm\n" +
                         $"Maneuverability: {_currentRobot.Kinematics.GetManeuverabilityScore():F0}%\n\n" +
                         $"Sensors:        {_currentRobot.Sensors.Count}\n" +
                         $"Created:        {_currentRobot.CreatedAt:yyyy-MM-dd HH:mm}";

            MessageBox.Show(info, "Robot Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
        #region Robot Management Methods
      
        private void CreateNewRobot()
        {
            using (var designer = new frmRobotDesigner())
            {
                if (designer.ShowDialog() == DialogResult.OK)
                {
                    // بعد إنشاء روبوت جديد، افتح المحدد لاختياره
                    OpenRobotSelector();
                }
            }
        }
        private void ApplySelectedRobot()
        {
            // Validate robot exists
            if (_currentRobot == null)
                return;

            System.Diagnostics.Debug.WriteLine($"=== ApplySelectedRobot START: {_currentRobot.RobotName} ===");

            // ========== STEP 1: Apply robot to MapControl for drawing ==========
            try
            {
                // Set the robot definition for custom drawing
                mapControl.CurrentRobot = _currentRobot;
                // Enable custom robot drawing (shapes, sensors, etc.)
                mapControl.SetUseCustomRobot(true);
                // Enable sensor FOV visualization
                mapControl.ShowSensorFOV = true;
                // Force robot visibility
                mapControl.ShowRobot = true;
                // Force immediate redraw
                mapControl.Invalidate();

                System.Diagnostics.Debug.WriteLine($"  - MapControl updated: {_currentRobot.RobotName}");
                System.Diagnostics.Debug.WriteLine($"  - Robot position: ({mapControl.RobotPosition.X}, {mapControl.RobotPosition.Y})");
                System.Diagnostics.Debug.WriteLine($"  - MapGrid bounds: 0-{mapControl.MapGrid?.Width}, 0-{mapControl.MapGrid?.Height}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"  - ERROR in MapControl update: {ex.Message}");
            }

            // ========== STEP 2: Apply robot speed to simulation service ==========
            try
            {
                // Convert speed from m/s to cm/s (simulation uses cm/s)
                double speedCmPerSec = _currentRobot.Kinematics.MaxForwardSpeed * 100;

                // Update simulation service
                _simulationService?.SetRobotSpeedFromSettings(speedCmPerSec);

                // Update ViewModel robot state
                if (_viewModel?.RobotState != null)
                {
                    _viewModel.RobotState.Speed = speedCmPerSec;
                }

                System.Diagnostics.Debug.WriteLine($"  - Robot speed applied: {speedCmPerSec:F1} cm/s");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"  - ERROR applying robot speed: {ex.Message}");
            }

            // ========== STEP 3: Apply robot dimensions to MapControl ==========
            try
            {
                double widthCm = _currentRobot.Appearance.Width;
                double lengthCm = _currentRobot.Appearance.Length;
                double heightCm = _currentRobot.Appearance.Height;

                mapControl.SetRobotDimensions((int)widthCm, (int)lengthCm, (int)heightCm);

                System.Diagnostics.Debug.WriteLine($"  - Robot dimensions applied: W={widthCm:F1}cm, L={lengthCm:F1}cm, H={heightCm:F1}cm");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"  - ERROR applying robot dimensions: {ex.Message}");
            }

            // ========== STEP 4: Apply robot dimensions to SimulationService for collision detection ==========
            try
            {
                double widthCm = _currentRobot.Appearance.Width;
                double lengthCm = _currentRobot.Appearance.Length;

                _simulationService?.SetRobotDimensions(widthCm, lengthCm);

                System.Diagnostics.Debug.WriteLine($"  - Robot dimensions sent to SimulationService: W={widthCm:F1}cm, L={lengthCm:F1}cm");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"  - ERROR setting robot dimensions in SimulationService: {ex.Message}");
            }

            // ========== STEP 5: Apply detection parameters from robot sensors ==========
            try
            {
                // Get the first enabled sensor's FOV and range, or use defaults
                double viewAngle = 180.0;
                int detectionRange = 3;

                var firstSensor = _currentRobot.Sensors.FirstOrDefault(s => s.IsEnabled);
                if (firstSensor != null)
                {
                    viewAngle = firstSensor.FieldOfView;
                    // Rough conversion from cm to cells (assuming 10cm per cell)
                    detectionRange = (int)(firstSensor.MaxRange / 10.0);
                    detectionRange = Math.Max(1, Math.Min(10, detectionRange));
                }

                _simulationService?.SetDetectionParameters(viewAngle, detectionRange, true);

                System.Diagnostics.Debug.WriteLine($"  - Detection params applied: Angle={viewAngle:F0}°, Range={detectionRange} cells");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"  - ERROR applying detection parameters: {ex.Message}");
            }

            // ========== STEP 6: Update RobotPanel UI controls ==========
            try
            {
                if (robotPanel != null)
                {
                    // Update speed control
                    var nudSpeed = robotPanel.Controls.Find("_nudSpeed", true).FirstOrDefault() as NumericUpDown;
                    if (nudSpeed != null)
                    {
                        nudSpeed.Value = (decimal)(_currentRobot.Kinematics.MaxForwardSpeed * 100);
                    }

                    // Update width control
                    var nudWidth = robotPanel.Controls.Find("_nudWidth", true).FirstOrDefault() as NumericUpDown;
                    if (nudWidth != null)
                    {
                        nudWidth.Value = (decimal)_currentRobot.Appearance.Width;
                    }

                    // Update length control
                    var nudLength = robotPanel.Controls.Find("_nudLength", true).FirstOrDefault() as NumericUpDown;
                    if (nudLength != null)
                    {
                        nudLength.Value = (decimal)_currentRobot.Appearance.Length;
                    }

                    // Trigger external dimensions update
                    robotPanel.UpdateDimensionsExternally(
                        (int)_currentRobot.Appearance.Width,
                        (int)_currentRobot.Appearance.Length,
                        (int)_currentRobot.Appearance.Height);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"  - ERROR updating RobotPanel: {ex.Message}");
            }

            // ========== STEP 7: Update ViewModel with current robot ==========
            try
            {
                _viewModel?.SetCurrentRobot(_currentRobot);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"  - ERROR updating ViewModel: {ex.Message}");
            }

            // ========== STEP 8: Update status bar ==========
            if (lblStatus != null)
            {
                lblStatus.Text = $"Robot: {_currentRobot.RobotName} | Type: {_currentRobot.RobotType} | Speed: {_currentRobot.Kinematics.MaxForwardSpeed:F1} m/s | Sensors: {_currentRobot.Sensors.Count} | Custom drawing: ON";
            }

            System.Diagnostics.Debug.WriteLine("=== ApplySelectedRobot completed successfully ===");
        }
        #endregion
    }
}
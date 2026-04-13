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
using SallamPathFinder4.Services.Simulation;
using SallamPathFinder4.WinForms.Container;
using SallamPathFinder4.WinForms.Controls;
using SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard;
using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentBrowser;
using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentDesigner;
using SallamPathFinder4.WinForms.Forms.Settings.frmMapSettings;
using SallamPathFinder4.WinForms.Forms.Settings.frmObstacleSettings;
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
        #endregion

        #region Constructor
        public frmEnvironment()
        {
            try
            {
                CreateMenuAndToolbar();
                InitializeComponent();
                InitializeCustomComponents();
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
            mapControl.RobotAngle = 0;
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
            }

            if (algorithmSettingsPanel != null)
            {
                algorithmSettingsPanel.FindPathRequested += (s, e) => _viewModel.FindPathAsync();
                algorithmSettingsPanel.SettingsChanged += (s, e) => UpdateAlgorithmSettings();
            }
            // Dynamic Charging Events
            WireChargingEvents();

            mapControl.MouseClick += MapControl_MouseClick;
            mapControl.MouseMove += MapControl_MouseMove;
            robotPanel.BatteryLevelChanged += (s, e) => UpdateBatteryFromPanel();
        }

        private void UpdateAlgorithmSettings()
        {
            if (_viewModel == null || algorithmSettingsPanel == null) return;

            _viewModel.SelectedAlgorithm = algorithmSettingsPanel.CurrentAlgorithm;
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
            lblRobotPos.Text = $"Robot: ({_viewModel.RobotState.Position.X},{_viewModel.RobotState.Position.Y}) {_viewModel.RobotState.Angle:F0}°";
            mapControl.UpdateRobotPosition(_viewModel.RobotState.Position, _viewModel.RobotState.Angle);
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
            if (InvokeRequired)
            {
                Invoke(new Action(DisplayPath));
                return;
            }

            var path = _viewModel.CurrentPathResult?.Path;
            if (path == null || path.Count == 0) return;

            pathDisplayPanel?.ClearPath();
            int step = 1;
            foreach (var node in path)
            {
                pathDisplayPanel?.AddPathStep(step++, node.X, node.Y, "Main", Color.Gold);
            }
            pathDisplayPanel?.UpdateStats(path.Count, _viewModel.CurrentPathResult.ComputationTimeSeconds * 1000, path.Count * 10.0);
            lblAlgoTime.Text = $"⏱️ Algo: {_viewModel.CurrentPathResult.ComputationTimeSeconds * 1000:F2}ms";
            mapControl.DrawPath(path.ToList(), Color.Gold);
            lblStatus.Text = $"🟢 Path found! Length: {path.Count} cells";
        }
        #endregion

        #region Goals and Parking Methods
        private void StartAddingGoal()
        {
            CancelCurrentDrawMode();
            _isAddingGoal = true;
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
            _viewModel.Goals.Clear();
            foreach (var goal in mapControl.Goals)
                _viewModel.Goals.Add(goal);
            goalsPanel?.RefreshList();
            _viewModel.RefreshHasGoals();

            if (_simulationService != null)
            {
                var goalsList = _viewModel.Goals.Select(g => g.Location).ToList();
                _simulationService.SetGoals(goalsList);
            }
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
            if (!mapControl.MapGrid.IsValidCoordinate(cell.X, cell.Y)) return;

            if (_isAddingGoal)
            {
                var color = Color.FromArgb(_random.Next(100, 255), _random.Next(100, 255), _random.Next(100, 255));
                mapControl.AddGoalAt(cell, color);
                RefreshGoalsList();
                _isAddingGoal = false;
                mapControl.Cursor = Cursors.Default;
                mapControl.CurrentDrawMode = MapControl.DrawMode.None;
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

        #region Form Events
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopDetectionZoneUpdater();
            base.OnFormClosing(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                CancelCurrentDrawMode();
                return true;
            }

            switch (keyData)
            {
                case Keys.W:
                case Keys.S:
                case Keys.A:
                case Keys.D:
                case Keys.Q:
                case Keys.E:
                    mapControl.MoveRobotManually(keyData);
                    _viewModel.RobotState.Position = mapControl.RobotPosition;
                    _viewModel.RobotState.Angle = mapControl.RobotAngle;
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

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
    }
}
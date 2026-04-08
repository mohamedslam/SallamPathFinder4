#region File Header
/// <summary>
/// File: frmEnvironment.cs
/// Description: Main environment form for robot pathfinding simulation
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-08
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Goals;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.WinForms.Container;
using SallamPathFinder4.WinForms.Controls;
using SallamPathFinder4.WinForms.Panels;
using SallamPathFinder4.WinForms.ViewModels;
using SallamPathFinder4.Services.Simulation;
using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentDesigner;
using SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard;
using SallamPathFinder4.WinForms.Forms.Settings.frmMapSettings;
using SallamPathFinder4.WinForms.Forms.Settings.frmObstacleSettings;
using SallamPathFinder4.WinForms.Forms.Environment;
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
        private Random _random;
        private ISimulationService _simulationService;
        private EnvironmentLogic _logic;
        private EnvironmentUI _ui;
        private EnvironmentInitializer _initializer;
        private EnvironmentEventHandlers _handlers;
        private EnvironmentMapOperations _mapOps;
        private EnvironmentDialogs _dialogs;
        //internal  ToolStripMenuItem robotToolbarDashboardMenuItem;
        //internal  ToolStripMenuItem robotToolbarCreateMenuItem;
        //internal System.Windows.Forms.ToolStripMenuItem robotToolbarManageMenuItem;
        #endregion

        #region Constructor


        public frmEnvironment()
        {
            try
            { 
                InitializeComponent();
                IntialMapControl();
                CreatMenuWeight();                
                InitializeCustomComponents();
                InitializeEnvironmentModules();
                 WireAllEvents();
                StartDetectionZoneUpdater();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing environment: {ex.Message}\n\n{ex.StackTrace}",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        internal void CreatMenuWeight() { 
            //robotToolbarDashboardMenuItem=new ToolStripMenuItem();
            //robotToolbarCreateMenuItem=new ToolStripMenuItem();
            //robotToolbarManageMenuItem=new ToolStripMenuItem();
        for (int weight = 0; weight <= 100; weight += 10)
                {
                    var item = new ToolStripMenuItem($"{weight}%");
                    int intensity = 255 - (int)((weight / 100.0) * 255);
                    item.BackColor = Color.FromArgb(intensity, intensity, intensity);
                    weightMenu.DropDownItems.Add(item);
                } 
        }
        internal void IntialMapControl()
        {

            mapControl = new MapControl();
            tlpMapArea.Controls.Add(mapControl, 1, 1);

            mapControl.BackColor = Color.White;
            mapControl.CellSize = DEFAULT_CELL_SIZE;
            mapControl.Dock = DockStyle.Fill;
            mapControl.RobotAngle = 0F;
            mapControl.RobotPosition = new Point(10, 10);
            mapControl.ShowCoordinates = false;
            mapControl.ShowGrid = true;
            mapControl.ShowRobot = true;
            mapControl.ZoomLevel = 1F;

        }
        #endregion

        #region Initialization Methods
        private void InitializeCustomComponents()
        {
            _random = new Random();
            ConfigureForm();
            InitializeMapGrid();
            InitializeServicesAndViewModel();
            InitializePanels();
            BindViewModel();
        }

        private void InitializeEnvironmentModules()
        {
            _logic = new EnvironmentLogic();
            _initializer = new EnvironmentInitializer(this, mapControl, _logic);
            _initializer.Initialize();

            _ui = new EnvironmentUI(this, mapControl, _initializer.ViewModel, _logic);
            _mapOps = new EnvironmentMapOperations(this, mapControl, _initializer.ViewModel,
                _initializer.SimulationService, _logic, _ui);
            _dialogs = new EnvironmentDialogs(this, mapControl, _initializer.ViewModel, _logic, _ui);
            _handlers = new EnvironmentEventHandlers(this, mapControl, _initializer.ViewModel,
                _initializer.SimulationService, _logic, _ui);
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
            mapControl.ScaleCmPerCell = 10.0;
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
            if (goalsPanel != null && _viewModel.Goals != null)
            {
                goalsPanel = new GoalsPanel(_viewModel.Goals);
                tabGoalsParking.Controls.Add(goalsPanel);
            }

            if (parkingPanel != null && _viewModel.ParkingPoints != null)
            {
                parkingPanel = new ParkingPanel(_viewModel.ParkingPoints);
                tabGoalsParking.Controls.Add(parkingPanel);
            }

            if (obstacleLogPanel != null && _viewModel.ObstacleLog != null)
            {
                obstacleLogPanel = new ObstacleLogPanel(_viewModel.ObstacleLog);
                tabObstacleLog.Controls.Add(obstacleLogPanel);
            }

            // Set panel docks
            if (robotPanel != null) robotPanel.Dock = DockStyle.Top;
            if (algorithmSettingsPanel != null) algorithmSettingsPanel.Dock = DockStyle.Fill;
            if (goalsPanel != null) goalsPanel.Dock = DockStyle.Top;
            if (parkingPanel != null) parkingPanel.Dock = DockStyle.Fill;
            if (pathDisplayPanel != null) pathDisplayPanel.Dock = DockStyle.Fill;
            if (obstacleLogPanel != null) obstacleLogPanel.Dock = DockStyle.Fill;
        }

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
        #endregion

        #region Event Wiring 
        private void WireAllEvents()
        {
             

            // File Menu
            if (newMapMenuItem != null) newMapMenuItem.Click += (s, e) => _mapOps.NewMap();
            if (openMapMenuItem != null) openMapMenuItem.Click += async (s, e) => await _mapOps.OpenMap();
            if (saveMapMenuItem != null) saveMapMenuItem.Click += async (s, e) => await _mapOps.SaveMap();
            if (exitMenuItem != null) exitMenuItem.Click += (s, e) => Application.Exit();

            // View Menu
            if (zoomInMenuItem != null) zoomInMenuItem.Click += (s, e) => { mapControl.ZoomLevel += 0.1f; _ui.UpdateRulers(mapControl.CellSize, mapControl.ZoomLevel); };
            if (zoomOutMenuItem != null) zoomOutMenuItem.Click += (s, e) => { mapControl.ZoomLevel -= 0.1f; _ui.UpdateRulers(mapControl.CellSize, mapControl.ZoomLevel); };
            if (zoomResetMenuItem != null) zoomResetMenuItem.Click += (s, e) => _mapOps.ResetView();
            if (mapSettingsMenuItem != null) mapSettingsMenuItem.Click += (s, e) => _mapOps.ShowMapSettings();
            if (showGridItem != null) showGridItem.Click += (s, e) => _mapOps.ToggleGrid();
            if (showCoordsItem != null) showCoordsItem.Click += (s, e) => _mapOps.ToggleCoordinates();

            // Robot Menu
            if (robotDashboardMenuItem != null) robotDashboardMenuItem.Click += (s, e) => _dialogs.ShowRobotDashboard();
            if (createRobotMenuItem != null) createRobotMenuItem.Click += (s, e) => _dialogs.ShowRobotCreator();
            if (manageRobotsMenuItem != null) manageRobotsMenuItem.Click += (s, e) => _dialogs.ShowRobotManager();
            if (robotSettingsMenuItem != null) robotSettingsMenuItem.Click += (s, e) => _dialogs.ShowRobotSettings();
            if (exportRobotMenuItem != null) exportRobotMenuItem.Click += (s, e) => _dialogs.ExportRobotProfile();

            // Experiments Menu
            if (experimentDesignerMenuItem != null) experimentDesignerMenuItem.Click += (s, e) => _dialogs.ShowExperimentDesigner();
            if (experimentResultsMenuItem != null) experimentResultsMenuItem.Click += (s, e) => _dialogs.ShowExperimentViewer();

            // Help Menu
            if (helpContentMenuItem != null) helpContentMenuItem.Click += (s, e) => _dialogs.ShowHelp();
            if (keyboardShortcutsMenuItem != null) keyboardShortcutsMenuItem.Click += (s, e) => _dialogs.ShowKeyboardShortcuts();
            if (documentationMenuItem != null) documentationMenuItem.Click += (s, e) => _dialogs.ShowDocumentation();
            if (checkUpdatesMenuItem != null) checkUpdatesMenuItem.Click += (s, e) => _dialogs.CheckForUpdates();
            if (aboutMenuItem != null) aboutMenuItem.Click += (s, e) => _dialogs.ShowAboutDialog();

            // Toolbar
            if (btnFindPath != null) btnFindPath.Click += async (s, e) => await _initializer.ViewModel.FindPathAsync();

            // Test Menu
            if (testAllMenuItem != null) testAllMenuItem.Click += async (s, e) => await TestAllAlgorithmsAsync();
            if (testAStarMenuItem != null) testAStarMenuItem.Click += async (s, e) => await TestSingleAlgorithmAsync(AlgorithmType.AStar);
            if (testSPPAMenuItem != null) testSPPAMenuItem.Click += async (s, e) => await TestSingleAlgorithmAsync(AlgorithmType.SPPA);
            if (testSPPA_DLMenuItem != null) testSPPA_DLMenuItem.Click += async (s, e) => await TestSingleAlgorithmAsync(AlgorithmType.SPPA_DL);
            if (clearTestResultsMenuItem != null) clearTestResultsMenuItem.Click += (s, e) => ClearTestResults();

            // Obstacle Menu
            if (wallMenuItem != null) wallMenuItem.Click += (s, e) => SetStaticElement(MapElementType.Wall);
            if (rampMenuItem != null) rampMenuItem.Click += (s, e) => SetStaticElement(MapElementType.Ramp);
            if (doorMenuItem != null) doorMenuItem.Click += (s, e) => SetStaticElement(MapElementType.Door);
            if (windowMenuItem != null) windowMenuItem.Click += (s, e) => SetStaticElement(MapElementType.Window);
            if (adultMenuItem != null) adultMenuItem.Click += (s, e) => SetDynamicObstacleType(ObstacleType.Adult);
            if (childMenuItem != null) childMenuItem.Click += (s, e) => SetDynamicObstacleType(ObstacleType.Child);
            if (animalMenuItem != null) animalMenuItem.Click += (s, e) => SetDynamicObstacleType(ObstacleType.Animal);
             
            if (equipmentMenuItem != null) equipmentMenuItem.Click += (s, e) => SetDynamicObstacleType(ObstacleType.Equipment);
            if (clearAllObstaclesMenuItem != null) clearAllObstaclesMenuItem.Click += (s, e) => _mapOps.ClearAllObstacles();
            if (obstacleSettingsMenuItem != null) obstacleSettingsMenuItem.Click += (s, e) => _dialogs.ShowObstacleSettings();

            // Surface Weights
            if (weightMenu != null)
            {
                for (int i = 0; i < weightMenu.DropDownItems.Count; i++)
                {
                    int weight = i * 10;
                    weightMenu.DropDownItems[i].Click += (s, e) => SetSurfaceWeight((byte)weight);
                }
            }

            // Robot Toolbar Menu
            //if (robotToolbarDashboardMenuItem != null) robotToolbarDashboardMenuItem.Click += (s, e) => _dialogs.ShowRobotDashboard();
            //if (robotToolbarCreateMenuItem != null) robotToolbarCreateMenuItem.Click += (s, e) => _dialogs.ShowRobotCreator();
            //if (robotToolbarManageMenuItem != null) robotToolbarManageMenuItem.Click += (s, e) => _dialogs.ShowRobotManager();
 
            // Panels
            if (robotPanel != null)
            {
                robotPanel.SimulateClick += (s, e) => _initializer.ViewModel.StartSimulation();
                robotPanel.PauseClick += (s, e) => _initializer.ViewModel.TogglePause();
                robotPanel.StopClick += (s, e) => _initializer.ViewModel.StopSimulation();
            }

            if (algorithmSettingsPanel != null)
            {
                algorithmSettingsPanel.FindPathRequested += (s, e) => _initializer.ViewModel.FindPathAsync();
            }

            // Map Events
            if (mapControl != null)
            {
                mapControl.MouseClick += MapControl_MouseClick;
                mapControl.MouseMove += MapControl_MouseMove;
            }
        }
        #endregion

        #region Update Methods
        private void UpdateRobotPositionDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateRobotPositionDisplay));
                return;
            }
            if (lblRobotPos != null && _viewModel != null)
            {
                lblRobotPos.Text = $"Robot: ({_viewModel.RobotState.Position.X},{_viewModel.RobotState.Position.Y}) {_viewModel.RobotState.Angle:F0}°";
            }
            if (mapControl != null && _viewModel != null)
            {
                mapControl.UpdateRobotPosition(_viewModel.RobotState.Position, _viewModel.RobotState.Angle);
            }
        }

        private void UpdateBatteryDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateBatteryDisplay));
                return;
            }
            if (lblBattery != null && _viewModel != null)
            {
                lblBattery.Text = $"🔋 Battery: {_viewModel.RobotState.BatteryLevel:F1}%";
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

            var path = _viewModel?.CurrentPathResult?.Path;
            if (path == null || path.Count == 0) return;

            pathDisplayPanel?.ClearPath();
            int step = 1;
            foreach (var node in path)
            {
                pathDisplayPanel?.AddPathStep(step++, node.X, node.Y, "Main", Color.Gold);
            }
            pathDisplayPanel?.UpdateStats(path.Count, _viewModel.CurrentPathResult.ComputationTimeSeconds * 1000, path.Count * 10.0);
            if (lblAlgoTime != null) lblAlgoTime.Text = $"⏱️ Algo: {_viewModel.CurrentPathResult.ComputationTimeSeconds * 1000:F2}ms";
            mapControl?.DrawPath(path.ToList(), Color.Gold);
            if (lblStatus != null) lblStatus.Text = $"🟢 Path found! Length: {path.Count} cells";
        }
        #endregion

        #region Drawing Mode Methods
        private void SetStaticElement(MapElementType element)
        {
            CancelCurrentDrawMode();
            if (mapControl != null)
            {
                mapControl.CurrentDrawMode = MapControl.DrawMode.SetElement;
                mapControl.CurrentElement = element;
            }
            if (lblStatus != null) lblStatus.Text = $"🟡 Click on map to add {element} (Press ESC to cancel)";
        }

        private void SetDynamicObstacleType(ObstacleType type)
        {
            CancelCurrentDrawMode();
            if (mapControl != null)
            {
                mapControl.CurrentDrawMode = MapControl.DrawMode.SetDynamicObstacle;
                mapControl.CurrentObstacleType = type;
            }
            if (lblStatus != null) lblStatus.Text = $"🟡 Click on map to add {type} obstacle (Press ESC to cancel)";
        }

        private void SetSurfaceWeight(byte weight)
        {
            CancelCurrentDrawMode();
            if (mapControl != null)
            {
                mapControl.CurrentDrawMode = MapControl.DrawMode.SetWeight;
                mapControl.CurrentWeight = weight;
            }
            if (lblStatus != null) lblStatus.Text = $"📊 Click on map to set {weight}% surface weight (Press ESC to cancel)";
        }

        private void CancelCurrentDrawMode()
        {
            SetAddingGoal(false);
            SetAddingParking(false);
            ClearMovingGoal();
            ClearMovingParking();

            if (mapControl != null)
            {
                mapControl.CurrentDrawMode = MapControl.DrawMode.None;
                mapControl.Cursor = Cursors.Default;
            }

            if (lblStatus != null) lblStatus.Text = "🟢 Operation cancelled. Ready";
        }
        #endregion

        #region Goals and Parking Methods
        private void RefreshGoalsList()
        {
            _ui?.RefreshGoalsList();
        }

        private void RefreshParkingList()
        {
            _ui?.RefreshParkingList();
        }
        #endregion

        #region Detection Zone Methods
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

        #region Testing Methods
        private async Task TestAllAlgorithmsAsync()
        {
            if (mapControl == null || _viewModel == null) return;

            var start = mapControl.RobotPosition;
            var end = _viewModel.Goals.Count > 0 ? _viewModel.Goals[0].Location : new Point(50, 50);

            if (lblStatus != null) lblStatus.Text = "🧪 Testing all algorithms...";
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
            if (lblStatus != null) lblStatus.Text = "✅ Algorithm testing completed";
        }

        private async Task TestSingleAlgorithmAsync(AlgorithmType type)
        {
            if (mapControl == null || _viewModel == null) return;

            var start = mapControl.RobotPosition;
            var end = _viewModel.Goals.Count > 0 ? _viewModel.Goals[0].Location : new Point(50, 50);

            if (lblStatus != null) lblStatus.Text = $"🧪 Testing {type}...";
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

            if (lblStatus != null) lblStatus.Text = $"✅ {type} testing completed";
        }

        private void ClearTestResults()
        {
            mapControl?.ClearPaths();
            pathDisplayPanel?.ClearPath();
            if (lblStatus != null) lblStatus.Text = "Test results cleared";
        }
        #endregion

        #region Map Event Handlers
        private void MapControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (mapControl == null || mapControl.MapGrid == null) return;

            var cell = mapControl.GetGridCellAtPoint(e.Location);
            if (!mapControl.MapGrid.IsValidCoordinate(cell.X, cell.Y)) return;

            if (IsAddingGoal)
            {
                var color = Color.FromArgb(_random.Next(100, 255), _random.Next(100, 255), _random.Next(100, 255));
                mapControl.AddGoalAt(cell, color);
                RefreshGoalsList();
                SetAddingGoal(false);
                mapControl.Cursor = Cursors.Default;
                mapControl.CurrentDrawMode = MapControl.DrawMode.None;
                if (lblStatus != null) lblStatus.Text = "🟢 Ready";
                return;
            }

            if (IsAddingParking)
            {
                mapControl.AddParkingAt(cell);
                RefreshParkingList();
                SetAddingParking(false);
                mapControl.Cursor = Cursors.Default;
                mapControl.CurrentDrawMode = MapControl.DrawMode.None;
                if (lblStatus != null) lblStatus.Text = "🟢 Ready";
                return;
            }

            if (IsMovingGoal && MovingGoal != null)
            {
                mapControl.RemoveGoalAt(MovingGoal.Location);
                MovingGoal.Location = cell;
                mapControl.AddGoalAt(cell, MovingGoal.Color);
                RefreshGoalsList();
                ClearMovingGoal();
                mapControl.Cursor = Cursors.Default;
                mapControl.CurrentDrawMode = MapControl.DrawMode.None;
                if (lblStatus != null) lblStatus.Text = "🟢 Ready";
                return;
            }

            if (IsMovingParking && MovingParking != null)
            {
                mapControl.RemoveParkingAt(MovingParking.Location);
                MovingParking.Location = cell;
                mapControl.AddParkingAt(cell);
                RefreshParkingList();
                ClearMovingParking();
                mapControl.Cursor = Cursors.Default;
                mapControl.CurrentDrawMode = MapControl.DrawMode.None;
                if (lblStatus != null) lblStatus.Text = "🟢 Ready";
                return;
            }

            if (mapControl.CurrentDrawMode == MapControl.DrawMode.SetDynamicObstacle)
            {
                mapControl.CurrentDrawMode = MapControl.DrawMode.None;
                mapControl.Cursor = Cursors.Default;
                if (lblStatus != null) lblStatus.Text = "🟢 Dynamic obstacle added. Ready";
                return;
            }

            if (lblCellPos != null) lblCellPos.Text = $"Cell: ({cell.X},{cell.Y})";
            if (lblStatus != null) lblStatus.Text = $"Cell ({cell.X},{cell.Y}) | Walkable: {mapControl.MapGrid[cell.X, cell.Y].IsWalkable}";
        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mapControl == null || mapControl.MapGrid == null) return;

            var cell = mapControl.GetGridCellAtPoint(e.Location);
            if (mapControl.MapGrid.IsValidCoordinate(cell.X, cell.Y))
            {
                if (lblMousePos != null) lblMousePos.Text = $"Mouse: ({e.X},{e.Y})";
                if (lblCellPos != null) lblCellPos.Text = $"Cell: ({cell.X},{cell.Y})";
                if (lblRealPos != null)
                {
                    double realX = cell.X * mapControl.ScaleCmPerCell;
                    double realY = cell.Y * mapControl.ScaleCmPerCell;
                    lblRealPos.Text = $"Real: ({realX:F1}cm, {realY:F1}cm)";
                }
            }
        }
        #endregion

        #region Form Events
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                StopDetectionZoneUpdater();
                if (_initializer != null && _initializer.SimulationService != null)
                {
                    _initializer.SimulationService.Stop();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnFormClosing error: {ex.Message}");
            }
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
                    if (mapControl != null && _viewModel != null)
                    {
                        mapControl.MoveRobotManually(keyData);
                        _viewModel.RobotState.Position = mapControl.RobotPosition;
                        _viewModel.RobotState.Angle = mapControl.RobotAngle;
                    }
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion
    }
}
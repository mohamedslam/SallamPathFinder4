#region File Header
/// <summary>
/// File: EnvironmentInitializer.cs
/// Description: Initialization logic for main environment form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-08
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Goals;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.WinForms.Container;
using SallamPathFinder4.WinForms.Controls;
using SallamPathFinder4.WinForms.Panels;
using SallamPathFinder4.WinForms.ViewModels;
using SallamPathFinder4.Services.Simulation;
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Environment
{
    public sealed class EnvironmentInitializer
    {
        #region Private Fields
        private readonly frmEnvironment _form;
        private readonly frmEnvironmentCopy _form1;
        private readonly MapControl _mapControl;
        private readonly EnvironmentLogic _logic;
        private MainViewModel _viewModel;
        private ISimulationService _simulationService;
        private MapGrid _mapGrid;
        #endregion

        #region Constructor
        public EnvironmentInitializer(frmEnvironment form, MapControl mapControl, EnvironmentLogic logic)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _mapControl = mapControl ?? throw new ArgumentNullException(nameof(mapControl));
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }
        public EnvironmentInitializer(frmEnvironmentCopy form, MapControl mapControl, EnvironmentLogic logic)
        {
            _form1 = form ?? throw new ArgumentNullException(nameof(form));
            _mapControl = mapControl ?? throw new ArgumentNullException(nameof(mapControl));
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }
        #endregion

        #region Properties
        public MainViewModel ViewModel => _viewModel;
        public ISimulationService SimulationService => _simulationService;
        public MapGrid MapGrid => _mapGrid;
        #endregion

        #region Public Methods
        public void Initialize()
        {
            InitializeMapGrid();
            InitializeServicesAndViewModel();
            InitializePanels();
            SyncRobotPosition();
        }
        #endregion

        #region Private Methods - Map Grid
        private void InitializeMapGrid()
        {
            _mapGrid = new MapGrid(
                EnvironmentLogic.DEFAULT_GRID_WIDTH,
                EnvironmentLogic.DEFAULT_GRID_HEIGHT);

            // Set default start position
            _mapGrid[10, 10].ElementType = MapElementType.StartPoint;
            _mapGrid.UpdateAllCellProperties();

            _mapControl.MapGrid = _mapGrid;
            _mapControl.RobotPosition = new Point(10, 10);
            _mapControl.RobotAngle = 0;
            _mapControl.ScaleCmPerCell = 10.0;
        }
        #endregion

        #region Private Methods - Services and ViewModel
        private void InitializeServicesAndViewModel()
        {
            // Create services
            var pathfindingService = ServiceContainer.CreatePathfindingService(_mapGrid);
            var batteryService = ServiceContainer.Resolve<IBatteryService>();
            var simulationService = ServiceContainer.CreateSimulationService(
                _mapGrid, _mapControl.DynamicObstacles);
            var fileService = ServiceContainer.Resolve<IFileService>();
            var experimentService = ServiceContainer.Resolve<IExperimentService>();

            // Store simulation service
            _simulationService = simulationService;

            // Create ViewModel
            _viewModel = new MainViewModel(
                pathfindingService,
                simulationService,
                batteryService,
                fileService,
                experimentService,
                _mapGrid,
                _mapControl,
                _form);

            // Set initial goals if any
            var goalsList = _viewModel.Goals?.Select(g => g.Location).ToList() ?? new List<Point>();
            simulationService.SetGoals(goalsList);
        }

        private void SyncRobotPosition()
        {
            if (_viewModel != null)
            {
                _viewModel.RobotState.Position = _mapControl.RobotPosition;
                _viewModel.RobotState.Angle = _mapControl.RobotAngle;
                _viewModel.RobotState.BatteryLevel = 100;
                _viewModel.RobotState.Speed = 10;
            }
        }
        #endregion

        #region Private Methods - Panels
        private void InitializePanels()
        {
            InitializeRobotPanel();
            InitializeAlgorithmSettingsPanel();
            InitializeGoalsPanel();
            InitializeParkingPanel();
            InitializePathDisplayPanel();
            InitializeObstacleLogPanel();

            ConfigureTabs();
        }

        private void InitializeRobotPanel()
        {
            if (_form.robotPanel == null)
            {
                _form.robotPanel = new RobotPanel();
            }
        }

        private void InitializeAlgorithmSettingsPanel()
        {
            if (_form.algorithmSettingsPanel == null)
            {
                _form.algorithmSettingsPanel = new AlgorithmSettingsPanel();
            }
        }

        private void InitializeGoalsPanel()
        {
            if (_form.goalsPanel == null && _viewModel.Goals != null)
            {
                _form.goalsPanel = new GoalsPanel(_viewModel.Goals);
            }
        }

        private void InitializeParkingPanel()
        {
            if (_form.parkingPanel == null && _viewModel.ParkingPoints != null)
            {
                _form.parkingPanel = new ParkingPanel(_viewModel.ParkingPoints);
            }
        }

        private void InitializePathDisplayPanel()
        {
            if (_form.pathDisplayPanel == null)
            {
                _form.pathDisplayPanel = new PathDisplayPanel();
            }
        }

        private void InitializeObstacleLogPanel()
        {
            if (_form.obstacleLogPanel == null && _viewModel.ObstacleLog != null)
            {
                _form.obstacleLogPanel = new ObstacleLogPanel(_viewModel.ObstacleLog);
            }
        }
        #endregion

        #region Private Methods - Tab Configuration
        private void ConfigureTabs()
        {
            ConfigureAlgorithmRobotTab();
            ConfigureGoalsParkingTab();
            ConfigurePathResultsTab();
            ConfigureObstacleLogTab();
        }

        private void ConfigureAlgorithmRobotTab()
        {
            if (_form.tabAlgorithmRobot == null) return;

            var algoLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(5)
            };
            algoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            algoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            algoLayout.Controls.Add(_form.robotPanel, 0, 0);
            algoLayout.Controls.Add(_form.algorithmSettingsPanel, 0, 1);

            _form.tabAlgorithmRobot.Controls.Clear();
            _form.tabAlgorithmRobot.Controls.Add(algoLayout);
        }

        private void ConfigureGoalsParkingTab()
        {
            if (_form.tabGoalsParking == null) return;

            var goalsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(5)
            };
            goalsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            goalsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            goalsLayout.Controls.Add(_form.goalsPanel, 0, 0);
            goalsLayout.Controls.Add(_form.parkingPanel, 0, 1);

            _form.tabGoalsParking.Controls.Clear();
            _form.tabGoalsParking.Controls.Add(goalsLayout);
        }

        private void ConfigurePathResultsTab()
        {
            if (_form.tabPathResults == null) return;

            _form.tabPathResults.Controls.Clear();
            _form.tabPathResults.Controls.Add(_form.pathDisplayPanel);
        }

        private void ConfigureObstacleLogTab()
        {
            if (_form.tabObstacleLog == null) return;

            _form.tabObstacleLog.Controls.Clear();
            _form.tabObstacleLog.Controls.Add(_form.obstacleLogPanel);
        }
        #endregion
    }
}
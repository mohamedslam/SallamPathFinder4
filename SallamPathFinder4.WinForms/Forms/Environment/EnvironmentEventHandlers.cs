//#region File Header
///// <summary>
///// File: EnvironmentEventHandlers.cs
///// Description: Event handlers for main environment form
///// Author: Mohamed ElSayed Sallam
///// Date: 2026-04-08
///// </summary>
//#endregion

//#region Namespace Imports
//using System;
//using System.Drawing;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using SallamPathFinder4.Core.Enums;
//using SallamPathFinder4.Core.Interfaces.Services;
//using SallamPathFinder4.Core.Models.Goals;
//using SallamPathFinder4.Core.Models.Obstacles;
//using SallamPathFinder4.Services.Simulation;
//using SallamPathFinder4.WinForms.Controls;
//using SallamPathFinder4.WinForms.ViewModels;
//#endregion

//namespace SallamPathFinder4.WinForms.Forms.Environment
//{
//    public sealed class EnvironmentEventHandlers
//    {
//        #region Private Fields
//        private readonly frmEnvironment _form;
//        private readonly MapControl _mapControl;
//        private readonly MainViewModel _viewModel;
//        private readonly ISimulationService _simulationService;
//        private readonly EnvironmentLogic _logic;
//        private readonly EnvironmentUI _ui;
//        #endregion

//        #region Constructor
//        public EnvironmentEventHandlers(
//            frmEnvironment form,
//            MapControl mapControl,
//            MainViewModel viewModel,
//            ISimulationService simulationService,
//            EnvironmentLogic logic,
//            EnvironmentUI ui)
//        {
//            _form = form ?? throw new ArgumentNullException(nameof(form));
//            _mapControl = mapControl ?? throw new ArgumentNullException(nameof(mapControl));
//            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
//            _simulationService = simulationService ?? throw new ArgumentNullException(nameof(simulationService));
//            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
//            _ui = ui ?? throw new ArgumentNullException(nameof(ui));
//        }
//        #endregion

//        #region Public Methods - Wire All Events
//        public void WireAllEvents()
//        {
//            WireGoalPanelEvents();
//            WireParkingPanelEvents();
//            WireRobotPanelEvents();
//            WireAlgorithmPanelEvents();
//            WireMapEvents();
//            WireSimulationEvents();
//            WireViewModelEvents();
//            WireFormEvents();
//        }
//        #endregion

//        #region Private Methods - Panel Events
//        private void WireGoalPanelEvents()
//        {
//            if (_form.goalsPanel == null) return;

//            _form.goalsPanel.GoalAddRequested += OnGoalAddRequested;
//            _form.goalsPanel.GoalRemoveRequested += OnGoalRemoveRequested;
//            _form.goalsPanel.GoalMoveRequested += OnGoalMoveRequested;
//        }

//        private void WireParkingPanelEvents()
//        {
//            if (_form.parkingPanel == null) return;

//            _form.parkingPanel.ParkingAddRequested += OnParkingAddRequested;
//            _form.parkingPanel.ParkingRemoveRequested += OnParkingRemoveRequested;
//            _form.parkingPanel.ParkingMoveRequested += OnParkingMoveRequested;
//        }

//        private void WireRobotPanelEvents()
//        {
//            if (_form.robotPanel == null) return;

//            _form.robotPanel.SimulateClick += OnSimulateClick;
//            _form.robotPanel.PauseClick += OnPauseClick;
//            _form.robotPanel.StopClick += OnStopClick;
//            _form.robotPanel.BatteryLevelChanged += OnBatteryLevelChanged;
//        }

//        private void WireAlgorithmPanelEvents()
//        {
//            if (_form.algorithmSettingsPanel == null) return;

//            _form.algorithmSettingsPanel.FindPathRequested += OnFindPathRequested;
//            _form.algorithmSettingsPanel.SettingsChanged += OnAlgorithmSettingsChanged;
//        }
//        #endregion

//        #region Private Methods - Map Events
//        private void WireMapEvents()
//        {
//            _mapControl.MouseClick += OnMapMouseClick;
//            _mapControl.MouseMove += OnMapMouseMove;
//        }
//        #endregion

//        #region Private Methods - Simulation Events
//        private void WireSimulationEvents()
//        {
//            _simulationService.RobotMoved += OnRobotMoved;
//            _simulationService.ObstacleCollision += OnObstacleCollision;
//            _simulationService.ObstacleDetected += OnObstacleDetected;
//            _simulationService.BatteryChanged += OnBatteryChanged;
//            _simulationService.BatteryEmpty += OnBatteryEmpty;
//            _simulationService.GoalReached += OnGoalReached;
//        }
//        #endregion

//        #region Private Methods - ViewModel Events
//        private void WireViewModelEvents()
//        {
//            if (_viewModel == null) return;

//            _viewModel.RobotState.PropertyChanged += OnRobotStateChanged;
//            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
//        }
//        #endregion

//        #region Private Methods - Form Events
//        private void WireFormEvents()
//        {
//            _form.FormClosing += OnFormClosing;
//            _form.KeyPreview = true;
//            _form.KeyDown += OnFormKeyDown;
//        }
//        #endregion

//        #region Event Handlers - Goal Panel
//        private void OnGoalAddRequested(object sender, Point location)
//        {
//            CancelCurrentDrawMode();
//            _form.SetAddingGoal(true);
//            _mapControl.Cursor = Cursors.Cross;
//            _ui.UpdateStatusText(_logic.GetAddingGoalStatus());
//        }

//        private void OnGoalRemoveRequested(object sender, GoalPoint goal)
//        {
//            _mapControl.RemoveGoalAt(goal.Location);
//            _ui.RefreshGoalsList();
//            _ui.UpdateStatusText(_logic.GetGoalRemovedStatus(goal.Name));
//        }

//        private void OnGoalMoveRequested(object sender, GoalPoint goal)
//        {
//            _form.SetMovingGoal(goal);
//            _mapControl.Cursor = Cursors.Cross;
//            _ui.UpdateStatusText(_logic.GetMovingGoalStatus(goal.Name));
//        }
//        #endregion

//        #region Event Handlers - Parking Panel
//        private void OnParkingAddRequested(object sender, Point location)
//        {
//            CancelCurrentDrawMode();
//            _form.SetAddingParking(true);
//            _mapControl.Cursor = Cursors.Cross;
//            _ui.UpdateStatusText(_logic.GetAddingParkingStatus());
//        }

//        private void OnParkingRemoveRequested(object sender, ParkingPoint parking)
//        {
//            _mapControl.RemoveParkingAt(parking.Location);
//            _ui.RefreshParkingList();
//            _ui.UpdateStatusText(_logic.GetParkingRemovedStatus(parking.Name));
//        }

//        private void OnParkingMoveRequested(object sender, ParkingPoint parking)
//        {
//            _form.SetMovingParking(parking);
//            _mapControl.Cursor = Cursors.Cross;
//            _ui.UpdateStatusText(_logic.GetMovingParkingStatus(parking.Name));
//        }
//        #endregion

//        #region Event Handlers - Robot Panel
//        private async void OnSimulateClick(object sender, EventArgs e)
//        {
//            await Task.Run(() => _viewModel.StartSimulation());
//        }

//        private void OnPauseClick(object sender, EventArgs e)
//        {
//            _viewModel.TogglePause();
//        }

//        private void OnStopClick(object sender, EventArgs e)
//        {
//            _viewModel.StopSimulation();
//        }

//        private void OnBatteryLevelChanged(object sender, EventArgs e)
//        {
//            _ui.UpdateBatteryFromPanel();
//        }
//        #endregion

//        #region Event Handlers - Algorithm Panel
//        private async void OnFindPathRequested(object sender, EventArgs e)
//        {
//            await _viewModel.FindPathAsync();
//        }

//        private void OnAlgorithmSettingsChanged(object sender, EventArgs e)
//        {
//            if (_viewModel == null || _form.algorithmSettingsPanel == null) return;

//            _viewModel.SelectedAlgorithm = _form.algorithmSettingsPanel.CurrentAlgorithm;
//            _viewModel.AllowDiagonals = _form.algorithmSettingsPanel.AllowDiagonals;
//            _viewModel.HeavyDiagonals = _form.algorithmSettingsPanel.HeavyDiagonals;
//            _viewModel.HeuristicWeight = _form.algorithmSettingsPanel.HeuristicWeight;
//            _viewModel.SearchLimit = _form.algorithmSettingsPanel.SearchLimit;
//        }
//        #endregion

//        #region Event Handlers - Map
//        private void OnMapMouseClick(object sender, MouseEventArgs e)
//        {
//            var cell = _mapControl.GetGridCellAtPoint(e.Location);
//            if (!_logic.IsValidGridCell(_mapControl.MapGrid, cell)) return;

//            // Handle goal adding
//            if (_form.IsAddingGoal)
//            {
//                var color = _logic.GetRandomGoalColor();
//                _mapControl.AddGoalAt(cell, color);
//                _ui.RefreshGoalsList();
//                _form.SetAddingGoal(false);
//                _mapControl.Cursor = Cursors.Default;
//                _mapControl.CurrentDrawMode = MapControl.DrawMode.None;
//                _ui.UpdateStatusText(_logic.GetReadyStatus());
//                return;
//            }

//            // Handle parking adding
//            if (_form.IsAddingParking)
//            {
//                _mapControl.AddParkingAt(cell);
//                _ui.RefreshParkingList();
//                _form.SetAddingParking(false);
//                _mapControl.Cursor = Cursors.Default;
//                _mapControl.CurrentDrawMode = MapControl.DrawMode.None;
//                _ui.UpdateStatusText(_logic.GetReadyStatus());
//                return;
//            }

//            // Handle goal moving
//            if (_form.IsMovingGoal && _form.MovingGoal != null)
//            {
//                _mapControl.RemoveGoalAt(_form.MovingGoal.Location);
//                _form.MovingGoal.Location = cell;
//                _mapControl.AddGoalAt(cell, _form.MovingGoal.Color);
//                _ui.RefreshGoalsList();
//                _form.ClearMovingGoal();
//                _mapControl.Cursor = Cursors.Default;
//                _mapControl.CurrentDrawMode = MapControl.DrawMode.None;
//                _ui.UpdateStatusText(_logic.GetReadyStatus());
//                return;
//            }

//            // Handle parking moving
//            if (_form.IsMovingParking && _form.MovingParking != null)
//            {
//                _mapControl.RemoveParkingAt(_form.MovingParking.Location);
//                _form.MovingParking.Location = cell;
//                _mapControl.AddParkingAt(cell);
//                _ui.RefreshParkingList();
//                _form.ClearMovingParking();
//                _mapControl.Cursor = Cursors.Default;
//                _mapControl.CurrentDrawMode = MapControl.DrawMode.None;
//                _ui.UpdateStatusText(_logic.GetReadyStatus());
//                return;
//            }

//            // Handle dynamic obstacle addition
//            if (_mapControl.CurrentDrawMode == MapControl.DrawMode.SetDynamicObstacle)
//            {
//                _mapControl.CurrentDrawMode = MapControl.DrawMode.None;
//                _mapControl.Cursor = Cursors.Default;
//                _ui.UpdateStatusText(_logic.GetReadyStatus());
//                return;
//            }

//            _ui.UpdateCellPositionDisplay(cell);
//        }

//        private void OnMapMouseMove(object sender, MouseEventArgs e)
//        {
//            var cell = _mapControl.GetGridCellAtPoint(e.Location);
//            if (_logic.IsValidGridCell(_mapControl.MapGrid, cell))
//            {
//                _ui.UpdateMousePositionDisplay(e.Location, cell);
//            }
//        }
//        #endregion

//        #region Event Handlers - Simulation
//        private void OnRobotMoved(Point position, float angle)
//        {
//            _viewModel.RobotState.Position = position;
//            _viewModel.RobotState.Angle = angle;
//        }

//        private void OnObstacleCollision(ObstacleData obstacle, Point position)
//        {
//            // Log collision
//            System.Diagnostics.Debug.WriteLine($"Collision with {obstacle.Type} at ({position.X},{position.Y})");
//        }

//        private void OnObstacleDetected(Point location, ObstacleType type, double distance)
//        {
//            System.Diagnostics.Debug.WriteLine($"Detected {type} at ({location.X},{location.Y}) distance {distance:F1}");
//        }

//        private void OnBatteryChanged(double level)
//        {
//            _viewModel.RobotState.BatteryLevel = level;
//        }

//        private void OnBatteryEmpty()
//        {
//            _ui.UpdateStatusText("🔴 Battery empty! Robot stopped.");
//        }

//        private void OnGoalReached(int goalIndex)
//        {
//            System.Diagnostics.Debug.WriteLine($"Goal {goalIndex} reached");
//        }
//        #endregion

//        #region Event Handlers - ViewModel
//        private void OnRobotStateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName == nameof(_viewModel.RobotState.Position) ||
//                e.PropertyName == nameof(_viewModel.RobotState.Angle))
//            {
//                _ui.UpdateRobotPositionDisplay();
//            }

//            if (e.PropertyName == nameof(_viewModel.RobotState.BatteryLevel))
//            {
//                _ui.UpdateBatteryDisplay();
//            }
//        }

//        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName == nameof(_viewModel.HasPath) && _viewModel.HasPath)
//            {
//                _ui.DisplayPath();
//            }

//            if (e.PropertyName == nameof(_viewModel.IsSearching))
//            {
//                _ui.UpdateStatusText(_viewModel.IsSearching ?
//                    _logic.GetSearchingStatus() : _logic.GetReadyStatus());
//            }

//            if (e.PropertyName == nameof(_viewModel.IsSimulating))
//            {
//                _ui.UpdateStatusText(_viewModel.IsSimulating ?
//                    _logic.GetSimulatingStatus() : _logic.GetReadyStatus());
//                _ui.UpdateRobotPanelButtons(_viewModel.IsSimulating, _viewModel.IsPaused);
//            }
//        }
//        #endregion

//        #region Event Handlers - Form
//        private void OnFormClosing(object sender, FormClosingEventArgs e)
//        {
//            _simulationService?.Stop();
//        }

//        private void OnFormKeyDown(object sender, KeyEventArgs e)
//        {
//            // ESC - Cancel current operation
//            if (e.KeyCode == Keys.Escape)
//            {
//                CancelCurrentDrawMode();
//                e.Handled = true;
//                return;
//            }

//            // WASD + QE - Manual robot control
//            switch (e.KeyCode)
//            {
//                case Keys.W:
//                case Keys.S:
//                case Keys.A:
//                case Keys.D:
//                case Keys.Q:
//                case Keys.E:
//                    _mapControl.MoveRobotManually(e.KeyCode);
//                    _viewModel.RobotState.Position = _mapControl.RobotPosition;
//                    _viewModel.RobotState.Angle = _mapControl.RobotAngle;
//                    e.Handled = true;
//                    break;
//            }
//        }
//        #endregion

//        #region Private Helper Methods
//        private void CancelCurrentDrawMode()
//        {
//            _form.SetAddingGoal(false);
//            _form.SetAddingParking(false);
//            _form.ClearMovingGoal();
//            _form.ClearMovingParking();

//            _mapControl.CurrentDrawMode = MapControl.DrawMode.None;
//            _mapControl.Cursor = Cursors.Default;
//            _ui.UpdateStatusText(_logic.GetOperationCancelledStatus());
//        }
//        #endregion
//    }
//}
#region File Header
/// <summary>
/// File: EnvironmentMapOperations.cs
/// Description: Map operations for main environment form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-08
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Services.Simulation;
using SallamPathFinder4.WinForms.Controls;
using SallamPathFinder4.WinForms.ViewModels;
using SallamPathFinder4.WinForms.Forms.Settings.frmMapSettings;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Services;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Environment
{
    public sealed class EnvironmentMapOperations
    {
        #region Private Fields
        private readonly frmEnvironment _form;
        private readonly frmEnvironmentCopy _form1;
        private readonly MapControl _mapControl;
        private readonly MainViewModel _viewModel;
        private readonly ISimulationService _simulationService;
        private readonly EnvironmentLogic _logic;
        private readonly EnvironmentUI _ui;
        #endregion

        #region Constructor
        public EnvironmentMapOperations(
            frmEnvironment form,
            MapControl mapControl,
            MainViewModel viewModel,
            ISimulationService simulationService,
            EnvironmentLogic logic,
            EnvironmentUI ui)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _mapControl = mapControl ?? throw new ArgumentNullException(nameof(mapControl));
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _simulationService = simulationService ?? throw new ArgumentNullException(nameof(simulationService));
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
            _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        }

        public EnvironmentMapOperations(
    frmEnvironmentCopy form,
    MapControl mapControl,
    MainViewModel viewModel,
    ISimulationService simulationService,
    EnvironmentLogic logic,
    EnvironmentUI ui)
        {
            _form1 = form ?? throw new ArgumentNullException(nameof(form));
            _mapControl = mapControl ?? throw new ArgumentNullException(nameof(mapControl));
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _simulationService = simulationService ?? throw new ArgumentNullException(nameof(simulationService));
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
            _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        }
        #endregion

        #region Public Methods - Map Operations
        public void NewMap()
        {
            var result = MessageBox.Show(
                "Create a new map? All unsaved changes will be lost.",
                "New Map",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            _mapControl.ClearPaths();
            _mapControl.ClearGoals();
            _mapControl.ClearParkingPoints();
            _mapControl.ResetStartPoints();
            _mapControl.AddStartPoint(new Point(10, 10));

            _viewModel.Goals?.Clear();
            _viewModel.ParkingPoints?.Clear();

            if (_mapControl.MapGrid != null)
            {
                _mapControl.MapGrid.Reset();
                _mapControl.MapGrid[10, 10].ElementType = MapElementType.StartPoint;
                _mapControl.MapGrid.UpdateAllCellProperties();
            }

            if (_simulationService is SimulationService simSvc)
            {
                simSvc.ReinitializeDoorGroups();
            }

            _form.pathDisplayPanel?.ClearPath();
            _ui.RefreshGoalsList();
            _ui.RefreshParkingList();
            _ui.UpdateStatusText(_logic.GetNewMapStatus());
            _mapControl.Invalidate();
        }

        public async Task OpenMap()
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Sallam Map (*.smap)|*.smap";
            ofd.Title = "Open Map File";

            if (ofd.ShowDialog() != DialogResult.OK) return;

            try
            {
                _ui.UpdateStatusText("Loading map...");
                await _viewModel.LoadMapAsync(ofd.FileName);

                if (_simulationService is SimulationService simSvc)
                {
                    simSvc.ReinitializeDoorGroups();
                }

                _ui.RefreshGoalsList();
                _ui.RefreshParkingList();
                _ui.UpdateStatusText(_logic.GetMapLoadedStatus(Path.GetFileName(ofd.FileName)));
                _mapControl.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading map: {ex.Message}", "Load Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ui.UpdateStatusText("Failed to load map");
            }
        }

        public async Task SaveMap()
        {
            if (_mapControl.MapGrid == null)
            {
                MessageBox.Show("No map to save.", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "Sallam Map (*.smap)|*.smap";
            sfd.DefaultExt = "smap";
            sfd.Title = "Save Map File";

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                _ui.UpdateStatusText("Saving map...");
                await _viewModel.SaveMapAsync(sfd.FileName);
                _ui.UpdateStatusText(_logic.GetMapSavedStatus(Path.GetFileName(sfd.FileName)));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving map: {ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ui.UpdateStatusText("Failed to save map");
            }
        }

        public void ClearAllObstacles()
        {
            var result = MessageBox.Show(
                "Clear all obstacles (static, semi-static, and dynamic)?",
                "Clear Obstacles",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            _mapControl.DynamicObstacles.Clear();

            if (_mapControl.MapGrid != null)
            {
                for (int x = 0; x < _mapControl.MapGrid.Width; x++)
                {
                    for (int y = 0; y < _mapControl.MapGrid.Height; y++)
                    {
                        var cell = _mapControl.MapGrid[x, y];

                        if (cell.ElementType == MapElementType.Wall ||
                            cell.ElementType == MapElementType.Door ||
                            cell.ElementType == MapElementType.Window ||
                            cell.ElementType == MapElementType.Ramp)
                        {
                            cell.ElementType = MapElementType.Empty;
                        }

                        cell.SurfaceWeight = 1;
                        cell.OccupyingObstacle = null;
                        cell.IsWalkable = true;
                        cell.IsDoorOpen = true;
                        cell.RampDifficulty = 0;
                    }
                }

                _mapControl.MapGrid.UpdateAllCellProperties();
            }

            _ui.UpdateStatusText(_logic.GetObstaclesClearedStatus());
            _mapControl.Invalidate();
        }
        #endregion

        #region Public Methods - View Operations
        public void ResetView()
        {
            _mapControl.ZoomLevel = EnvironmentLogic.ZOOM_DEFAULT;
            _mapControl.CellSize = EnvironmentLogic.DEFAULT_CELL_SIZE;
            _ui.UpdateRulers(_mapControl.CellSize, _mapControl.ZoomLevel);
            _ui.UpdateStatusText(_logic.GetViewResetStatus());
            _mapControl.Invalidate();
        }

        public void ToggleGrid()
        {
            _mapControl.ShowGrid = !_mapControl.ShowGrid;
            if (_form.showGridItem != null)
                _form.showGridItem.Checked = _mapControl.ShowGrid;
            _mapControl.Invalidate();
        }

        public void ToggleCoordinates()
        {
            _mapControl.ShowCoordinates = !_mapControl.ShowCoordinates;
            if (_form.showCoordsItem != null)
                _form.showCoordsItem.Checked = _mapControl.ShowCoordinates;
            _mapControl.Invalidate();
        }

        public void CenterMapOnRobot()
        {
            if (_mapControl == null || _viewModel == null) return;

            var robotPos = _viewModel.RobotState.Position;
            float scaledCellSize = _mapControl.CellSize * _mapControl.ZoomLevel;

            int splitterDistance = 860;
            int centerX = splitterDistance / 2;
            int centerY = _form.ClientSize.Height / 2;

            _mapControl.Invalidate();
            _ui.UpdateStatusText($"Centered on robot at ({robotPos.X}, {robotPos.Y})");
        }
        #endregion

        #region Public Methods - Map Settings
        public void ShowMapSettings()
        {
            if (_mapControl.MapGrid == null) return;

            double currentScale = _mapControl.ScaleCmPerCell;
            if (currentScale <= 0) currentScale = 10.0;

            var settingsForm = new frmMapSettings(_mapControl.MapGrid, _mapControl.CellSize, currentScale);

            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                _mapControl.CellSize = _logic.ClampCellSize(settingsForm.CellSize);
                _mapControl.ScaleCmPerCell = settingsForm.Scale;
                _ui.UpdateRulers(_mapControl.CellSize, _mapControl.ZoomLevel);
                _mapControl.Invalidate();
                _mapControl.Refresh();
                _ui.UpdateStatusText(_logic.GetMapUpdatedStatus(_mapControl.CellSize, _mapControl.ScaleCmPerCell));
            }
        }
        #endregion
    }
}
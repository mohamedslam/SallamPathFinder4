#region File Header
/// <summary>
/// File: EnvironmentUI.cs
/// Description: UI update methods for main environment form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-08
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Core.Models.Goals;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.WinForms.Controls;
using SallamPathFinder4.WinForms.ViewModels;
using SallamPathFinder4.WinForms.Panels;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Environment
{
    public sealed class EnvironmentUI
    {
        #region Private Fields
        private readonly frmEnvironment _form;
        private readonly frmEnvironmentCopy _form1;
        private readonly MapControl _mapControl;
        private readonly MainViewModel _viewModel;
        private readonly EnvironmentLogic _logic;
        #endregion

        #region Constructor
        // تأكد من أن الكود يبدو هكذا:
        public EnvironmentUI(frmEnvironment form, MapControl mapControl, MainViewModel viewModel, EnvironmentLogic logic)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _mapControl = mapControl ?? throw new ArgumentNullException(nameof(mapControl));
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));  // هذا هو السطر 39
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        } 
        public EnvironmentUI(frmEnvironmentCopy form, MapControl mapControl, MainViewModel viewModel, EnvironmentLogic logic)
        {
            _form1 = _form1 ?? throw new ArgumentNullException(nameof(form));
            _mapControl = mapControl ?? throw new ArgumentNullException(nameof(mapControl));
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));   
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }
        #endregion

        #region Public Methods - Status Updates
        public void UpdateStatusText(string text)
        {
            ExecuteOnUIThread(() =>
            {
                if (_form.lblStatus != null)
                    _form.lblStatus.Text = text;
            });
        }

        public void UpdateAlgorithmTimeDisplay(double timeMs)
        {
            ExecuteOnUIThread(() =>
            {
                if (_form.lblAlgoTime != null)
                    _form.lblAlgoTime.Text = $"⏱️ Algo: {timeMs:F2}ms";
            });
        }

        public void UpdateTravelTimeDisplay(double seconds)
        {
            ExecuteOnUIThread(() =>
            {
                if (_form.lblTravelTime != null)
                    _form.lblTravelTime.Text = $"🕒 Travel: {seconds:F1}s";
            });
        }
        #endregion

        #region Public Methods - Robot Display
        public void UpdateRobotPositionDisplay()
        {
            ExecuteOnUIThread(() =>
            {
                if (_form.lblRobotPos != null)
                {
                    _form.lblRobotPos.Text = _logic.FormatRobotPosition(
                        _viewModel.RobotState.Position,
                        _viewModel.RobotState.Angle);
                }

                _mapControl.UpdateRobotPosition(_viewModel.RobotState.Position, _viewModel.RobotState.Angle);
            });
        }

        public void UpdateBatteryDisplay()
        {
            ExecuteOnUIThread(() =>
            {
                if (_form.lblBattery != null)
                {
                    _form.lblBattery.Text = _logic.FormatBatteryDisplay(_viewModel.RobotState.BatteryLevel);

                    // Change color based on battery level
                    _form.lblBattery.ForeColor = _logic.GetBatteryColor(_viewModel.RobotState.BatteryLevel);
                }

                _form.robotPanel?.UpdateBattery(_viewModel.RobotState.BatteryLevel);
            });
        }

        public void UpdateBatteryFromPanel()
        {
            if (_viewModel != null && _form.robotPanel != null)
            {
                double newBatteryLevel = _form.robotPanel.SetBatteryLevel;
                _viewModel.RobotState.BatteryLevel = newBatteryLevel;
                _viewModel.SetBatteryLevel(newBatteryLevel);
                UpdateBatteryDisplay();
            }
        }
        #endregion

        #region Public Methods - Mouse Display
        public void UpdateMousePositionDisplay(Point mousePos, Point cellPos)
        {
            ExecuteOnUIThread(() =>
            {
                if (_form.lblMousePos != null)
                    _form.lblMousePos.Text = _logic.FormatMousePosition(mousePos);

                if (_form.lblCellPos != null)
                    _form.lblCellPos.Text = _logic.FormatCellPosition(cellPos);

                if (_form.lblRealPos != null && _mapControl.MapGrid != null)
                {
                    _form.lblRealPos.Text = _logic.FormatRealPosition(cellPos, _mapControl.ScaleCmPerCell);
                }
            });
        }

        public void UpdateCellPositionDisplay(Point cellPos)
        {
            ExecuteOnUIThread(() =>
            {
                if (_form.lblCellPos != null)
                    _form.lblCellPos.Text = _logic.FormatCellPosition(cellPos);

                if (_form.lblStatus != null && _mapControl.MapGrid != null)
                {
                    _form.lblStatus.Text = _logic.FormatCellStatus(_mapControl.MapGrid, cellPos);
                }
            });
        }
        #endregion

        #region Public Methods - Path Display
        public void DisplayPath()
        {
            ExecuteOnUIThread(() =>
            {
                var path = _viewModel.CurrentPathResult?.Path;
                if (path == null || path.Count == 0) return;

                // Clear previous path display
                _form.pathDisplayPanel?.ClearPath();

                // Display each step
                int step = 1;
                foreach (var node in path)
                {
                    _form.pathDisplayPanel?.AddPathStep(step++, node.X, node.Y, "Main", Color.Gold);
                }

                // Update statistics
                double timeMs = _viewModel.CurrentPathResult.ComputationTimeSeconds * 1000;
                double lengthCm = path.Count * _mapControl.ScaleCmPerCell;

                _form.pathDisplayPanel?.UpdateStats(path.Count, timeMs, lengthCm);

                // Update status bar
                _form.lblAlgoTime.Text = $"⏱️ Algo: {timeMs:F2}ms";
                _form.lblStatus.Text = _logic.FormatPathDisplay(path.Count, timeMs);

                // Draw path on map
                _mapControl.DrawPath(path.ToList(), Color.Gold);
            });
        }

        public void ClearPathDisplay()
        {
            ExecuteOnUIThread(() =>
            {
                _form.pathDisplayPanel?.ClearPath();
                _mapControl.ClearPaths();
            });
        }
        #endregion

        #region Public Methods - Goals and Parking Display
        public void RefreshGoalsList()
        {
            ExecuteOnUIThread(() =>
            {
                if (_viewModel.Goals == null) return;

                _viewModel.Goals.Clear();
                foreach (var goal in _mapControl.Goals)
                    _viewModel.Goals.Add(goal);

                _form.goalsPanel?.RefreshList();
                _viewModel.RefreshHasGoals();
            });
        }

        public void RefreshParkingList()
        {
            ExecuteOnUIThread(() =>
            {
                if (_viewModel.ParkingPoints == null) return;

                _viewModel.ParkingPoints.Clear();
                foreach (var parking in _mapControl.ParkingPoints)
                    _viewModel.ParkingPoints.Add(parking);

                _form.parkingPanel?.RefreshList();
            });
        }
        #endregion

        #region Public Methods - View Updates
        public void UpdateRulers(int cellSize, float scale)
        {
            ExecuteOnUIThread(() =>
            {
                if (_form.rulerTop != null)
                {
                    _form.rulerTop.CellSize = cellSize;
                    _form.rulerTop.Scale = scale;
                    _form.rulerTop.Invalidate();
                }

                if (_form.rulerLeft != null)
                {
                    _form.rulerLeft.CellSize = cellSize;
                    _form.rulerLeft.Scale = scale;
                    _form.rulerLeft.Invalidate();
                }
            });
        }

        public void UpdateDetectionZone(List<Point> zoneCells)
        {
            ExecuteOnUIThread(() =>
            {
                _mapControl.UpdateDetectionZone(zoneCells);
            });
        }

        public void ClearDetectionZone()
        {
            ExecuteOnUIThread(() =>
            {
                _mapControl.ClearDetectionZone();
            });
        }
        #endregion

        #region Public Methods - Button States
        public void UpdateRobotPanelButtons(bool isSimulating, bool isPaused)
        {
            ExecuteOnUIThread(() =>
            {
                _form.robotPanel?.SetButtonStates(isSimulating, isPaused);
            });
        }

        public void UpdateAlgorithmPanelButtons(bool isSimulating)
        {
            ExecuteOnUIThread(() =>
            {
                _form.algorithmSettingsPanel?.SetButtonStates(isSimulating, false);
            });
        }
        #endregion

        #region Private Methods - Thread Safety
        private void ExecuteOnUIThread(Action action)
        {
            if (_form.InvokeRequired)
            {
                _form.Invoke(action);
            }
            else
            {
                action();
            }
        }
        #endregion
    }
}
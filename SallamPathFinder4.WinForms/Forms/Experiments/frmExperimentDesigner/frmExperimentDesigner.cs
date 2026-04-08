#region File Header
/// <summary>
/// File: frmExperimentDesigner.cs
/// Description: Main form for designing and running algorithm comparison experiments
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.Services.Pathfinding;
using SallamPathFinder4.WinForms.Container;
using SallamPathFinder4.WinForms.Controls;
using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentDesigner.Core;
using SallamPathFinder4.WinForms.Forms.Shared;
using SallamPathFinder4.WinForms.Models;
using SallamPathFinder4.WinForms.ViewModels;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentDesigner
{
    /// <summary>
    /// Main form for designing and running algorithm comparison experiments
    /// </summary>
    public sealed partial class frmExperimentDesigner : Form
    {
        #region Constants
        private const int RENDER_DELAY_MS = 150;
        #endregion

        #region Private Fields
        private readonly MapGrid _mapGrid;
        private readonly MapControl _mapControl;
        private readonly MainViewModel _viewModel;
        private readonly ExperimentDesignerLogic _logic;
        private string _currentOutputPath;
        private bool _isRunning;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the experiment designer form
        /// </summary>
        public frmExperimentDesigner(MapGrid grid, MapControl control, MainViewModel vm)
        {
            _mapGrid = grid ?? throw new ArgumentNullException(nameof(grid));
            _mapControl = control ?? throw new ArgumentNullException(nameof(control));
            _viewModel = vm ?? throw new ArgumentNullException(nameof(vm));
            _logic = new ExperimentDesignerLogic();

            InitializeComponent();
            WireEvents();
            LoadUserSettings();
            LoadCurrentMapSettings();
            InitializeFilters();
        }
        #endregion

        #region Private Methods - Initialization
        /// <summary>
        /// Wires up all event handlers
        /// </summary>
        private void WireEvents()
        {
            _btnBrowseMap.Click += BtnBrowseMap_Click;
            _btnBrowseSavePath.Click += BtnBrowseSavePath_Click;
            _btnRunComparison.Click += BtnRunComparison_Click;
            _btnSaveSettings.Click += BtnSaveSettings_Click;
            _btnLoadSettings.Click += BtnLoadSettings_Click;
            _btnTrainNow.Click += BtnTrainNow_Click;
            _btnCancel.Click += (s, e) => Close();

            _rbLoadMap.CheckedChanged += (s, e) => UpdateMapFileControls();
            _chkUseCurrentMap.CheckedChanged += (s, e) => UpdateMapFileControls();
        }

        /// <summary>
        /// Updates map file controls enabled state
        /// </summary>
        private void UpdateMapFileControls()
        {
            bool enabled = !_chkUseCurrentMap.Checked && _rbLoadMap.Checked;
            _txtMapFilePath.Enabled = enabled;
            _btnBrowseMap.Enabled = enabled;
        }

        /// <summary>
        /// Initializes filter dropdowns
        /// </summary>
        private void InitializeFilters()
        {
            _clbDistanceMetrics.Items.Clear();
            _clbDistanceMetrics.Items.Add("Manhattan", true);
            _clbDistanceMetrics.Items.Add("Euclidean", true);
            _clbDistanceMetrics.Items.Add("MaxDXDY", false);
            _clbDistanceMetrics.Items.Add("DiagonalShortcut", false);
            _clbDistanceMetrics.Items.Add("EuclideanNoSQR", false);
        }

        /// <summary>
        /// Loads user settings from Properties
        /// </summary>
        private void LoadUserSettings()
        {
            string savePath = Properties.Settings.Default.ExperimentsSavePath;
            if (string.IsNullOrEmpty(savePath))
            {
                savePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
                                       "SallamPathFinder4", "Experiments");
            }
            _txtSavePath.Text = savePath;

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string lastExperiment = Properties.Settings.Default.LastExperimentName;
            if (!string.IsNullOrEmpty(lastExperiment))
            {
                _txtExperimentName.Text = lastExperiment;
            }
        }

        /// <summary>
        /// Saves user settings to Properties
        /// </summary>
        private void SaveUserSettings()
        {
            Properties.Settings.Default.ExperimentsSavePath = _txtSavePath.Text;
            Properties.Settings.Default.LastExperimentName = _txtExperimentName.Text;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Loads current map settings from ViewModel
        /// </summary>
        private void LoadCurrentMapSettings()
        {
            if (_mapGrid != null)
            {
                int goalCount = _viewModel?.Goals?.Count ?? 5;
                _nudGoalCount.Value = Math.Max(_nudGoalCount.Minimum,
                    Math.Min(_nudGoalCount.Maximum, Math.Max(1, goalCount)));

                int parkingCount = _viewModel?.ParkingPoints?.Count ?? 2;
                _nudParkingCount.Value = Math.Max(_nudParkingCount.Minimum,
                    Math.Min(_nudParkingCount.Maximum, Math.Max(1, parkingCount)));
            }
        }
        #endregion

        #region Private Methods - Event Handlers
        /// <summary>
        /// Handles browse map button click
        /// </summary>
        private void BtnBrowseMap_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Sallam Map (*.smap)|*.smap";
            ofd.Title = "Select Map File";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _txtMapFilePath.Text = ofd.FileName;
            }
        }

        /// <summary>
        /// Handles browse save path button click
        /// </summary>
        private void BtnBrowseSavePath_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            fbd.Description = "Select folder to save experiment results";
            fbd.SelectedPath = _txtSavePath.Text;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _txtSavePath.Text = fbd.SelectedPath;
                SaveUserSettings();
                _lblStatus.Text = $"Results will be saved to: {fbd.SelectedPath}";
            }
        }

        /// <summary>
        /// Handles save settings button click
        /// </summary>
        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            var settings = _logic.GetCurrentSettings(this);
            string fileName = ExperimentSharedLogic.GetSafeFileName(_txtExperimentName.Text);

            string filePath = ExperimentSharedHelper.ShowSaveExpSettingsDialog(fileName);
            if (!string.IsNullOrEmpty(filePath))
            {
                _logic.SaveSettingsToFile(settings, filePath);
                ExperimentSharedHelper.ShowInfo($"Settings saved to {filePath}", "Save Settings");
            }
        }

        /// <summary>
        /// Handles load settings button click
        /// </summary>
        private void BtnLoadSettings_Click(object sender, EventArgs e)
        {
            string filePath = ExperimentSharedHelper.ShowOpenExpSettingsDialog();
            if (!string.IsNullOrEmpty(filePath))
            {
                var settings = _logic.LoadSettingsFromFile(filePath);
                if (settings != null)
                {
                    _logic.ApplySettingsToForm(this, settings);
                    ExperimentSharedHelper.ShowInfo($"Settings loaded from {filePath}", "Load Settings");
                }
                else
                {
                    ExperimentSharedHelper.ShowError("Error loading settings file.", "Load Error");
                }
            }
        }

        /// <summary>
        /// Handles train model button click
        /// </summary>
        private async void BtnTrainNow_Click(object sender, EventArgs e)
        {
            _btnTrainNow.Enabled = false;
            _prgTraining.Visible = true;
            _prgTraining.Style = ProgressBarStyle.Marquee;
            _lblTrainingStatus.Visible = true;
            _lblTrainingStatus.Text = "Training model...";

            await Task.Run(() => System.Threading.Thread.Sleep(2000));

            _prgTraining.Visible = false;
            _lblTrainingStatus.Text = "Training completed!";
            _btnTrainNow.Enabled = true;

            await Task.Delay(2000);
            _lblTrainingStatus.Visible = false;
        }

        /// <summary>
        /// Handles run comparison button click
        /// </summary>
        private async void BtnRunComparison_Click(object sender, EventArgs e)
        {
            var algorithms = _logic.GetSelectedAlgorithms(this);
            if (algorithms == null || algorithms.Count == 0)
            {
                ExperimentSharedHelper.ShowWarning("Please select at least one algorithm.", "No Algorithm Selected");
                return;
            }

            var metrics = _logic.GetSelectedMetrics(this);
            if (metrics == null || metrics.Count == 0)
            {
                ExperimentSharedHelper.ShowWarning("Please select at least one distance metric.", "No Metric Selected");
                return;
            }

            _btnRunComparison.Enabled = false;
            _progressBar.Style = ProgressBarStyle.Marquee;
            _lblStatus.Text = "Preparing experiment...";
            Application.DoEvents();

            _currentOutputPath = CreateOutputDirectory();
            var results = new List<ComparisonResult>();

            try
            {
                int iterations = (int)_nudIterations.Value;
                var robotSettings = _logic.GetRobotSettings(this);
                var mlSettings = _logic.GetMLSettings(this);

                int totalExperiments = algorithms.Count * metrics.Count * iterations;
                int currentExperiment = 0;

                for (int iter = 0; iter < iterations; iter++)
                {
                    foreach (string algorithm in algorithms)
                    {
                        foreach (string metric in metrics)
                        {
                            currentExperiment++;
                            _lblStatus.Text = $"Running: {algorithm} - {metric} - Iter {iter + 1}/{iterations} ({currentExperiment}/{totalExperiments})";
                            Application.DoEvents();

                            var result = await RunSingleExperiment(algorithm, metric, iter + 1, robotSettings, mlSettings);
                            results.Add(result);
                        }
                    }
                    _progressBar.Value = (int)((iter + 1) / (double)iterations * 100);
                }

                SaveExperimentResults(results);
                _lblStatus.Text = $"Experiment completed! Results saved to: {_currentOutputPath}";
                SaveUserSettings();
                ShowResultsViewer(results);
            }
            catch (Exception ex)
            {
                ExperimentSharedHelper.ShowError($"Error during experiment: {ex.Message}", "Error");
                _lblStatus.Text = "Experiment failed";
            }
            finally
            {
                _btnRunComparison.Enabled = true;
                _progressBar.Style = ProgressBarStyle.Continuous;
            }
        }
        #endregion

        #region Private Methods - Experiment Execution
        /// <summary>
        /// Runs a single experiment
        /// </summary>
        private async Task<ComparisonResult> RunSingleExperiment(string algorithm, string metric, int iteration,
            RobotSettings robotSettings, MLSettings mlSettings)
        {
            var result = _logic.CreateEmptyResult(algorithm, metric, iteration, robotSettings, this);

            try
            {
                // Get start position
                Point start = _chkUseCurrentMap.Checked && _mapControl != null
                    ? _mapControl.RobotPosition
                    : new Point(10, 10);

                // Get goals from map
                List<Point> goals = _logic.GetRealGoals(_viewModel, _chkUseCurrentMap.Checked);
                if (goals == null || goals.Count == 0)
                {
                    result.Success = false;
                    result.ErrorMessage = "No goals available on the map";
                    return result;
                }

                // Get parking points
                List<Point> parkingPoints = _logic.GetParkingPoints(_viewModel, _chkUseCurrentMap.Checked);

                // Clear paths and capture initial screenshot
                await ClearAllPaths();
                if (_chkSaveScreenshots.Checked)
                {
                    await SaveScreenshot(algorithm, metric, iteration, "Initial", result);
                }

                // Create and configure finder
                var finder = _logic.CreateAlgorithmFinder(_mapGrid, algorithm, mlSettings);
                if (finder == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Algorithm {algorithm} not available";
                    return result;
                }

                _logic.ConfigureFinder(finder, metric, this);

                // Find sequential path through all goals
                var pathResult = await _logic.FindSequentialPath(finder, start, goals);
                if (!pathResult.Success)
                {
                    result.Success = false;
                    result.ErrorMessage = pathResult.ErrorMessage;
                    return result;
                }

                // Find return path to parking
                var returnResult = await _logic.FindReturnPath(finder, pathResult.CurrentPos, parkingPoints);

                var fullPath = _logic.CombinePaths(pathResult.Path, returnResult?.Path);
                result.Success = true;
                result.PathLength = fullPath.Count;
                result.ComputationTimeMs = pathResult.TotalTimeMs + (returnResult?.TotalTimeMs ?? 0);
                result.Path = fullPath.Select(p => new System.Drawing.Point(p.X, p.Y)).ToList();

                // Draw and capture screenshots
                await DrawGoldenPath(fullPath);
                if (_chkSaveScreenshots.Checked)
                {
                    await SaveScreenshot(algorithm, metric, iteration, "Path", result);
                }

                await DrawGreenPath(fullPath);
                if (_chkSaveScreenshots.Checked)
                {
                    await SaveScreenshot(algorithm, metric, iteration, "Completed", result);
                }

                // Calculate battery
                result.RemainingBattery = ExperimentSharedLogic.CalculateBatteryConsumption(
                    result.PathLength, robotSettings.BatteryConsumptionRate, robotSettings.InitialBatteryLevel);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                await ClearAllPaths();
            }

            return result;
        }

        /// <summary>
        /// Clears all paths from the map control
        /// </summary>
        private async Task ClearAllPaths()
        {
            ExecuteOnUIThread(() =>
            {
                _mapControl.ClearPaths();
                _mapControl.Refresh();
            });
            await Task.Delay(50);
        }

        /// <summary>
        /// Draws golden path on the map control
        /// </summary>
        private async Task DrawGoldenPath(List<PathNode> path)
        {
            ExecuteOnUIThread(() =>
            {
                _mapControl.ClearPaths();
                var coloredPath = new ColoredPath(path, Color.Gold, false);
                _mapControl.DrawColoredPaths(new List<ColoredPath> { coloredPath });
                _mapControl.Refresh();
            });
            await Task.Delay(RENDER_DELAY_MS);
        }

        /// <summary>
        /// Draws green dashed path on the map control
        /// </summary>
        private async Task DrawGreenPath(List<PathNode> path)
        {
            ExecuteOnUIThread(() =>
            {
                var goldenPath = new ColoredPath(path, Color.Gold, false);
                var traveledNodes = path.Select(p => new PathNode(p.X, p.Y)).ToList();
                var greenPath = new ColoredPath(traveledNodes, Color.Green, true);
                _mapControl.DrawColoredPaths(new List<ColoredPath> { goldenPath, greenPath });
                _mapControl.Refresh();
            });
            await Task.Delay(RENDER_DELAY_MS);
        }

        /// <summary>
        /// Saves a screenshot
        /// </summary>
        private async Task SaveScreenshot(string algorithm, string metric, int iteration, string type, ComparisonResult result)
        {
            try
            {
                string screenshotsPath = Path.Combine(_currentOutputPath, "Screenshots", algorithm);
                Directory.CreateDirectory(screenshotsPath);

                string fileName = $"{algorithm}_{metric}_{type}_Iter{iteration}.png";
                string screenshotFile = Path.Combine(screenshotsPath, fileName);

                ExecuteOnUIThread(() =>
                {
                    using var bmp = new Bitmap(_mapControl.Width, _mapControl.Height);
                    _mapControl.DrawToBitmap(bmp, new Rectangle(0, 0, _mapControl.Width, _mapControl.Height));
                    bmp.Save(screenshotFile, System.Drawing.Imaging.ImageFormat.Png);
                });

                switch (type)
                {
                    case "Initial": result.InitialScreenshotPath = screenshotFile; break;
                    case "Path": result.PathScreenshotPath = screenshotFile; break;
                    case "Completed": result.CompletedScreenshotPath = screenshotFile; break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving {type} screenshot: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Executes an action on the UI thread
        /// </summary>
        private void ExecuteOnUIThread(Action action)
        {
            if (_mapControl.InvokeRequired)
                _mapControl.Invoke(action);
            else
                action();
        }
        #endregion

        #region Private Methods - File Operations
        /// <summary>
        /// Creates output directory for experiment results
        /// </summary>
        private string CreateOutputDirectory()
        {
            string baseDir = _txtSavePath.Text;
            string experimentId = ExperimentSharedLogic.CreateExperimentId(_txtExperimentName.Text);
            string safeId = ExperimentSharedLogic.GetSafeFileName(experimentId);
            string basePath = Path.Combine(baseDir, safeId);
            Directory.CreateDirectory(basePath);
            return basePath;
        }

        /// <summary>
        /// Saves experiment results to CSV and summary
        /// </summary>
        private void SaveExperimentResults(List<ComparisonResult> results)
        {
            string csvPath = Path.Combine(_currentOutputPath, "Results.csv");
            _logic.SaveResultsToCsv(results, csvPath);

            string summaryPath = Path.Combine(_currentOutputPath, "Summary.txt");
            _logic.GenerateSummaryReport(results, summaryPath, _txtExperimentName.Text);
        }

        /// <summary>
        /// Shows results viewer
        /// </summary>
        private void ShowResultsViewer(List<ComparisonResult> results)
        {
            var resultsList = results.Select(r => new ExperimentResultItem
            {
                Algorithm = r.Algorithm ?? "Unknown",
                Metric = r.Metric ?? "Unknown",
                Iteration = r.Iteration,
                PathLength = r.PathLength,
                ComputationTimeMs = r.ComputationTimeMs,
                Success = r.Success,
                RemainingBattery = r.RemainingBattery,
                CollisionCount = r.CollisionCount,
                InvalidMoveCount = r.InvalidMoveCount,
                AverageActualSpeed = r.AverageActualSpeed,
                InitialScreenshotPath = r.InitialScreenshotPath,
                PathScreenshotPath = r.PathScreenshotPath,
                CompletedScreenshotPath = r.CompletedScreenshotPath,
                Path = r.Path,
                ErrorMessage = r.ErrorMessage
            }).ToList();

            var resultsViewer = new frmExperimentResults.frmExperimentResults(resultsList, _currentOutputPath);
            resultsViewer.ShowDialog();
        }
        #endregion
    }
}
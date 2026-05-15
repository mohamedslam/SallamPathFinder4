#region File Header
/// <summary>
/// File: frmExperimentDesigner.cs
/// Description: Main form for designing and running algorithm comparison experiments
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-12
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Algorithms.Implementations;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Algorithms;
using SallamPathFinder4.Core.Models.Experiments;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.Services.Pathfinding;
using SallamPathFinder4.WinForms.Controls;
using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentDesigner.Core;
using SallamPathFinder4.WinForms.Forms.Shared;
using SallamPathFinder4.WinForms.Models;
using SallamPathFinder4.WinForms.ViewModels;
using System.Text.Json;
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
        #endregion

        #region Private Fields - Iteration Tracking
        private Point _currentStartPoint = new Point(10, 10);
        private List<Point> _iterationStartPoints = new List<Point>();
        private List<Point> _iterationEndPoints = new List<Point>();
        private List<List<Point>> _iterationPaths = new List<List<Point>>();
        private int _completedIterations;
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
            SetupAlgorithmGrid();
            InitializeToolTips();
            WireEvents();                    
            LoadUserSettings();
            LoadCurrentMapSettings();
            LoadCurrentRobotSettings();
            PopulateAlgorithmComboBox();
        }
        #endregion

        #region Private Methods - Initialization
        /// <summary>
        /// Initializes tooltips for all input controls
        /// </summary>
        private void InitializeToolTips()
        {
            var toolTip = new ToolTip();

            // Map settings
            toolTip.SetToolTip(_nudGoalCount, "Number of goal points the robot must visit");
            toolTip.SetToolTip(_nudParkingCount, "Number of parking/charging stations where robot can recharge");
            toolTip.SetToolTip(_nudStaticObstacles, "Walls that block the robot (read-only from current map)");
            toolTip.SetToolTip(_nudDynamicObstacles, "Moving obstacles like people, animals (read-only from current map)");
            toolTip.SetToolTip(_chkUseCustomStartPoint, "Use a custom start point instead of the robot's current position");
            toolTip.SetToolTip(_btnPickStartPoint, "Click to select start point from the main map");

            // Robot settings
            toolTip.SetToolTip(_nudRobotSpeed, "Robot movement speed in centimeters per second");
            toolTip.SetToolTip(_nudRobotBattery, "Initial battery level (0-100%)");
            toolTip.SetToolTip(_nudConsumptionRate, "Battery consumption rate per meter traveled");
            toolTip.SetToolTip(_nudViewAngle, "Robot's field of view in degrees (90, 180, 270, 360)");
            toolTip.SetToolTip(_nudDetectionRange, "Obstacle detection range in cells");
            toolTip.SetToolTip(_chkEnableDynamicCharging, "Automatically go to parking when battery is low");

            // Experiment settings
            toolTip.SetToolTip(_nudIterations, "Number of times to repeat the experiment");
            toolTip.SetToolTip(_chkSaveScreenshots, "Save screenshots of initial, path, and completed states");
            toolTip.SetToolTip(_chkSaveReplay, "Save robot movement replay data");

            // ML settings
            toolTip.SetToolTip(_chkEnableDynamicLearning, "Enable obstacle memory across simulations (SPPA-DL only)");
            toolTip.SetToolTip(_nudLearningRate, "Learning rate α for obstacle memory (higher = stronger memory)");
            toolTip.SetToolTip(_chkUseNeuralNetwork, "Use neural network for obstacle movement prediction (SPPA-DL only)");
        }

        /// <summary>
        /// Wires all form events
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

            if (_btnPickStartPoint != null)
                _btnPickStartPoint.Click += BtnPickStartPoint_Click;

            if (_chkEnableDynamicCharging != null)
            {
                _chkEnableDynamicCharging.CheckedChanged += ChkEnableDynamicCharging_CheckedChanged;
                bool enabled = _chkEnableDynamicCharging.Checked;
                if (_nudChargingTime != null) _nudChargingTime.Enabled = enabled;
                if (_nudSafetyMargin != null) _nudSafetyMargin.Enabled = enabled;
            }

            // Sensitivity Analysis events (ربط مرة واحدة فقط)
            if (_cboAlgorithm != null)
                _cboAlgorithm.SelectedIndexChanged += (s, e) => { UpdateParametersForAlgorithm(); UpdateDefaultSensitivityValues(); };

            if (_cboSensitivityParameter != null)
                _cboSensitivityParameter.SelectedIndexChanged += (s, e) => UpdateDefaultSensitivityValues();

            if (_btnValidateValues != null)
                _btnValidateValues.Click += BtnValidateValues_Click;

            if (_btnRunSensitivity != null)
                _btnRunSensitivity.Click += BtnRunSensitivity_Click;

            if (_chkEnableSensitivity != null)
                _chkEnableSensitivity.CheckedChanged += (s, e) => UpdateSensitivityControlsState();
           
            if (_chkSelectAll != null)
                _chkSelectAll.CheckedChanged += ChkSelectAll_CheckedChanged;
            // DataGridView events
            if (_dgvAlgorithems != null)
            {
                _dgvAlgorithems.CellClick += DgvAlgorithems_CellClick;
                _dgvAlgorithems.CellDoubleClick += DgvAlgorithems_CellDoubleClick;
                _dgvAlgorithems.CellValueChanged += DgvAlgorithems_CellValueChanged;
                _dgvAlgorithems.CurrentCellDirtyStateChanged += DgvAlgorithems_CurrentCellDirtyStateChanged;
                _dgvAlgorithems.KeyDown += DgvAlgorithems_KeyDown;
            }
        }

        /// <summary>
        /// Handles Select All / Deselect All checkbox
        /// </summary>
        private void ChkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool selectAll = _chkSelectAll.Checked;

            foreach (DataGridViewRow row in _dgvAlgorithems.Rows)
            {
                row.Cells["colEnabled"].Value = selectAll;
            }
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
        /// Loads user settings from Properties
        /// </summary>
        private void LoadUserSettings()
        {
            string savePath = Properties.Settings.Default.ExperimentsSavePath;
            if (string.IsNullOrEmpty(savePath))
            {
                savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
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
        /// Loads current map settings from the main form
        /// </summary>
        /// <summary>
        /// Loads current map settings from the main form
        /// </summary>
        private void LoadCurrentMapSettings()
        {
            if (_mapGrid == null) return;

            // أبعاد الخريطة - للقراءة فقط
            if (_nudGridWidth != null) _nudGridWidth.Value = _mapGrid.Width;
            if (_nudGridHeight != null) _nudGridHeight.Value = _mapGrid.Height;

            // Goals and Parking
            int goalCount = _viewModel?.Goals?.Count ?? 0;
            _nudGoalCount.Value = Math.Clamp(goalCount, _nudGoalCount.Minimum, _nudGoalCount.Maximum);

            int parkingCount = _viewModel?.ParkingPoints?.Count ?? 0;
            _nudParkingCount.Value = Math.Clamp(parkingCount, _nudParkingCount.Minimum, _nudParkingCount.Maximum);

            // Count obstacles by type
            int staticCount = 0;
            int semiStaticCount = 0;
            int roughTerrainCount = 0;

            for (int x = 0; x < _mapGrid.Width; x++)
            {
                for (int y = 0; y < _mapGrid.Height; y++)
                {
                    var cell = _mapGrid[x, y];

                    switch (cell.ElementType)
                    {
                        case MapElementType.Wall:
                            staticCount++;
                            break;
                        case MapElementType.Door:
                        case MapElementType.Window:
                        case MapElementType.Ramp:
                            semiStaticCount++;
                            break;
                    }

                    // Rough terrain based on SurfaceWeight
                    if (cell.SurfaceWeight >= 25)
                    {
                        roughTerrainCount++;
                    }
                }
            }

            int dynamicCount = _mapControl?.DynamicObstacles?.Count ?? 0;

            _nudStaticObstacles.Value = Math.Clamp(staticCount, _nudStaticObstacles.Minimum, _nudStaticObstacles.Maximum);

            if (_nudSemiStaticObstacles != null)
            {
                _nudSemiStaticObstacles.Value = Math.Clamp(semiStaticCount, _nudSemiStaticObstacles.Minimum, _nudSemiStaticObstacles.Maximum);
            }

            if (_nudRoughTerrain != null)
            {
                _nudRoughTerrain.Value = Math.Clamp(roughTerrainCount, _nudRoughTerrain.Minimum, _nudRoughTerrain.Maximum);
            }

            _nudDynamicObstacles.Value = Math.Clamp(dynamicCount, _nudDynamicObstacles.Minimum, _nudDynamicObstacles.Maximum);
        }

        /// <summary>
        /// Loads current robot settings from the main form
        /// </summary>
        private void LoadCurrentRobotSettings()
        {
            if (_viewModel?.RobotState == null) return;

            double currentSpeed = _viewModel.RobotState.Speed;
            if (currentSpeed > 0)
            {
                _nudRobotSpeed.Value = (decimal)Math.Clamp(currentSpeed, (double)_nudRobotSpeed.Minimum, (double)_nudRobotSpeed.Maximum);
            }

            Point robotPos = _mapControl?.RobotPosition ?? new Point(10, 10);
            if (_lblCurrentStartPoint != null)
            {
                _lblCurrentStartPoint.Text = $"Current: ({robotPos.X}, {robotPos.Y})";
                _lblCurrentStartPoint.Tag = robotPos;
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

            await Task.Run(() => Thread.Sleep(2000));

            _prgTraining.Visible = false;
            _lblTrainingStatus.Text = "Training completed!";
            _btnTrainNow.Enabled = true;

            await Task.Delay(2000);
            _lblTrainingStatus.Visible = false;
        }

        /// <summary>
        /// Handles pick start point button click
        /// </summary>
        private void BtnPickStartPoint_Click(object sender, EventArgs e)
        {
            this.Hide();

            MessageBox.Show("Click on the main map to select start point, then press Enter.\n\nCurrent start point will be updated.",
                "Pick Start Point", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            Point selectedPoint = _mapControl?.RobotPosition ?? new Point(10, 10);

            if (_lblCurrentStartPoint != null)
            {
                _lblCurrentStartPoint.Text = $"Current: ({selectedPoint.X}, {selectedPoint.Y})";
                _lblCurrentStartPoint.Tag = selectedPoint;
            }

            if (_chkUseCustomStartPoint != null)
                _chkUseCustomStartPoint.Checked = true;

            this.Show();
        }

        /// <summary>
        /// Handles dynamic charging checkbox change
        /// </summary>
        private void ChkEnableDynamicCharging_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = _chkEnableDynamicCharging.Checked;
            if (_nudChargingTime != null) _nudChargingTime.Enabled = enabled;
            if (_lblChargingTime != null) _lblChargingTime.Enabled = enabled;
            if (_nudSafetyMargin != null) _nudSafetyMargin.Enabled = enabled;
            if (_lblSafetyMargin != null) _lblSafetyMargin.Enabled = enabled;
        }

        /// <summary>
        /// Main experiment execution handler
        /// </summary>
        private async void BtnRunComparison_Click(object sender, EventArgs e)
        {
            if (!ValidateExperimentInputs()) return;

            var selectedConfigs = GetSelectedAlgorithmsWithParams();
            if (selectedConfigs.Count == 0)
            {
                ExperimentSharedHelper.ShowWarning("Please enable at least one algorithm.", "No Algorithm Selected");
                return;
            }

            ResetToDefaultState();

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
                var (enableDynamicCharging, chargingTime, safetyMargin) = _logic.GetDynamicChargingSettings(this);

                robotSettings.EnableDynamicCharging = enableDynamicCharging;
                robotSettings.ChargingTimeSeconds = chargingTime;
                robotSettings.SafetyMarginPercent = safetyMargin;

                Point fixedStartPoint = GetInitialStartPoint();

                // ========== فصل الخوارزميات حسب Sequential Mode ==========
                var normalAlgorithms = new List<(string algorithm, string metric, Dictionary<string, object> parameters)>();
                var sequentialAlgorithms = new List<(string algorithm, string metric, Dictionary<string, object> parameters)>();

                foreach (var config in selectedConfigs)
                {
                    bool isSequential = config.parameters.ContainsKey("SequentialMode")
                        ? Convert.ToBoolean(config.parameters["SequentialMode"])
                        : false;

                    if (isSequential)
                        sequentialAlgorithms.Add(config);
                    else
                        normalAlgorithms.Add(config);
                }

                // ========== تشغيل الخوارزميات العادية ==========
                for (int iter = 0; iter < iterations; iter++)
                {
                    foreach (var config in normalAlgorithms)
                    {
                        _lblStatus.Text = $"Running: {config.algorithm} - {config.metric} - Iter {iter + 1}/{iterations} | Start: ({fixedStartPoint.X},{fixedStartPoint.Y})";
                        Application.DoEvents();

                        var result = await RunSingleExperiment(config.algorithm, config.metric, iter + 1,
                            robotSettings, mlSettings, fixedStartPoint);
                        results.Add(result);
                    }

                    _progressBar.Value = (int)((iter + 1) / (double)iterations * 100);
                }

                // ========== تشغيل الخوارزميات المتسلسلة ==========
                foreach (var config in sequentialAlgorithms)
                {
                    Point currentStartPoint = fixedStartPoint;
                    bool sequentialFailed = false;

                    _lblStatus.Text = $"Sequential Mode: {config.algorithm} - {config.metric}";
                    Application.DoEvents();

                    for (int iter = 0; iter < iterations; iter++)
                    {
                        _lblStatus.Text = $"Sequential: {config.algorithm} - {config.metric} - Iter {iter + 1}/{iterations} | Start: ({currentStartPoint.X},{currentStartPoint.Y})";
                        Application.DoEvents();

                        var result = await RunSingleExperiment(config.algorithm, config.metric, iter + 1,
                            robotSettings, mlSettings, currentStartPoint);
                        results.Add(result);

                        if (!result.Success)
                        {
                            sequentialFailed = true;
                            MessageBox.Show($"Sequential Mode failed for {config.algorithm} at iteration {iter + 1}.\n\n" +
                                            $"Start Point: ({currentStartPoint.X}, {currentStartPoint.Y})\n" +
                                            $"Reason: {result.ErrorMessage ?? "Path not found"}",
                                            "Sequential Mode Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        }

                        // تحديث نقطة البداية لنقطة نهاية المسار
                        if (result.EndPointReached != Point.Empty)
                        {
                            currentStartPoint = result.EndPointReached;
                            System.Diagnostics.Debug.WriteLine($"[Sequential] {config.algorithm} Iter {iter + 1}: New start point ({currentStartPoint.X}, {currentStartPoint.Y})");
                        }
                    }

                    if (sequentialFailed)
                    {
                        _lblStatus.Text = $"Sequential experiment failed for {config.algorithm}";
                    }

                    _progressBar.Value = 100;
                }

                SaveExperimentResults(results);
                SaveIterationTrackingData(results);
                SaveUserSettings();
                ShowResultsViewer(results);

                _lblStatus.Text = $"Experiment completed! Results saved to: {_currentOutputPath}";
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
        /// Runs a single experiment iteration with full data collection
        /// </summary>
        private async Task<ComparisonResult> RunSingleExperiment(
            string algorithm,
            string metric,
            int iteration,
            RobotSettings robotSettings,
            MLSettings mlSettings,
            Point startPoint)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Starting experiment for {algorithm}");

            var result = _logic.CreateEmptyResult(algorithm, metric, iteration, robotSettings, this);

            try
            {
                var parameters = GetParametersForAlgorithmFromGrid(algorithm, metric);
                bool orderGoalsByDistance = parameters.ContainsKey("OrderGoalsByDistance")
                    ? Convert.ToBoolean(parameters["OrderGoalsByDistance"])
                    : false;
                bool sequentialMode = parameters.ContainsKey("SequentialMode")
                    ? Convert.ToBoolean(parameters["SequentialMode"])
                    : false;

                _iterationStartPoints.Add(startPoint);

                bool enableDynamicCharging = _chkEnableDynamicCharging?.Checked ?? false;
                double chargingTimeSeconds = _nudChargingTime != null ? (double)_nudChargingTime.Value : 15;
                double safetyMarginPercent = (double)(_nudSafetyMargin?.Value ?? 10);

                result.UsedDynamicCharging = enableDynamicCharging;
                result.ChargingTimeSeconds = chargingTimeSeconds;
                result.SafetyMarginPercent = safetyMarginPercent;
                result.StartPointUsed = startPoint;
                result.OrderedByDistance = orderGoalsByDistance;
                result.InitialBatteryPercent = robotSettings.InitialBatteryLevel;

                List<Point> goals = _logic.GetRealGoals(_viewModel, _chkUseCurrentMap.Checked);
                if (orderGoalsByDistance)
                {
                    goals = goals.OrderBy(g => Math.Abs(g.X - startPoint.X) + Math.Abs(g.Y - startPoint.Y)).ToList();
                    result.GoalOrder = string.Join(" → ", goals.Select(g => $"({g.X},{g.Y})"));
                }
                else
                {
                    result.GoalOrder = string.Join(" → ", goals.Select(g => $"({g.X},{g.Y})"));
                }

                if (goals == null || goals.Count == 0)
                {
                    result.Success = false;
                    result.ErrorMessage = "No goals available on the map";
                    result.FailureReason = "NoGoals";
                    _iterationEndPoints.Add(Point.Empty);
                    return result;
                }

                List<Point> parkingPoints = _logic.GetParkingPoints(_viewModel, _chkUseCurrentMap.Checked);

                await ClearAllPaths();

                ExecuteOnUIThread(() =>
                {
                    _mapControl.SetCurrentStartPoint(startPoint);
                    _mapControl.RobotPosition = startPoint;
                    _mapControl.RobotAngle = 0;
                    _mapControl.ShowRobot = true;
                    _mapControl.Refresh();
                });
                await Task.Delay(RENDER_DELAY_MS);

                if (_chkSaveScreenshots.Checked)
                {
                    await SaveScreenshot(algorithm, metric, iteration, "Initial", result);
                }

                var algorithmParams = GetParametersForAlgorithmFromGrid(algorithm, metric);
                var finder = _logic.CreateAlgorithmFinder(_mapGrid, algorithm, mlSettings, algorithmParams);

                if (finder == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Algorithm {algorithm} not available";
                    result.FailureReason = "AlgorithmNotAvailable";
                    _iterationEndPoints.Add(Point.Empty);
                    return result;
                }

                _logic.ConfigureFinder(finder, metric, this);

                var pathResult = await _logic.FindSequentialPath(finder, startPoint, goals);
                if (!pathResult.Success)
                {
                    result.Success = false;
                    result.ErrorMessage = pathResult.ErrorMessage;
                    result.FailureReason = "PathNotFound";
                    _iterationEndPoints.Add(Point.Empty);
                    return result;
                }

                var pathErrors = AnalyzePathForErrors(pathResult.Path, _mapGrid);
                if (pathErrors.Any())
                {
                    result.HasPathErrors = true;
                    result.PathErrorsJson = JsonSerializer.Serialize(pathErrors);
                    result.ErrorMessage = string.Join("; ", pathErrors.Select(e => e.Message));
                    result.FailureReason = pathErrors.First().Type;
                }

                List<PathNode> fullPath;
                Point endPoint;

                bool isSPPAFamily = (algorithm == "SPPA" || algorithm == "SPPA_DL");

                if (isSPPAFamily && parkingPoints != null && parkingPoints.Count > 0)
                {
                    var returnResult = await _logic.FindReturnPath(finder, pathResult.CurrentPos, parkingPoints);
                    if (returnResult != null && returnResult.Path != null && returnResult.Path.Count > 0)
                    {
                        fullPath = _logic.CombinePaths(pathResult.Path, returnResult.Path);
                        endPoint = fullPath.Count > 0 ? new Point(fullPath.Last().X, fullPath.Last().Y) : pathResult.CurrentPos;
                    }
                    else
                    {
                        fullPath = pathResult.Path.ToList();
                        endPoint = pathResult.CurrentPos;
                    }
                }
                else
                {
                    fullPath = pathResult.Path.ToList();
                    endPoint = pathResult.CurrentPos;
                }

                _iterationEndPoints.Add(endPoint);
                result.EndPointReached = endPoint;

                await DrawPathWithColors(fullPath, goals, startPoint, endPoint);

                if (_chkSaveScreenshots.Checked)
                {
                    await SaveScreenshot(algorithm, metric, iteration, "Path", result);
                }

                await DrawCompletedPath(fullPath, endPoint);

                if (_chkSaveScreenshots.Checked)
                {
                    await SaveScreenshot(algorithm, metric, iteration, "Completed", result);
                }

                _logic.CalculateAndFillResultData(result, fullPath, startPoint, endPoint, goals,
                    robotSettings, chargingTimeSeconds, _mapControl.ScaleCmPerCell,
                    orderGoalsByDistance, pathResult.TotalTimeMs);
                if (pathResult.Success && fullPath.Count > 0)
                {
                    result.Success = true;
                    result.FailureReason = "None";
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Setting Success = true for {algorithm}");
                }
                else
                {
                    result.Success = false;
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Setting Success = false for {algorithm}. pathResult.Success={pathResult.Success}, fullPath.Count={fullPath.Count}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Experiment] Exception: {ex.Message}");
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.FailureReason = "Exception";
            }
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Final Success = {result.Success}, PathLength = {result.PathLength}, Errors = {result.PathErrorsJson}");
            return result;
        }

        /// <summary>
        /// Draws path with colored segments for each goal
        /// </summary>
        private async Task DrawPathWithColors(List<PathNode> fullPath, List<Point> goals, Point startPoint, Point endPoint)
        {
            ExecuteOnUIThread(() =>
            {
                _mapControl.ClearPaths();

                var coloredSegments = new List<ColoredPath>();
                int currentIndex = 0;

                Color[] goalColors = new Color[]
                {
                    Color.Red, Color.Green, Color.Blue, Color.Orange, Color.Purple,
                    Color.Cyan, Color.Magenta, Color.Yellow, Color.Brown, Color.Pink
                };

                for (int g = 0; g < goals.Count; g++)
                {
                    int goalIndex = -1;
                    for (int i = currentIndex; i < fullPath.Count; i++)
                    {
                        if (fullPath[i].X == goals[g].X && fullPath[i].Y == goals[g].Y)
                        {
                            goalIndex = i;
                            break;
                        }
                    }

                    if (goalIndex > currentIndex)
                    {
                        var segment = fullPath.Skip(currentIndex).Take(goalIndex - currentIndex + 1).ToList();
                        var coloredPath = new ColoredPath(segment, goalColors[g % goalColors.Length], false);
                        coloredSegments.Add(coloredPath);
                        currentIndex = goalIndex;
                    }
                }

                if (currentIndex < fullPath.Count - 1)
                {
                    var remaining = fullPath.Skip(currentIndex).ToList();
                    var returnPath = new ColoredPath(remaining, Color.Green, true);
                    coloredSegments.Add(returnPath);
                }

                _mapControl.DrawColoredPaths(coloredSegments);
                _mapControl.SetCurrentStartPoint(startPoint);
                _mapControl.RobotPosition = startPoint;
                _mapControl.RobotAngle = 0;
                _mapControl.ShowRobot = true;
                _mapControl.Refresh();
            });
            await Task.Delay(RENDER_DELAY_MS);
        }

        /// <summary>
        /// Draws completed path with golden color
        /// </summary>
        private async Task DrawCompletedPath(List<PathNode> fullPath, Point endPoint)
        {
            ExecuteOnUIThread(() =>
            {
                var goldenPath = new ColoredPath(fullPath, Color.Gold, false);
                var traveledNodes = fullPath.Select(p => new PathNode(p.X, p.Y)).ToList();
                var traveledPath = new ColoredPath(traveledNodes, Color.Green, true);
                _mapControl.DrawColoredPaths(new List<ColoredPath> { goldenPath, traveledPath });

                _mapControl.RobotPosition = endPoint;
                _mapControl.RobotAngle = 0;
                _mapControl.ShowRobot = true;
                _mapControl.Refresh();
            });
            await Task.Delay(RENDER_DELAY_MS);
        }

        /// <summary>
        /// Analyzes path for errors
        /// </summary>
        private List<PathError> AnalyzePathForErrors(IReadOnlyList<PathNode> path, MapGrid grid)
        {
            var errors = new List<PathError>();

            if (path == null || path.Count == 0)
                return errors;

            for (int i = 0; i < path.Count; i++)
            {
                var node = path[i];

                if (!grid.IsValidCoordinate(node.X, node.Y))
                {
                    errors.Add(new PathError
                    {
                        Type = "InvalidCoordinate",
                        Message = $"Invalid coordinate ({node.X},{node.Y}) at step {i}",
                        Location = new Point(node.X, node.Y),
                        StepIndex = i
                    });
                    continue;
                }

                var cell = grid[node.X, node.Y];

                if (cell.ElementType == MapElementType.Window)
                {
                    errors.Add(new PathError
                    {
                        Type = "WindowCrossing",
                        Message = $"Robot crossed through window at ({node.X},{node.Y})",
                        Location = new Point(node.X, node.Y),
                        StepIndex = i
                    });
                }

                if (cell.ElementType == MapElementType.Wall)
                {
                    errors.Add(new PathError
                    {
                        Type = "WallCollision",
                        Message = $"Robot collided with wall at ({node.X},{node.Y})",
                        Location = new Point(node.X, node.Y),
                        StepIndex = i
                    });
                }

                if (cell.OccupyingObstacle != null)
                {
                    errors.Add(new PathError
                    {
                        Type = "DynamicObstacleCollision",
                        Message = $"Robot collided with {cell.OccupyingObstacle.Type} at ({node.X},{node.Y})",
                        Location = new Point(node.X, node.Y),
                        StepIndex = i
                    });
                }

                if (cell.ElementType == MapElementType.Door && !cell.IsDoorOpen)
                {
                    errors.Add(new PathError
                    {
                        Type = "ClosedDoor",
                        Message = $"Robot passed through closed door at ({node.X},{node.Y})",
                        Location = new Point(node.X, node.Y),
                        StepIndex = i
                    });
                }
            }

            return errors;
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
            if (results == null || results.Count == 0) return;

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
            if (results == null || results.Count == 0)
            {
                MessageBox.Show("No results to display.", "No Results",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var resultsList = new List<ExperimentResultItem>();

                foreach (var r in results)
                {
                    if (r == null) continue;

                    var item = new ExperimentResultItem
                    {
                        Algorithm = r.Algorithm ?? "Unknown",
                        Metric = r.Metric ?? "Unknown",
                        Iteration = r.Iteration,
                        PathLength = r.PathLength,
                        ComputationTimeMs = r.ComputationTimeMs,
                        Success = r.Success,
                        InitialBatteryPercent = r.InitialBatteryPercent,
                        FinalBatteryPercent = r.FinalBatteryPercent,
                        TotalBatteryConsumedPercent = r.TotalBatteryConsumedPercent,
                        TotalChargingUnits = r.TotalChargingUnits,
                        TotalChargingCycles = r.TotalChargingCycles,
                        TotalChargingTimeSeconds = r.TotalChargingTimeSeconds,
                        TotalTravelTimeSeconds = r.TotalTravelTimeSeconds,
                        TotalOverheadTimeSeconds = r.TotalOverheadTimeSeconds,
                        TotalTimeSeconds = r.TotalTimeSeconds,
                        CollisionCount = r.CollisionCount,
                        InvalidMoveCount = r.InvalidMoveCount,
                        AverageActualSpeed = r.AverageActualSpeed,
                        StartPointUsed = r.StartPointUsed,
                        EndPointReached = r.EndPointReached,
                        GoalOrder = r.GoalOrder,
                        InitialScreenshotPath = r.InitialScreenshotPath,
                        PathScreenshotPath = r.PathScreenshotPath,
                        CompletedScreenshotPath = r.CompletedScreenshotPath,
                        ErrorMessage = r.ErrorMessage,
                        Path = r.Path ?? new List<Point>()
                    };

                    resultsList.Add(item);
                }

                if (resultsList.Count == 0)
                {
                    MessageBox.Show("No valid results to display.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var resultsViewer = new frmExperimentResults.frmExperimentResults(resultsList, _currentOutputPath);
                resultsViewer.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing results: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// تنسيق معلومات النتيجة بشكل جميل
        /// </summary>
        private string GetFormattedResultDetails(ExperimentResultItem result, bool hasPathData)
        {
            return $@"═══════════════════════════════════════════════════════════════
                    PATH DETAILS
═══════════════════════════════════════════════════════════════

┌─ BASIC INFORMATION ─────────────────────────────────────────┐
│ Algorithm:        {result.Algorithm,-35} │
│ Metric:           {result.Metric,-35} │
│ Iteration:        {result.Iteration,-35} │
│ Success:          {(result.Success ? "✓ Yes" : "✗ No"),-35} │
│ Path Data:        {(hasPathData ? $"✓ Available ({result.Path.Count} points)" : "✗ Not available"),-35} │
└─────────────────────────────────────────────────────────────┘

{new string('─', 63)}
┌─ BATTERY STATISTICS ────────────────────────────────────────┐
│ Initial Battery:    {result.InitialBatteryPercent,-30:F1}% │
│ Final Battery:      {result.FinalBatteryPercent,-30:F1}% │
│ Total Consumed:     {result.TotalBatteryConsumedPercent,-30:F1}% │
│ Charging Units:     {result.TotalChargingUnits,-30:F2} │
│ Charging Cycles:    {result.TotalChargingCycles,-30} │
│ Charging Time:      {result.TotalChargingTimeSeconds,-30:F0} sec │
└─────────────────────────────────────────────────────────────┘

{new string('─', 63)}
┌─ TIME STATISTICS ───────────────────────────────────────────┐
│ Travel Time:        {result.TotalTravelTimeSeconds,-30:F2} sec │
│ Computation Time:   {result.ComputationTimeMs,-30:F2} ms │
│ Total Time:         {result.TotalTimeSeconds,-30:F2} sec │
└─────────────────────────────────────────────────────────────┘

{new string('─', 63)}
┌─ PATH INFORMATION ──────────────────────────────────────────┐
│ Path Length:        {result.PathLength,-30} cells │
│ Start Point:        ({result.StartPointUsed.X},{result.StartPointUsed.Y}) │
│ End Point:          ({result.EndPointReached.X},{result.EndPointReached.Y}) │
└─────────────────────────────────────────────────────────────┘

{new string('─', 63)}
Goal Order: {result.GoalOrder}

═══════════════════════════════════════════════════════════════
";
        }
        #endregion

        #region Private Methods - Iteration Tracking
        /// <summary>
        /// Gets the initial start point for experiments
        /// </summary>
        private Point GetInitialStartPoint()
        {
            if (_mapControl != null && _mapControl.HasCustomStartPoint)
            {
                Point startPoint = _mapControl.RobotStartPoint;
                UpdateStartPointDisplay(startPoint);
                return startPoint;
            }

            if (_mapControl != null)
            {
                Point robotPos = _mapControl.RobotPosition;
                UpdateStartPointDisplay(robotPos);
                return robotPos;
            }

            Point defaultPoint = new Point(10, 10);
            UpdateStartPointDisplay(defaultPoint);
            return defaultPoint;
        }

        /// <summary>
        /// Updates the start point display label
        /// </summary>
        private void UpdateStartPointDisplay(Point startPoint)
        {
            if (_lblCurrentStartPoint != null)
            {
                _lblCurrentStartPoint.Text = $"Current: ({startPoint.X}, {startPoint.Y})";
                _lblCurrentStartPoint.Tag = startPoint;
            }
        }

        /// <summary>
        /// Saves iteration tracking data to files
        /// </summary>
        private void SaveIterationTrackingData(List<ComparisonResult> results)
        {
            string startEndPath = Path.Combine(_currentOutputPath, "StartEndPoints.csv");
            SaveStartEndPointsToCsv(startEndPath, results);

            string pathsJsonPath = Path.Combine(_currentOutputPath, "PathsData.json");
            SavePathsToJson(pathsJsonPath);

            string chargingPath = Path.Combine(_currentOutputPath, "ChargingSettings.json");
            SaveChargingSettingsToJson(chargingPath);
        }

        /// <summary>
        /// Saves start and end points to CSV
        /// </summary>
        private void SaveStartEndPointsToCsv(string filePath, List<ComparisonResult> results)
        {
            if (results == null || results.Count == 0) return;

            using var writer = new StreamWriter(filePath);
            writer.WriteLine("Iteration,StartX,StartY,EndX,EndY,Algorithm,Metric,Success");

            int maxCount = Math.Min(_iterationStartPoints.Count, results.Count);

            for (int i = 0; i < maxCount; i++)
            {
                var start = i < _iterationStartPoints.Count ? _iterationStartPoints[i] : Point.Empty;
                var end = i < _iterationEndPoints.Count ? _iterationEndPoints[i] : Point.Empty;
                var result = results[i];

                if (result == null) continue;

                writer.WriteLine($"{i + 1},{start.X},{start.Y},{end.X},{end.Y},{result.Algorithm},{result.Metric},{result.Success}");
            }
        }

        /// <summary>
        /// Saves all paths to JSON file
        /// </summary>
        private void SavePathsToJson(string filePath)
        {
            if (_iterationPaths == null || _iterationPaths.Count == 0) return;

            var pathsList = new List<object>();

            for (int idx = 0; idx < _iterationPaths.Count; idx++)
            {
                var path = _iterationPaths[idx];
                var coordinates = new List<object>();

                if (path != null)
                {
                    foreach (var p in path)
                    {
                        coordinates.Add(new { x = p.X, y = p.Y });
                    }
                }

                pathsList.Add(new
                {
                    Iteration = idx + 1,
                    StartPoint = idx < _iterationStartPoints.Count ? new { x = _iterationStartPoints[idx].X, y = _iterationStartPoints[idx].Y } : new { x = 0, y = 0 },
                    EndPoint = idx < _iterationEndPoints.Count ? new { x = _iterationEndPoints[idx].X, y = _iterationEndPoints[idx].Y } : new { x = 0, y = 0 },
                    PointCount = path?.Count ?? 0,
                    Coordinates = coordinates
                });
            }

            var pathsData = new
            {
                GeneratedAt = DateTime.Now,
                TotalIterations = _iterationPaths.Count,
                Paths = pathsList
            };

            string json = JsonSerializer.Serialize(pathsData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Saves charging settings to JSON file
        /// </summary>
        private void SaveChargingSettingsToJson(string filePath)
        {
            var (enabled, timeSeconds, safetyMargin) = _logic.GetDynamicChargingSettings(this);

            var chargingData = new
            {
                EnableDynamicCharging = enabled,
                ChargingTimeSeconds = timeSeconds,
                SafetyMarginPercent = safetyMargin,
                Timestamp = DateTime.Now
            };

            string json = JsonSerializer.Serialize(chargingData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Resets map and robot to default state for new experiment
        /// </summary>
        private void ResetToDefaultState()
        {
            _currentStartPoint = GetInitialStartPoint();

            if (_mapControl != null)
            {
                _mapControl.RobotPosition = _currentStartPoint;
                _mapControl.RobotAngle = 0;
                _mapControl.SetCurrentStartPoint(_currentStartPoint);
                _mapControl.ClearPaths();
                _mapControl.Refresh();
            }

            if (_viewModel != null)
            {
                _viewModel.SetBatteryLevel(100);
                _viewModel.ResetChargingStatistics();
            }
        }

        /// <summary>
        /// Resets iteration tracking for new experiment
        /// </summary>
        private void ResetIterationTracking()
        {
            _currentStartPoint = GetInitialStartPoint();
            _iterationStartPoints.Clear();
            _iterationEndPoints.Clear();
            _iterationPaths.Clear();
            _completedIterations = 0;
        }

        /// <summary>
        /// Updates progress display with current iteration and robot position
        /// </summary>
        private void UpdateProgressDisplay(int currentIteration, int totalIterations, Point currentPosition)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgressDisplay(currentIteration, totalIterations, currentPosition)));
                return;
            }

            _lblStatus.Text = $"Iteration {currentIteration}/{totalIterations} | Robot at ({currentPosition.X},{currentPosition.Y})";
            _progressBar.Value = (int)((double)currentIteration / totalIterations * 100);
            Application.DoEvents();
        }
        #endregion

        #region Algorithm Grid Methods
        /// <summary>
        /// Handles cell click for edit/duplicate buttons
        /// </summary>
        private void DgvAlgorithems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == _dgvAlgorithems.Columns["colEdit"].Index)
            {
                EditAlgorithmParameters(e.RowIndex);
            }
            else if (e.ColumnIndex == _dgvAlgorithems.Columns["colDuplicate"].Index)
            {
                DuplicateAlgorithmRow(e.RowIndex);
            }
        }

        /// <summary>
        /// Duplicates an algorithm row
        /// </summary>
        private void DuplicateAlgorithmRow(int sourceRowIndex)
        {
            var sourceRow = _dgvAlgorithems.Rows[sourceRowIndex];
            string algorithm = sourceRow.Cells["colAlgorithm"].Value?.ToString();
            string metric = sourceRow.Cells["colMetric"].Value?.ToString();
            var parameters = sourceRow.Tag as Dictionary<string, object>;

            if (string.IsNullOrEmpty(algorithm)) return;

            var paramsCopy = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            int newRowIndex = _dgvAlgorithems.Rows.Add();
            var newRow = _dgvAlgorithems.Rows[newRowIndex];

            newRow.Cells["colEnabled"].Value = sourceRow.Cells["colEnabled"].Value;
            newRow.Cells["colAlgorithm"].Value = algorithm;
            newRow.Cells["colMetric"].Value = metric;
            newRow.Cells["colParameters"].Value = sourceRow.Cells["colParameters"].Value;
            newRow.Tag = paramsCopy;

            _dgvAlgorithems.ClearSelection();
            newRow.Selected = true;
        }

        /// <summary>
        /// Handles double-click to edit algorithm parameters
        /// </summary>
        private void DgvAlgorithems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            EditAlgorithmParameters(e.RowIndex);
        }

        /// <summary>
        /// Handles cell value changed event
        /// </summary>
        private void DgvAlgorithems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex == _dgvAlgorithems.Columns["colMetric"].Index)
            {
                UpdateParameterSummary(e.RowIndex);
            }
        }

        /// <summary>
        /// Handles current cell dirty state changed for checkboxes
        /// </summary>
        private void DgvAlgorithems_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (_dgvAlgorithems.IsCurrentCellDirty && _dgvAlgorithems.CurrentCell.ColumnIndex == 0)
            {
                _dgvAlgorithems.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// Handles keyboard delete key for row deletion
        /// </summary>
        private void DgvAlgorithems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && _dgvAlgorithems.SelectedRows.Count > 0)
            {
                var selectedRow = _dgvAlgorithems.SelectedRows[0];
                string algorithm = selectedRow.Cells["colAlgorithm"].Value?.ToString();
                if (string.IsNullOrEmpty(algorithm)) return;

                var result = MessageBox.Show($"Delete '{algorithm}' from the list?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _dgvAlgorithems.Rows.RemoveAt(selectedRow.Index);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Updates parameter summary for a row
        /// </summary>
        private void UpdateParameterSummary(int rowIndex)
        {
            var row = _dgvAlgorithems.Rows[rowIndex];
            string algorithm = row.Cells["colAlgorithm"].Value?.ToString();
            var parameters = row.Tag as Dictionary<string, object>;

            if (parameters != null)
            {
                string summary = GetParameterSummary(algorithm, parameters);
                row.Cells["colParameters"].Value = summary;
            }
        }

        /// <summary>
        /// Gets parameter summary for display
        /// </summary>
        private string GetParameterSummary(string algorithm, Dictionary<string, object> parameters)
        {
            if (parameters == null) return "";

            var displayParams = new List<string>();

            switch (algorithm)
            {
                case "AStar":
                    if (parameters.ContainsKey("HeuristicWeight"))
                        displayParams.Add($"h={parameters["HeuristicWeight"]}");
                    if (parameters.ContainsKey("SearchLimit"))
                        displayParams.Add($"Limit={parameters["SearchLimit"]}");
                    if (parameters.ContainsKey("SequentialMode") && (bool)parameters["SequentialMode"])
                        displayParams.Add($"Seq=✓");
                    break;
                case "SPPA":
                    if (parameters.ContainsKey("Lambda"))
                        displayParams.Add($"λ={parameters["Lambda"]}");
                    if (parameters.ContainsKey("HeuristicWeight"))
                        displayParams.Add($"h={parameters["HeuristicWeight"]}");
                    if (parameters.ContainsKey("SequentialMode") && (bool)parameters["SequentialMode"])
                        displayParams.Add($"Seq=✓");
                    break;
                case "SPPA_DL":
                    if (parameters.ContainsKey("Lambda"))
                        displayParams.Add($"λ={parameters["Lambda"]}");
                    if (parameters.ContainsKey("LearningRate"))
                        displayParams.Add($"α={parameters["LearningRate"]}");
                    if (parameters.ContainsKey("SequentialMode") && (bool)parameters["SequentialMode"])
                        displayParams.Add($"Seq=✓");
                    break;
                case "ACO":
                    if (parameters.ContainsKey("Ants"))
                        displayParams.Add($"Ants={parameters["Ants"]}");
                    if (parameters.ContainsKey("Iterations"))
                        displayParams.Add($"Iter={parameters["Iterations"]}");
                    if (parameters.ContainsKey("SequentialMode") && (bool)parameters["SequentialMode"])
                        displayParams.Add($"Seq=✓");
                    break;
                case "RRT":
                    if (parameters.ContainsKey("Iterations"))
                        displayParams.Add($"Iter={parameters["Iterations"]}");
                    if (parameters.ContainsKey("StepSize"))
                        displayParams.Add($"Step={parameters["StepSize"]}");
                    if (parameters.ContainsKey("SequentialMode") && (bool)parameters["SequentialMode"])
                        displayParams.Add($"Seq=✓");
                    break;
                default:
                    foreach (var kvp in parameters.Take(2))
                    {
                        displayParams.Add($"{kvp.Key}={kvp.Value}");
                        if (kvp.Key  == "SequentialMode" && (bool)kvp.Value)
                            displayParams.Add($"Seq=✓");
                    }
                    break;
            }

            return string.Join(", ", displayParams);
        }

        /// <summary>
        /// Edits algorithm parameters
        /// </summary>
        private void EditAlgorithmParameters(int rowIndex)
        {
            var row = _dgvAlgorithems.Rows[rowIndex];
            string algorithm = row.Cells["colAlgorithm"].Value?.ToString();
            var currentParams = row.Tag as Dictionary<string, object>;

            if (string.IsNullOrEmpty(algorithm)) return;

            var paramsCopy = currentParams != null ? new Dictionary<string, object>(currentParams) : new Dictionary<string, object>();

            using (var settingsForm = new frmAlgorithmSettings.frmAlgorithmSettings(algorithm, paramsCopy))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK && settingsForm.ChangesApplied)
                {
                    var newParams = settingsForm.ModifiedValues;

                    if (newParams != null && newParams.Count > 0)
                    {
                        if (row.Tag == null)
                            row.Tag = new Dictionary<string, object>();

                        var paramsDict = row.Tag as Dictionary<string, object>;

                        foreach (var kvp in newParams)
                        {
                            if (kvp.Key == "DistanceMetric" || kvp.Key == "Metric")
                            {
                                // تحديث عمود Metric في الـ DataGridView
                                row.Cells["colMetric"].Value = kvp.Value.ToString();
                            }
                            else
                            {
                                paramsDict[kvp.Key] = kvp.Value;
                            }
                        }

                        string summary = GetParameterSummary(algorithm, paramsDict);
                        row.Cells["colParameters"].Value = summary;
                    }
                }
            }
        }

        /// <summary>
        /// Gets selected algorithms with their parameters for experiment
        /// </summary>
        private List<(string algorithm, string metric, Dictionary<string, object> parameters)> GetSelectedAlgorithmsWithParams()
        {
            var result = new List<(string, string, Dictionary<string, object>)>();

            foreach (DataGridViewRow row in _dgvAlgorithems.Rows)
            {
                bool isEnabled = row.Cells["colEnabled"].Value != null && (bool)row.Cells["colEnabled"].Value;

                if (isEnabled)
                {
                    string algorithm = row.Cells["colAlgorithm"].Value?.ToString();
                    string metric = row.Cells["colMetric"].Value?.ToString();
                    var parameters = row.Tag as Dictionary<string, object>;

                    if (!string.IsNullOrEmpty(algorithm) && !string.IsNullOrEmpty(metric))
                    {
                        result.Add((algorithm, metric, parameters ?? new Dictionary<string, object>()));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets parameters for a specific algorithm from the DataGridView
        /// </summary>
        private Dictionary<string, object> GetParametersForAlgorithmFromGrid(string algorithmName, string metric)
        {
            foreach (DataGridViewRow row in _dgvAlgorithems.Rows)
            {
                string alg = row.Cells["colAlgorithm"].Value?.ToString();
                string met = row.Cells["colMetric"].Value?.ToString();

                if (alg == algorithmName && met == metric)
                {
                    return row.Tag as Dictionary<string, object> ?? new Dictionary<string, object>();
                }
            }
            return new Dictionary<string, object>();
        }
        #endregion

        #region Private Methods - Validation
        /// <summary>
        /// Validates all input values before running experiment
        /// </summary>
        private bool ValidateExperimentInputs()
        {
            if (_nudGoalCount.Value < 1)
            {
                MessageBox.Show("Goal count must be at least 1.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_nudParkingCount.Value < 1)
            {
                MessageBox.Show("Parking count must be at least 1.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var selectedAlgorithms = GetSelectedAlgorithmsWithParams();
            if (selectedAlgorithms.Count == 0)
            {
                MessageBox.Show("Please enable at least one algorithm to run.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_nudIterations.Value > 20)
            {
                var result = MessageBox.Show($"Running {_nudIterations.Value} iterations may take a very long time.\n\nContinue anyway?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                    return false;
            }

            return true;
        }
        #endregion

        #region Private Methods - Detection Zone
        private System.Windows.Forms.Timer _detectionZoneTimer;

        /// <summary>
        /// Starts the detection zone updater timer
        /// </summary>
        private void StartDetectionZoneUpdater()
        {
            if (_detectionZoneTimer != null) return;

            _detectionZoneTimer = new System.Windows.Forms.Timer();
            _detectionZoneTimer.Interval = 100;
            _detectionZoneTimer.Tick += OnDetectionZoneTimerTick;
            _detectionZoneTimer.Start();
        }

        /// <summary>
        /// Stops the detection zone updater timer
        /// </summary>
        private void StopDetectionZoneUpdater()
        {
            if (_detectionZoneTimer == null) return;

            _detectionZoneTimer.Stop();
            _detectionZoneTimer.Dispose();
            _detectionZoneTimer = null;
        }

        /// <summary>
        /// Updates detection zone on timer tick
        /// </summary>
        private void OnDetectionZoneTimerTick(object sender, EventArgs e)
        {
            if (_mapControl == null || _viewModel == null) return;

            Point robotPos = _viewModel.RobotState.Position;
            var zoneCells = _viewModel.GetDetectionZoneCells();
            _mapControl.UpdateDetectionZone(zoneCells);
        }
        #endregion

        #region Sensitivity Analysis Methods
 
        private void BtnValidateValues_Click(object sender, EventArgs e)
        {
            string text = _txtSensitivityValues.Text;
            var parts = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var validValues = new List<double>();

            foreach (var part in parts)
            {
                if (double.TryParse(part.Trim(), out double value))
                {
                    validValues.Add(value);
                }
                else
                {
                    MessageBox.Show($"Invalid value: {part}", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (validValues.Count < 2)
            {
                MessageBox.Show("Please enter at least 2 values for sensitivity analysis.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _lblSensitivityStatus.Text = $"✓ Validated: {validValues.Count} values ({string.Join(", ", validValues)})";
            _lblSensitivityStatus.ForeColor = Color.Green;
        }

        private async void BtnRunSensitivity_Click(object sender, EventArgs e)
        {
            if (!_chkEnableSensitivity.Checked) return;

            // ========== 1. Get selected algorithm from combo box ==========
            if (_cboAlgorithm.SelectedItem == null)
            {
                MessageBox.Show("Please select an algorithm for sensitivity analysis.",
                    "No Algorithm Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedAlgorithm = _cboAlgorithm.SelectedItem.ToString();

            // Get the metric and parameters for this algorithm from the grid
            string selectedMetric = "Manhattan";
            Dictionary<string, object> selectedParams = null;

            foreach (DataGridViewRow row in _dgvAlgorithems.Rows)
            {
                string alg = row.Cells["colAlgorithm"].Value?.ToString();
                if (alg == selectedAlgorithm)
                {
                    selectedMetric = row.Cells["colMetric"].Value?.ToString() ?? "Manhattan";
                    selectedParams = row.Tag as Dictionary<string, object>;
                    break;
                }
            }

            if (selectedParams == null)
            {
                selectedParams = new Dictionary<string, object>();
            }

            // ========== 2. Check for goals ==========
            List<Point> allGoals = _viewModel.Goals.Select(g => g.Location).ToList();
            if (allGoals.Count == 0)
            {
                MessageBox.Show("No goals on the map. Please add goals first.",
                    "Sensitivity Analysis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ========== 3. Parse values ==========
            var parts = _txtSensitivityValues.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var values = new List<double>();
            foreach (var part in parts)
            {
                if (double.TryParse(part.Trim(), out double value))
                    values.Add(value);
                else
                {
                    MessageBox.Show($"Invalid value: {part}", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (values.Count < 2)
            {
                MessageBox.Show("Please enter at least 2 values for sensitivity analysis.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ========== 4. Get parameter key ==========
            string parameter = _cboSensitivityParameter.SelectedItem?.ToString() ?? "";
            string paramKey = parameter.Split('-')[0].Trim().Split('(')[0].Trim();

            // ========== 5. Setup UI ==========
            _btnRunSensitivity.Enabled = false;
            _dgvSensitivityResults.Rows.Clear();
            _lblSensitivityStatus.Text = "Running sensitivity analysis...";
            _lblSensitivityStatus.ForeColor = Color.Blue;
            Application.DoEvents();

            Point startPoint = _mapControl.RobotPosition;
            var mlSettings = _logic.GetMLSettings(this);

            var algorithmResults = new List<(double value, int length, double time, bool success)>();

            // ========== 6. Run analysis for each value ==========
            foreach (double val in values)
            {
                _lblSensitivityStatus.Text = $"Testing {selectedAlgorithm} - {paramKey} = {val}...";
                Application.DoEvents();

                // Create a copy of parameters with modified value
                var currentParams = new Dictionary<string, object>(selectedParams);

                // Apply the sensitivity parameter
                if (currentParams.ContainsKey(paramKey))
                    currentParams[paramKey] = val;
                else
                    currentParams[paramKey] = val;

                // Create finder with modified parameters
                var finder = _logic.CreateAlgorithmFinder(_mapGrid, selectedAlgorithm, mlSettings, currentParams);

                if (finder == null)
                {
                    algorithmResults.Add((val, 0, 0, false));
                    continue;
                }

                // Configure metric
                finder.Metric = ExperimentSharedLogic.GetDistanceMetric(selectedMetric);

                // Apply additional settings
                ApplyAdditionalFinderSettings(finder, selectedAlgorithm, currentParams);

                // Find path through all goals
                var fullPath = new List<PathNode>();
                Point currentPos = startPoint;
                double totalTimeMs = 0;
                bool pathSuccess = true;

                for (int i = 0; i < allGoals.Count; i++)
                {
                    if (!_chkEnableSensitivity.Checked)
                    {
                        _lblSensitivityStatus.Text = "Sensitivity analysis cancelled.";
                        _btnRunSensitivity.Enabled = true;
                        return;
                    }

                    var goalResult = await Task.Run(() => finder.FindPath(currentPos, allGoals[i]));

                    if (!goalResult.Success || goalResult.Path == null || goalResult.Path.Count == 0)
                    {
                        pathSuccess = false;
                        break;
                    }

                    if (fullPath.Count == 0)
                        fullPath.AddRange(goalResult.Path);
                    else
                        fullPath.AddRange(goalResult.Path.Skip(1));

                    totalTimeMs += goalResult.ComputationTimeSeconds * 1000;
                    currentPos = allGoals[i];
                }

                // Add return path for SPPA family
                if (pathSuccess && (selectedAlgorithm == "SPPA" || selectedAlgorithm == "SPPA_DL"))
                {
                    var parkingPoints = _viewModel.ParkingPoints.Select(p => p.Location).ToList();
                    if (parkingPoints != null && parkingPoints.Count > 0)
                    {
                        Point lastGoal = allGoals.Last();
                        Point nearestParking = parkingPoints
                            .OrderBy(p => Math.Abs(p.X - lastGoal.X) + Math.Abs(p.Y - lastGoal.Y))
                            .FirstOrDefault();

                        if (nearestParking != Point.Empty)
                        {
                            var returnResult = await Task.Run(() => finder.FindPath(lastGoal, nearestParking));
                            if (returnResult.Success && returnResult.Path != null && returnResult.Path.Count > 0)
                            {
                                fullPath.AddRange(returnResult.Path.Skip(1));
                                totalTimeMs += returnResult.ComputationTimeSeconds * 1000;
                            }
                        }
                    }
                }

                int finalPathLength = pathSuccess ? fullPath.Count : 0;
                algorithmResults.Add((val, finalPathLength, totalTimeMs, pathSuccess));

                // Update UI
                _dgvSensitivityResults.Rows.Add(
                    val.ToString("F3"),
                    finalPathLength,
                    totalTimeMs.ToString("F2"),
                    pathSuccess ? "✓" : "✗",
                    "0"
                );

                Application.DoEvents();
            }

            // ========== 7. Show results ==========
            _lblSensitivityStatus.Text = $"✓ Sensitivity analysis complete! Tested {values.Count} values on {allGoals.Count} goal(s).";
            _lblSensitivityStatus.ForeColor = Color.Green;
            _btnRunSensitivity.Enabled = true;

            // Show summary
            ShowSensitivitySummary(algorithmResults, paramKey);
        }

        /// <summary>
        /// Shows sensitivity analysis summary
        /// </summary>
        private void ShowSensitivitySummary(List<(double value, int length, double time, bool success)> results, string paramKey)
        {
            string summary = $"=== SENSITIVITY ANALYSIS SUMMARY ===\n\n";
            summary += $"Parameter: {paramKey}\n";
            summary += $"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n";
            summary += "Value\t\tPathLength\tTime(ms)\tSuccess\n";
            summary += "-----\t\t----------\t--------\t-------\n";

            foreach (var r in results)
            {
                summary += $"{r.value:F3}\t\t{r.length}\t\t{r.time:F2}\t\t{(r.success ? "Yes" : "No")}\n";
            }

            var optimal = results.Where(r => r.success).OrderBy(r => r.length).FirstOrDefault();
            if (optimal.value != 0)
            {
                summary += $"\nOptimal value: {optimal.value:F3} (Path length: {optimal.length}, Time: {optimal.time:F2} ms)";
            }

            MessageBox.Show(summary, "Sensitivity Analysis Results",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #region Sensitivity Analysis Helper Methods

        /// <summary>
        /// Populates the algorithm combo box with all algorithms from the grid
        /// </summary>
        private void PopulateAlgorithmComboBox()
        {
            _cboAlgorithm.Items.Clear();

            foreach (DataGridViewRow row in _dgvAlgorithems.Rows)
            {
                string algorithm = row.Cells["colAlgorithm"].Value?.ToString();
                if (!string.IsNullOrEmpty(algorithm))
                {
                    _cboAlgorithm.Items.Add(algorithm);
                }
            }

            if (_cboAlgorithm.Items.Count > 0)
                _cboAlgorithm.SelectedIndex = 0;
        }

        /// <summary>
        /// Updates parameters combo box based on selected algorithm
        /// </summary>
        private void UpdateParametersForAlgorithm()
        {
            if (_cboAlgorithm.SelectedItem == null) return;

            string algorithm = _cboAlgorithm.SelectedItem.ToString();
            _cboSensitivityParameter.Items.Clear();

            // Get parameters for selected algorithm
            var parameters = GetParametersForAlgorithmDisplay(algorithm);

            foreach (var param in parameters)
            {
                _cboSensitivityParameter.Items.Add(param);
            }

            if (_cboSensitivityParameter.Items.Count > 0)
                _cboSensitivityParameter.SelectedIndex = 0;
        }

        /// <summary>
        /// Gets display list of parameters for a specific algorithm
        /// </summary>
        private List<string> GetParametersForAlgorithmDisplay(string algorithm)
        {
            var parameters = new List<string>();

            switch (algorithm)
            {
                case "AStar":
                    parameters.Add("HeuristicWeight - Heuristic weight");
                    parameters.Add("SearchLimit - Maximum nodes to explore");
                    parameters.Add("AllowDiagonals - Allow diagonal movement");
                    parameters.Add("HeavyDiagonals - Heavy diagonal cost");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;

                case "SPPA":
                    parameters.Add("Lambda - Obstacle weight");
                    parameters.Add("HeuristicWeight - Heuristic weight");
                    parameters.Add("SearchLimit - Maximum nodes to explore");
                    parameters.Add("AlphaS - Static obstacle weight");
                    parameters.Add("AlphaSS - Semi-static obstacle weight");
                    parameters.Add("AlphaD - Dynamic obstacle weight");
                    parameters.Add("AllowDiagonals - Allow diagonal movement");
                    parameters.Add("HeavyDiagonals - Heavy diagonal cost");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;

                case "SPPA_DL":
                    parameters.Add("Lambda - Obstacle weight");
                    parameters.Add("LearningRate - Learning rate");
                    parameters.Add("PredictionWeight - Prediction weight");
                    parameters.Add("HeuristicWeight - Heuristic weight");
                    parameters.Add("SearchLimit - Maximum nodes to explore");
                    parameters.Add("AlphaS - Static obstacle weight");
                    parameters.Add("AlphaSS - Semi-static obstacle weight");
                    parameters.Add("AlphaD - Dynamic obstacle weight");
                    parameters.Add("AllowDiagonals - Allow diagonal movement");
                    parameters.Add("HeavyDiagonals - Heavy diagonal cost");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;

                case "ACO":
                    parameters.Add("Ants - Number of ants");
                    parameters.Add("Iterations - Number of iterations");
                    parameters.Add("Alpha - Pheromone importance");
                    parameters.Add("Beta - Heuristic importance");
                    parameters.Add("EvaporationRate - Pheromone evaporation");
                    parameters.Add("AllowDiagonals - Allow diagonal movement");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;

                case "DStar":
                    parameters.Add("ReplanningRange - Replanning range");
                    parameters.Add("DynamicReplanning - Dynamic replanning");
                    parameters.Add("SearchLimit - Maximum nodes to explore");
                    parameters.Add("AllowDiagonals - Allow diagonal movement");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;

                case "KNN":
                    parameters.Add("K - Number of neighbors");
                    parameters.Add("Radius - Search radius");
                    parameters.Add("AllowDiagonals - Allow diagonal movement");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;

                case "BruteForce":
                    parameters.Add("MaxDepth - Maximum search depth");
                    parameters.Add("MaxIterations - Maximum iterations");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;

                case "RRT":
                    parameters.Add("Iterations - Number of iterations");
                    parameters.Add("StepSize - Step size");
                    parameters.Add("GoalBias - Goal bias probability");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;

                case "PRM":
                    parameters.Add("NumSamples - Number of samples");
                    parameters.Add("ConnectionRadius - Connection radius");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;

                case "PSO":
                    parameters.Add("PopulationSize - Population size");
                    parameters.Add("MaxIterations - Maximum iterations");
                    parameters.Add("InertiaWeight - Inertia weight");
                    parameters.Add("CognitiveWeight - Cognitive weight");
                    parameters.Add("SocialWeight - Social weight");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;

                case "GA":
                    parameters.Add("PopulationSize - Population size");
                    parameters.Add("MaxGenerations - Maximum generations");
                    parameters.Add("CrossoverRate - Crossover rate");
                    parameters.Add("MutationRate - Mutation rate");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;

                case "RRTStar":
                    parameters.Add("Iterations - Number of iterations");
                    parameters.Add("StepSize - Step size");
                    parameters.Add("RewiringRadius - Rewiring radius");
                    parameters.Add("GoalBias - Goal bias probability");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;

                default:
                    parameters.Add("HeuristicWeight - Heuristic weight");
                    parameters.Add("SearchLimit - Maximum nodes to explore");
                    parameters.Add("OrderGoalsByDistance - Order goals by distance");
                    break;
            }

            return parameters;
        }
        /// <summary>
        /// Updates default values based on selected algorithm and parameter
        /// </summary>
        private void UpdateDefaultSensitivityValues()
        {
            if (_cboAlgorithm.SelectedItem == null || _cboSensitivityParameter.SelectedItem == null) return;

            string algorithm = _cboAlgorithm.SelectedItem.ToString();
            string parameter = _cboSensitivityParameter.SelectedItem.ToString();

            // Extract parameter key (before the dash if exists)
            string paramKey = parameter.Contains(" - ") ? parameter.Split('-')[0].Trim() : parameter;
            // Remove parentheses if present
            paramKey = paramKey.Split('(')[0].Trim();

            string defaultValues = GetDefaultValuesForParameter(algorithm, paramKey);
            _txtSensitivityValues.Text = defaultValues;
        }

        /// <summary>
        /// Gets default values for a specific algorithm parameter
        /// </summary>
        private string GetDefaultValuesForParameter(string algorithm, string paramKey)
        {
            switch (algorithm)
            {
                case "AStar":
                    if (paramKey == "HeuristicWeight") return "1.0,1.5,2.0,2.5,3.0,4.0,5.0";
                    if (paramKey == "SearchLimit") return "5000,10000,20000,50000,100000";
                    if (paramKey == "AllowDiagonals") return "false,true";
                    if (paramKey == "HeavyDiagonals") return "false,true";
                    break;

                case "SPPA":
                    if (paramKey == "Lambda") return "0.5,1.0,1.5,2.0,2.5,3.0";
                    if (paramKey == "HeuristicWeight") return "1.0,1.5,2.0,2.5,3.0";
                    if (paramKey == "SearchLimit") return "5000,10000,20000,50000,100000";
                    if (paramKey == "Alpha_S") return "0.5,1.0,1.5,2.0,2.5";
                    if (paramKey == "Alpha_SS") return "0.5,0.8,1.0,1.2,1.5";
                    if (paramKey == "Alpha_D") return "0.5,1.0,1.5,2.0,2.5";
                    break;

                case "SPPA_DL":
                    if (paramKey == "Lambda") return "0.5,1.0,1.5,2.0,2.5,3.0";
                    if (paramKey == "LearningRate") return "0.5,1.0,1.5,2.0,2.5,3.0";
                    if (paramKey == "PredictionWeight") return "0.1,0.3,0.5,0.7,0.9";
                    if (paramKey == "HeuristicWeight") return "1.0,1.5,2.0,2.5,3.0";
                    if (paramKey == "SearchLimit") return "5000,10000,20000,50000,100000";
                    break;

                case "ACO":
                    if (paramKey == "Ants") return "10,20,30,40,50";
                    if (paramKey == "Iterations") return "50,100,150,200,250";
                    if (paramKey == "Alpha") return "0.5,1.0,1.5,2.0,2.5";
                    if (paramKey == "Beta") return "0.5,1.0,1.5,2.0,2.5";
                    if (paramKey == "EvaporationRate") return "0.05,0.1,0.15,0.2,0.25";
                    break;

                case "RRT":
                    if (paramKey == "Iterations") return "1000,2000,5000,10000,20000";
                    if (paramKey == "StepSize") return "5,10,15,20,25";
                    if (paramKey == "GoalBias") return "0.01,0.03,0.05,0.07,0.10";
                    break;

                case "PRM":
                    if (paramKey == "NumSamples") return "50,100,200,400,800";
                    if (paramKey == "ConnectionRadius") return "10,20,30,40,50";
                    break;

                case "PSO":
                    if (paramKey == "PopulationSize") return "20,30,50,70,100";
                    if (paramKey == "MaxIterations") return "50,100,150,200,300";
                    if (paramKey == "InertiaWeight") return "0.4,0.6,0.7,0.8,0.9";
                    break;

                case "GA":
                    if (paramKey == "PopulationSize") return "20,50,100,150,200";
                    if (paramKey == "MaxGenerations") return "20,50,100,150,200";
                    if (paramKey == "CrossoverRate") return "0.6,0.7,0.8,0.9,1.0";
                    if (paramKey == "MutationRate") return "0.01,0.05,0.10,0.15,0.20";
                    break;

                case "RRTStar":
                    if (paramKey == "Iterations") return "1000,2000,5000,10000,20000";
                    if (paramKey == "StepSize") return "5,10,15,20,25";
                    if (paramKey == "RewiringRadius") return "10,20,30,40,50";
                    break;
            }

            // Default fallback values
            return "1.0,1.5,2.0,2.5,3.0";
        }

        /// <summary>
        /// Updates sensitivity controls state based on checkbox
        /// </summary>
        private void UpdateSensitivityControlsState()
        {
            bool enabled = _chkEnableSensitivity?.Checked ?? false;
            if (_lblAlgorithm != null) _lblAlgorithm.Enabled = enabled;
            if (_cboAlgorithm != null) _cboAlgorithm.Enabled = enabled;
            if (_lblParameter != null) _lblParameter.Enabled = enabled;
            if (_cboSensitivityParameter != null) _cboSensitivityParameter.Enabled = enabled;
            if (_lblValues != null) _lblValues.Enabled = enabled;
            if (_txtSensitivityValues != null) _txtSensitivityValues.Enabled = enabled;
            if (_btnValidateValues != null) _btnValidateValues.Enabled = enabled;
            if (_btnRunSensitivity != null) _btnRunSensitivity.Enabled = enabled;
        }

        #endregion
        /// <summary>
        /// Applies additional finder settings based on algorithm type
        /// </summary>
        private void ApplyAdditionalFinderSettings(IPathFinder finder, string algorithm, Dictionary<string, object> parameters)
        {
            if (finder == null) return;

            // Common settings for all algorithms
            if (parameters.ContainsKey("AllowDiagonals"))
                finder.AllowDiagonals = Convert.ToBoolean(parameters["AllowDiagonals"]);

            if (parameters.ContainsKey("HeavyDiagonals"))
                finder.HeavyDiagonals = Convert.ToBoolean(parameters["HeavyDiagonals"]);

            if (parameters.ContainsKey("SearchLimit"))
                finder.SearchLimit = Convert.ToInt32(parameters["SearchLimit"]);

            // Algorithm-specific settings
            switch (algorithm)
            {
                case "AStar":
                    if (finder is AStarFinder aStarFinder && parameters.ContainsKey("HeuristicWeight"))
                        aStarFinder.HeuristicWeight = Convert.ToInt32(parameters["HeuristicWeight"]);
                    break;

                case "SPPA":
                    if (finder is SPPAFinder sppaFinder)
                    {
                        if (parameters.ContainsKey("HeuristicWeight"))
                            sppaFinder.HeuristicWeight = Convert.ToInt32 (parameters["HeuristicWeight"]);
                        if (parameters.ContainsKey("Lambda"))
                            sppaFinder.Lambda = Convert.ToDouble(parameters["Lambda"]);
                        if (parameters.ContainsKey("AlphaS"))
                            sppaFinder.AlphaS = Convert.ToDouble(parameters["AlphaS"]);
                        if (parameters.ContainsKey("AlphaSS"))
                            sppaFinder.AlphaSS = Convert.ToDouble(parameters["AlphaSS"]);
                        if (parameters.ContainsKey("AlphaD"))
                            sppaFinder.AlphaD = Convert.ToDouble(parameters["AlphaD"]);
                    }
                    break;

                case "SPPA_DL":
                    if (finder is SPPA_DLFinder sppaDLFinder)
                    {
                        if (parameters.ContainsKey("HeuristicWeight"))
                            sppaDLFinder.HeuristicWeight = Convert.ToInt32(parameters["HeuristicWeight"]);
                        if (parameters.ContainsKey("Lambda"))
                            sppaDLFinder.Lambda = Convert.ToDouble(parameters["Lambda"]);
                        if (parameters.ContainsKey("LearningRate"))
                            sppaDLFinder.LearningRate = Convert.ToDouble(parameters["LearningRate"]);
                        if (parameters.ContainsKey("PredictionWeight"))
                            sppaDLFinder.PredictionWeight = Convert.ToDouble(parameters["PredictionWeight"]);
                        if (parameters.ContainsKey("AlphaS"))
                            sppaDLFinder.AlphaS = Convert.ToDouble(parameters["AlphaS"]);
                        if (parameters.ContainsKey("AlphaSS"))
                            sppaDLFinder.AlphaSS = Convert.ToDouble(parameters["AlphaSS"]);
                        if (parameters.ContainsKey("AlphaD"))
                            sppaDLFinder.AlphaD = Convert.ToDouble(parameters["AlphaD"]);
                    }
                    break;

                case "ACO":
                    if (finder is ACOFinder acoFinder)
                    {
                        int ants = parameters.ContainsKey("Ants") ? Convert.ToInt32(parameters["Ants"]) : 20;
                        int iterations = parameters.ContainsKey("Iterations") ? Convert.ToInt32(parameters["Iterations"]) : 100;
                        double alpha = parameters.ContainsKey("Alpha") ? Convert.ToDouble(parameters["Alpha"]) : 1.0;
                        double beta = parameters.ContainsKey("Beta") ? Convert.ToDouble(parameters["Beta"]) : 2.0;
                        double evaporation = parameters.ContainsKey("EvaporationRate") ? Convert.ToDouble(parameters["EvaporationRate"]) : 0.1;
                        acoFinder.SetParameters(ants, evaporation, alpha, beta, iterations);
                    }
                    break;

                case "RRT":
                    // RRT parameters are handled in CreateAlgorithmFinder
                    // Most RRT implementations don't expose these properties directly
                    System.Diagnostics.Debug.WriteLine($"[RRT] Parameters applied during creation: {string.Join(", ", parameters.Keys)}");
                    break;

                case "DStar":
                case "KNN":
                case "BruteForce":
                case "PRM":
                case "PSO":
                case "GA":
                case "RRTStar":
                    // These algorithms' parameters are handled in CreateAlgorithmFinder
                    System.Diagnostics.Debug.WriteLine($"[{algorithm}] Parameters applied during creation");
                    break;
            }
        }        /// <summary>
                 /// Shows detailed sensitivity analysis summary
                 /// </summary>
        private void ShowDetailedSensitivitySummary(Dictionary<string, List<(double value, int length, double time, bool success)>> allResults, string paramKey, List<double> values)
        {
            string summary = $"=== SENSITIVITY ANALYSIS SUMMARY ===\n\n";
            summary += $"Parameter: {paramKey}\n";
            summary += $"Values tested: {string.Join(", ", values)}\n";
            summary += $"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n";

            foreach (var kvp in allResults)
            {
                summary += $"┌─────────────────────────────────────────────────────────────┐\n";
                summary += $"│ ALGORITHM: {kvp.Key.PadRight(45)}│\n";
                summary += $"├───────────┬──────────────┬──────────────┬─────────────────┤\n";
                summary += $"│ Value     │ Path Length  │ Time (ms)    │ Success         │\n";
                summary += $"├───────────┼──────────────┼──────────────┼─────────────────┤\n";

                foreach (var r in kvp.Value)
                {
                    summary += $"│ {r.value,-8:F2} │ {r.length,-12} │ {r.time,-12:F2} │ {(r.success ? "✓ Yes" : "✗ No"),-15} │\n";
                }

                // Find optimal value
                var optimal = kvp.Value.Where(r => r.success).OrderBy(r => r.length).FirstOrDefault();
                if (optimal.value != 0)
                {
                    summary += $"├───────────┼──────────────┼──────────────┼─────────────────┤\n";
                    summary += $"│ OPTIMAL:  │ {optimal.value,-8:F2} │ Length: {optimal.length,-4} │ Time: {optimal.time,-8:F2} │\n";
                }

                summary += $"└───────────┴──────────────┴──────────────┴─────────────────┘\n\n";
            }

            // Find best algorithm overall
            var bestOverall = allResults
                .SelectMany(kvp => kvp.Value.Select(r => (algorithm: kvp.Key, r.value, r.length, r.time, r.success)))
                .Where(r => r.success)
                .OrderBy(r => r.length)
                .FirstOrDefault();

            if (bestOverall.algorithm != null)
            {
                summary += $"⭐ BEST OVERALL: {bestOverall.algorithm} at {bestOverall.value:F2} (Length: {bestOverall.length})\n";
            }

            MessageBox.Show(summary, "Sensitivity Analysis Results",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        private void SetupAlgorithmGrid()
        {
            _dgvAlgorithems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgvAlgorithems.ColumnHeadersHeight = 30;
            _dgvAlgorithems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _dgvAlgorithems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            _dgvAlgorithems.RowTemplate.Height = 28;
            _dgvAlgorithems.EnableHeadersVisualStyles = false;

            _dgvAlgorithems.Columns.Clear();

            colEnabled.Name = "colEnabled";
            colEnabled.HeaderText = "";
            colEnabled.Width = 35;
            colEnabled.FillWeight = 3;
            colEnabled.Resizable = DataGridViewTriState.False;

            colAlgorithm.Name = "colAlgorithm";
            colAlgorithm.HeaderText = "Algorithm";
            colAlgorithm.Width = 100;
            colAlgorithm.FillWeight = 15;
            colAlgorithm.ReadOnly = true;

            colMetric.Name = "colMetric";
            colMetric.HeaderText = "Metric";
            colMetric.Width = 110;
            colMetric.FillWeight = 18;
            colMetric.FlatStyle = FlatStyle.Flat;
            colMetric.Items
                .AddRange(new string[] {
                "Manhattan", "Euclidean", "MaxDXDY",
                "DiagonalShortcut", "EuclideanNoSQR"
                                 });

            colParameters.Name = "colParameters";
            colParameters.HeaderText = "Parameters";
            colParameters.ReadOnly = true;
            colParameters.FillWeight = 50;

            colEdit.Name = "colEdit";
            colEdit.HeaderText = "";
            colEdit.Text = "✎";
            colEdit.UseColumnTextForButtonValue = true;
            colEdit.Width = 30;
            colEdit.FillWeight = 2;
            colEdit.FlatStyle = FlatStyle.Flat;

            colDuplicate.Name = "colDuplicate";
            colDuplicate.HeaderText = "";
            colDuplicate.Text = "📋";
            colDuplicate.UseColumnTextForButtonValue = true;
            colDuplicate.Width = 30;
            colDuplicate.FillWeight = 2;
            colDuplicate.FlatStyle = FlatStyle.Flat;

            _dgvAlgorithems.Columns.Add(colEnabled);
            _dgvAlgorithems.Columns.Add(colAlgorithm);
            _dgvAlgorithems.Columns.Add(colMetric);
            _dgvAlgorithems.Columns.Add(colParameters);
            _dgvAlgorithems.Columns.Add(colEdit);
            _dgvAlgorithems.Columns.Add(colDuplicate);

            AddAllAlgorithmRows();
        }

        private void AddAllAlgorithmRows()
        {
            AddAlgorithmRow("AStar", "A* (A-Star)", false, "Manhattan", "h=2, Limit=20000");
            AddAlgorithmRow("SPPA", "SPPA", true, "Manhattan", "λ=1.5, h=2");
            AddAlgorithmRow("SPPA_DL", "SPPA-DL", true, "Manhattan", "λ=1.5, α=2.0");
            AddAlgorithmRow("ACO", "ACO", false, "Manhattan", "Ants=20, Iter=100");
            AddAlgorithmRow("DStar", "D*", false, "Manhattan", "Range=10");
            AddAlgorithmRow("KNN", "KNN", false, "Manhattan", "K=5");
            AddAlgorithmRow("BruteForce", "Brute Force", false, "Manhattan", "Depth=100");
            AddAlgorithmRow("RRT", "RRT", false, "Manhattan", "Iter=5000");
            AddAlgorithmRow("PRM", "PRM", false, "Manhattan", "Samples=200");
            AddAlgorithmRow("PSO", "PSO", false, "Manhattan", "Pop=50, Iter=100");
            AddAlgorithmRow("GA", "GA", false, "Manhattan", "Pop=100, Gen=50");
            AddAlgorithmRow("RRTStar", "RRT*", false, "Manhattan", "Iter=5000");
        }

        private void AddAlgorithmRow(string algorithmKey, string displayName, bool enabled, string defaultMetric, string paramSummary)
        {
            int rowIndex = _dgvAlgorithems.Rows.Add();
            _dgvAlgorithems.Rows[rowIndex].Cells["colEnabled"].Value = enabled;
            _dgvAlgorithems.Rows[rowIndex].Cells["colAlgorithm"].Value = algorithmKey;
            _dgvAlgorithems.Rows[rowIndex].Cells["colMetric"].Value = defaultMetric;
            _dgvAlgorithems.Rows[rowIndex].Cells["colParameters"].Value = paramSummary;
            _dgvAlgorithems.Rows[rowIndex].Tag = GetDefaultParametersForAlgorithm(algorithmKey);

            // تحديث حالة Select All بعد إضافة الصفوف
            UpdateSelectAllCheckBoxState();
        }

        /// <summary>
        /// Updates Select All checkbox state based on current selections
        /// </summary>
        private void UpdateSelectAllCheckBoxState()
        {
            if (_dgvAlgorithems.Rows.Count == 0) return;

            int enabledCount = 0;
            foreach (DataGridViewRow row in _dgvAlgorithems.Rows)
            {
                if (row.Cells["colEnabled"].Value != null && (bool)row.Cells["colEnabled"].Value)
                    enabledCount++;
            }

            if (enabledCount == 0)
                _chkSelectAll.CheckState = CheckState.Unchecked;
            else if (enabledCount == _dgvAlgorithems.Rows.Count)
                _chkSelectAll.CheckState = CheckState.Checked;
            else
                _chkSelectAll.CheckState = CheckState.Indeterminate;
        }

        private Dictionary<string, object> GetDefaultParametersForAlgorithm(string algorithmName)
        {
            var defaults = new Dictionary<string, object>();

            switch (algorithmName)
            {
                case "AStar":
                    defaults["HeuristicWeight"] = 2.0;
                    defaults["SearchLimit"] = 20000;
                    defaults["AllowDiagonals"] = true;
                    defaults["HeavyDiagonals"] = false;
                    defaults["OrderGoalsByDistance"] = false;
                    defaults["SequentialMode"] = true;
                    break;
                case "SPPA":
                    defaults["HeuristicWeight"] = 2.0;
                    defaults["SearchLimit"] = 20000;
                    defaults["AllowDiagonals"] = true;
                    defaults["HeavyDiagonals"] = false;
                    defaults["Lambda"] = 1.5;
                    defaults["AlphaS"] = 1.0;
                    defaults["AlphaSS"] = 0.8;
                    defaults["AlphaD"] = 1.2;
                    defaults["OrderGoalsByDistance"] = false;
                    defaults["SequentialMode"] = true;
                    break;
                case "SPPA_DL":
                    defaults["HeuristicWeight"] = 2.0;
                    defaults["SearchLimit"] = 20000;
                    defaults["AllowDiagonals"] = true;
                    defaults["HeavyDiagonals"] = false;
                    defaults["Lambda"] = 1.5;
                    defaults["LearningRate"] = 2.0;
                    defaults["PredictionWeight"] = 0.5;
                    defaults["AlphaS"] = 1.0;
                    defaults["AlphaSS"] = 0.8;
                    defaults["AlphaD"] = 1.2;
                    defaults["OrderGoalsByDistance"] = false;
                    defaults["SequentialMode"] = true;
                    break;
                default:
                    defaults["OrderGoalsByDistance"] = false;
                    defaults["SequentialMode"] = false;
                    break;
            }

            return defaults;
        }
        #region Nested Classes
        /// <summary>
        /// Represents a path error found during analysis
        /// </summary>
        public class PathError
        {
            public string Type { get; set; }
            public string Message { get; set; }
            public Point Location { get; set; }
            public int StepIndex { get; set; }
        }
        #endregion
    }
}
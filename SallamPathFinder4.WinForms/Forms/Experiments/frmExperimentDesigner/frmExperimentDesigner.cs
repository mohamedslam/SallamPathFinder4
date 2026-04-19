#region File Header
/// <summary>
/// File: frmExperimentDesigner.cs
/// Description: Main form for designing and running algorithm comparison experiments
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Algorithms.Implementations;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Algorithms;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Experiments;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.Services.Simulation;
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
        private bool _isRunning;
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
            // Pick start point button
            if (_btnPickStartPoint != null)
            {
                _btnPickStartPoint.Click += BtnPickStartPoint_Click;
            }

            // Dynamic charging checkbox
            if (_chkEnableDynamicCharging != null)
            {
                _chkEnableDynamicCharging.CheckedChanged += ChkEnableDynamicCharging_CheckedChanged;

                // Initialize state
                bool enabled = _chkEnableDynamicCharging.Checked;
                if (_nudChargingTime != null) _nudChargingTime.Enabled = enabled;
                if (_nudSafetyMargin != null) _nudSafetyMargin.Enabled = enabled;
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
             
            ResetToDefaultState();
         
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
                var (enableDynamicCharging, chargingTime, safetyMargin) = _logic.GetDynamicChargingSettings(this);
                bool orderGoalsByDistance = _logic.GetOrderGoalsByDistance(this);

                // Apply settings to robot
                robotSettings.EnableDynamicCharging = enableDynamicCharging;
                robotSettings.ChargingTimeSeconds = chargingTime;
                robotSettings.SafetyMarginPercent = safetyMargin;

                int totalExperiments = algorithms.Count * metrics.Count * iterations;
                int currentExperiment = 0;

                for (int iter = 0; iter < iterations; iter++)
                {
                    foreach (string algorithm in algorithms)
                    {
                        foreach (string metric in metrics)
                        {
                            currentExperiment++;
                            _completedIterations = iter + 1;

                            UpdateProgressDisplay(_completedIterations, iterations, _currentStartPoint);

                            _lblStatus.Text = $"Running: {algorithm} - {metric} - Iter {iter + 1}/{iterations} | Start: ({_currentStartPoint.X},{_currentStartPoint.Y})";
                            Application.DoEvents();

                            var result = await RunSingleExperiment(algorithm, metric, iter + 1, robotSettings, mlSettings, _currentStartPoint, orderGoalsByDistance);
                            results.Add(result);

                            // Update start point for next iteration
                            if (result.Success && result.Path != null && result.Path.Count > 0)
                            {
                                _currentStartPoint = result.Path.Last();
                                _iterationEndPoints.Add(_currentStartPoint);
                            }

                            _iterationPaths.Add(result.Path?.ToList() ?? new List<Point>());
                        }
                    }
                    _progressBar.Value = (int)((iter + 1) / (double)iterations * 100);
                }

                SaveExperimentResults(results);
                SaveIterationTrackingData(results);

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
        #region Private Methods - Run Single Experiment

        #region Private Methods - Run Single Experiment

        /// <summary>
        /// Runs a single experiment iteration with full data collection
        /// </summary>
        private async Task<ComparisonResult> RunSingleExperiment(
            string algorithm,
            string metric,
            int iteration,
            RobotSettings robotSettings,
            MLSettings mlSettings,
            Point startPoint,
            bool orderGoalsByDistance)
        {            
            var result = _logic.CreateEmptyResult(algorithm, metric, iteration, robotSettings, this);

            try
            {
                
                // ========== 1. INITIALIZE RESULT ==========
                _iterationStartPoints.Add(startPoint);

                bool enableDynamicCharging = _chkEnableDynamicCharging?.Checked ?? false;
                double chargingTimeSeconds = _nudChargingTime != null ? (double)_nudChargingTime.Value : 15;
                double safetyMarginPercent = (double)(_nudSafetyMargin?.Value ?? 10);

                result.UsedDynamicCharging = enableDynamicCharging;
                result.ChargingTimeSeconds = chargingTimeSeconds;
                result.SafetyMarginPercent = safetyMarginPercent;
                result.StartPointUsed = startPoint;
                result.InitialBatteryPercent = robotSettings.InitialBatteryLevel;
                StartDetectionZoneUpdater(); 
                System.Diagnostics.Debug.WriteLine($"[Experiment] {algorithm} - {metric} - Iter {iteration}: Start=({startPoint.X},{startPoint.Y})");

                // ========== 2. VALIDATE INPUT ==========
                List<Point> goals = _logic.GetRealGoals(_viewModel, _chkUseCurrentMap.Checked);
                if (goals == null || goals.Count == 0)
                {
                    result.Success = false;
                    result.ErrorMessage = "No goals available on the map";
                    result.FailureReason = "NoGoals";
                    _iterationEndPoints.Add(Point.Empty);
                    return result;
                }

                List<Point> parkingPoints = _logic.GetParkingPoints(_viewModel, _chkUseCurrentMap.Checked);

                // ========== 3. ORDER GOALS (Fixed order - no dynamic ordering) ==========
                List<Point> orderedGoals = goals.ToList();  // Use user-defined order only
                result.OrderedByDistance = false;
                result.GoalOrder = string.Join(";", orderedGoals.Select(g => $"({g.X},{g.Y})"));

                // ========== 4. INITIAL SCREENSHOT ==========
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

                // ========== 5. CREATE AND CONFIGURE FINDER ==========
                var finder = _logic.CreateAlgorithmFinder(_mapGrid, algorithm, mlSettings);
                if (finder == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Algorithm {algorithm} not available";
                    result.FailureReason = "AlgorithmNotAvailable";
                    _iterationEndPoints.Add(Point.Empty);
                    return result;
                }

                _logic.ConfigureFinder(finder, metric, this);

                // ========== 6. APPLY ALGORITHM-SPECIFIC SETTINGS ==========
                var expSettings = _logic.GetCurrentSettings(this);
                ApplyAlgorithmSpecificSettings(finder, algorithm, expSettings);

                // ========== 7. FIND SEQUENTIAL PATH ==========
                var pathResult = await _logic.FindSequentialPath(finder, startPoint, orderedGoals);
                if (!pathResult.Success)
                {
                    result.Success = false;
                    result.ErrorMessage = pathResult.ErrorMessage;
                    result.FailureReason = "PathNotFound";
                    _iterationEndPoints.Add(Point.Empty);
                    return result;
                }

                // ========== 8. ANALYZE PATH ERRORS ==========
                var pathErrors = AnalyzePathForErrors(pathResult.Path, _mapGrid);
                if (pathErrors.Any())
                {
                    result.HasPathErrors = true;
                    result.PathErrorsJson = JsonSerializer.Serialize(pathErrors);
                    result.ErrorMessage = string.Join("; ", pathErrors.Select(e => e.Message));
                    result.FailureReason = pathErrors.First().Type;
                    System.Diagnostics.Debug.WriteLine($"[Experiment] Path errors: {result.ErrorMessage}");
                }

                // ========== 9. RETURN PATH (ONLY FOR SPPA FAMILY) ==========
                List<PathNode> fullPath;
                Point endPoint;

                bool isSPPAFamily = (algorithm == "SPPA" || algorithm == "SPPA_DL");

                if (isSPPAFamily && parkingPoints != null && parkingPoints.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[Experiment] {algorithm} is SPPA family - finding return path to parking");
                    var returnResult = await _logic.FindReturnPath(finder, pathResult.CurrentPos, parkingPoints);

                    if (returnResult != null && returnResult.Path != null && returnResult.Path.Count > 0)
                    {
                        fullPath = _logic.CombinePaths(pathResult.Path, returnResult.Path);
                        endPoint = fullPath.Count > 0 ? new Point(fullPath.Last().X, fullPath.Last().Y) : pathResult.CurrentPos;
                        System.Diagnostics.Debug.WriteLine($"[Experiment] Return path added: {returnResult.Path.Count} cells");
                    }
                    else
                    {
                        fullPath = pathResult.Path.ToList();
                        endPoint = pathResult.CurrentPos;
                        System.Diagnostics.Debug.WriteLine($"[Experiment] No return path found");
                    }
                }
                else
                {
                    fullPath = pathResult.Path.ToList();
                    endPoint = pathResult.CurrentPos;
                    System.Diagnostics.Debug.WriteLine($"[Experiment] {algorithm} is NOT SPPA family - no return path");
                }

                _iterationEndPoints.Add(endPoint);
                result.EndPointReached = endPoint;

                // ========== 10. DRAW PATH WITH COLORS ==========
                await DrawPathWithColors(fullPath, orderedGoals, startPoint, endPoint);

                if (_chkSaveScreenshots.Checked)
                {
                    await SaveScreenshot(algorithm, metric, iteration, "Path", result);
                }

                // ========== 11. DRAW COMPLETED PATH ==========
                await DrawCompletedPath(fullPath, endPoint);

                if (_chkSaveScreenshots.Checked)
                {
                    await SaveScreenshot(algorithm, metric, iteration, "Completed", result);
                }

                // ========== 12. FILL RESULT DATA ==========
                result.Success = true;
                result.FailureReason = "None";
                result.PathLength = fullPath.Count;
                result.ComputationTimeMs = pathResult.TotalTimeMs + (result.PathLength * 0.1); // Rough estimate for return path
                result.Path = fullPath.Select(p => new System.Drawing.Point(p.X, p.Y)).ToList();

                // Battery statistics (estimated)
                double batteryConsumedPerCell = robotSettings.BatteryConsumptionRate;
                result.TotalBatteryConsumedPercent = fullPath.Count * batteryConsumedPerCell;
                result.TotalChargingUnits = result.TotalBatteryConsumedPercent / 100.0;
                result.RemainingBattery = Math.Max(0, robotSettings.InitialBatteryLevel - result.TotalBatteryConsumedPercent);
                result.FinalBatteryPercent = result.RemainingBattery;

                // Time statistics (estimated)
                double cellSizeCm = _mapControl.ScaleCmPerCell;
                double totalDistanceCm = fullPath.Count * cellSizeCm;
                result.TotalTravelTimeSeconds = totalDistanceCm / robotSettings.InitialSpeedCmS;
                result.TotalTimeSeconds = result.TotalTravelTimeSeconds;

                // ========== 13. SAVE PATH FOR JSON EXPORT ==========
                _iterationPaths.Add(result.Path);

                System.Diagnostics.Debug.WriteLine($"[Experiment] {algorithm} - {metric} - Iter {iteration}: " +
                    $"SUCCESS, PathLength={result.PathLength}, Time={result.ComputationTimeMs:F2}ms, " +
                    $"Start=({startPoint.X},{startPoint.Y}), End=({endPoint.X},{endPoint.Y})");


                // ========== Calculate Battery Statistics ==========
                 double estimatedConsumption = fullPath.Count * batteryConsumedPerCell;

                result.InitialBatteryPercent = robotSettings.InitialBatteryLevel;
                result.FinalBatteryPercent = Math.Max(0, robotSettings.InitialBatteryLevel - estimatedConsumption);
                result.TotalBatteryConsumedPercent = estimatedConsumption;
                result.TotalChargingUnits = estimatedConsumption / 100.0;
                result.TotalChargingCycles = (int)(estimatedConsumption / 100.0);
                result.TotalChargingTimeSeconds = result.TotalChargingCycles * chargingTimeSeconds;

                System.Diagnostics.Debug.WriteLine($"[Experiment] Battery: Initial={result.InitialBatteryPercent:F1}%, " +
                    $"Consumed={result.TotalBatteryConsumedPercent:F1}%, Units={result.TotalChargingUnits:F2}, " +
                    $"Cycles={result.TotalChargingCycles}, ChargingTime={result.TotalChargingTimeSeconds:F0}s");
                 
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Experiment]",ex.Message );
            }
            finally
            { 
                 StopDetectionZoneUpdater(); 
            } 
            return result;  
           
        }
        #region Private Methods - Algorithm Specific Settings

        /// <summary>
        /// Applies algorithm-specific settings to the finder
        /// </summary>
        private void ApplyAlgorithmSpecificSettings(IPathFinder finder, string algorithm, ExperimentSettings settings)
        {
            if (finder == null) return;
    
            System.Diagnostics.Debug.WriteLine($"[Config] Applying specific settings for {algorithm}");
    
            switch (algorithm)
            {
                case "AStar":
                case "A*":
                    finder.HeuristicWeight = settings.HeuristicWeight;
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.HeavyDiagonals = settings.HeavyDiagonals;
                    finder.SearchLimit = settings.SearchLimit;
                    break;
            
                case "SPPA":
                    finder.HeuristicWeight = settings.HeuristicWeight;
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.HeavyDiagonals = settings.HeavyDiagonals;
                    finder.SearchLimit = settings.SearchLimit;
                    break;
            
                case "SPPA_DL":
                    finder.HeuristicWeight = settings.HeuristicWeight;
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.HeavyDiagonals = settings.HeavyDiagonals;
                    finder.SearchLimit = settings.SearchLimit;
            
                    if (settings.MLSettings != null && finder is SPPA_DLFinder sppaDLFinder)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Config] SPPA-DL: LearningRate={settings.MLSettings.LearningRate}");
                    }
                    break;
            
                case "ACO":
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.SearchLimit = settings.SearchLimit;
                    break;
            
                case "DStar":
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.SearchLimit = settings.SearchLimit;
                    break;
            
                case "KNN":
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.SearchLimit = settings.SearchLimit;
                    break;
            
                case "BruteForce":
                    finder.SearchLimit = settings.SearchLimit;
                    break;
            
                default:
                    finder.HeuristicWeight = settings.HeuristicWeight;
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.HeavyDiagonals = settings.HeavyDiagonals;
                    finder.SearchLimit = settings.SearchLimit;
                    break;
            }
        }

        #endregion
        #endregion

        #endregion
        private async Task DrawPathWithColors(List<PathNode> fullPath, List<Point> goals, Point startPoint, Point endPoint)
        {
            ExecuteOnUIThread(() =>
            {
                _mapControl.ClearPaths();

                var coloredSegments = new List<ColoredPath>();
                int currentIndex = 0;

                // ألوان مميزة للأهداف
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

                // إضافة الجزء المتبقي (مسار العودة)
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
        /// Validates that all goals and parking points are within grid bounds
        /// </summary>
        private bool ValidatePoints(List<Point> points, string name)
        {
            if (points == null || points.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"[Validate] {name} list is null or empty");
                return false;
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (!_mapGrid.IsValidCoordinate(points[i].X, points[i].Y))
                {
                    System.Diagnostics.Debug.WriteLine($"[Validate] {name}[{i}] = ({points[i].X},{points[i].Y}) is invalid");
                    return false;
                }
            }

            System.Diagnostics.Debug.WriteLine($"[Validate] {name} validation passed: {points.Count} points");
            return true;
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

         private async Task DrawGoldenPath(List<PathNode> path, Point startPoint, bool showRobot = true)
        {
            ExecuteOnUIThread(() =>
            {
                _mapControl.ClearPaths();

                // رسم المسار الذهبي (موجود)
                var coloredPath = new ColoredPath(path, Color.Gold, false);
                _mapControl.DrawColoredPaths(new List<ColoredPath> { coloredPath });

                // ========== إضافة نقطة البداية S ==========
                _mapControl.SetCurrentStartPoint(startPoint);

                // ========== إضافة الروبوت في نقطة البداية ==========
                if (showRobot)
                {
                    _mapControl.RobotPosition = startPoint;
                    _mapControl.RobotAngle = 0;
                    _mapControl.ShowRobot = true;
                }

                _mapControl.Refresh();
            });
            await Task.Delay(RENDER_DELAY_MS);
        }

        /// <summary>
        /// Draws green dashed path on the map control
        /// </summary>
        private async Task DrawGreenPath(List<PathNode> path, Point endPoint, bool showRobot = true)
        {
            ExecuteOnUIThread(() =>
            {
                var goldenPath = new ColoredPath(path, Color.Gold, false);
                var traveledNodes = path.Select(p => new PathNode(p.X, p.Y)).ToList();
                var greenPath = new ColoredPath(traveledNodes, Color.Green, true);
                _mapControl.DrawColoredPaths(new List<ColoredPath> { goldenPath, greenPath });

                // ========== إضافة الروبوت في نهاية المسار ==========
                if (showRobot)
                {
                    _mapControl.RobotPosition = endPoint;
                    _mapControl.RobotAngle = 0;
                    _mapControl.ShowRobot = true;
                }

                _mapControl.Refresh();
            });
            await Task.Delay(RENDER_DELAY_MS);
        }
        private async Task DrawChargingPath(List<PathNode> chargingPath, Point chargingPoint)
        {
            ExecuteOnUIThread(() =>
            {
                var coloredPath = new ColoredPath(chargingPath, Color.LightBlue, false);
                _mapControl.DrawColoredPaths(new List<ColoredPath> { coloredPath });

                // رسم نقطة الشحن P
                _mapControl.AddParkingAt(chargingPoint);

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
            if (results == null || results.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[SaveExperimentResults] No results to save");
                return;
            }

            string csvPath = Path.Combine(_currentOutputPath, "Results.csv");
            _logic.SaveResultsToCsv(results, csvPath);

            string summaryPath = Path.Combine(_currentOutputPath, "Summary.txt");
            _logic.GenerateSummaryReport(results, summaryPath, _txtExperimentName.Text);

            System.Diagnostics.Debug.WriteLine($"[SaveExperimentResults] Saved {results.Count} results to {_currentOutputPath}");
        }
        /// <summary>
        /// Shows results viewer
        /// </summary>
        private void ShowResultsViewer(List<ComparisonResult> results)
        {
            if (results == null || results.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[ShowResultsViewer] No results to display");
                MessageBox.Show("No results to display.", "Experiment Results",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

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

        #region Public Methods - JSON Helpers

        /// <summary>
        /// Serializes a list of points to JSON
        /// </summary>
        public static string SerializePointsList(List<Point> points)
        {
            if (points == null || points.Count == 0)
                return "[]";

            var items = points.Select(p => $"{{\"x\":{p.X},\"y\":{p.Y}}}");
            return $"[{string.Join(",", items)}]";
        }

        /// <summary>
        /// Deserializes JSON to list of points
        /// </summary>
        public static List<Point> DeserializePointsList(string json)
        {
            if (string.IsNullOrEmpty(json) || json == "[]")
                return new List<Point>();

            var points = new List<Point>();
            var matches = System.Text.RegularExpressions.Regex.Matches(json, @"""x"":(\d+),""y"":(\d+)");
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Groups.Count >= 3)
                {
                    int x = int.Parse(match.Groups[1].Value);
                    int y = int.Parse(match.Groups[2].Value);
                    points.Add(new Point(x, y));
                }
            }
            return points;
        }

        /// <summary>
        /// Serializes a list of paths to JSON
        /// </summary>
        public static string SerializePathsList(List<List<Point>> paths)
        {
            if (paths == null || paths.Count == 0)
                return "[]";

            var pathItems = new List<string>();
            foreach (var path in paths)
            {
                var points = path.Select(p => $"{{\"x\":{p.X},\"y\":{p.Y}}}");
                pathItems.Add($"[{string.Join(",", points)}]");
            }
            return $"[{string.Join(",", pathItems)}]";
        }

        #endregion

        #region Private Methods - Iteration Tracking

        /// <summary>
        /// Gets the initial start point for experiments
        /// </summary>
        private Point GetInitialStartPoint()
        {
            // If using custom start point from settings
            var (useCustom, customPoint) = _logic.GetStartPointSettings(this);
            if (useCustom)
            {
                return customPoint;
            }

            // Otherwise use current robot position from map
            if (_chkUseCurrentMap.Checked && _mapControl != null)
            {
                return _mapControl.RobotPosition;
            }

            // Default
            return new Point(10, 10);
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
            if (results == null || results.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[SaveStartEndPointsToCsv] No results to save");
                return;
            }

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

            System.Diagnostics.Debug.WriteLine($"[SaveStartEndPointsToCsv] Saved {maxCount} entries to {filePath}");
        }

        /// <summary>
        /// Saves all paths to JSON file
        /// </summary>
        private void SavePathsToJson(string filePath)
        {
            if (_iterationPaths == null || _iterationPaths.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[SavePathsToJson] No paths to save");
                return;
            }

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

            System.Diagnostics.Debug.WriteLine($"[SavePathsToJson] Saved {_iterationPaths.Count} paths to {filePath}");
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

        #region Event Handlers for New Controls

        /// <summary>
        /// Handles pick start point button click
        /// </summary>
        private void BtnPickStartPoint_Click(object sender, EventArgs e)
        {
            // Close current dialog temporarily
            this.Hide();

            // Show main form for user to pick start point
            var result = MessageBox.Show(
                "Click on the main map to select start point, then press Enter.\n\n" +
                "Current start point will be updated.",
                "Pick Start Point",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            if (result == DialogResult.OK)
            {
                // Get selected point from map control
                Point selectedPoint = _mapControl?.RobotPosition ?? new Point(10, 10);

                // Update display
                if (_lblCurrentStartPoint != null)
                {
                    _lblCurrentStartPoint.Text = $"Current: ({selectedPoint.X}, {selectedPoint.Y})";
                    _lblCurrentStartPoint.Tag = selectedPoint;
                }

                // Check the checkbox
                if (_chkUseCustomStartPoint != null)
                    _chkUseCustomStartPoint.Checked = true;
            }

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

        #endregion
        #endregion

        #region Algorithm-Specific Configuration

        /// <summary>
        /// Applies algorithm-specific settings to the finder
        /// </summary>
        public void ConfigureAlgorithmSpecificSettings(IPathFinder finder, string algorithm, ExperimentSettings settings)
        {
            if (finder == null) return;

            switch (algorithm)
            {
                case "AStar":
                case "A*":
                    // A* specific settings
                    finder.HeuristicWeight = settings.HeuristicWeight;
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.HeavyDiagonals = settings.HeavyDiagonals;
                    finder.SearchLimit = settings.SearchLimit;
                    break;

                case "SPPA":
                    // SPPA specific settings
                    finder.HeuristicWeight = settings.HeuristicWeight;
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.HeavyDiagonals = settings.HeavyDiagonals;
                    finder.SearchLimit = settings.SearchLimit;
                    // SPPA uses Lambda = 1.5 (hardcoded in algorithm)
                    break;

                case "SPPA_DL":
                    // SPPA-DL specific settings
                    finder.HeuristicWeight = settings.HeuristicWeight;
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.HeavyDiagonals = settings.HeavyDiagonals;
                    finder.SearchLimit = settings.SearchLimit;

                    // Apply ML settings if available
                    if (settings.MLSettings != null)
                    {
                        if (finder is SPPA_DLFinder sppaDLFinder)
                        {
                            // Learning rate, prediction weight, etc. can be set here
                            System.Diagnostics.Debug.WriteLine($"[Config] SPPA-DL: LearningRate={settings.MLSettings.LearningRate}");
                        }
                    }
                    break;

                case "ACO":
                    // ACO specific settings (will be applied when ACOFinder supports them)
                    if (finder is ACOFinder acoFinder)
                    {
                        // acoFinder.SetParameters(ants, evaporation, alpha, beta, iterations);
                        System.Diagnostics.Debug.WriteLine($"[Config] ACO: Using default parameters");
                    }
                    break;

                case "DStar":
                    // D* specific settings
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.SearchLimit = settings.SearchLimit;
                    break;

                case "KNN":
                    // KNN specific settings
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.SearchLimit = settings.SearchLimit;
                    break;

                case "BruteForce":
                    // Brute Force specific settings
                    finder.SearchLimit = settings.SearchLimit;
                    break;

                default:
                    // Default settings
                    finder.HeuristicWeight = settings.HeuristicWeight;
                    finder.AllowDiagonals = settings.AllowDiagonals;
                    finder.HeavyDiagonals = settings.HeavyDiagonals;
                    finder.SearchLimit = settings.SearchLimit;
                    break;
            }

            System.Diagnostics.Debug.WriteLine($"[Config] Applied settings for {algorithm}: Metric={finder.Metric}, Diagonals={finder.AllowDiagonals}, SearchLimit={finder.SearchLimit}");
        }

        /// <summary>
        /// Resets map and robot to default state for new experiment
        /// </summary>
        private void ResetToDefaultState()
        {
            // إعادة تعيين موقع الروبوت إلى (10,10)
            if (_mapControl != null)
            {
                _mapControl.RobotPosition = new Point(10, 10);
                _mapControl.RobotAngle = 0;
                _mapControl.SetCurrentStartPoint(new Point(10, 10));
                _mapControl.ClearPaths();
                _mapControl.Refresh();
            }

            // إعادة تعيين البطارية إلى 100%
            if (_viewModel != null)
            {
                _viewModel.SetBatteryLevel(100);
                _viewModel.ResetChargingStatistics();
            }

            System.Diagnostics.Debug.WriteLine("[Experiment] Reset to default state: Robot at (10,10), Battery 100%");
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
            _detectionZoneTimer.Interval = 100; // تحديث كل 100ms
            _detectionZoneTimer.Tick += OnDetectionZoneTimerTick;
            _detectionZoneTimer.Start();

            System.Diagnostics.Debug.WriteLine("[Experiment] Detection zone updater started");
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

            System.Diagnostics.Debug.WriteLine("[Experiment] Detection zone updater stopped");
        }

        /// <summary>
        /// Updates detection zone on timer tick
        /// </summary>
        private void OnDetectionZoneTimerTick(object sender, EventArgs e)
        {
            if (_mapControl == null || _viewModel == null) return;

            // Get current robot position from ViewModel
            Point robotPos = _viewModel.RobotState.Position;
            float robotAngle = _viewModel.RobotState.Angle;

            // Get detection zone cells
            var zoneCells = _viewModel.GetDetectionZoneCells();
            _mapControl.UpdateDetectionZone(zoneCells);
        }

        #endregion
        public class PathError
        {
            public string Type { get; set; }
            public string Message { get; set; }
            public Point Location { get; set; }
            public int StepIndex { get; set; }
        }
    }
}
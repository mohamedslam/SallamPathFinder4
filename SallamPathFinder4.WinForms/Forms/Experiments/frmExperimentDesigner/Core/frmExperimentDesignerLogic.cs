#region File Header
/// <summary>
/// File: ExperimentDesignerLogic.cs
/// Description: Business logic for experiment designer form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Algorithms;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.Services.Pathfinding;
using SallamPathFinder4.WinForms.Forms.Shared;
using SallamPathFinder4.WinForms.ViewModels;
using SallamPathFinder4.Core.Models.Experiments;
using System.Text.Json;
using static SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentDesigner.frmExperimentDesigner;
using SallamPathFinder4.Core.Algorithms.Implementations;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentDesigner.Core
{
    public sealed class ExperimentDesignerLogic
    {
        #region Constructor
        public ExperimentDesignerLogic()
        {
        }
        #endregion

        #region Algorithm Selection
        /// <summary>
        /// Gets selected algorithms from the DataGridView
        /// </summary>
        public List<string> GetSelectedAlgorithms(Form form)
        {
            var algorithms = new List<string>();
            var dgv = form.Controls.Find("_dgvAlgorithems", true).FirstOrDefault() as DataGridView;

            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["colEnabled"]?.Value != null && (bool)row.Cells["colEnabled"].Value)
                    {
                        string algorithm = row.Cells["colAlgorithm"]?.Value?.ToString();
                        if (!string.IsNullOrEmpty(algorithm))
                        {
                            algorithms.Add(algorithm);
                        }
                    }
                }
            }

            return algorithms;
        }

        /// <summary>
        /// Gets selected metrics from the DataGridView
        /// </summary>
        public List<string> GetSelectedMetrics(Form form)
        {
            var metrics = new List<string>();
            var dgv = form.Controls.Find("_dgvAlgorithems", true).FirstOrDefault() as DataGridView;

            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["colEnabled"]?.Value != null && (bool)row.Cells["colEnabled"].Value)
                    {
                        string metric = row.Cells["colMetric"]?.Value?.ToString();
                        if (!string.IsNullOrEmpty(metric) && !metrics.Contains(metric))
                        {
                            metrics.Add(metric);
                        }
                    }
                }
            }

            if (metrics.Count == 0) metrics.Add("Manhattan");
            return metrics;
        }

        /// <summary>
        /// Gets selected algorithms with their metrics and parameters from DataGridView
        /// </summary>
        public List<(string algorithm, string metric, Dictionary<string, object> parameters)> GetSelectedAlgorithmsWithParams(Form form)
        {
            var result = new List<(string, string, Dictionary<string, object>)>();
            var dgv = form.Controls.Find("_dgvAlgorithems", true).FirstOrDefault() as DataGridView;

            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["colEnabled"]?.Value != null && (bool)row.Cells["colEnabled"].Value)
                    {
                        string algorithm = row.Cells["colAlgorithm"]?.Value?.ToString();
                        string metric = row.Cells["colMetric"]?.Value?.ToString();
                        var parameters = row.Tag as Dictionary<string, object>;

                        if (!string.IsNullOrEmpty(algorithm) && !string.IsNullOrEmpty(metric))
                        {
                            result.Add((algorithm, metric, parameters ?? new Dictionary<string, object>()));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the metric for a specific algorithm from DataGridView
        /// </summary>
        public string GetMetricForAlgorithm(Form form, string algorithmName)
        {
            var dgv = form.Controls.Find("_dgvAlgorithems", true).FirstOrDefault() as DataGridView;

            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    string algorithm = row.Cells["colAlgorithm"]?.Value?.ToString();
                    if (algorithm == algorithmName)
                    {
                        return row.Cells["colMetric"]?.Value?.ToString() ?? "Manhattan";
                    }
                }
            }

            return "Manhattan";
        }

        /// <summary>
        /// Gets parameters for a specific algorithm from DataGridView
        /// </summary>
        public Dictionary<string, object> GetParametersForAlgorithm(Form form, string algorithmName)
        {
            var dgv = form.Controls.Find("_dgvAlgorithems", true).FirstOrDefault() as DataGridView;

            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    string algorithm = row.Cells["colAlgorithm"]?.Value?.ToString();
                    if (algorithm == algorithmName)
                    {
                        return row.Tag as Dictionary<string, object> ?? new Dictionary<string, object>();
                    }
                }
            }

            return new Dictionary<string, object>();
        }
        #endregion

        #region Settings Management
        /// <summary>
        /// Gets current settings from the form
        /// </summary>
        public ExperimentSettings GetCurrentSettings(Form form)
        {
            var nudGoalCount = form.Controls.Find("_nudGoalCount", true).FirstOrDefault() as NumericUpDown;
            var nudParkingCount = form.Controls.Find("_nudParkingCount", true).FirstOrDefault() as NumericUpDown;
            var nudStaticObstacles = form.Controls.Find("_nudStaticObstacles", true).FirstOrDefault() as NumericUpDown;
            var nudDynamicObstacles = form.Controls.Find("_nudDynamicObstacles", true).FirstOrDefault() as NumericUpDown;
            var nudIterations = form.Controls.Find("_nudIterations", true).FirstOrDefault() as NumericUpDown;
            var chkSaveScreenshots = form.Controls.Find("_chkSaveScreenshots", true).FirstOrDefault() as CheckBox;
            var chkSaveReplay = form.Controls.Find("_chkSaveReplay", true).FirstOrDefault() as CheckBox;
            var chkShowPath = form.Controls.Find("_chkShowPathOnScreenshots", true).FirstOrDefault() as CheckBox;
            var txtExperimentName = form.Controls.Find("_txtExperimentName", true).FirstOrDefault() as TextBox;
            var txtSavePath = form.Controls.Find("_txtSavePath", true).FirstOrDefault() as TextBox;
            var (enableDynamicCharging, chargingTime, safetyMargin) = GetDynamicChargingSettings(form);
            var (useCustomStartPoint, customStartPoint) = GetStartPointSettings(form);

            var selectedAlgorithms = GetSelectedAlgorithms(form);
            var selectedMetrics = GetSelectedMetrics(form);
            var distanceMetric = selectedMetrics.FirstOrDefault() ?? "Manhattan";

            int heuristicWeight = 2;
            int searchLimit = 20000;

            var dgv = form.Controls.Find("_dgvAlgorithems", true).FirstOrDefault() as DataGridView;
            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["colEnabled"]?.Value != null && (bool)row.Cells["colEnabled"].Value)
                    {
                        var parameters = row.Tag as Dictionary<string, object>;
                        if (parameters != null)
                        {
                            if (parameters.ContainsKey("HeuristicWeight"))
                                heuristicWeight = Convert.ToInt32(parameters["HeuristicWeight"]);
                            if (parameters.ContainsKey("SearchLimit"))
                                searchLimit = Convert.ToInt32(parameters["SearchLimit"]);
                        }
                        break;
                    }
                }
            }

            return new ExperimentSettings
            {
                ExperimentName = txtExperimentName?.Text ?? "Experiment",
                SelectedAlgorithms = selectedAlgorithms,
                SelectedMetrics = selectedMetrics,
                GoalCount = (int)(nudGoalCount?.Value ?? 5),
                ParkingCount = (int)(nudParkingCount?.Value ?? 2),
                StaticObstacles = (int)(nudStaticObstacles?.Value ?? 20),
                DynamicObstacles = (int)(nudDynamicObstacles?.Value ?? 5),
                HeuristicWeight = heuristicWeight,
                SearchLimit = searchLimit,
                AllowDiagonals = true,
                HeavyDiagonals = false,
                Iterations = (int)(nudIterations?.Value ?? 5),
                SaveScreenshots = chkSaveScreenshots?.Checked == true,
                SaveReplay = chkSaveReplay?.Checked == true,
                ShowPathOnScreenshots = chkShowPath?.Checked == true,
                SavePath = txtSavePath?.Text ?? string.Empty,
                EnableDynamicCharging = enableDynamicCharging,
                ChargingTimeSeconds = chargingTime,
                SafetyMarginPercent = safetyMargin,
                UseCustomStartPoint = useCustomStartPoint,
                CustomStartPoint = customStartPoint,
                OrderGoalsByDistance = GetOrderGoalsByDistance(form),
                DistanceMetric = distanceMetric
            };
        }

        /// <summary>
        /// Applies settings to the form
        /// </summary>
        public void ApplySettingsToForm(Form form, ExperimentSettings settings)
        {
            var txtExperimentName = form.Controls.Find("_txtExperimentName", true).FirstOrDefault() as TextBox;
            var nudGoalCount = form.Controls.Find("_nudGoalCount", true).FirstOrDefault() as NumericUpDown;
            var nudParkingCount = form.Controls.Find("_nudParkingCount", true).FirstOrDefault() as NumericUpDown;
            var nudStaticObstacles = form.Controls.Find("_nudStaticObstacles", true).FirstOrDefault() as NumericUpDown;
            var nudDynamicObstacles = form.Controls.Find("_nudDynamicObstacles", true).FirstOrDefault() as NumericUpDown;
            var nudIterations = form.Controls.Find("_nudIterations", true).FirstOrDefault() as NumericUpDown;
            var chkSaveScreenshots = form.Controls.Find("_chkSaveScreenshots", true).FirstOrDefault() as CheckBox;
            var chkSaveReplay = form.Controls.Find("_chkSaveReplay", true).FirstOrDefault() as CheckBox;
            var chkShowPath = form.Controls.Find("_chkShowPathOnScreenshots", true).FirstOrDefault() as CheckBox;
            var txtSavePath = form.Controls.Find("_txtSavePath", true).FirstOrDefault() as TextBox;

            if (txtExperimentName != null) txtExperimentName.Text = settings.ExperimentName;
            if (nudGoalCount != null) nudGoalCount.Value = Math.Clamp(settings.GoalCount, 1, 50);
            if (nudParkingCount != null) nudParkingCount.Value = Math.Clamp(settings.ParkingCount, 1, 10);
            if (nudStaticObstacles != null) nudStaticObstacles.Value = Math.Clamp(settings.StaticObstacles, 0, 500);
            if (nudDynamicObstacles != null) nudDynamicObstacles.Value = Math.Clamp(settings.DynamicObstacles, 0, 50);
            if (nudIterations != null) nudIterations.Value = Math.Clamp(settings.Iterations, 1, 100);
            if (chkSaveScreenshots != null) chkSaveScreenshots.Checked = settings.SaveScreenshots;
            if (chkSaveReplay != null) chkSaveReplay.Checked = settings.SaveReplay;
            if (chkShowPath != null) chkShowPath.Checked = settings.ShowPathOnScreenshots;
            if (txtSavePath != null && !string.IsNullOrEmpty(settings.SavePath)) txtSavePath.Text = settings.SavePath;

            ApplyAlgorithmsToGrid(form, settings);
            ApplyDynamicChargingSettings(form, settings.EnableDynamicCharging, settings.ChargingTimeSeconds, settings.SafetyMarginPercent);
            ApplyStartPointSettings(form, settings.UseCustomStartPoint, settings.CustomStartPoint);
            ApplyOrderGoalsByDistance(form, settings.OrderGoalsByDistance);
        }

        /// <summary>
        /// Applies algorithms to the DataGridView
        /// </summary>
        private void ApplyAlgorithmsToGrid(Form form, ExperimentSettings settings)
        {
            var dgv = form.Controls.Find("_dgvAlgorithems", true).FirstOrDefault() as DataGridView;
            if (dgv == null) return;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                row.Cells["colEnabled"].Value = false;
            }

            foreach (string algorithm in settings.SelectedAlgorithms)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    string alg = row.Cells["colAlgorithm"].Value?.ToString();
                    if (alg == algorithm)
                    {
                        row.Cells["colEnabled"].Value = true;
                        if (settings.SelectedMetrics != null && settings.SelectedMetrics.Count > 0)
                        {
                            row.Cells["colMetric"].Value = settings.SelectedMetrics[0];
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Saves settings to file
        /// </summary>
        public void SaveSettingsToFile(ExperimentSettings settings, string filePath)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads settings from file
        /// </summary>
        public ExperimentSettings LoadSettingsFromFile(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<ExperimentSettings>(json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets OrderGoalsByDistance setting from the DataGridView
        /// </summary>
        public bool GetOrderGoalsByDistance(Form form)
        {
            var dgv = form.Controls.Find("_dgvAlgorithems", true).FirstOrDefault() as DataGridView;
            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["colEnabled"]?.Value != null && (bool)row.Cells["colEnabled"].Value)
                    {
                        var parameters = row.Tag as Dictionary<string, object>;
                        if (parameters != null && parameters.ContainsKey("OrderGoalsByDistance"))
                        {
                            return Convert.ToBoolean(parameters["OrderGoalsByDistance"]);
                        }
                        break;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Robot and ML Settings
        /// <summary>
        /// Gets robot settings from the form
        /// </summary>
        public RobotSettings GetRobotSettings(Form form)
        {
            var txtRobotName = form.Controls.Find("_txtRobotName", true).FirstOrDefault() as TextBox;
            var nudRobotSpeed = form.Controls.Find("_nudRobotSpeed", true).FirstOrDefault() as NumericUpDown;
            var nudRobotBattery = form.Controls.Find("_nudRobotBattery", true).FirstOrDefault() as NumericUpDown;
            var nudConsumptionRate = form.Controls.Find("_nudConsumptionRate", true).FirstOrDefault() as NumericUpDown;
            var nudViewAngle = form.Controls.Find("_nudViewAngle", true).FirstOrDefault() as NumericUpDown;
            var nudDetectionRange = form.Controls.Find("_nudDetectionRange", true).FirstOrDefault() as NumericUpDown;

            double speed = nudRobotSpeed != null ? (double)nudRobotSpeed.Value : 10.0;
            double battery = nudRobotBattery != null ? (double)nudRobotBattery.Value : 100.0;
            double consumption = nudConsumptionRate != null ? (double)nudConsumptionRate.Value : 1.0;
            double viewAngle = nudViewAngle != null ? (double)nudViewAngle.Value : 180.0;
            int detectionRange = nudDetectionRange != null ? (int)nudDetectionRange.Value : 2;

            return new RobotSettings
            {
                RobotName = txtRobotName?.Text ?? "SallamBot",
                InitialSpeedCmS = speed,
                InitialBatteryLevel = battery,
                BatteryConsumptionRate = consumption,
                ViewAngleDegrees = viewAngle,
                DetectionRangeCells = detectionRange
            };
        }

        /// <summary>
        /// Gets ML settings from the form
        /// </summary>
        public MLSettings GetMLSettings(Form form)
        {
            var chkEnableDynamicLearning = form.Controls.Find("_chkEnableDynamicLearning", true).FirstOrDefault() as CheckBox;
            var nudLearningRate = form.Controls.Find("_nudLearningRate", true).FirstOrDefault() as NumericUpDown;
            var chkUseNeuralNetwork = form.Controls.Find("_chkUseNeuralNetwork", true).FirstOrDefault() as CheckBox;
            var chkCollectTrainingData = form.Controls.Find("_chkCollectTrainingData", true).FirstOrDefault() as CheckBox;
            var chkTrainBeforeExperiment = form.Controls.Find("_chkTrainBeforeExperiment", true).FirstOrDefault() as CheckBox;

            double learningRate = nudLearningRate != null ? (double)nudLearningRate.Value : 2.0;

            return new MLSettings
            {
                EnableDynamicLearning = chkEnableDynamicLearning?.Checked == true,
                LearningRate = learningRate,
                UseNeuralNetwork = chkUseNeuralNetwork?.Checked == true,
                CollectTrainingData = chkCollectTrainingData?.Checked == true,
                TrainBeforeExperiment = chkTrainBeforeExperiment?.Checked == true
            };
        }
        #endregion

        #region Map Data
        /// <summary>
        /// Gets real goals from the view model
        /// </summary>
        public List<Point> GetRealGoals(MainViewModel viewModel, bool useCurrentMap)
        {
            if (!useCurrentMap || viewModel?.Goals == null || viewModel.Goals.Count == 0)
                return new List<Point>();

            return viewModel.Goals.Select(g => g.Location).ToList();
        }

        /// <summary>
        /// Gets parking points from the view model
        /// </summary>
        public List<Point> GetParkingPoints(MainViewModel viewModel, bool useCurrentMap)
        {
            if (!useCurrentMap || viewModel?.ParkingPoints == null || viewModel.ParkingPoints.Count == 0)
                return new List<Point>();

            return viewModel.ParkingPoints.Select(p => p.Location).ToList();
        }
        #endregion

        #region Pathfinding
        /// <summary>
        /// Creates an algorithm finder with parameters
        /// </summary>
        public IPathFinder CreateAlgorithmFinder(MapGrid grid, string algorithm, MLSettings mlSettings, Dictionary<string, object> parameters = null)
        {
            var factory = new AlgorithmFactory(grid);
            var algoType = ExperimentSharedLogic.GetAlgorithmType(algorithm);

            IPathFinder finder = null;

            if (algoType == AlgorithmType.SPPA_DL)
            {
                finder = factory.Create(algoType, mlSettings.UseNeuralNetwork,
                    mlSettings.CollectTrainingData, mlSettings.LearningRate);

                if (finder is SPPA_DLFinder sppaDLFinder && parameters != null)
                {
                    ApplyParametersToSPPA_DL(sppaDLFinder, parameters);
                }
            }
            else if (algoType == AlgorithmType.SPPA)
            {
                finder = factory.Create(algoType);

                if (finder is SPPAFinder sppaFinder && parameters != null)
                {
                    ApplyParametersToSPPA(sppaFinder, parameters);
                }
            }
            else if (algoType == AlgorithmType.AStar)
            {
                finder = factory.Create(algoType);

                if (finder is AStarFinder aStarFinder && parameters != null)
                {
                    ApplyParametersToAStar(aStarFinder, parameters);
                }
            }
            else if (algoType == AlgorithmType.ACO)
            {
                finder = factory.Create(algoType);

                if (finder is ACOFinder acoFinder && parameters != null)
                {
                    ApplyParametersToACO(acoFinder, parameters);
                }
            }
            else
            {
                finder = factory.Create(algoType);
            }

            return finder;
        }

        #region Apply Parameters Methods
        private void ApplyParametersToAStar(AStarFinder finder, Dictionary<string, object> parameters)
        {
            if (parameters.ContainsKey("HeuristicWeight"))
                finder.HeuristicWeight = Convert.ToInt32(parameters["HeuristicWeight"]);
            if (parameters.ContainsKey("SearchLimit"))
                finder.SearchLimit = Convert.ToInt32(parameters["SearchLimit"]);
            if (parameters.ContainsKey("AllowDiagonals"))
                finder.AllowDiagonals = Convert.ToBoolean(parameters["AllowDiagonals"]);
            if (parameters.ContainsKey("HeavyDiagonals"))
                finder.HeavyDiagonals = Convert.ToBoolean(parameters["HeavyDiagonals"]);
        }

        private void ApplyParametersToSPPA(SPPAFinder finder, Dictionary<string, object> parameters)
        {
            if (parameters.ContainsKey("HeuristicWeight"))
                finder.HeuristicWeight = Convert.ToInt32(parameters["HeuristicWeight"]);
            if (parameters.ContainsKey("SearchLimit"))
                finder.SearchLimit = Convert.ToInt32(parameters["SearchLimit"]);
            if (parameters.ContainsKey("AllowDiagonals"))
                finder.AllowDiagonals = Convert.ToBoolean(parameters["AllowDiagonals"]);
            if (parameters.ContainsKey("HeavyDiagonals"))
                finder.HeavyDiagonals = Convert.ToBoolean(parameters["HeavyDiagonals"]);
            if (parameters.ContainsKey("Lambda"))
                finder.Lambda = Convert.ToDouble(parameters["Lambda"]);
            if (parameters.ContainsKey("AlphaS"))
                finder.AlphaS = Convert.ToDouble(parameters["AlphaS"]);
            if (parameters.ContainsKey("AlphaSS"))
                finder.AlphaSS = Convert.ToDouble(parameters["AlphaSS"]);
            if (parameters.ContainsKey("AlphaD"))
                finder.AlphaD = Convert.ToDouble(parameters["AlphaD"]);
        }

        private void ApplyParametersToSPPA_DL(SPPA_DLFinder finder, Dictionary<string, object> parameters)
        {
            if (parameters.ContainsKey("HeuristicWeight"))
                finder.HeuristicWeight = Convert.ToInt32(parameters["HeuristicWeight"]);
            if (parameters.ContainsKey("SearchLimit"))
                finder.SearchLimit = Convert.ToInt32(parameters["SearchLimit"]);
            if (parameters.ContainsKey("AllowDiagonals"))
                finder.AllowDiagonals = Convert.ToBoolean(parameters["AllowDiagonals"]);
            if (parameters.ContainsKey("HeavyDiagonals"))
                finder.HeavyDiagonals = Convert.ToBoolean(parameters["HeavyDiagonals"]);
            if (parameters.ContainsKey("Lambda"))
                finder.Lambda = Convert.ToDouble(parameters["Lambda"]);
            if (parameters.ContainsKey("LearningRate"))
                finder.LearningRate = Convert.ToDouble(parameters["LearningRate"]);
            if (parameters.ContainsKey("PredictionWeight"))
                finder.PredictionWeight = Convert.ToDouble(parameters["PredictionWeight"]);
            if (parameters.ContainsKey("AlphaS"))
                finder.AlphaS = Convert.ToDouble(parameters["AlphaS"]);
            if (parameters.ContainsKey("AlphaSS"))
                finder.AlphaSS = Convert.ToDouble(parameters["AlphaSS"]);
            if (parameters.ContainsKey("AlphaD"))
                finder.AlphaD = Convert.ToDouble(parameters["AlphaD"]);
        }

        private void ApplyParametersToACO(ACOFinder finder, Dictionary<string, object> parameters)
        {
            int ants = parameters.ContainsKey("Ants") ? Convert.ToInt32(parameters["Ants"]) : 20;
            int iterations = parameters.ContainsKey("Iterations") ? Convert.ToInt32(parameters["Iterations"]) : 100;
            double alpha = parameters.ContainsKey("Alpha") ? Convert.ToDouble(parameters["Alpha"]) : 1.0;
            double beta = parameters.ContainsKey("Beta") ? Convert.ToDouble(parameters["Beta"]) : 2.0;
            double evaporation = parameters.ContainsKey("EvaporationRate") ? Convert.ToDouble(parameters["EvaporationRate"]) : 0.1;

            finder.SetParameters(ants, evaporation, alpha, beta, iterations);

            if (parameters.ContainsKey("AllowDiagonals"))
                finder.AllowDiagonals = Convert.ToBoolean(parameters["AllowDiagonals"]);
        }
        #endregion

        /// <summary>
        /// Configures the finder with basic settings (only metric)
        /// </summary>
        public void ConfigureFinder(IPathFinder finder, string metric, Form form)
        {
            // Only set the metric - other parameters come from the grid
            finder.Metric = ExperimentSharedLogic.GetDistanceMetric(metric);
        }

        /// <summary>
        /// Finds sequential path through all goals
        /// </summary>
        public async Task<SequentialPathResult> FindSequentialPath(IPathFinder finder, Point start, List<Point> goals)
        {
            var fullPath = new List<PathNode>();
            Point currentPos = start;
            double totalTimeMs = 0;

            for (int i = 0; i < goals.Count; i++)
            {
                if (goals[i] == null)
                {
                    return SequentialPathResult.CreateFailure($"Goal {i + 1} is null");
                }

                var pathResult = await Task.Run(() => finder.FindPath(currentPos, goals[i]));

                if (!pathResult.Success)
                {
                    return SequentialPathResult.CreateFailure($"No path to goal {i + 1}: {pathResult.ErrorMessage}");
                }

                if (pathResult.Path == null || pathResult.Path.Count == 0)
                {
                    return SequentialPathResult.CreateFailure($"Empty path to goal {i + 1}");
                }

                if (fullPath.Count == 0)
                    fullPath.AddRange(pathResult.Path);
                else
                    fullPath.AddRange(pathResult.Path.Skip(1));

                totalTimeMs += pathResult.ComputationTimeSeconds * 1000;
                currentPos = goals[i];
            }

            return SequentialPathResult.CreateSuccess(fullPath, currentPos, totalTimeMs);
        }

        /// <summary>
        /// Finds return path to nearest parking point
        /// </summary>
        public async Task<ReturnPathResult> FindReturnPath(IPathFinder finder, Point currentPos, List<Point> parkingPoints)
        {
            if (parkingPoints == null || parkingPoints.Count == 0)
            {
                return null;
            }

            Point nearestParking = Point.Empty;
            int minDistance = int.MaxValue;

            foreach (var parking in parkingPoints)
            {
                int distance = Math.Abs(parking.X - currentPos.X) + Math.Abs(parking.Y - currentPos.Y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestParking = parking;
                }
            }

            if (nearestParking == Point.Empty)
            {
                return null;
            }

            var pathResult = await Task.Run(() => finder.FindPath(currentPos, nearestParking));

            if (!pathResult.Success || pathResult.Path == null)
            {
                return null;
            }

            return ReturnPathResult.CreateSuccess(pathResult.Path.ToList(), pathResult.ComputationTimeSeconds * 1000);
        }

        /// <summary>
        /// Combines forward and return paths
        /// </summary>
        public List<PathNode> CombinePaths(List<PathNode> forwardPath, List<PathNode> returnPath)
        {
            var combined = new List<PathNode>(forwardPath);

            if (returnPath != null && returnPath.Count > 0)
            {
                combined.AddRange(returnPath.Skip(1));
            }

            return combined;
        }

        /// <summary>
        /// Creates an empty result object
        /// </summary>
        public ComparisonResult CreateEmptyResult(string algorithm, string metric, int iteration,
            RobotSettings robotSettings, Form form)
        {
            var nudGoalCount = form.Controls.Find("_nudGoalCount", true).FirstOrDefault() as NumericUpDown;
            var nudParkingCount = form.Controls.Find("_nudParkingCount", true).FirstOrDefault() as NumericUpDown;
            var nudStaticObstacles = form.Controls.Find("_nudStaticObstacles", true).FirstOrDefault() as NumericUpDown;
            var nudDynamicObstacles = form.Controls.Find("_nudDynamicObstacles", true).FirstOrDefault() as NumericUpDown;

            int goalCount = nudGoalCount != null ? (int)nudGoalCount.Value : 5;
            int parkingCount = nudParkingCount != null ? (int)nudParkingCount.Value : 2;
            int staticObstacles = nudStaticObstacles != null ? (int)nudStaticObstacles.Value : 20;
            int dynamicObstacles = nudDynamicObstacles != null ? (int)nudDynamicObstacles.Value : 5;

            int heuristicWeight = 2;
            int searchLimit = 20000;

            var dgv = form.Controls.Find("_dgvAlgorithems", true).FirstOrDefault() as DataGridView;
            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    string alg = row.Cells["colAlgorithm"].Value?.ToString();
                    if (alg == algorithm)
                    {
                        var parameters = row.Tag as Dictionary<string, object>;
                        if (parameters != null)
                        {
                            if (parameters.ContainsKey("HeuristicWeight"))
                                heuristicWeight = Convert.ToInt32(parameters["HeuristicWeight"]);
                            if (parameters.ContainsKey("SearchLimit"))
                                searchLimit = Convert.ToInt32(parameters["SearchLimit"]);
                        }
                        break;
                    }
                }
            }

            return new ComparisonResult
            {
                Algorithm = algorithm,
                Metric = metric,
                Iteration = iteration,
                RobotName = robotSettings.RobotName,
                RobotSpeedCmS = robotSettings.InitialSpeedCmS,
                RobotInitialBatteryPercent = robotSettings.InitialBatteryLevel,
                GoalCount = goalCount,
                ParkingCount = parkingCount,
                StaticObstacles = staticObstacles,
                DynamicObstacles = dynamicObstacles,
                RemainingBattery = robotSettings.InitialBatteryLevel,
                AverageActualSpeed = robotSettings.InitialSpeedCmS,
                CollisionCount = 0,
                InvalidMoveCount = 0,
                HeuristicWeight = heuristicWeight,
                SearchLimit = searchLimit
            };
        }
        #endregion

        #region Export
        /// <summary>
        /// Saves results to CSV file
        /// </summary>
        public void SaveResultsToCsv(List<ComparisonResult> results, string filePath)
        {
            using var writer = new StreamWriter(filePath);

            writer.WriteLine("Algorithm,Metric,Iteration,Success,PathLength,TimeMs," +
                "InitialBattery%,FinalBattery%,TotalConsumed%,ChargingUnits,ChargingCycles,ChargingTimeSec," +
                "Collisions,Errors,AvgSpeed,GoalCount,ParkingCount,StaticObstacles,DynamicObstacles," +
                "InitialScreenshot,PathScreenshot,CompletedScreenshot,ErrorMessage");

            foreach (var r in results)
            {
                writer.WriteLine($"{r.Algorithm},{r.Metric},{r.Iteration},{r.Success},{r.PathLength},{r.ComputationTimeMs:F2}," +
                    $"{r.InitialBatteryPercent:F1},{r.FinalBatteryPercent:F1},{r.TotalBatteryConsumedPercent:F1}," +
                    $"{r.TotalChargingUnits:F2},{r.TotalChargingCycles},{r.TotalChargingTimeSeconds:F0}," +
                    $"{r.CollisionCount},{r.InvalidMoveCount},{r.AverageActualSpeed:F1}," +
                    $"{r.GoalCount},{r.ParkingCount},{r.StaticObstacles},{r.DynamicObstacles}," +
                    $"\"{r.InitialScreenshotPath}\",\"{r.PathScreenshotPath}\",\"{r.CompletedScreenshotPath}\"," +
                    $"\"{r.ErrorMessage}\"");
            }
        }

        /// <summary>
        /// Generates a summary report
        /// </summary>
        public void GenerateSummaryReport(List<ComparisonResult> results, string filePath, string experimentName)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("=== EXPERIMENT SUMMARY ===");
            writer.WriteLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine($"Experiment: {experimentName}");
            writer.WriteLine($"Total Experiments: {results.Count}");
            writer.WriteLine($"Successful: {results.Count(r => r.Success)}");
            writer.WriteLine($"Failed: {results.Count(r => !r.Success)}");

            var byAlgorithm = results.GroupBy(r => r.Algorithm);
            writer.WriteLine("\n=== BY ALGORITHM ===");
            foreach (var group in byAlgorithm)
            {
                var successful = group.Where(r => r.Success).ToList();
                double successRate = group.Any() ? (double)successful.Count / group.Count() * 100 : 0;
                double avgTime = successful.Any() ? successful.Average(r => r.ComputationTimeMs) : 0;
                double avgLength = successful.Any() ? successful.Average(r => (double)r.PathLength) : 0;

                writer.WriteLine($"{group.Key}: Success={successRate:F1}%, AvgTime={avgTime:F2}ms, AvgLength={avgLength:F0}");
            }
        }
        #endregion

        #region Calculate and Fill Result Data
        /// <summary>
        /// Calculates all battery, time, and path statistics and fills the result object
        /// </summary>
        public void CalculateAndFillResultData(
            ComparisonResult result,
            List<PathNode> fullPath,
            Point startPoint,
            Point endPoint,
            List<Point> goals,
            RobotSettings robotSettings,
            double chargingTimeSeconds,
            double cellSizeCm,
            bool orderGoalsByDistance,
            double pathfindingTimeMs)
        {
            // Basic path metrics
            result.PathLength = fullPath.Count;
            result.Path = fullPath.Select(p => new Point(p.X, p.Y)).ToList();
            result.ComputationTimeMs = pathfindingTimeMs;
            result.StartPointUsed = startPoint;
            result.EndPointReached = endPoint;
            result.OrderedByDistance = orderGoalsByDistance;

            // Goal order
            if (orderGoalsByDistance)
            {
                var orderedGoals = goals
                    .OrderBy(g => Math.Abs(g.X - startPoint.X) + Math.Abs(g.Y - startPoint.Y))
                    .ToList();
                result.GoalOrder = string.Join(" → ", orderedGoals.Select(g => $"({g.X},{g.Y})"));
            }
            else
            {
                result.GoalOrder = string.Join(" → ", goals.Select(g => $"({g.X},{g.Y})"));
            }

            // Time statistics
            if (cellSizeCm <= 0) cellSizeCm = 10.0;
            double totalDistanceCm = fullPath.Count * cellSizeCm;
            double travelTimeSeconds = totalDistanceCm / Math.Max(0.1, robotSettings.InitialSpeedCmS);

            result.TotalTravelTimeSeconds = travelTimeSeconds;
            result.AverageActualSpeed = travelTimeSeconds > 0 ? totalDistanceCm / travelTimeSeconds : robotSettings.InitialSpeedCmS;

            // Battery statistics
            double batteryConsumedPerCell = robotSettings.BatteryConsumptionRate;
            if (batteryConsumedPerCell <= 0) batteryConsumedPerCell = 1.0;

            double estimatedConsumption = fullPath.Count * batteryConsumedPerCell;
            double totalChargingNeeded = Math.Max(0, estimatedConsumption - robotSettings.InitialBatteryLevel);
            int chargingCycles = (int)Math.Ceiling(totalChargingNeeded / 100.0);
            double totalChargingTimeSeconds = chargingCycles * chargingTimeSeconds;

            result.InitialBatteryPercent = robotSettings.InitialBatteryLevel;
            result.FinalBatteryPercent = Math.Max(0, robotSettings.InitialBatteryLevel - estimatedConsumption);
            result.TotalBatteryConsumedPercent = estimatedConsumption;
            result.TotalChargingUnits = estimatedConsumption / 100.0;
            result.TotalChargingCycles = chargingCycles;
            result.TotalChargingTimeSeconds = totalChargingTimeSeconds;
            result.RemainingBattery = result.FinalBatteryPercent;

            // Total time
            result.TotalOverheadTimeSeconds = 0;
            result.TotalTimeSeconds = travelTimeSeconds + totalChargingTimeSeconds;
        }
        #endregion

        #region Dynamic Charging Settings
        /// <summary>
        /// Gets dynamic charging settings from form
        /// </summary>
        public (bool enabled, int timeSeconds, double safetyMargin) GetDynamicChargingSettings(Form form)
        {
            var chkEnable = form.Controls.Find("_chkEnableDynamicCharging", true).FirstOrDefault() as CheckBox;
            var nudTime = form.Controls.Find("_nudChargingTime", true).FirstOrDefault() as NumericUpDown;
            var nudSafety = form.Controls.Find("_nudSafetyMargin", true).FirstOrDefault() as NumericUpDown;

            bool enabled = chkEnable?.Checked ?? false;
            int timeSeconds = nudTime != null ? (int)nudTime.Value : 15;
            double safetyMargin = nudSafety != null ? (double)nudSafety.Value : 10.0;

            return (enabled, timeSeconds, safetyMargin);
        }

        /// <summary>
        /// Applies dynamic charging settings to form
        /// </summary>
        public void ApplyDynamicChargingSettings(Form form, bool enabled, int timeSeconds, double safetyMargin)
        {
            var chkEnable = form.Controls.Find("_chkEnableDynamicCharging", true).FirstOrDefault() as CheckBox;
            var nudTime = form.Controls.Find("_nudChargingTime", true).FirstOrDefault() as NumericUpDown;
            var nudSafety = form.Controls.Find("_nudSafetyMargin", true).FirstOrDefault() as NumericUpDown;

            if (chkEnable != null) chkEnable.Checked = enabled;
            if (nudTime != null) nudTime.Value = timeSeconds;
            if (nudSafety != null) nudSafety.Value = (decimal)safetyMargin;
        }
        #endregion

        #region Start Point Settings
        /// <summary>
        /// Gets start point settings from form
        /// </summary>
        public (bool useCustom, Point startPoint) GetStartPointSettings(Form form)
        {
            var chkUseCustom = form.Controls.Find("_chkUseCustomStartPoint", true).FirstOrDefault() as CheckBox;
            var lblCurrent = form.Controls.Find("_lblCurrentStartPoint", true).FirstOrDefault() as Label;

            bool useCustom = chkUseCustom?.Checked ?? false;
            Point startPoint = new Point(10, 10);

            if (lblCurrent != null && lblCurrent.Tag != null)
            {
                startPoint = (Point)lblCurrent.Tag;
            }

            return (useCustom, startPoint);
        }

        /// <summary>
        /// Applies start point settings to form
        /// </summary>
        public void ApplyStartPointSettings(Form form, bool useCustom, Point startPoint)
        {
            var chkUseCustom = form.Controls.Find("_chkUseCustomStartPoint", true).FirstOrDefault() as CheckBox;
            var lblCurrent = form.Controls.Find("_lblCurrentStartPoint", true).FirstOrDefault() as Label;

            if (chkUseCustom != null)
                chkUseCustom.Checked = useCustom;

            if (lblCurrent != null)
            {
                lblCurrent.Text = $"Current: ({startPoint.X}, {startPoint.Y})";
                lblCurrent.Tag = startPoint;
            }
        }
        #endregion

        #region Goal Ordering Helpers
        /// <summary>
        /// Applies goal ordering setting to form
        /// </summary>
        public void ApplyOrderGoalsByDistance(Form form, bool orderByDistance)
        {
            // Apply to all enabled algorithms in the grid
            var dgv = form.Controls.Find("_dgvAlgorithems", true).FirstOrDefault() as DataGridView;
            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["colEnabled"]?.Value != null && (bool)row.Cells["colEnabled"].Value)
                    {
                        var parameters = row.Tag as Dictionary<string, object>;
                        if (parameters != null)
                        {
                            parameters["OrderGoalsByDistance"] = orderByDistance;
                            UpdateParameterSummary(row.Index, parameters);
                        }
                    }
                }
            }
        }

        private void UpdateParameterSummary(int rowIndex, Dictionary<string, object> parameters)
        {
            // This method would be implemented to update the display
            // For now, it's a placeholder
        }
        #endregion

        #region Distance Metric Helpers
        /// <summary>
        /// Gets selected distance metric from form
        /// </summary>
        public string GetSelectedDistanceMetric(Form form)
        {
            var dgv = form.Controls.Find("_dgvAlgorithems", true).FirstOrDefault() as DataGridView;
            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["colEnabled"]?.Value != null && (bool)row.Cells["colEnabled"].Value)
                    {
                        string metric = row.Cells["colMetric"]?.Value?.ToString();
                        if (!string.IsNullOrEmpty(metric))
                            return metric;
                    }
                }
            }
            return "Manhattan";
        }

        /// <summary>
        /// Applies distance metric to form
        /// </summary>
        public void ApplyDistanceMetric(Form form, string metric)
        {
            var dgv = form.Controls.Find("_dgvAlgorithems", true).FirstOrDefault() as DataGridView;
            if (dgv != null)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["colEnabled"]?.Value != null && (bool)row.Cells["colEnabled"].Value)
                    {
                        row.Cells["colMetric"].Value = metric;
                    }
                }
            }
        }
        #endregion

        #region Path Analysis
        /// <summary>
        /// Analyzes path for errors
        /// </summary>
        public PathErrorAnalysis AnalyzePathDetailed(IReadOnlyList<PathNode> path, MapGrid grid)
        {
            var analysis = new PathErrorAnalysis();

            for (int i = 0; i < path.Count; i++)
            {
                var node = path[i];
                var cell = grid[node.X, node.Y];

                if (cell.ElementType == MapElementType.Wall)
                {
                    analysis.HasError = true;
                    analysis.IsPathValid = false;
                    analysis.Errors.Add(new PathError
                    {
                        Type = "WallCollision",
                        Message = $"CRITICAL: Path goes through wall at ({node.X},{node.Y})",
                        Location = new Point(node.X, node.Y),
                        StepIndex = i
                    });
                }

                if (cell.ElementType == MapElementType.Window)
                {
                    analysis.HasError = true;
                    analysis.IsPathValid = false;
                    analysis.Errors.Add(new PathError
                    {
                        Type = "WindowCrossing",
                        Message = $"WARNING: Path crosses window at ({node.X},{node.Y})",
                        Location = new Point(node.X, node.Y),
                        StepIndex = i
                    });
                }

                if (cell.OccupyingObstacle != null)
                {
                    analysis.HasError = true;
                    analysis.Errors.Add(new PathError
                    {
                        Type = "DynamicObstacleCollision",
                        Message = $"COLLISION: Robot collided with {cell.OccupyingObstacle.Type} at ({node.X},{node.Y})",
                        Location = new Point(node.X, node.Y),
                        StepIndex = i
                    });
                }
            }

            return analysis;
        }
        #endregion
    }

    #region Helper Result Classes
    /// <summary>
    /// Result of sequential path finding
    /// </summary>
    public sealed class SequentialPathResult
    {
        public bool Success { get; set; }
        public List<PathNode> Path { get; set; }
        public Point CurrentPos { get; set; }
        public double TotalTimeMs { get; set; }
        public string ErrorMessage { get; set; }

        public static SequentialPathResult CreateSuccess(List<PathNode> path, Point currentPos, double totalTimeMs)
        {
            return new SequentialPathResult
            {
                Success = true,
                Path = path,
                CurrentPos = currentPos,
                TotalTimeMs = totalTimeMs
            };
        }

        public static SequentialPathResult CreateFailure(string errorMessage)
        {
            return new SequentialPathResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                Path = new List<PathNode>()
            };
        }
    }

    /// <summary>
    /// Result of return path finding
    /// </summary>
    public sealed class ReturnPathResult
    {
        public bool Success { get; set; }
        public List<PathNode> Path { get; set; }
        public double TotalTimeMs { get; set; }

        public static ReturnPathResult CreateSuccess(List<PathNode> path, double totalTimeMs)
        {
            return new ReturnPathResult
            {
                Success = true,
                Path = path,
                TotalTimeMs = totalTimeMs
            };
        }
    }

    /// <summary>
    /// Path error analysis result
    /// </summary>
    public class PathErrorAnalysis
    {
        public bool HasError { get; set; }
        public List<PathError> Errors { get; set; } = new List<PathError>();
        public bool IsPathValid { get; set; } = true;
    }
    #endregion
}
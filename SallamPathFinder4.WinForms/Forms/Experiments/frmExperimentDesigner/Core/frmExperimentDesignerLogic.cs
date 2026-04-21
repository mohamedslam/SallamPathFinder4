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
        public List<string> GetSelectedAlgorithms(Form form)
        {
            var algorithms = new List<string>();

            var chkAStar = form.Controls.Find("_chkAStar", true).FirstOrDefault() as CheckBox;
            var chkSPPA = form.Controls.Find("_chkSPPA", true).FirstOrDefault() as CheckBox;
            var chkSPPA_DL = form.Controls.Find("_chkSPPA_DL", true).FirstOrDefault() as CheckBox;
            var chkACO = form.Controls.Find("_chkACO", true).FirstOrDefault() as CheckBox;
            var chkDStar = form.Controls.Find("_chkDStar", true).FirstOrDefault() as CheckBox;
            var chkKNN = form.Controls.Find("_chkKNN", true).FirstOrDefault() as CheckBox;
            var chkBruteForce = form.Controls.Find("_chkBruteForce", true).FirstOrDefault() as CheckBox;

            if (chkAStar?.Checked == true) algorithms.Add("AStar");
            if (chkSPPA?.Checked == true) algorithms.Add("SPPA");
            if (chkSPPA_DL?.Checked == true) algorithms.Add("SPPA_DL");
            if (chkACO?.Checked == true) algorithms.Add("ACO");
            if (chkDStar?.Checked == true) algorithms.Add("DStar");
            if (chkKNN?.Checked == true) algorithms.Add("KNN");
            if (chkBruteForce?.Checked == true) algorithms.Add("BruteForce");

            return algorithms;
        }

        public List<string> GetSelectedMetrics(Form form)
        {
            var metrics = new List<string>();
            var clb = form.Controls.Find("_clbDistanceMetrics", true).FirstOrDefault() as CheckedListBox;

            if (clb != null)
            {
                foreach (var item in clb.CheckedItems)
                {
                    metrics.Add(item.ToString());
                }
            }

            if (metrics.Count == 0) metrics.Add("Manhattan");
            return metrics;
        }
        #endregion

        #region Settings Management
        public SallamPathFinder4.Core.Models.Experiments.ExperimentSettings GetCurrentSettings(Form form)
        {
            var nudGoalCount = form.Controls.Find("_nudGoalCount", true).FirstOrDefault() as NumericUpDown;
            var nudParkingCount = form.Controls.Find("_nudParkingCount", true).FirstOrDefault() as NumericUpDown;
            var nudStaticObstacles = form.Controls.Find("_nudStaticObstacles", true).FirstOrDefault() as NumericUpDown;
            var nudDynamicObstacles = form.Controls.Find("_nudDynamicObstacles", true).FirstOrDefault() as NumericUpDown;
            var nudHeuristicWeight = form.Controls.Find("_nudHeuristicWeight", true).FirstOrDefault() as NumericUpDown;
            var nudSearchLimit = form.Controls.Find("_nudSearchLimit", true).FirstOrDefault() as NumericUpDown;
            var nudIterations = form.Controls.Find("_nudIterations", true).FirstOrDefault() as NumericUpDown;
            var chkAllowDiagonals = form.Controls.Find("_chkAllowDiagonals", true).FirstOrDefault() as CheckBox;
            var chkHeavyDiagonals = form.Controls.Find("_chkHeavyDiagonals", true).FirstOrDefault() as CheckBox;
            var chkSaveScreenshots = form.Controls.Find("_chkSaveScreenshots", true).FirstOrDefault() as CheckBox;
            var chkSaveReplay = form.Controls.Find("_chkSaveReplay", true).FirstOrDefault() as CheckBox;
            var chkShowPath = form.Controls.Find("_chkShowPathOnScreenshots", true).FirstOrDefault() as CheckBox;
            var txtExperimentName = form.Controls.Find("_txtExperimentName", true).FirstOrDefault() as TextBox;
            var txtSavePath = form.Controls.Find("_txtSavePath", true).FirstOrDefault() as TextBox;
            var (enableDynamicCharging, chargingTime, safetyMargin) = GetDynamicChargingSettings(form);
            var (useCustomStartPoint, customStartPoint) = GetStartPointSettings(form);
            bool orderGoalsByDistance = GetOrderGoalsByDistance(form);

            return new SallamPathFinder4.Core.Models.Experiments.ExperimentSettings
            {
                ExperimentName = txtExperimentName?.Text ?? "Experiment",
                SelectedAlgorithms = GetSelectedAlgorithms(form),
                SelectedMetrics = GetSelectedMetrics(form),
                GoalCount = (int)(nudGoalCount?.Value ?? 5),
                ParkingCount = (int)(nudParkingCount?.Value ?? 2),
                StaticObstacles = (int)(nudStaticObstacles?.Value ?? 20),
                DynamicObstacles = (int)(nudDynamicObstacles?.Value ?? 5),
                HeuristicWeight = (int)(nudHeuristicWeight?.Value ?? 2),
                SearchLimit = (int)(nudSearchLimit?.Value ?? 20000),
                AllowDiagonals = chkAllowDiagonals?.Checked == true,
                HeavyDiagonals = chkHeavyDiagonals?.Checked == true,
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
                OrderGoalsByDistance = orderGoalsByDistance,
                DistanceMetric = GetSelectedDistanceMetric(form)
            };
        }

        public void ApplySettingsToForm(Form form, SallamPathFinder4.Core.Models.Experiments.ExperimentSettings settings)
        {
            var txtExperimentName = form.Controls.Find("_txtExperimentName", true).FirstOrDefault() as TextBox;
            var nudGoalCount = form.Controls.Find("_nudGoalCount", true).FirstOrDefault() as NumericUpDown;
            var nudParkingCount = form.Controls.Find("_nudParkingCount", true).FirstOrDefault() as NumericUpDown;
            var nudStaticObstacles = form.Controls.Find("_nudStaticObstacles", true).FirstOrDefault() as NumericUpDown;
            var nudDynamicObstacles = form.Controls.Find("_nudDynamicObstacles", true).FirstOrDefault() as NumericUpDown;
            var nudHeuristicWeight = form.Controls.Find("_nudHeuristicWeight", true).FirstOrDefault() as NumericUpDown;
            var nudSearchLimit = form.Controls.Find("_nudSearchLimit", true).FirstOrDefault() as NumericUpDown;
            var nudIterations = form.Controls.Find("_nudIterations", true).FirstOrDefault() as NumericUpDown;
            var chkAllowDiagonals = form.Controls.Find("_chkAllowDiagonals", true).FirstOrDefault() as CheckBox;
            var chkHeavyDiagonals = form.Controls.Find("_chkHeavyDiagonals", true).FirstOrDefault() as CheckBox;
            var chkSaveScreenshots = form.Controls.Find("_chkSaveScreenshots", true).FirstOrDefault() as CheckBox;
            var chkSaveReplay = form.Controls.Find("_chkSaveReplay", true).FirstOrDefault() as CheckBox;
            var chkShowPath = form.Controls.Find("_chkShowPathOnScreenshots", true).FirstOrDefault() as CheckBox;
            var txtSavePath = form.Controls.Find("_txtSavePath", true).FirstOrDefault() as TextBox;
            var clbMetrics = form.Controls.Find("_clbDistanceMetrics", true).FirstOrDefault() as CheckedListBox;

            if (txtExperimentName != null) txtExperimentName.Text = settings.ExperimentName;
            if (nudGoalCount != null) nudGoalCount.Value = Math.Clamp(settings.GoalCount, 1, 50);
            if (nudParkingCount != null) nudParkingCount.Value = Math.Clamp(settings.ParkingCount, 1, 10);
            if (nudStaticObstacles != null) nudStaticObstacles.Value = Math.Clamp(settings.StaticObstacles, 0, 500);
            if (nudDynamicObstacles != null) nudDynamicObstacles.Value = Math.Clamp(settings.DynamicObstacles, 0, 50);
            if (nudHeuristicWeight != null) nudHeuristicWeight.Value = Math.Clamp(settings.HeuristicWeight, 1, 10);
            if (nudSearchLimit != null) nudSearchLimit.Value = Math.Clamp(settings.SearchLimit, 1000, 100000);
            if (nudIterations != null) nudIterations.Value = Math.Clamp(settings.Iterations, 1, 100);
            if (chkAllowDiagonals != null) chkAllowDiagonals.Checked = settings.AllowDiagonals;
            if (chkHeavyDiagonals != null) chkHeavyDiagonals.Checked = settings.HeavyDiagonals;
            if (chkSaveScreenshots != null) chkSaveScreenshots.Checked = settings.SaveScreenshots;
            if (chkSaveReplay != null) chkSaveReplay.Checked = settings.SaveReplay;
            if (chkShowPath != null) chkShowPath.Checked = settings.ShowPathOnScreenshots;
            if (txtSavePath != null && !string.IsNullOrEmpty(settings.SavePath)) txtSavePath.Text = settings.SavePath;

            if (clbMetrics != null && settings.SelectedMetrics != null)
            {
                for (int i = 0; i < clbMetrics.Items.Count; i++)
                {
                    string item = clbMetrics.Items[i].ToString();
                    clbMetrics.SetItemChecked(i, settings.SelectedMetrics.Contains(item));
                }
            }
            ApplyDynamicChargingSettings(form, settings.EnableDynamicCharging,settings.ChargingTimeSeconds, settings.SafetyMarginPercent);
            ApplyStartPointSettings(form, settings.UseCustomStartPoint, settings.CustomStartPoint);
            ApplyOrderGoalsByDistance(form, settings.OrderGoalsByDistance);
            ApplyDistanceMetric(form, settings.DistanceMetric);
        }

        public void SaveSettingsToFile(SallamPathFinder4.Core.Models.Experiments.ExperimentSettings settings, string filePath)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public SallamPathFinder4.Core.Models.Experiments.ExperimentSettings LoadSettingsFromFile(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<SallamPathFinder4.Core.Models.Experiments.ExperimentSettings>(json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Robot and ML Settings
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
        public List<Point> GetRealGoals(MainViewModel viewModel, bool useCurrentMap)
        {
            if (!useCurrentMap || viewModel?.Goals == null || viewModel.Goals.Count == 0)
                return new List<Point>();

            return viewModel.Goals.Select(g => g.Location).ToList();
        }

        public List<Point> GetParkingPoints(MainViewModel viewModel, bool useCurrentMap)
        {
            if (!useCurrentMap || viewModel?.ParkingPoints == null || viewModel.ParkingPoints.Count == 0)
                return new List<Point>();

            return viewModel.ParkingPoints.Select(p => p.Location).ToList();
        }
        #endregion

        #region Pathfinding
        public IPathFinder CreateAlgorithmFinder(MapGrid grid, string algorithm, MLSettings mlSettings)
        {
            var factory = new AlgorithmFactory(grid);
            var algoType = ExperimentSharedLogic.GetAlgorithmType(algorithm);

            if (algoType == AlgorithmType.SPPA_DL)
            {
                return factory.Create(algoType, mlSettings.UseNeuralNetwork,
                    mlSettings.CollectTrainingData, mlSettings.LearningRate);
            }

            return factory.Create(algoType);
        }

        public void ConfigureFinder(IPathFinder finder, string metric, Form form)
        {
            var chkAllowDiagonals = form.Controls.Find("_chkAllowDiagonals", true).FirstOrDefault() as CheckBox;
            var chkHeavyDiagonals = form.Controls.Find("_chkHeavyDiagonals", true).FirstOrDefault() as CheckBox;
            var nudHeuristicWeight = form.Controls.Find("_nudHeuristicWeight", true).FirstOrDefault() as NumericUpDown;
            var nudSearchLimit = form.Controls.Find("_nudSearchLimit", true).FirstOrDefault() as NumericUpDown;
             
            finder.Metric = ExperimentSharedLogic.GetDistanceMetric(metric);
            finder.AllowDiagonals = chkAllowDiagonals?.Checked == true;
            finder.HeavyDiagonals = chkHeavyDiagonals?.Checked == true;
            finder.HeuristicWeight = nudHeuristicWeight != null ? (int)nudHeuristicWeight.Value : 2;
            finder.SearchLimit = nudSearchLimit != null ? (int)nudSearchLimit.Value : 20000;
        }

        public async Task<SequentialPathResult> FindSequentialPath(IPathFinder finder, Point start, List<Point> goals)
        {
            System.Diagnostics.Debug.WriteLine($"[FindSequentialPath] START: start=({start.X},{start.Y}), goals={goals.Count}");
            var fullPath = new List<PathNode>();
            Point currentPos = start;
            double totalTimeMs = 0;

            for (int i = 0; i < goals.Count; i++)
            {
                // التحقق من صحة الهدف
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
                System.Diagnostics.Debug.WriteLine($"[FindSequentialPath] Goal {i + 1} completed. Path so far: {fullPath.Count} cells");
            
        }
            System.Diagnostics.Debug.WriteLine($"[FindSequentialPath] SUCCESS: Total path={fullPath.Count} cells, Time={totalTimeMs:F2}ms");
            return SequentialPathResult.CreateSuccess(fullPath, currentPos, totalTimeMs);
        }

        public async Task<ReturnPathResult> FindReturnPath(IPathFinder finder, Point currentPos, List<Point> parkingPoints)
        {
            System.Diagnostics.Debug.WriteLine($"[FindReturnPath] START: currentPos=({currentPos.X},{currentPos.Y}), parkingPoints={parkingPoints?.Count ?? 0}");
            if (parkingPoints == null || parkingPoints.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[FindReturnPath] No parking points available");
                return null;
            }

            // البحث عن أقرب نقطة باركينج
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
                System.Diagnostics.Debug.WriteLine("[FindReturnPath] No valid parking point found");
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"[FindReturnPath] Nearest parking: ({nearestParking.X},{nearestParking.Y}), Distance={minDistance}");
            
            var pathResult = await Task.Run(() => finder.FindPath(currentPos, nearestParking));

            if (!pathResult.Success || pathResult.Path == null)
            {
                System.Diagnostics.Debug.WriteLine($"[FindReturnPath] Failed to find return path: {pathResult.ErrorMessage}");
                return null;
            }
            else { 
                        System.Diagnostics.Debug.WriteLine($"[FindReturnPath] Result: Success={pathResult.Success}, PathLength={pathResult.PathLength}");
            }

            return ReturnPathResult.CreateSuccess(pathResult.Path.ToList(), pathResult.ComputationTimeSeconds * 1000);
        }

        public List<PathNode> CombinePaths(List<PathNode> forwardPath, List<PathNode> returnPath)
        {
            var combined = new List<PathNode>(forwardPath);

            if (returnPath != null && returnPath.Count > 0)
            {
                combined.AddRange(returnPath.Skip(1));
            }

            return combined;
        }

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
                InvalidMoveCount = 0
            };
        }
        #endregion

        #region Export
        public void SaveResultsToCsv(List<ComparisonResult> results, string filePath)
        {
            using var writer = new StreamWriter(filePath);

            // CSV Header with new battery columns
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

            System.Diagnostics.Debug.WriteLine($"[SaveResultsToCsv] Saved {results.Count} results to {filePath}");
        }

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
        #region Private Method - CalculateAndFillResultData

        /// <summary>
        /// Calculates all battery, time, and path statistics and fills the result object
        /// </summary>
        public  void CalculateAndFillResultData(
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
            // ========== 1. BASIC PATH METRICS ==========
            result.PathLength = fullPath.Count;
            result.Path = fullPath.Select(p => new Point(p.X, p.Y)).ToList();
            result.ComputationTimeMs = pathfindingTimeMs;
            result.StartPointUsed = startPoint;
            result.EndPointReached = endPoint;
            result.OrderedByDistance = orderGoalsByDistance;

            // ========== 2. GOAL ORDER ==========
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

            // ========== 3. TIME STATISTICS ==========
            if (cellSizeCm <= 0) cellSizeCm = 10.0;
            double totalDistanceCm = fullPath.Count * cellSizeCm;
            double travelTimeSeconds = totalDistanceCm / Math.Max(0.1, robotSettings.InitialSpeedCmS);

            result.TotalTravelTimeSeconds = travelTimeSeconds;
            result.AverageActualSpeed = travelTimeSeconds > 0 ? totalDistanceCm / travelTimeSeconds : robotSettings.InitialSpeedCmS;

            // ========== 4. BATTERY STATISTICS ==========
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

            // ========== 5. TOTAL TIME ==========
            result.TotalOverheadTimeSeconds = 0; // Can be updated when actual charging occurs
            result.TotalTimeSeconds = travelTimeSeconds + totalChargingTimeSeconds;

            // ========== 6. DEBUG OUTPUT ==========
            System.Diagnostics.Debug.WriteLine($"[CalculateResult] PathLength={result.PathLength}, Distance={totalDistanceCm:F1}cm");
            System.Diagnostics.Debug.WriteLine($"[CalculateResult] Battery: Initial={result.InitialBatteryPercent:F1}%, Consumed={result.TotalBatteryConsumedPercent:F1}%, Final={result.FinalBatteryPercent:F1}%");
            System.Diagnostics.Debug.WriteLine($"[CalculateResult] Charging: Units={result.TotalChargingUnits:F2}, Cycles={result.TotalChargingCycles}, Time={result.TotalChargingTimeSeconds:F0}s");
            System.Diagnostics.Debug.WriteLine($"[CalculateResult] Time: Travel={result.TotalTravelTimeSeconds:F2}s, Total={result.TotalTimeSeconds:F2}s");
            System.Diagnostics.Debug.WriteLine($"[CalculateResult] Start=({result.StartPointUsed.X},{result.StartPointUsed.Y}), End=({result.EndPointReached.X},{result.EndPointReached.Y})");
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
        /// Updates start point display
        /// </summary>
        public void UpdateStartPointDisplay(Form form, Point startPoint)
        {
            var lblCurrent = form.Controls.Find("_lblCurrentStartPoint", true).FirstOrDefault() as Label;
            if (lblCurrent != null)
            {
                lblCurrent.Text = $"Current: ({startPoint.X}, {startPoint.Y})";
                lblCurrent.Tag = startPoint;
            }
        }
        #endregion

        #region Goal Ordering Settings
        /// <summary>
        /// Gets goal ordering setting from form
        /// </summary>
        public bool GetOrderGoalsByDistance(Form form)
        {
            var chkOrder = form.Controls.Find("_chkOrderGoalsByDistance", true).FirstOrDefault() as CheckBox;
            return chkOrder?.Checked ?? false;
        }
        #endregion

        #region Distance Metric Helpers

        /// <summary>
        /// Gets selected distance metric from form
        /// </summary>
        public string GetSelectedDistanceMetric(Form form)
        {
            var cboMetric = form.Controls.Find("_cboDistanceMetric", true).FirstOrDefault() as ComboBox;
            if (cboMetric != null && cboMetric.SelectedItem != null)
            {
                string selected = cboMetric.SelectedItem.ToString();
                if (selected.Contains("Manhattan")) return "Manhattan";
                if (selected.Contains("Euclidean")) return "Euclidean";
                if (selected.Contains("MaxDXDY")) return "MaxDXDY";
                if (selected.Contains("DiagonalShortcut")) return "DiagonalShortcut";
                if (selected.Contains("EuclideanNoSQR")) return "EuclideanNoSQR";
                if (selected.Contains("Custom")) return "Custom";
            }
            return "Manhattan";
        }

        /// <summary>
        /// Applies distance metric to form
        /// </summary>
        public void ApplyDistanceMetric(Form form, string metric)
        {
            var cboMetric = form.Controls.Find("_cboDistanceMetric", true).FirstOrDefault() as ComboBox;
            if (cboMetric == null) return;

            int index = metric switch
            {
                "Manhattan" => 0,
                "Euclidean" => 1,
                "MaxDXDY" => 2,
                "DiagonalShortcut" => 3,
                "EuclideanNoSQR" => 4,
                "Custom" => 5,
                _ => 0
            };

            if (index < cboMetric.Items.Count)
                cboMetric.SelectedIndex = index;
        }

        #endregion

        #region Start Point Helpers

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
            var chkOrder = form.Controls.Find("_chkOrderGoalsByDistance", true).FirstOrDefault() as CheckBox;
            if (chkOrder != null)
                chkOrder.Checked = orderByDistance;
        }


        #endregion
   
        public PathErrorAnalysis AnalyzePathDetailed(IReadOnlyList<PathNode> path, MapGrid grid)
        {
            var analysis = new PathErrorAnalysis();

            for (int i = 0; i < path.Count; i++)
            {
                var node = path[i];
                var cell = grid[node.X, node.Y];

                // فحص عبور الجدران
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

                // فحص عبور النوافذ
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

                // فحص الاقتراب الشديد من العقبات
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
    }

    #region Helper Result Classes

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

    public class PathErrorAnalysis
    {
        public bool HasError { get; set; }
        public List<PathError> Errors { get; set; } = new List<PathError>();
        public bool IsPathValid { get; set; } = true;
    }

    #endregion


}
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
using System.Text.Json;
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
        public ExperimentSettings GetCurrentSettings(Form form)
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

            return new ExperimentSettings
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
                SavePath = txtSavePath?.Text ?? string.Empty
            };
        }

        public void ApplySettingsToForm(Form form, ExperimentSettings settings)
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
        }

        public void SaveSettingsToFile(ExperimentSettings settings, string filePath)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

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
            var fullPath = new List<PathNode>();
            Point currentPos = start;
            double totalTimeMs = 0;

            for (int i = 0; i < goals.Count; i++)
            {
                var pathResult = await Task.Run(() => finder.FindPath(currentPos, goals[i]));

                if (!pathResult.Success)
                {
                    return SequentialPathResult.CreateFailure($"No path to goal {i + 1}");
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

        public async Task<ReturnPathResult> FindReturnPath(IPathFinder finder, Point currentPos, List<Point> parkingPoints)
        {
            if (parkingPoints == null || parkingPoints.Count == 0)
                return null;

            var nearestParking = parkingPoints
                .OrderBy(p => Math.Abs(p.X - currentPos.X) + Math.Abs(p.Y - currentPos.Y))
                .FirstOrDefault();

            if (nearestParking == null)
                return null;

            var pathResult = await Task.Run(() => finder.FindPath(currentPos, nearestParking));

            if (!pathResult.Success || pathResult.Path == null)
                return null;

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
            writer.WriteLine("Algorithm,Metric,Iteration,Success,PathLength,TimeMs,BatteryRemaining,Collisions,Errors,AvgSpeed,GoalCount,ParkingCount,StaticObstacles,DynamicObstacles,InitialScreenshot,PathScreenshot,CompletedScreenshot,ErrorMessage");

            foreach (var r in results)
            {
                writer.WriteLine($"{r.Algorithm},{r.Metric},{r.Iteration},{r.Success},{r.PathLength},{r.ComputationTimeMs:F2}," +
                    $"{r.RemainingBattery:F1},{r.CollisionCount},{r.InvalidMoveCount},{r.AverageActualSpeed:F1}," +
                    $"{r.GoalCount},{r.ParkingCount},{r.StaticObstacles},{r.DynamicObstacles}," +
                    $"\"{r.InitialScreenshotPath}\",\"{r.PathScreenshotPath}\",\"{r.CompletedScreenshotPath}\"," +
                    $"\"{r.ErrorMessage}\"");
            }
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
    #endregion
}
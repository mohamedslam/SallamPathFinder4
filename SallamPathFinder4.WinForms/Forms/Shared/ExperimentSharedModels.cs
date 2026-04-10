#region File Header
/// <summary>
/// File: ExperimentSharedModels.cs
/// Description: Shared data models for all experiment-related forms
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Robot;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Shared
{
    /// <summary>
    /// Represents the result of a single experiment run
    /// </summary>
    public class ComparisonResult
    {
        public string Algorithm { get; set; }
        public string Metric { get; set; }
        public int Iteration { get; set; }
        public bool Success { get; set; }
        public int PathLength { get; set; }
        public double ComputationTimeMs { get; set; }
        public double RemainingBattery { get; set; }
        public int CollisionCount { get; set; }
        public int InvalidMoveCount { get; set; }
        public double AverageActualSpeed { get; set; }
        public double ReturnPathLength { get; set; }
        public int GoalCount { get; set; }
        public int ParkingCount { get; set; }
        public int StaticObstacles { get; set; }
        public int DynamicObstacles { get; set; }
        public string RobotName { get; set; }
        public double RobotSpeedCmS { get; set; }
        public double RobotInitialBatteryPercent { get; set; }
        public string ErrorMessage { get; set; }
        public string InitialScreenshotPath { get; set; }
        public string PathScreenshotPath { get; set; }
        public string CompletedScreenshotPath { get; set; }
        public string ReplayPath { get; set; }
        public List<Point> Path { get; set; }
    }

    /// <summary>
    /// Represents the complete settings for an experiment
    /// </summary>
    public class ExperimentSettings
    {
        public string ExperimentName { get; set; }
        public List<string> SelectedAlgorithms { get; set; }
        public List<string> SelectedMetrics { get; set; }
        public int GoalCount { get; set; }
        public int ParkingCount { get; set; }
        public int StaticObstacles { get; set; }
        public int DynamicObstacles { get; set; }
        public int HeuristicWeight { get; set; }
        public int SearchLimit { get; set; }
        public bool AllowDiagonals { get; set; }
        public bool HeavyDiagonals { get; set; }
        public RobotSettings RobotSettings { get; set; }
        public MLSettings MLSettings { get; set; }
        public int Iterations { get; set; }
        public bool SaveScreenshots { get; set; }
        public bool SaveReplay { get; set; }
        public bool ShowPathOnScreenshots { get; set; }
        public string SavePath { get; set; }
    }

    /// <summary>
    /// Represents machine learning settings for SPPA-DL algorithm
    /// </summary>
    public class MLSettings
    {
        public bool EnableDynamicLearning { get; set; }
        public double LearningRate { get; set; }
        public bool UseNeuralNetwork { get; set; }
        public bool CollectTrainingData { get; set; }
        public bool TrainBeforeExperiment { get; set; }
    }
}
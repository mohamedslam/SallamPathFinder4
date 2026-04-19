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
        public string GoalOrder { get; set; }
        public bool UsedDynamicCharging { get; set; }
        public double  ChargingTimeSeconds { get; set; }
        public double SafetyMarginPercent { get; set; }
        public bool OrderedByDistance { get; set; }
        public Point StartPointUsed { get; set; }
        public Point EndPointReached { get; set; }
        public double EstimatedTimeSeconds { get; set; }
        // Battery Statistics
        public double InitialBatteryPercent { get; set; }
        public double FinalBatteryPercent { get; set; }
        public double TotalBatteryConsumedPercent { get; set; }
        public double TotalChargingUnits { get; set; }
        public int TotalChargingCycles { get; set; }
        public double TotalChargingTimeSeconds { get; set; }

        // Time Statistics
        public double TotalTravelTimeSeconds { get; set; }
        public double TotalOverheadTimeSeconds { get; set; }
        public double TotalTimeSeconds { get; set; }

        // Failure Analysis Path Errors
        public bool HasPathErrors { get; set; }
        public string PathErrorsJson { get; set; } 
        public string FailureReason { get; set; } = string.Empty;
    }
}
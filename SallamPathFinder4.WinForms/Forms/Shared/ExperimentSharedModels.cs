#region File Header
/// <summary>
/// File: ExperimentSharedModels.cs
/// Description: Shared data models for all experiment-related forms
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-19
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Robot;
using System.Drawing;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Shared
{
    #region Class Documentation
    /// <summary>
    /// Represents the result of a single experiment run
    /// Contains all battery, time, and path statistics
    /// </summary>
    #endregion
    public class ComparisonResult
    {
        #region Constructor
        public ComparisonResult()
        {
            Path = new List<Point>();
            GoalOrder = string.Empty;
            StartPointUsed = Point.Empty;
            EndPointReached = Point.Empty;
            ErrorMessage = string.Empty;
            FailureReason = string.Empty;
        }
        #endregion

        #region Properties - Basic Info
        public string Algorithm { get; set; }
        public string Metric { get; set; }
        public int Iteration { get; set; }
        public bool Success { get; set; }
        public string FailureReason { get; set; }
        public string ErrorMessage { get; set; }
        #endregion

        #region Properties - Path Metrics
        public int PathLength { get; set; }
        public double ComputationTimeMs { get; set; }
        public double ReturnPathLength { get; set; }
        public List<Point> Path { get; set; }
        #endregion

        #region Properties - Battery Statistics
        public double InitialBatteryPercent { get; set; }
        public double FinalBatteryPercent { get; set; }
        public double TotalBatteryConsumedPercent { get; set; }
        public double TotalChargingUnits { get; set; }
        public int TotalChargingCycles { get; set; }
        public double TotalChargingTimeSeconds { get; set; }
        public double RemainingBattery
        {
            get => FinalBatteryPercent;
            set => FinalBatteryPercent = value;
        }
        #endregion

        #region Properties - Time Statistics
        public double TotalTravelTimeSeconds { get; set; }
        public double TotalOverheadTimeSeconds { get; set; }
        public double TotalTimeSeconds { get; set; }
        #endregion

        #region Properties - Path Information
        public Point StartPointUsed { get; set; }
        public Point EndPointReached { get; set; }
        public string GoalOrder { get; set; }
        public bool OrderedByDistance { get; set; }
        #endregion

        #region Properties - Collision and Error Metrics
        public int CollisionCount { get; set; }
        public int InvalidMoveCount { get; set; }
        public double AverageActualSpeed { get; set; }
        #endregion

        #region Properties - Configuration
        public int GoalCount { get; set; }
        public int ParkingCount { get; set; }
        public int StaticObstacles { get; set; }
        public int DynamicObstacles { get; set; }
        public string RobotName { get; set; }
        public double RobotSpeedCmS { get; set; }
        public double RobotInitialBatteryPercent { get; set; }
        public bool UsedDynamicCharging { get; set; }
        public double ChargingTimeSeconds { get; set; }
        public double SafetyMarginPercent { get; set; }
        #endregion

        #region Properties - Screenshot Paths
        public string InitialScreenshotPath { get; set; }
        public string PathScreenshotPath { get; set; }
        public string CompletedScreenshotPath { get; set; }
        public string ReplayPath { get; set; }
        #endregion

        #region Properties - Path Errors
        public bool HasPathErrors { get; set; }
        public string PathErrorsJson { get; set; }
        #endregion

        #region Public Methods - Formatted Strings
        public string GetBatteryStatsText()
        {
            return $"Initial: {InitialBatteryPercent:F1}% | " +
                   $"Final: {FinalBatteryPercent:F1}% | " +
                   $"Consumed: {TotalBatteryConsumedPercent:F1}% | " +
                   $"Charging: {TotalChargingUnits:F2} units ({TotalChargingCycles} cycles, {TotalChargingTimeSeconds:F0}s)";
        }

        public string GetTimeStatsText()
        {
            return $"Travel: {TotalTravelTimeSeconds:F0}s | " +
                   $"Overhead: {TotalOverheadTimeSeconds:F0}s | " +
                   $"Charging: {TotalChargingTimeSeconds:F0}s | " +
                   $"Total: {TotalTimeSeconds:F0}s";
        }

        public string GetPathInfoText()
        {
            return $"Start: ({StartPointUsed.X},{StartPointUsed.Y}) | " +
                   $"End: ({EndPointReached.X},{EndPointReached.Y}) | " +
                   $"Length: {PathLength} cells | " +
                   $"OrderedByDistance: {(OrderedByDistance ? "Yes" : "No")}";
        }

        public override string ToString()
        {
            return $"{Algorithm} - {Metric} - Iter {Iteration}: " +
                   $"Success={Success}, Length={PathLength}, Time={ComputationTimeMs:F2}ms, " +
                   $"Battery={FinalBatteryPercent:F1}%, ChargingUnits={TotalChargingUnits:F2}";
        }
        #endregion
    }
}
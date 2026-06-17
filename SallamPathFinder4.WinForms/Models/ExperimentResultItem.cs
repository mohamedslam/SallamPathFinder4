#region File Header
/// <summary>
/// File: ExperimentResultItem.cs
/// Description: Represents a single experiment result item with full battery, time, and path statistics
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-19
/// </summary>
#endregion

#region Namespace Imports
using System.Drawing;
#endregion

namespace SallamPathFinder4.WinForms.Models
{
    #region Class Documentation
    /// <summary>
    /// Represents a single experiment result item
    /// Used by frmExperimentDesigner, frmExperimentResults, and frmStatisticsViewer
    /// Following DRY principle - single source of truth
    /// </summary>
    #endregion
    public class ExperimentResultItem
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance with default values
        /// </summary>
        public ExperimentResultItem()
        {
            Path = new List<Point>();
            GoalOrder = string.Empty;
            StartPointUsed = Point.Empty;
            EndPointReached = Point.Empty;
            ErrorMessage = string.Empty;
        }
        #endregion

        #region Properties - Basic Info
        public string Algorithm { get; set; }
        public string Metric { get; set; }
        public int Iteration { get; set; }
        #endregion

        #region Properties - Path Metrics
        public int PathLength { get; set; }
        public double ComputationTimeMs { get; set; }
        public bool Success { get; set; }
        public double ReturnPathLength { get; set; }
        #endregion

        #region Properties - Battery Statistics
        /// <summary>Initial battery percentage before experiment (0-100)</summary>
        public double InitialBatteryPercent { get; set; }

        /// <summary>Final battery percentage after experiment (0-100)</summary>
        public double FinalBatteryPercent { get; set; }

        /// <summary>Total battery consumed during experiment (%)</summary>
        public double TotalBatteryConsumedPercent { get; set; }

        /// <summary>Number of charging units used (e.g., 2.3 means 2 full charges + 30%)</summary>
        public double TotalChargingUnits { get; set; }

        /// <summary>Number of full charging cycles performed</summary>
        public int TotalChargingCycles { get; set; }

        /// <summary>Total time spent charging in seconds</summary>
        public double TotalChargingTimeSeconds { get; set; }

        /// <summary>Battery remaining at end of experiment (alias for FinalBatteryPercent)</summary>
        public double RemainingBattery
        {
            get => FinalBatteryPercent;
            set => FinalBatteryPercent = value;
        }

        /// <summary>Success rate for this experiment (0-100)</summary>
        public double SuccessRate { get; set; }
        #endregion

        #region Properties - Time Statistics
        /// <summary>Total time spent traveling (actual movement) in seconds</summary>
        public double TotalTravelTimeSeconds { get; set; }

        /// <summary>Total overhead time (exiting/re-entering path) in seconds</summary>
        public double TotalOverheadTimeSeconds { get; set; }

        /// <summary>Total time (travel + charging + overhead) in seconds</summary>
        public double TotalTimeSeconds { get; set; }
        #endregion

        #region Properties - Path Information
        /// <summary>Complete list of points in the path</summary>
        public List<Point> Path { get; set; }

        /// <summary>Order of goals visited (formatted string)</summary>
        public string GoalOrder { get; set; }

        /// <summary>Start point used for this experiment</summary>
        public Point StartPointUsed { get; set; }

        /// <summary>End point reached at experiment completion</summary>
        public Point EndPointReached { get; set; }

        /// <summary>Whether goals were ordered by distance from start</summary>
        public bool OrderedByDistance { get; set; }
        #endregion

        #region Properties - Collision and Error Metrics
        public int CollisionCount { get; set; }
        public int InvalidMoveCount { get; set; }
        public double AverageActualSpeed { get; set; }
        #endregion

        #region Properties - Screenshot Paths
        public string InitialScreenshotPath { get; set; }
        public string PathScreenshotPath { get; set; }
        public string CompletedScreenshotPath { get; set; }
        #endregion

        #region Properties - Additional Data
        public string ScreenshotPath { get; set; }
        public string ErrorMessage { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns formatted string for display
        /// </summary>
        public override string ToString()
        {
            return $"{Algorithm} - {Metric} - Iter {Iteration}: " +
                   $"Length={PathLength}, Time={ComputationTimeMs:F2}ms, " +
                   $"Battery={FinalBatteryPercent:F1}%, " +
                   $"ChargingUnits={TotalChargingUnits:F2}, " +
                   $"TotalTime={TotalTimeSeconds:F0}s";
        }

        /// <summary>
        /// Gets battery statistics as formatted text
        /// </summary>
        public string GetBatteryStatsText()
        {
            return $"Initial: {InitialBatteryPercent:F1}% | " +
                   $"Final: {FinalBatteryPercent:F1}% | " +
                   $"Consumed: {TotalBatteryConsumedPercent:F1}% | " +
                   $"Charging: {TotalChargingUnits:F2} units ({TotalChargingCycles} cycles, {TotalChargingTimeSeconds:F0}s)";
        }

        /// <summary>
        /// Gets time statistics as formatted text
        /// </summary>
        public string GetTimeStatsText()
        {
            return $"Travel: {TotalTravelTimeSeconds:F0}s | " +
                   $"Overhead: {TotalOverheadTimeSeconds:F0}s | " +
                   $"Charging: {TotalChargingTimeSeconds:F0}s | " +
                   $"Total: {TotalTimeSeconds:F0}s";
        }

        /// <summary>
        /// Gets path information as formatted text
        /// </summary>
        public string GetPathInfoText()
        {
            return $"Start: ({StartPointUsed.X},{StartPointUsed.Y}) | " +
                   $"End: ({EndPointReached.X},{EndPointReached.Y}) | " +
                   $"Length: {PathLength} cells | " +
                   $"OrderedByDistance: {(OrderedByDistance ? "Yes" : "No")}";
        }
        #endregion
        // أضف هذه الخصائص إلى كلاس ExperimentResultItem الموجود في Models

        /// <summary>
        /// هل تم استخدام الشحن الديناميكي
        /// </summary>
        public bool UsedDynamicCharging { get; set; }

        /// <summary>
        /// وقت الشحن بالثواني
        /// </summary>
        public double ChargingTimeSeconds { get; set; }

        /// <summary>
        /// نسبة هامش الأمان للبطارية
        /// </summary>
        public double SafetyMarginPercent { get; set; }  

        /// <summary>
        /// هل يوجد أخطاء في المسار
        /// </summary>
        public bool HasPathErrors { get; set; }

        /// <summary>
        /// سبب الفشل (إن وجد)
        /// </summary>
        public string FailureReason { get; set; }

  
        /// <summary>
        /// السرعة المتوسطة (م/ث)
        /// </summary>
        public double AverageSpeedMs { get; set; }

        /// <summary>
        /// المسافة الكلية بالأمتار
        /// </summary>
        public double TotalDistanceMeters { get; set; }

        /// <summary>
        /// نقاط المسار
        /// </summary>
        public List<Point> PathNodes { get; set; }

        /// <summary>
        /// نقطة البداية
        /// </summary>
        public Point StartPoint { get; set; }

        /// <summary>
        /// نقطة النهاية
        /// </summary>
        public Point EndPoint { get; set; }
    }
}
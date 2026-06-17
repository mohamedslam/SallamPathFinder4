#region File Header
/// <summary>
/// File: RobotSettings.cs
/// Description: Complete robot configuration including physical properties and SPPA-DL parameters
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Models.Robot
{
    #region Class Documentation
    /// <summary>
    /// Complete robot configuration settings including physical properties,
    /// movement parameters, and SPPA-DL learning parameters
    /// </summary>
    #endregion
    public sealed class RobotSettings
    {
        #region Constants
        private const double DEFAULT_WIDTH_CM = 60;
        private const double DEFAULT_LENGTH_CM = 60;
        private const double DEFAULT_HEIGHT_CM = 30;
        private const double DEFAULT_SPEED_CM_S = 10;
        private const double DEFAULT_BATTERY_PERCENT = 100;
        private const double DEFAULT_CONSUMPTION_RATE = 1.0;
        private const double DEFAULT_VIEW_ANGLE = 180.0;
        private const int DEFAULT_DETECTION_RANGE = 2;
        private const double DEFAULT_LEARNING_RATE = 2.0;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes robot settings with default values
        /// </summary>
        public RobotSettings()
        {
            RobotName = "SallamBot";
            WidthCm = DEFAULT_WIDTH_CM;
            LengthCm = DEFAULT_LENGTH_CM;
            HeightCm = DEFAULT_HEIGHT_CM;
            InitialSpeedCmS = DEFAULT_SPEED_CM_S;
            MaxSpeedCmS = DEFAULT_SPEED_CM_S * 5;
            InitialBatteryLevel = DEFAULT_BATTERY_PERCENT;
            BatteryConsumptionRate = DEFAULT_CONSUMPTION_RATE;
            RobotColor = Color.FromArgb(52, 73, 94);

            // SPPA-DL properties
            ViewAngleDegrees = DEFAULT_VIEW_ANGLE;
            DetectionRangeCells = DEFAULT_DETECTION_RANGE;
            LearningRateAlpha = DEFAULT_LEARNING_RATE;
            EnableDynamicLearning = true;
            EnableAutoReplanning = true;
            ShowDetectionZone = true;
            MemoryFilePath = "ObstacleMemory.json";
            SaveMemoryOnEachDetection = true;
            ShowReplanningNotification = false;
            DetectionZoneColor = Color.FromArgb(80, 52, 152, 219);

            // Dynamic Charging defaults
            EnableDynamicCharging = DEFAULT_DYNAMIC_CHARGING_ENABLED;
            ChargingTimeSeconds = DEFAULT_CHARGING_TIME_SECONDS;
            SafetyMarginPercent = DEFAULT_SAFETY_MARGIN_PERCENT;
        }
        #endregion

        #region Properties - Basic Robot Properties
        /// <summary>Name of the robot</summary>
        public string RobotName { get; set; }

        /// <summary>Robot width in centimeters</summary>
        public double WidthCm { get; set; }

        /// <summary>Robot length in centimeters</summary>
        public double LengthCm { get; set; }

        /// <summary>Robot height in centimeters</summary>
        public double HeightCm { get; set; }

        /// <summary>Initial movement speed in cm/s</summary>
        public double InitialSpeedCmS { get; set; }

        /// <summary>Maximum allowed speed in cm/s</summary>
        public double MaxSpeedCmS { get; set; }

        /// <summary>Initial battery level (0-100%)</summary>
        public double InitialBatteryLevel { get; set; }

        /// <summary>Battery consumption rate per meter (%/m)</summary>
        public double BatteryConsumptionRate { get; set; }

        /// <summary>Robot color for map visualization</summary>
        public Color RobotColor { get; set; }
        #endregion

        #region Properties - SPPA-DL Vision & Detection
        /// <summary>Robot's field of view in degrees (90, 180, 270, 360)</summary>
        public double ViewAngleDegrees { get; set; }

        /// <summary>Obstacle detection range in cells (1, 2, 3, ...)</summary>
        public int DetectionRangeCells { get; set; }

        /// <summary>Learning rate alpha (α) for obstacle frequency influence</summary>
        public double LearningRateAlpha { get; set; }

        /// <summary>Enable/disable SPPA-DL dynamic learning feature</summary>
        public bool EnableDynamicLearning { get; set; }

        /// <summary>Enable/disable auto-replanning when obstacle detected</summary>
        public bool EnableAutoReplanning { get; set; }

        /// <summary>Show/hide detection zone visualization on map</summary>
        public bool ShowDetectionZone { get; set; }

        /// <summary>File path for obstacle memory storage</summary>
        public string MemoryFilePath { get; set; }

        /// <summary>Save memory to file after every detection</summary>
        public bool SaveMemoryOnEachDetection { get; set; }

        /// <summary>Show/hide replanning notifications in status bar</summary>
        public bool ShowReplanningNotification { get; set; }

        /// <summary>Color of the detection zone overlay on map</summary>
        public Color DetectionZoneColor { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a deep copy of the robot settings
        /// </summary>
        public RobotSettings Clone()
        {
            return new RobotSettings
            {
                RobotName = this.RobotName,
                WidthCm = this.WidthCm,
                LengthCm = this.LengthCm,
                HeightCm = this.HeightCm,
                InitialSpeedCmS = this.InitialSpeedCmS,
                MaxSpeedCmS = this.MaxSpeedCmS,
                InitialBatteryLevel = this.InitialBatteryLevel,
                BatteryConsumptionRate = this.BatteryConsumptionRate,
                RobotColor = this.RobotColor,
                ViewAngleDegrees = this.ViewAngleDegrees,
                DetectionRangeCells = this.DetectionRangeCells,
                LearningRateAlpha = this.LearningRateAlpha,
                EnableDynamicLearning = this.EnableDynamicLearning,
                EnableAutoReplanning = this.EnableAutoReplanning,
                ShowDetectionZone = this.ShowDetectionZone,
                MemoryFilePath = this.MemoryFilePath,
                SaveMemoryOnEachDetection = this.SaveMemoryOnEachDetection,
                ShowReplanningNotification = this.ShowReplanningNotification,
                DetectionZoneColor = this.DetectionZoneColor,
                // Dynamic Charging
                EnableDynamicCharging = this.EnableDynamicCharging,
                ChargingTimeSeconds = this.ChargingTimeSeconds,
                SafetyMarginPercent = this.SafetyMarginPercent
            };
        }

        /// <summary>
        /// Validates and clamps all settings to valid ranges
        /// </summary>
        public void Validate()
        {
            WidthCm = System.Math.Max(10, System.Math.Min(200, WidthCm));
            LengthCm = System.Math.Max(10, System.Math.Min(200, LengthCm));
            HeightCm = System.Math.Max(5, System.Math.Min(150, HeightCm));
            InitialSpeedCmS = System.Math.Max(1, System.Math.Min(100, InitialSpeedCmS));
            MaxSpeedCmS = System.Math.Max(InitialSpeedCmS, System.Math.Min(200, MaxSpeedCmS));
            InitialBatteryLevel = System.Math.Max(0, System.Math.Min(100, InitialBatteryLevel));
            BatteryConsumptionRate = System.Math.Max(0.1, System.Math.Min(10, BatteryConsumptionRate));
            ViewAngleDegrees = System.Math.Max(45, System.Math.Min(360, ViewAngleDegrees));
            DetectionRangeCells = System.Math.Max(1, System.Math.Min(10, DetectionRangeCells));
            LearningRateAlpha = System.Math.Max(0.1, System.Math.Min(10, LearningRateAlpha));
            // Validate charging settings
            this.ChargingTimeSeconds = Math.Max(60, Math.Min(7200, this.ChargingTimeSeconds)); // 1 min to 2 hours
            this.SafetyMarginPercent = Math.Max(5.0, Math.Min(20.0, this.SafetyMarginPercent)); // 5% to 20%

        }
        #endregion

        #region Constants - Charging
        private const double DEFAULT_SAFETY_MARGIN_PERCENT = 10.0;
        private const int DEFAULT_CHARGING_TIME_SECONDS = 1800; // 30 دقيقة
        private const bool DEFAULT_DYNAMIC_CHARGING_ENABLED = false;
        #endregion

        #region Properties - Dynamic Charging
        /// <summary>
        /// Enable/disable automatic dynamic charging
        /// When disabled, robot stops and waits for user to replace battery
        /// When enabled, robot automatically goes to nearest parking to charge
        /// </summary>
        public bool EnableDynamicCharging { get; set; }

        /// <summary>
        /// Charging time in seconds (user defined: minutes:seconds)
        /// Default: 1800 seconds (30 minutes)
        /// </summary>
        public int ChargingTimeSeconds { get; set; }

        /// <summary>
        /// Safety margin percentage for battery (5-20%)
        /// Robot will go to charge when remaining battery <= needed battery + safety margin
        /// Default: 10%
        /// </summary>
        public double SafetyMarginPercent { get; set; }
        #endregion

    }
}
#region File Header
/// <summary>
/// File: GPSSensor.cs
/// Description: GPS sensor for global positioning
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Map;
using System.Drawing;

namespace SallamPathFinder4.Core.Models.Sensors
{
    /// <summary>
    /// GPS sensor for global positioning and navigation
    /// </summary>
    public class GPSSensor : SensorBase
    {
        #region Properties
        public override SensorType Type => SensorType.GPS;
        public override Color DisplayColor => Color.FromArgb(241, 196, 15); // Yellow

        /// <summary>Position accuracy in meters (CEP)</summary>
        public double Accuracy { get; set; } = 2.5;

        /// <summary>Number of satellites locked</summary>
        public int SatelliteCount { get; set; } = 8;

        /// <summary>Whether differential GPS is enabled</summary>
        public bool UseDifferentialGPS { get; set; } = false;

        /// <summary>Last known position</summary>
        public Point? LastKnownPosition { get; set; }

        /// <summary>HDOP (Horizontal Dilution of Precision)</summary>
        public double HDOP { get; set; } = 1.2;
        #endregion

        #region Constructor
        public GPSSensor()
        {
            SensorName = "GPS Sensor";
            MaxRange = double.MaxValue;
            FieldOfView = 360;
            PowerConsumption = 0.2;
            UpdateRate = 5;
        }
        #endregion

        #region Public Methods
        public override async Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle)
        {
            var result = new DetectionResult();

            // GPS doesn't detect obstacles, it provides position
            result.ObstacleDetected = false;
            result.Confidence = CalculateConfidence();
            result.Timestamp = DateTime.Now;

            LastKnownPosition = robotPosition;

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Calculates current GPS confidence based on satellite count and HDOP
        /// </summary>
        private double CalculateConfidence()
        {
            double satFactor = Math.Min(SatelliteCount / 10.0, 1.0);
            double hdopFactor = Math.Max(0, 1.0 - (HDOP / 5.0));
            double accuracyFactor = Math.Max(0, 1.0 - (Accuracy / 10.0));

            double confidence = satFactor * 0.4 + hdopFactor * 0.3 + accuracyFactor * 0.3;

            if (UseDifferentialGPS)
                confidence += 0.1;

            return Math.Min(0.95, confidence);
        }

        /// <summary>
        /// Gets estimated position error in meters
        /// </summary>
        public double GetEstimatedError()
        {
            double error = Accuracy;
            error *= (1.0 / Math.Sqrt(SatelliteCount));
            error *= HDOP;
            return error;
        }

        public override string GetReadingAsString()
        {
            return $"GPS: {SatelliteCount} satellites, HDOP={HDOP:F2}, Error={GetEstimatedError():F2}m, DGPS={(UseDifferentialGPS ? "On" : "Off")}";
        }

        public override ISensor Clone()
        {
            return new GPSSensor
            {
                SensorId = this.SensorId,
                SensorName = this.SensorName,
                Position = this.Position,
                MountAngle = this.MountAngle,
                IsEnabled = this.IsEnabled,
                UpdateRate = this.UpdateRate,
                PowerConsumption = this.PowerConsumption,
                Accuracy = this.Accuracy,
                SatelliteCount = this.SatelliteCount,
                UseDifferentialGPS = this.UseDifferentialGPS,
                HDOP = this.HDOP
            };
        }
        #endregion
    }
}
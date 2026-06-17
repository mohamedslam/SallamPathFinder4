#region File Header
/// <summary>
/// File: InfraredSensor.cs
/// Description: Infrared proximity sensor implementation
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Map;
using System.Drawing;

namespace SallamPathFinder4.Core.Models.Sensors
{
    /// <summary>
    /// Infrared sensor for short-range obstacle detection
    /// </summary>
    public class InfraredSensor : SensorBase
    {
        #region Properties
        public override SensorType Type => SensorType.Infrared;
        public override Color DisplayColor => Color.FromArgb(231, 76, 60); // Red

        /// <summary>Minimum detectable distance (cm)</summary>
        public double MinRange { get; set; } = 2;

        /// <summary>Maximum detectable distance (cm)</summary>
        public double MaxRange { get; set; } = 100;

        /// <summary>Ambient light sensitivity (0-1, higher = more affected)</summary>
        public double AmbientLightSensitivity { get; set; } = 0.3;

        /// <summary>Surface reflectivity compensation</summary>
        public bool CompensateReflectivity { get; set; } = true;
        #endregion

        #region Constructor
        public InfraredSensor()
        {
            SensorName = "Infrared Sensor";
            FieldOfView = 15;
            PowerConsumption = 0.03;
        }
        #endregion

        #region Public Methods
        public override async Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle)
        {
            double radAngle = (robotAngle + MountAngle) * Math.PI / 180.0;
            double sensorX = robotPosition.X + Position.X * Math.Cos(radAngle) - Position.Y * Math.Sin(radAngle);
            double sensorY = robotPosition.Y + Position.X * Math.Sin(radAngle) + Position.Y * Math.Cos(radAngle);

            var result = await PerformRayCast(grid, new Point((int)sensorX, (int)sensorY), robotAngle + MountAngle, MaxRange);
            result.Angle = MountAngle;

            // IR sensors have less accuracy at longer ranges
            double accuracyFactor = 1.0 - (result.Distance / MaxRange) * 0.5;
            result.Confidence = Math.Max(0.6, accuracyFactor);

            // Add ambient light noise
            var random = new Random();
            double ambientNoise = (random.NextDouble() - 0.5) * AmbientLightSensitivity * 0.2;
            result.Confidence = Math.Max(0.5, result.Confidence + ambientNoise);

            return result;
        }

        public override string GetReadingAsString()
        {
            return $"IR Sensor: Range={MinRange}-{MaxRange}cm, FOV={FieldOfView}°, Ambient sensitivity={AmbientLightSensitivity:F1}";
        }

        public override ISensor Clone()
        {
            return new InfraredSensor
            {
                SensorId = this.SensorId,
                SensorName = this.SensorName,
                Position = this.Position,
                MountAngle = this.MountAngle,
                IsEnabled = this.IsEnabled,
                MaxRange = this.MaxRange,
                FieldOfView = this.FieldOfView,
                UpdateRate = this.UpdateRate,
                PowerConsumption = this.PowerConsumption,
                MinRange = this.MinRange,
                AmbientLightSensitivity = this.AmbientLightSensitivity,
                CompensateReflectivity = this.CompensateReflectivity
            };
        }
        #endregion
    }
}
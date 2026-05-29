#region File Header
/// <summary>
/// File: UltrasonicSensor.cs
/// Description: Ultrasonic distance sensor implementation
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Map;
using System.Drawing;

namespace SallamPathFinder4.Core.Models.Sensors
{
    /// <summary>
    /// Ultrasonic sensor (like HC-SR04) for distance measurement
    /// </summary>
    public class UltrasonicSensor : SensorBase
    {
        #region Properties
        public override SensorType Type => SensorType.Ultrasonic;
        public override Color DisplayColor => Color.FromArgb(46, 204, 113); // Green

        /// <summary>Accuracy of distance measurement in cm</summary>
        public double Accuracy { get; set; } = 1.0;

        /// <summary>Minimum measurable distance in cm</summary>
        public double MinRange { get; set; } = 2;
        #endregion

        #region Constructor
        public UltrasonicSensor()
        {
            SensorName = "Ultrasonic Sensor";
            MaxRange = 400;
            FieldOfView = 30;
            PowerConsumption = 0.05;
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

            // Add noise based on accuracy
            double noise = (new Random().NextDouble() - 0.5) * Accuracy * 2;
            result.Distance += noise;
            result.Confidence = Math.Max(0, 1.0 - (noise / Accuracy));

            return result;
        }

        public override string GetReadingAsString()
        {
            return $"Ultrasonic: Range={MaxRange}cm, FOV={FieldOfView}°, Accuracy={Accuracy:F1}cm";
        }

        public override ISensor Clone()
        {
            return new UltrasonicSensor
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
                Accuracy = this.Accuracy,
                MinRange = this.MinRange
            };
        }
        #endregion
    }
}
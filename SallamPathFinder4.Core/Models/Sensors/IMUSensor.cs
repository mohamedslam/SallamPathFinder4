#region File Header
/// <summary>
/// File: IMUSensor.cs
/// Description: Inertial Measurement Unit (accelerometer + gyroscope)
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Map;
using System.Drawing;

namespace SallamPathFinder4.Core.Models.Sensors
{
    /// <summary>
    /// IMU sensor data reading
    /// </summary>
    public class IMUReading
    {
        public double AccelerometerX { get; set; }  // m/s²
        public double AccelerometerY { get; set; }  // m/s²
        public double AccelerometerZ { get; set; }  // m/s²
        public double GyroscopeX { get; set; }      // degrees/sec
        public double GyroscopeY { get; set; }      // degrees/sec
        public double GyroscopeZ { get; set; }      // degrees/sec
        public double MagnetometerX { get; set; }   // μT
        public double MagnetometerY { get; set; }   // μT
        public double MagnetometerZ { get; set; }   // μT
        public double Temperature { get; set; }     // Celsius
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Calculates orientation from gyroscope integration
        /// </summary>
        public double CalculateOrientation(double previousOrientation, double deltaTime)
        {
            return previousOrientation + GyroscopeZ * deltaTime;
        }

        /// <summary>
        /// Calculates tilt angle from accelerometer
        /// </summary>
        public double CalculateTilt()
        {
            return Math.Atan2(AccelerometerY, AccelerometerZ) * 180 / Math.PI;
        }

        public override string ToString()
        {
            return $"Accel: ({AccelerometerX:F2}, {AccelerometerY:F2}, {AccelerometerZ:F2}), Gyro: ({GyroscopeZ:F1}°/s)";
        }
    }

    /// <summary>
    /// IMU sensor for orientation and motion tracking
    /// </summary>
    public class IMUSensor : SensorBase
    {
        #region Properties
        public override SensorType Type => SensorType.IMU;
        public override Color DisplayColor => Color.FromArgb(52, 152, 219); // Blue

        /// <summary>Current IMU reading</summary>
        public IMUReading CurrentReading { get; private set; } = new IMUReading();

        /// <summary>Accelerometer range (±g)</summary>
        public double AccelerometerRange { get; set; } = 16;

        /// <summary>Gyroscope range (±degrees/sec)</summary>
        public double GyroscopeRange { get; set; } = 2000;

        /// <summary>Gyroscope drift compensation factor</summary>
        public double DriftCompensation { get; set; } = 0.01;

        private double _lastOrientation;
        private DateTime _lastReadingTime;
        #endregion

        #region Constructor
        public IMUSensor()
        {
            SensorName = "IMU Sensor";
            PowerConsumption = 0.1;
            UpdateRate = 100;
            _lastReadingTime = DateTime.Now;
        }
        #endregion

        #region Public Methods
        public override async Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle)
        {
            var result = new DetectionResult();

            // Simulate IMU reading based on robot motion
            var now = DateTime.Now;
            double deltaTime = (now - _lastReadingTime).TotalSeconds;

            // Estimate motion from robot position change
            if (deltaTime > 0 && _lastReadingTime != DateTime.MinValue)
            {
                double orientationChange = (robotAngle - _lastOrientation) % 360;
                if (orientationChange > 180) orientationChange -= 360;

                CurrentReading.GyroscopeZ = orientationChange / deltaTime;
                CurrentReading.GyroscopeZ += (new Random().NextDouble() - 0.5) * DriftCompensation * 10;
            }

            CurrentReading.Timestamp = now;
            _lastOrientation = robotAngle;
            _lastReadingTime = now;

            result.ObstacleDetected = false;
            result.Confidence = 0.95;

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Integrates gyroscope data to get orientation
        /// </summary>
        public double GetIntegratedOrientation(double initialOrientation, double deltaTime)
        {
            return initialOrientation + CurrentReading.GyroscopeZ * deltaTime;
        }

        /// <summary>
        /// Gets corrected orientation using complementary filter
        /// </summary>
        public double GetCorrectedOrientation(double gyroOrientation, double accelOrientation)
        {
            return 0.98 * gyroOrientation + 0.02 * accelOrientation;
        }

        public override string GetReadingAsString()
        {
            return $"IMU: Gyro Z={CurrentReading.GyroscopeZ:F1}°/s, Accel=({CurrentReading.AccelerometerX:F1})";
        }

        public override ISensor Clone()
        {
            return new IMUSensor
            {
                SensorId = this.SensorId,
                SensorName = this.SensorName,
                Position = this.Position,
                MountAngle = this.MountAngle,
                IsEnabled = this.IsEnabled,
                UpdateRate = this.UpdateRate,
                PowerConsumption = this.PowerConsumption,
                AccelerometerRange = this.AccelerometerRange,
                GyroscopeRange = this.GyroscopeRange,
                DriftCompensation = this.DriftCompensation
            };
        }
        #endregion
    }
}
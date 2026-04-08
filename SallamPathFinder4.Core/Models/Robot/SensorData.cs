#region File Header
/// <summary>
/// File: SensorData.cs
/// Description: Sensor readings from the robot (ultrasonic and IMU)
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

namespace SallamPathFinder4.Core.Models.Robot
{
    #region Class Documentation
    /// <summary>
    /// Represents sensor readings from the robot
    /// Includes ultrasonic distance sensors and IMU orientation data
    /// </summary>
    #endregion
    public sealed class SensorData
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance with default values
        /// </summary>
        public SensorData()
        {
            FrontDistance = 0;
            LeftDistance = 0;
            RightDistance = 0;
            BackDistance = 0;
            Pitch = 0;
            Roll = 0;
            Yaw = 0;
            Timestamp = System.DateTime.UtcNow;
        }
        #endregion

        #region Properties - Ultrasonic Sensors
        /// <summary>Front ultrasonic sensor distance in centimeters</summary>
        public double FrontDistance { get; set; }

        /// <summary>Left ultrasonic sensor distance in centimeters</summary>
        public double LeftDistance { get; set; }

        /// <summary>Right ultrasonic sensor distance in centimeters</summary>
        public double RightDistance { get; set; }

        /// <summary>Back ultrasonic sensor distance in centimeters</summary>
        public double BackDistance { get; set; }
        #endregion

        #region Properties - IMU (Gyroscope/Accelerometer)
        /// <summary>Pitch angle in degrees (rotation around X-axis)</summary>
        public double Pitch { get; set; }

        /// <summary>Roll angle in degrees (rotation around Y-axis)</summary>
        public double Roll { get; set; }

        /// <summary>Yaw angle in degrees (rotation around Z-axis)</summary>
        public double Yaw { get; set; }
        #endregion

        #region Properties - Metadata
        /// <summary>Timestamp when sensor data was captured (UTC)</summary>
        public System.DateTime Timestamp { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Checks if any sensor detects an obstacle within the specified range
        /// </summary>
        public bool IsObstacleDetected(double rangeCm)
        {
            return FrontDistance <= rangeCm ||
                   LeftDistance <= rangeCm ||
                   RightDistance <= rangeCm ||
                   BackDistance <= rangeCm;
        }

        /// <summary>
        /// Gets the minimum distance from all sensors
        /// </summary>
        public double GetMinimumDistance()
        {
            return System.Math.Min(FrontDistance,
                   System.Math.Min(LeftDistance,
                   System.Math.Min(RightDistance, BackDistance)));
        }

        /// <summary>
        /// Creates a deep copy of the sensor data
        /// </summary>
        public SensorData Clone()
        {
            return new SensorData
            {
                FrontDistance = this.FrontDistance,
                LeftDistance = this.LeftDistance,
                RightDistance = this.RightDistance,
                BackDistance = this.BackDistance,
                Pitch = this.Pitch,
                Roll = this.Roll,
                Yaw = this.Yaw,
                Timestamp = this.Timestamp
            };
        }
        #endregion

        #region Object Overrides
        /// <summary>
        /// Returns string representation of sensor data
        /// </summary>
        public override string ToString()
        {
            return $"F:{FrontDistance:F1} L:{LeftDistance:F1} R:{RightDistance:F1} B:{BackDistance:F1} | Pitch:{Pitch:F1} Roll:{Roll:F1} Yaw:{Yaw:F1}";
        }
        #endregion
    }
}
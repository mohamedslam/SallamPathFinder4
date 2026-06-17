#region File Header
/// <summary>
/// File: SensorType.cs
/// Description: Enumeration of all supported sensor types
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

namespace SallamPathFinder4.Core.Models.Sensors
{
    /// <summary>
    /// Types of sensors available for robot configuration
    /// </summary>
    public enum SensorType
    {
        /// <summary>
        /// Ultrasonic distance sensor (HC-SR04, etc.)
        /// </summary>
        Ultrasonic,

        /// <summary>
        /// Infrared proximity sensor
        /// </summary>
        Infrared,

        /// <summary>
        /// LiDAR for 360-degree scanning
        /// </summary>
        Lidar,

        /// <summary>
        /// Visual camera
        /// </summary>
        Camera,

        /// <summary>
        /// GPS for global positioning
        /// </summary>
        GPS,

        /// <summary>
        /// Inertial Measurement Unit (accelerometer + gyroscope)
        /// </summary>
        IMU,

        /// <summary>
        /// Bumper sensor for collision detection
        /// </summary>
        Bumper,

        /// <summary>
        /// Temperature sensor
        /// </summary>
        Temperature,

        /// <summary>
        /// Pressure sensor (barometer)
        /// </summary>
        Pressure,

        /// <summary>
        /// Humidity sensor
        /// </summary>
        Humidity,

        /// <summary>
        /// Wind speed/direction sensor (anemometer)
        /// </summary>
        Wind,

        /// <summary>
        /// Proximity sensor for detecting nearby objects
        /// </summary>
        Proximity
    }
}
#region File Header
/// <summary>
/// File: DetectedObstacle.cs
/// Description: Represents an obstacle detected by a sensor
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-06-01
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.Core.Models.Obstacles
{
    /// <summary>
    /// Represents an obstacle detected by a sensor
    /// </summary>
    public sealed class DetectedObstacle
    {
        #region Properties
        /// <summary>
        /// Unique identifier for this detection
        /// </summary>
        public string DetectionId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// ID of the sensor that detected this obstacle
        /// </summary>
        public string SensorId { get; set; }

        /// <summary>
        /// Type of sensor (Ultrasonic, Infrared, Lidar, Camera, etc.)
        /// </summary>
        public string SensorType { get; set; }

        /// <summary>
        /// Grid location of the obstacle
        /// </summary>
        public Point Location { get; set; }

        /// <summary>
        /// Distance from robot to obstacle in centimeters
        /// </summary>
        public double DistanceCm { get; set; }

        /// <summary>
        /// Angle of detection relative to robot forward (degrees)
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Type of obstacle (Adult, Child, Animal, OtherRobot, Equipment)
        /// </summary>
        public ObstacleType ObstacleType { get; set; }

        /// <summary>
        /// Whether the obstacle is moving or stationary
        /// </summary>
        public bool IsMoving { get; set; }

        /// <summary>
        /// Speed of moving obstacle (cm/s), 0 if stationary
        /// </summary>
        public double MovementSpeed { get; set; }

        /// <summary>
        /// Direction of moving obstacle (degrees), 0 if stationary
        /// </summary>
        public double MovementDirection { get; set; }

        /// <summary>
        /// Time when obstacle was first detected
        /// </summary>
        public DateTime FirstDetectionTime { get; set; }

        /// <summary>
        /// Time of last detection
        /// </summary>
        public DateTime LastDetectionTime { get; set; }

        /// <summary>
        /// Number of consecutive detections
        /// </summary>
        public int PersistenceCount { get; set; }

        /// <summary>
        /// Confidence level of detection (0-100%)
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Path to captured image (if camera was used)
        /// </summary>
        public string CapturedImagePath { get; set; }

        /// <summary>
        /// Whether an obstacle was detected (convenience property)
        /// </summary>
        public bool ObstacleDetected => true;  // If instance exists, obstacle is detected

        #endregion

        #region Constructors
        public DetectedObstacle()
        {
            FirstDetectionTime = DateTime.UtcNow;
            LastDetectionTime = DateTime.UtcNow;
            PersistenceCount = 1;
        }

        public DetectedObstacle(string sensorId, string sensorType, Point location, double distanceCm, double angle)
            : this()
        {
            SensorId = sensorId;
            SensorType = sensorType;
            Location = location;
            DistanceCm = distanceCm;
            Angle = angle;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the detection record with a new sighting
        /// </summary>
        public void Update()
        {
            LastDetectionTime = DateTime.UtcNow;
            PersistenceCount++;
        }

        /// <summary>
        /// Returns true if obstacle has been detected consistently
        /// </summary>
        public bool IsConfirmed => PersistenceCount >= 3;

        /// <summary>
        /// Returns true if obstacle is old (not seen for a while)
        /// </summary>
        public bool IsExpired => (DateTime.UtcNow - LastDetectionTime).TotalSeconds > 5;

        /// <summary>
        /// Returns string representation for logging
        /// </summary>
        public override string ToString()
        {
            return $"[{SensorType}] {ObstacleType} at ({Location.X},{Location.Y}) - {DistanceCm:F1}cm, Confirmed: {IsConfirmed}";
        }
        #endregion
    }
}
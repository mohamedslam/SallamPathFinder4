#region File Header
/// <summary>
/// File: ISensor.cs
/// Description: Interface for all robot sensors
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
using System.Drawing;

namespace SallamPathFinder4.Core.Models.Sensors
{
    /// <summary>
    /// Detection result from a sensor
    /// </summary>
    public class DetectionResult
    {
        /// <summary>Whether an obstacle was detected</summary>
        public bool ObstacleDetected { get; set; }

        /// <summary>Distance to detected obstacle (cm)</summary>
        public double Distance { get; set; }

        /// <summary>Angle to detected obstacle (degrees, relative to sensor)</summary>
        public double Angle { get; set; }

        /// <summary>Position of detected obstacle in world coordinates</summary>
        public Point? ObstaclePosition { get; set; }

        /// <summary>Type of obstacle detected</summary>
        public string ObstacleType { get; set; }

        /// <summary>Confidence level of detection (0-1)</summary>
        public double Confidence { get; set; } = 1.0;

        /// <summary>Timestamp of detection</summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // NEW: Sensor information
        /// <summary>ID of the sensor that detected this obstacle</summary>
        public string SensorId { get; set; }

        /// <summary>Type of sensor (Ultrasonic, Infrared, etc.)</summary>
        public string SensorType { get; set; }

        /// <summary>Original distance before noise (cm)</summary>
        public double OriginalDistanceCm { get; set; }

        /// <summary>Amount of noise applied (cm)</summary>
        public double NoiseAppliedCm { get; set; }

        /// <summary>Whether this reading was a dropout</summary>
        public bool IsDropout { get; set; }

        /// <summary>Detected obstacle object (if any)</summary>
        public DetectedObstacle DetectedObstacle { get; set; }

        /// <summary>Whether camera should be triggered</summary>
        public bool TriggerCamera { get; set; }

        public override string ToString()
        {
            return $"Detected: {ObstacleDetected}, Distance: {Distance:F1}cm, Confidence: {Confidence:P0}";
        }
    }



    /// <summary>
    /// Proximity data for objects around the robot
    /// </summary>
    public class ProximityData
    {
        public Point TargetPosition { get; set; }
        public double Distance { get; set; }           // cm
        public double Bearing { get; set; }            // Degrees relative to robot forward
        public double RelativeVelocity { get; set; }   // cm/s
        public string ObjectType { get; set; }

        public override string ToString()
        {
            return $"Object at {Distance:F1}cm, bearing {Bearing:F0}°";
        }
    }

    /// <summary>
    /// Main interface for all sensors
    /// </summary>
    public interface ISensor
    {
        /// <summary>Unique identifier for the sensor</summary>
        string SensorId { get; set; }

        /// <summary>Display name of the sensor</summary>
        string SensorName { get; set; }

        /// <summary>Type of sensor</summary>
        SensorType Type { get; }

        /// <summary>Position on robot (relative coordinates in cm)</summary>
        Point Position { get; set; }

        /// <summary>Mounting angle in degrees (0 = forward, 90 = right)</summary>
        double MountAngle { get; set; }

        /// <summary>Whether the sensor is enabled</summary>
        bool IsEnabled { get; set; }

        /// <summary>Color for visualization</summary>
        Color DisplayColor { get; }

        /// <summary>Maximum detection range in centimeters</summary>
        double MaxRange { get; set; }

        /// <summary>Field of view in degrees</summary>
        double FieldOfView { get; set; }

        /// <summary>Update rate in Hz (readings per second)</summary>
        double UpdateRate { get; set; }

        /// <summary>Power consumption in watts</summary>
        double PowerConsumption { get; set; }

        /// <summary>Detects obstacles and returns detection results</summary>
        Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle);

        /// <summary>Gets sensor reading as string (for debugging)</summary>
        string GetReadingAsString();

        /// <summary>Creates a deep copy of the sensor</summary>
        ISensor Clone();

        /// <summary>Gets the coverage area polygon for visualization</summary>
        List<Point> GetCoveragePolygon(Point robotPosition, double robotAngle, double scale = 1.0);
    }
}
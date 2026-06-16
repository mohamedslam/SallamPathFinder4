#region File Header
/// <summary>
/// File: CameraHandler.cs
/// Description: Handles camera capture and obstacle type identification
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-06-01
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Obstacles;
using System.Drawing;
using System.Drawing.Imaging;
#endregion

namespace SallamPathFinder4.Services.Simulation
{
    /// <summary>
    /// Handles camera capture and obstacle type identification
    /// </summary>
    public sealed class CameraHandler : IDisposable
    {
        #region Constants
        private const string SCREENSHOTS_FOLDER = "Screenshots";
        private const double DETECTION_CONFIDENCE_THRESHOLD = 0.7;
        #endregion

        #region Private Fields
        private readonly Random _random;
        private bool _isDisposed;
        private string _currentScreenshotPath;
        #endregion

        #region Constructor
        public CameraHandler()
        {
            _random = new Random(42);

            // Ensure screenshots directory exists
            if (!Directory.Exists(SCREENSHOTS_FOLDER))
            {
                Directory.CreateDirectory(SCREENSHOTS_FOLDER);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event raised when an obstacle is identified by camera
        /// </summary>
        public event Action<DetectedObstacle, Image> ObstacleIdentified;
        #endregion

        #region Public Methods
        /// <summary>
        /// Captures a screenshot and identifies the obstacle type
        /// </summary>
        /// <param name="obstacle">The detected obstacle</param>
        /// <param name="robotPosition">Current robot position</param>
        /// <param name="robotAngle">Current robot angle</param>
        /// <returns>Updated obstacle with type information</returns>
        public async Task<DetectedObstacle> CaptureAndIdentifyAsync(DetectedObstacle obstacle, Point robotPosition, float robotAngle)
        {
            if (obstacle == null) return null;

            // Simulate camera capture delay
            await Task.Delay(100);

            // Generate screenshot path
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            string fileName = $"Obstacle_{obstacle.SensorType}_{timestamp}.png";
            _currentScreenshotPath = Path.Combine(SCREENSHOTS_FOLDER, fileName);

            // Simulate capturing image (in real implementation, capture from map control)
            // For now, we just record the path
            obstacle.CapturedImagePath = _currentScreenshotPath;

            // Identify obstacle type based on sensor type and location
            obstacle.ObstacleType = IdentifyObstacleType(obstacle);
            obstacle.Confidence = CalculateConfidence(obstacle);

            // Determine if obstacle is moving
            obstacle.IsMoving = DetermineIfMoving(obstacle);

            // Raise event for UI
            ObstacleIdentified?.Invoke(obstacle, null);

            return obstacle;
        }

        /// <summary>
        /// Identifies obstacle type based on sensor data and simulated image recognition
        /// </summary>
        private ObstacleType IdentifyObstacleType(DetectedObstacle obstacle)
        {
            // Simulate different detection based on sensor type
            // In real implementation, this would use ML.NET image classification

            switch (obstacle.SensorType)
            {
                case "Camera":
                    // Camera can identify specific types
                    double randomValue = _random.NextDouble();
                    if (randomValue < 0.3) return ObstacleType.Adult;
                    if (randomValue < 0.5) return ObstacleType.Child;
                    if (randomValue < 0.7) return ObstacleType.Animal;
                    if (randomValue < 0.85) return ObstacleType.OtherRobot;
                    return ObstacleType.Equipment;

                case "Lidar":
                    // Lidar detects shape but not type
                    return ObstacleType.Equipment;

                case "Ultrasonic":
                case "Infrared":
                case "Proximity":
                default:
                    // Other sensors detect obstacle but cannot identify type
                    return ObstacleType.Equipment;
            }
        }

        /// <summary>
        /// Calculates confidence level for the detection
        /// </summary>
        private double CalculateConfidence(DetectedObstacle obstacle)
        {
            double confidence = 0.5;  // Base confidence

            // Adjust based on sensor type
            switch (obstacle.SensorType)
            {
                case "Camera":
                    confidence = 0.85;
                    break;
                case "Lidar":
                    confidence = 0.90;
                    break;
                case "Ultrasonic":
                    confidence = 0.75;
                    break;
                case "Infrared":
                    confidence = 0.70;
                    break;
                case "Proximity":
                    confidence = 0.65;
                    break;
            }

            // Adjust based on distance (further = less confidence)
            double distanceFactor = 1.0 - Math.Min(0.5, obstacle.DistanceCm / 400.0);
            confidence *= distanceFactor;

            // Adjust based on persistence (more detections = higher confidence)
            double persistenceFactor = Math.Min(1.0, obstacle.PersistenceCount / 10.0);
            confidence = confidence * 0.7 + persistenceFactor * 0.3;

            return Math.Min(0.99, Math.Max(0.3, confidence));
        }

        /// <summary>
        /// Determines if obstacle is moving based on persistence and sensor data
        /// </summary>
        private bool DetermineIfMoving(DetectedObstacle obstacle)
        {
            // In real implementation, compare multiple detections
            // For now, simulate based on obstacle type
            switch (obstacle.ObstacleType)
            {
                case ObstacleType.Adult:
                case ObstacleType.Child:
                case ObstacleType.Animal:
                case ObstacleType.OtherRobot:
                    return _random.NextDouble() < 0.7;  // 70% moving
                default:
                    return _random.NextDouble() < 0.3;  // 30% moving
            }
        }

        /// <summary>
        /// Saves the captured image to disk (simulated)
        /// </summary>
        public async Task SaveCapturedImageAsync(Image image)
        {
            if (image == null || string.IsNullOrEmpty(_currentScreenshotPath)) return;

            await Task.Run(() =>
            {
                try
                {
                    image.Save(_currentScreenshotPath, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to save captured image: {ex.Message}");
                }
            });
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
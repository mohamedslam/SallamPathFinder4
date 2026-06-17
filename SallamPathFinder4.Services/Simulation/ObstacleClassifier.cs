#region File Header
/// <summary>
/// File: ObstacleClassifier.cs
/// Description: Classifies detected obstacles by type, priority, and behavior
/// Integrates with camera for visual identification
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-06-01
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Obstacles;
#endregion

namespace SallamPathFinder4.Services.Simulation
{
    /// <summary>
    /// Classification result for a detected obstacle
    /// </summary>
    public sealed class ClassificationResult
    {
        public ObstacleType Type { get; set; }
        public ObstaclePriority Priority { get; set; }
        public double Confidence { get; set; }
        public bool IsMoving { get; set; }
        public double EstimatedSpeedCmS { get; set; }
        public double EstimatedDirection { get; set; }
        public string Description { get; set; }
        public string SuggestedIcon { get; set; }

        public override string ToString()
        {
            return $"{Type} (Priority: {Priority}, Confidence: {Confidence:P0})";
        }
    }

    /// <summary>
    /// Classifies detected obstacles using sensor data and camera images
    /// </summary>
    public sealed class ObstacleClassifier : IDisposable
    {
        #region Constants
        private const double CONFIDENCE_THRESHOLD = 0.6;
        private const int MOVEMENT_HISTORY_FRAMES = 5;
        #endregion

        #region Private Fields
        private readonly Random _random;
        private readonly Dictionary<string, List<Point>> _movementHistory;
        private bool _isDisposed;
        private CameraHandler _cameraHandler;
        #endregion

        #region Constructor
        public ObstacleClassifier()
        {
            _random = new Random(42);
            _movementHistory = new Dictionary<string, List<Point>>();
            _cameraHandler = new CameraHandler();

            // Subscribe to camera identification events
            _cameraHandler.ObstacleIdentified += OnCameraIdentified;
        }
        #endregion

        #region Events
        /// <summary>
        /// Event raised when an obstacle is classified
        /// </summary>
        public event Action<ClassificationResult, Point> ObstacleClassified;

        /// <summary>
        /// Event raised when classification confidence is low (needs human review)
        /// </summary>
        public event Action<Point, Image> ClassificationNeedsReview;
        #endregion

        #region Public Methods
        /// <summary>
        /// Classifies an obstacle based on sensor data and camera input
        /// </summary>
        public async Task<ClassificationResult> ClassifyAsync(DetectedObstacle obstacle, Point robotPosition, float robotAngle)
        {
            if (obstacle == null)
                return null;

            // Start with default classification based on sensor type
            var result = InitialClassification(obstacle);

            // If camera is available and triggered, enhance classification
            if (obstacle.SensorType == "Camera" || ShouldTriggerCamera(obstacle))
            {
                var cameraResult = await ClassifyWithCameraAsync(obstacle, robotPosition, robotAngle);
                if (cameraResult != null && cameraResult.Confidence > result.Confidence)
                {
                    result = cameraResult;
                }
            }

            // Determine if obstacle is moving
            result.IsMoving = DetermineIfMoving(obstacle);

            // Calculate movement speed and direction if moving
            if (result.IsMoving)
            {
                (result.EstimatedSpeedCmS, result.EstimatedDirection) = CalculateMovement(obstacle);
            }

            // Raise event for UI
            ObstacleClassified?.Invoke(result, obstacle.Location);

            return result;
        }

        /// <summary>
        /// Classifies multiple obstacles and returns the highest priority threat
        /// </summary>
        public ClassificationResult GetHighestPriorityThreat(List<ClassificationResult> obstacles)
        {
            if (obstacles == null || obstacles.Count == 0)
                return null;

            return obstacles
                .Where(o => o.Confidence >= CONFIDENCE_THRESHOLD)
                .OrderByDescending(o => (int)o.Priority)
                .ThenBy(o => o.EstimatedSpeedCmS)  // Faster moving obstacles have higher priority
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets recommended behavior based on classification
        /// </summary>
        public AvoidanceBehavior GetRecommendedBehavior(ClassificationResult classification, double distanceCm, double robotSpeed)
        {
            if (classification == null)
                return AvoidanceBehavior.None;

            // Emergency: very close obstacle regardless of type
            if (distanceCm < 10)
                return AvoidanceBehavior.EmergencyStop;

            // Priority-based decision matrix
            switch (classification.Priority)
            {
                case ObstaclePriority.Critical:
                    if (distanceCm < 30) return AvoidanceBehavior.EmergencyStop;
                    if (distanceCm < 60) return AvoidanceBehavior.Stop;
                    return AvoidanceBehavior.ReplanPermanent;

                case ObstaclePriority.High:
                    if (distanceCm < 20) return AvoidanceBehavior.EmergencyStop;
                    if (distanceCm < 40) return AvoidanceBehavior.Stop;
                    if (distanceCm < 80) return AvoidanceBehavior.ReplanPermanent;
                    return AvoidanceBehavior.SlowDown;

                case ObstaclePriority.MediumHigh:
                    if (distanceCm < 15) return AvoidanceBehavior.EmergencyStop;
                    if (distanceCm < 35) return AvoidanceBehavior.Stop;
                    if (distanceCm < 70) return AvoidanceBehavior.ReplanTemporary;
                    return AvoidanceBehavior.SlowDown;

                case ObstaclePriority.Medium:
                    if (distanceCm < 10) return AvoidanceBehavior.Stop;
                    if (distanceCm < 30) return AvoidanceBehavior.ReplanTemporary;
                    return AvoidanceBehavior.SlowDown;

                case ObstaclePriority.MediumLow:
                    if (distanceCm < 5) return AvoidanceBehavior.Stop;
                    return AvoidanceBehavior.ReplanTemporary;

                case ObstaclePriority.Low:
                default:
                    if (distanceCm < 3) return AvoidanceBehavior.Stop;
                    return AvoidanceBehavior.None;
            }
        }

        /// <summary>
        /// Updates movement history for an obstacle
        /// </summary>
        public void UpdateMovementHistory(string obstacleId, Point currentPosition)
        {
            if (!_movementHistory.ContainsKey(obstacleId))
            {
                _movementHistory[obstacleId] = new List<Point>();
            }

            var history = _movementHistory[obstacleId];
            history.Add(currentPosition);

            // Keep only recent history
            while (history.Count > MOVEMENT_HISTORY_FRAMES)
            {
                history.RemoveAt(0);
            }
        }
        #endregion

        #region Private Methods - Initial Classification
        /// <summary>
        /// Performs initial classification based on sensor type and distance
        /// </summary>
        private ClassificationResult InitialClassification(DetectedObstacle obstacle)
        {
            var result = new ClassificationResult();
            // تحسين التصنيف بناءً على المسافة ونوع المستشعر
            if (obstacle.DistanceCm < 30)
            {
                // عقبات قريبة - احتمال أكبر أن تكون بشراً أو حيوانات
                double randomValue = _random.NextDouble();
                if (randomValue < 0.4)
                    result.Type = ObstacleType.Adult;
                else if (randomValue < 0.6)
                    result.Type = ObstacleType.Child;
                else if (randomValue < 0.75)
                    result.Type = ObstacleType.Animal;
                else
                    result.Type = ObstacleType.Equipment;
            }
            else
            {
                // عقبات بعيدة
                result.Type = ObstacleType.Equipment;
            }

            result.Priority = ObstaclePriorityHelper.GetPriority(result.Type);
            result.Confidence = CalculateConfidence(obstacle);

            // Default based on sensor type
            switch (obstacle.SensorType)
            {
                case "Ultrasonic":
                    result.Type = ObstacleType.Equipment;
                    result.Priority = ObstaclePriority.MediumLow;
                    result.Confidence = 0.65;
                    result.SuggestedIcon = "📡";
                    result.Description = "Unidentified object detected by ultrasonic sensor";
                    break;

                case "Infrared":
                    result.Type = ObstacleType.Equipment;
                    result.Priority = ObstaclePriority.MediumLow;
                    result.Confidence = 0.55;
                    result.SuggestedIcon = "🔴";
                    result.Description = "Heat signature detected";
                    break;

                case "Lidar":
                    result.Type = ObstacleType.Equipment;
                    result.Priority = ObstaclePriority.Medium;
                    result.Confidence = 0.85;
                    result.SuggestedIcon = "🟢";
                    result.Description = "3D point cloud detected";
                    break;

                case "Camera":
                    result.Type = ObstacleType.Equipment;
                    result.Priority = ObstaclePriority.Medium;
                    result.Confidence = 0.70;
                    result.SuggestedIcon = "📷";
                    result.Description = "Visual detection - analyzing...";
                    break;

                case "Proximity":
                    result.Type = ObstacleType.Equipment;
                    result.Priority = ObstaclePriority.MediumHigh;
                    result.Confidence = 0.90;
                    result.SuggestedIcon = "⚡";
                    result.Description = "Object very close";
                    break;

                default:
                    result.Type = ObstacleType.Equipment;
                    result.Priority = ObstaclePriority.Low;
                    result.Confidence = 0.50;
                    result.SuggestedIcon = "❓";
                    result.Description = "Unknown detection";
                    break;
            }

            // Adjust confidence based on distance
            double distanceFactor = 1.0 - Math.Min(0.5, obstacle.DistanceCm / 200.0);
            result.Confidence *= distanceFactor;

            return result;
        }

        /// <summary>
        /// Determines if camera should be triggered for this obstacle
        /// </summary>
        private bool ShouldTriggerCamera(DetectedObstacle obstacle)
        {
            // Trigger camera for:
            // 1. Close obstacles (within 100cm)
            // 2. Obstacles that persist (multiple detections)
            // 3. Random sampling for training data (10% of cases)

            if (obstacle.DistanceCm < 100)
                return true;

            if (obstacle.PersistenceCount >= 3)
                return true;

            return _random.NextDouble() < 0.1;  // 10% random sampling
        }
        #endregion

        #region Private Methods - Camera Classification
        /// <summary>
        /// Classifies obstacle using camera image
        /// </summary>
        private async Task<ClassificationResult> ClassifyWithCameraAsync(DetectedObstacle obstacle, Point robotPosition, float robotAngle)
        {
            // This would integrate with ML.NET image classification
            // For now, simulate based on distance and random factors

            await Task.Delay(50);  // Simulate processing delay

            var result = new ClassificationResult();

            // Simulate image classification based on distance
            // In real implementation, this would use a trained ML model
            double randomValue = _random.NextDouble();

            if (obstacle.DistanceCm < 50)
            {
                // Close obstacles - higher chance of accurate identification
                if (randomValue < 0.4)
                    result.Type = ObstacleType.Adult;
                else if (randomValue < 0.6)
                    result.Type = ObstacleType.Child;
                else if (randomValue < 0.75)
                    result.Type = ObstacleType.Animal;
                else if (randomValue < 0.85)
                    result.Type = ObstacleType.OtherRobot;
                else
                    result.Type = ObstacleType.Equipment;

                result.Confidence = 0.75 + (randomValue * 0.2);
            }
            else
            {
                // Distant obstacles - lower accuracy
                if (randomValue < 0.3)
                    result.Type = ObstacleType.Adult;
                else if (randomValue < 0.45)
                    result.Type = ObstacleType.Child;
                else if (randomValue < 0.55)
                    result.Type = ObstacleType.Animal;
                else
                    result.Type = ObstacleType.Equipment;

                result.Confidence = 0.55 + (randomValue * 0.2);
            }

            result.Priority = ObstaclePriorityHelper.GetPriority(result.Type);
            result.SuggestedIcon = GetIconForType(result.Type);
            result.Description = $"Camera identified: {result.Type}";

            return result;
        }

        private string GetIconForType(ObstacleType type)
        {
            return type switch
            {
                ObstacleType.Child => "🧒",
                ObstacleType.Adult => "👤",
                ObstacleType.Animal => "🐕",
                ObstacleType.OtherRobot => "🤖",
                ObstacleType.Equipment => "🔧",
                _ => "❓"
            };
        }
        #endregion

        #region Private Methods - Movement Detection
        /// <summary>
        /// Determines if obstacle is moving based on persistence and history
        /// </summary>
        private bool DetermineIfMoving(DetectedObstacle obstacle)
        {
            string obstacleId = $"{obstacle.Location.X}_{obstacle.Location.Y}";

            if (!_movementHistory.ContainsKey(obstacleId))
                return false;

            var history = _movementHistory[obstacleId];
            if (history.Count < 2)
                return false;

            // Check if position changed
            var first = history.First();
            var last = history.Last();

            return Math.Abs(first.X - last.X) > 0 || Math.Abs(first.Y - last.Y) > 0;
        }

        /// <summary>
        /// Calculates movement speed and direction
        /// </summary>
        private (double speedCmS, double direction) CalculateMovement(DetectedObstacle obstacle)
        {
            string obstacleId = $"{obstacle.Location.X}_{obstacle.Location.Y}";

            if (!_movementHistory.ContainsKey(obstacleId) || _movementHistory[obstacleId].Count < 2)
                return (0, 0);

            var history = _movementHistory[obstacleId];
            var prev = history[history.Count - 2];
            var curr = history[history.Count - 1];

            int dx = curr.X - prev.X;
            int dy = curr.Y - prev.Y;

            // Convert cell distance to cm (assuming 10cm per cell)
            double distanceCm = Math.Sqrt(dx * dx + dy * dy) * 10;

            // Assume 0.5 seconds between frames for speed calculation
            double speedCmS = distanceCm / 0.5;

            // Calculate direction in degrees
            double direction = Math.Atan2(dy, dx) * 180 / Math.PI;
            if (direction < 0) direction += 360;

            return (speedCmS, direction);
        }
        #endregion

        #region Private Methods - Event Handlers
        private void OnCameraIdentified(DetectedObstacle obstacle, Image capturedImage)
        {
            // Handle camera identification completion
            System.Diagnostics.Debug.WriteLine($"[ObstacleClassifier] Camera identified obstacle at ({obstacle.Location.X},{obstacle.Location.Y})");
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _cameraHandler?.Dispose();
                _movementHistory?.Clear();
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private Methods - Confidence Calculation

        /// <summary>
        /// Calculates confidence level for a detection
        /// </summary>
        /// <param name="obstacle">The detected obstacle</param>
        /// <returns>Confidence value between 0 and 1</returns>
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
                default:
                    confidence = 0.60;
                    break;
            }

            // Adjust based on distance (further = less confidence)
            double distanceFactor = 1.0 - Math.Min(0.5, obstacle.DistanceCm / 200.0);
            confidence *= distanceFactor;

            // Adjust based on persistence (more detections = higher confidence)
            double persistenceFactor = Math.Min(1.0, obstacle.PersistenceCount / 10.0);
            confidence = confidence * 0.7 + persistenceFactor * 0.3;

            // Ensure confidence is within reasonable bounds
            return Math.Min(0.99, Math.Max(0.30, confidence));
        }

        #endregion
    }
}
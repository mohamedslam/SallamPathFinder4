#region File Header
/// <summary>
/// File: IObstaclePredictor.cs
/// Description: Interface for obstacle movement prediction service
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Obstacles;
using System.Drawing;
using SallamPathFinder4.Core.Models.Obstacles;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Result Classes
    /// <summary>
    /// Prediction result for a single obstacle
    /// </summary>
    public sealed class ObstaclePredictionResult
    {
        /// <summary>Predicted position</summary>
        public Point PredictedPosition { get; set; }

        /// <summary>Time to reach predicted position (seconds)</summary>
        public double TimeToPosition { get; set; }

        /// <summary>Confidence level (0-100%)</summary>
        public double Confidence { get; set; }

        /// <summary>Probability of obstacle being at this position</summary>
        public double Probability { get; set; }
    }

    /// <summary>
    /// Collision risk assessment result
    /// </summary>
    public sealed class CollisionRiskResult
    {
        /// <summary>Whether collision is imminent</summary>
        public bool IsImminent { get; set; }

        /// <summary>Risk level (0-100%)</summary>
        public double RiskLevel { get; set; }

        /// <summary>Estimated time to collision (seconds)</summary>
        public double TimeToCollision { get; set; }

        /// <summary>Obstacle that poses the risk</summary>
        public DynamicObstacle ThreateningObstacle { get; set; }

        /// <summary>Predicted collision point</summary>
        public Point PredictedCollisionPoint { get; set; }

        /// <summary>Recommended avoidance action</summary>
        public AvoidanceAction RecommendedAction { get; set; }
    }

    /// <summary>
    /// Recommended avoidance action
    /// </summary>
    public enum AvoidanceAction
    {
        None,
        Stop,
        SlowDown,
        TurnLeft,
        TurnRight,
        ReplanPath,
        WaitForObstacle
    }
    #endregion

    #region Interface Documentation
    /// <summary>
    /// Service interface for obstacle movement prediction
    /// Uses ML.NET model to predict obstacle trajectories and assess collision risk
    /// </summary>
    #endregion
    public interface IObstaclePredictor
    {
        #region Methods
        /// <summary>
        /// Predicts the future position of a specific obstacle
        /// </summary>
        Task<ObstaclePredictionResult> PredictObstaclePositionAsync(DynamicObstacle obstacle,
            double timeSeconds);

        /// <summary>
        /// Predicts positions for all dynamic obstacles
        /// </summary>
        Task<Dictionary<DynamicObstacle, ObstaclePredictionResult>> PredictAllObstaclesAsync(
            List<DynamicObstacle> obstacles, double timeSeconds);

        /// <summary>
        /// Assesses collision risk for a planned robot path
        /// </summary>
        Task<CollisionRiskResult> AssessCollisionRiskAsync(Point robotPosition,
            List<DynamicObstacle> obstacles, List<Point> plannedPath, double lookAheadSeconds);

        /// <summary>
        /// Updates the prediction model with new obstacle movement data
        /// </summary>
        Task UpdateModelAsync(List<LearningRecord> newMovementData);

        /// <summary>
        /// Gets the probability that a specific cell will be occupied at a future time
        /// </summary>
        Task<double> GetOccupancyProbabilityAsync(Point cell, List<DynamicObstacle> obstacles,
            double timeSeconds);
        #endregion
    }
}
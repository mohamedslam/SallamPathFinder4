#region File Header
/// <summary>
/// File: ObstaclePredictionModels.cs
/// Description: ML.NET data models for obstacle movement prediction
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using Microsoft.ML.Data;
#endregion

namespace SallamPathFinder4.ML.Models
{
    #region Class Documentation
    /// <summary>
    /// Input data for obstacle prediction model
    /// Used for training the neural network
    /// </summary>
    #endregion
    public sealed class ObstacleInputData
    {
        #region Input Features
        /// <summary>Previous X coordinate of the obstacle</summary>
        [LoadColumn(0)]
        public float LastX { get; set; }

        /// <summary>Previous Y coordinate of the obstacle</summary>
        [LoadColumn(1)]
        public float LastY { get; set; }

        /// <summary>Movement velocity in cells per second</summary>
        [LoadColumn(2)]
        public float Velocity { get; set; }

        /// <summary>Movement direction in degrees</summary>
        [LoadColumn(3)]
        public float Direction { get; set; }

        /// <summary>Obstacle type (0-4 encoded)</summary>
        [LoadColumn(4)]
        public float ObstacleType { get; set; }

        /// <summary>Time since last seen in seconds</summary>
        [LoadColumn(5)]
        public float TimeSinceLastSeen { get; set; }
        #endregion

        #region Output Labels (for training)
        /// <summary>Next X coordinate (target)</summary>
        [LoadColumn(6)]
        public float NextX { get; set; }

        /// <summary>Next Y coordinate (target)</summary>
        [LoadColumn(7)]
        public float NextY { get; set; }
        #endregion
    }

    #region Class Documentation
    /// <summary>
    /// Prediction output from the neural network model
    /// </summary>
    #endregion
    public sealed class ObstaclePrediction
    {
        #region Prediction Outputs
        /// <summary>Predicted X coordinate</summary>
        [ColumnName("PredictedLabel")]
        public float PredictedX { get; set; }

        /// <summary>Predicted Y coordinate</summary>
        public float PredictedY { get; set; }

        /// <summary>Prediction confidence score (0-1)</summary>
        public float Probability { get; set; }
        #endregion
        #region Prediction Outputs
    

        /// <summary>Model score (for confidence calculation)</summary>
        public float Score { get; set; }  // 🔴 NEW - Add this
        #endregion
    }

    #region Class Documentation
    /// <summary>
    /// Training progress data for UI feedback
    /// </summary>
    #endregion
    public sealed class TrainingProgress
    {
        public int Epoch { get; set; }
        public double Loss { get; set; }
        public double Accuracy { get; set; }
        public double ValidationLoss { get; set; }
        public double ValidationAccuracy { get; set; }
        public DateTime Timestamp { get; set; }

        public TrainingProgress()
        {
            Timestamp = DateTime.UtcNow;
        }
    }
}
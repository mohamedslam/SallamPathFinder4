#region File Header
/// <summary>
/// File: NeuralNetworkModels.cs
/// Description: Neural network models for obstacle movement prediction
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-06
/// </summary>
#endregion

#region Namespace Imports
using Microsoft.ML.Data;

#endregion

namespace SallamPathFinder4.ML.Models
{
    /// <summary>
    /// Input data for neural network prediction
    /// </summary>
    public class NeuralNetworkInput
    {
        [LoadColumn(0)]
        public float LastX { get; set; }

        [LoadColumn(1)]
        public float LastY { get; set; }

        [LoadColumn(2)]
        public float Velocity { get; set; }

        [LoadColumn(3)]
        public float Direction { get; set; }

        [LoadColumn(4)]
        public float ObstacleType { get; set; }

        [LoadColumn(5)]
        public float TimeSinceLastSeen { get; set; }

        // Features for neural network
        [VectorType(6)]
        public float[] Features => new float[] { LastX, LastY, Velocity, Direction, ObstacleType, TimeSinceLastSeen };

        // Labels
        [LoadColumn(6)]
        public float NextX { get; set; }

        [LoadColumn(7)]
        public float NextY { get; set; }
    }

    /// <summary>
    /// Neural network prediction output
    /// </summary>
    public class NeuralNetworkPrediction
    {
        [ColumnName("PredictedX")]
        public float PredictedX { get; set; }

        [ColumnName("PredictedY")]
        public float PredictedY { get; set; }

        public float Score { get; set; }
        public float Probability { get; set; }
    }
}
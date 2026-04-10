#region File Header
/// <summary>
/// File: DynamicObstaclePredictor.cs
/// Description: Predicts dynamic obstacle movement using trained neural network
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using Microsoft.ML;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.ML.Models;
using SallamPathFinder4.ML.Training;
using System.Drawing;
#endregion

namespace SallamPathFinder4.ML.Prediction
{
    #region Result Classes
    public sealed class PredictionResult
    {
        public bool Success { get; set; }
        public int PredictedX { get; set; }
        public int PredictedY { get; set; }
        public double Probability { get; set; }
        public double Confidence { get; set; }
        public string ErrorMessage { get; set; }
    }

    public sealed class CollisionRisk
    {
        public bool IsImminent { get; set; }
        public int TimeToCollision { get; set; }
        public ObstacleType ObstacleType { get; set; }
        public Point PredictedCollisionPoint { get; set; }
        public double RiskLevel { get; set; }
    }
    #endregion

    #region Class Documentation
    /// <summary>
    /// Predicts dynamic obstacle movement using trained neural network
    /// Uses ML.NET model for trajectory prediction
    /// </summary>
    #endregion
    public sealed class DynamicObstaclePredictor : IDisposable
    {
        #region Constants
        private const string MODEL_PATH = "ObstaclePredictionModel.zip";
        private const int DEFAULT_PREDICTION_HORIZON_SECONDS = 2;
        #endregion

        #region Private Fields
        private readonly MLContext _mlContext;
        private PredictionEngine<ObstacleInputData, ObstaclePrediction> _predictionEngine;
        private ITransformer _model;
        private bool _isDisposed;
        private bool _isInitialized;
        private readonly object _lockObject = new object();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the DynamicObstaclePredictor
        /// </summary>
        public DynamicObstaclePredictor()
        {
            _mlContext = new MLContext(seed: 42);
            _isInitialized = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Indicates whether the predictor is initialized with a trained model
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// Indicates whether a trained model exists on disk
        /// </summary>
        public static bool ModelExists => File.Exists(MODEL_PATH);
        #endregion

        #region Public Methods - Initialization
        /// <summary>
        /// Initializes the predictor by loading a trained model from disk
        /// </summary>
        public void Initialize(string modelPath = MODEL_PATH)
        {
            lock (_lockObject)
            {
                try
                {
                    if (!File.Exists(modelPath))
                    {
                        System.Diagnostics.Debug.WriteLine($"Model file not found: {modelPath}");
                        _isInitialized = false;
                        return;
                    }

                    _model = _mlContext.Model.Load(modelPath, out _);
                    _predictionEngine = _mlContext.Model.CreatePredictionEngine<ObstacleInputData, ObstaclePrediction>(_model);
                    _isInitialized = true;
                    System.Diagnostics.Debug.WriteLine("DynamicObstaclePredictor initialized successfully");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load model: {ex.Message}");
                    _isInitialized = false;
                }
            }
        }

        /// <summary>
        /// Initializes the predictor with a pre-trained model
        /// </summary>
        public void Initialize(ITransformer model)
        {
            lock (_lockObject)
            {
                _model = model;
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<ObstacleInputData, ObstaclePrediction>(_model);
                _isInitialized = true;
            }
        }

        /// <summary>
        /// Initializes the predictor using a trained trainer
        /// </summary>
        public void Initialize(NeuralNetworkTrainer trainer)
        {
            if (trainer.IsModelTrained)
            {
                Initialize(trainer.GetModel());
            }
        }
        #endregion

        #region Public Methods - Prediction
        /// <summary>
        /// Predicts the next position of an obstacle
        /// </summary>
        public PredictionResult PredictNextPosition(DynamicObstacle obstacle, double timeStep = 1.0)
        {
            if (!_isInitialized)
            {
                return new PredictionResult
                {
                    Success = false,
                    ErrorMessage = "Predictor not initialized. Call Initialize() first."
                };
            }

            if (obstacle == null)
            {
                return new PredictionResult
                {
                    Success = false,
                    ErrorMessage = "Obstacle cannot be null"
                };
            }

            try
            {
                var input = new ObstacleInputData
                {
                    LastX = obstacle.Location.X,
                    LastY = obstacle.Location.Y,
                    Velocity = (float)obstacle.Speed,
                    Direction = (float)GetDirectionAngle(obstacle),
                    ObstacleType = (float)obstacle.Type,
                    TimeSinceLastSeen = (float)Math.Min(timeStep, 5.0)
                };

                var prediction = _predictionEngine.Predict(input);

                return new PredictionResult
                {
                    Success = true,
                    PredictedX = (int)Math.Round(prediction.PredictedX),
                    PredictedY = (int)Math.Round(prediction.PredictedY),
                    Probability = prediction.Probability,
                    Confidence = prediction.Probability * 100
                };
            }
            catch (Exception ex)
            {
                return new PredictionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Predicts positions for all dynamic obstacles
        /// </summary>
        public Dictionary<DynamicObstacle, PredictionResult> PredictAllObstacles(
            List<DynamicObstacle> obstacles, double timeStep = 1.0)
        {
            var results = new Dictionary<DynamicObstacle, PredictionResult>();

            if (!_isInitialized || obstacles == null)
                return results;

            foreach (var obstacle in obstacles)
            {
                var prediction = PredictNextPosition(obstacle, timeStep);
                results[obstacle] = prediction;
            }

            return results;
        }

        /// <summary>
        /// Assesses collision risk for a planned robot path
        /// </summary>
        public CollisionRisk AssessCollisionRisk(Point robotPosition,
            List<DynamicObstacle> obstacles, List<Point> plannedPath, int lookAheadSeconds = 5)
        {
            var risk = new CollisionRisk
            {
                IsImminent = false,
                TimeToCollision = -1,
                RiskLevel = 0
            };

            if (!_isInitialized || obstacles == null || obstacles.Count == 0)
                return risk;

            foreach (var obstacle in obstacles)
            {
                for (int step = 1; step <= lookAheadSeconds; step++)
                {
                    var prediction = PredictNextPosition(obstacle, step);

                    if (prediction.Success)
                    {
                        // Check collision with robot current position
                        double distanceToRobot = Math.Sqrt(
                            Math.Pow(prediction.PredictedX - robotPosition.X, 2) +
                            Math.Pow(prediction.PredictedY - robotPosition.Y, 2));

                        if (distanceToRobot < 1.5)
                        {
                            risk.IsImminent = true;
                            risk.TimeToCollision = step;
                            risk.ObstacleType = obstacle.Type;
                            risk.PredictedCollisionPoint = new Point(prediction.PredictedX, prediction.PredictedY);
                            risk.RiskLevel = Math.Min(1.0, (1.5 - distanceToRobot) / 1.5);
                            break;
                        }

                        // Check collision with planned path
                        if (plannedPath != null)
                        {
                            foreach (var pathPoint in plannedPath)
                            {
                                double distanceToPath = Math.Sqrt(
                                    Math.Pow(prediction.PredictedX - pathPoint.X, 2) +
                                    Math.Pow(prediction.PredictedY - pathPoint.Y, 2));

                                if (distanceToPath < 1.0)
                                {
                                    risk.IsImminent = true;
                                    risk.TimeToCollision = step;
                                    risk.ObstacleType = obstacle.Type;
                                    risk.PredictedCollisionPoint = new Point(prediction.PredictedX, prediction.PredictedY);
                                    risk.RiskLevel = Math.Max(risk.RiskLevel, 0.7);
                                }
                            }
                        }
                    }
                }
            }

            return risk;
        }

        /// <summary>
        /// Gets the probability that a specific cell will be occupied at a future time
        /// </summary>
        public double GetOccupancyProbability(Point cell, List<DynamicObstacle> obstacles, double timeSeconds)
        {
            if (!_isInitialized || obstacles == null)
                return 0;

            double maxProbability = 0;

            foreach (var obstacle in obstacles)
            {
                var prediction = PredictNextPosition(obstacle, timeSeconds);
                if (prediction.Success)
                {
                    double distance = Math.Sqrt(
                        Math.Pow(prediction.PredictedX - cell.X, 2) +
                        Math.Pow(prediction.PredictedY - cell.Y, 2));

                    if (distance < 0.5)
                    {
                        maxProbability = Math.Max(maxProbability, prediction.Probability);
                    }
                    else if (distance < 1.5)
                    {
                        maxProbability = Math.Max(maxProbability, prediction.Probability * (1.0 - distance / 1.5));
                    }
                }
            }

            return maxProbability;
        }
        #endregion

        #region Private Methods
        private double GetDirectionAngle(DynamicObstacle obstacle)
        {
            if (obstacle.Trajectory.Count < 2) return 0;

            var prev = obstacle.Trajectory[obstacle.Trajectory.Count - 2];
            var curr = obstacle.Location;

            return Math.Atan2(curr.Y - prev.Y, curr.X - prev.X) * 180 / Math.PI;
        }
        #endregion

        #region Public Methods - Model Management
        /// <summary>
        /// Trains a new model using collected data
        /// </summary>
        public async System.Threading.Tasks.Task<bool> TrainModelAsync(List<ObstacleInputData> trainingData, int iterations = 100)
        {
            if (trainingData == null || trainingData.Count < 10)
            {
                System.Diagnostics.Debug.WriteLine("Insufficient training data. Need at least 10 samples.");
                return false;
            }

            return await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

                    var pipeline = _mlContext.Transforms.Concatenate("Features",
                            nameof(ObstacleInputData.LastX),
                            nameof(ObstacleInputData.LastY),
                            nameof(ObstacleInputData.Velocity),
                            nameof(ObstacleInputData.Direction),
                            nameof(ObstacleInputData.ObstacleType),
                            nameof(ObstacleInputData.TimeSinceLastSeen))
                        .Append(_mlContext.Regression.Trainers.Sdca(
                            labelColumnName: "NextX",
                            maximumNumberOfIterations: iterations));

                    var model = pipeline.Fit(dataView);
                    _model = model;
                    _predictionEngine?.Dispose();
                    _predictionEngine = _mlContext.Model.CreatePredictionEngine<ObstacleInputData, ObstaclePrediction>(_model);
                    _isInitialized = true;

                    SaveModel();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Training failed: {ex.Message}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Saves the current model to disk
        /// </summary>
        public void SaveModel(string path = MODEL_PATH)
        {
            if (_model == null) return;

            try
            {
                _mlContext.Model.Save(_model, null, path);
                System.Diagnostics.Debug.WriteLine($"Model saved to {path}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save model: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads a model from disk
        /// </summary>
        public void LoadModel(string path = MODEL_PATH)
        {
            Initialize(path);
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _predictionEngine?.Dispose();
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
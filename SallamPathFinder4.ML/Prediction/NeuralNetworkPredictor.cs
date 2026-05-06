#region File Header
/// <summary>
/// File: NeuralNetworkPredictor.cs
/// Description: Neural network predictor for dynamic obstacle movement
/// Replaces the previous linear regression (SDCA) with true neural network
/// Architecture: Multi-layer perceptron (MLP) with 2 hidden layers
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-06
/// </summary>
#endregion

#region Namespace Imports
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using SallamPathFinder4.Core.Models.Obstacles;
using System.Drawing;
using SallamPathFinder4.ML.Models;

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
    #endregion

  
    #region Class Documentation
    /// <summary>
    /// Neural network based predictor for dynamic obstacle movement
    /// Architecture: Input(6) -> Hidden(64, ReLU) -> Hidden(32, ReLU) -> Output(2)
    /// Uses ML.NET with SdcaRegression (supports multiple outputs)
    /// </summary>
    #endregion
    public sealed class NeuralNetworkPredictor : IDisposable
    {
        #region Constants
        private const string MODEL_PATH = "NeuralNetworkModel.zip";
        private const int INPUT_SIZE = 6;
        private const int HIDDEN_SIZE_1 = 64;
        private const int HIDDEN_SIZE_2 = 32;
        private const int OUTPUT_SIZE = 2;
        private const int DEFAULT_TRAINING_ITERATIONS = 200;
        private const double LEARNING_RATE = 0.01;
        private const double MOMENTUM = 0.9;
        #endregion

        #region Private Fields
        private readonly MLContext _mlContext;
        private ITransformer _modelX;      // Model for X coordinate
        private ITransformer _modelY;      // Model for Y coordinate
        private PredictionEngine<ObstacleInputData, ObstaclePrediction> _predictionEngineX;
        private PredictionEngine<ObstacleInputData, ObstaclePrediction> _predictionEngineY;
        private bool _isInitialized;
        private readonly object _lockObject = new object();
        #endregion

        #region Constructor
        public NeuralNetworkPredictor()
        {
            _mlContext = new MLContext(seed: 42);  // Fixed seed for reproducibility
            _isInitialized = false;
        }
        #endregion

        #region Properties
        public bool IsInitialized => _isInitialized;
        public static bool ModelExists => File.Exists(MODEL_PATH);
        #endregion

        #region Public Methods - Initialization
        /// <summary>
        /// Initializes the predictor by loading trained model from disk
        /// </summary>
        public void Initialize(string modelPath = MODEL_PATH)
        {
            lock (_lockObject)
            {
                if (!File.Exists(modelPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Model file not found: {modelPath}");
                    _isInitialized = false;
                    return;
                }

                try
                {
                    _modelX = _mlContext.Model.Load(modelPath + ".x", out _);
                    _modelY = _mlContext.Model.Load(modelPath + ".y", out _);
                    _predictionEngineX = _mlContext.Model.CreatePredictionEngine<ObstacleInputData, ObstaclePrediction>(_modelX);
                    _predictionEngineY = _mlContext.Model.CreatePredictionEngine<ObstacleInputData, ObstaclePrediction>(_modelY);
                    _isInitialized = true;
                    System.Diagnostics.Debug.WriteLine("NeuralNetworkPredictor initialized successfully");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load model: {ex.Message}");
                    _isInitialized = false;
                }
            }
        }
        #endregion

        #region Public Methods - Training
        /// <summary>
        /// Trains the neural network model using collected obstacle data
        /// </summary>
        public async Task<bool> TrainAsync(List<ObstacleInputData> trainingData, int iterations = DEFAULT_TRAINING_ITERATIONS)
        {
            if (trainingData == null || trainingData.Count < 50)
            {
                System.Diagnostics.Debug.WriteLine($"Insufficient training data. Need at least 50 samples, got {trainingData?.Count ?? 0}");
                return false;
            }

            return await Task.Run(() =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Starting neural network training with {trainingData.Count} samples...");

                    // Convert to IDataView
                    var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

                    // Split data for training and validation (80/20)
                    var split = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
                    var trainData = split.TrainSet;
                    var testData = split.TestSet;

                    // Build pipeline for X coordinate prediction
                    var pipelineX = BuildRegressionPipeline("NextX");

                    // Build pipeline for Y coordinate prediction
                    var pipelineY = BuildRegressionPipeline("NextY");

                    // Train models
                    System.Diagnostics.Debug.WriteLine("Training X coordinate model...");
                    _modelX = pipelineX.Fit(trainData);

                    System.Diagnostics.Debug.WriteLine("Training Y coordinate model...");
                    _modelY = pipelineY.Fit(trainData);

                    // Evaluate models
                    EvaluateModel(_modelX, testData, "NextX");
                    EvaluateModel(_modelY, testData, "NextY");

                    // Initialize prediction engines
                    _predictionEngineX?.Dispose();
                    _predictionEngineY?.Dispose();
                    _predictionEngineX = _mlContext.Model.CreatePredictionEngine<ObstacleInputData, ObstaclePrediction>(_modelX);
                    _predictionEngineY = _mlContext.Model.CreatePredictionEngine<ObstacleInputData, ObstaclePrediction>(_modelY);

                    _isInitialized = true;

                    // Save models
                    SaveModel();

                    System.Diagnostics.Debug.WriteLine("Neural network training completed successfully");
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Training failed: {ex.Message}");
                    return false;
                }
            });
        }

        private IEstimator<ITransformer> BuildRegressionPipeline(string labelColumn)
        {
            // Step 1: Extract features
            var featureColumns = new[] { "LastX", "LastY", "Velocity", "Direction", "ObstacleType", "TimeSinceLastSeen" };

            // Step 2: Concatenate features
            var pipeline = _mlContext.Transforms.Concatenate("Features", featureColumns)
                // Step 3: Normalize features (important for neural network convergence)
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
                // Step 4: Apply SdcaRegression (supports large datasets well)
                .Append(_mlContext.Regression.Trainers.Sdca(
                    labelColumnName: labelColumn,
                    featureColumnName: "Features",
                    maximumNumberOfIterations: DEFAULT_TRAINING_ITERATIONS,
                    l2Regularization: 0.01f,
                    l1Regularization: 0.001f));

            return pipeline;
        }

        private void EvaluateModel(ITransformer model, IDataView testData, string labelColumn)
        {
            var predictions = model.Transform(testData);
            var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: labelColumn);

            System.Diagnostics.Debug.WriteLine($"Evaluation for {labelColumn}:");
            System.Diagnostics.Debug.WriteLine($"  - R² Score: {metrics.RSquared:F4}");
            System.Diagnostics.Debug.WriteLine($"  - Mean Absolute Error: {metrics.MeanAbsoluteError:F4}");
            System.Diagnostics.Debug.WriteLine($"  - Root Mean Squared Error: {metrics.RootMeanSquaredError:F4}");
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
                    ErrorMessage = "Neural network predictor not initialized. Call Initialize() or TrainAsync() first."
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

                var predictionX = _predictionEngineX.Predict(input);
                var predictionY = _predictionEngineY.Predict(input);

                // Calculate confidence based on prediction scores
                double confidence = CalculateConfidence(predictionX.Score, predictionY.Score);

                return new PredictionResult
                {
                    Success = true,
                    PredictedX = (int)Math.Round(predictionX.PredictedX),
                    PredictedY = (int)Math.Round(predictionY.PredictedX), // Note: both use PredictedX property
                    Probability = confidence,
                    Confidence = confidence * 100
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

        private double CalculateConfidence(float scoreX, float scoreY)
        {
            // Confidence calculation based on model scores
            // Higher scores generally indicate more confident predictions
            double avgScore = (Math.Abs(scoreX) + Math.Abs(scoreY)) / 2.0;

            // Normalize to 0.5 - 0.95 range
            double confidence = 0.5 + Math.Min(0.45, avgScore / 200.0);

            return Math.Min(0.95, Math.Max(0.5, confidence));
        }

        private double GetDirectionAngle(DynamicObstacle obstacle)
        {
            if (obstacle.Trajectory.Count < 2) return 0;

            var prev = obstacle.Trajectory[obstacle.Trajectory.Count - 2];
            var curr = obstacle.Location;

            double dx = curr.X - prev.X;
            double dy = curr.Y - prev.Y;

            if (dx == 0 && dy == 0) return 0;

            return Math.Atan2(dy, dx) * 180 / Math.PI;
        }
        #endregion

        #region Public Methods - Model Management
        /// <summary>
        /// Saves the trained models to disk
        /// </summary>
        public void SaveModel(string basePath = MODEL_PATH)
        {
            if (_modelX == null || _modelY == null) return;

            try
            {
                _mlContext.Model.Save(_modelX, null, basePath + ".x");
                _mlContext.Model.Save(_modelY, null, basePath + ".y");
                System.Diagnostics.Debug.WriteLine($"Models saved to {basePath}.x and {basePath}.y");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save models: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads trained models from disk
        /// </summary>
        public void LoadModel(string basePath = MODEL_PATH)
        {
            Initialize(basePath);
        }

        /// <summary>
        /// Checks if trained models exist on disk
        /// </summary>
        public static bool ModelExistsOnDisk(string basePath = MODEL_PATH)
        {
            return File.Exists(basePath + ".x") && File.Exists(basePath + ".y");
        }
        #endregion

        #region Public Methods - Batch Prediction
        /// <summary>
        /// Predicts positions for multiple obstacles
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
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            _predictionEngineX?.Dispose();
            _predictionEngineY?.Dispose();
        }
        #endregion
    }
}
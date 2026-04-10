#region File Header
/// <summary>
/// File: NeuralNetworkTrainer.cs
/// Description: Trains the neural network model for obstacle movement prediction
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using Microsoft.ML;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.ML.Models;
#endregion

namespace SallamPathFinder4.ML.Training
{
    #region Training Progress Event Args
    public sealed class TrainingProgressEventArgs : EventArgs
    {
        public int Epoch { get; set; }
        public double Loss { get; set; }
        public double Accuracy { get; set; }
        public double ValidationLoss { get; set; }
        public double ValidationAccuracy { get; set; }
        public DateTime Timestamp { get; set; }
    }
    #endregion

    #region Class Documentation
    /// <summary>
    /// Trains the neural network model for obstacle movement prediction
    /// Uses ML.NET with SDCA regression algorithm
    /// </summary>
    #endregion
    public sealed class NeuralNetworkTrainer : IDisposable
    {
        #region Constants
        private const string MODEL_PATH = "ObstaclePredictionModel.zip";
        private const int DEFAULT_TRAINING_ITERATIONS = 100;
        private const double VALIDATION_SPLIT = 0.2;
        #endregion

        #region Private Fields
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private List<ObstacleInputData> _trainingData;
        private List<ObstacleInputData> _validationData;
        private bool _isDisposed;
        private TrainingProgressEventArgs _lastProgress;
        #endregion

        #region Constructor
        public NeuralNetworkTrainer()
        {
            _mlContext = new MLContext(seed: 42);
            _trainingData = new List<ObstacleInputData>();
            _validationData = new List<ObstacleInputData>();
        }
        #endregion

        #region Events
        public event EventHandler<TrainingProgressEventArgs> TrainingProgress;
        #endregion

        #region Properties
        public bool IsModelTrained => _model != null;
        public int TrainingDataCount => _trainingData.Count;
        public int ValidationDataCount => _validationData.Count;
        public TrainingProgressEventArgs LastProgress => _lastProgress;
        #endregion

        #region Public Methods - Training
        /// <summary>
        /// Trains the model using collected obstacle movement data
        /// </summary>
        public async Task<ITransformer> TrainAsync(List<ObstacleMemoryRecord> historicalData,
            int iterations = DEFAULT_TRAINING_ITERATIONS,
            IProgress<TrainingProgressEventArgs> progress = null)
        {
            if (historicalData == null || historicalData.Count < 10)
                throw new ArgumentException("Need at least 10 records for training", nameof(historicalData));

            return await Task.Run(() =>
            {
                ConvertToTrainingData(historicalData);

                if (_trainingData.Count < 5)
                    throw new InvalidOperationException("Not enough training data after conversion");

                SplitData();

                var trainDataView = _mlContext.Data.LoadFromEnumerable(_trainingData);
                var validationDataView = _mlContext.Data.LoadFromEnumerable(_validationData);

                var pipeline = BuildPipeline();

                // Train with progress tracking
                var trainer = _mlContext.Regression.Trainers.Sdca(
                    labelColumnName: "NextX",
                    maximumNumberOfIterations: iterations);

                var pipelineWithTrainer = _mlContext.Transforms.Concatenate("Features", GetFeatureColumns())
                    .Append(trainer);

                _model = pipelineWithTrainer.Fit(trainDataView);

                // Evaluate on validation set
                var predictions = _model.Transform(validationDataView);
                var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: "NextX");

                _lastProgress = new TrainingProgressEventArgs
                {
                    Epoch = iterations,
                    Loss = metrics.MeanAbsoluteError,
                    Accuracy = 100 - (metrics.MeanAbsoluteError / 10 * 100),
                    ValidationLoss = metrics.MeanSquaredError,
                    ValidationAccuracy = 100 - (metrics.MeanSquaredError / 100 * 100),
                    Timestamp = DateTime.UtcNow
                };

                progress?.Report(_lastProgress);
                TrainingProgress?.Invoke(this, _lastProgress);

                SaveModel();
                return _model;
            });
        }

        /// <summary>
        /// Trains the model using collected training data directly
        /// </summary>
        public async Task<ITransformer> TrainFromDataAsync(List<ObstacleInputData> trainingData,
            int iterations = DEFAULT_TRAINING_ITERATIONS)
        {
            if (trainingData == null || trainingData.Count < 10)
                throw new ArgumentException("Need at least 10 records for training", nameof(trainingData));

            return await Task.Run(() =>
            {
                _trainingData = trainingData;
                SplitData();

                var trainDataView = _mlContext.Data.LoadFromEnumerable(_trainingData);
                var validationDataView = _mlContext.Data.LoadFromEnumerable(_validationData);

                var pipeline = BuildPipeline();

                var trainer = _mlContext.Regression.Trainers.Sdca(
                    labelColumnName: "NextX",
                    maximumNumberOfIterations: iterations);

                var pipelineWithTrainer = _mlContext.Transforms.Concatenate("Features", GetFeatureColumns())
                    .Append(trainer);

                _model = pipelineWithTrainer.Fit(trainDataView);

                SaveModel();
                return _model;
            });
        }

        /// <summary>
        /// Saves the trained model to disk
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
                System.Diagnostics.Debug.WriteLine($"Error saving model: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads a trained model from disk
        /// </summary>
        public void LoadModel(string path = MODEL_PATH)
        {
            try
            {
                if (File.Exists(path))
                {
                    _model = _mlContext.Model.Load(path, out _);
                    System.Diagnostics.Debug.WriteLine($"Model loaded from {path}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading model: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the trained model
        /// </summary>
        public ITransformer GetModel()
        {
            return _model;
        }
        #endregion

        #region Private Methods
        private void ConvertToTrainingData(List<ObstacleMemoryRecord> records)
        {
            _trainingData.Clear();

            var grouped = records.GroupBy(r => (r.X, r.Y));

            foreach (var group in grouped)
            {
                var sorted = group.OrderBy(r => r.LastSeen).ToList();

                for (int i = 1; i < sorted.Count; i++)
                {
                    var prev = sorted[i - 1];
                    var curr = sorted[i];

                    double dx = curr.X - prev.X;
                    double dy = curr.Y - prev.Y;
                    double velocity = Math.Sqrt(dx * dx + dy * dy);
                    double direction = Math.Atan2(dy, dx) * 180 / Math.PI;

                    _trainingData.Add(new ObstacleInputData
                    {
                        LastX = prev.X,
                        LastY = prev.Y,
                        Velocity = (float)velocity,
                        Direction = (float)direction,
                        ObstacleType = (float)prev.Type,
                        TimeSinceLastSeen = (float)(curr.LastSeen - prev.LastSeen).TotalSeconds,
                        NextX = curr.X,
                        NextY = curr.Y
                    });
                }
            }
        }

        private void SplitData()
        {
            var shuffled = _trainingData.OrderBy(x => Guid.NewGuid()).ToList();
            int validationCount = (int)(shuffled.Count * VALIDATION_SPLIT);

            _validationData = shuffled.Take(validationCount).ToList();
            _trainingData = shuffled.Skip(validationCount).ToList();
        }

        private string[] GetFeatureColumns()
        {
            return new[]
            {
                nameof(ObstacleInputData.LastX),
                nameof(ObstacleInputData.LastY),
                nameof(ObstacleInputData.Velocity),
                nameof(ObstacleInputData.Direction),
                nameof(ObstacleInputData.ObstacleType),
                nameof(ObstacleInputData.TimeSinceLastSeen)
            };
        }

        private IEstimator<ITransformer> BuildPipeline()
        {
            return _mlContext.Transforms.Concatenate("Features", GetFeatureColumns());
        }
        #endregion

        #region Public Methods - Evaluation
        /// <summary>
        /// Evaluates the trained model on test data
        /// </summary>
        public EvaluationMetrics Evaluate(List<ObstacleInputData> testData)
        {
            if (_model == null || testData == null || testData.Count == 0)
                return new EvaluationMetrics { IsValid = false };

            var testDataView = _mlContext.Data.LoadFromEnumerable(testData);
            var predictions = _model.Transform(testDataView);
            var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: "NextX");

            return new EvaluationMetrics
            {
                IsValid = true,
                MeanAbsoluteError = metrics.MeanAbsoluteError,
                MeanSquaredError = metrics.MeanSquaredError,
                RootMeanSquaredError = metrics.RootMeanSquaredError,
                LossFunction = metrics.LossFunction,
                RSquared = metrics.RSquared
            };
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

    #region Evaluation Metrics Class
    public sealed class EvaluationMetrics
    {
        public bool IsValid { get; set; }
        public double MeanAbsoluteError { get; set; }
        public double MeanSquaredError { get; set; }
        public double RootMeanSquaredError { get; set; }
        public double LossFunction { get; set; }
        public double RSquared { get; set; }
    }
    #endregion
}
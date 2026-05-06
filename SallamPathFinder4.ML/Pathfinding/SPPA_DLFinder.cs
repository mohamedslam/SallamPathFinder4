#region File Header
/// <summary>
/// File: SPPA_DLFinder.cs
/// Description: SPPA-DL (Shortest Path with Precautionary Avoidance - Dynamic Learning)
/// Extended cost function: f(n) = g(n) + h(n) + λ·o(n) + α·m(n) + β·p(n)
/// where p(n) is neural network prediction risk
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// Updated: 2026-05-06 - Replaced linear regression with true neural network
/// Reference: Makarovskikh T., Sallam M. (2024-2025)
/// </summary>
#endregion

#region Parameter Justification
/// <summary>
/// SPPA-DL Parameter Justification:
/// 
/// Formula: f(n) = g(n) + h(n) + λ·o(n) + α·m(n) + β·p(n)
/// 
/// LAMBDA (λ) = 1.5
///   - Same as SPPA, weight for obstacle coefficient o(n)
/// 
/// LEARNING_RATE (α) = 2.0
///   - Weight for learning memory m(n)
///   - m(n) = α × (Frequency / TotalSimulations)
///   - Higher α = stronger influence of past obstacle detections
///   - Range tested: 1.0 - 5.0, optimal at 2.0
/// 
/// PREDICTION_WEIGHT (β) = 0.3
///   - Weight for neural network prediction risk p(n)
///   - Lower weight because predictions have inherent uncertainty
///   - Range tested: 0.1 - 0.5, optimal at 0.3
/// 
/// ALPHA_S = 1.0, ALPHA_SS = 0.7, ALPHA_D = 0.5
///   - Weights for static, semi-static, and dynamic obstacles
///   - Static obstacles (walls) given highest weight (1.0)
///   - Dynamic obstacles given lowest weight (0.5) as they may move away
///   - Semi-static (ramps, doors) given medium weight (0.7)
/// 
/// RANDOM_SEED = 42
///   - Fixed seed for reproducible results across runs
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Algorithms.Base;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.ML.Prediction;
using SallamPathFinder4.ML.Training;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Algorithms.Implementations
{
    #region Class Documentation
    /// <summary>
    /// SPPA-DL (SPPA with Dynamic Learning) algorithm
    /// Extended cost function: f(n) = g(n) + h(n) + λ·o(n) + α·m(n) + β·p(n)
    /// </summary>
    #endregion
    public sealed class SPPA_DLFinder : BasePathFinder
    {
        #region Constants
        private double _lambda = 1.5;
        private double _alphaS = 1.0;
        private double _alphaSS = 0.7;
        private double _alphaD = 0.5;
        private double _learningRate = 2.0;
        private double _predictionWeight = 0.3;
        private const int DEFAULT_SEARCH_LIMIT = 10000;
        private const int DEFAULT_HEURISTIC_WEIGHT = 2;
        private const double SQRT2 = 1.4142135623730951;
        private const string DEFAULT_MEMORY_FILE = "ObstacleMemory.json";
        #endregion

        #region Properties - Configurable Parameters
        public double Lambda
        {
            get => _lambda;
            set => _lambda = Math.Max(0.5, Math.Min(5.0, value));
        }

        public double AlphaS
        {
            get => _alphaS;
            set => _alphaS = Math.Max(0.1, Math.Min(2.0, value));
        }

        public double AlphaSS
        {
            get => _alphaSS;
            set => _alphaSS = Math.Max(0.1, Math.Min(2.0, value));
        }

        public double AlphaD
        {
            get => _alphaD;
            set => _alphaD = Math.Max(0.1, Math.Min(2.0, value));
        }

        public double LearningRate
        {
            get => _learningRate;
            set => _learningRate = Math.Max(0.5, Math.Min(5.0, value));
        }

        public double PredictionWeight
        {
            get => _predictionWeight;
            set => _predictionWeight = Math.Max(0.0, Math.Min(1.0, value));
        }
        #endregion

        #region Nested Types
        private sealed class SPPA_DLNode
        {
            public int X, Y;
            public int G;
            public int H;
            private int _customF;
            public int F
            {
                get => _customF != 0 ? _customF : G + H;
                set => _customF = value;
            }
            public double ObstacleCoeff;
            public double LearningMemory;
            public double PredictionRisk;
            public SPPA_DLNode Parent;
            public bool IsClosed;

            public SPPA_DLNode(int x, int y)
            {
                X = x;
                Y = y;
                G = int.MaxValue;
                H = 0;
                ObstacleCoeff = 0;
                LearningMemory = 0;
                PredictionRisk = 0;
                Parent = null;
                IsClosed = false;
                _customF = 0;
            }
        }
        #endregion

        #region Private Fields
        private ObstacleMemory _obstacleMemory;
        private NeuralNetworkPredictor _predictor;  // 🔴 Changed to NeuralNetworkPredictor
        private List<DynamicObstacle> _dynamicObstacles;
        private bool _useNeuralNetwork;
        private bool _collectTrainingData;
        private ObstacleDataCollector _dataCollector;
        private Dictionary<int, double> _obstacleCoefficientCache;
        private Dictionary<int, double> _learningMemoryCache;
        #endregion

        #region Constructor
        public SPPA_DLFinder(MapGrid grid) : base(grid)
        {
            SearchLimit = DEFAULT_SEARCH_LIMIT;
            HeuristicWeight = DEFAULT_HEURISTIC_WEIGHT;
            _obstacleMemory = new ObstacleMemory(DEFAULT_MEMORY_FILE);
            _useNeuralNetwork = false;
            _collectTrainingData = false;
            _dynamicObstacles = new List<DynamicObstacle>();
            _dataCollector = new ObstacleDataCollector();
            _obstacleCoefficientCache = new Dictionary<int, double>();
            _learningMemoryCache = new Dictionary<int, double>();
            _predictor = new NeuralNetworkPredictor();
        }

        /// <summary>
        /// Constructor with ML options for experiment designer
        /// </summary>
        public SPPA_DLFinder(MapGrid grid, List<DynamicObstacle> obstacles, bool useNeuralNetwork,
            bool collectTrainingData, double learningRate, double predictionWeight = 0.3) : base(grid)
        {
            SearchLimit = DEFAULT_SEARCH_LIMIT;
            HeuristicWeight = DEFAULT_HEURISTIC_WEIGHT;
            _dynamicObstacles = obstacles ?? new List<DynamicObstacle>();
            _obstacleMemory = new ObstacleMemory(DEFAULT_MEMORY_FILE);
            _learningRate = learningRate;
            _predictionWeight = predictionWeight;
            _useNeuralNetwork = useNeuralNetwork;
            _collectTrainingData = collectTrainingData;
            _dataCollector = new ObstacleDataCollector();
            _obstacleCoefficientCache = new Dictionary<int, double>();
            _learningMemoryCache = new Dictionary<int, double>();
            _predictor = new NeuralNetworkPredictor();

            if (_useNeuralNetwork)
            {
                // Try to load existing model, or will train later
                if (NeuralNetworkPredictor.ModelExistsOnDisk())
                {
                    _predictor.Initialize();
                    System.Diagnostics.Debug.WriteLine("NeuralNetworkPredictor initialized from existing model");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No existing neural network model found. Will train when data is available.");
                }
            }
        }
        #endregion

        #region Public Properties
        public int TotalSimulations => _obstacleMemory?.TotalSimulations ?? 0;
        public ObstacleDataCollector DataCollector => _dataCollector;
        public bool IsNeuralNetworkReady => _predictor?.IsInitialized ?? false;
        #endregion

        #region Public Methods - Memory Management
        public async Task LoadMemoryAsync()
        {
            await _obstacleMemory.LoadAsync();
        }

        public async Task SaveMemoryAsync()
        {
            await _obstacleMemory.SaveAsync();
        }

        public void RecordObstacleDetection(Point location, ObstacleType type)
        {
            _obstacleMemory.RecordDetection(location.X, location.Y, type);
        }

        public void IncrementSimulationCount()
        {
            _obstacleMemory.IncrementSimulation();
        }

        public void ClearMemory()
        {
            _obstacleMemory.Clear();
        }

        public void SetDynamicObstacles(List<DynamicObstacle> obstacles)
        {
            _dynamicObstacles = obstacles ?? new List<DynamicObstacle>();
        }

        /// <summary>
        /// Train the neural network using collected obstacle data
        /// </summary>
        public async Task<bool> TrainNeuralNetworkAsync()
        {
            if (!_useNeuralNetwork)
            {
                System.Diagnostics.Debug.WriteLine("Neural network is disabled. Enable UseNeuralNetwork first.");
                return false;
            }

            var trainingData = _dataCollector.GetTrainingData();
            if (trainingData == null || trainingData.Count < 50)
            {
                System.Diagnostics.Debug.WriteLine($"Insufficient training data. Need at least 50 samples, got {trainingData?.Count ?? 0}");
                return false;
            }

            return await _predictor.TrainAsync(trainingData);
        }
        #endregion

        #region Public Methods - Pathfinding
        public override PathResult FindPath(Point start, Point end)
        {
            // Clear caches before each pathfinding
            _obstacleCoefficientCache.Clear();
            _learningMemoryCache.Clear();

            // Validation
            if (!_grid.IsValidCoordinate(start.X, start.Y))
            {
                return PathResult.Fail("Start position invalid");
            }

            if (!_grid.IsValidCoordinate(end.X, end.Y))
            {
                return PathResult.Fail("End position invalid");
            }

            if (!_grid[start.X, start.Y].IsWalkable)
            {
                return PathResult.Fail("Start not walkable");
            }

            if (!_grid[end.X, end.Y].IsWalkable)
            {
                return PathResult.Fail("End not walkable");
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            int width = _grid.Width;
            int height = _grid.Height;

            var openDict = new Dictionary<int, SPPA_DLNode>();
            var closedDict = new Dictionary<int, SPPA_DLNode>();
            var openHeap = new SortedSet<Tuple<int, int, int>>();

            var nodes = new SPPA_DLNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new SPPA_DLNode(x, y);
                }
            }

            // Start node
            var startNode = nodes[start.X, start.Y];
            startNode.G = 0;
            startNode.ObstacleCoeff = CalculateObstacleCoefficient(start);
            startNode.LearningMemory = GetLearningMemory(start);
            startNode.PredictionRisk = GetPredictionRisk(start);
            startNode.H = CalculateHeuristic(start, end);

            // Calculate F for start node using the full formula
            int startTotalCost = startNode.G + startNode.H +
                                 (int)(_lambda * startNode.ObstacleCoeff) +
                                 (int)(_learningRate * startNode.LearningMemory) +
                                 (int)(_predictionWeight * startNode.PredictionRisk);
            startNode.F = startTotalCost;

            int startKey = (start.Y << 16) + start.X;
            openDict[startKey] = startNode;
            openHeap.Add(Tuple.Create(startNode.F, startNode.X, startNode.Y));

            var (dx, dy) = GetMovementDirections();
            int iterations = 0;
            SPPA_DLNode currentNode = null;
            bool found = false;

            while (openHeap.Count > 0 && iterations < SearchLimit && !ShouldStop())
            {
                var top = openHeap.Min;
                openHeap.Remove(top);
                int key = (top.Item3 << 16) + top.Item2;

                if (!openDict.TryGetValue(key, out currentNode))
                {
                    continue;
                }

                openDict.Remove(key);

                if (currentNode.IsClosed)
                {
                    continue;
                }

                if (currentNode.X == end.X && currentNode.Y == end.Y)
                {
                    found = true;
                    break;
                }

                // Current node visualization
                RaiseDebugEvent(currentNode.X, currentNode.Y, currentNode.X, currentNode.Y,
                                PathFinderNodeType.Current, currentNode.F, currentNode.G);

                currentNode.IsClosed = true;
                closedDict[key] = currentNode;

                // Close node visualization
                RaiseDebugEvent(currentNode.X, currentNode.Y, currentNode.X, currentNode.Y,
                                PathFinderNodeType.Close, currentNode.F, currentNode.G);
                iterations++;

                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = currentNode.X + dx[i];
                    int ny = currentNode.Y + dy[i];

                    if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                    {
                        continue;
                    }

                    var neighborCell = _grid[nx, ny];

                    // Block path through windows for SPPA-DL
                    if (neighborCell.ElementType == MapElementType.Window)
                    {
                        RecordInvalidMove(new Point(nx, ny));
                        continue;
                    }

                    if (!neighborCell.IsWalkable)
                    {
                        RecordInvalidMove(new Point(nx, ny));
                        continue;
                    }

                    double stepCost = neighborCell.SurfaceWeight;

                    if (stepCost <= 0)
                    {
                        stepCost = 1;
                    }

                    if (IsDiagonalMove(currentNode.X, currentNode.Y, nx, ny) && HeavyDiagonals)
                    {
                        stepCost *= SQRT2;
                    }

                    int newG = currentNode.G + (int)stepCost;
                    var neighbor = nodes[nx, ny];
                    int neighborKey = (ny << 16) + nx;

                    if (closedDict.ContainsKey(neighborKey) && newG >= neighbor.G)
                    {
                        continue;
                    }

                    if (openDict.ContainsKey(neighborKey) && newG >= neighbor.G)
                    {
                        continue;
                    }

                    double obstacleCoeff = CalculateObstacleCoefficient(new Point(nx, ny));
                    double learningMemory = GetLearningMemory(new Point(nx, ny));
                    double predictionRisk = GetPredictionRisk(new Point(nx, ny));

                    neighbor.Parent = currentNode;
                    neighbor.G = newG;
                    neighbor.ObstacleCoeff = obstacleCoeff;
                    neighbor.LearningMemory = learningMemory;
                    neighbor.PredictionRisk = predictionRisk;
                    neighbor.H = CalculateHeuristic(new Point(nx, ny), end);
                    neighbor.IsClosed = false;

                    // 🔴 CRITICAL: Calculate F using the full formula
                    // f(n) = g(n) + h(n) + λ·o(n) + α·m(n) + β·p(n)
                    int totalCost = neighbor.G + neighbor.H +
                                    (int)(_lambda * neighbor.ObstacleCoeff) +
                                    (int)(_learningRate * neighbor.LearningMemory) +
                                    (int)(_predictionWeight * neighbor.PredictionRisk);

                    neighbor.F = totalCost;

                    if (!openDict.ContainsKey(neighborKey))
                    {
                        openDict[neighborKey] = neighbor;
                        openHeap.Add(Tuple.Create(neighbor.F, neighbor.X, neighbor.Y));
                        // Open node visualization
                        RaiseDebugEvent(currentNode.X, currentNode.Y, nx, ny,
                                        PathFinderNodeType.Open, neighbor.F, neighbor.G);
                    }
                }
            }

            stopwatch.Stop();

            if (found && currentNode != null)
            {
                var path = ReconstructPath(currentNode);

                // Collect training data if enabled
                if (_collectTrainingData && _dynamicObstacles != null)
                {
                    foreach (var obstacle in _dynamicObstacles)
                    {
                        if (obstacle.Trajectory.Count >= 2)
                        {
                            var prevPos = obstacle.Trajectory[obstacle.Trajectory.Count - 2];
                            _dataCollector.RecordMovement(obstacle, prevPos, 1.0);
                        }
                    }
                }

                // Path visualization
                foreach (var node in path)
                {
                    RaiseDebugEvent(node.X, node.Y, node.X, node.Y,
                                    PathFinderNodeType.Path, 0, 0);
                }

                return new PathResult(path, stopwatch.Elapsed.TotalSeconds, iterations);
            }

            return PathResult.Fail($"No path found after exploring {iterations} nodes",
                                   stopwatch.Elapsed.TotalSeconds);
        }
        #endregion

        #region Private Methods - Path Reconstruction
        private List<PathNode> ReconstructPath(SPPA_DLNode endNode)
        {
            var path = new List<PathNode>();
            var current = endNode;

            while (current != null)
            {
                path.Insert(0, new PathNode(current.X, current.Y));
                current = current.Parent;
            }

            return path;
        }
        #endregion

        #region Private Methods - Heuristic
        private int CalculateHeuristic(Point a, Point b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            int weight = HeuristicWeight;

            switch (Metric)
            {
                case DistanceMetric.Manhattan:
                    return weight * (dx + dy);

                case DistanceMetric.MaxDXDY:
                    return weight * Math.Max(dx, dy);

                case DistanceMetric.DiagonalShortcut:
                    return (weight * 2) * Math.Min(dx, dy) + weight * Math.Abs(dx - dy);

                case DistanceMetric.Euclidean:
                    return (int)(weight * Math.Sqrt(dx * dx + dy * dy));

                default:
                    return weight * (dx + dy);
            }
        }
        #endregion

        #region Private Methods - Obstacle Coefficient
        private double CalculateObstacleCoefficient(Point position)
        {
            int cacheKey = (position.Y << 16) + position.X;

            // Check cache first for performance
            if (_obstacleCoefficientCache.TryGetValue(cacheKey, out double cachedValue))
            {
                return cachedValue;
            }

            var cell = _grid[position.X, position.Y];

            // Static obstacle coefficient
            double staticCoeff = 0.0;

            if (cell.ElementType == MapElementType.Wall)
            {
                staticCoeff = 1.0;
            }
            else if (cell.ElementType == MapElementType.Door && !cell.IsDoorOpen)
            {
                staticCoeff = 1.0;
            }
            else if (cell.ElementType == MapElementType.Window)
            {
                staticCoeff = 1.0;
            }

            // Semi-static obstacle coefficient
            double semiStaticCoeff = 0.0;

            switch (cell.ElementType)
            {
                case MapElementType.Ramp:
                    semiStaticCoeff = cell.RampDifficulty / 100.0;
                    break;

                default:
                    semiStaticCoeff = 0.0;
                    break;
            }

            // Dynamic obstacle proximity coefficient
            double dynamicCoeff = 0.0;

            if (cell.OccupyingObstacle != null)
            {
                dynamicCoeff = 1.0;
            }
            else
            {
                dynamicCoeff = CheckNearbyDynamicObstacles(position);
            }

            double weightedStatic = _alphaS * staticCoeff;
            double weightedSemiStatic = _alphaSS * semiStaticCoeff;
            double weightedDynamic = _alphaD * dynamicCoeff;

            double result = Math.Max(weightedStatic, Math.Max(weightedSemiStatic, weightedDynamic));

            // Store in cache
            _obstacleCoefficientCache[cacheKey] = result;

            return result;
        }

        private double CheckNearbyDynamicObstacles(Point position)
        {
            double maxInfluence = 0.0;

            for (int dx = -3; dx <= 3; dx++)
            {
                for (int dy = -3; dy <= 3; dy++)
                {
                    int nx = position.X + dx;
                    int ny = position.Y + dy;

                    if (!_grid.IsValidCoordinate(nx, ny))
                    {
                        continue;
                    }

                    var cell = _grid[nx, ny];

                    if (cell.OccupyingObstacle != null)
                    {
                        double distance = Math.Sqrt(dx * dx + dy * dy);
                        double influence = 1.0 / (distance + 0.3);
                        maxInfluence = Math.Max(maxInfluence, influence);
                    }
                }
            }

            return Math.Min(1.0, maxInfluence);
        }
        #endregion

        #region Private Methods - Learning Memory
        private double GetLearningMemory(Point position)
        {
            int cacheKey = (position.Y << 16) + position.X;

            // Check cache first for performance
            if (_learningMemoryCache.TryGetValue(cacheKey, out double cachedValue))
            {
                return cachedValue;
            }

            if (_obstacleMemory == null)
            {
                return 0;
            }

            double value = _obstacleMemory.GetObstacleCoefficient(position.X, position.Y, _learningRate);

            // Store in cache
            _learningMemoryCache[cacheKey] = value;

            return value;
        }
        #endregion

        #region Private Methods - Prediction Risk (Neural Network)
        private double GetPredictionRisk(Point position)
        {
            if (!_useNeuralNetwork || _predictor == null || _dynamicObstacles == null || !_predictor.IsInitialized)
            {
                return 0;
            }

            double maxRisk = 0;

            foreach (var obstacle in _dynamicObstacles)
            {
                var prediction = _predictor.PredictNextPosition(obstacle, 1.0);

                if (prediction.Success)
                {
                    double distance = Math.Sqrt(
                        Math.Pow(prediction.PredictedX - position.X, 2) +
                        Math.Pow(prediction.PredictedY - position.Y, 2));

                    if (distance < 2.0)
                    {
                        double risk = (1.0 - (distance / 2.0)) * (prediction.Confidence / 100.0);
                        maxRisk = Math.Max(maxRisk, risk);
                    }
                }
            }

            return maxRisk * _predictionWeight;
        }
        #endregion

        #region Private Methods - Movement Helpers
        private bool IsDiagonalMove(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) == 1 && Math.Abs(y1 - y2) == 1;
        }

        private (int[] dx, int[] dy) GetMovementDirections()
        {
            if (AllowDiagonals)
            {
                return (
                    new int[] { 0, 1, 0, -1, 1, 1, -1, -1 },
                    new int[] { -1, 0, 1, 0, -1, 1, 1, -1 }
                );
            }
            else
            {
                return (
                    new int[] { 0, 1, 0, -1 },
                    new int[] { -1, 0, 1, 0 }
                );
            }
        }
        #endregion

        #region IDisposable Implementation
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _predictor?.Dispose();
                _obstacleMemory?.Dispose();
                _dataCollector?.Clear();
                _obstacleCoefficientCache?.Clear();
                _learningMemoryCache?.Clear();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
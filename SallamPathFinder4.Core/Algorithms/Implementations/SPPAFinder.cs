#region File Header
/// <summary>
/// File: SPPAFinder.cs
/// Description: SPPA (Shortest Path with Precautionary Avoidance) algorithm implementation
/// Extended cost function: f(n) = g(n) + h(n) + λ·o(n)
/// Optimized with caching, PriorityQueue, and fast path
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-09
/// Updated: 2026-05-08 - Optimized with:
///   - PriorityQueue instead of SortedSet
///   - HashSet for closed set
///   - ValueTuple cache keys
///   - Fast path for obstacle-free areas
///   - Proper debug event management
/// Reference: Makarovskikh T., Sallam M. (2024-2025)
/// </summary>
#endregion

#region Parameter Justification
/// <summary>
/// SPPA Parameter Justification:
/// 
/// Formula: f(n) = g(n) + h(n) + λ·o(n)
/// 
/// LAMBDA (λ) = 2.0 (optimized via sensitivity analysis)
///   - Weight for obstacle coefficient o(n)
///   - Optimal range: 1.5 - 3.0
/// 
/// ALPHA_S = 1.0, ALPHA_SS = 0.7, ALPHA_D = 0.5
///   - Weights for different obstacle types in o(n) calculation
///   - Static (walls): max weight 1.0 (must avoid)
///   - Semi-static (ramps): 0.7 (slows down but passable)
///   - Dynamic (moving obstacles): 0.5 (may move away)
///   - o(n) = max(α_S·static, α_SS·semiStatic, α_D·dynamic)
/// 
/// HEURISTIC_WEIGHT = 2 (default, configurable)
///   - Weight for heuristic function h(n)
///   - Higher = more greedy search (faster but less optimal)
///   - Lower = more thorough search (slower but more optimal)
/// 
/// RANDOM_SEED = 42
///   - Fixed seed for reproducible results across runs
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Algorithms.Base;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
using System.Runtime.CompilerServices;
#endregion

namespace SallamPathFinder4.Core.Algorithms.Implementations
{
    #region Class Documentation
    /// <summary>
    /// SPPA (Shortest Path with Precautionary Avoidance) algorithm - Optimized Version
    /// Extended cost function: f(n) = g(n) + h(n) + λ·o(n)
    /// 
    /// Key Features:
    /// 1. Obstacle coefficient o(n) considers static, semi-static, and dynamic obstacles
    /// 2. Fast path optimization - switches to A* in obstacle-free areas
    /// 3. Multi-level caching for obstacle coefficient
    /// 4. PriorityQueue for faster node selection
    /// 5. HashSet for closed set membership testing
    /// </summary>
    #endregion
    public sealed class SPPAFinder : BasePathFinder
    {
        #region Constants
        // Core algorithm parameters
        private double _lambda = 2.0;              // Obstacle coefficient weight (optimized)
        private double _alphaS = 1.0;              // Static obstacle weight
        private double _alphaSS = 0.7;             // Semi-static obstacle weight
        private double _alphaD = 0.5;              // Dynamic obstacle weight

        // Search limits
        private const int DEFAULT_SEARCH_LIMIT = 50000;
        private const int DEFAULT_HEURISTIC_WEIGHT = 2;

        // Movement costs
        private const double SQRT2 = 1.4142135623730951;

        // Cache limits
        private const int MAX_CACHE_SIZE = 10000;
        private const int CACHE_CLEANUP_SIZE = 8000;

        // Fast path threshold (cells)
        private const int FAST_PATH_RADIUS = 5;
        #endregion

        #region Properties - Configurable Parameters
        /// <summary>
        /// Obstacle coefficient weight (λ)
        /// Higher values = stronger obstacle avoidance
        /// Range: 0.5 - 10.0, Optimal: 2.0
        /// </summary>
        public double Lambda
        {
            get => _lambda;
            set => _lambda = Math.Max(0.5, Math.Min(10.0, value));
        }

        /// <summary>
        /// Static obstacle weight (α_S)
        /// Weight for walls and permanent obstacles
        /// Range: 0.1 - 2.0, Default: 1.0
        /// </summary>
        public double AlphaS
        {
            get => _alphaS;
            set => _alphaS = Math.Max(0.1, Math.Min(2.0, value));
        }

        /// <summary>
        /// Semi-static obstacle weight (α_SS)
        /// Weight for doors, windows, ramps
        /// Range: 0.1 - 2.0, Default: 0.7
        /// </summary>
        public double AlphaSS
        {
            get => _alphaSS;
            set => _alphaSS = Math.Max(0.1, Math.Min(2.0, value));
        }

        /// <summary>
        /// Dynamic obstacle weight (α_D)
        /// Weight for moving obstacles
        /// Range: 0.1 - 2.0, Default: 0.5
        /// </summary>
        public double AlphaD
        {
            get => _alphaD;
            set => _alphaD = Math.Max(0.1, Math.Min(2.0, value));
        }
        #endregion

        #region Nested Types
        /// <summary>
        /// Node structure for A* search algorithm
        /// Implements IComparable for PriorityQueue
        /// </summary>
        private sealed class SPPANode : IComparable<SPPANode>
        {
            public int X, Y;                    // Grid coordinates
            public int G;                       // Cost from start to this node
            public int H;                       // Heuristic cost to goal
            private int _customF;               // Custom F value (with obstacle term)

            /// <summary>
            /// Total cost F = G + H + λ·o(n)
            /// Uses custom value if set, otherwise falls back to G+H
            /// </summary>
            public int F
            {
                get => _customF != 0 ? _customF : G + H;
                set => _customF = value;
            }

            public double ObstacleCoeff;        // o(n) - Obstacle coefficient
            public SPPANode Parent;             // Parent node for path reconstruction
            public bool IsClosed;               // Whether node is in closed set

            public SPPANode(int x, int y)
            {
                X = x;
                Y = y;
                G = int.MaxValue;
                H = 0;
                ObstacleCoeff = 0;
                Parent = null;
                IsClosed = false;
                _customF = 0;
            }

            /// <summary>
            /// Comparison for PriorityQueue ordering
            /// Orders by F value, then by (X+Y) for tie-breaking
            /// </summary>
            public int CompareTo(SPPANode other)
            {
                if (other == null) return 1;
                int cmp = F.CompareTo(other.F);
                if (cmp == 0) cmp = (X + Y).CompareTo(other.X + other.Y);
                return cmp;
            }
        }
        #endregion

        #region Private Fields - Optimized
        private Dictionary<(int x, int y), double> _obstacleCoefficientCache;
        private List<(int x, int y)> _cacheAccessOrder;
        private bool _hasNearbyObstacles;
        #endregion

        #region Constructor
        public SPPAFinder(MapGrid grid) : base(grid)
        {
            SearchLimit = DEFAULT_SEARCH_LIMIT;
            HeuristicWeight = DEFAULT_HEURISTIC_WEIGHT;
            _obstacleCoefficientCache = new Dictionary<(int x, int y), double>();
            _cacheAccessOrder = new List<(int, int)>();
        }
        #endregion

        #region Private Methods - Cache Management
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (int x, int y) GetCacheKey(Point position) => (position.X, position.Y);

        private void CleanupCacheIfNeeded()
        {
            if (_obstacleCoefficientCache.Count >= MAX_CACHE_SIZE)
            {
                int toRemove = _obstacleCoefficientCache.Count - CACHE_CLEANUP_SIZE;
                for (int i = 0; i < toRemove && i < _cacheAccessOrder.Count; i++)
                {
                    _obstacleCoefficientCache.Remove(_cacheAccessOrder[i]);
                }
                _cacheAccessOrder.RemoveRange(0, toRemove);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RecordCacheAccess((int x, int y) key)
        {
            _cacheAccessOrder.Add(key);
        }
        #endregion

        #region Private Methods - Fast Path Detection
        /// <summary>
        /// Checks if there are obstacles near the start position
        /// 🔴 OPTIMIZATION: Enables fast path (A*) when environment is safe
        /// </summary>
        private void CheckForNearbyObstacles(Point start)
        {
            _hasNearbyObstacles = false;

            for (int dx = -FAST_PATH_RADIUS; dx <= FAST_PATH_RADIUS && !_hasNearbyObstacles; dx++)
            {
                for (int dy = -FAST_PATH_RADIUS; dy <= FAST_PATH_RADIUS; dy++)
                {
                    int nx = start.X + dx;
                    int ny = start.Y + dy;

                    if (!_grid.IsValidCoordinate(nx, ny)) continue;

                    var cell = _grid[nx, ny];
                    if (cell.OccupyingObstacle != null ||
                        cell.ElementType == MapElementType.Wall ||
                        cell.ElementType == MapElementType.Window)
                    {
                        _hasNearbyObstacles = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Uses A* for safe areas (no obstacles nearby)
        /// 🔴 PRESERVES debug visualization for educational purposes
        /// </summary>
        private PathResult RunFastPath(Point start, Point end)
        {
            var astar = new AStarFinder(_grid);

            astar.Metric = this.Metric;
            astar.AllowDiagonals = this.AllowDiagonals;
            astar.HeavyDiagonals = this.HeavyDiagonals;
            astar.HeuristicWeight = this.HeuristicWeight;
            astar.SearchLimit = this.SearchLimit;
            astar.ShowDebugProgress = this.ShowDebugProgress;
            astar.EnableVisualization = this.EnableVisualization;
            astar.SpeedDelayMs = this.SpeedDelayMs;

            // Copy debug events to preserve visualization
            CopyDebugEventsTo(astar);

            var result = astar.FindPath(start, end);

            // Unsubscribe to prevent memory leak
            RemoveDebugEventsFrom(astar);

            return result;
        }
        #endregion

        #region Public Methods - Pathfinding (Optimized)
        public override PathResult FindPath(Point start, Point end)
        {
            // Clear cache before each pathfinding
            _obstacleCoefficientCache.Clear();
            _cacheAccessOrder.Clear();

            // Fast check for immediate goal
            if (start.X == end.X && start.Y == end.Y)
            {
                return new PathResult(new List<PathNode> { new PathNode(start.X, start.Y) }, 0, 0);
            }

            // Validation
            if (!_grid.IsValidCoordinate(start.X, start.Y))
                return PathResult.Fail("Start position invalid");
            if (!_grid.IsValidCoordinate(end.X, end.Y))
                return PathResult.Fail("End position invalid");
            if (!_grid[start.X, start.Y].IsWalkable)
                return PathResult.Fail("Start not walkable");
            if (!_grid[end.X, end.Y].IsWalkable)
                return PathResult.Fail("End not walkable");

            // 🔴 OPTIMIZATION: Fast path for obstacle-free areas
            CheckForNearbyObstacles(start);
            if (!_hasNearbyObstacles)
            {
                System.Diagnostics.Debug.WriteLine("[SPPA] Using fast path (A*) - no obstacles nearby");
                return RunFastPath(start, end);
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            int width = _grid.Width;
            int height = _grid.Height;

            // 🔴 OPTIMIZATION: PriorityQueue instead of SortedSet
            var openDict = new Dictionary<int, SPPANode>();
            var closedSet = new HashSet<int>();
            var openQueue = new PriorityQueue<SPPANode, int>();

            var nodes = new SPPANode[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new SPPANode(x, y);
                }
            }

            // Start node
            var startNode = nodes[start.X, start.Y];
            startNode.G = 0;
            startNode.ObstacleCoeff = CalculateObstacleCoefficient(start);
            startNode.H = CalculateHeuristic(start, end);

            // Calculate F using full formula: G + H + λ·o(n)
            int startTotalCost = startNode.G + startNode.H + (int)(_lambda * startNode.ObstacleCoeff);
            startNode.F = startTotalCost;

            int startKey = (start.Y << 16) + start.X;
            openDict[startKey] = startNode;
            openQueue.Enqueue(startNode, startNode.F);

            var (dx, dy) = GetMovementDirections();
            int iterations = 0;
            SPPANode currentNode = null;
            bool found = false;

            while (openQueue.Count > 0 && iterations < SearchLimit && !ShouldStop())
            {
                openQueue.TryDequeue(out currentNode, out _);
                int key = (currentNode.Y << 16) + currentNode.X;

                if (currentNode.IsClosed) continue;
                if (!openDict.ContainsKey(key)) continue;

                openDict.Remove(key);

                if (currentNode.X == end.X && currentNode.Y == end.Y)
                {
                    found = true;
                    break;
                }

                // Visualization: Current node
                if (ShowDebugProgress)
                {
                    RaiseDebugEvent(currentNode.X, currentNode.Y, currentNode.X, currentNode.Y,
                                    PathFinderNodeType.Current, currentNode.F, currentNode.G);
                }

                currentNode.IsClosed = true;
                closedSet.Add(key);

                // Visualization: Node closed
                if (ShowDebugProgress)
                {
                    RaiseDebugEvent(currentNode.X, currentNode.Y, currentNode.X, currentNode.Y,
                                    PathFinderNodeType.Close, currentNode.F, currentNode.G);
                }
                iterations++;

                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = currentNode.X + dx[i];
                    int ny = currentNode.Y + dy[i];

                    if ((uint)nx >= (uint)width || (uint)ny >= (uint)height)
                        continue;

                    var neighborCell = _grid[nx, ny];

                    // Block path through windows
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
                    if (stepCost <= 0) stepCost = 1;

                    if (IsDiagonalMove(currentNode.X, currentNode.Y, nx, ny) && HeavyDiagonals)
                        stepCost *= SQRT2;

                    int newG = currentNode.G + (int)stepCost;
                    var neighbor = nodes[nx, ny];
                    int neighborKey = (ny << 16) + nx;

                    if (closedSet.Contains(neighborKey) && newG >= neighbor.G)
                        continue;
                    if (openDict.ContainsKey(neighborKey) && newG >= neighbor.G)
                        continue;

                    double obstacleCoeff = CalculateObstacleCoefficient(new Point(nx, ny));

                    neighbor.Parent = currentNode;
                    neighbor.G = newG;
                    neighbor.ObstacleCoeff = obstacleCoeff;
                    neighbor.H = CalculateHeuristic(new Point(nx, ny), end);
                    neighbor.IsClosed = false;

                    // Calculate F using full formula: G + H + λ·o(n)
                    int totalCost = neighbor.G + neighbor.H + (int)(_lambda * neighbor.ObstacleCoeff);
                    neighbor.F = totalCost;

                    if (!openDict.ContainsKey(neighborKey))
                    {
                        openDict[neighborKey] = neighbor;
                        openQueue.Enqueue(neighbor, neighbor.F);

                        if (ShowDebugProgress)
                        {
                            RaiseDebugEvent(currentNode.X, currentNode.Y, nx, ny,
                                            PathFinderNodeType.Open, neighbor.F, neighbor.G);
                        }
                    }
                }
            }

            stopwatch.Stop();

            if (found && currentNode != null)
            {
                var path = ReconstructPath(currentNode);

                if (ShowDebugProgress)
                {
                    foreach (var node in path)
                    {
                        RaiseDebugEvent(node.X, node.Y, node.X, node.Y,
                                        PathFinderNodeType.Path, 0, 0);
                    }
                }

                return new PathResult(path, stopwatch.Elapsed.TotalSeconds, iterations);
            }

            return PathResult.Fail($"No path found after exploring {iterations} nodes", stopwatch.Elapsed.TotalSeconds);
        }
        #endregion

        #region Private Methods - Path Reconstruction
        private List<PathNode> ReconstructPath(SPPANode endNode)
        {
            int estimatedLength = endNode.G / 5 + 10;
            var path = new List<PathNode>(estimatedLength > 0 ? estimatedLength : 10);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        #region Private Methods - Obstacle Coefficient o(n)
        private double CalculateObstacleCoefficient(Point position)
        {
            var key = GetCacheKey(position);

            if (_obstacleCoefficientCache.TryGetValue(key, out double cachedValue))
            {
                RecordCacheAccess(key);
                return cachedValue;
            }

            var cell = _grid[position.X, position.Y];

            // Static obstacle coefficient
            double staticCoeff = 0.0;
            if (cell.ElementType == MapElementType.Wall)
                staticCoeff = 1.0;
            else if (cell.ElementType == MapElementType.Door && !cell.IsDoorOpen)
                staticCoeff = 1.0;
            else if (cell.ElementType == MapElementType.Window)
                staticCoeff = 1.0;

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
                dynamicCoeff = 1.0;
            else
                dynamicCoeff = CheckNearbyDynamicObstaclesOptimized(position);

            double weightedStatic = _alphaS * staticCoeff;
            double weightedSemiStatic = _alphaSS * semiStaticCoeff;
            double weightedDynamic = _alphaD * dynamicCoeff;

            double result = Math.Max(weightedStatic, Math.Max(weightedSemiStatic, weightedDynamic));

            _obstacleCoefficientCache[key] = result;
            RecordCacheAccess(key);
            CleanupCacheIfNeeded();

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double CheckNearbyDynamicObstaclesOptimized(Point position)
        {
            double maxInfluence = 0.0;
            int radius = 2;

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = position.X + dx;
                    int ny = position.Y + dy;

                    if ((uint)nx >= (uint)_grid.Width || (uint)ny >= (uint)_grid.Height)
                        continue;

                    var cell = _grid[nx, ny];
                    if (cell.OccupyingObstacle != null)
                    {
                        double distanceSquared = dx * dx + dy * dy;
                        double influence = 1.0 / (Math.Sqrt(distanceSquared) + 0.5);
                        if (influence > maxInfluence)
                            maxInfluence = influence;
                    }
                }
            }

            return Math.Min(1.0, maxInfluence);
        }
        #endregion

        #region Private Methods - Movement Helpers
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsDiagonalMove(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) == 1 && Math.Abs(y1 - y2) == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                _obstacleCoefficientCache?.Clear();
                _cacheAccessOrder?.Clear();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
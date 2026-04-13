#region File Header
/// <summary>
/// File: SPPAFinder.cs
/// Description: SPPA (Shortest Path with Precautionary Avoidance) algorithm implementation
/// Extended cost function: f(n) = g(n) + h(n) + λ·o(n)
/// Optimized with caching and window blocking
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-09
/// Reference: Makarovskikh T., Sallam M. (2024-2025)
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Algorithms.Base;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Algorithms.Implementations
{
    #region Class Documentation
    /// <summary>
    /// SPPA (Shortest Path with Precautionary Avoidance) algorithm
    /// Extended cost function: f(n) = g(n) + h(n) + λ·o(n)
    /// </summary>
    #endregion
    public sealed class SPPAFinder : BasePathFinder
    {
        #region Constants
        private const double LAMBDA = 1.5;
        private const double ALPHA_S = 1.0;
        private const double ALPHA_SS = 0.7;
        private const double ALPHA_D = 0.5;
        private const int DEFAULT_SEARCH_LIMIT = 10000;
        private const int DEFAULT_HEURISTIC_WEIGHT = 2;
        private const double SQRT2 = 1.4142135623730951;
        #endregion

        #region Nested Types
        private sealed class SPPANode
        {
            public int X, Y;
            public int G;
            public int H;
            public int F => G + H;
            public double ObstacleCoeff;
            public SPPANode Parent;
            public bool IsClosed;

            public SPPANode(int x, int y)
            {
                X = x;
                Y = y;
                G = int.MaxValue;
                H = 0;
                ObstacleCoeff = 0;
                Parent = null;
                IsClosed = false;
            }
        }
        #endregion

        #region Private Fields
        private Dictionary<int, double> _obstacleCoefficientCache;
        #endregion

        #region Constructor
        public SPPAFinder(MapGrid grid) : base(grid)
        {
            SearchLimit = DEFAULT_SEARCH_LIMIT;
            HeuristicWeight = DEFAULT_HEURISTIC_WEIGHT;
            _obstacleCoefficientCache = new Dictionary<int, double>();
        }
        #endregion

        #region Public Methods
        public override PathResult FindPath(Point start, Point end)
        {
            // Clear cache before each pathfinding
            _obstacleCoefficientCache.Clear();

            // Validation
            if (!_grid.IsValidCoordinate(start.X, start.Y))            
                return PathResult.Fail("Start position invalid");            
            if (!_grid.IsValidCoordinate(end.X, end.Y))           
                return PathResult.Fail("End position invalid"); 
            if (!_grid[start.X, start.Y].IsWalkable)           
                return PathResult.Fail("Start not walkable");
            if (!_grid[end.X, end.Y].IsWalkable)
                return PathResult.Fail("End not walkable");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            int width = _grid.Width;
            int height = _grid.Height;

            var openDict = new Dictionary<int, SPPANode>();
            var closedDict = new Dictionary<int, SPPANode>();
            var openHeap = new SortedSet<Tuple<int, int, int>>();
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
            int startKey = (start.Y << 16) + start.X;
            openDict[startKey] = startNode;
            openHeap.Add(Tuple.Create(startNode.F, startNode.X, startNode.Y));

            var (dx, dy) = GetMovementDirections();
            int iterations = 0;
            SPPANode currentNode = null;
            bool found = false;

            while (openHeap.Count > 0 && iterations < SearchLimit && !ShouldStop())
            {
                var top = openHeap.Min;
                openHeap.Remove(top);
                int key = (top.Item3 << 16) + top.Item2;

                if (!openDict.TryGetValue(key, out currentNode))
                    continue;

                openDict.Remove(key);

                if (currentNode.IsClosed)
                    continue;

                if (currentNode.X == end.X && currentNode.Y == end.Y)
                {
                    found = true;
                    break;
                }

                currentNode.IsClosed = true;
                closedDict[key] = currentNode;
                iterations++;

                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = currentNode.X + dx[i];
                    int ny = currentNode.Y + dy[i];

                    if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                        continue;

                    var neighborCell = _grid[nx, ny];

                    // Block path through windows for SPPA
                    if (neighborCell.ElementType == MapElementType.Window)
                    {
                        RecordInvalidMove(new Point(nx, ny));
                        continue;  // منع المرور عبر النافذة
                    }

                    if (!neighborCell.IsWalkable)
                    {
                        RecordInvalidMove(new Point(nx, ny));
                        continue;
                    }

                    double stepCost = neighborCell.SurfaceWeight;

                    if (stepCost <= 0)
                        stepCost = 1;

                    if (IsDiagonalMove(currentNode.X, currentNode.Y, nx, ny) && HeavyDiagonals)
                        stepCost *= SQRT2;

                    int newG = currentNode.G + (int)stepCost;
                    var neighbor = nodes[nx, ny];
                    int neighborKey = (ny << 16) + nx;

                    if (closedDict.ContainsKey(neighborKey) && newG >= neighbor.G)
                        continue;

                    if (openDict.ContainsKey(neighborKey) && newG >= neighbor.G)
                        continue;

                    double obstacleCoeff = CalculateObstacleCoefficient(new Point(nx, ny));

                    neighbor.Parent = currentNode;
                    neighbor.G = newG;
                    neighbor.ObstacleCoeff = obstacleCoeff;
                    neighbor.H = CalculateHeuristic(new Point(nx, ny), end);
                    neighbor.IsClosed = false;

                    if (!openDict.ContainsKey(neighborKey))
                    {
                        openDict[neighborKey] = neighbor;
                        openHeap.Add(Tuple.Create(neighbor.F, neighbor.X, neighbor.Y));
                    }
                }
            }

            stopwatch.Stop();

            if (found && currentNode != null)
            {
                var path = ReconstructPath(currentNode);
                return new PathResult(path, stopwatch.Elapsed.TotalSeconds, iterations);
            }

            return PathResult.Fail($"No path found after exploring {iterations} nodes", stopwatch.Elapsed.TotalSeconds);
        }
        #endregion

        #region Private Methods
        private List<PathNode> ReconstructPath(SPPANode endNode)
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

        private double CalculateObstacleCoefficient(Point position)
        {
            int cacheKey = (position.Y << 16) + position.X;

            // Check cache first for performance
            if (_obstacleCoefficientCache.TryGetValue(cacheKey, out double cachedValue))
                return cachedValue;

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
            {
                dynamicCoeff = 1.0;
            }
            else
            {
                dynamicCoeff = CheckNearbyDynamicObstacles(position);
            }

            double weightedStatic = ALPHA_S * staticCoeff;
            double weightedSemiStatic = ALPHA_SS * semiStaticCoeff;
            double weightedDynamic = ALPHA_D * dynamicCoeff;

            double result = Math.Max(weightedStatic, Math.Max(weightedSemiStatic, weightedDynamic));

            // Store in cache
            _obstacleCoefficientCache[cacheKey] = result;

            return result;
        }

        private double CheckNearbyDynamicObstacles(Point position)
        {
            double maxInfluence = 0.0;

            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    int nx = position.X + dx;
                    int ny = position.Y + dy;

                    if (!_grid.IsValidCoordinate(nx, ny))
                        continue;

                    var cell = _grid[nx, ny];
                    if (cell.OccupyingObstacle != null)
                    {
                        double distance = Math.Sqrt(dx * dx + dy * dy);
                        double influence = 1.0 / (distance + 0.5);
                        maxInfluence = Math.Max(maxInfluence, influence * 0.5);
                    }
                }
            }

            return Math.Min(1.0, maxInfluence);
        }

        private bool IsDiagonalMove(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) == 1 && Math.Abs(y1 - y2) == 1;
        }

        private (int[] dx, int[] dy) GetMovementDirections()
        {
            if (AllowDiagonals)
            {
                return (new int[] { 0, 1, 0, -1, 1, 1, -1, -1 },
                    new int[] { -1, 0, 1, 0, -1, 1, 1, -1 });
            }
            else
            {
                return (new int[] { 0, 1, 0, -1 },
                    new int[] { -1, 0, 1, 0 });
            }
        }
        #endregion
    }
}
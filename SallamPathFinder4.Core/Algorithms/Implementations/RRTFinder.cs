#region File Header
/// <summary>
/// File: RRTFinder.cs
/// Description: RRT (Rapidly-exploring Random Tree) pathfinding algorithm for continuous spaces
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-21
/// Reference: LaValle, S. M. (1998). "Rapidly-exploring random trees: A new tool for path planning"
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
    /// RRT (Rapidly-exploring Random Tree) algorithm for path planning
    /// Suitable for continuous spaces and high-dimensional problems
    /// Not guaranteed to find optimal path, but very fast
    /// </summary>
    #endregion
    public sealed class RRTFinder : BasePathFinder
    {
        #region Constants
        private const int DEFAULT_MAX_ITERATIONS = 5000;
        private const double DEFAULT_STEP_SIZE = 1.0;
        private const double DEFAULT_GOAL_BIAS = 0.1;
        private const double DEFAULT_SEARCH_RADIUS = 5.0;
        private const int DEFAULT_MAX_NODES = 10000;
        #endregion

        #region Nested Types
        /// <summary>
        /// RRT Node structure
        /// </summary>
        private sealed class RRTNode
        {
            public int X, Y;
            public RRTNode Parent;
            public double Cost;
            public double DistanceFromStart;

            public RRTNode(int x, int y)
            {
                X = x;
                Y = y;
                Parent = null;
                Cost = 0;
                DistanceFromStart = 0;
            }

            public RRTNode(int x, int y, RRTNode parent, double cost)
            {
                X = x;
                Y = y;
                Parent = parent;
                Cost = cost;
                DistanceFromStart = parent != null ? parent.DistanceFromStart + 1 : 0;
            }

            public Point ToPoint() => new Point(X, Y);
        }

        /// <summary>
        /// Configuration for RRT algorithm
        /// </summary>
        public sealed class RRTConfig
        {
            public int MaxIterations { get; set; } = DEFAULT_MAX_ITERATIONS;
            public double StepSize { get; set; } = DEFAULT_STEP_SIZE;
            public double GoalBias { get; set; } = DEFAULT_GOAL_BIAS;
            public double SearchRadius { get; set; } = DEFAULT_SEARCH_RADIUS;
            public int MaxNodes { get; set; } = DEFAULT_MAX_NODES;
            public bool UseRRTStar { get; set; } = true;  // RRT* optimization
            public bool SmoothPath { get; set; } = true;   // Path smoothing
            public bool Bidirectional { get; set; } = false; // Bidirectional RRT
        }
        #endregion

        #region Private Fields
        private readonly Random _random;
        private RRTConfig _config;
        private List<RRTNode> _nodes;
        private HashSet<Point> _nodePositions;
        private Point _start;
        private Point _goal;
        #endregion

        #region Constructor
        public RRTFinder(MapGrid grid) : base(grid)
        {
            _random = new Random();
            _config = new RRTConfig();
            _nodes = new List<RRTNode>();
            _nodePositions = new HashSet<Point>();

            // Default settings
            SearchLimit = _config.MaxIterations;
            AllowDiagonals = true;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the RRT configuration
        /// </summary>
        public RRTConfig Configuration
        {
            get => _config;
            set => _config = value ?? new RRTConfig();
        }

        /// <summary>
        /// Gets the number of nodes in the tree
        /// </summary>
        public int NodeCount => _nodes.Count;

        /// <summary>
        /// Gets whether RRT* optimization is enabled
        /// </summary>
        public bool UseRRTStar
        {
            get => _config.UseRRTStar;
            set => _config.UseRRTStar = value;
        }

        /// <summary>
        /// Gets whether bidirectional search is enabled
        /// </summary>
        public bool UseBidirectional
        {
            get => _config.Bidirectional;
            set => _config.Bidirectional = value;
        }
        #endregion

        #region Public Methods - Configuration
        /// <summary>
        /// Sets RRT parameters
        /// </summary>
        public void SetParameters(int maxIterations, double stepSize, double goalBias, bool useRRTStar = true)
        {
            _config.MaxIterations = Math.Max(100, Math.Min(50000, maxIterations));
            _config.StepSize = Math.Max(0.5, Math.Min(10.0, stepSize));
            _config.GoalBias = Math.Max(0.0, Math.Min(1.0, goalBias));
            _config.UseRRTStar = useRRTStar;
            SearchLimit = _config.MaxIterations;
        }

        /// <summary>
        /// Sets RRT* parameters
        /// </summary>
        public void SetRRTStarParameters(double searchRadius)
        {
            _config.SearchRadius = Math.Max(1.0, Math.Min(20.0, searchRadius));
            _config.UseRRTStar = true;
        }
        #endregion

        #region Public Methods - Pathfinding
        /// <inheritdoc/>
        public override PathResult FindPath(Point start, Point end)
        {
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

            try
            {
                _start = start;
                _goal = end;
                _nodes.Clear();
                _nodePositions.Clear();

                if (_config.Bidirectional)
                {
                    return FindPathBidirectional(start, end, stopwatch);
                }
                else
                {
                    return FindPathUnidirectional(start, end, stopwatch);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return PathResult.Fail($"RRT error: {ex.Message}", stopwatch.Elapsed.TotalSeconds);
            }
        }
        #endregion

        #region Private Methods - Unidirectional RRT
        private PathResult FindPathUnidirectional(Point start, Point end, System.Diagnostics.Stopwatch stopwatch)
        {
            // Initialize tree with start node
            var startNode = new RRTNode(start.X, start.Y);
            _nodes.Add(startNode);
            _nodePositions.Add(start);

            RRTNode goalNode = null;
            int iterations = 0;

            for (int i = 0; i < _config.MaxIterations && !ShouldStop(); i++)
            {
                iterations++;

                // Sample random point (with goal bias)
                Point randomPoint = SamplePoint();

                // Find nearest node in tree
                RRTNode nearestNode = FindNearestNode(randomPoint);

                // Steer from nearest node toward random point
                Point newPoint = Steer(nearestNode.ToPoint(), randomPoint);

                // Check if new point is valid
                if (!IsValidPoint(newPoint))
                {
                    RecordInvalidMove(newPoint);
                    continue;
                }

                // Check if path to new point is collision-free
                if (!IsPathClear(nearestNode.ToPoint(), newPoint))
                {
                    continue;
                }

                // Create new node
                double stepCost = CalculateStepCost(nearestNode.ToPoint(), newPoint);
                var newNode = new RRTNode(newPoint.X, newPoint.Y, nearestNode, nearestNode.Cost + stepCost);

                // RRT* optimization: rewire nearby nodes
                if (_config.UseRRTStar)
                {
                    RewireNearbyNodes(newNode);
                }

                _nodes.Add(newNode);
                // Open node - new node added to tree
                RaiseDebugEvent(nearestNode.X, nearestNode.Y, newPoint.X, newPoint.Y, PathFinderNodeType.Open, 0, 0);

                _nodePositions.Add(newPoint);

                // Current node - currently expanding node
                RaiseDebugEvent(nearestNode.X, nearestNode.Y, newPoint.X, newPoint.Y, PathFinderNodeType.Current, 0, 0);
                // Check if reached goal
                if (IsReachedGoal(newPoint))
                {
                    goalNode = newNode;
                    break;
                }
            }

            stopwatch.Stop();

            if (goalNode != null)
            {
                var path = ReconstructPath(goalNode);
                // Path nodes - final path
                foreach (var node in path)
                {
                    RaiseDebugEvent(node.X, node.Y, node.X, node.Y, PathFinderNodeType.Path, 0, 0);
                }
                // Smooth path if enabled
                if (_config.SmoothPath && path.Count > 2)
                {
                    path = SmoothPath(path);
                }

                return new PathResult(path, stopwatch.Elapsed.TotalSeconds, iterations);
            }

            return PathResult.Fail($"No path found after {iterations} iterations", stopwatch.Elapsed.TotalSeconds);
        }
        #endregion

        #region Private Methods - Bidirectional RRT
        private PathResult FindPathBidirectional(Point start, Point end, System.Diagnostics.Stopwatch stopwatch)
        {
            // Initialize two trees: one from start, one from goal
            var treeStart = new List<RRTNode> { new RRTNode(start.X, start.Y) };
            var treeGoal = new List<RRTNode> { new RRTNode(end.X, end.Y) };
            var positionsStart = new HashSet<Point> { start };
            var positionsGoal = new HashSet<Point> { end };

            RRTNode connectionNode = null;
            bool connectedFromStart = false;
            int iterations = 0;

            for (int i = 0; i < _config.MaxIterations && !ShouldStop(); i++)
            {
                iterations++;

                // Sample random point
                Point randomPoint = SamplePoint();

                // Expand start tree
                var newNodeStart = ExpandTree(treeStart, positionsStart, randomPoint);
                if (newNodeStart != null)
                {
                    // Check for connection with goal tree
                    var nearestInGoal = FindNearestNodeInList(treeGoal, newNodeStart.ToPoint());
                    if (nearestInGoal != null)
                    {
                        double distance = EuclideanDistance(newNodeStart.ToPoint(), nearestInGoal.ToPoint());
                        if (distance <= _config.StepSize && IsPathClear(newNodeStart.ToPoint(), nearestInGoal.ToPoint()))
                        {
                            connectionNode = newNodeStart;
                            connectedFromStart = true;
                            break;
                        }
                    }
                }

                // Expand goal tree
                var newNodeGoal = ExpandTree(treeGoal, positionsGoal, randomPoint);
                if (newNodeGoal != null)
                {
                    // Check for connection with start tree
                    var nearestInStart = FindNearestNodeInList(treeStart, newNodeGoal.ToPoint());
                    if (nearestInStart != null)
                    {
                        double distance = EuclideanDistance(newNodeGoal.ToPoint(), nearestInStart.ToPoint());
                        if (distance <= _config.StepSize && IsPathClear(newNodeGoal.ToPoint(), nearestInStart.ToPoint()))
                        {
                            connectionNode = newNodeGoal;
                            connectedFromStart = false;
                            break;
                        }
                    }
                }
            }

            stopwatch.Stop();

            if (connectionNode != null)
            {
                var path = ReconstructBidirectionalPath(treeStart, treeGoal, connectionNode, connectedFromStart);

                if (_config.SmoothPath && path.Count > 2)
                {
                    path = SmoothPath(path);
                }

                return new PathResult(path, stopwatch.Elapsed.TotalSeconds, iterations);
            }

            return PathResult.Fail($"No path found after {iterations} iterations", stopwatch.Elapsed.TotalSeconds);
        }

        private RRTNode ExpandTree(List<RRTNode> tree, HashSet<Point> positions, Point target)
        {
            var nearest = FindNearestNodeInList(tree, target);
            if (nearest == null) return null;

            Point newPoint = Steer(nearest.ToPoint(), target);

            if (!IsValidPoint(newPoint) || positions.Contains(newPoint))
                return null;

            if (!IsPathClear(nearest.ToPoint(), newPoint))
                return null;

            double stepCost = CalculateStepCost(nearest.ToPoint(), newPoint);
            var newNode = new RRTNode(newPoint.X, newPoint.Y, nearest, nearest.Cost + stepCost);

            tree.Add(newNode);
            positions.Add(newPoint);

            return newNode;
        }

        private RRTNode FindNearestNodeInList(List<RRTNode> tree, Point point)
        {
            RRTNode nearest = null;
            double minDistance = double.MaxValue;

            foreach (var node in tree)
            {
                double distance = EuclideanDistance(node.ToPoint(), point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = node;
                }
            }

            return nearest;
        }

        private List<PathNode> ReconstructBidirectionalPath(
            List<RRTNode> treeStart,
            List<RRTNode> treeGoal,
            RRTNode connectionNode,
            bool fromStart)
        {
            var path = new List<PathNode>();

            // Add path from start to connection
            var current = connectionNode;
            while (current != null)
            {
                path.Insert(0, new PathNode(current.X, current.Y));
                current = current.Parent;
            }

            // Find connecting node in other tree
            Point connectionPoint = connectionNode.ToPoint();
            var connectingNode = FindNearestNodeInList(fromStart ? treeGoal : treeStart, connectionPoint);

            // Add path from connection to goal
            var tempPath = new List<PathNode>();
            current = connectingNode;
            while (current != null)
            {
                tempPath.Insert(0, new PathNode(current.X, current.Y));
                current = current.Parent;
            }

            // Add without duplicating connection point
            for (int i = 1; i < tempPath.Count; i++)
            {
                path.Add(tempPath[i]);
            }

            return path;
        }
        #endregion

        #region Private Methods - RRT* Rewiring
        private void RewireNearbyNodes(RRTNode newNode)
        {
            // Find nearby nodes within search radius
            var nearbyNodes = new List<RRTNode>();
            double radiusSquared = _config.SearchRadius * _config.SearchRadius;

            foreach (var node in _nodes)
            {
                double dx = node.X - newNode.X;
                double dy = node.Y - newNode.Y;
                if (dx * dx + dy * dy <= radiusSquared)
                {
                    nearbyNodes.Add(node);
                }
            }

            // Try to rewire nearby nodes to new node (if cheaper)
            foreach (var node in nearbyNodes)
            {
                if (node == newNode) continue;

                double newCost = newNode.Cost + EuclideanDistance(newNode.ToPoint(), node.ToPoint());
                if (newCost < node.Cost && IsPathClear(newNode.ToPoint(), node.ToPoint()))
                {
                    node.Parent = newNode;
                    node.Cost = newCost;
                    // Close node - rewired node
                    RaiseDebugEvent(newNode.X, newNode.Y, node.X, node.Y, PathFinderNodeType.Close, 0, 0);
                }
            }

            // Try to rewire new node to nearby nodes (if cheaper)
            foreach (var node in nearbyNodes)
            {
                if (node == newNode) continue;

                double newCost = node.Cost + EuclideanDistance(node.ToPoint(), newNode.ToPoint());
                if (newCost < newNode.Cost && IsPathClear(node.ToPoint(), newNode.ToPoint()))
                {
                    newNode.Parent = node;
                    newNode.Cost = newCost;
                    break;
                }
            }
        }
        #endregion

        #region Private Methods - Helper Functions
        private Point SamplePoint()
        {
            // Goal bias: sometimes sample the goal directly
            if (_random.NextDouble() < _config.GoalBias)
            {
                return _goal;
            }

            // Otherwise sample random point within grid bounds
            int x = _random.Next(0, _grid.Width);
            int y = _random.Next(0, _grid.Height);
            return new Point(x, y);
        }

        private RRTNode FindNearestNode(Point point)
        {
            RRTNode nearest = null;
            double minDistance = double.MaxValue;

            foreach (var node in _nodes)
            {
                double distance = EuclideanDistance(node.ToPoint(), point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = node;
                }
            }

            return nearest;
        }

        private Point Steer(Point from, Point to)
        {
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance <= _config.StepSize)
            {
                return to;
            }

            double ratio = _config.StepSize / distance;
            int newX = from.X + (int)Math.Round(dx * ratio);
            int newY = from.Y + (int)Math.Round(dy * ratio);

            // Clamp to grid bounds
            newX = Math.Max(0, Math.Min(_grid.Width - 1, newX));
            newY = Math.Max(0, Math.Min(_grid.Height - 1, newY));

            return new Point(newX, newY);
        }

        private bool IsValidPoint(Point point)
        {
            if (!_grid.IsValidCoordinate(point.X, point.Y))
                return false;

            var cell = _grid[point.X, point.Y];
            return cell.IsWalkable && cell.OccupyingObstacle == null;
        }

        private bool IsPathClear(Point from, Point to)
        {
            // Bresenham line algorithm to check all cells along the path
            var points = GetLinePoints(from, to);

            foreach (var point in points)
            {
                if (!_grid.IsValidCoordinate(point.X, point.Y))
                    return false;

                var cell = _grid[point.X, point.Y];
                if (!cell.IsWalkable || cell.OccupyingObstacle != null)
                {
                    RecordInvalidMove(point);
                    return false;
                }
            }

            return true;
        }

        private List<Point> GetLinePoints(Point from, Point to)
        {
            var points = new List<Point>();

            int x0 = from.X;
            int y0 = from.Y;
            int x1 = to.X;
            int y1 = to.Y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                points.Add(new Point(x0, y0));

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return points;
        }

        private bool IsReachedGoal(Point point)
        {
            return EuclideanDistance(point, _goal) <= _config.StepSize;
        }

        private double EuclideanDistance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private List<PathNode> ReconstructPath(RRTNode endNode)
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

        private List<PathNode> SmoothPath(List<PathNode> path)
        {
            if (path.Count < 3) return path;

            var smoothed = new List<PathNode>(path);
            bool improved = true;
            int maxIterations = 100;
            int iteration = 0;

            while (improved && iteration < maxIterations)
            {
                improved = false;
                iteration++;

                for (int i = 0; i < smoothed.Count - 2; i++)
                {
                    for (int j = i + 2; j < smoothed.Count; j++)
                    {
                        Point from = new Point(smoothed[i].X, smoothed[i].Y);
                        Point to = new Point(smoothed[j].X, smoothed[j].Y);

                        if (IsPathClear(from, to))
                        {
                            // Remove intermediate points
                            int removeCount = j - i - 1;
                            smoothed.RemoveRange(i + 1, removeCount);
                            improved = true;
                            break;
                        }
                    }
                    if (improved) break;
                }
            }

            return smoothed;
        }
        #endregion

        #region Private Methods - Cost Calculation
        private double CalculateStepCost(Point from, Point to)
        {
            var cell = _grid[to.X, to.Y];
            double cost = cell.SurfaceWeight;

            if (cost <= 0) cost = 1;

            // Add penalty for diagonal movement
            if (Math.Abs(from.X - to.X) == 1 && Math.Abs(from.Y - to.Y) == 1 && HeavyDiagonals)
            {
                cost *= 1.414; // √2
            }

            return cost;
        }
        #endregion
    }
}
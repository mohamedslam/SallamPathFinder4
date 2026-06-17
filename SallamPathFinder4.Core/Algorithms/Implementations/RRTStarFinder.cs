#region File Header
/// <summary>
/// File: RRTStarFinder.cs
/// Description: RRT* (RRT-Star) pathfinding algorithm - asymptotically optimal version of RRT
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-21
/// Reference: Karaman, S., & Frazzoli, E. (2011). "Sampling-based algorithms for optimal motion planning"
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
    /// RRT* (RRT-Star) algorithm
    /// Asymptotically optimal version of RRT with rewiring and near-neighbor search
    /// Guarantees convergence to optimal solution as number of samples increases
    /// </summary>
    #endregion
    public sealed class RRTStarFinder : BasePathFinder
    {
        #region Constants
        private const int DEFAULT_MAX_ITERATIONS = 5000;
        private const double DEFAULT_STEP_SIZE = 1.0;
        private const double DEFAULT_GOAL_BIAS = 0.1;
        private const double DEFAULT_REWIRING_RADIUS = 10.0;
        private const double DEFAULT_GOAL_SAMPLE_RADIUS = 2.0;
        private const int DEFAULT_MAX_NODES = 10000;
        #endregion

        #region Nested Types
        /// <summary>
        /// RRT* Node structure
        /// </summary>
        private sealed class RRTStarNode
        {
            public int X, Y;
            public RRTStarNode Parent;
            public double Cost;
            public List<RRTStarNode> Children;
            public double DistanceFromStart;

            public RRTStarNode(int x, int y)
            {
                X = x;
                Y = y;
                Parent = null;
                Cost = 0;
                Children = new List<RRTStarNode>();
                DistanceFromStart = 0;
            }

            public RRTStarNode(int x, int y, RRTStarNode parent, double cost)
            {
                X = x;
                Y = y;
                Parent = parent;
                Cost = cost;
                Children = new List<RRTStarNode>();
                DistanceFromStart = parent != null ? parent.DistanceFromStart + 1 : 0;

                if (parent != null)
                {
                    parent.Children.Add(this);
                }
            }

            public Point ToPoint() => new Point(X, Y);
        }

        /// <summary>
        /// RRT* Configuration
        /// </summary>
        public sealed class RRTStarConfig
        {
            public int MaxIterations { get; set; } = DEFAULT_MAX_ITERATIONS;
            public double StepSize { get; set; } = DEFAULT_STEP_SIZE;
            public double GoalBias { get; set; } = DEFAULT_GOAL_BIAS;
            public double RewiringRadius { get; set; } = DEFAULT_REWIRING_RADIUS;
            public double GoalSampleRadius { get; set; } = DEFAULT_GOAL_SAMPLE_RADIUS;
            public int MaxNodes { get; set; } = DEFAULT_MAX_NODES;
            public bool UseInformedSampling { get; set; } = true;
            public bool SmoothPath { get; set; } = true;
            public bool Bidirectional { get; set; } = false;
        }
        #endregion

        #region Private Fields
        private readonly Random _random;
        private RRTStarConfig _config;
        private List<RRTStarNode> _nodes;
        private Dictionary<Point, RRTStarNode> _nodeMap;
        private Point _start;
        private Point _goal;
        private RRTStarNode _goalNode;
        private double _bestPathCost;
        private List<PathNode> _bestPath;
        #endregion

        #region Constructor
        public RRTStarFinder(MapGrid grid) : base(grid)
        {
            _random = new Random(42);
            _config = new RRTStarConfig();
            _nodes = new List<RRTStarNode>();
            _nodeMap = new Dictionary<Point, RRTStarNode>();
            _bestPathCost = double.MaxValue;
            _bestPath = new List<PathNode>();

            SearchLimit = _config.MaxIterations;
            AllowDiagonals = true;
        }
        #endregion

        #region Public Properties
        public RRTStarConfig Configuration
        {
            get => _config;
            set => _config = value ?? new RRTStarConfig();
        }

        public int NodeCount => _nodes.Count;
        public double BestPathCost => _bestPathCost;
        public bool HasPath => _bestPath.Count > 0;
        #endregion

        #region Public Methods - Configuration
        public void SetParameters(int maxIterations, double stepSize, double goalBias, double rewiringRadius)
        {
            _config.MaxIterations = Math.Max(100, Math.Min(50000, maxIterations));
            _config.StepSize = Math.Max(0.5, Math.Min(10.0, stepSize));
            _config.GoalBias = Math.Max(0.0, Math.Min(1.0, goalBias));
            _config.RewiringRadius = Math.Max(1.0, Math.Min(50.0, rewiringRadius));
            SearchLimit = _config.MaxIterations;
        }

        public void ClearTree()
        {
            _nodes.Clear();
            _nodeMap.Clear();
            _bestPathCost = double.MaxValue;
            _bestPath.Clear();
            _goalNode = null;
        }
        #endregion

        #region Public Methods - Pathfinding
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
                ClearTree();

                // Initialize tree with start node
                var startNode = new RRTStarNode(start.X, start.Y);
                _nodes.Add(startNode);
                _nodeMap[start] = startNode;

                for (int i = 0; i < _config.MaxIterations && !ShouldStop(); i++)
                {
                    // Sample random point (with goal bias)
                    Point randomPoint = SamplePoint();

                    // Find nearest node in tree
                    RRTStarNode nearestNode = FindNearestNode(randomPoint);

                    // Steer toward random point
                    Point newPoint = Steer(nearestNode.ToPoint(), randomPoint);

                    // Validate new point
                    if (!IsValidPoint(newPoint))
                    {
                        RecordInvalidMove(newPoint);
                        continue;
                    }

                    // Check path collision
                    if (!IsPathClear(nearestNode.ToPoint(), newPoint))
                    {
                        continue;
                    }

                    // Create new node
                    double stepCost = CalculateStepCost(nearestNode.ToPoint(), newPoint);
                    var newNode = new RRTStarNode(newPoint.X, newPoint.Y, nearestNode, nearestNode.Cost + stepCost);
                    RaiseDebugEvent(nearestNode.X, nearestNode.Y, newPoint.X, newPoint.Y, PathFinderNodeType.Open, 0, 0);

                    // RRT*: Find near neighbors for rewiring
                    var nearNodes = FindNearNodes(newNode);

                    // RRT*: Choose best parent (minimum cost)
                    RRTStarNode bestParent = ChooseBestParent(newNode, nearNodes);
                    if (bestParent != null && bestParent != nearestNode)
                    {
                        // Re-parent to best parent
                        nearestNode.Children.Remove(newNode);
                        newNode.Parent = bestParent;
                        newNode.Cost = bestParent.Cost + CalculateStepCost(bestParent.ToPoint(), newNode.ToPoint());
                        bestParent.Children.Add(newNode);
                    }

                    _nodes.Add(newNode);
                    // Open node - new node added to tree
                    _nodeMap[newPoint] = newNode;

                    // RRT*: Rewire near nodes
                    RewireNearNodes(newNode, nearNodes);
                    // Current node - currently expanding node
                    RaiseDebugEvent(nearestNode.X, nearestNode.Y, newPoint.X, newPoint.Y, PathFinderNodeType.Current, 0, 0);
                    // Check if reached goal
                    if (IsReachedGoal(newPoint))
                    {
                        _goalNode = newNode;
                        double currentCost = newNode.Cost;

                        if (currentCost < _bestPathCost)
                        {
                            _bestPathCost = currentCost;
                            _bestPath = ReconstructPath(newNode);
                            // Path nodes - updated best path
                            foreach (var node in _bestPath)
                            {
                                RaiseDebugEvent(node.X, node.Y, node.X, node.Y, PathFinderNodeType.Path, 0, 0);
                            }
                            System.Diagnostics.Debug.WriteLine($"RRT* found better path: Cost = {_bestPathCost:F2}, Nodes = {_nodes.Count}");

                            // Update sampling region for informed RRT*
                            if (_config.UseInformedSampling && _bestPathCost < double.MaxValue)
                            {
                                _config.RewiringRadius = CalculateInformedRadius();
                            }
                        }
                    }

                    // Debug output every 500 iterations
                    if (i % 500 == 0 && _bestPathCost < double.MaxValue)
                    {
                        System.Diagnostics.Debug.WriteLine($"RRT* Iteration {i}: Best Cost = {_bestPathCost:F2}, Nodes = {_nodes.Count}");
                    }
                }

                stopwatch.Stop();

                if (_bestPath.Count > 0)
                {
                    var path = _bestPath;
                    if (_config.SmoothPath && path.Count > 2)
                    {
                        path = SmoothPath(path);
                    }
                    // Final path visualization (in case it wasn't updated during search)
                    foreach (var node in path)
                    {
                        RaiseDebugEvent(node.X, node.Y, node.X, node.Y, PathFinderNodeType.Path, 0, 0);
                    }
                    return new PathResult(path, stopwatch.Elapsed.TotalSeconds, _nodes.Count);
                }

                return PathResult.Fail($"No path found after {_nodes.Count} nodes", stopwatch.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return PathResult.Fail($"RRT* error: {ex.Message}", stopwatch.Elapsed.TotalSeconds);
            }
        }
        #endregion

        #region Private Methods - Sampling
        private Point SamplePoint()
        {
            // Informed sampling: sample within ellipse if a solution exists
            if (_config.UseInformedSampling && _bestPathCost < double.MaxValue)
            {
                return SampleInEllipse();
            }

            // Goal bias
            if (_random.NextDouble() < _config.GoalBias)
            {
                return _goal;
            }

            int x = _random.Next(0, _grid.Width);
            int y = _random.Next(0, _grid.Height);
            return new Point(x, y);
        }

        private Point SampleInEllipse()
        {
            // Sample within ellipse defined by start, goal, and current best cost
            double cBest = _bestPathCost;
            double cMin = EuclideanDistance(_start, _goal);

            if (cBest <= cMin)
                return _goal;

            double a = cBest / 2;
            double c = cMin / 2;
            double b = Math.Sqrt(a * a - c * c);

            // Sample uniformly in ellipse
            double theta = _random.NextDouble() * 2 * Math.PI;
            double r = _random.NextDouble();
            double x = a * Math.Sqrt(r) * Math.Cos(theta);
            double y = b * Math.Sqrt(r) * Math.Sin(theta);

            // Rotate and translate to align with start-goal line
            double angle = Math.Atan2(_goal.Y - _start.Y, _goal.X - _start.X);
            double centerX = (_start.X + _goal.X) / 2.0;
            double centerY = (_start.Y + _goal.Y) / 2.0;

            int sampleX = (int)(centerX + x * Math.Cos(angle) - y * Math.Sin(angle));
            int sampleY = (int)(centerY + x * Math.Sin(angle) + y * Math.Cos(angle));

            sampleX = Math.Max(0, Math.Min(_grid.Width - 1, sampleX));
            sampleY = Math.Max(0, Math.Min(_grid.Height - 1, sampleY));

            return new Point(sampleX, sampleY);
        }
        #endregion

        #region Private Methods - Tree Operations
        private RRTStarNode FindNearestNode(Point point)
        {
            RRTStarNode nearest = null;
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

        private List<RRTStarNode> FindNearNodes(RRTStarNode node)
        {
            var nearNodes = new List<RRTStarNode>();
            double radius = _config.RewiringRadius;

            // Adaptive radius based on number of nodes
            double adaptiveRadius = radius * Math.Sqrt(Math.Log(_nodes.Count) / _nodes.Count);

            foreach (var other in _nodes)
            {
                if (other == node) continue;

                double distance = EuclideanDistance(node.ToPoint(), other.ToPoint());
                if (distance <= adaptiveRadius)
                {
                    nearNodes.Add(other);
                }
            }

            return nearNodes;
        }

        private RRTStarNode ChooseBestParent(RRTStarNode newNode, List<RRTStarNode> nearNodes)
        {
            RRTStarNode bestParent = newNode.Parent;
            double minCost = newNode.Cost;

            foreach (var node in nearNodes)
            {
                double distance = EuclideanDistance(node.ToPoint(), newNode.ToPoint());
                double potentialCost = node.Cost + distance;

                if (potentialCost < minCost && IsPathClear(node.ToPoint(), newNode.ToPoint()))
                {
                    minCost = potentialCost;
                    bestParent = node;
                    // Close node - better parent found, old parent replaced
                    RaiseDebugEvent(node.X, node.Y, newNode.X, newNode.Y, PathFinderNodeType.Close, 0, 0);
                }
            }

            return bestParent;
        }

        private void RewireNearNodes(RRTStarNode newNode, List<RRTStarNode> nearNodes)
        {
            foreach (var node in nearNodes)
            {
                if (node == newNode.Parent) continue;

                double distance = EuclideanDistance(newNode.ToPoint(), node.ToPoint());
                double potentialCost = newNode.Cost + distance;

                if (potentialCost < node.Cost && IsPathClear(newNode.ToPoint(), node.ToPoint()))
                {
                    // Rewire: make newNode the parent of node
                    node.Parent.Children.Remove(node);
                    node.Parent = newNode;
                    node.Cost = potentialCost;
                    newNode.Children.Add(node);
                    // Rewire - improved connection
                    RaiseDebugEvent(newNode.X, newNode.Y, node.X, node.Y, PathFinderNodeType.Close, 0, 0);
                    // Update costs of all descendants
                    UpdateDescendantCosts(node);
                }
            }
        }

        private void UpdateDescendantCosts(RRTStarNode node)
        {
            foreach (var child in node.Children)
            {
                double newCost = node.Cost + EuclideanDistance(node.ToPoint(), child.ToPoint());
                if (newCost < child.Cost)
                {
                    child.Cost = newCost;
                    UpdateDescendantCosts(child);
                }
            }
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

            newX = Math.Max(0, Math.Min(_grid.Width - 1, newX));
            newY = Math.Max(0, Math.Min(_grid.Height - 1, newY));

            return new Point(newX, newY);
        }

        private List<PathNode> ReconstructPath(RRTStarNode endNode)
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

        #region Private Methods - Validation
        private bool IsValidPoint(Point point)
        {
            if (!_grid.IsValidCoordinate(point.X, point.Y))
                return false;

            var cell = _grid[point.X, point.Y];
            return cell.IsWalkable && cell.OccupyingObstacle == null;
        }

        private bool IsPathClear(Point from, Point to)
        {
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

        private bool IsReachedGoal(Point point)
        {
            return EuclideanDistance(point, _goal) <= _config.GoalSampleRadius;
        }
        #endregion

        #region Private Methods - Helper Functions
        private double CalculateStepCost(Point from, Point to)
        {
            var cell = _grid[to.X, to.Y];
            double cost = cell.SurfaceWeight;

            if (cost <= 0) cost = 1;

            if (Math.Abs(from.X - to.X) == 1 && Math.Abs(from.Y - to.Y) == 1 && HeavyDiagonals)
            {
                cost *= 1.414;
            }

            return cost;
        }

        private double EuclideanDistance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private double CalculateInformedRadius()
        {
            // Adaptive radius for informed sampling
            double cBest = _bestPathCost;
            double cMin = EuclideanDistance(_start, _goal);

            if (cBest <= cMin)
                return _config.RewiringRadius;

            double a = cBest / 2;
            double c = cMin / 2;
            double b = Math.Sqrt(a * a - c * c);

            return Math.Max(_config.StepSize, b);
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
    }
}
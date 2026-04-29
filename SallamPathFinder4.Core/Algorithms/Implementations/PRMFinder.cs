#region File Header
/// <summary>
/// File: PRMFinder.cs
/// Description: PRM (Probabilistic Roadmap) pathfinding algorithm for multi-query path planning
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-21
/// Reference: Kavraki, L. E., et al. (1996). "Probabilistic Roadmaps for Path Planning in High-Dimensional Configuration Spaces"
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
    /// PRM (Probabilistic Roadmap) algorithm
    /// Suitable for multi-query path planning in static environments
    /// Phases: Learning (sampling + connection) + Query (graph search)
    /// </summary>
    #endregion
    public sealed class PRMFinder : BasePathFinder
    {
        #region Constants
        private const int DEFAULT_NUM_SAMPLES = 500;
        private const double DEFAULT_CONNECTION_RADIUS = 10.0;
        private const int DEFAULT_MAX_NEIGHBORS = 15;
        private const double DEFAULT_SAMPLE_BIAS = 0.1;
        #endregion

        #region Nested Types
        /// <summary>
        /// PRM Node structure
        /// </summary>
        private sealed class PRMNode
        {
            public int X, Y;
            public int Index;
            public List<int> Neighbors;
            public double CostFromStart;

            public PRMNode(int x, int y, int index)
            {
                X = x;
                Y = y;
                Index = index;
                Neighbors = new List<int>();
                CostFromStart = double.MaxValue;
            }

            public Point ToPoint() => new Point(X, Y);
        }

        /// <summary>
        /// PRM Configuration
        /// </summary>
        public sealed class PRMConfig
        {
            public int NumSamples { get; set; } = DEFAULT_NUM_SAMPLES;
            public double ConnectionRadius { get; set; } = DEFAULT_CONNECTION_RADIUS;
            public int MaxNeighbors { get; set; } = DEFAULT_MAX_NEIGHBORS;
            public double SampleBias { get; set; } = DEFAULT_SAMPLE_BIAS;
            public bool UseKDTree { get; set; } = true;
            public bool LazyCollisionCheck { get; set; } = false;
        }
        #endregion

        #region Private Fields
        private readonly Random _random;
        private PRMConfig _config;
        private List<PRMNode> _nodes;
        private Dictionary<Point, int> _nodePositionMap;
        private bool _roadmapBuilt;
        #endregion

        #region Constructor
        public PRMFinder(MapGrid grid) : base(grid)
        {
            _random = new Random();
            _config = new PRMConfig();
            _nodes = new List<PRMNode>();
            _nodePositionMap = new Dictionary<Point, int>();
            _roadmapBuilt = false;

            SearchLimit = _config.NumSamples;
            AllowDiagonals = true;
        }
        #endregion

        #region Public Properties
        public PRMConfig Configuration
        {
            get => _config;
            set => _config = value ?? new PRMConfig();
        }

        public int NodeCount => _nodes.Count;
        public bool RoadmapBuilt => _roadmapBuilt;
        #endregion

        #region Public Methods - Configuration
        public void SetParameters(int numSamples, double connectionRadius, double sampleBias = 0.1)
        {
            _config.NumSamples = Math.Max(50, Math.Min(5000, numSamples));
            _config.ConnectionRadius = Math.Max(1.0, Math.Min(50.0, connectionRadius));
            _config.SampleBias = Math.Max(0.0, Math.Min(1.0, sampleBias));
            _roadmapBuilt = false;
            SearchLimit = _config.NumSamples;
        }

        public void ClearRoadmap()
        {
            _nodes.Clear();
            _nodePositionMap.Clear();
            _roadmapBuilt = false;
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
                // Build roadmap if not already built or if environment changed
                if (!_roadmapBuilt)
                {
                    BuildRoadmap();
                }

                // Add start and goal to roadmap
                AddStartAndGoal(start, end);

                // Find path using Dijkstra/A*
                var path = FindPathOnRoadmap(start, end);

                // Remove temporary nodes
                RemoveStartAndGoal();

                stopwatch.Stop();

                if (path != null && path.Count > 0)
                {
                    // Smooth path if enabled
                    if (_config.LazyCollisionCheck)
                    {
                        path = SmoothPath(path);
                    }
                    return new PathResult(path, stopwatch.Elapsed.TotalSeconds, _nodes.Count);
                }

                return PathResult.Fail("No path found in roadmap", stopwatch.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return PathResult.Fail($"PRM error: {ex.Message}", stopwatch.Elapsed.TotalSeconds);
            }
        }
        #endregion

        #region Private Methods - Roadmap Building
        private void BuildRoadmap()
        {
            _nodes.Clear();
            _nodePositionMap.Clear();

            // Phase 1: Sampling
            SampleNodes();

            // Phase 2: Connection
            ConnectNodes();

            _roadmapBuilt = true;
            System.Diagnostics.Debug.WriteLine($"PRM roadmap built: {_nodes.Count} nodes, {GetTotalEdges()} edges");
        }

        private void SampleNodes()
        {
            int samplesToGenerate = _config.NumSamples;

            for (int i = 0; i < samplesToGenerate; i++)
            {
                Point sample = SamplePoint();

                if (!IsValidNodePosition(sample))
                {
                    continue;
                }

                var node = new PRMNode(sample.X, sample.Y, _nodes.Count);
                _nodes.Add(node);
                _nodePositionMap[sample] = node.Index;
                // Open node - new sample point added to roadmap
                RaiseDebugEvent(sample.X, sample.Y, sample.X, sample.Y, PathFinderNodeType.Open, 0, 0);
            }

            System.Diagnostics.Debug.WriteLine($"PRM sampling: Generated {_nodes.Count} valid nodes");
        }

        private Point SamplePoint()
        {
            // Bias sampling toward areas near obstacles for better coverage
            if (_random.NextDouble() < _config.SampleBias)
            {
                return SampleNearObstacle();
            }

            int x = _random.Next(0, _grid.Width);
            int y = _random.Next(0, _grid.Height);
            return new Point(x, y);
        }

        private Point SampleNearObstacle()
        {
            // Sample near obstacles to improve narrow passage coverage
            int maxAttempts = 50;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                int x = _random.Next(0, _grid.Width);
                int y = _random.Next(0, _grid.Height);

                if (IsNearObstacle(x, y))
                {
                    return new Point(x, y);
                }
            }

            return new Point(_random.Next(0, _grid.Width), _random.Next(0, _grid.Height));
        }

        private bool IsNearObstacle(int x, int y)
        {
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (!_grid.IsValidCoordinate(nx, ny))
                        continue;

                    if (!_grid[nx, ny].IsWalkable)
                        return true;
                }
            }
            return false;
        }

        private bool IsValidNodePosition(Point point)
        {
            if (!_grid.IsValidCoordinate(point.X, point.Y))
                return false;

            var cell = _grid[point.X, point.Y];
            return cell.IsWalkable && cell.OccupyingObstacle == null;
        }

        private void ConnectNodes()
        {
            double radiusSquared = _config.ConnectionRadius * _config.ConnectionRadius;

            for (int i = 0; i < _nodes.Count; i++)
            {
                var nodeA = _nodes[i];
                var candidates = new List<(int index, double distance)>();

                // Find nearby nodes
                for (int j = i + 1; j < _nodes.Count; j++)
                {
                    var nodeB = _nodes[j];
                    double dx = nodeA.X - nodeB.X;
                    double dy = nodeA.Y - nodeB.Y;
                    double distanceSquared = dx * dx + dy * dy;

                    if (distanceSquared <= radiusSquared)
                    {
                        candidates.Add((j, Math.Sqrt(distanceSquared)));
                    }
                }

                // Sort by distance and connect to nearest neighbors
                candidates = candidates.OrderBy(c => c.distance).Take(_config.MaxNeighbors).ToList();

                foreach (var candidate in candidates)
                {
                    var nodeB = _nodes[candidate.index];

                    if (IsPathClear(nodeA.ToPoint(), nodeB.ToPoint()))
                    {
                        nodeA.Neighbors.Add(candidate.index);
                        nodeB.Neighbors.Add(i);
                        // Current node - connection being established
                        RaiseDebugEvent(nodeA.X, nodeA.Y, nodeB.X, nodeB.Y, PathFinderNodeType.Current, 0, 0);
                    }
                }
            }
        }

        private int GetTotalEdges()
        {
            int total = 0;
            foreach (var node in _nodes)
            {
                total += node.Neighbors.Count;
            }
            return total / 2;
        }
        #endregion

        #region Private Methods - Pathfinding on Roadmap
        private void AddStartAndGoal(Point start, Point goal)
        {
            // Find nearest nodes to start and goal
            int startNodeIndex = FindNearestNodeIndex(start);
            int goalNodeIndex = FindNearestNodeIndex(goal);

            // Create temporary nodes
            var startNode = new PRMNode(start.X, start.Y, _nodes.Count);
            var goalNode = new PRMNode(goal.X, goal.Y, _nodes.Count);

            // Connect start node to nearby roadmap nodes
            ConnectToRoadmap(startNode, startNodeIndex);

            // Connect goal node to nearby roadmap nodes
            ConnectToRoadmap(goalNode, goalNodeIndex);

            _nodes.Add(startNode);
            _nodes.Add(goalNode);
            // Close node - start and goal added to roadmap
            RaiseDebugEvent(start.X, start.Y, start.X, start.Y, PathFinderNodeType.Close, 0, 0);
            RaiseDebugEvent(goal.X, goal.Y, goal.X, goal.Y, PathFinderNodeType.Close, 0, 0);
        }

        private int FindNearestNodeIndex(Point point)
        {
            int nearestIndex = -1;
            double minDistance = double.MaxValue;

            for (int i = 0; i < _nodes.Count; i++)
            {
                var node = _nodes[i];
                double distance = EuclideanDistance(node.ToPoint(), point);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }

        private void ConnectToRoadmap(PRMNode newNode, int nearestIndex)
        {
            double radiusSquared = _config.ConnectionRadius * _config.ConnectionRadius;

            for (int i = 0; i < _nodes.Count; i++)
            {
                if (i == nearestIndex) continue;

                var node = _nodes[i];
                double dx = newNode.X - node.X;
                double dy = newNode.Y - node.Y;
                double distanceSquared = dx * dx + dy * dy;

                if (distanceSquared <= radiusSquared && IsPathClear(newNode.ToPoint(), node.ToPoint()))
                {
                    newNode.Neighbors.Add(i);
                    node.Neighbors.Add(newNode.Index);
                }
            }
        }

        private void RemoveStartAndGoal()
        {
            // Remove last two nodes (start and goal)
            if (_nodes.Count >= 2)
            {
                _nodes.RemoveRange(_nodes.Count - 2, 2);
            }
        }

        private List<PathNode> FindPathOnRoadmap(Point start, Point goal)
        {
            int startIdx = _nodes.Count - 2;
            int goalIdx = _nodes.Count - 1;

            // Dijkstra's algorithm
            var distances = new double[_nodes.Count];
            var previous = new int[_nodes.Count];
            var visited = new bool[_nodes.Count];

            for (int i = 0; i < _nodes.Count; i++)
            {
                distances[i] = double.MaxValue;
                previous[i] = -1;
            }

            distances[startIdx] = 0;

            var priorityQueue = new SortedSet<(double dist, int index)>();
            priorityQueue.Add((0, startIdx));

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Min;
                priorityQueue.Remove(current);

                if (visited[current.index])
                    continue;

                visited[current.index] = true;

                if (current.index == goalIdx)
                    break;

                var currentNode = _nodes[current.index];

                foreach (int neighborIdx in currentNode.Neighbors)
                {
                    if (visited[neighborIdx])
                        continue;

                    var neighborNode = _nodes[neighborIdx];
                    double edgeCost = EuclideanDistance(currentNode.ToPoint(), neighborNode.ToPoint());
                    double newDist = distances[current.index] + edgeCost;

                    if (newDist < distances[neighborIdx])
                    {
                        distances[neighborIdx] = newDist;
                        previous[neighborIdx] = current.index;
                        priorityQueue.Add((newDist, neighborIdx));
                        // Current node - exploring neighbor during search
                        RaiseDebugEvent(currentNode.X, currentNode.Y, neighborNode.X, neighborNode.Y, PathFinderNodeType.Current, 0, 0);
                    }
                }
            }

            if (previous[goalIdx] == -1)
                return null;

            // Reconstruct path
            var path = new List<PathNode>();
            int currentIdx = goalIdx;

            while (currentIdx != -1)
            {
                var node = _nodes[currentIdx];
                path.Insert(0, new PathNode(node.X, node.Y));
                currentIdx = previous[currentIdx];
            }
            // Path nodes - final path
            foreach (var node in path)
            {
                RaiseDebugEvent(node.X, node.Y, node.X, node.Y, PathFinderNodeType.Path, 0, 0);
            }
            return path;
        }
        #endregion

        #region Private Methods - Helper Functions
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

        private double EuclideanDistance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
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
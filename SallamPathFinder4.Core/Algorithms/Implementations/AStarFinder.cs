#region File Header
/// <summary>
/// File: AStarFinder.cs
/// Description: A* (A-Star) pathfinding algorithm implementation - Optimized version
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Algorithms.Base;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
using System.Xml.Linq;
#endregion

namespace SallamPathFinder4.Core.Algorithms.Implementations
{
    public sealed class AStarFinder : BasePathFinder
    {
        #region Constants
        private const int DEFAULT_SEARCH_LIMIT = 5000;
        private const int DEFAULT_HEURISTIC_WEIGHT = 2;
        private const double SQRT2 = 1.4142135623730951;
        #endregion

        #region Nested Types
        private sealed class AStarNode
        {
            public int X, Y;
            public int G;
            public int H;
            public int F => G + H;
            public AStarNode Parent;
            public bool IsClosed;

            public AStarNode(int x, int y)
            {
                X = x;
                Y = y;
                G = int.MaxValue;
                H = 0;
                Parent = null;
                IsClosed = false;
            }
        }
        #endregion

        #region Constructor
        public AStarFinder(MapGrid grid) : base(grid)
        {
            SearchLimit = DEFAULT_SEARCH_LIMIT;
            HeuristicWeight = DEFAULT_HEURISTIC_WEIGHT;
        }
        #endregion

        #region Public Methods
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

            int width = _grid.Width;
            int height = _grid.Height;

            // Use dictionaries for open and closed sets (faster for dynamic operations)
            var openDict = new Dictionary<int, AStarNode>();
            var closedDict = new Dictionary<int, AStarNode>();
            var openHeap = new SortedSet<Tuple<int, int, int>>(); // F, X, Y

            var nodes = new AStarNode[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new AStarNode(x, y);
                }
            }

            // Start node
            var startNode = nodes[start.X, start.Y];
            startNode.G = 0;
            startNode.H = CalculateHeuristic(start, end);
            int startKey = (start.Y << 16) + start.X;
            openDict[startKey] = startNode;
            openHeap.Add(Tuple.Create(startNode.F, startNode.X, startNode.Y));

            var (dx, dy) = GetMovementDirections();
            int iterations = 0;
            AStarNode currentNode = null;
            bool found = false;

            while (openHeap.Count > 0 && iterations < SearchLimit)
            {
                // Get node with smallest F
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
                
                // Current node
                RaiseDebugEvent(currentNode.X, currentNode.Y, currentNode.X, currentNode.Y, PathFinderNodeType.Current, currentNode.F, currentNode.G);

                currentNode.IsClosed = true;
                closedDict[key] = currentNode;
                iterations++;
                // Close node
                RaiseDebugEvent(currentNode.X, currentNode.Y, currentNode.X, currentNode.Y, PathFinderNodeType.Close, currentNode.F, currentNode.G);

                // Explore neighbors
                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = currentNode.X + dx[i];
                    int ny = currentNode.Y + dy[i];

                    if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                        continue;

                    var neighborCell = _grid[nx, ny];
                    if (!neighborCell.IsWalkable)
                        continue;

                    // Calculate step cost
                    double stepCost = neighborCell.SurfaceWeight;
                    if (stepCost <= 0) stepCost = 1;
                    if (IsDiagonalMove(currentNode.X, currentNode.Y, nx, ny) && HeavyDiagonals)
                        stepCost *= SQRT2;

                    int newG = currentNode.G + (int)stepCost;
                    var neighbor = nodes[nx, ny];
                    int neighborKey = (ny << 16) + nx;

                    // Skip if already closed and not reopening
                    if (closedDict.ContainsKey(neighborKey) && newG >= neighbor.G)
                        continue;

                    // Skip if open and not better
                    if (openDict.ContainsKey(neighborKey) && newG >= neighbor.G)
                        continue;

                    // Update or add node
                    neighbor.Parent = currentNode;
                    neighbor.G = newG;
                    neighbor.H = CalculateHeuristic(new Point(nx, ny), end);
                    neighbor.IsClosed = false;

                    if (!openDict.ContainsKey(neighborKey))
                    {
                        openDict[neighborKey] = neighbor;
                        openHeap.Add(Tuple.Create(neighbor.F, neighbor.X, neighbor.Y));
                    }
                    // Open node
                    RaiseDebugEvent(currentNode.X, currentNode.Y, nx, ny, PathFinderNodeType.Open, neighbor.F, neighbor.G);
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
        private List<PathNode> ReconstructPath(AStarNode endNode)
        {
            var path = new List<PathNode>();
            var current = endNode;
            while (current != null)
            {
                path.Insert(0, new PathNode(current.X, current.Y));
                current = current.Parent;
            }
            foreach (var node in path)
            {
                RaiseDebugEvent(node.X, node.Y, node.X, node.Y, PathFinderNodeType.Path, 0, 0);
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
#region File Header
/// <summary>
/// File: BruteForceFinder.cs
/// Description: Brute Force exhaustive search pathfinding algorithm
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Algorithms.Base;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Algorithms.Implementations
{
    public sealed class BruteForceFinder : BasePathFinder
    {
        #region Constants
        private const int DEFAULT_MAX_DEPTH = 5000;
        private const int DEFAULT_MAX_ITERATIONS = 100000;
        #endregion

        #region Nested Types
        private sealed class NodeState
        {
            public int X, Y;
            public List<PathNode> Path;
            public int Depth;

            public NodeState(int x, int y, List<PathNode> path, int depth)
            {
                X = x; Y = y;
                Path = new List<PathNode>(path);
                Path.Add(new PathNode(x, y));
                Depth = depth;
            }
        }
        #endregion

        #region Private Fields
        private int _maxDepth = DEFAULT_MAX_DEPTH;
        private int _maxIterations = DEFAULT_MAX_ITERATIONS;
        #endregion

        #region Constructor
        public BruteForceFinder(MapGrid grid) : base(grid)
        {
            _maxDepth = DEFAULT_MAX_DEPTH;
            _maxIterations = DEFAULT_MAX_ITERATIONS;
        }
        #endregion

        #region Public Methods
        public void SetLimits(int maxDepth, int maxIterations)
        {
            _maxDepth = Math.Max(100, Math.Min(50000, maxDepth));
            _maxIterations = Math.Max(1000, Math.Min(500000, maxIterations));
        }

        public override PathResult FindPath(Point start, Point end)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            if (!_grid.IsValidCoordinate(start.X, start.Y))
                return PathResult.Fail("Start position invalid");
            if (!_grid.IsValidCoordinate(end.X, end.Y))
                return PathResult.Fail("End position invalid");

            int mapSize = _grid.Width * _grid.Height;
            if (mapSize > 2500)
                return PathResult.Fail($"Map too large for Brute Force ({mapSize} cells). Max recommended: 2500");

            try
            {
                var queue = new Queue<NodeState>();
                var bestPath = new List<PathNode>();
                int minLength = int.MaxValue;
                int maxDepth = Math.Min(_maxDepth, SearchLimit > 0 ? SearchLimit : _maxDepth);

                queue.Enqueue(new NodeState(start.X, start.Y, new List<PathNode>(), 0));
                var (dx, dy) = GetMovementDirections();
                int iterations = 0;

                while (queue.Count > 0 && iterations < _maxIterations && !ShouldStop())
                {
                    var current = queue.Dequeue();
                    iterations++;

                    if (current.X == end.X && current.Y == end.Y)
                    {
                        if (current.Path.Count < minLength)
                        {
                            minLength = current.Path.Count;
                            bestPath = new List<PathNode>(current.Path);
                        }
                        continue;
                    }

                    if (current.Depth >= maxDepth || current.Path.Count >= minLength)
                        continue;

                    for (int i = 0; i < dx.Length; i++)
                    {
                        int nx = current.X + dx[i];
                        int ny = current.Y + dy[i];

                        if (!_grid.IsValidCoordinate(nx, ny)) continue;

                        var cell = _grid[nx, ny];
                        if (!cell.IsWalkable)
                        {
                            RecordInvalidMove(new Point(nx, ny));
                            continue;
                        }
                        if (current.Path.Any(p => p.X == nx && p.Y == ny)) continue;

                        queue.Enqueue(new NodeState(nx, ny, current.Path, current.Depth + 1));
                    }
                }

                stopwatch.Stop();

                if (bestPath.Count > 0)
                    return new PathResult(bestPath, stopwatch.Elapsed.TotalSeconds, iterations);

                return PathResult.Fail("No path found", stopwatch.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return PathResult.Fail(ex.Message, stopwatch.Elapsed.TotalSeconds);
            }
        }
        #endregion
    }
}
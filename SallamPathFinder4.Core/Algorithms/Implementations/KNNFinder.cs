#region File Header
/// <summary>
/// File: KNNFinder.cs
/// Description: K-Nearest Neighbors (KNN) pathfinding algorithm
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
#endregion

namespace SallamPathFinder4.Core.Algorithms.Implementations
{
    public sealed class KNNFinder : BasePathFinder
    {
        #region Constants
        private const int DEFAULT_K_NEIGHBORS = 3;
        private const int SEARCH_RADIUS = 5;
        #endregion

        #region Private Fields
        private int _kNeighbors = DEFAULT_K_NEIGHBORS;
        #endregion

        #region Constructor
        public KNNFinder(MapGrid grid) : base(grid)
        {
            _kNeighbors = DEFAULT_K_NEIGHBORS;
        }
        #endregion

        #region Public Methods
        public void SetKNeighbors(int k)
        {
            _kNeighbors = Math.Max(1, Math.Min(10, k));
        }

        public override PathResult FindPath(Point start, Point end)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            if (!_grid.IsValidCoordinate(start.X, start.Y))
                return PathResult.Fail("Start position invalid");
            if (!_grid.IsValidCoordinate(end.X, end.Y))
                return PathResult.Fail("End position invalid");
            if (!_grid[start.X, start.Y].IsWalkable)
                return PathResult.Fail("Start not walkable");
            if (!_grid[end.X, end.Y].IsWalkable)
                return PathResult.Fail("End not walkable");

            try
            {
                var path = new List<PathNode> { new PathNode(start.X, start.Y) };
                var visited = new HashSet<(int, int)> { (start.X, start.Y) };
                Point current = start;
                int iterations = 0;
                int maxIterations = SearchLimit > 0 ? Math.Min(SearchLimit, 50000) : 50000;

                while (!(current.X == end.X && current.Y == end.Y) && iterations < maxIterations && !ShouldStop())
                {
                    var candidates = GetNeighborCandidates(current, SEARCH_RADIUS);
                    candidates = candidates.Where(p => !visited.Contains((p.X, p.Y))).ToList();

                    if (candidates.Count == 0)
                    {
                        if (path.Count > 1)
                        {
                            path.RemoveAt(path.Count - 1);
                            current = new Point(path.Last().X, path.Last().Y);
                        }
                        else break;
                        continue;
                    }

                    var best = candidates
                        .OrderBy(p => Math.Abs(p.X - end.X) + Math.Abs(p.Y - end.Y))
                        .First();
                    // Current node visualization
                    RaiseDebugEvent(current.X, current.Y, best.X, best.Y, PathFinderNodeType.Current, 0, 0);

                    int stepX = Math.Sign(best.X - current.X);
                    int stepY = Math.Sign(best.Y - current.Y);
                    int tempX = current.X;
                    int tempY = current.Y;

                    while (!(tempX == best.X && tempY == best.Y))
                    {
                        if (stepX != 0 && tempX != best.X) tempX += stepX;
                        if (stepY != 0 && tempY != best.Y) tempY += stepY;

                        if (!_grid.IsValidCoordinate(tempX, tempY)) break;

                        var cell = _grid[tempX, tempY];
                        if (!cell.IsWalkable)
                        {
                            RecordInvalidMove(new Point(tempX, tempY));
                            break;
                        }

                        if (!visited.Contains((tempX, tempY)))
                        {
                            path.Add(new PathNode(tempX, tempY));
                            visited.Add((tempX, tempY));
                            // Close node visualization
                            RaiseDebugEvent(current.X, current.Y, tempX, tempY, PathFinderNodeType.Close, 0, 0);
                        }
                    }

                    current = best;
                    iterations++;
                }

                stopwatch.Stop();

                if (current.X == end.X && current.Y == end.Y)
                {
                    var uniquePath = new List<PathNode>();
                    foreach (var node in path)
                    {
                        if (uniquePath.Count == 0 || uniquePath.Last().X != node.X || uniquePath.Last().Y != node.Y)
                            uniquePath.Add(node);
                    }
                    // Path visualization
                    foreach (var node in uniquePath)
                    {
                        RaiseDebugEvent(node.X, node.Y, node.X, node.Y, PathFinderNodeType.Path, 0, 0);
                    }
                    return new PathResult(uniquePath, stopwatch.Elapsed.TotalSeconds, iterations);
                }

                return PathResult.Fail("No path found", stopwatch.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return PathResult.Fail(ex.Message, stopwatch.Elapsed.TotalSeconds);
            }
        }
        #endregion

        #region Private Methods
        private List<Point> GetNeighborCandidates(Point center, int radius)
        {
            var candidates = new List<Point>();
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = center.X + dx;
                    int ny = center.Y + dy;
                    if (_grid.IsValidCoordinate(nx, ny))
                    {
                        var cell = _grid[nx, ny];
                        if (cell.IsWalkable)
                            candidates.Add(new Point(nx, ny));
                        else
                            RecordInvalidMove(new Point(nx, ny));
                    }
                }
            }
            return candidates;
        }
        #endregion
    }
}
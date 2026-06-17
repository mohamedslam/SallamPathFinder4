#region File Header
/// <summary>
/// File: DStarFinder.cs
/// Description: D* (Dynamic A*) pathfinding algorithm for dynamic environments
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
    public sealed class DStarFinder : BasePathFinder
    {
        #region Nested Types
        private sealed class DStarNode : IComparable<DStarNode>
        {
            public int X, Y;
            public double G;
            public double H;
            public double F => G + H;
            public DStarNode Parent;
            public bool IsClosed;
            public bool IsOpen;

            public DStarNode(int x, int y)
            {
                X = x; Y = y;
                G = double.MaxValue;
                H = 0;
                Parent = null;
                IsClosed = false;
                IsOpen = false;
            }

            public int CompareTo(DStarNode other)
            {
                if (other == null) return 1;
                int cmp = F.CompareTo(other.F);
                if (cmp == 0) cmp = (X + Y).CompareTo(other.X + other.Y);
                return cmp;
            }
        }
        #endregion

        #region Private Fields
        private Dictionary<(int, int), DStarNode> _nodes;
        private List<DStarNode> _openList;
        #endregion

        #region Constructor
        public DStarFinder(MapGrid grid) : base(grid)
        {
            _nodes = new Dictionary<(int, int), DStarNode>();
            _openList = new List<DStarNode>();
        }
        #endregion

        #region Public Methods
        public override PathResult FindPath(Point start, Point end)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            if (!_grid.IsValidCoordinate(start.X, start.Y))
                return PathResult.Fail("Start position invalid");
            if (!_grid.IsValidCoordinate(end.X, end.Y))
                return PathResult.Fail("End position invalid");

            InitializeNodes();

            var startNode = GetOrCreateNode(start.X, start.Y);
            var endNode = GetOrCreateNode(end.X, end.Y);

            startNode.G = 0;
            startNode.H = CalculateHeuristic(start, end);
            _openList.Clear();
            _openList.Add(startNode);
            startNode.IsOpen = true;

            var (dx, dy) = GetMovementDirections();
            int iterations = 0;
            DStarNode currentNode = null;
            bool found = false;

            while (_openList.Count > 0 && iterations < SearchLimit && !ShouldStop())
            {
                _openList = _openList.OrderBy(n => n.F).ToList();
                currentNode = _openList[0];
                _openList.RemoveAt(0);
                currentNode.IsOpen = false;
                currentNode.IsClosed = true;

                // Current node visualization
                RaiseDebugEvent(currentNode.X, currentNode.Y, currentNode.X, currentNode.Y, PathFinderNodeType.Current, (int)currentNode.F, (int)currentNode.G);
                // Close node visualization
                RaiseDebugEvent(currentNode.X, currentNode.Y, currentNode.X, currentNode.Y, PathFinderNodeType.Close, (int)currentNode.F, (int)currentNode.G);
                if (currentNode.X == end.X && currentNode.Y == end.Y)
                {
                    found = true;
                    break;
                }

                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = currentNode.X + dx[i];
                    int ny = currentNode.Y + dy[i];

                    if (!_grid.IsValidCoordinate(nx, ny)) continue;

                    var cell = _grid[nx, ny];
                    if (!cell.IsWalkable)
                    {
                        RecordInvalidMove(new Point(nx, ny));
                        continue;
                    }

                    var neighbor = GetOrCreateNode(nx, ny);
                    if (neighbor.IsClosed) continue;

                    double stepCost = CalculateStepCost(new Point(currentNode.X, currentNode.Y), new Point(nx, ny));
                    double tentativeG = currentNode.G + stepCost;

                    if (tentativeG < neighbor.G)
                    {
                        neighbor.Parent = currentNode;
                        neighbor.G = tentativeG;
                        neighbor.H = CalculateHeuristic(new Point(nx, ny), end);

                        if (!neighbor.IsOpen)
                        {
                            neighbor.IsOpen = true;
                            _openList.Add(neighbor);
                            // Open node visualization
                            RaiseDebugEvent(currentNode.X, currentNode.Y, nx, ny, PathFinderNodeType.Open, (int)neighbor.F, (int)neighbor.G);
                        }
                    }
                }

                iterations++;
            }

            stopwatch.Stop();

            if (found && currentNode != null)
            {
                var path = ReconstructPath(currentNode);
                return new PathResult(path, stopwatch.Elapsed.TotalSeconds, iterations);
            }

            return PathResult.Fail("No path found", stopwatch.Elapsed.TotalSeconds);
        }

        public PathResult Replan(Point start, Point end, Point changedCell, bool becameBlocked)
        {
            if (becameBlocked)
                UpdateNodeCost(changedCell, double.MaxValue);
            else
                UpdateNodeCost(changedCell, _grid[changedCell.X, changedCell.Y].Cost);

            return FindPath(start, end);
        }
        #endregion

        #region Private Methods
        private void InitializeNodes()
        {
            _nodes.Clear();
            _openList.Clear();
        }

        private DStarNode GetOrCreateNode(int x, int y)
        {
            var key = (x, y);
            if (!_nodes.ContainsKey(key))
                _nodes[key] = new DStarNode(x, y);
            return _nodes[key];
        }

        private void UpdateNodeCost(Point position, double newCost)
        {
            var key = (position.X, position.Y);
            if (_nodes.ContainsKey(key))
                _nodes[key].G = newCost;
        }

        private List<PathNode> ReconstructPath(DStarNode endNode)
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
    }
}
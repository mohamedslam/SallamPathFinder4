#region File Header
/// <summary>
/// File: BasePathFinder.cs
/// Description: Abstract base class for all pathfinding algorithms
/// Implements common functionality following DRY principle
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Algorithms;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
#endregion

namespace SallamPathFinder4.Core.Algorithms.Base
{
    public abstract class BasePathFinder : IPathFinder
    {
        #region Constants
        private const double SQRT2 = 1.4142135623730951;
        private const int DEFAULT_HEURISTIC_WEIGHT = 2;
        private const int DEFAULT_SEARCH_LIMIT = 50000;
        #endregion

        #region Private Fields
        protected readonly MapGrid _grid;
        protected CancellationTokenSource _cts;
        protected bool _isPaused;
        protected bool _isStopped;
        protected readonly object _lockObject = new object();

        /// <summary>
        /// Collection of cells where algorithm attempted to move through walls
        /// Used for debugging and visualization (marked with X)
        /// </summary>
        protected HashSet<Point> _invalidPathCells;
        #endregion

        #region Constructor
        protected BasePathFinder(MapGrid grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _cts = new CancellationTokenSource();
            _invalidPathCells = new HashSet<Point>();

            Metric = DistanceMetric.Manhattan;
            AllowDiagonals = true;
            HeavyDiagonals = false;
            HeuristicWeight = DEFAULT_HEURISTIC_WEIGHT;
            SearchLimit = DEFAULT_SEARCH_LIMIT;
            ShowDebugProgress = false;
        }
        #endregion

        #region Public Properties - Configuration
        public DistanceMetric Metric { get; set; }
        public bool AllowDiagonals { get; set; }
        public bool HeavyDiagonals { get; set; }
        public int HeuristicWeight { get; set; }
        public int SearchLimit { get; set; }
        public bool ShowDebugProgress { get; set; }
        public bool IsStopped => _isStopped;

        /// <summary>
        /// Gets the collection of invalid path cells (attempted to walk through walls)
        /// </summary>
        public IReadOnlyCollection<Point> InvalidPathCells => _invalidPathCells;
        #endregion

        #region Public Properties - Metrics
        public double LastComputationTimeSeconds { get; protected set; }
        #endregion

        #region Events
        public event PathFinderDebugHandler DebugUpdate;
        #endregion

        #region Public Methods - Control
        public virtual void Stop()
        {
            lock (_lockObject)
            {
                _isStopped = true;
                _cts?.Cancel();
            }
        }

        public virtual void Pause()
        {
            lock (_lockObject)
            {
                _isPaused = true;
            }
        }

        public virtual void Resume()
        {
            lock (_lockObject)
            {
                _isPaused = false;
            }
        }

        /// <summary>
        /// Records an invalid move attempt (trying to walk through a wall)
        /// </summary>
        protected void RecordInvalidMove(Point cell)
        {
            lock (_lockObject)
            {
                _invalidPathCells.Add(cell);
            }
        }

        /// <summary>
        /// Clears all recorded invalid path cells
        /// </summary>
        public void ClearInvalidPathCells()
        {
            lock (_lockObject)
            {
                _invalidPathCells.Clear();
            }
        }
        #endregion

        #region Protected Methods - Heuristic
        protected int CalculateHeuristic(Point a, Point b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            int weight = HeuristicWeight;

            return Metric switch
            {
                DistanceMetric.Manhattan => weight * (dx + dy),
                DistanceMetric.MaxDXDY => weight * Math.Max(dx, dy),
                DistanceMetric.DiagonalShortcut => (weight * 2) * Math.Min(dx, dy) + weight * Math.Abs(dx - dy),
                DistanceMetric.Euclidean => (int)(weight * Math.Sqrt(dx * dx + dy * dy)),
                DistanceMetric.EuclideanNoSQR => weight * (dx * dx + dy * dy),
                DistanceMetric.Custom => CalculateCustomHeuristic(dx, dy, weight),
                _ => weight * (dx + dy)
            };
        }

        protected virtual int CalculateCustomHeuristic(int dx, int dy, int weight)
        {
            return (int)(weight * Math.Sqrt(dx * dx + dy * dy));
        }
        #endregion

        #region Protected Methods - Cost Calculation
        protected double CalculateStepCost(Point from, Point to)
        {
            if (!_grid.IsValidCoordinate(to.X, to.Y))
                return double.MaxValue;

            var cell = _grid[to.X, to.Y];

            if (!cell.IsWalkable)
            {
                RecordInvalidMove(to);
                return double.MaxValue;
            }

            double cost = cell.Cost;

            if (cost <= 0 || double.IsInfinity(cost))
                cost = 1.0;

            if (HeavyDiagonals && IsDiagonalMove(from, to))
            {
                cost *= SQRT2;
            }

            return Math.Max(0.1, Math.Min(1000, cost));
        }

        protected static bool IsDiagonalMove(Point from, Point to)
        {
            return Math.Abs(from.X - to.X) == 1 && Math.Abs(from.Y - to.Y) == 1;
        }

        protected (int[] dx, int[] dy) GetMovementDirections()
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

        #region Protected Methods - Path Reconstruction
        protected List<PathNode> ReconstructPath<T>(T endNode, Func<T, T> getParent, Func<T, Point> getPosition)
            where T : class
        {
            var path = new List<PathNode>();
            var current = endNode;

            while (current != null)
            {
                var pos = getPosition(current);
                path.Insert(0, new PathNode(pos.X, pos.Y));
                current = getParent(current);
            }

            return path;
        }
        #endregion

        #region Protected Methods - Debug
        protected void RaiseDebugEvent(int fromX, int fromY, int x, int y, PathFinderNodeType type, int totalCost, int cost)
        {
            if (ShowDebugProgress)
            {
                DebugUpdate?.Invoke(fromX, fromY, x, y, type, totalCost, cost);
            }
        }

        protected bool ShouldStop()
        {
            lock (_lockObject)
            {
                return _isStopped || _isPaused;
            }
        }

        protected bool IsCancellationRequested()
        {
            return _cts?.IsCancellationRequested == true;
        }
        #endregion

        #region Abstract Method
        public abstract PathResult FindPath(Point start, Point end);
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
            }
        }
        #endregion
    }
}
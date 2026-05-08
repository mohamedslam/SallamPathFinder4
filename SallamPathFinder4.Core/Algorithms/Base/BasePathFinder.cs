#region File Header
/// <summary>
/// File: BasePathFinder.cs
/// Description: Abstract base class for all pathfinding algorithms
/// Implements common functionality following DRY principle
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-10
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Algorithms;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Algorithms.Base
{
    #region Class Documentation
    /// <summary>
    /// Abstract base class for all pathfinding algorithms
    /// Provides common functionality for pathfinding operations
    /// Implements IDisposable for resource management
    /// </summary>
    #endregion
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
        private readonly object _pauseLock = new object();

        // Visualization settings
        private bool _enableVisualization = false;
        private int _speedDelayMs = 0;

        /// <summary>
        /// Collection of cells where algorithm attempted to move through walls
        /// Used for debugging and visualization (marked with X)
        /// </summary>
        protected HashSet<Point> _invalidPathCells;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the BasePathFinder
        /// </summary>
        /// <param name="grid">Map grid for pathfinding</param>
        protected BasePathFinder(MapGrid grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _cts = new CancellationTokenSource();
            _invalidPathCells = new HashSet<Point>();

            this.Metric = DistanceMetric.Manhattan;
            this.AllowDiagonals = true;
            this.HeavyDiagonals = false;
            this.HeuristicWeight = DEFAULT_HEURISTIC_WEIGHT;
            this.SearchLimit = DEFAULT_SEARCH_LIMIT;
            this.ShowDebugProgress = false;
        }
        #endregion

        #region Public Properties - Configuration
        /// <inheritdoc/>
        public DistanceMetric Metric { get; set; }

        /// <inheritdoc/>
        public bool AllowDiagonals { get; set; }

        /// <inheritdoc/>
        public bool HeavyDiagonals { get; set; }

        /// <inheritdoc/>
        public int HeuristicWeight { get; set; }

        /// <inheritdoc/>
        public int SearchLimit { get; set; }

        /// <inheritdoc/>
        public bool ShowDebugProgress { get; set; }

        /// <inheritdoc/>
        public bool IsStopped
        {
            get
            {
                lock (_lockObject)
                {
                    return _isStopped;
                }
            }
        }

        /// <summary>
        /// Gets the collection of invalid path cells (attempted to walk through walls)
        /// </summary>
        public IReadOnlyCollection<Point> InvalidPathCells
        {
            get
            {
                lock (_lockObject)
                {
                    return _invalidPathCells;
                }
            }
        }
        #endregion

        #region Public Properties - Visualization
        /// <summary>
        /// تفعيل/إيقاف تصور عملية البحث
        /// </summary>
        public bool EnableVisualization
        {
            get => _enableVisualization;
            set => _enableVisualization = value;
        }

        /// <summary>
        /// سرعة العرض بالمللي ثانية
        /// </summary>
        public int SpeedDelayMs
        {
            get => _speedDelayMs;
            set => _speedDelayMs = Math.Max(0, Math.Min(500, value));
        }
        #endregion

        #region Public Properties - Metrics
        /// <inheritdoc/>
        public double LastComputationTimeSeconds { get; protected set; }
        #endregion

        #region Events
        /// <inheritdoc/>
        public event PathFinderDebugHandler DebugUpdate;
        #endregion

        #region Protected Methods - Debug Event Management (NEW)
        /// <summary>
        /// Copies debug event handlers to another pathfinder
        /// Used when temporarily switching to another algorithm (e.g., Fast Path)
        /// 🔴 IMPORTANT: Preserves visualization for educational purposes
        /// </summary>
        /// <param name="target">The target pathfinder to receive debug events</param>
        protected void CopyDebugEventsTo(IPathFinder target)
        {
            if (target == null) return;

            // Only copy if debug visualization is enabled
            if (this.DebugUpdate != null && this.ShowDebugProgress)
            {
                target.DebugUpdate += this.DebugUpdate;
            }

            // Copy visualization settings
            target.EnableVisualization = this.EnableVisualization;
            target.SpeedDelayMs = this.SpeedDelayMs;
            target.ShowDebugProgress = this.ShowDebugProgress;
        }

        /// <summary>
        /// Removes debug event handlers from another pathfinder
        /// 🔴 IMPORTANT: Prevents memory leaks by unsubscribing
        /// </summary>
        /// <param name="target">The target pathfinder to remove events from</param>
        protected void RemoveDebugEventsFrom(IPathFinder target)
        {
            if (target == null) return;

            if (this.DebugUpdate != null)
            {
                target.DebugUpdate -= this.DebugUpdate;
            }
        }

        /// <summary>
        /// Temporarily switches to another pathfinder while preserving debug events
        /// Used for educational visualization of different algorithms
        /// </summary>
        /// <typeparam name="T">Type of pathfinder to create</typeparam>
        /// <param name="finderCreator">Function to create the new finder</param>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        /// <returns>Path result from the temporary finder</returns>
        protected PathResult RunWithDebugEvents<T>(Func<T> finderCreator, Point start, Point end) where T : IPathFinder
        {
            var tempFinder = finderCreator();

            // Copy debug events to temporary finder
            CopyDebugEventsTo(tempFinder);

            // Copy algorithm settings
            tempFinder.Metric = this.Metric;
            tempFinder.AllowDiagonals = this.AllowDiagonals;
            tempFinder.HeavyDiagonals = this.HeavyDiagonals;
            tempFinder.HeuristicWeight = this.HeuristicWeight;
            tempFinder.SearchLimit = this.SearchLimit;

            // Execute pathfinding
            var result = tempFinder.FindPath(start, end);

            // Remove debug events to prevent memory leaks
            RemoveDebugEventsFrom(tempFinder);

            // Clean up if disposable
            if (tempFinder is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return result;
        }
        #endregion

        #region Public Methods - Control
        /// <inheritdoc/>
        public virtual void Stop()
        {
            lock (_pauseLock)
            {
                _isStopped = true;
                _isPaused = false;
                _cts?.Cancel();
                Monitor.Pulse(_pauseLock);
                System.Diagnostics.Debug.WriteLine("Stop: _isStopped = true");
            }
        }

        /// <inheritdoc/>
        public virtual void Pause()
        {
            lock (_lockObject)
            {
                _isPaused = true;
            }
        }

        /// <inheritdoc/>
        public virtual void ResumeSearch()
        {
            lock (_pauseLock)
            {
                _isPaused = false;
                Monitor.Pulse(_pauseLock);
                System.Diagnostics.Debug.WriteLine("ResumeSearch: _isPaused = false, search resumed");
            }
        }

        public virtual void PauseSearch()
        {
            lock (_pauseLock)
            {
                _isPaused = true;
                System.Diagnostics.Debug.WriteLine("PauseSearch: _isPaused = true");
            }
        }

        /// <summary>
        /// Records an invalid move attempt (trying to walk through a wall)
        /// </summary>
        /// <param name="cell">The cell that was attempted</param>
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
        /// <summary>
        /// Calculates heuristic distance between two points
        /// </summary>
        /// <param name="a">First point</param>
        /// <param name="b">Second point</param>
        /// <returns>Heuristic distance value</returns>
        protected int CalculateHeuristic(Point a, Point b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            int weight = this.HeuristicWeight;

            switch (this.Metric)
            {
                case DistanceMetric.Manhattan:
                    return weight * (dx + dy);

                case DistanceMetric.MaxDXDY:
                    return weight * Math.Max(dx, dy);

                case DistanceMetric.DiagonalShortcut:
                    return (weight * 2) * Math.Min(dx, dy) + weight * Math.Abs(dx - dy);

                case DistanceMetric.Euclidean:
                    return (int)(weight * Math.Sqrt(dx * dx + dy * dy));

                case DistanceMetric.EuclideanNoSQR:
                    return weight * (dx * dx + dy * dy);

                case DistanceMetric.Custom:
                    return CalculateCustomHeuristic(dx, dy, weight);

                default:
                    return weight * (dx + dy);
            }
        }

        /// <summary>
        /// Calculates custom heuristic - can be overridden for specialized needs
        /// </summary>
        protected virtual int CalculateCustomHeuristic(int dx, int dy, int weight)
        {
            return (int)(weight * Math.Sqrt(dx * dx + dy * dy));
        }
        #endregion

        #region Protected Methods - Cost Calculation
        /// <summary>
        /// Calculates step cost between two cells
        /// </summary>
        /// <param name="from">Source cell</param>
        /// <param name="to">Destination cell</param>
        /// <returns>Cost value</returns>
        protected double CalculateStepCost(Point from, Point to)
        {
            if (!_grid.IsValidCoordinate(to.X, to.Y))
            {
                return double.MaxValue;
            }

            var cell = _grid[to.X, to.Y];

            if (!cell.IsWalkable)
            {
                this.RecordInvalidMove(to);
                return double.MaxValue;
            }

            double cost = cell.Cost;

            if (cost <= 0 || double.IsInfinity(cost))
            {
                cost = 1.0;
            }

            if (this.HeavyDiagonals && IsDiagonalMove(from, to))
            {
                cost *= SQRT2;
            }

            return Math.Max(0.1, Math.Min(1000, cost));
        }

        /// <summary>
        /// Checks if movement is diagonal
        /// </summary>
        protected static bool IsDiagonalMove(Point from, Point to)
        {
            return Math.Abs(from.X - to.X) == 1 && Math.Abs(from.Y - to.Y) == 1;
        }

        /// <summary>
        /// Gets movement directions based on diagonal settings
        /// </summary>
        protected (int[] dx, int[] dy) GetMovementDirections()
        {
            if (this.AllowDiagonals)
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
        /// <summary>
        /// Reconstructs path from end node to start node
        /// </summary>
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

        #region Protected Methods - Debug (Enhanced for Education)
        /// <summary>
        /// Raises debug event for visualization
        /// 🔴 ENHANCED: Supports pause/resume and GIF recording
        /// </summary>
        protected void RaiseDebugEvent(int fromX, int fromY, int x, int y,
            PathFinderNodeType type, int totalCost, int cost)
        {
            if (!this.ShowDebugProgress) return;

            lock (_pauseLock)
            {
                while (_isPaused && !_isStopped)
                {
                    Monitor.Wait(_pauseLock);
                }
            }

            if (_isStopped) return;

            var handler = this.DebugUpdate;
            handler?.Invoke(fromX, fromY, x, y, type, totalCost, cost);

            if (_enableVisualization && _speedDelayMs > 0)
            {
                Thread.Sleep(_speedDelayMs);
            }
        }

        /// <summary>
        /// Checks if algorithm should stop (stopped or paused)
        /// </summary>
        protected bool ShouldStop()
        {
            lock (_lockObject)
            {
                return _isStopped || _isPaused;
            }
        }

        /// <summary>
        /// Checks if cancellation was requested
        /// </summary>
        protected bool IsCancellationRequested()
        {
            return _cts?.IsCancellationRequested == true;
        }
        #endregion

        #region Abstract Method
        /// <inheritdoc/>
        public abstract PathResult FindPath(Point start, Point end);
        #endregion

        #region IDisposable Implementation
        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
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
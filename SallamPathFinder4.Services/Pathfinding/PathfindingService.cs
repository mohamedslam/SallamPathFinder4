#region File Header
/// <summary>
/// File: PathfindingService.cs
/// Description: Service for pathfinding operations supporting multiple algorithms
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Services.Pathfinding;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Algorithms;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Services.Pathfinding
{
    #region Class Documentation
    /// <summary>
    /// Service for pathfinding operations
    /// Supports multiple algorithms and provides methods for finding paths
    /// between points and sequences of goals
    /// Thread-safe with proper locking
    /// </summary>
    #endregion
    public sealed class PathfindingService : IPathfindingService
    {
        #region Constants
        private const bool DEFAULT_ALLOW_DIAGONALS = true;
        private const bool DEFAULT_HEAVY_DIAGONALS = false;
        private const int DEFAULT_HEURISTIC_WEIGHT = 2;
        private const int DEFAULT_SEARCH_LIMIT = 50000;
        #endregion

        #region Private Fields
        private readonly MapGrid _mapGrid;
        private readonly AlgorithmFactory _algorithmFactory;
        private readonly object _lockObject = new object();

        private bool _allowDiagonals = DEFAULT_ALLOW_DIAGONALS;
        private bool _heavyDiagonals = DEFAULT_HEAVY_DIAGONALS;
        private int _heuristicWeight = DEFAULT_HEURISTIC_WEIGHT;
        private int _searchLimit = DEFAULT_SEARCH_LIMIT;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new pathfinding service
        /// </summary>
        public PathfindingService(MapGrid grid)
        {
            _mapGrid = grid ?? throw new ArgumentNullException(nameof(grid));
            _algorithmFactory = new AlgorithmFactory(_mapGrid);
        }
        #endregion

        #region Events
        /// <inheritdoc/>
        public event PathFinderDebugHandler NodeProcessed;
        #endregion

        #region Public Methods - Algorithm Parameters
        /// <inheritdoc/>
        public void SetAlgorithmParameters(bool allowDiagonals, bool heavyDiagonals, int heuristicWeight, int searchLimit)
        {
            lock (_lockObject)
            {
                _allowDiagonals = allowDiagonals;
                _heavyDiagonals = heavyDiagonals;
                _heuristicWeight = Math.Max(1, Math.Min(10, heuristicWeight));
                _searchLimit = Math.Max(1000, Math.Min(1000000, searchLimit));
            }
        }

        /// <inheritdoc/>
        public (bool AllowDiagonals, bool HeavyDiagonals, int HeuristicWeight, int SearchLimit) GetAlgorithmParameters()
        {
            lock (_lockObject)
            {
                return (_allowDiagonals, _heavyDiagonals, _heuristicWeight, _searchLimit);
            }
        }
        #endregion

        #region Public Methods - Pathfinding
        /// <inheritdoc/>
        public async Task<PathResult> FindFullPathAsync(Point start, IReadOnlyList<Point> goals,
            AlgorithmType algorithm, DistanceMetric metric)
        {
            if (goals == null || goals.Count == 0)
                return PathResult.Fail("No goals specified");

            return await Task.Run(() =>
            {
                var fullPath = new List<PathNode>();
                double totalTime = 0;
                int totalNodes = 0;
                Point currentPos = start;

                for (int i = 0; i < goals.Count; i++)
                {
                    var pathResult = FindSegment(currentPos, goals[i], algorithm, metric);

                    if (!pathResult.Success)
                    {
                        return PathResult.Fail($"Failed to find path to goal {i + 1}: {pathResult.ErrorMessage}");
                    }

                    // Add path, avoiding duplicate points at junctions
                    if (fullPath.Count == 0)
                    {
                        fullPath.AddRange(pathResult.Path);
                    }
                    else
                    {
                        fullPath.AddRange(pathResult.Path.Skip(1));
                    }

                    totalTime += pathResult.ComputationTimeSeconds;
                    totalNodes += pathResult.NodesExplored;
                    currentPos = goals[i];
                }

                return new PathResult(fullPath, totalTime, totalNodes);
            });
        }

        /// <inheritdoc/>
        public async Task<PathResult> FindReturnPathAsync(Point from, IReadOnlyList<Point> parkingPoints,
            AlgorithmType algorithm, DistanceMetric metric)
        {
            if (parkingPoints == null || parkingPoints.Count == 0)
                return PathResult.Fail("No parking points available");

            var nearest = parkingPoints
                .OrderBy(p => Math.Abs(p.X - from.X) + Math.Abs(p.Y - from.Y))
                .FirstOrDefault();

            return await FindSegmentAsync(from, nearest, algorithm, metric);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Finds a path between two points synchronously
        /// </summary>
        private PathResult FindSegment(Point start, Point end, AlgorithmType algorithm, DistanceMetric metric)
        {
            var finder = CreateFinder(algorithm, metric);

            if (finder == null)
                return PathResult.Fail($"Algorithm {algorithm} not available");

            return finder.FindPath(start, end);
        }

        /// <summary>
        /// Finds a path between two points asynchronously
        /// </summary>
        private async Task<PathResult> FindSegmentAsync(Point start, Point end, AlgorithmType algorithm, DistanceMetric metric)
        {
            return await Task.Run(() => FindSegment(start, end, algorithm, metric));
        }

        /// <summary>
        /// Creates and configures a pathfinder instance
        /// </summary>
        private IPathFinder CreateFinder(AlgorithmType algorithm, DistanceMetric metric)
        {
            lock (_lockObject)
            {
                var finder = _algorithmFactory.Create(algorithm);

                if (finder == null)
                    return null;

                // Apply common settings
                finder.Metric = metric;
                finder.AllowDiagonals = _allowDiagonals;
                finder.HeavyDiagonals = _heavyDiagonals;
                finder.HeuristicWeight = _heuristicWeight;
                finder.SearchLimit = _searchLimit;

                // Attach debug handler if needed
                if (NodeProcessed != null)
                {
                    finder.DebugUpdate += NodeProcessed;
                }

                return finder;
            }
        }
        #endregion
    }
}
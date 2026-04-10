#region File Header
/// <summary>
/// File: IPathfindingService.cs
/// Description: Interface for pathfinding operations service
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Algorithms;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Interface Documentation
    /// <summary>
    /// Service interface for pathfinding operations
    /// Handles finding paths between points and sequences of goals
    /// </summary>
    #endregion
    public interface IPathfindingService
    {
        #region Methods
        /// <summary>
        /// Finds a path that visits all goals in sequence
        /// </summary>
        Task<PathResult> FindFullPathAsync(Point start, IReadOnlyList<Point> goals,
            AlgorithmType algorithm, DistanceMetric metric);

        /// <summary>
        /// Finds a return path from a point to the nearest parking
        /// </summary>
        Task<PathResult> FindReturnPathAsync(Point from, IReadOnlyList<Point> parkingPoints,
            AlgorithmType algorithm, DistanceMetric metric);

        /// <summary>
        /// Sets the current algorithm parameters
        /// </summary>
        void SetAlgorithmParameters(bool allowDiagonals, bool heavyDiagonals,
            int heuristicWeight, int searchLimit);

        /// <summary>
        /// Gets the current algorithm parameters
        /// </summary>
        (bool AllowDiagonals, bool HeavyDiagonals, int HeuristicWeight, int SearchLimit) GetAlgorithmParameters();
        #endregion

        #region Events
        /// <summary>Event raised when a node is processed (for debug visualization)</summary>
        event PathFinderDebugHandler NodeProcessed;
        #endregion
    }
}
#region File Header
/// <summary>
/// File: IPathFinder.cs
/// Description: Core interface for all pathfinding algorithms
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Algorithms
{
    #region Delegate Documentation
    /// <summary>
    /// Delegate for debug visualization during pathfinding
    /// Called when nodes are processed to show algorithm progress
    /// </summary>
    /// <param name="fromX">Source X coordinate</param>
    /// <param name="fromY">Source Y coordinate</param>
    /// <param name="x">Current node X coordinate</param>
    /// <param name="y">Current node Y coordinate</param>
    /// <param name="type">Type of node (Open, Close, Current, Path)</param>
    /// <param name="totalCost">Total cost F value</param>
    /// <param name="cost">Cost from start G value</param>
    #endregion
    public delegate void PathFinderDebugHandler(int fromX, int fromY, int x, int y, PathFinderNodeType type, int totalCost, int cost);

    #region Interface Documentation
    /// <summary>
    /// Core interface for all pathfinding algorithm implementations
    /// Follows Interface Segregation Principle (ISP)
    /// </summary>
    #endregion
    public interface IPathFinder
    {
        #region Control Methods
        /// <summary>Stops the current pathfinding operation</summary>
        void Stop();

        /// <summary>Pauses the current pathfinding operation</summary>
        void Pause();

        /// <summary>Resumes a paused pathfinding operation</summary>
        void Resume();

        /// <summary>Gets whether the algorithm is stopped</summary>
        bool IsStopped { get; }
        #endregion

        #region Configuration Properties
        /// <summary>Distance metric for heuristic calculation</summary>
        DistanceMetric Metric { get; set; }

        /// <summary>Allows diagonal movement (8-directional vs 4-directional)</summary>
        bool AllowDiagonals { get; set; }

        /// <summary>Higher cost for diagonal movement (1.414x vs 1x)</summary>
        bool HeavyDiagonals { get; set; }

        /// <summary>Weight multiplier for heuristic (higher = more greedy)</summary>
        int HeuristicWeight { get; set; }

        /// <summary>Maximum nodes to explore before giving up</summary>
        int SearchLimit { get; set; }

        /// <summary>Enables debug visualization events</summary>
        bool ShowDebugProgress { get; set; }
        #endregion

        #region Events
        /// <summary>Event raised for debug visualization during pathfinding</summary>
        event PathFinderDebugHandler DebugUpdate;
        #endregion

        #region Core Method
        /// <summary>
        /// Finds a path from start to end points
        /// </summary>
        /// <param name="start">Starting grid coordinates</param>
        /// <param name="end">Target grid coordinates</param>
        /// <returns>PathResult containing path or failure status</returns>
        PathResult FindPath(Point start, Point end);
        #endregion
    }
}
#region File Header
/// <summary>
/// File: PathFinderNodeType.cs
/// Description: Defines node states for debug visualization during pathfinding
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

namespace SallamPathFinder4.Core.Enums
{
    #region Enum Documentation
    /// <summary>
    /// Node states used for debug visualization during pathfinding
    /// Helps visualize algorithm progression on the map
    /// Used by DebugUpdate event in IPathFinder
    /// </summary>
    #endregion
    public enum PathFinderNodeType
    {
        /// <summary>
        /// Node in open set - discovered but not yet processed
        /// Colored green in debug view
        /// </summary>
        Open = 0,

        /// <summary>
        /// Node in closed set - already processed
        /// Colored red in debug view
        /// </summary>
        Close = 1,

        /// <summary>
        /// Current node being processed
        /// Colored blue in debug view
        /// </summary>
        Current = 2,

        /// <summary>
        /// Node that is part of the final path
        /// Colored yellow in debug view
        /// </summary>
        Path = 3
    }
}
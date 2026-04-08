#region File Header
/// <summary>
/// File: AlgorithmType.cs
/// Description: Defines all supported pathfinding algorithm types
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

namespace SallamPathFinder4.Core.Enums
{
    #region Enum Documentation
    /// <summary>
    /// Supported pathfinding algorithm types
    /// Used for algorithm selection and factory pattern
    /// </summary>
    #endregion
    public enum AlgorithmType
    {
        /// <summary>
        /// A* (A-Star) algorithm - Standard informed search
        /// Uses heuristic function: f(n) = g(n) + h(n)
        /// Best for static environments
        /// </summary>
        AStar = 0,

        /// <summary>
        /// SPPA (Shortest Path with Precautionary Avoidance)
        /// Extended cost function: f(n) = g(n) + h(n) + λ·o(n)
        /// Handles static and semi-static obstacles
        /// Reference: Makarovskikh T., Sallam M. (2024)
        /// </summary>
        SPPA = 1,

        /// <summary>
        /// SPPA-DL (SPPA with Dynamic Learning)
        /// Extended cost function: f(n) = g(n) + h(n) + λ·o(n) + α·m(n)
        /// Includes obstacle memory and learning from past simulations
        /// Reference: Makarovskikh T., Sallam M. (2025)
        /// </summary>
        SPPA_DL = 2,

        /// <summary>
        /// ACO (Ant Colony Optimization)
        /// Swarm-based algorithm using pheromone trails
        /// Best for finding near-optimal paths in complex environments
        /// </summary>
        ACO = 3,

        /// <summary>
        /// D* (Dynamic A*)
        /// Designed for dynamic environments with changing obstacles
        /// Efficiently replans only affected parts of the path
        /// </summary>
        DStar = 4,

        /// <summary>
        /// KNN (K-Nearest Neighbors)
        /// Simple heuristic-based local search
        /// Fast but not guaranteed optimal
        /// </summary>
        KNN = 5,

        /// <summary>
        /// Brute Force Search
        /// Exhaustive search exploring all possibilities
        /// Only suitable for very small maps (≤ 50x50)
        /// </summary>
        BruteForce = 6
    }
}
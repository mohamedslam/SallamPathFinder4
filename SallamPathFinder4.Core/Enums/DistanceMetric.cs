#region File Header
/// <summary>
/// File: DistanceMetric.cs
/// Description: Defines distance calculation methods for heuristics
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

namespace SallamPathFinder4.Core.Enums
{
    #region Enum Documentation
    /// <summary>
    /// Distance metrics for heuristic calculation in pathfinding algorithms
    /// Each metric affects the algorithm's behavior and path quality
    /// </summary>
    #endregion
    public enum DistanceMetric
    {
        /// <summary>
        /// Manhattan distance: |dx| + |dy|
        /// Best for 4-directional movement (no diagonals)
        /// Admissible and consistent heuristic
        /// </summary>
        Manhattan = 0,

        /// <summary>
        /// Euclidean distance: √(dx² + dy²)
        /// Best for 8-directional movement with free diagonals
        /// Admissible but less informed than Manhattan
        /// </summary>
        Euclidean = 1,

        /// <summary>
        /// MaxDX/DY distance: max(|dx|, |dy|)
        /// Chebyshev distance, best for 8-directional movement
        /// When diagonal movement cost equals straight movement
        /// </summary>
        MaxDXDY = 2,

        /// <summary>
        /// Diagonal shortcut: 2×min(dx,dy) + (max(dx,dy) - min(dx,dy))
        /// Optimized for 8-directional movement with diagonal cost = 2
        /// More accurate than Manhattan for diagonal-heavy paths
        /// </summary>
        DiagonalShortcut = 3,

        /// <summary>
        /// Euclidean without square root: dx² + dy²
        /// Faster computation than Euclidean (no sqrt)
        /// Not admissible (overestimates), but useful for speed
        /// </summary>
        EuclideanNoSQR = 4,

        /// <summary>
        /// Custom distance metric
        /// Can be overridden by algorithm-specific implementation
        /// Allows for specialized heuristics
        /// </summary>
        Custom = 5
    }
}
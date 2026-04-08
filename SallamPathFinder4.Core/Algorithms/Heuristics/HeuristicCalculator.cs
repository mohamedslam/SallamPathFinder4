#region File Header
/// <summary>
/// File: HeuristicCalculator.cs
/// Description: Static utility class for heuristic distance calculations
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.Core.Algorithms.Heuristics
{
    #region Class Documentation
    /// <summary>
    /// Static utility class for calculating heuristic distances
    /// Supports multiple distance metrics for pathfinding algorithms
    /// All methods are thread-safe and stateless
    /// </summary>
    #endregion
    public static class HeuristicCalculator
    {
        #region Constants
        private const double SQRT2 = 1.4142135623730951;
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates heuristic distance between two points
        /// </summary>
        /// <param name="a">First point</param>
        /// <param name="b">Second point</param>
        /// <param name="metric">Distance metric to use</param>
        /// <param name="weight">Weight multiplier (default = 1)</param>
        /// <returns>Heuristic distance value</returns>
        public static int Calculate(Point a, Point b, DistanceMetric metric, int weight = 1)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);

            return metric switch
            {
                DistanceMetric.Manhattan => weight * (dx + dy),

                DistanceMetric.MaxDXDY => weight * Math.Max(dx, dy),

                DistanceMetric.DiagonalShortcut => (weight * 2) * Math.Min(dx, dy) + weight * Math.Abs(dx - dy),

                DistanceMetric.Euclidean => (int)(weight * Math.Sqrt(dx * dx + dy * dy)),

                DistanceMetric.EuclideanNoSQR => weight * (dx * dx + dy * dy),

                DistanceMetric.Custom => CalculateCustom(dx, dy, weight),

                _ => weight * (dx + dy)
            };
        }

        /// <summary>
        /// Calculates Manhattan distance (|dx| + |dy|)
        /// Best for 4-directional movement
        /// </summary>
        public static int Manhattan(Point a, Point b, int weight = 1)
        {
            return weight * (Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y));
        }

        /// <summary>
        /// Calculates Euclidean distance (√(dx² + dy²))
        /// Best for 8-directional movement with free diagonals
        /// </summary>
        public static int Euclidean(Point a, Point b, int weight = 1)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return (int)(weight * Math.Sqrt(dx * dx + dy * dy));
        }

        /// <summary>
        /// Calculates Chebyshev distance (max(|dx|, |dy|))
        /// Best for 8-directional movement when diagonal cost equals straight
        /// </summary>
        public static int Chebyshev(Point a, Point b, int weight = 1)
        {
            return weight * Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
        }

        /// <summary>
        /// Calculates Octile distance (diagonal shortcut)
        /// Best for 8-directional movement with diagonal cost = 2
        /// </summary>
        public static int Octile(Point a, Point b, int weight = 1)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            int diagonal = Math.Min(dx, dy);
            int straight = dx + dy;
            return weight * (2 * diagonal + (straight - 2 * diagonal));
        }

        /// <summary>
        /// Calculates custom heuristic - can be overridden for specialized needs
        /// Default implementation returns Euclidean
        /// </summary>
        public static int CalculateCustom(int dx, int dy, int weight)
        {
            return (int)(weight * Math.Sqrt(dx * dx + dy * dy));
        }

        /// <summary>
        /// Calculates real distance in centimeters based on grid distance and scale
        /// </summary>
        public static double ToRealDistance(int gridDistance, double scaleCmPerCell)
        {
            return gridDistance * scaleCmPerCell;
        }

        /// <summary>
        /// Converts real distance in centimeters to grid distance
        /// </summary>
        public static int ToGridDistance(double realDistanceCm, double scaleCmPerCell)
        {
            return (int)Math.Ceiling(realDistanceCm / scaleCmPerCell);
        }
        #endregion
    }
}
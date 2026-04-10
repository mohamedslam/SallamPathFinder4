#region File Header
/// <summary>
/// File: MathHelper.cs
/// Description: Extended mathematical utility functions
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Helpers
{
    #region Class Documentation
    /// <summary>
    /// Static utility class for mathematical operations
    /// Provides helper methods for geometry, statistics, and calculations
    /// </summary>
    #endregion
    public static class MathHelper
    {
        #region Constants
        private const double SQRT2 = 1.4142135623730951;
        private const double DEG_TO_RAD = Math.PI / 180.0;
        private const double RAD_TO_DEG = 180.0 / Math.PI;
        #endregion

        #region Angle Conversion
        /// <summary>Converts degrees to radians</summary>
        public static double ToRadians(double degrees) => degrees * DEG_TO_RAD;

        /// <summary>Converts radians to degrees</summary>
        public static double ToDegrees(double radians) => radians * RAD_TO_DEG;

        /// <summary>Normalizes an angle to [0, 360) range</summary>
        public static double NormalizeAngle(double angle)
        {
            angle %= 360;
            return angle < 0 ? angle + 360 : angle;
        }

        /// <summary>Gets the shortest angle difference between two angles</summary>
        public static double AngleDifference(double angle1, double angle2)
        {
            double diff = NormalizeAngle(angle1 - angle2);
            return diff > 180 ? 360 - diff : diff;
        }
        #endregion

        #region Distance Calculations
        /// <summary>Euclidean distance between two points</summary>
        public static double EuclideanDistance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>Manhattan distance between two points</summary>
        public static int ManhattanDistance(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        /// <summary>Chebyshev distance between two points</summary>
        public static int ChebyshevDistance(Point a, Point b)
        {
            return Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
        }

        /// <summary>Octile distance (diagonal shortcut)</summary>
        public static int OctileDistance(Point a, Point b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            int diagonal = Math.Min(dx, dy);
            int straight = dx + dy;
            return (int)(2 * diagonal + (straight - 2 * diagonal));
        }
        #endregion

        #region Clamping
        /// <summary>Clamps a value between min and max</summary>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }

        /// <summary>Clamps an integer value</summary>
        public static int Clamp(int value, int min, int max) => Math.Max(min, Math.Min(max, value));

        /// <summary>Clamps a double value</summary>
        public static double Clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));
        #endregion

        #region Statistics
        /// <summary>Calculates the average of a list of values</summary>
        public static double Average(IEnumerable<double> values)
        {
            var list = values.ToList();
            return list.Count == 0 ? 0 : list.Sum() / list.Count;
        }

        /// <summary>Calculates the standard deviation of a list of values</summary>
        public static double StandardDeviation(IEnumerable<double> values)
        {
            var list = values.ToList();
            if (list.Count == 0) return 0;

            double avg = list.Average();
            double sumOfSquares = list.Sum(v => Math.Pow(v - avg, 2));
            return Math.Sqrt(sumOfSquares / list.Count);
        }

        /// <summary>Calculates the median of a list of values</summary>
        public static double Median(IEnumerable<double> values)
        {
            var sorted = values.OrderBy(v => v).ToList();
            if (sorted.Count == 0) return 0;
            int mid = sorted.Count / 2;
            return sorted.Count % 2 == 0 ? (sorted[mid - 1] + sorted[mid]) / 2 : sorted[mid];
        }
        #endregion

        #region Linear Interpolation
        /// <summary>Linear interpolation between two values</summary>
        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * Clamp(t, 0, 1);
        }

        /// <summary>Linear interpolation between two points</summary>
        public static Point Lerp(Point a, Point b, double t)
        {
            return new Point(
                (int)Lerp(a.X, b.X, t),
                (int)Lerp(a.Y, b.Y, t));
        }
        #endregion

        #region Smoothing
        /// <summary>Moving average smoothing of a list of points</summary>
        public static List<Point> SmoothPath(List<Point> path, int windowSize = 3)
        {
            if (path.Count < windowSize) return new List<Point>(path);

            var smoothed = new List<Point>();
            for (int i = 0; i < path.Count; i++)
            {
                int start = Math.Max(0, i - windowSize / 2);
                int end = Math.Min(path.Count - 1, i + windowSize / 2);
                int count = end - start + 1;

                int sumX = 0, sumY = 0;
                for (int j = start; j <= end; j++)
                {
                    sumX += path[j].X;
                    sumY += path[j].Y;
                }
                smoothed.Add(new Point(sumX / count, sumY / count));
            }
            return smoothed;
        }
        #endregion

        #region Probability
        /// <summary>Random value with Gaussian (normal) distribution</summary>
        public static double GaussianRandom(Random random, double mean = 0, double stdDev = 1)
        {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }
        #endregion
    }
}
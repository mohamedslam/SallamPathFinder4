#region File Header
/// <summary>
/// File: ObstaclePriority.cs
/// Description: Defines priority levels for different obstacle types
/// Used for decision making when multiple obstacles are detected
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-06-01
/// </summary>
#endregion

namespace SallamPathFinder4.Core.Enums
{
    /// <summary>
    /// Priority levels for obstacle avoidance
    /// Higher priority = more urgent response required
    /// </summary>
    public enum ObstaclePriority
    {
        /// <summary>Lowest priority - static obstacles, equipment</summary>
        Low = 10,

        /// <summary>Medium-low priority - other robots</summary>
        MediumLow = 30,

        /// <summary>Medium priority - animals</summary>
        Medium = 50,

        /// <summary>Medium-high priority - adults</summary>
        MediumHigh = 70,

        /// <summary>High priority - children</summary>
        High = 90,

        /// <summary>Critical priority - emergency (collision imminent)</summary>
        Critical = 100
    }

    /// <summary>
    /// Helper class for obstacle priority operations
    /// </summary>
    public static class ObstaclePriorityHelper
    {
        /// <summary>
        /// Gets priority level for an obstacle type
        /// </summary>
        public static ObstaclePriority GetPriority(ObstacleType type)
        {
            return type switch
            {
                ObstacleType.Child => ObstaclePriority.High,
                ObstacleType.Adult => ObstaclePriority.MediumHigh,
                ObstacleType.Animal => ObstaclePriority.Medium,
                ObstacleType.OtherRobot => ObstaclePriority.MediumLow,
                ObstacleType.Equipment => ObstaclePriority.Low,
                _ => ObstaclePriority.Low
            };
        }

        /// <summary>
        /// Gets priority value as integer (0-100)
        /// </summary>
        public static int GetPriorityValue(ObstaclePriority priority)
        {
            return (int)priority;
        }

        /// <summary>
        /// Gets priority value for an obstacle type
        /// </summary>
        public static int GetPriorityValue(ObstacleType type)
        {
            return GetPriorityValue(GetPriority(type));
        }

        /// <summary>
        /// Gets display color for priority level
        /// </summary>
        public static System.Drawing.Color GetPriorityColor(ObstaclePriority priority)
        {
            return priority switch
            {
                ObstaclePriority.Critical => System.Drawing.Color.FromArgb(255, 139, 0, 0),      // Dark Red
                ObstaclePriority.High => System.Drawing.Color.FromArgb(255, 231, 76, 60),       // Red
                ObstaclePriority.MediumHigh => System.Drawing.Color.FromArgb(255, 230, 126, 34), // Orange
                ObstaclePriority.Medium => System.Drawing.Color.FromArgb(255, 241, 196, 15),     // Yellow
                ObstaclePriority.MediumLow => System.Drawing.Color.FromArgb(255, 52, 152, 219),  // Blue
                ObstaclePriority.Low => System.Drawing.Color.FromArgb(255, 127, 140, 141),       // Gray
                _ => System.Drawing.Color.FromArgb(255, 149, 165, 166)                           // Light Gray
            };
        }

        /// <summary>
        /// Gets icon for priority level display
        /// </summary>
        public static string GetPriorityIcon(ObstaclePriority priority)
        {
            return priority switch
            {
                ObstaclePriority.Critical => "⚠️⚠️",
                ObstaclePriority.High => "⚠️",
                ObstaclePriority.MediumHigh => "❗",
                ObstaclePriority.Medium => "⚡",
                ObstaclePriority.MediumLow => "📌",
                ObstaclePriority.Low => "●",
                _ => "○"
            };
        }

        /// <summary>
        /// Gets recommended wait time in seconds for an obstacle type
        /// </summary>
        public static double GetRecommendedWaitTime(ObstacleType type)
        {
            return type switch
            {
                ObstacleType.Child => 5.0,
                ObstacleType.Adult => 3.0,
                ObstacleType.Animal => 2.0,
                ObstacleType.OtherRobot => 4.0,
                ObstacleType.Equipment => 1.0,
                _ => 2.0
            };
        }

        /// <summary>
        /// Gets maximum wait time in seconds for an obstacle type
        /// </summary>
        public static double GetMaxWaitTime(ObstacleType type)
        {
            return type switch
            {
                ObstacleType.Child => 10.0,
                ObstacleType.Adult => 8.0,
                ObstacleType.Animal => 5.0,
                ObstacleType.OtherRobot => 8.0,
                ObstacleType.Equipment => 3.0,
                _ => 5.0
            };
        }
    }
}
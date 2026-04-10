#region File Header
/// <summary>
/// File: ValidationHelper.cs
/// Description: Centralized validation utilities
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Helpers
{
    #region Class Documentation
    /// <summary>
    /// Static utility class for validation operations
    /// Centralizes all validation logic following DRY principle
    /// </summary>
    #endregion
    public static class ValidationHelper
    {
        #region Grid Validation
        /// <summary>
        /// Validates grid coordinates
        /// </summary>
        public static bool IsValidCell(MapGrid grid, int x, int y)
        {
            return grid != null && grid.IsValidCoordinate(x, y);
        }

        /// <summary>
        /// Validates grid coordinates with exception
        /// </summary>
        public static void EnsureValidCell(MapGrid grid, int x, int y, string paramName = null)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            if (!grid.IsValidCoordinate(x, y))
                throw new ArgumentOutOfRangeException(paramName ?? $"({x},{y})", $"Invalid coordinates: ({x},{y})");
        }

        /// <summary>
        /// Validates grid dimensions
        /// </summary>
        public static bool IsValidGridSize(int width, int height)
        {
            return width >= CommonConstants.MIN_GRID_SIZE && width <= CommonConstants.MAX_GRID_SIZE &&
                   height >= CommonConstants.MIN_GRID_SIZE && height <= CommonConstants.MAX_GRID_SIZE;
        }
        #endregion

        #region Path Validation
        /// <summary>
        /// Validates that a path is continuous and valid
        /// </summary>
        public static bool IsValidPath(IEnumerable<PathNode> path, MapGrid grid)
        {
            if (path == null) return false;

            var list = path.ToList();
            if (list.Count == 0) return false;

            for (int i = 0; i < list.Count; i++)
            {
                if (!grid.IsValidCoordinate(list[i].X, list[i].Y))
                    return false;

                if (!grid[list[i].X, list[i].Y].IsWalkable)
                    return false;

                if (i > 0)
                {
                    int dx = Math.Abs(list[i].X - list[i - 1].X);
                    int dy = Math.Abs(list[i].Y - list[i - 1].Y);
                    if (dx > 1 || dy > 1 || (dx == 0 && dy == 0))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if start and end are valid for pathfinding
        /// </summary>
        public static bool AreValidPathPoints(MapGrid grid, Point start, Point end, out string error)
        {
            error = null;

            if (!grid.IsValidCoordinate(start.X, start.Y))
            {
                error = "Start position is outside grid bounds";
                return false;
            }

            if (!grid.IsValidCoordinate(end.X, end.Y))
            {
                error = "End position is outside grid bounds";
                return false;
            }

            if (!grid[start.X, start.Y].IsWalkable)
            {
                error = "Start position is not walkable";
                return false;
            }

            if (!grid[end.X, end.Y].IsWalkable)
            {
                error = "End position is not walkable";
                return false;
            }

            return true;
        }
        #endregion

        #region Robot Validation
        /// <summary>
        /// Validates robot settings
        /// </summary>
        public static bool AreValidRobotSettings(double widthCm, double lengthCm, double heightCm, double speedCmS, out string error)
        {
            error = null;

            if (widthCm <= 0 || widthCm > 200)
            {
                error = "Robot width must be between 1 and 200 cm";
                return false;
            }

            if (lengthCm <= 0 || lengthCm > 200)
            {
                error = "Robot length must be between 1 and 200 cm";
                return false;
            }

            if (heightCm <= 0 || heightCm > 150)
            {
                error = "Robot height must be between 1 and 150 cm";
                return false;
            }

            if (speedCmS <= 0 || speedCmS > 100)
            {
                error = "Robot speed must be between 1 and 100 cm/s";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates battery level
        /// </summary>
        public static bool HasEnoughBattery(double batteryPercent, double requiredPercent)
        {
            return batteryPercent >= requiredPercent;
        }

        /// <summary>
        /// Gets battery status message
        /// </summary>
        public static string GetBatteryStatus(double batteryPercent)
        {
            if (batteryPercent <= 0)
                return "EMPTY - Robot stopped";
            if (batteryPercent < CommonConstants.CRITICAL_BATTERY_THRESHOLD)
                return "CRITICAL - Find charging station immediately";
            if (batteryPercent < CommonConstants.LOW_BATTERY_THRESHOLD)
                return "LOW - Consider recharging soon";
            return "NORMAL - OK";
        }
        #endregion

        #region Surface Validation
        /// <summary>
        /// Validates surface weight
        /// </summary>
        public static bool IsValidSurfaceWeight(byte weight)
        {
            return weight >= CommonConstants.MIN_SURFACE_WEIGHT &&
                   weight <= CommonConstants.MAX_SURFACE_WEIGHT;
        }

        /// <summary>
        /// Validates ramp difficulty
        /// </summary>
        public static bool IsValidRampDifficulty(byte difficulty)
        {
            return difficulty >= CommonConstants.MIN_RAMP_DIFFICULTY &&
                   difficulty <= CommonConstants.MAX_RAMP_DIFFICULTY;
        }
        #endregion

        #region Parameter Validation
        /// <summary>
        /// Ensures parameter is not null
        /// </summary>
        public static T EnsureNotNull<T>(T value, string paramName) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
            return value;
        }

        /// <summary>
        /// Ensures string is not null or empty
        /// </summary>
        public static string EnsureNotEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"{paramName} cannot be null or empty", paramName);
            return value;
        }

        /// <summary>
        /// Ensures value is within range
        /// </summary>
        public static int EnsureInRange(int value, int min, int max, string paramName)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName, $"Value must be between {min} and {max}");
            return value;
        }

        /// <summary>
        /// Ensures double value is within range
        /// </summary>
        public static double EnsureInRange(double value, double min, double max, string paramName)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName, $"Value must be between {min} and {max}");
            return value;
        }
        #endregion
    }
}
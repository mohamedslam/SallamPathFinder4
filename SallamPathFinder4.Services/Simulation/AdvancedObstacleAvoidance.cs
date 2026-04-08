#region File Header
/// <summary>
/// File: AdvancedObstacleAvoidance.cs
/// Description: Advanced obstacle avoidance strategies for dynamic obstacles
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Path;
#endregion

namespace SallamPathFinder4.Services.Simulation
{
    #region Result Classes
    public sealed class AvoidanceResult
    {
        public bool Success { get; set; }
        public AvoidanceAction Action { get; set; }
        public Point NewPosition { get; set; }
        public float NewAngle { get; set; }
        public string Message { get; set; }
        public double WaitTimeSeconds { get; set; }
    }

    public enum AvoidanceAction
    {
        None,
        Stop,
        SlowDown,
        TurnLeft,
        TurnRight,
        StepBack,
        Wait,
        Replan
    }
    #endregion

    #region Class Documentation
    /// <summary>
    /// Advanced obstacle avoidance strategies for dynamic obstacles
    /// Provides context-aware decisions based on obstacle type and behavior
    /// </summary>
    #endregion
    public sealed class AdvancedObstacleAvoidance
    {
        #region Constants
        private const double SAFE_DISTANCE_CELLS = 1.5;
        private const double WARNING_DISTANCE_CELLS = 2.5;
        private const double SLOW_DOWN_FACTOR = 0.5;
        private const double WAIT_MAX_SECONDS = 5.0;
        #endregion

        #region Private Fields
        private readonly Random _random = new Random();
        #endregion

        #region Public Methods
        /// <summary>
        /// Determines the best avoidance action based on obstacle type and distance
        /// </summary>
        public async Task<AvoidanceResult> DetermineAvoidanceActionAsync(
            DynamicObstacle obstacle, Point robotPosition, float robotAngle, double currentSpeed)
        {
            return await Task.Run(() =>
            {
                double distance = Math.Sqrt(
                    Math.Pow(obstacle.Location.X - robotPosition.X, 2) +
                    Math.Pow(obstacle.Location.Y - robotPosition.Y, 2));

                // Determine action based on distance and obstacle type
                if (distance <= SAFE_DISTANCE_CELLS)
                {
                    return HandleImminentCollision(obstacle, robotPosition, robotAngle);
                }

                if (distance <= WARNING_DISTANCE_CELLS)
                {
                    return HandleWarningDistance(obstacle, robotPosition, robotAngle, currentSpeed);
                }

                return new AvoidanceResult
                {
                    Success = true,
                    Action = AvoidanceAction.None,
                    Message = "No action needed - obstacle at safe distance"
                };
            });
        }

        /// <summary>
        /// Gets the priority weight for different obstacle types
        /// </summary>
        public double GetObstaclePriorityWeight(ObstacleType type)
        {
            return type switch
            {
                ObstacleType.Child => 1.0,      // Highest priority
                ObstacleType.Animal => 0.9,
                ObstacleType.Adult => 0.8,
                ObstacleType.OtherRobot => 0.6,
                ObstacleType.Equipment => 0.4,
                _ => 0.5
            };
        }

        /// <summary>
        /// Checks if there is an alternative path around the obstacle
        /// </summary>
        public bool HasAlternativePath(Point currentPosition, Point goal,
            List<Point> blockedCells, MapGrid grid, int maxAttempts = 5)
        {
            var directions = new (int dx, int dy)[]
            {
                (1, 0), (-1, 0), (0, 1), (0, -1),
                (1, 1), (1, -1), (-1, 1), (-1, -1)
            };

            foreach (var dir in directions)
            {
                Point newPos = new Point(currentPosition.X + dir.dx, currentPosition.Y + dir.dy);

                if (!grid.IsValidCoordinate(newPos.X, newPos.Y))
                    continue;

                if (blockedCells.Contains(newPos))
                    continue;

                if (grid[newPos.X, newPos.Y].IsWalkable)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates recommended wait time based on obstacle type
        /// </summary>
        public double GetRecommendedWaitTime(ObstacleType type)
        {
            return type switch
            {
                ObstacleType.Adult => 2.0,
                ObstacleType.Child => 1.0,   // Children move faster
                ObstacleType.Animal => 1.5,
                ObstacleType.OtherRobot => 3.0,
                ObstacleType.Equipment => 4.0, // Slow moving
                _ => 2.0
            };
        }
        #endregion

        #region Private Methods
        private AvoidanceResult HandleImminentCollision(DynamicObstacle obstacle,
            Point robotPosition, float robotAngle)
        {
            // Determine escape direction (opposite to obstacle approach)
            int dx = robotPosition.X - obstacle.Location.X;
            int dy = robotPosition.Y - obstacle.Location.Y;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                // Move horizontally away
                int newX = robotPosition.X + Math.Sign(dx);
                return new AvoidanceResult
                {
                    Success = true,
                    Action = dx > 0 ? AvoidanceAction.TurnRight : AvoidanceAction.TurnLeft,
                    NewPosition = new Point(newX, robotPosition.Y),
                    Message = $"Emergency avoidance from {obstacle.Type}"
                };
            }
            else
            {
                // Move vertically away
                int newY = robotPosition.Y + Math.Sign(dy);
                return new AvoidanceResult
                {
                    Success = true,
                    Action = AvoidanceAction.StepBack,
                    NewPosition = new Point(robotPosition.X, newY),
                    Message = $"Emergency step back from {obstacle.Type}"
                };
            }
        }

        private AvoidanceResult HandleWarningDistance(DynamicObstacle obstacle,
            Point robotPosition, float robotAngle, double currentSpeed)
        {
            // Slower, more cautious approach for children and animals
            bool isHighPriority = obstacle.Type == ObstacleType.Child ||
                                  obstacle.Type == ObstacleType.Animal;

            if (isHighPriority)
            {
                return new AvoidanceResult
                {
                    Success = true,
                    Action = AvoidanceAction.SlowDown,
                    Message = $"Slowing down for {obstacle.Type} ahead",
                    WaitTimeSeconds = 0
                };
            }

            // For adults and other robots, try to go around
            return new AvoidanceResult
            {
                Success = true,
                Action = AvoidanceAction.TurnLeft,
                Message = $"Preparing to avoid {obstacle.Type}",
                WaitTimeSeconds = 0
            };
        }
        #endregion
    }
}
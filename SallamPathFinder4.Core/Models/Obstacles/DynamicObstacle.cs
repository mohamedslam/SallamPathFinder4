#region File Header
/// <summary>
/// File: DynamicObstacle.cs
/// Description: Represents a moving obstacle in the environment
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.Core.Models.Obstacles
{
    #region Class Documentation
    /// <summary>
    /// Represents a dynamic obstacle that can move through the environment
    /// Each obstacle type has unique movement properties and behavior
    /// Tracks trajectory for learning and prediction
    /// </summary>
    #endregion
    public sealed class DynamicObstacle
    {
        #region Constructor
        /// <summary>
        /// Creates a new dynamic obstacle of the specified type at the given location
        /// </summary>
        public DynamicObstacle(ObstacleType type, Point location)
        {
            Type = type;
            Location = location;
            Trajectory = new List<Point> { location };
            FirstDetected = DateTime.UtcNow;
            ResponseSuccessful = false;
            InitializePropertiesByType();
        }
        #endregion

        #region Properties
        /// <summary>Type of obstacle (Adult, Child, Animal, etc.)</summary>
        public ObstacleType Type { get; set; }

        /// <summary>Current location in grid coordinates</summary>
        public Point Location { get; set; }

        /// <summary>Movement speed in cells per second</summary>
        public double Speed { get; set; }

        /// <summary>Movement randomness factor (0-1)</summary>
        public double MovementRandomness { get; set; }

        /// <summary>Physical radius in cells (affects avoidance)</summary>
        public double Radius { get; set; }

        /// <summary>Physical height in meters</summary>
        public double Height { get; set; }

        /// <summary>Weight factor (0-1) affecting robot's reaction</summary>
        public double Weight { get; set; }

        /// <summary>Time when obstacle was first detected</summary>
        public DateTime FirstDetected { get; set; }

        /// <summary>Historical trajectory of the obstacle</summary>
        public List<Point> Trajectory { get; set; }

        /// <summary>Strategy used to respond to this obstacle</summary>
        public string ResponseStrategy { get; set; }

        /// <summary>Whether the response was successful</summary>
        public bool ResponseSuccessful { get; set; }

        /// <summary>Time since last seen in seconds</summary>
        public double TimeSinceLastSeen => (DateTime.UtcNow - LastSeen).TotalSeconds;

        /// <summary>Last time this obstacle was seen</summary>
        public DateTime LastSeen { get; set; }
        #endregion

        #region Private Methods
        private void InitializePropertiesByType()
        {
            switch (Type)
            {
                case ObstacleType.Adult:
                    Speed = 1;
                    MovementRandomness = 0.3;
                    Radius = 0.5;
                    Height = 1.7;
                    Weight = 0.8;
                    break;

                case ObstacleType.Child:
                    Speed = 1.2;
                    MovementRandomness = 0.6;
                    Radius = 0.4;
                    Height = 1.2;
                    Weight = 0.6;
                    break;

                case ObstacleType.Animal:
                    Speed = 1.0;
                    MovementRandomness = 0.5;
                    Radius = 0.5;
                    Height = 0.6;
                    Weight = 0.7;
                    break;

                case ObstacleType.OtherRobot:
                    Speed = 1.9;
                    MovementRandomness = 0.2;
                    Radius = 0.6;
                    Height = 0.8;
                    Weight = 0.9;
                    break;

                case ObstacleType.Equipment:
                    Speed = 1.4;
                    MovementRandomness = 0.1;
                    Radius = 0.8;
                    Height = 1.0;
                    Weight = 0.5;
                    break;

                default:
                    Speed = 0.5;
                    MovementRandomness = 0.3;
                    Radius = 0.5;
                    Height = 1.0;
                    Weight = 0.5;
                    break;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the obstacle's position and records trajectory
        /// </summary>
        public void UpdatePosition(Point newLocation)
        {
            Location = newLocation;
            Trajectory.Add(newLocation);
            LastSeen = DateTime.UtcNow;
        }

        /// <summary>
        /// Calculates the current movement direction in degrees
        /// </summary>
        public double GetCurrentDirection()
        {
            if (Trajectory.Count < 2) return 0;

            var prev = Trajectory[^2];
            var curr = Location;

            return Math.Atan2(curr.Y - prev.Y, curr.X - prev.X) * 180 / Math.PI;
        }

        /// <summary>
        /// Calculates the current speed based on recent trajectory
        /// </summary>
        public double GetCurrentSpeed()
        {
            if (Trajectory.Count < 2) return Speed;

            var prev = Trajectory[^2];
            var curr = Location;

            double dx = curr.X - prev.X;
            double dy = curr.Y - prev.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Resets the obstacle's trajectory (keep only current position)
        /// </summary>
        public void ResetTrajectory()
        {
            Trajectory.Clear();
            Trajectory.Add(Location);
        }

        /// <summary>
        /// Creates a deep copy of this obstacle
        /// </summary>
        public DynamicObstacle Clone()
        {
            var clone = new DynamicObstacle(Type, Location)
            {
                Speed = this.Speed,
                MovementRandomness = this.MovementRandomness,
                Radius = this.Radius,
                Height = this.Height,
                Weight = this.Weight,
                FirstDetected = this.FirstDetected,
                ResponseStrategy = this.ResponseStrategy,
                ResponseSuccessful = this.ResponseSuccessful,
                LastSeen = this.LastSeen
            };

            foreach (var point in this.Trajectory)
            {
                clone.Trajectory.Add(point);
            }

            return clone;
        }
        #endregion

        #region Object Overrides
        /// <summary>
        /// Returns string representation of the obstacle
        /// </summary>
        public override string ToString()
        {
            return $"{Type} at ({Location.X},{Location.Y}) | Speed: {Speed:F1} cells/s";
        }
        #endregion
    }
}
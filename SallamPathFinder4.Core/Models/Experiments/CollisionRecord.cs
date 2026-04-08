#region File Header
/// <summary>
/// File: CollisionRecord.cs
/// Description: Records collision events between robot and obstacles
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.Core.Models.Experiments
{
    #region Class Documentation
    /// <summary>
    /// Records a collision event between the robot and an obstacle
    /// Used for logging and analysis in experiments
    /// </summary>
    #endregion
    public sealed class CollisionRecord
    {
        #region Constructor
        /// <summary>
        /// Initializes a new collision record with current timestamp
        /// </summary>
        public CollisionRecord()
        {
            Timestamp = DateTime.UtcNow;
            StrategyUsed = "None";
            Success = false;
            ResolutionTime = 0;
        }

        /// <summary>
        /// Initializes a collision record with obstacle and position data
        /// </summary>
        public CollisionRecord(ObstacleType type, Point location, Point robotPos) : this()
        {
            ObstacleType = type;
            Location = location;
            RobotPosition = robotPos;
            StrategyUsed = "Avoidance attempted";
            Success = false;
            ResolutionTime = 0;
        }
        #endregion

        #region Properties
        /// <summary>Time when collision occurred (UTC)</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Type of obstacle involved in collision</summary>
        public ObstacleType ObstacleType { get; set; }

        /// <summary>Location of the obstacle at collision time</summary>
        public Point Location { get; set; }

        /// <summary>Robot position at collision time</summary>
        public Point RobotPosition { get; set; }

        /// <summary>Strategy used to avoid/respond to collision</summary>
        public string StrategyUsed { get; set; }

        /// <summary>Whether the avoidance strategy was successful</summary>
        public bool Success { get; set; }

        /// <summary>Time taken to resolve the collision (seconds)</summary>
        public double ResolutionTime { get; set; }

        /// <summary>Battery level at time of collision</summary>
        public double BatteryLevel { get; set; }

        /// <summary>Robot speed at time of collision (cm/s)</summary>
        public double RobotSpeed { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a deep copy of the collision record
        /// </summary>
        public CollisionRecord Clone()
        {
            return new CollisionRecord
            {
                Timestamp = this.Timestamp,
                ObstacleType = this.ObstacleType,
                Location = this.Location,
                RobotPosition = this.RobotPosition,
                StrategyUsed = this.StrategyUsed,
                Success = this.Success,
                ResolutionTime = this.ResolutionTime,
                BatteryLevel = this.BatteryLevel,
                RobotSpeed = this.RobotSpeed
            };
        }
        #endregion

        #region Object Overrides
        /// <summary>
        /// Returns formatted string for display
        /// </summary>
        public override string ToString()
        {
            string icon = ObstacleType switch
            {
                ObstacleType.Adult => "👤",
                ObstacleType.Child => "🧒",
                ObstacleType.Animal => "🐕",
                ObstacleType.OtherRobot => "🤖",
                ObstacleType.Equipment => "🔧",
                _ => "⚠️"
            };

            return $"[{Timestamp:HH:mm:ss}] {icon} {ObstacleType} at ({Location.X},{Location.Y})";
        }
        #endregion
    }
}
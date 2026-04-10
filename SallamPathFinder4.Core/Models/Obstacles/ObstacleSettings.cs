#region File Header
/// <summary>
/// File: ObstacleSettings.cs
/// Description: Configuration settings for dynamic obstacle behavior
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.Core.Models.Obstacles
{
    #region Enum Documentation
    /// <summary>
    /// How the robot responds when colliding with an obstacle
    /// </summary>
    #endregion
    public enum CollisionResponseType
    {
        StopRobot = 0,
        ReduceBattery = 1,
        WaitAndReroute = 2,
        LogOnly = 3
    }

    #region Class Documentation
    /// <summary>
    /// Settings for a specific obstacle type
    /// </summary>
    #endregion
    public sealed class ObstacleTypeSettings
    {
        public ObstacleTypeSettings()
        {
            Speed = 1.0;
            MovementRandomness = 0.3;
            Radius = 0.5;
            Weight = 0.5;
        }

        public ObstacleTypeSettings(double speed, double randomness, double radius, double weight)
        {
            Speed = speed;
            MovementRandomness = randomness;
            Radius = radius;
            Weight = weight;
        }

        public double Speed { get; set; }
        public double MovementRandomness { get; set; }
        public double Radius { get; set; }
        public double Weight { get; set; }
    }

    #region Class Documentation
    /// <summary>
    /// Complete settings for dynamic obstacle system
    /// </summary>
    #endregion
    public sealed class ObstacleSettings
    {
        public ObstacleSettings()
        {
            TypeSettings = new Dictionary<ObstacleType, ObstacleTypeSettings>();
            InitializeDefaultSettings();
        }

        public Dictionary<ObstacleType, ObstacleTypeSettings> TypeSettings { get; set; }
        public bool EnableRandomMovement { get; set; } = true;
        public bool FollowWaypoints { get; set; } = false;
        public bool AvoidRobot { get; set; } = false;
        public bool AttractToRobot { get; set; } = false;
        public double DirectionChangeProbability { get; set; } = 0.3;
        public double MaxTurnAngle { get; set; } = 90.0;
        public CollisionResponseType CollisionResponse { get; set; } = CollisionResponseType.LogOnly;
        public int InitialObstacleCount { get; set; } = 5;
        public int MaxObstacleCount { get; set; } = 20;
        public bool DynamicSpawning { get; set; } = false;
        public double SpawnIntervalSeconds { get; set; } = 10.0;

        private void InitializeDefaultSettings()
        {
            TypeSettings[ObstacleType.Adult] = new ObstacleTypeSettings(0.8, 0.3, 0.5, 0.8);
            TypeSettings[ObstacleType.Child] = new ObstacleTypeSettings(1.2, 0.6, 0.4, 0.6);
            TypeSettings[ObstacleType.Animal] = new ObstacleTypeSettings(1.0, 0.5, 0.5, 0.7);
            TypeSettings[ObstacleType.OtherRobot] = new ObstacleTypeSettings(0.9, 0.2, 0.6, 0.9);
            TypeSettings[ObstacleType.Equipment] = new ObstacleTypeSettings(0.4, 0.1, 0.8, 0.5);
        }

        public ObstacleSettings Clone()
        {
            var clone = new ObstacleSettings
            {
                EnableRandomMovement = this.EnableRandomMovement,
                FollowWaypoints = this.FollowWaypoints,
                AvoidRobot = this.AvoidRobot,
                AttractToRobot = this.AttractToRobot,
                DirectionChangeProbability = this.DirectionChangeProbability,
                MaxTurnAngle = this.MaxTurnAngle,
                CollisionResponse = this.CollisionResponse,
                InitialObstacleCount = this.InitialObstacleCount,
                MaxObstacleCount = this.MaxObstacleCount,
                DynamicSpawning = this.DynamicSpawning,
                SpawnIntervalSeconds = this.SpawnIntervalSeconds
            };

            foreach (var kvp in TypeSettings)
            {
                clone.TypeSettings[kvp.Key] = new ObstacleTypeSettings(
                    kvp.Value.Speed,
                    kvp.Value.MovementRandomness,
                    kvp.Value.Radius,
                    kvp.Value.Weight);
            }

            return clone;
        }
    }
}
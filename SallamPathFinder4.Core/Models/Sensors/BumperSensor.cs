#region File Header
/// <summary>
/// File: BumperSensor.cs
/// Description: Bumper sensor for collision detection
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Map;
using System.Drawing;

namespace SallamPathFinder4.Core.Models.Sensors
{
    /// <summary>
    /// Bumper sensor for physical collision detection
    /// </summary>
    public class BumperSensor : SensorBase
    {
        #region Properties
        public override SensorType Type => SensorType.Bumper;
        public override Color DisplayColor => Color.FromArgb(230, 126, 34); // Orange

        /// <summary>Whether the bumper is currently pressed</summary>
        public bool IsPressed { get; private set; }

        /// <summary>Number of collisions detected</summary>
        public int CollisionCount { get; set; }

        /// <summary>Bumper width in cm</summary>
        public double BumperWidth { get; set; } = 30;

        /// <summary>Bumper sensitivity (0-1, higher = more sensitive)</summary>
        public double Sensitivity { get; set; } = 0.8;
        #endregion

        #region Constructor
        public BumperSensor()
        {
            SensorName = "Bumper Sensor";
            MaxRange = 5;  // Bumper only triggers on contact
            FieldOfView = 180;
            PowerConsumption = 0.01;
        }
        #endregion

        #region Public Methods
        public override async Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle)
        {
            var result = new DetectionResult();

            // Check for collision at robot's front based on MountAngle
            double radAngle = (robotAngle + MountAngle) * Math.PI / 180.0;
            double frontX = robotPosition.X + (BumperWidth / 2) * Math.Cos(radAngle);
            double frontY = robotPosition.Y + (BumperWidth / 2) * Math.Sin(radAngle);

            int checkX = (int)frontX;
            int checkY = (int)frontY;

            if (grid.IsValidCoordinate(checkX, checkY))
            {
                var cell = grid[checkX, checkY];
                if (!cell.IsWalkable || cell.OccupyingObstacle != null)
                {
                    IsPressed = true;
                    CollisionCount++;
                    result.ObstacleDetected = true;
                    result.Distance = 0;
                    result.ObstaclePosition = new Point(checkX, checkY);
                    result.Confidence = 1.0;
                }
                else
                {
                    IsPressed = false;
                }
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Resets the collision counter
        /// </summary>
        public void ResetCollisionCount()
        {
            CollisionCount = 0;
        }

        public override string GetReadingAsString()
        {
            return $"Bumper: {(IsPressed ? "PRESSED" : "Released")}, Collisions: {CollisionCount}";
        }

        public override ISensor Clone()
        {
            return new BumperSensor
            {
                SensorId = this.SensorId,
                SensorName = this.SensorName,
                Position = this.Position,
                MountAngle = this.MountAngle,
                IsEnabled = this.IsEnabled,
                PowerConsumption = this.PowerConsumption,
                BumperWidth = this.BumperWidth,
                Sensitivity = this.Sensitivity
            };
        }
        #endregion
    }
}
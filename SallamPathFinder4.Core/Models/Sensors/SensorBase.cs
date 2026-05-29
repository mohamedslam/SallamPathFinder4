#region File Header
/// <summary>
/// File: SensorBase.cs
/// Description: Abstract base class for all sensors
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Map;
using System.Drawing;

namespace SallamPathFinder4.Core.Models.Sensors
{
    /// <summary>
    /// Abstract base class for all sensor implementations
    /// </summary>
    public abstract class SensorBase : ISensor
    {
        #region Properties
        public string SensorId { get; set; } = Guid.NewGuid().ToString();
        public string SensorName { get; set; }
        public abstract SensorType Type { get; }
        public Point Position { get; set; }
        public double MountAngle { get; set; }
        public bool IsEnabled { get; set; } = true;
        public virtual Color DisplayColor => Color.Gray;
        public virtual double MaxRange { get; set; } = 500;
        public virtual double FieldOfView { get; set; } = 30;
        public virtual double UpdateRate { get; set; } = 10;
        public virtual double PowerConsumption { get; set; } = 0.5;
        #endregion

        #region Abstract Methods
        public abstract Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle);
        public abstract string GetReadingAsString();
        public abstract ISensor Clone();
        #endregion

        #region Virtual Methods
        public virtual List<Point> GetCoveragePolygon(Point robotPosition, double robotAngle, double scale = 1.0)
        {
            var points = new List<Point>();
            double radAngle = (robotAngle + MountAngle) * Math.PI / 180.0;

            double halfFOV = FieldOfView / 2.0;
            double startAngle = (robotAngle + MountAngle - halfFOV) * Math.PI / 180.0;
            double endAngle = (robotAngle + MountAngle + halfFOV) * Math.PI / 180.0;

            // Calculate sensor position in world coordinates
            double sensorX = robotPosition.X + Position.X * Math.Cos(radAngle) - Position.Y * Math.Sin(radAngle);
            double sensorY = robotPosition.Y + Position.X * Math.Sin(radAngle) + Position.Y * Math.Cos(radAngle);

            points.Add(new Point((int)(sensorX * scale), (int)(sensorY * scale)));

            // Add arc points
            int arcPoints = 20;
            for (int i = 0; i <= arcPoints; i++)
            {
                double angle = startAngle + (endAngle - startAngle) * i / arcPoints;
                double x = (sensorX + MaxRange * Math.Cos(angle)) * scale;
                double y = (sensorY + MaxRange * Math.Sin(angle)) * scale;
                points.Add(new Point((int)x, (int)y));
            }

            return points;
        }
        #endregion

        #region Helper Methods
        protected virtual async Task<DetectionResult> PerformRayCast(MapGrid grid, Point start, double angle, double maxRange)
        {
            var result = new DetectionResult();
            double radAngle = angle * Math.PI / 180.0;

            for (double d = 0; d <= maxRange; d += 2)
            {
                int x = (int)(start.X + d * Math.Cos(radAngle));
                int y = (int)(start.Y + d * Math.Sin(radAngle));

                if (!grid.IsValidCoordinate(x, y))
                    break;

                var cell = grid[x, y];
                if (!cell.IsWalkable || cell.OccupyingObstacle != null)
                {
                    result.ObstacleDetected = true;
                    result.Distance = d;
                    result.ObstaclePosition = new Point(x, y);
                    result.ObstacleType = cell.ElementType.ToString();
                    break;
                }
            }

            return await Task.FromResult(result);
        }
        #endregion
    }
}
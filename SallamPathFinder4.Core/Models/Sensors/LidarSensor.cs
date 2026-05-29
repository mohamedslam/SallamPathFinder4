#region File Header
/// <summary>
/// File: LidarSensor.cs
/// Description: LiDAR sensor for 360-degree scanning
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Map;
using System.Drawing;

namespace SallamPathFinder4.Core.Models.Sensors
{
    /// <summary>
    /// LiDAR sensor for high-resolution 360-degree scanning
    /// </summary>
    public class LidarSensor : SensorBase
    {
        #region Properties
        public override SensorType Type => SensorType.Lidar;
        public override Color DisplayColor => Color.FromArgb(44, 62, 80); // Dark Gray

        /// <summary>Number of points generated per second</summary>
        public int PointsPerSecond { get; set; } = 20000;

        /// <summary>Angular resolution in degrees</summary>
        public double AngularResolution { get; set; } = 0.5;

        /// <summary>Full point cloud data</summary>
        public List<DetectionResult> PointCloud { get; set; } = new List<DetectionResult>();

        /// <summary>Number of scan layers (for 3D LiDAR)</summary>
        public int Layers { get; set; } = 1;
        #endregion

        #region Constructor
        public LidarSensor()
        {
            SensorName = "LiDAR Sensor";
            MaxRange = 10000;
            FieldOfView = 360;
            PowerConsumption = 1.2;
        }
        #endregion

        #region Public Methods
        public override async Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle)
        {
            var result = new DetectionResult();
            PointCloud.Clear();
            double minDistance = MaxRange;
            Point? closestPoint = null;

            double radAngle = (robotAngle + MountAngle) * Math.PI / 180.0;
            double sensorX = robotPosition.X + Position.X * Math.Cos(radAngle) - Position.Y * Math.Sin(radAngle);
            double sensorY = robotPosition.Y + Position.X * Math.Sin(radAngle) + Position.Y * Math.Cos(radAngle);

            // For each layer
            for (int layer = 0; layer < Layers; layer++)
            {
                double layerAngleOffset = (layer - (Layers - 1) / 2.0) * AngularResolution;

                // For each angle
                int numRays = (int)(FieldOfView / AngularResolution);
                for (int i = 0; i <= numRays; i++)
                {
                    double angle = robotAngle + MountAngle - FieldOfView / 2 + i * AngularResolution + layerAngleOffset;
                    double angleRad = angle * Math.PI / 180.0;

                    for (double d = 0; d <= MaxRange; d += 5)
                    {
                        int x = (int)(sensorX + d * Math.Cos(angleRad));
                        int y = (int)(sensorY + d * Math.Sin(angleRad));

                        if (!grid.IsValidCoordinate(x, y))
                            break;

                        var cell = grid[x, y];
                        if (!cell.IsWalkable || cell.OccupyingObstacle != null)
                        {
                            var point = new DetectionResult
                            {
                                ObstacleDetected = true,
                                Distance = d,
                                Angle = angle,
                                ObstaclePosition = new Point(x, y),
                                ObstacleType = cell.ElementType.ToString(),
                                Confidence = 0.98
                            };
                            PointCloud.Add(point);

                            if (d < minDistance)
                            {
                                minDistance = d;
                                closestPoint = new Point(x, y);
                            }
                            break;
                        }
                    }
                }
            }

            result.ObstacleDetected = minDistance < MaxRange;
            result.Distance = minDistance;
            result.ObstaclePosition = closestPoint;
            result.Confidence = 0.98;

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Gets the point cloud as a list of points for visualization
        /// </summary>
        public List<Point> GetPointCloudPoints()
        {
            return PointCloud.Where(p => p.ObstacleDetected && p.ObstaclePosition.HasValue)
                            .Select(p => p.ObstaclePosition.Value)
                            .ToList();
        }

        public override string GetReadingAsString()
        {
            return $"LiDAR: {PointsPerSecond} pts/sec, {AngularResolution:F1}° res, {Layers} layers, Range={MaxRange}cm";
        }

        public override ISensor Clone()
        {
            return new LidarSensor
            {
                SensorId = this.SensorId,
                SensorName = this.SensorName,
                Position = this.Position,
                MountAngle = this.MountAngle,
                IsEnabled = this.IsEnabled,
                MaxRange = this.MaxRange,
                FieldOfView = this.FieldOfView,
                UpdateRate = this.UpdateRate,
                PowerConsumption = this.PowerConsumption,
                PointsPerSecond = this.PointsPerSecond,
                AngularResolution = this.AngularResolution,
                Layers = this.Layers
            };
        }
        #endregion
    }
}
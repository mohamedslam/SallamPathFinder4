#region File Header
/// <summary>
/// File: ProximitySensor.cs
/// Description: Proximity sensor for detecting nearby objects and spatial awareness
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Map;
using System.Drawing;

namespace SallamPathFinder4.Core.Models.Sensors
{
    /// <summary>
    /// Proximity sensor for spatial awareness and 3D perception
    /// </summary>
    public class ProximitySensor : SensorBase
    {
        #region Properties
        public override SensorType Type => SensorType.Proximity;
        public override Color DisplayColor => Color.FromArgb(241, 196, 15); // Yellow

        /// <summary>Threshold distance for proximity alert (cm)</summary>
        public double ProximityThreshold { get; set; } = 50;

        /// <summary>List of nearby objects detected</summary>
        public List<ProximityData> NearbyObjects { get; set; } = new List<ProximityData>();

        /// <summary>3D depth perception enabled</summary>
        public bool EnableDepthPerception { get; set; } = true;

        /// <summary>Scan resolution in degrees</summary>
        public double ScanResolution { get; set; } = 10;
        #endregion

        #region Constructor
        public ProximitySensor()
        {
            SensorName = "Proximity Sensor";
            MaxRange = 200;
            FieldOfView = 180;
            PowerConsumption = 0.1;
        }
        #endregion

        #region Public Methods
        public override async Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle)
        {
            var result = new DetectionResult();
            NearbyObjects.Clear();

            double radAngle = robotAngle * Math.PI / 180.0;
            double sensorX = robotPosition.X + Position.X * Math.Cos(radAngle) - Position.Y * Math.Sin(radAngle);
            double sensorY = robotPosition.Y + Position.X * Math.Sin(radAngle) + Position.Y * Math.Cos(radAngle);

            double minDistance = MaxRange;
            Point? closestPoint = null;

            // Scan in a cone from -FOV/2 to +FOV/2
            int numRays = (int)(FieldOfView / ScanResolution);
            for (int i = 0; i <= numRays; i++)
            {
                double scanAngle = robotAngle + MountAngle - FieldOfView / 2 + i * ScanResolution;
                double scanRad = scanAngle * Math.PI / 180.0;

                for (double d = 0; d <= MaxRange; d += 2)
                {
                    int x = (int)(sensorX + d * Math.Cos(scanRad));
                    int y = (int)(sensorY + d * Math.Sin(scanRad));

                    if (!grid.IsValidCoordinate(x, y))
                        break;

                    var cell = grid[x, y];
                    if (!cell.IsWalkable || cell.OccupyingObstacle != null)
                    {
                        double dx = x - robotPosition.X;
                        double dy = y - robotPosition.Y;
                        double bearing = Math.Atan2(dy, dx) * 180 / Math.PI - robotAngle;
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        var proximityData = new ProximityData
                        {
                            TargetPosition = new Point(x, y),
                            Distance = distance,
                            Bearing = bearing,
                            ObjectType = cell.ElementType.ToString(),
                            RelativeVelocity = 0
                        };

                        NearbyObjects.Add(proximityData);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestPoint = new Point(x, y);
                        }
                        break;
                    }
                }
            }

            result.ObstacleDetected = minDistance < MaxRange;
            result.Distance = minDistance;
            result.ObstaclePosition = closestPoint;
            result.Angle = MountAngle;
            result.Confidence = result.ObstacleDetected ? 0.9 : 0.95;

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Gets the minimum distance to any detected object
        /// </summary>
        public double GetMinimumDistance()
        {
            return NearbyObjects.Any() ? NearbyObjects.Min(o => o.Distance) : MaxRange;
        }

        /// <summary>
        /// Gets all objects within the proximity threshold
        /// </summary>
        public List<ProximityData> GetThreats()
        {
            return NearbyObjects.Where(o => o.Distance <= ProximityThreshold).ToList();
        }

        public override string GetReadingAsString()
        {
            return $"Proximity: {NearbyObjects.Count} objects, Closest: {GetMinimumDistance():F1}cm, Threats: {GetThreats().Count}";
        }

        public override ISensor Clone()
        {
            return new ProximitySensor
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
                ProximityThreshold = this.ProximityThreshold,
                EnableDepthPerception = this.EnableDepthPerception,
                ScanResolution = this.ScanResolution
            };
        }
        #endregion
    }
}
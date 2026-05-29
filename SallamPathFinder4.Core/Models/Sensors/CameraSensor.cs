#region File Header
/// <summary>
/// File: CameraSensor.cs
/// Description: Camera sensor for visual perception
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Map;
using System.Drawing;

namespace SallamPathFinder4.Core.Models.Sensors
{
    /// <summary>
    /// Camera sensor for object detection and tracking
    /// </summary>
    public class CameraSensor : SensorBase
    {
        #region Properties
        public override SensorType Type => SensorType.Camera;
        public override Color DisplayColor => Color.FromArgb(155, 89, 182); // Purple

        /// <summary>Resolution width in pixels</summary>
        public int ResolutionWidth { get; set; } = 1920;

        /// <summary>Resolution height in pixels</summary>
        public int ResolutionHeight { get; set; } = 1080;

        /// <summary>Frame rate in FPS</summary>
        public double FrameRate { get; set; } = 30;

        /// <summary>Detected object types (if object detection is enabled)</summary>
        public List<string> DetectedObjects { get; set; } = new List<string>();

        /// <summary>Whether object detection is enabled</summary>
        public bool EnableObjectDetection { get; set; } = false;

        /// <summary>Whether color detection is enabled</summary>
        public bool EnableColorDetection { get; set; } = false;
        #endregion

        #region Constructor
        public CameraSensor()
        {
            SensorName = "Camera Sensor";
            MaxRange = 1000;
            FieldOfView = 90;
            PowerConsumption = 0.5;
        }
        #endregion

        #region Public Methods
        public override async Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle)
        {
            var result = new DetectionResult();

            // Camera can detect goals and special markers
            double radAngle = (robotAngle + MountAngle) * Math.PI / 180.0;
            double sensorX = robotPosition.X + Position.X * Math.Cos(radAngle) - Position.Y * Math.Sin(radAngle);
            double sensorY = robotPosition.Y + Position.X * Math.Sin(radAngle) + Position.Y * Math.Cos(radAngle);

            double halfFOVRad = (FieldOfView / 2.0) * Math.PI / 180.0;
            double startAngle = radAngle - halfFOVRad;
            double endAngle = radAngle + halfFOVRad;

            // Simulate camera detection - checks for goals within FOV
            // In a real implementation, this would use image processing

            result.ObstacleDetected = false;
            result.Confidence = 0.85; // Camera has good confidence
            result.Timestamp = DateTime.Now;

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Detects if a specific point is within camera's field of view
        /// </summary>
        public bool IsPointInView(Point robotPosition, double robotAngle, Point targetPoint)
        {
            double radAngle = (robotAngle + MountAngle) * Math.PI / 180.0;
            double sensorX = robotPosition.X + Position.X * Math.Cos(radAngle) - Position.Y * Math.Sin(radAngle);
            double sensorY = robotPosition.Y + Position.X * Math.Sin(radAngle) + Position.Y * Math.Cos(radAngle);

            double dx = targetPoint.X - sensorX;
            double dy = targetPoint.Y - sensorY;
            double targetAngle = Math.Atan2(dy, dx) * 180 / Math.PI;

            double angleDiff = Math.Abs(targetAngle - (robotAngle + MountAngle));
            angleDiff = Math.Min(angleDiff, 360 - angleDiff);

            return angleDiff <= FieldOfView / 2;
        }

        public override string GetReadingAsString()
        {
            return $"Camera: {ResolutionWidth}x{ResolutionHeight} @ {FrameRate}FPS, FOV={FieldOfView}°";
        }

        public override ISensor Clone()
        {
            return new CameraSensor
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
                ResolutionWidth = this.ResolutionWidth,
                ResolutionHeight = this.ResolutionHeight,
                FrameRate = this.FrameRate,
                EnableObjectDetection = this.EnableObjectDetection,
                EnableColorDetection = this.EnableColorDetection
            };
        }
        #endregion
    }
}
#region File Header
/// <summary>
/// File: RobotDefinition.cs
/// Description: Complete robot definition with appearance, kinematics, and sensors
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-29
/// </summary>
#endregion

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SallamPathFinder4.Core.Models.Robot
{
    #region Enums
    public enum RobotType
    {
        Wheeled,
        Tracked,
        Flying,
        Humanoid,
        Omnidirectional,
        Custom
    }

    public enum RobotShapeType
    {
        Rectangle,
        Square,
        Circle,
        RoundedRect,
        Triangle,
        Hexagon,
        Custom
    }

    public enum SensorType
    {
        Ultrasonic,
        Infrared,
        Lidar,
        Camera,
        Proximity,
        Temperature,
        Pressure,
        Humidity,
        GPS,
        IMU
    }
    #endregion

    #region RobotAppearance
    public class RobotAppearance
    {
        public RobotShapeType ShapeType { get; set; } = RobotShapeType.RoundedRect;
        public double Width { get; set; } = 50;
        public double Height { get; set; } = 30;
        public double Length { get; set; } = 60;
        public string Color { get; set; } = "#3498db";
        public string SecondaryColor { get; set; } = "#2c3e50";
        public string Icon { get; set; } = "🤖";
        public bool ShowWheels { get; set; } = true;
        public bool ShowDirectionArrow { get; set; } = true;
        public bool ShowSensorPoints { get; set; } = true;
        public double Opacity { get; set; } = 1.0;
        public string TexturePath { get; set; } = string.Empty;
    }
    #endregion

    #region RobotKinematics
    public class RobotKinematics
    {
        public double MaxForwardSpeed { get; set; } = 1.5;
        public double MaxReverseSpeed { get; set; } = 0.8;
        public double MaxLateralSpeed { get; set; } = 0.5;
        public double MaxTurnRate { get; set; } = 90;
        public double MinTurnRadius { get; set; } = 30;
        public double TurnAcceleration { get; set; } = 45;
        public double LinearAcceleration { get; set; } = 0.5;
        public double LinearDeceleration { get; set; } = 1.0;
        public double MaxSlopeAngle { get; set; } = 30;
        public double MaxStepHeight { get; set; } = 5;
        public double MaxGapWidth { get; set; } = 10;
        public double Wheelbase { get; set; } = 40;
        public double TrackWidth { get; set; } = 35;

        public double GetManeuverabilityScore()
        {
            double score = 0;
            score += Math.Min(MaxTurnRate / 90, 1) * 25;
            score += Math.Min(MaxForwardSpeed / 2, 1) * 20;
            score += (MaxLateralSpeed > 0 ? 15 : 0);
            score += Math.Min(1 / (MinTurnRadius / 30), 1) * 20;
            score += Math.Min(LinearAcceleration / 1, 1) * 10;
            score += Math.Min(MaxSlopeAngle / 45, 1) * 10;
            return Math.Min(score, 100);
        }
    }
    #endregion

    #region SimpleSensor
    public class SimpleSensor 
    {
        public string SensorId { get; set; } = Guid.NewGuid().ToString();
        public string SensorName { get; set; } = "Sensor";
        public string SensorType { get; set; } = "Ultrasonic";
        public int PositionX { get; set; } = 0;
        public int PositionY { get; set; } = 0;
        public double MountAngle { get; set; } = 0;
        public double FieldOfView { get; set; } = 30;
        public double MaxRange { get; set; } = 100;
        public string DisplayColor { get; set; } = "#2ecc71";
        public bool IsEnabled { get; set; } = true;

        [JsonIgnore]
        public Point Position
        {
            get => new Point(PositionX, PositionY);
            set { PositionX = value.X; PositionY = value.Y; }
        }

        [JsonIgnore]
        public string Type => SensorType;

        public SimpleSensor Clone()
        {
            return new SimpleSensor
            {
                SensorId = this.SensorId,
                SensorName = this.SensorName,
                SensorType = this.SensorType,
                PositionX = this.PositionX,
                PositionY = this.PositionY,
                MountAngle = this.MountAngle,
                FieldOfView = this.FieldOfView,
                MaxRange = this.MaxRange,
                DisplayColor = this.DisplayColor,
                IsEnabled = this.IsEnabled
            };
        }
    }
    #endregion

    #region RobotDefinition
    public class RobotDefinition
    {
        public string RobotId { get; set; } = Guid.NewGuid().ToString();
        public string RobotName { get; set; } = "New Robot";
        public RobotType RobotType { get; set; } = RobotType.Wheeled;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0";

        public RobotAppearance Appearance { get; set; } = new RobotAppearance();
        public RobotKinematics Kinematics { get; set; } = new RobotKinematics();
        public List<SimpleSensor> Sensors { get; set; } = new List<SimpleSensor>();

        public RobotDefinition Clone()
        {
            var clone = new RobotDefinition
            {
                RobotId = this.RobotId,
                RobotName = this.RobotName,
                RobotType = this.RobotType,
                CreatedAt = this.CreatedAt,
                ModifiedAt = DateTime.Now,
                Description = this.Description,
                Version = this.Version,
                Appearance = new RobotAppearance
                {
                    ShapeType = this.Appearance.ShapeType,
                    Width = this.Appearance.Width,
                    Height = this.Appearance.Height,
                    Length = this.Appearance.Length,
                    Color = this.Appearance.Color,
                    SecondaryColor = this.Appearance.SecondaryColor,
                    Icon = this.Appearance.Icon,
                    ShowWheels = this.Appearance.ShowWheels,
                    ShowDirectionArrow = this.Appearance.ShowDirectionArrow,
                    ShowSensorPoints = this.Appearance.ShowSensorPoints,
                    Opacity = this.Appearance.Opacity,
                    TexturePath = this.Appearance.TexturePath
                },
                Kinematics = new RobotKinematics
                {
                    MaxForwardSpeed = this.Kinematics.MaxForwardSpeed,
                    MaxReverseSpeed = this.Kinematics.MaxReverseSpeed,
                    MaxLateralSpeed = this.Kinematics.MaxLateralSpeed,
                    MaxTurnRate = this.Kinematics.MaxTurnRate,
                    MinTurnRadius = this.Kinematics.MinTurnRadius,
                    TurnAcceleration = this.Kinematics.TurnAcceleration,
                    LinearAcceleration = this.Kinematics.LinearAcceleration,
                    LinearDeceleration = this.Kinematics.LinearDeceleration,
                    MaxSlopeAngle = this.Kinematics.MaxSlopeAngle,
                    MaxStepHeight = this.Kinematics.MaxStepHeight,
                    MaxGapWidth = this.Kinematics.MaxGapWidth,
                    Wheelbase = this.Kinematics.Wheelbase,
                    TrackWidth = this.Kinematics.TrackWidth
                }
            };

            foreach (var sensor in this.Sensors)
            {
                clone.Sensors.Add(sensor.Clone());
            }

            return clone;
        }

        public void SaveToFile(string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(filePath, json);
        }

        public static RobotDefinition LoadFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<RobotDefinition>(json);
        }
        public void Draw(Graphics g, Point position, double angle, float scale = 1.0f)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TranslateTransform(position.X, position.Y);
            g.RotateTransform((float)angle);
            g.ScaleTransform(scale, scale);

            var appearance = this.Appearance;
            int width = (int)appearance.Width;
            int height = (int)appearance.Height;
            var rect = new Rectangle(-width / 2, -height / 2, width, height);

            Color bodyColor;
            try { bodyColor = ColorTranslator.FromHtml(appearance.Color); }
            catch { bodyColor = Color.FromArgb(52, 152, 219); }

            using (var bodyBrush = new SolidBrush(bodyColor))
            using (var borderPen = new Pen(Color.FromArgb(44, 62, 80), 2))
            {
                // Draw based on shape type
                switch (appearance.ShapeType)
                {
                    case RobotShapeType.Circle:
                        g.FillEllipse(bodyBrush, rect);
                        g.DrawEllipse(borderPen, rect);
                        break;
                    case RobotShapeType.RoundedRect:
                        int radius = Math.Min(width, height) / 4;
                        using (var path = GetRoundedRectanglePath(rect, radius))
                        {
                            g.FillPath(bodyBrush, path);
                            g.DrawPath(borderPen, path);
                        }
                        break;
                    default:
                        g.FillRectangle(bodyBrush, rect);
                        g.DrawRectangle(borderPen, rect);
                        break;
                }
            }

            // Draw wheels
            if (appearance.ShowWheels && (this.RobotType == RobotType.Wheeled || this.RobotType == RobotType.Omnidirectional))
            {
                int wheelWidth = Math.Max(4, width / 8);
                int wheelHeight = Math.Max(8, height / 4);
                int wheelOffset = width / 3;
                using (var wheelBrush = new SolidBrush(Color.FromArgb(44, 62, 80)))
                {
                    g.FillEllipse(wheelBrush, -wheelOffset - wheelWidth / 2, height / 2 - wheelHeight / 2, wheelWidth, wheelHeight);
                    g.FillEllipse(wheelBrush, wheelOffset - wheelWidth / 2, height / 2 - wheelHeight / 2, wheelWidth, wheelHeight);
                }
            }

            // Draw direction arrow
            if (appearance.ShowDirectionArrow)
            {
                using (var arrowBrush = new SolidBrush(Color.FromArgb(46, 204, 113)))
                {
                    var arrowPoints = new Point[]
                    {
                new Point(0, -height / 2 - 5),
                new Point(-6, -height / 2 + 8),
                new Point(6, -height / 2 + 8)
                    };
                    g.FillPolygon(arrowBrush, arrowPoints);
                }
            }

            g.ResetTransform();
        }
        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.X + rect.Width - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.X + rect.Width - radius * 2, rect.Y + rect.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
        #region Power System

        /// <summary>
        /// Power system configuration for the robot
        /// </summary>
        public class RobotPowerSystem
        {
            /// <summary>
            /// Battery capacity in Watt-hours (Wh)
            /// </summary>
            public double BatteryCapacityWh { get; set; } = 100.0;

            /// <summary>
            /// Base power consumption in Watts (when idling)
            /// </summary>
            public double BasePowerConsumptionW { get; set; } = 20.0;

            /// <summary>
            /// Weight of the robot in kilograms
            /// </summary>
            public double WeightKg { get; set; } = 10.0;

            /// <summary>
            /// Motor efficiency (0-1)
            /// </summary>
            public double MotorEfficiency { get; set; } = 0.85;

            /// <summary>
            /// Rolling resistance coefficient
            /// </summary>
            public double RollingResistance { get; set; } = 0.015;

            /// <summary>
            /// Aerodynamic drag coefficient (Cd × A)
            /// </summary>
            public double DragCoefficient { get; set; } = 0.5;

            /// <summary>
            /// Regenerative braking efficiency (0-1)
            /// </summary>
            public double RegenerativeBrakingEfficiency { get; set; } = 0.2;

            /// <summary>
            /// Gets power factor based on robot type
            /// </summary>
            public double GetRobotTypePowerFactor(RobotType robotType)
            {
                return robotType switch
                {
                    RobotType.Flying => 2.5,      // Very high - needs to overcome gravity
                    RobotType.Tracked => 1.5,     // High - more friction
                    RobotType.Wheeled => 1.0,     // Baseline
                    RobotType.Omnidirectional => 1.2,  // Slightly higher - 4 motors
                    RobotType.Humanoid => 2.0,    // High - balancing takes energy
                    RobotType.Custom => 1.0,
                    _ => 1.0
                };
            }

            /// <summary>
            /// Calculates power consumption at given speed (Watts)
            /// </summary>
            public double CalculatePowerAtSpeed(double speedMs, RobotType robotType, double slopeAngle = 0)
            {
                double typeFactor = GetRobotTypePowerFactor(robotType);
                double speedKmh = speedMs * 3.6;

                // Rolling resistance power (Watts)
                double rollingPower = RollingResistance * WeightKg * 9.8 * speedMs;

                // Aerodynamic drag power (Watts) - ½ × ρ × Cd×A × v³
                double airDensity = 1.225;  // kg/m³ at sea level
                double dragPower = 0.5 * airDensity * DragCoefficient * speedMs * speedMs * speedMs;

                // Gravity power (if climbing slope)
                double gravityPower = WeightKg * 9.8 * Math.Sin(slopeAngle * Math.PI / 180.0) * speedMs;

                // Total mechanical power
                double mechanicalPower = rollingPower + dragPower + gravityPower;

                // Electrical power (accounting for motor efficiency)
                double electricalPower = mechanicalPower / MotorEfficiency;

                // Add base consumption and type factor
                double totalPower = (BasePowerConsumptionW + electricalPower) * typeFactor;

                return Math.Max(0, totalPower);
            }

            /// <summary>
            /// Calculates energy consumed per meter (Joules/meter)
            /// </summary>
            public double CalculateEnergyPerMeter(double speedMs, RobotType robotType, double slopeAngle = 0)
            {
                double powerWatts = CalculatePowerAtSpeed(speedMs, robotType, slopeAngle);
                if (speedMs <= 0) return 0;
                return powerWatts / speedMs;  // J/m
            }

            /// <summary>
            /// Calculates battery percentage consumed per meter
            /// </summary>
            public double CalculateBatteryConsumptionPerMeter(double speedMs, RobotType robotType)
            {
                double batteryCapacityJoules = BatteryCapacityWh * 3600;  // Convert Wh to Joules
                double energyPerMeter = CalculateEnergyPerMeter(speedMs, robotType);
                return (energyPerMeter / batteryCapacityJoules) * 100;
            }

            /// <summary>
            /// Calculates remaining range in meters
            /// </summary>
            public double CalculateRemainingRange(double currentBatteryPercent, double speedMs, RobotType robotType)
            {
                double consumptionPerMeter = CalculateBatteryConsumptionPerMeter(speedMs, robotType);
                if (consumptionPerMeter <= 0) return 0;
                return currentBatteryPercent / consumptionPerMeter;
            }

            /// <summary>
            /// Formats power status for display
            /// </summary>
            public string FormatPowerStatus(double currentSpeedMs, RobotType robotType, double currentBatteryPercent)
            {
                double currentPower = CalculatePowerAtSpeed(currentSpeedMs, robotType);
                double range = CalculateRemainingRange(currentBatteryPercent, currentSpeedMs, robotType);

                return $"⚡ {currentPower:F0}W | 🔋 {currentBatteryPercent:F1}% | 📏 {range:F0}m range";
            }
        }

        /// <summary>
        /// Gets or sets the power system configuration
        /// </summary>
        public RobotPowerSystem PowerSystem { get; set; } = new RobotPowerSystem();

        #endregion
    }
    #endregion
}
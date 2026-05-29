#region File Header 
/// <summary>
/// File: EnvironmentalReading.cs
/// Description: Environmental data structure for temperature, pressure, humidity sensors
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion
using SallamPathFinder4.Core.Models.Map; 
using System.Drawing;
namespace SallamPathFinder4.Core.Models.Sensors
{
    /// <summary>
    /// Environmental sensor reading
    /// </summary>
    public class EnvironmentalReading
    {
        public double Temperature { get; set; }      // Celsius
        public double Pressure { get; set; }         // hPa
        public double Humidity { get; set; }         // Percentage (0-100)
        public double WindSpeed { get; set; }        // m/s
        public double WindDirection { get; set; }    // Degrees (0-360)
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"Temp: {Temperature:F1}°C, Humidity: {Humidity:F0}%, Wind: {WindSpeed:F1}m/s @ {WindDirection:F0}°";
        }
    }

    /// <summary>
    /// Temperature sensor (thermometer)
    /// </summary>
    public class TemperatureSensor : SensorBase
    {
        public override SensorType Type => SensorType.Temperature;
        public override Color DisplayColor => Color.FromArgb(241, 196, 15);

        public double CurrentTemperature { get; private set; }
        public double Accuracy { get; set; } = 0.5;

        public TemperatureSensor()
        {
            SensorName = "Temperature Sensor";
            PowerConsumption = 0.02;
            UpdateRate = 1;
        }

        public override async Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle)
        {
            // Simulate temperature reading (would read from map data in real implementation)
            var random = new Random();
            CurrentTemperature = 22.0 + (random.NextDouble() - 0.5) * 5;

            return new DetectionResult
            {
                ObstacleDetected = false,
                Confidence = 0.98,
                Timestamp = DateTime.Now
            };
        }

        public override string GetReadingAsString() => $"Temperature: {CurrentTemperature:F1}°C (±{Accuracy:F1})";

        public override ISensor Clone()
        {
            return new TemperatureSensor
            {
                SensorId = SensorId,
                SensorName = SensorName,
                Position = Position,
                MountAngle = MountAngle,
                IsEnabled = IsEnabled,
                Accuracy = Accuracy
            };
        }
    }

    /// <summary>
    /// Pressure sensor (barometer)
    /// </summary>
    public class PressureSensor : SensorBase
    {
        public override SensorType Type => SensorType.Pressure;
        public override Color DisplayColor => Color.FromArgb(52, 152, 219);

        public double CurrentPressure { get; private set; }
        public double Accuracy { get; set; } = 1.0;

        public PressureSensor()
        {
            SensorName = "Pressure Sensor";
            PowerConsumption = 0.02;
            UpdateRate = 1;
        }

        public override async Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle)
        {
            var random = new Random();
            CurrentPressure = 1013.25 + (random.NextDouble() - 0.5) * 20;

            return new DetectionResult
            {
                ObstacleDetected = false,
                Confidence = 0.99,
                Timestamp = DateTime.Now
            };
        }

        public override string GetReadingAsString() => $"Pressure: {CurrentPressure:F1} hPa (±{Accuracy:F1})";

        public override ISensor Clone()
        {
            return new PressureSensor
            {
                SensorId = SensorId,
                SensorName = SensorName,
                Position = Position,
                MountAngle = MountAngle,
                IsEnabled = IsEnabled,
                Accuracy = Accuracy
            };
        }
    }

    /// <summary>
    /// Humidity sensor
    /// </summary>
    public class HumiditySensor : SensorBase
    {
        public override SensorType Type => SensorType.Humidity;
        public override Color DisplayColor => Color.FromArgb(46, 204, 113);

        public double CurrentHumidity { get; private set; }
        public double Accuracy { get; set; } = 3.0;

        public HumiditySensor()
        {
            SensorName = "Humidity Sensor";
            PowerConsumption = 0.02;
            UpdateRate = 1;
        }

        public override async Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle)
        {
            var random = new Random();
            CurrentHumidity = 50.0 + (random.NextDouble() - 0.5) * 40;

            return new DetectionResult
            {
                ObstacleDetected = false,
                Confidence = 0.97,
                Timestamp = DateTime.Now
            };
        }

        public override string GetReadingAsString() => $"Humidity: {CurrentHumidity:F0}% (±{Accuracy:F0})";

        public override ISensor Clone()
        {
            return new HumiditySensor
            {
                SensorId = SensorId,
                SensorName = SensorName,
                Position = Position,
                MountAngle = MountAngle,
                IsEnabled = IsEnabled,
                Accuracy = Accuracy
            };
        }
    }

    /// <summary>
    /// Wind sensor (anemometer)
    /// </summary>
    public class WindSensor : SensorBase
    {
        public override SensorType Type => SensorType.Wind;
        public override Color DisplayColor => Color.FromArgb(155, 89, 182);

        public double CurrentWindSpeed { get; private set; }
        public double CurrentWindDirection { get; private set; }
        public double Accuracy { get; set; } = 0.5;

        public WindSensor()
        {
            SensorName = "Wind Sensor";
            PowerConsumption = 0.05;
            UpdateRate = 2;
        }

        public override async Task<DetectionResult> DetectAsync(MapGrid grid, Point robotPosition, double robotAngle)
        {
            var random = new Random();
            CurrentWindSpeed = 5.0 + random.NextDouble() * 15;
            CurrentWindDirection = random.NextDouble() * 360;

            return new DetectionResult
            {
                ObstacleDetected = false,
                Confidence = 0.9,
                Timestamp = DateTime.Now
            };
        }

        public override string GetReadingAsString() => $"Wind: {CurrentWindSpeed:F1} m/s @ {CurrentWindDirection:F0}°";

        public override ISensor Clone()
        {
            return new WindSensor
            {
                SensorId = SensorId,
                SensorName = SensorName,
                Position = Position,
                MountAngle = MountAngle,
                IsEnabled = IsEnabled,
                Accuracy = Accuracy
            };
        }
    }
}
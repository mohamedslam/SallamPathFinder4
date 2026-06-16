#region File Header
/// <summary>
/// File: ObstacleLogEntry.cs
/// Description: Represents a single obstacle detection log entry for export and analysis
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-06-01
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.Text;
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.Core.Models.Obstacles
{
    /// <summary>
    /// Represents a single obstacle detection log entry
    /// Used for exporting to CSV/JSON and for analysis
    /// </summary>
    public sealed class ObstacleLogEntry
    {
        #region Constructor
        public ObstacleLogEntry()
        {
            EntryId = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
        }

        public ObstacleLogEntry(DetectedObstacle obstacle, string actionTaken, double robotSpeed, Point robotPosition) : this()
        {
            Location = obstacle.Location;
            ObstacleType = obstacle.ObstacleType;
            Priority = ObstaclePriorityHelper.GetPriorityValue(obstacle.ObstacleType);
            DistanceCm = obstacle.DistanceCm;
            Angle = obstacle.Angle;
            SensorType = obstacle.SensorType;
            ActionTaken = actionTaken;
            RobotSpeedAtDetection = robotSpeed;
            RobotPositionAtDetection = robotPosition;
            Confidence = obstacle.Confidence;
            IsMoving = obstacle.IsMoving;
            MovementSpeedCmS = obstacle.MovementSpeed;
            MovementDirection = obstacle.MovementDirection;
            PersistenceCount = obstacle.PersistenceCount;
            CapturedImagePath = obstacle.CapturedImagePath;
        }
        #endregion

        #region Properties - Identification
        /// <summary>
        /// Unique identifier for this log entry
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// Timestamp of the detection (UTC)
        /// </summary>
        public DateTime Timestamp { get; set; }
        #endregion

        #region Properties - Obstacle Information
        /// <summary>
        /// Grid location of the detected obstacle
        /// </summary>
        public Point Location { get; set; }

        /// <summary>
        /// Type of obstacle detected
        /// </summary>
        public ObstacleType ObstacleType { get; set; }

        /// <summary>
        /// Priority value (0-100)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Distance from robot to obstacle in centimeters
        /// </summary>
        public double DistanceCm { get; set; }

        /// <summary>
        /// Angle of detection relative to robot forward (degrees)
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Type of sensor that detected the obstacle
        /// </summary>
        public string SensorType { get; set; }

        /// <summary>
        /// Confidence level of detection (0-100%)
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Whether the obstacle is moving
        /// </summary>
        public bool IsMoving { get; set; }

        /// <summary>
        /// Movement speed in cm/s (if moving)
        /// </summary>
        public double MovementSpeedCmS { get; set; }

        /// <summary>
        /// Movement direction in degrees (if moving)
        /// </summary>
        public double MovementDirection { get; set; }

        /// <summary>
        /// Number of consecutive detections
        /// </summary>
        public int PersistenceCount { get; set; }
        #endregion

        #region Properties - Robot Context
        /// <summary>
        /// Action taken by the robot in response to this detection
        /// </summary>
        public string ActionTaken { get; set; }

        /// <summary>
        /// Robot speed at time of detection (cm/s)
        /// </summary>
        public double RobotSpeedAtDetection { get; set; }

        /// <summary>
        /// Robot position at time of detection
        /// </summary>
        public Point RobotPositionAtDetection { get; set; }

        /// <summary>
        /// Path to captured image (if camera was used)
        /// </summary>
        public string CapturedImagePath { get; set; }
        #endregion

        #region Properties - Learning & Analysis
        /// <summary>
        /// Whether this obstacle was recorded for learning
        /// </summary>
        public bool RecordedForLearning { get; set; }

        /// <summary>
        /// Learning coefficient applied (for SPPA-DL)
        /// </summary>
        public double LearningCoefficient { get; set; }

        /// <summary>
        /// Whether this obstacle was successfully avoided
        /// </summary>
        public bool WasAvoided { get; set; }

        /// <summary>
        /// Time taken to resolve the obstacle (seconds)
        /// </summary>
        public double ResolutionTimeSeconds { get; set; }
        #endregion

        #region Public Methods - Export
        /// <summary>
        /// Converts log entry to CSV line
        /// </summary>
        public string ToCsvLine()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss.fff}," +
                   $"{Location.X},{Location.Y}," +
                   $"{ObstacleType}," +
                   $"{Priority}," +
                   $"{DistanceCm:F1}," +
                   $"{Angle:F1}," +
                   $"{SensorType}," +
                   $"{Confidence:F2}," +
                   $"{IsMoving}," +
                   $"{MovementSpeedCmS:F1}," +
                   $"{MovementDirection:F1}," +
                   $"{PersistenceCount}," +
                   $"{ActionTaken}," +
                   $"{RobotSpeedAtDetection:F1}," +
                   $"{RobotPositionAtDetection.X},{RobotPositionAtDetection.Y}," +
                   $"\"{CapturedImagePath ?? ""}\"," +
                   $"{RecordedForLearning}," +
                   $"{LearningCoefficient:F3}," +
                   $"{WasAvoided}," +
                   $"{ResolutionTimeSeconds:F2}";
        }

        /// <summary>
        /// Returns CSV header line
        /// </summary>
        public static string GetCsvHeader()
        {
            return "Timestamp,LocationX,LocationY,ObstacleType,Priority,DistanceCm,Angle,SensorType,Confidence," +
                   "IsMoving,MovementSpeedCmS,MovementDirection,PersistenceCount,ActionTaken," +
                   "RobotSpeedCmS,RobotPositionX,RobotPositionY,CapturedImagePath," +
                   "RecordedForLearning,LearningCoefficient,WasAvoided,ResolutionTimeSeconds";
        }

        /// <summary>
        /// Converts log entry to JSON string
        /// </summary>
        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        /// <summary>
        /// Converts log entry to human-readable string
        /// </summary>
        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {ObstacleType} at ({Location.X},{Location.Y}) - " +
                   $"{DistanceCm:F0}cm, {ActionTaken}, Confidence: {Confidence:P0}";
        }
        #endregion

        #region Public Methods - Formatting
        /// <summary>
        /// Gets HTML representation for dashboard
        /// </summary>
        public string ToHtml()
        {
            string color = GetPriorityColorHtml();
            string icon = GetPriorityIcon();

            return $@"
<div style='border-left: 4px solid {color}; padding: 8px; margin: 4px 0; background: #f5f5f5;'>
    <div style='font-weight: bold;'>
        {icon} {ObstacleType} at ({Location.X}, {Location.Y})
    </div>
    <div style='font-size: 12px; color: #666;'>
        Distance: {DistanceCm:F0}cm | Action: {ActionTaken} | Confidence: {Confidence:P0}
    </div>
    <div style='font-size: 11px; color: #999;'>
        {Timestamp:yyyy-MM-dd HH:mm:ss} | Sensor: {SensorType}
    </div>
</div>";
        }

        private string GetPriorityColorHtml()
        {
            if (Priority >= 90) return "#e74c3c";
            if (Priority >= 70) return "#e67e22";
            if (Priority >= 50) return "#f1c40f";
            if (Priority >= 30) return "#3498db";
            return "#95a5a6";
        }

        private string GetPriorityIcon()
        {
            if (Priority >= 90) return "⚠️⚠️";
            if (Priority >= 70) return "⚠️";
            if (Priority >= 50) return "❗";
            if (Priority >= 30) return "📌";
            return "●";
        }
        #endregion
    }

    #region Obstacle Log Collection
    /// <summary>
    /// Collection of obstacle log entries with export capabilities
    /// </summary>
    public sealed class ObstacleLogCollection
    {
        private readonly List<ObstacleLogEntry> _entries;
        private readonly object _lockObject = new object();

        public ObstacleLogCollection()
        {
            _entries = new List<ObstacleLogEntry>();
        }

        public IReadOnlyList<ObstacleLogEntry> Entries
        {
            get
            {
                lock (_lockObject)
                {
                    return _entries.ToList().AsReadOnly();
                }
            }
        }

        public int Count
        {
            get
            {
                lock (_lockObject)
                {
                    return _entries.Count;
                }
            }
        }

        /// <summary>
        /// Adds a log entry
        /// </summary>
        public void Add(ObstacleLogEntry entry)
        {
            lock (_lockObject)
            {
                _entries.Add(entry);

                // Keep only last 10,000 entries
                while (_entries.Count > 10000)
                {
                    _entries.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Adds multiple log entries
        /// </summary>
        public void AddRange(IEnumerable<ObstacleLogEntry> entries)
        {
            foreach (var entry in entries)
            {
                Add(entry);
            }
        }

        /// <summary>
        /// Clears all log entries
        /// </summary>
        public void Clear()
        {
            lock (_lockObject)
            {
                _entries.Clear();
            }
        }

        /// <summary>
        /// Gets entries filtered by obstacle type
        /// </summary>
        public List<ObstacleLogEntry> GetByType(ObstacleType type)
        {
            lock (_lockObject)
            {
                return _entries.Where(e => e.ObstacleType == type).ToList();
            }
        }

        /// <summary>
        /// Gets entries filtered by time range
        /// </summary>
        public List<ObstacleLogEntry> GetByTimeRange(DateTime start, DateTime end)
        {
            lock (_lockObject)
            {
                return _entries.Where(e => e.Timestamp >= start && e.Timestamp <= end).ToList();
            }
        }

        /// <summary>
        /// Gets entries within a radius of a point
        /// </summary>
        public List<ObstacleLogEntry> GetNearLocation(Point center, int radiusCells)
        {
            lock (_lockObject)
            {
                return _entries.Where(e =>
                    Math.Abs(e.Location.X - center.X) <= radiusCells &&
                    Math.Abs(e.Location.Y - center.Y) <= radiusCells).ToList();
            }
        }

        /// <summary>
        /// Exports all entries to CSV
        /// </summary>
        public string ExportToCsv()
        {
            lock (_lockObject)
            {
                var sb = new StringBuilder();
                sb.AppendLine(ObstacleLogEntry.GetCsvHeader());

                foreach (var entry in _entries)
                {
                    sb.AppendLine(entry.ToCsvLine());
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Exports entries to CSV file
        /// </summary>
        public async Task ExportToCsvFileAsync(string filePath)
        {
            string csv;
            lock (_lockObject)
            {
                csv = ExportToCsv();
            }
            await File.WriteAllTextAsync(filePath, csv, Encoding.UTF8);
        }

        /// <summary>
        /// Exports entries to JSON file
        /// </summary>
        public async Task ExportToJsonFileAsync(string filePath)
        {
            string json;
            lock (_lockObject)
            {
                json = System.Text.Json.JsonSerializer.Serialize(_entries, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
        }

        /// <summary>
        /// Gets statistics about logged obstacles
        /// </summary>
        public ObstacleLogStatistics GetStatistics()
        {
            lock (_lockObject)
            {
                if (_entries.Count == 0)
                    return new ObstacleLogStatistics();

                return new ObstacleLogStatistics
                {
                    TotalEntries = _entries.Count,
                    UniqueLocations = _entries.Select(e => e.Location).Distinct().Count(),
                    ObstacleTypeDistribution = _entries.GroupBy(e => e.ObstacleType)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    AverageConfidence = _entries.Average(e => e.Confidence),
                    MostCommonAction = _entries.GroupBy(e => e.ActionTaken)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault()?.Key ?? "None",
                    AverageResolutionTime = _entries.Average(e => e.ResolutionTimeSeconds),
                    SuccessRate = _entries.Any() ? _entries.Count(e => e.WasAvoided) / (double)_entries.Count * 100 : 0
                };
            }
        }
    }

    /// <summary>
    /// Statistics for obstacle log collection
    /// </summary>
    public sealed class ObstacleLogStatistics
    {
        public int TotalEntries { get; set; }
        public int UniqueLocations { get; set; }
        public Dictionary<ObstacleType, int> ObstacleTypeDistribution { get; set; }
        public double AverageConfidence { get; set; }
        public string MostCommonAction { get; set; }
        public double AverageResolutionTime { get; set; }
        public double SuccessRate { get; set; }
    }
    #endregion
}
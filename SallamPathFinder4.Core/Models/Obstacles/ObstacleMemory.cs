#region File Header
/// <summary>
/// File: LearningMemory.cs
/// Description: Persistent memory for obstacle detections across multiple simulations
/// Enables long-term learning for SPPA-DL algorithm
/// Records obstacle locations, types, frequencies, and patterns
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-06-01
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
#endregion

namespace SallamPathFinder4.Core.Models.Obstacles
{
    #region Learning Record Class
    /// <summary>
    /// Represents a learning record for a specific cell
    /// Stores frequency, patterns, and obstacle type distribution
    /// </summary>
    public sealed class LearningRecord
    {
        #region Constructor
        public LearningRecord()
        {
            TypeCounts = new Dictionary<ObstacleType, int>();
            HourlyDistribution = new Dictionary<int, int>();
            LastSeen = DateTime.UtcNow;
            FirstSeen = DateTime.UtcNow;
        }

        public LearningRecord(int x, int y, ObstacleType type) : this()
        {
            X = x;
            Y = y;
            Frequency = 1;
            LastSeen = DateTime.UtcNow;
            Type = type;
            TypeCounts[type] = 1;
            UpdateHourlyDistribution();
        }
        #endregion

        #region Properties
        /// <summary>X coordinate of the cell</summary>
        public int X { get; set; }

        /// <summary>Y coordinate of the cell</summary>
        public int Y { get; set; }

        /// <summary>Number of times obstacle was detected at this cell</summary>
        public int Frequency { get; set; }

        /// <summary>Last time an obstacle was detected at this cell (UTC)</summary>
        public DateTime LastSeen { get; set; }

        /// <summary>First time an obstacle was detected at this cell (UTC)</summary>
        public DateTime FirstSeen { get; set; }

        /// <summary>Dominant obstacle type at this cell</summary>
        public ObstacleType Type { get; set; }

        /// <summary>Distribution of obstacle types by count</summary>
        public Dictionary<ObstacleType, int> TypeCounts { get; set; }

        /// <summary>Distribution of detections by hour of day (0-23)</summary>
        public Dictionary<int, int> HourlyDistribution { get; set; }

        /// <summary>Average time between detections (seconds)</summary>
        public double AverageIntervalSeconds { get; set; }

        /// <summary>Whether this location is considered a hotspot</summary>
        public bool IsHotspot => Frequency > 10 && (DateTime.UtcNow - LastSeen).TotalDays < 7;

        /// <summary>Risk level (0-100) based on frequency and recency</summary>
        public double RiskLevel => CalculateRiskLevel();
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the record with a new obstacle detection
        /// </summary>
        public void Update(ObstacleType type)
        {
            double timeSinceLastSeen = (DateTime.UtcNow - LastSeen).TotalSeconds;

            // Update average interval
            if (Frequency > 0)
            {
                AverageIntervalSeconds = (AverageIntervalSeconds * Frequency + timeSinceLastSeen) / (Frequency + 1);
            }

            Frequency++;
            LastSeen = DateTime.UtcNow;

            if (TypeCounts.ContainsKey(type))
                TypeCounts[type]++;
            else
                TypeCounts[type] = 1;

            // Update dominant type (most frequent)
            int maxCount = 0;
            foreach (var kvp in TypeCounts)
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    Type = kvp.Key;
                }
            }

            UpdateHourlyDistribution();
        }

        /// <summary>
        /// Updates hourly distribution based on current time
        /// </summary>
        public void UpdateHourlyDistribution()
        {
            int hour = DateTime.UtcNow.Hour;
            if (HourlyDistribution.ContainsKey(hour))
                HourlyDistribution[hour]++;
            else
                HourlyDistribution[hour] = 1;
        }

        /// <summary>
        /// Calculates risk level based on frequency, recency, and time patterns
        /// </summary>
        private double CalculateRiskLevel()
        {
            double risk = 0;

            // Frequency factor (0-50)
            risk += Math.Min(50, Frequency * 2);

            // Recency factor (0-30)
            double daysSinceLastSeen = (DateTime.UtcNow - LastSeen).TotalDays;
            risk += Math.Max(0, 30 - daysSinceLastSeen * 5);

            // Time pattern factor (0-20)
            int currentHour = DateTime.UtcNow.Hour;
            if (HourlyDistribution.ContainsKey(currentHour))
            {
                double hourFrequency = HourlyDistribution[currentHour] / (double)Frequency;
                risk += hourFrequency * 20;
            }

            return Math.Min(100, risk);
        }

        /// <summary>
        /// Gets the obstacle coefficient for learning formula
        /// Formula: α * (Frequency / TotalSimulations) * RiskLevel/100
        /// </summary>
        public double GetObstacleCoefficient(int totalSimulations, double learningRate)
        {
            if (totalSimulations <= 0) return 0;

            double baseCoefficient = learningRate * ((double)Frequency / totalSimulations);
            double riskAdjusted = baseCoefficient * (RiskLevel / 100.0);

            return Math.Min(learningRate, riskAdjusted);
        }

        /// <summary>
        /// Predicts probability of obstacle occurrence at given time
        /// </summary>
        public double GetProbabilityAtTime(DateTime time)
        {
            int hour = time.Hour;
            if (!HourlyDistribution.ContainsKey(hour))
                return 0;

            double hourProbability = HourlyDistribution[hour] / (double)Frequency;
            double recencyFactor = Math.Exp(-(time - LastSeen).TotalHours / 24);

            return hourProbability * recencyFactor;
        }
        #endregion
    }
    #endregion

    #region Learning Memory Class
    /// <summary>
    /// Manages persistent learning memory for obstacle detection across simulations
    /// Supports pattern recognition, hotspot detection, and risk assessment
    /// </summary>
    public sealed class LearningMemory : IDisposable
    {
        #region Constants
        private const string DEFAULT_FOLDER_NAME = "SallamPathFinder4";
        private const string DEFAULT_FILE_NAME = "LearningMemory.json";
        private const int MAX_RECORDS = 10000;
        private const int HOTSPOT_RADIUS = 3;
        #endregion

        #region Private Fields
        private ConcurrentDictionary<string, LearningRecord> _memory;
        private int _totalDetections;
        private int _totalSimulations;
        private readonly string _filePath;
        private readonly object _lockObject;
        private bool _isDisposed;
        private bool _learningEnabled = true;
        #endregion

        #region Constructor
        public LearningMemory(string fileName = DEFAULT_FILE_NAME)
        {
            _memory = new ConcurrentDictionary<string, LearningRecord>();
            _totalDetections = 0;
            _totalSimulations = 0;
            _lockObject = new object();
            _isDisposed = false;

            // Use AppData folder for persistent storage
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder =System.IO.Path.Combine(appDataPath, DEFAULT_FOLDER_NAME);

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            _filePath = System.IO.Path.Combine(appFolder, fileName);
        }
        #endregion

        #region Properties
        public bool LearningEnabled
        {
            get => _learningEnabled;
            set => _learningEnabled = value;
        }

        public int TotalDetections => _totalDetections;
        public int TotalSimulations => _totalSimulations;
        public int RecordCount => _memory.Count;
        public int HotspotCount => _memory.Values.Count(r => r.IsHotspot);
        #endregion

        #region Events
        public event Action<LearningRecord> NewHotspotDetected;
        public event Action<LearningRecord> RiskThresholdExceeded;
        #endregion

        #region Private Methods
        private static string GetKey(int x, int y) => $"{x},{y}";

        private string GetKeyForRadius(int x, int y, int radius)
        {
            // Round to hotspot grid cell
            int gridX = (int)Math.Round(x / (double)radius) * radius;
            int gridY = (int)Math.Round(y / (double)radius) * radius;
            return $"{gridX},{gridY}";
        }
        #endregion

        #region Public Methods - Recording
        /// <summary>
        /// Records an obstacle detection at the specified cell
        /// </summary>
        public void RecordDetection(int x, int y, ObstacleType type)
        {
            if (!_learningEnabled) return;

            string key = GetKey(x, y);

            _memory.AddOrUpdate(key,
                new LearningRecord(x, y, type),
                (k, existing) => { existing.Update(type); return existing; });

            _totalDetections++;

            // Check for new hotspot
            if (_memory.TryGetValue(key, out var record) && record.IsHotspot && record.Frequency == 11)
            {
                NewHotspotDetected?.Invoke(record);
            }

            // Check for risk threshold (80% risk)
            if (record != null && record.RiskLevel > 80)
            {
                RiskThresholdExceeded?.Invoke(record);
            }
        }

        /// <summary>
        /// Records an obstacle detection with learning confidence
        /// </summary>
        public void RecordDetectionWithConfidence(int x, int y, ObstacleType type, double confidence)
        {
            if (!_learningEnabled) return;
            if (confidence < 0.5) return;  // Ignore low confidence detections

            RecordDetection(x, y, type);
        }

        /// <summary>
        /// Increments the total simulations counter
        /// </summary>
        public void IncrementSimulation()
        {
            lock (_lockObject)
            {
                _totalSimulations++;
            }
        }

        /// <summary>
        /// Clears all learning memory
        /// </summary>
        public void Clear()
        {
            _memory.Clear();
            _totalDetections = 0;
            _totalSimulations = 0;
        }
        #endregion

        #region Public Methods - Querying
        /// <summary>
        /// Gets the obstacle coefficient for a specific cell
        /// </summary>
        public double GetObstacleCoefficient(int x, int y, double learningRate)
        {
            if (!_learningEnabled) return 0;

            string key = GetKey(x, y);
            if (_memory.TryGetValue(key, out var record))
            {
                return record.GetObstacleCoefficient(_totalSimulations, learningRate);
            }
            return 0;
        }

        /// <summary>
        /// Gets the obstacle coefficient for a cell with radius consideration
        /// </summary>
        public double GetObstacleCoefficientWithRadius(int x, int y, double learningRate, int radius = 2)
        {
            if (!_learningEnabled) return 0;

            double maxCoefficient = 0;

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    double coeff = GetObstacleCoefficient(nx, ny, learningRate);
                    if (coeff > maxCoefficient) maxCoefficient = coeff;
                }
            }

            return maxCoefficient;
        }

        /// <summary>
        /// Gets the risk level for a specific cell
        /// </summary>
        public double GetRiskLevel(int x, int y)
        {
            string key = GetKey(x, y);
            return _memory.TryGetValue(key, out var record) ? record.RiskLevel : 0;
        }

        /// <summary>
        /// Gets the frequency of obstacle detections at a specific cell
        /// </summary>
        public int GetFrequency(int x, int y)
        {
            string key = GetKey(x, y);
            return _memory.TryGetValue(key, out var record) ? record.Frequency : 0;
        }

        /// <summary>
        /// Gets the dominant obstacle type at a specific cell
        /// </summary>
        public ObstacleType? GetDominantType(int x, int y)
        {
            string key = GetKey(x, y);
            return _memory.TryGetValue(key, out var record) ? record.Type : (ObstacleType?)null;
        }

        /// <summary>
        /// Checks if a cell has any learning records
        /// </summary>
        public bool HasMemory(int x, int y)
        {
            return _memory.ContainsKey(GetKey(x, y));
        }

        /// <summary>
        /// Gets the probability of obstacle occurrence at a cell at given time
        /// </summary>
        public double GetProbabilityAtTime(int x, int y, DateTime time)
        {
            string key = GetKey(x, y);
            return _memory.TryGetValue(key, out var record) ? record.GetProbabilityAtTime(time) : 0;
        }

        /// <summary>
        /// Gets all cells that have memory records
        /// </summary>
        public List<Point> GetMemoryCells()
        {
            return _memory.Values.Select(r => new Point(r.X, r.Y)).ToList();
        }

        /// <summary>
        /// Gets all memory records
        /// </summary>
        public List<LearningRecord> GetAllRecords()
        {
            return _memory.Values.ToList();
        }

        /// <summary>
        /// Gets hotspot locations (frequently visited cells)
        /// </summary>
        public List<LearningRecord> GetHotspots()
        {
            return _memory.Values.Where(r => r.IsHotspot).ToList();
        }

        /// <summary>
        /// Gets cells with risk level above threshold
        /// </summary>
        public List<LearningRecord> GetHighRiskCells(double riskThreshold = 70)
        {
            return _memory.Values.Where(r => r.RiskLevel >= riskThreshold).ToList();
        }
        #endregion

        #region Public Methods - Path Planning
        /// <summary>
        /// Calculates risk cost for path planning
        /// </summary>
        public double CalculatePathRiskCost(List<Point> path, double learningRate)
        {
            if (!_learningEnabled || path == null || path.Count == 0)
                return 0;

            double totalRisk = 0;
            foreach (var point in path)
            {
                totalRisk += GetObstacleCoefficient(point.X, point.Y, learningRate);
            }

            return totalRisk / path.Count;
        }

        /// <summary>
        /// Finds safer alternative path based on learning memory
        /// </summary>
        public List<Point> FindSaferPath(List<Point> originalPath, MapGrid grid, double learningRate)
        {
            if (!_learningEnabled) return originalPath;

            var saferPath = new List<Point>(originalPath);
            bool improved = true;
            int maxIterations = 10;
            int iteration = 0;

            while (improved && iteration < maxIterations)
            {
                improved = false;
                iteration++;

                for (int i = 1; i < saferPath.Count - 1; i++)
                {
                    var point = saferPath[i];
                    double currentRisk = GetObstacleCoefficient(point.X, point.Y, learningRate);

                    // Check neighboring cells for lower risk
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;

                            int nx = point.X + dx;
                            int ny = point.Y + dy;

                            if (!grid.IsValidCoordinate(nx, ny)) continue;
                            if (!grid[nx, ny].IsWalkable) continue;

                            double neighborRisk = GetObstacleCoefficient(nx, ny, learningRate);
                            if (neighborRisk < currentRisk * 0.8)  // 20% improvement
                            {
                                saferPath[i] = new Point(nx, ny);
                                currentRisk = neighborRisk;
                                improved = true;
                            }
                        }
                    }
                }
            }

            return saferPath;
        }
        #endregion

        #region Public Methods - Export
        /// <summary>
        /// Exports learning memory to CSV
        /// </summary>
        public string ExportToCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("X,Y,Frequency,ObstacleType,RiskLevel,FirstSeen,LastSeen,AverageIntervalSeconds,IsHotspot");

            foreach (var record in _memory.Values)
            {
                sb.AppendLine($"{record.X},{record.Y},{record.Frequency},{record.Type}," +
                             $"{record.RiskLevel:F1},{record.FirstSeen:yyyy-MM-dd HH:mm:ss}," +
                             $"{record.LastSeen:yyyy-MM-dd HH:mm:ss},{record.AverageIntervalSeconds:F1},{record.IsHotspot}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Exports learning memory to JSON
        /// </summary>
        public async Task ExportToJsonAsync(string filePath = null)
        {
            string path = filePath ?? _filePath;

            await Task.Run(() =>
            {
                try
                {
                    var data = new MemorySaveData
                    {
                        TotalDetections = _totalDetections,
                        TotalSimulations = _totalSimulations,
                        Records = _memory.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                    };

                    string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    File.WriteAllText(path, json);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error saving learning memory: {ex.Message}");
                }
            });
        }
        #endregion

        #region Public Methods - Statistics
        /// <summary>
        /// Gets learning statistics
        /// </summary>
        public LearningStatistics GetStatistics()
        {
            var stats = new LearningStatistics
            {
                TotalRecords = _memory.Count,
                TotalDetections = _totalDetections,
                TotalSimulations = _totalSimulations,
                HotspotCount = HotspotCount,
                AverageRiskLevel = _memory.Values.Average(r => r.RiskLevel),
                HighestRiskCell = _memory.Values.OrderByDescending(r => r.RiskLevel).FirstOrDefault(),
                MostFrequentCell = _memory.Values.OrderByDescending(r => r.Frequency).FirstOrDefault(),
                ObstacleTypeDistribution = _memory.Values
                    .GroupBy(r => r.Type)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return stats;
        }
        #endregion

        #region Save/Load
        /// <summary>
        /// Saves learning memory to disk
        /// </summary>
        public async Task SaveAsync()
        {
            await ExportToJsonAsync();
        }

        /// <summary>
        /// Loads learning memory from disk
        /// </summary>
        public async Task LoadAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(_filePath))
                    {
                        string json = File.ReadAllText(_filePath);
                        var data = JsonSerializer.Deserialize<MemorySaveData>(json);

                        if (data != null)
                        {
                            _totalDetections = data.TotalDetections;
                            _totalSimulations = data.TotalSimulations;
                            _memory = new ConcurrentDictionary<string, LearningRecord>(data.Records ?? new Dictionary<string, LearningRecord>());
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading learning memory: {ex.Message}");
                    _memory = new ConcurrentDictionary<string, LearningRecord>();
                    _totalDetections = 0;
                    _totalSimulations = 0;
                }
            });
        }
        #endregion

        #region Save Data Structure
        private sealed class MemorySaveData
        {
            public int TotalDetections { get; set; }
            public int TotalSimulations { get; set; }
            public Dictionary<string, LearningRecord> Records { get; set; }
        }
        #endregion

        #region Learning Statistics
        public sealed class LearningStatistics
        {
            public int TotalRecords { get; set; }
            public int TotalDetections { get; set; }
            public int TotalSimulations { get; set; }
            public int HotspotCount { get; set; }
            public double AverageRiskLevel { get; set; }
            public LearningRecord HighestRiskCell { get; set; }
            public LearningRecord MostFrequentCell { get; set; }
            public Dictionary<ObstacleType, int> ObstacleTypeDistribution { get; set; }
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (!_isDisposed)
            {
                SaveAsync().Wait();
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    #endregion
}
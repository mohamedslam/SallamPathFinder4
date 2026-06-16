#region File Header
/// <summary>
/// File: ObstacleDataCollector.cs
/// Description: Collects and manages training data for the neural network
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.ML.Models;
using System.Drawing;
using System.Text.Json;
#endregion

namespace SallamPathFinder4.ML.Training
{
    #region Movement Record Class
    public sealed class ObstacleMovementRecord
    {
        public int ObstacleId { get; set; }
        public ObstacleType ObstacleType { get; set; }
        public int PreviousX { get; set; }
        public int PreviousY { get; set; }
        public int CurrentX { get; set; }
        public int CurrentY { get; set; }
        public double Speed { get; set; }
        public double Direction { get; set; }
        public double Timestamp { get; set; }
        public DateTime RecordedAt { get; set; }
    }
    #endregion

    #region Class Documentation
    /// <summary>
    /// Collects and manages training data for the neural network
    /// Records obstacle movements and exports data for model training
    /// Thread-safe with locking mechanism
    /// </summary>
    #endregion
    public sealed class ObstacleDataCollector
    {
        #region Constants
        private const string DATA_FILE_NAME = "ObstacleTrainingData.json";
        private const int MAX_RECORDS = 100000;
        #endregion

        #region Private Fields
        private readonly List<ObstacleMovementRecord> _movementRecords;
        private readonly object _lockObject = new object();
        private int _nextObstacleId;
        private readonly Dictionary<DynamicObstacle, int> _obstacleIds;
        #endregion

        #region Constructor
        public ObstacleDataCollector()
        {
            _movementRecords = new List<ObstacleMovementRecord>();
            _obstacleIds = new Dictionary<DynamicObstacle, int>();
            _nextObstacleId = 1;
        }
        #endregion

        #region Properties
        public int TotalRecords => _movementRecords.Count;
        public bool HasData => TotalRecords > 0;
        public int MaxCapacity => MAX_RECORDS;
        #endregion

        #region Public Methods - Recording
        /// <summary>
        /// Records a movement of a dynamic obstacle
        /// </summary>
        public void RecordMovement(DynamicObstacle obstacle, Point previousPosition, double timestamp)
        {
            if (obstacle == null) return;

            lock (_lockObject)
            {
                // Get or assign obstacle ID
                if (!_obstacleIds.TryGetValue(obstacle, out int obstacleId))
                {
                    obstacleId = _nextObstacleId++;
                    _obstacleIds[obstacle] = obstacleId;
                }

                var record = new ObstacleMovementRecord
                {
                    ObstacleId = obstacleId,
                    ObstacleType = obstacle.Type,
                    PreviousX = previousPosition.X,
                    PreviousY = previousPosition.Y,
                    CurrentX = obstacle.Location.X,
                    CurrentY = obstacle.Location.Y,
                    Speed = obstacle.Speed,
                    Direction = CalculateDirection(previousPosition, obstacle.Location),
                    Timestamp = timestamp,
                    RecordedAt = DateTime.UtcNow
                };

                _movementRecords.Add(record);

                // Trim if exceeds max capacity
                if (_movementRecords.Count > MAX_RECORDS)
                {
                    _movementRecords.RemoveRange(0, _movementRecords.Count - MAX_RECORDS);
                }
            }
        }

        /// <summary>
        /// Records movements for all dynamic obstacles
        /// </summary>
        public void RecordMovements(List<DynamicObstacle> obstacles, double timestamp)
        {
            if (obstacles == null) return;

            foreach (var obstacle in obstacles)
            {
                if (obstacle.Trajectory.Count >= 2)
                {
                    var previous = obstacle.Trajectory[obstacle.Trajectory.Count - 2];
                    RecordMovement(obstacle, previous, timestamp);
                }
            }
        }
        #endregion

        #region Public Methods - Data Conversion
        /// <summary>
        /// Converts collected records to ML.NET input data format
        /// </summary>
        public List<ObstacleInputData> GetTrainingData()
        {
            lock (_lockObject)
            {
                var trainingData = new List<ObstacleInputData>();

                foreach (var record in _movementRecords)
                {
                    trainingData.Add(new ObstacleInputData
                    {
                        LastX = record.PreviousX,
                        LastY = record.PreviousY,
                        Velocity = (float)record.Speed,
                        Direction = (float)record.Direction,
                        ObstacleType = (float)record.ObstacleType,
                        TimeSinceLastSeen = (float)record.Timestamp,
                        NextX = record.CurrentX,
                        NextY = record.CurrentY
                    });
                }

                return trainingData;
            }
        }

        /// <summary>
        /// Gets historical data as obstacle memory records
        /// </summary>
        public List<LearningRecord> GetHistoricalData()
        {
            lock (_lockObject)
            {
                var historicalData = new List<LearningRecord>();
                var grouped = _movementRecords.GroupBy(r => (r.CurrentX, r.CurrentY));
                foreach (var group in grouped)
                {
                    var first = group.First();
                    var record = new LearningRecord(first.CurrentX, first.CurrentY, first.ObstacleType);
                    record.Frequency = group.Count();
                    record.LastSeen = group.Max(r => r.RecordedAt);
                    historicalData.Add(record);
                }

                return historicalData;
            }
        }
        #endregion

        #region Public Methods - Export
        /// <summary>
        /// Exports training data to JSON file
        /// </summary>
        public async Task ExportToJsonAsync(string filePath = DATA_FILE_NAME)
        {
            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    try
                    {
                        var json = JsonSerializer.Serialize(_movementRecords, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });
                        File.WriteAllText(filePath, json);
                        System.Diagnostics.Debug.WriteLine($"Exported {_movementRecords.Count} records to {filePath}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error exporting data: {ex.Message}");
                    }
                }
            });
        }

        /// <summary>
        /// Exports training data to CSV file
        /// </summary>
        public async Task ExportToCsvAsync(string filePath)
        {
            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    try
                    {
                        using var writer = new StreamWriter(filePath);
                        writer.WriteLine("ObstacleId,Type,PrevX,PrevY,CurrX,CurrY,Speed,Direction,Timestamp,RecordedAt");

                        foreach (var record in _movementRecords)
                        {
                            writer.WriteLine($"{record.ObstacleId},{record.ObstacleType},{record.PreviousX},{record.PreviousY}," +
                                $"{record.CurrentX},{record.CurrentY},{record.Speed:F2},{record.Direction:F2},{record.Timestamp},{record.RecordedAt:O}");
                        }

                        System.Diagnostics.Debug.WriteLine($"Exported {_movementRecords.Count} records to {filePath}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error exporting CSV: {ex.Message}");
                    }
                }
            });
        }

        /// <summary>
        /// Imports training data from JSON file
        /// </summary>
        public async Task ImportFromJsonAsync(string filePath)
        {
            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            var json = File.ReadAllText(filePath);
                            var records = JsonSerializer.Deserialize<List<ObstacleMovementRecord>>(json);

                            if (records != null)
                            {
                                _movementRecords.Clear();
                                _movementRecords.AddRange(records);
                                System.Diagnostics.Debug.WriteLine($"Imported {_movementRecords.Count} records from {filePath}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error importing data: {ex.Message}");
                    }
                }
            });
        }
        #endregion

        #region Public Methods - Management
        /// <summary>
        /// Clears all collected data
        /// </summary>
        public void Clear()
        {
            lock (_lockObject)
            {
                _movementRecords.Clear();
                _obstacleIds.Clear();
                _nextObstacleId = 1;
            }
        }

        /// <summary>
        /// Gets statistics about collected data
        /// </summary>
        public DataStatistics GetStatistics()
        {
            lock (_lockObject)
            {
                if (_movementRecords.Count == 0)
                    return new DataStatistics();

                return new DataStatistics
                {
                    TotalRecords = _movementRecords.Count,
                    UniqueObstacles = _obstacleIds.Count,
                    DateRange = new DateTimeRange
                    {
                        Start = _movementRecords.Min(r => r.RecordedAt),
                        End = _movementRecords.Max(r => r.RecordedAt)
                    },
                    ObstacleTypeDistribution = _movementRecords
                        .GroupBy(r => r.ObstacleType)
                        .ToDictionary(g => g.Key, g => g.Count())
                };
            }
        }
        #endregion

        #region Private Methods
        private static double CalculateDirection(Point from, Point to)
        {
            return Math.Atan2(to.Y - from.Y, to.X - from.X) * 180 / Math.PI;
        }
        #endregion
    }

    #region Statistics Classes
    public sealed class DataStatistics
    {
        public int TotalRecords { get; set; }
        public int UniqueObstacles { get; set; }
        public DateTimeRange DateRange { get; set; }
        public Dictionary<ObstacleType, int> ObstacleTypeDistribution { get; set; }
    }

    public sealed class DateTimeRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double TotalDays => (End - Start).TotalDays;
    }
    #endregion
}
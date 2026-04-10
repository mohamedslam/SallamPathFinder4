#region File Header
/// <summary>
/// File: ObstacleMemory.cs
/// Description: Persistent memory for obstacle detections across multiple simulations
/// Enables long-term learning for SPPA-DL algorithm
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using System.Drawing;
using System.Text.Json;
#endregion

namespace SallamPathFinder4.Core.Models.Obstacles
{
    #region Record Class Documentation
    /// <summary>
    /// Represents a single obstacle memory record for a specific cell
    /// Stores frequency, last seen time, and obstacle type distribution
    /// </summary>
    #endregion
    public sealed class ObstacleMemoryRecord
    {
        #region Constructor
        /// <summary>
        /// Default constructor for JSON serialization
        /// </summary>
        public ObstacleMemoryRecord()
        {
            TypeCounts = new Dictionary<ObstacleType, int>();
            LastSeen = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a new obstacle memory record at specified cell
        /// </summary>
        public ObstacleMemoryRecord(int x, int y, ObstacleType type) : this()
        {
            X = x;
            Y = y;
            Frequency = 1;
            LastSeen = DateTime.UtcNow;
            Type = type;
            TypeCounts[type] = 1;
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

        /// <summary>Dominant obstacle type at this cell</summary>
        public ObstacleType Type { get; set; }

        /// <summary>Distribution of obstacle types by count</summary>
        public Dictionary<ObstacleType, int> TypeCounts { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the record with a new obstacle detection
        /// </summary>
        public void Update(ObstacleType type)
        {
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
        }

        /// <summary>
        /// Calculates the obstacle coefficient for learning formula
        /// Formula: α * (Frequency / TotalSimulations)
        /// </summary>
        public double GetObstacleCoefficient(int totalSimulations, double learningRate)
        {
            if (totalSimulations <= 0) return 0;
            double coefficient = learningRate * ((double)Frequency / totalSimulations);
            return Math.Min(learningRate, coefficient);
        }
        #endregion
    }

    #region Class Documentation
    /// <summary>
    /// Manages persistent memory of obstacle detections across multiple simulations
    /// Saves and loads data to/from JSON file for long-term learning
    /// Thread-safe for concurrent access
    /// </summary>
    #endregion
    public sealed class ObstacleMemory : IDisposable
    {
        #region Constants
        private const string DEFAULT_FOLDER_NAME = "SallamPathFinder4";
        private const string DEFAULT_FILE_NAME = "ObstacleMemory.json";
        #endregion

        #region Private Fields
        private Dictionary<string, ObstacleMemoryRecord> _memory;
        private int _totalDetections;
        private int _totalSimulations;
        private readonly string _filePath;
        private readonly object _lockObject;
        private bool _isDisposed;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new obstacle memory manager
        /// </summary>
        /// <param name="fileName">Name of the JSON file to store memory</param>
        
        public ObstacleMemory(string fileName = DEFAULT_FILE_NAME)
        {
            _memory = new Dictionary<string, ObstacleMemoryRecord>();
            _totalDetections = 0;
            _totalSimulations = 0;
            _lockObject = new object();
            _isDisposed = false;

            // Use AppData folder for persistent storage
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder =System.IO. Path.Combine(appDataPath, "SallamPathFinder4");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            _filePath = System.IO.Path.Combine(appFolder, fileName);
        }
        #endregion

        #region Properties
        /// <summary>Total number of obstacle detections recorded</summary>
        public int TotalDetections
        {
            get
            {
                lock (_lockObject) { return _totalDetections; }
            }
        }

        /// <summary>Total number of simulations completed</summary>
        public int TotalSimulations
        {
            get
            {
                lock (_lockObject) { return _totalSimulations; }
            }
        }

        /// <summary>Number of cells with memory records</summary>
        public int RecordCount
        {
            get
            {
                lock (_lockObject) { return _memory.Count; }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Generates a unique key for cell coordinates
        /// </summary>
        private static string GetKey(int x, int y) => $"{x},{y}";
        #endregion

        #region Public Methods - Recording
        /// <summary>
        /// Records an obstacle detection at the specified cell
        /// </summary>
        public void RecordDetection(int x, int y, ObstacleType type)
        {
            lock (_lockObject)
            {
                string key = GetKey(x, y);

                if (_memory.ContainsKey(key))
                {
                    _memory[key].Update(type);
                }
                else
                {
                    _memory[key] = new ObstacleMemoryRecord(x, y, type);
                }

                _totalDetections++;
            }
        }

        /// <summary>
        /// Increments the total simulations counter
        /// Call at the end of each complete simulation
        /// </summary>
        public void IncrementSimulation()
        {
            lock (_lockObject)
            {
                _totalSimulations++;
            }
        }

        /// <summary>
        /// Clears all obstacle memory
        /// </summary>
        public void Clear()
        {
            lock (_lockObject)
            {
                _memory.Clear();
                _totalDetections = 0;
                _totalSimulations = 0;
            }
        }
        #endregion

        #region Public Methods - Querying
        /// <summary>
        /// Gets the obstacle coefficient for a specific cell
        /// </summary>
        public double GetObstacleCoefficient(int x, int y, double learningRate)
        {
            lock (_lockObject)
            {
                string key = GetKey(x, y);
                if (_memory.ContainsKey(key))
                {
                    return _memory[key].GetObstacleCoefficient(_totalSimulations, learningRate);
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the obstacle coefficient with default learning rate (2.0)
        /// </summary>
        public double GetObstacleCoefficient(int x, int y)
        {
            return GetObstacleCoefficient(x, y, 2.0);
        }

        /// <summary>
        /// Gets the frequency of obstacle detections at a specific cell
        /// </summary>
        public int GetFrequency(int x, int y)
        {
            lock (_lockObject)
            {
                string key = GetKey(x, y);
                return _memory.ContainsKey(key) ? _memory[key].Frequency : 0;
            }
        }

        /// <summary>
        /// Gets the dominant obstacle type at a specific cell
        /// </summary>
        public ObstacleType? GetDominantType(int x, int y)
        {
            lock (_lockObject)
            {
                string key = GetKey(x, y);
                return _memory.ContainsKey(key) ? _memory[key].Type : (ObstacleType?)null;
            }
        }

        /// <summary>
        /// Checks if a cell has any memory records
        /// </summary>
        public bool HasMemory(int x, int y)
        {
            lock (_lockObject)
            {
                return _memory.ContainsKey(GetKey(x, y));
            }
        }

        /// <summary>
        /// Gets all cells that have memory records
        /// </summary>
        public List<Point> GetMemoryCells()
        {
            lock (_lockObject)
            {
                var cells = new List<Point>();
                foreach (var key in _memory.Keys)
                {
                    var parts = key.Split(',');
                    if (parts.Length == 2)
                    {
                        if (int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                        {
                            cells.Add(new Point(x, y));
                        }
                    }
                }
                return cells;
            }
        }

        /// <summary>
        /// Gets all memory records
        /// </summary>
        public List<ObstacleMemoryRecord> GetAllRecords()
        {
            lock (_lockObject)
            {
                return new List<ObstacleMemoryRecord>(_memory.Values);
            }
        }
        #endregion

        #region Public Methods - Persistence
        /// <summary>
        /// Saves obstacle memory to JSON file asynchronously
        /// </summary>
        public async Task SaveAsync()
        {
            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    try
                    {
                        var data = new MemorySaveData
                        {
                            TotalDetections = _totalDetections,
                            TotalSimulations = _totalSimulations,
                            Records = _memory
                        };

                        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        File.WriteAllText(_filePath, json);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error saving obstacle memory: {ex.Message}");
                    }
                }
            });
        }

        /// <summary>
        /// Loads obstacle memory from JSON file asynchronously
        /// </summary>
        public async Task LoadAsync()
        {
            await Task.Run(() =>
            {
                lock (_lockObject)
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
                                _memory = data.Records ?? new Dictionary<string, ObstacleMemoryRecord>();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading obstacle memory: {ex.Message}");
                        _memory = new Dictionary<string, ObstacleMemoryRecord>();
                        _totalDetections = 0;
                        _totalSimulations = 0;
                    }
                }
            });
        }
        #endregion

        #region Save Data Structure
        /// <summary>
        /// Save data structure for JSON serialization
        /// </summary>
        private sealed class MemorySaveData
        {
            public int TotalDetections { get; set; }
            public int TotalSimulations { get; set; }
            public Dictionary<string, ObstacleMemoryRecord> Records { get; set; }
        }
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Disposes of resources
        /// </summary>
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
}
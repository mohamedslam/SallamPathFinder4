#region File Header
/// <summary>
/// File: ExperimentService.cs
/// Description: Service for experiment logging and management
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Experiments;
#endregion

namespace SallamPathFinder4.Services.Experiment
{
    #region Class Documentation
    /// <summary>
    /// Service for experiment logging and management
    /// Saves experiment results to CSV files and provides querying capabilities
    /// Thread-safe with proper locking
    /// </summary>
    #endregion
    public sealed class ExperimentService : IExperimentService
    {
        #region Constants
        private const string DEFAULT_LOG_DIRECTORY = "Experiments";
        private const string DEFAULT_LOG_FILE = "log.csv";
        #endregion

        #region Private Fields
        private readonly string _logPath;
        private readonly object _lockObject = new object();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new experiment service
        /// </summary>
        public ExperimentService(string logDirectory = DEFAULT_LOG_DIRECTORY)
        {
            _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logDirectory, DEFAULT_LOG_FILE);

            var directory = Path.GetDirectoryName(_logPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public async Task LogExperimentAsync(ExperimentData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    bool fileExists = System.IO.File.Exists(_logPath);
                    using var writer = new StreamWriter(_logPath, true, Encoding.UTF8);

                    if (!fileExists)
                        writer.WriteLine(ExperimentData.GetCsvHeader());

                    writer.WriteLine(data.ToCsvLine());
                }
            });
        }

        /// <inheritdoc/>
        public List<ExperimentData> GetAllExperiments()
        {
            lock (_lockObject)
            {
                var experiments = new List<ExperimentData>();

                if (!System.IO. File.Exists(_logPath))
                    return experiments;

                var lines = System.IO.File.ReadAllLines(_logPath);

                if (lines.Length < 2)
                    return experiments;

                var header = lines[0].Split(',');
                var columnIndex = GetColumnIndexMap(header);

                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    var experiment = ParseCsvLine(lines[i], columnIndex);
                    if (experiment != null)
                        experiments.Add(experiment);
                }

                return experiments;
            }
        }

        /// <inheritdoc/>
        public async Task ExportAllAsync(string outputPath)
        {
            if (string.IsNullOrEmpty(outputPath))
                throw new ArgumentNullException(nameof(outputPath));

            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    if (System.IO.File.Exists(_logPath))
                        System.IO.File.Copy(_logPath, outputPath, true);
                }
            });
        }

        /// <inheritdoc/>
        public async Task ExportSelectedAsync(List<string> experimentIds, string outputPath)
        {
            if (experimentIds == null || experimentIds.Count == 0)
                throw new ArgumentException("Experiment IDs cannot be null or empty", nameof(experimentIds));

            if (string.IsNullOrEmpty(outputPath))
                throw new ArgumentNullException(nameof(outputPath));

            await Task.Run(() =>
            {
                var allExperiments = GetAllExperiments();
                var selected = allExperiments.Where(e => experimentIds.Contains(e.ExperimentId)).ToList();

                using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);
                writer.WriteLine(ExperimentData.GetCsvHeader());

                foreach (var exp in selected)
                    writer.WriteLine(exp.ToCsvLine());
            });
        }
        #endregion

        /// <inheritdoc/>
        public async Task<bool> DeleteExperimentAsync(string experimentId)
        {
            if (string.IsNullOrEmpty(experimentId))
                return false;

            return await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    try
                    {
                        var experiments = GetAllExperiments();
                        var toRemove = experiments.FirstOrDefault(e => e.ExperimentId == experimentId);

                        if (toRemove == null)
                            return false;

                        experiments.Remove(toRemove);
                        RewriteAllExperiments(experiments);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error deleting experiment: {ex.Message}");
                        return false;
                    }
                }
            });
        }

        /// <inheritdoc/>
        public async Task<int> DeleteExperimentsAsync(List<string> experimentIds)
        {
            if (experimentIds == null || experimentIds.Count == 0)
                return 0;

            return await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    try
                    {
                        var experiments = GetAllExperiments();
                        var toRemove = experiments.Where(e => experimentIds.Contains(e.ExperimentId)).ToList();

                        if (toRemove.Count == 0)
                            return 0;

                        foreach (var exp in toRemove)
                            experiments.Remove(exp);

                        RewriteAllExperiments(experiments);
                        return toRemove.Count;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error deleting experiments: {ex.Message}");
                        return 0;
                    }
                }
            });
        }

        private void RewriteAllExperiments(List<ExperimentData> experiments)
        {
            var directory = Path.GetDirectoryName(_logPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using var writer = new StreamWriter(_logPath, false, Encoding.UTF8);
            writer.WriteLine(ExperimentData.GetCsvHeader());

            foreach (var exp in experiments)
                writer.WriteLine(exp.ToCsvLine());
        }

        #region Private Helper Methods
        private Dictionary<string, int> GetColumnIndexMap(string[] header)
        {
            var map = new Dictionary<string, int>();
            for (int i = 0; i < header.Length; i++)
            {
                map[header[i].Trim()] = i;
            }
            return map;
        }

        private string GetValue(string[] parts, Dictionary<string, int> columnIndex, string columnName)
        {
            if (columnIndex.TryGetValue(columnName, out int idx) && idx < parts.Length)
                return parts[idx];
            return "";
        }

        private ExperimentData ParseCsvLine(string line, Dictionary<string, int> columnIndex)
        {
            try
            {
                var parts = line.Split(',');

                return new ExperimentData
                {
                    ExperimentId = GetValue(parts, columnIndex, "ExperimentId"),
                    Timestamp = DateTime.TryParse(GetValue(parts, columnIndex, "Timestamp"), out DateTime ts) ? ts : DateTime.UtcNow,
                    AlgorithmName = GetValue(parts, columnIndex, "Algorithm"),
                    DistanceMetric = GetValue(parts, columnIndex, "DistanceMetric"),
                    GridWidth = int.TryParse(GetValue(parts, columnIndex, "GridWidth"), out int gw) ? gw : 0,
                    GridHeight = int.TryParse(GetValue(parts, columnIndex, "GridHeight"), out int gh) ? gh : 0,
                    ScaleCmPerCell = double.TryParse(GetValue(parts, columnIndex, "ScaleCmPerCell"), out double sc) ? sc : 1.0,
                    GoalCount = int.TryParse(GetValue(parts, columnIndex, "GoalCount"), out int gc) ? gc : 0,
                    ParkingCount = int.TryParse(GetValue(parts, columnIndex, "ParkingCount"), out int pc) ? pc : 0,
                    SearchTimeMs = double.TryParse(GetValue(parts, columnIndex, "SearchTimeMs"), out double st) ? st : 0,
                    PathLengthCells = int.TryParse(GetValue(parts, columnIndex, "PathLengthCells"), out int pl) ? pl : 0,
                    ReplanCount = int.TryParse(GetValue(parts, columnIndex, "ReplanCount"), out int rc) ? rc : 0,
                    SimulationTimeSec = double.TryParse(GetValue(parts, columnIndex, "SimulationTimeSec"), out double sim) ? sim : 0,
                    BatteryConsumption = double.TryParse(GetValue(parts, columnIndex, "BatteryConsumption"), out double bc) ? bc : 0,
                    Success = bool.TryParse(GetValue(parts, columnIndex, "Success"), out bool suc) ? suc : false,
                    RobotSpeedCms = double.TryParse(GetValue(parts, columnIndex, "RobotSpeedCms"), out double rs) ? rs : 0,
                    HeuristicWeight = double.TryParse(GetValue(parts, columnIndex, "HeuristicWeight"), out double hw) ? hw : 0,
                    Diagonals = bool.TryParse(GetValue(parts, columnIndex, "Diagonals"), out bool diag) ? diag : false
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parsing CSV line: {ex.Message}");
                return null;
            }
        }
        #endregion
    }
}
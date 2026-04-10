#region File Header
/// <summary>
/// File: ExperimentBrowserLogic.cs
/// Description: Business logic for experiment browser form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-09
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SallamPathFinder4.Core.Models.Experiments;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentBrowser
{
    #region Class Documentation
    /// <summary>
    /// Business logic for experiment browser operations
    /// </summary>
    #endregion
    public sealed class ExperimentBrowserLogic
    {
        #region Constants
        private const string EXPERIMENTS_SUBFOLDER = "Experiments";
        #endregion

        #region Constructor
        public ExperimentBrowserLogic()
        {
        }
        #endregion

        #region Public Methods - Path
        /// <summary>
        /// Gets the experiments folder path
        /// </summary>
        public string GetExperimentsPath()
        {
            string documentsPath = System. Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentsPath, "SallamPathFinder4", EXPERIMENTS_SUBFOLDER);
        }
        #endregion

        #region Public Methods - Load
        /// <summary>
        /// Loads experiment data from a folder
        /// </summary>
        public ExperimentData LoadExperimentFromFolder(string folderPath)
        {
            // Try to find CSV file (could be Results.csv or ExperimentData.csv)
            string csvPath = Path.Combine(folderPath, "Results.csv");

            if (!File.Exists(csvPath))
            {
                csvPath = Path.Combine(folderPath, "ExperimentData.csv");

                if (!File.Exists(csvPath))
                {
                    System.Diagnostics.Debug.WriteLine($"No CSV file found in {folderPath}");
                    return null;
                }
            }

            try
            {
                var lines = File.ReadAllLines(csvPath);

                if (lines.Length < 2)
                {
                    System.Diagnostics.Debug.WriteLine($"CSV file has only {lines.Length} lines in {folderPath}");
                    return null;
                }

                // Read header to determine column indices
                string header = lines[0];
                var headers = header.Split(',');

                // Parse the first data line
                string dataLine = lines[1];
                var parts = dataLine.Split(',');

                System.Diagnostics.Debug.WriteLine($"Parsing CSV: {parts.Length} columns");

                // Create experiment with default values
                var experiment = new ExperimentData();

                // Map columns by index (adjust based on your CSV format)
                for (int i = 0; i < Math.Min(headers.Length, parts.Length); i++)
                {
                    string columnName = headers[i].Trim();
                    string value = parts[i].Trim();

                    switch (columnName)
                    {
                        case "ExperimentId":
                            experiment.ExperimentId = value;
                            break;

                        case "Timestamp":
                            DateTime.TryParse(value, out DateTime dt);
                            experiment.Timestamp = dt;
                            break;

                        case "Algorithm":
                        case "AlgorithmName":
                            experiment.AlgorithmName = value;
                            break;

                        case "DistanceMetric":
                        case "Metric":
                            experiment.DistanceMetric = value;
                            break;

                        case "SearchTimeMs":
                        case "ComputationTimeMs":
                            double.TryParse(value, out double time);
                            experiment.SearchTimeMs = time;
                            break;

                        case "PathLengthCells":
                        case "PathLength":
                            int.TryParse(value, out int length);
                            experiment.PathLengthCells = length;
                            break;

                        case "Success":
                            bool.TryParse(value, out bool success);
                            experiment.Success = success;
                            break;

                        case "CollisionCount":
                            int.TryParse(value, out int collisions);
                            experiment.CollisionCount = collisions;
                            break;
                    }
                }

                // Set default values if not found
                if (string.IsNullOrEmpty(experiment.ExperimentId))
                {
                    experiment.ExperimentId = Path.GetFileName(folderPath);
                }

                if (string.IsNullOrEmpty(experiment.AlgorithmName))
                {
                    experiment.AlgorithmName = "Unknown";
                }

                System.Diagnostics.Debug.WriteLine($"Loaded experiment: {experiment.ExperimentId} - {experiment.AlgorithmName}");

                return experiment;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading {folderPath}: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Public Methods - Statistics
        /// <summary>
        /// Calculates statistics for a list of experiments
        /// </summary>
        public string CalculateStatistics(List<ExperimentData> experiments)
        {
            if (experiments == null || experiments.Count == 0)
            {
                return "No experiments selected";
            }

            int total = experiments.Count;
            int successCount = experiments.Count(e => e.Success);
            double successRate = total > 0 ? (double)successCount / total * 100 : 0;
            double avgTime = experiments.Average(e => e.SearchTimeMs);
            double avgLength = experiments.Average(e => (double)e.PathLengthCells);
            double avgCollisions = experiments.Average(e => (double)e.CollisionCount);

            var bestAlgorithm = experiments
                .Where(e => e.Success)
                .GroupBy(e => e.AlgorithmName)
                .Select(g => new { Algorithm = g.Key, AvgTime = g.Average(e => e.SearchTimeMs) })
                .OrderBy(a => a.AvgTime)
                .FirstOrDefault();

            return $"📊 Total: {total} | ✅ Success: {successCount} ({successRate:F1}%) | " +
                   $"⏱️ Avg Time: {avgTime:F2} ms | 📏 Avg Length: {avgLength:F0} | " +
                   $"💥 Avg Collisions: {avgCollisions:F1} | " +
                   $"🏆 Best: {(bestAlgorithm?.Algorithm ?? "N/A")}";
        }
        #endregion

        #region Public Methods - Delete
        /// <summary>
        /// Deletes selected experiments
        /// </summary>
        public async Task<int> DeleteExperimentsAsync(string experimentsPath, List<int> indices, CheckedListBox checkedListFolders)
        {
            return await Task.Run(() =>
            {
                int deletedCount = 0;

                foreach (int index in indices.OrderByDescending(i => i))
                {
                    if (index >= checkedListFolders.Items.Count)
                    {
                        continue;
                    }

                    string displayName = checkedListFolders.Items[index].ToString();
                    string folderName = displayName.Replace("📁 ", "");
                    string folderPath = Path.Combine(experimentsPath, folderName);

                    try
                    {
                        if (Directory.Exists(folderPath))
                        {
                            Directory.Delete(folderPath, true);
                            deletedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error deleting {folderName}: {ex.Message}");
                    }
                }

                return deletedCount;
            });
        }
        #endregion

        #region Public Methods - Export
        /// <summary>
        /// Exports experiments to CSV file
        /// </summary>
        public void ExportToCsv(List<ExperimentData> experiments, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("ExperimentId,Date,Algorithm,TimeMs,PathLength,Success,Collisions");

            foreach (var exp in experiments)
            {
                writer.WriteLine($"{exp.ExperimentId},{exp.Timestamp:yyyy-MM-dd HH:mm:ss}," +
                    $"{exp.AlgorithmName},{exp.SearchTimeMs:F2},{exp.PathLengthCells}," +
                    $"{exp.Success},{exp.CollisionCount}");
            }
        }
        #endregion
    }
}
#region File Header
/// <summary>
/// File: StatisticsViewerLogic.cs
/// Description: Business logic for statistics viewer calculations
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.WinForms.Models;
using System.Windows.Forms.DataVisualization.Charting;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmStatisticsViewer.Core
{
    /// <summary>
    /// Business logic for calculating statistics and managing chart exports
    /// </summary>
    public sealed class StatisticsViewerLogic
    {
        #region Constructor
        public StatisticsViewerLogic()
        {
        }
        #endregion

        #region Algorithm Statistics
        /// <summary>
        /// Calculates algorithm statistics for a specific algorithm
        /// </summary>
        public AlgorithmStats CalculateAlgorithmStats(List<ExperimentResultItem> results, string algorithm)
        {
            var algoResults = results.Where(r => r.Algorithm == algorithm).ToList();
            var successful = algoResults.Where(r => r.Success).ToList();

            return new AlgorithmStats
            {
                TotalRuns = algoResults.Count,
                SuccessCount = successful.Count,
                SuccessRate = algoResults.Count > 0 ? (double)successful.Count / algoResults.Count * 100 : 0,
                AvgTimeMs = successful.Any() ? successful.Average(r => r.ComputationTimeMs) : 0,
                MinTimeMs = successful.Any() ? successful.Min(r => r.ComputationTimeMs) : 0,
                MaxTimeMs = successful.Any() ? successful.Max(r => r.ComputationTimeMs) : 0,
                StdDevTimeMs = CalculateStdDev(successful.Select(r => r.ComputationTimeMs)),
                AvgLength = successful.Any() ? successful.Average(r => (double)r.PathLength) : 0,
                MinLength = successful.Any() ? successful.Min(r => r.PathLength) : 0,
                MaxLength = successful.Any() ? successful.Max(r => r.PathLength) : 0,
                AvgBattery = successful.Any() ? successful.Average(r => r.RemainingBattery) : 0,
                AvgCollisions = algoResults.Any() ? algoResults.Average(r => r.CollisionCount) : 0,
                AvgErrors = algoResults.Any() ? algoResults.Average(r => r.InvalidMoveCount) : 0,
                AvgSpeed = successful.Any() ? successful.Average(r => r.AverageActualSpeed) : 0
            };
        }

        /// <summary>
        /// Calculates metric-based statistics
        /// </summary>
        public MetricStats CalculateMetricStats(List<ExperimentResultItem> results)
        {
            int total = results.Count;
            int success = results.Count(r => r.Success);

            return new MetricStats
            {
                TotalRuns = total,
                SuccessRate = total > 0 ? (double)success / total * 100 : 0,
                AvgTimeMs = results.Where(r => r.Success).Select(r => r.ComputationTimeMs).DefaultIfEmpty(0).Average(),
                AvgLength = results.Where(r => r.Success).Select(r => (double)r.PathLength).DefaultIfEmpty(0).Average(),
                AvgBattery = results.Where(r => r.Success).Select(r => r.RemainingBattery).DefaultIfEmpty(0).Average(),
                AvgCollisions = results.Any() ? results.Average(r => r.CollisionCount) : 0
            };
        }

        /// <summary>
        /// Calculates collision and error statistics
        /// </summary>
        public CollisionStats CalculateCollisionStats(List<ExperimentResultItem> results, string algorithm)
        {
            var algoResults = results.Where(r => r.Algorithm == algorithm).ToList();
            int failureCount = algoResults.Count(r => !r.Success);

            return new CollisionStats
            {
                TotalCollisions = algoResults.Sum(r => r.CollisionCount),
                AvgCollisions = algoResults.Any() ? algoResults.Average(r => r.CollisionCount) : 0,
                MaxCollisions = algoResults.Any() ? algoResults.Max(r => r.CollisionCount) : 0,
                TotalErrors = algoResults.Sum(r => r.InvalidMoveCount),
                AvgErrors = algoResults.Any() ? algoResults.Average(r => r.InvalidMoveCount) : 0,
                FailureRate = algoResults.Any() ? (double)failureCount / algoResults.Count * 100 : 0
            };
        }

        /// <summary>
        /// Calculates overall summary statistics
        /// </summary>
        public SummaryStats CalculateSummaryStats(List<ExperimentResultItem> results)
        {
            int total = results.Count;
            int successCount = results.Count(r => r.Success);

            var bestTime = results.Where(r => r.Success).OrderBy(r => r.ComputationTimeMs).FirstOrDefault();
            var bestLength = results.Where(r => r.Success).OrderBy(r => r.PathLength).FirstOrDefault();
            var leastCollisions = results.OrderBy(r => r.CollisionCount).FirstOrDefault();

            return new SummaryStats
            {
                TotalExperiments = total,
                SuccessCount = successCount,
                SuccessRate = total > 0 ? (double)successCount / total * 100 : 0,
                AvgTimeMs = results.Where(r => r.Success).Select(r => r.ComputationTimeMs).DefaultIfEmpty(0).Average(),
                AvgLength = results.Where(r => r.Success).Select(r => (double)r.PathLength).DefaultIfEmpty(0).Average(),
                AvgBattery = results.Where(r => r.Success).Select(r => r.RemainingBattery).DefaultIfEmpty(0).Average(),
                AvgCollisions = results.Any() ? results.Average(r => r.CollisionCount) : 0,
                AvgSpeed = results.Where(r => r.Success).Select(r => r.AverageActualSpeed).DefaultIfEmpty(0).Average(),
                FastestAlgorithm = bestTime?.Algorithm ?? "N/A",
                FastestTimeMs = bestTime?.ComputationTimeMs ?? 0,
                ShortestPathAlgorithm = bestLength?.Algorithm ?? "N/A",
                ShortestPathLength = bestLength?.PathLength ?? 0,
                SafestAlgorithm = leastCollisions?.Algorithm ?? "N/A",
                SafestCollisions = leastCollisions?.CollisionCount ?? 0
            };
        }

        /// <summary>
        /// Gets average time for a specific algorithm
        /// </summary>
        public double GetAverageTimeForAlgorithm(List<ExperimentResultItem> results, string algorithm)
        {
            return results.Where(r => r.Algorithm == algorithm && r.Success)
                         .Select(r => r.ComputationTimeMs)
                         .DefaultIfEmpty(0)
                         .Average();
        }

        /// <summary>
        /// Gets success rate for a specific algorithm
        /// </summary>
        public double GetSuccessRateForAlgorithm(List<ExperimentResultItem> results, string algorithm)
        {
            var algoResults = results.Where(r => r.Algorithm == algorithm).ToList();
            int total = algoResults.Count;
            int success = algoResults.Count(r => r.Success);
            return total > 0 ? (double)success / total * 100 : 0;
        }

        /// <summary>
        /// Gets average collisions for a specific algorithm
        /// </summary>
        public double GetAverageCollisionsForAlgorithm(List<ExperimentResultItem> results, string algorithm)
        {
            return results.Where(r => r.Algorithm == algorithm)
                         .Select(r => (double)r.CollisionCount)
                         .DefaultIfEmpty(0)
                         .Average();
        }

        /// <summary>
        /// Gets average speed for a specific algorithm
        /// </summary>
        public double GetAverageSpeedForAlgorithm(List<ExperimentResultItem> results, string algorithm)
        {
            return results.Where(r => r.Algorithm == algorithm && r.Success)
                         .Select(r => r.AverageActualSpeed)
                         .DefaultIfEmpty(0)
                         .Average();
        }
        #endregion

        #region Export Methods
        /// <summary>
        /// Saves a chart as an image file
        /// </summary>
        public void SaveChartAsImage(Chart chart, string filePath)
        {
            try
            {
                chart.SaveImage(filePath, ChartImageFormat.Png);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save chart: {ex.Message}");
            }
        }

        /// <summary>
        /// Exports statistics to CSV file
        /// </summary>
        public void ExportStatisticsToCsv(List<ExperimentResultItem> results, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("Algorithm,TotalRuns,SuccessCount,SuccessRate,AvgTimeMs,AvgLength,AvgCollisions,AvgSpeedCmS");

            var algorithms = results.Select(r => r.Algorithm).Distinct();
            foreach (var algo in algorithms)
            {
                var stats = CalculateAlgorithmStats(results, algo);
                writer.WriteLine($"{algo},{stats.TotalRuns},{stats.SuccessCount},{stats.SuccessRate:F1}," +
                    $"{stats.AvgTimeMs:F2},{stats.AvgLength:F0},{stats.AvgCollisions:F1},{stats.AvgSpeed:F1}");
            }
        }
        #endregion

        #region Helper Methods
        private double CalculateStdDev(IEnumerable<double> values)
        {
            var list = values.ToList();
            if (!list.Any()) return 0;

            double avg = list.Average();
            double sumOfSquares = list.Sum(v => Math.Pow(v - avg, 2));
            return Math.Sqrt(sumOfSquares / list.Count);
        }
        #endregion
    }

    #region Statistics Data Classes
    public class AlgorithmStats
    {
        public int TotalRuns { get; set; }
        public int SuccessCount { get; set; }
        public double SuccessRate { get; set; }
        public double AvgTimeMs { get; set; }
        public double MinTimeMs { get; set; }
        public double MaxTimeMs { get; set; }
        public double StdDevTimeMs { get; set; }
        public double AvgLength { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public double AvgBattery { get; set; }
        public double AvgCollisions { get; set; }
        public double AvgErrors { get; set; }
        public double AvgSpeed { get; set; }
    }

    public class MetricStats
    {
        public int TotalRuns { get; set; }
        public double SuccessRate { get; set; }
        public double AvgTimeMs { get; set; }
        public double AvgLength { get; set; }
        public double AvgBattery { get; set; }
        public double AvgCollisions { get; set; }
    }

    public class CollisionStats
    {
        public int TotalCollisions { get; set; }
        public double AvgCollisions { get; set; }
        public int MaxCollisions { get; set; }
        public int TotalErrors { get; set; }
        public double AvgErrors { get; set; }
        public double FailureRate { get; set; }
    }

    public class SummaryStats
    {
        public int TotalExperiments { get; set; }
        public int SuccessCount { get; set; }
        public double SuccessRate { get; set; }
        public double AvgTimeMs { get; set; }
        public double AvgLength { get; set; }
        public double AvgBattery { get; set; }
        public double AvgCollisions { get; set; }
        public double AvgSpeed { get; set; }
        public string FastestAlgorithm { get; set; }
        public double FastestTimeMs { get; set; }
        public string ShortestPathAlgorithm { get; set; }
        public int ShortestPathLength { get; set; }
        public string SafestAlgorithm { get; set; }
        public int SafestCollisions { get; set; }
    }
    #endregion
}
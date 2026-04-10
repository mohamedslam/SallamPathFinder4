#region File Header
/// <summary>
/// File: ExperimentViewerLogic.cs
/// Description: Business logic for experiment viewer form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Experiments;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentViewer.Core
{
    /// <summary>
    /// Business logic for filtering and managing experiment data
    /// </summary>
    public sealed class ExperimentViewerLogic
    {
        #region Constructor
        public ExperimentViewerLogic()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Filters experiments based on provided criteria
        /// </summary>
        public List<ExperimentData> FilterExperiments(
            List<ExperimentData> experiments,
            string algorithmFilter,
            DateTime fromDate,
            DateTime toDate,
            int? goalsFilter,
            bool successOnly,
            string searchText)
        {
            if (experiments == null) return new List<ExperimentData>();

            var filtered = experiments.AsEnumerable();

            if (!string.IsNullOrEmpty(algorithmFilter) && algorithmFilter != "All Algorithms")
                filtered = filtered.Where(e => e.AlgorithmName == algorithmFilter);

            filtered = filtered.Where(e => e.Timestamp >= fromDate && e.Timestamp <= toDate);

            if (goalsFilter.HasValue)
                filtered = filtered.Where(e => e.GoalCount == goalsFilter.Value);

            if (successOnly)
                filtered = filtered.Where(e => e.Success);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(e =>
                    e.ExperimentId.ToLower().Contains(searchText) ||
                    e.ExperimentName?.ToLower().Contains(searchText) == true ||
                    e.MapName?.ToLower().Contains(searchText) == true ||
                    e.AlgorithmName.ToLower().Contains(searchText));
            }

            return filtered.OrderByDescending(e => e.Timestamp).ToList();
        }

        /// <summary>
        /// Calculates statistics for a list of experiments
        /// </summary>
        public ExperimentStatistics CalculateStatistics(List<ExperimentData> experiments)
        {
            var stats = new ExperimentStatistics();

            if (experiments == null || experiments.Count == 0)
                return stats;

            stats.TotalCount = experiments.Count;
            stats.SuccessCount = experiments.Count(e => e.Success);
            stats.SuccessRate = (double)stats.SuccessCount / stats.TotalCount * 100;
            stats.AvgSearchTimeMs = experiments.Average(e => e.SearchTimeMs);
            stats.AvgPathLength = experiments.Average(e => (double)e.PathLengthCells);
            stats.AvgCollisions = experiments.Average(e => (double)e.CollisionCount);
            stats.TotalCollisions = experiments.Sum(e => e.CollisionCount);

            var bestAlgorithm = experiments
                .Where(e => e.Success)
                .GroupBy(e => e.AlgorithmName)
                .Select(g => new { Algorithm = g.Key, AvgTime = g.Average(e => e.SearchTimeMs) })
                .OrderBy(a => a.AvgTime)
                .FirstOrDefault();

            stats.BestAlgorithm = bestAlgorithm?.Algorithm ?? "N/A";

            return stats;
        }
        #endregion
    }

    /// <summary>
    /// Statistics data for experiments
    /// </summary>
    public class ExperimentStatistics
    {
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public double SuccessRate { get; set; }
        public double AvgSearchTimeMs { get; set; }
        public double AvgPathLength { get; set; }
        public double AvgCollisions { get; set; }
        public int TotalCollisions { get; set; }
        public string BestAlgorithm { get; set; }
    }
}
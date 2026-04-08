#region File Header
/// <summary>
/// File: AlgorithmPerformanceData.cs
/// Description: Performance data for algorithm comparison
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.Core.Models.Algorithms
{
    #region Class Documentation
    /// <summary>
    /// Performance metrics for a single algorithm run
    /// </summary>
    #endregion
    public sealed class AlgorithmPerformanceData
    {
        public AlgorithmType Algorithm { get; set; }
        public DistanceMetric Metric { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
        public int PathLength { get; set; }
        public double ComputationTimeMs { get; set; }
        public int NodesExplored { get; set; }
        public double MemoryUsageMb { get; set; }
        public double BatteryConsumption { get; set; }
        public int ReplanCount { get; set; }
    }

    #region Class Documentation
    /// <summary>
    /// Aggregated performance statistics for an algorithm
    /// </summary>
    #endregion
    public sealed class AlgorithmStatistics
    {
        public AlgorithmType Algorithm { get; set; }
        public int TotalRuns { get; set; }
        public int SuccessfulRuns { get; set; }
        public double SuccessRate => TotalRuns > 0 ? (double)SuccessfulRuns / TotalRuns * 100 : 0;
        public double AverageTimeMs { get; set; }
        public double MinTimeMs { get; set; }
        public double MaxTimeMs { get; set; }
        public double AveragePathLength { get; set; }
        public int MinPathLength { get; set; }
        public int MaxPathLength { get; set; }
        public double AverageMemoryMb { get; set; }
        public double AverageBatteryConsumption { get; set; }
        public double AverageReplanCount { get; set; }
    }
}
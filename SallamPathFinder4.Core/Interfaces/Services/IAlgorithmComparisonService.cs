#region File Header
/// <summary>
/// File: IAlgorithmComparisonService.cs
/// Description: Interface for algorithm comparison service
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Path;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Result Classes
    public sealed class AlgorithmComparisonResult
    {
        public AlgorithmType Algorithm { get; set; }
        public bool Success { get; set; }
        public int PathLength { get; set; }
        public double ComputationTimeMs { get; set; }
        public int NodesExplored { get; set; }
        public double BatteryConsumption { get; set; }
        public string ErrorMessage { get; set; }
    }

    public sealed class ComparisonReport
    {
        public List<AlgorithmComparisonResult> Results { get; set; }
        public AlgorithmType FastestAlgorithm { get; set; }
        public AlgorithmType ShortestPathAlgorithm { get; set; }
        public AlgorithmType MostEfficientAlgorithm { get; set; }
        public double FastestTimeMs { get; set; }
        public int ShortestPathLength { get; set; }
        public double LowestBatteryConsumption { get; set; }
        public double AverageTimeMs { get; set; }
        public double AveragePathLength { get; set; }
        public double SuccessRate { get; set; }
    }
    #endregion

    #region Interface Documentation
    /// <summary>
    /// Service interface for comparing multiple pathfinding algorithms
    /// </summary>
    #endregion
    public interface IAlgorithmComparisonService
    {
        Task<ComparisonReport> CompareAlgorithmsAsync(
            Point start,
            Point end,
            List<AlgorithmType> algorithms,
            DistanceMetric metric);

        Task<ComparisonReport> CompareWithParametersAsync(
            Point start,
            Point end,
            List<AlgorithmType> algorithms,
            DistanceMetric metric,
            bool allowDiagonals,
            bool heavyDiagonals,
            int heuristicWeight,
            int searchLimit);

        Task<AlgorithmComparisonResult> RunSingleAlgorithmAsync(
            AlgorithmType algorithm,
            Point start,
            Point end,
            DistanceMetric metric);
    }
}
#region File Header
/// <summary>
/// File: AlgorithmComparisonService.cs
/// Description: Service for comparing multiple pathfinding algorithms
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SallamPathFinder4.Services.Pathfinding;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
#endregion

namespace SallamPathFinder4.Services.Pathfinding
{
    #region Class Documentation
    /// <summary>
    /// Service for comparing multiple pathfinding algorithms
    /// </summary>
    #endregion
    public sealed class AlgorithmComparisonService : IAlgorithmComparisonService
    {
        #region Private Fields
        private readonly MapGrid _grid;
        private readonly AlgorithmFactory _factory;
        #endregion

        #region Constructor
        public AlgorithmComparisonService(MapGrid grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _factory = new AlgorithmFactory(grid);
        }
        #endregion

        #region Public Methods
        public async Task<ComparisonReport> CompareAlgorithmsAsync(
            Point start, Point end, List<AlgorithmType> algorithms, DistanceMetric metric)
        {
            return await CompareWithParametersAsync(start, end, algorithms, metric, true, false, 2, 50000);
        }

        public async Task<ComparisonReport> CompareWithParametersAsync(
            Point start, Point end, List<AlgorithmType> algorithms, DistanceMetric metric,
            bool allowDiagonals, bool heavyDiagonals, int heuristicWeight, int searchLimit)
        {
            var results = new List<AlgorithmComparisonResult>();

            foreach (var algorithm in algorithms)
            {
                var result = await RunSingleAlgorithmAsync(algorithm, start, end, metric);
                results.Add(result);
            }

            var successful = results.Where(r => r.Success).ToList();

            return new ComparisonReport
            {
                Results = results,
                FastestAlgorithm = successful.OrderBy(r => r.ComputationTimeMs).FirstOrDefault()?.Algorithm ?? AlgorithmType.AStar,
                ShortestPathAlgorithm = successful.OrderBy(r => r.PathLength).FirstOrDefault()?.Algorithm ?? AlgorithmType.AStar,
                MostEfficientAlgorithm = successful.OrderBy(r => r.BatteryConsumption).FirstOrDefault()?.Algorithm ?? AlgorithmType.AStar,
                FastestTimeMs = successful.Any() ? successful.Min(r => r.ComputationTimeMs) : 0,
                ShortestPathLength = successful.Any() ? successful.Min(r => r.PathLength) : 0,
                LowestBatteryConsumption = successful.Any() ? successful.Min(r => r.BatteryConsumption) : 0,
                AverageTimeMs = successful.Any() ? successful.Average(r => r.ComputationTimeMs) : 0,
                AveragePathLength = successful.Any() ? successful.Average(r => r.PathLength) : 0,
                SuccessRate = results.Any() ? (double)successful.Count / results.Count * 100 : 0
            };
        }

        public async Task<AlgorithmComparisonResult> RunSingleAlgorithmAsync(
            AlgorithmType algorithm, Point start, Point end, DistanceMetric metric)
        {
            var finder = _factory.Create(algorithm);

            if (finder == null)
            {
                return new AlgorithmComparisonResult
                {
                    Algorithm = algorithm,
                    Success = false,
                    ErrorMessage = $"Algorithm {algorithm} not available"
                };
            }

            finder.Metric = metric;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = finder.FindPath(start, end);
            stopwatch.Stop();

            return await Task.Run(() => new AlgorithmComparisonResult
            {
                Algorithm = algorithm,
                Success = result.Success,
                PathLength = result.PathLength,
                ComputationTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                NodesExplored = result.NodesExplored,
                BatteryConsumption = result.PathLength * 0.5,
                ErrorMessage = result.Success ? null : "No path found"
            });
        }
        #endregion
    }
}
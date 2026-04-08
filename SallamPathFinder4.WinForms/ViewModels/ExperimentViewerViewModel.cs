#region Namespace Imports
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Experiments;
#endregion

namespace SallamPathFinder4.WinForms.ViewModels
{
    public sealed class ExperimentViewerViewModel : INotifyPropertyChanged
    {
        #region Private Fields
        private readonly IExperimentService _experimentService;
        private BindingList<ExperimentData> _filteredExperiments;
        private ExperimentStatistics _statistics;
        #endregion

        #region Constructor
        public ExperimentViewerViewModel(IExperimentService service)
        {
            _experimentService = service;
            _filteredExperiments = new BindingList<ExperimentData>();
            _statistics = new ExperimentStatistics();
            FilteredExperiments = _filteredExperiments;
        }
        #endregion

        #region Properties
        public BindingList<ExperimentData> FilteredExperiments { get; }

        public ExperimentStatistics Statistics
        {
            get => _statistics;
            private set
            {
                _statistics = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Public Methods
        public void LoadExperiments(string algorithmFilter, DateTime from, DateTime to, int? goalsFilter, bool successOnly)
        {
            var all = _experimentService.GetAllExperiments();

            var filtered = all.Where(e =>
                (string.IsNullOrEmpty(algorithmFilter) || e.AlgorithmName == algorithmFilter) &&
                e.Timestamp >= from && e.Timestamp <= to &&
                (goalsFilter == null || e.GoalCount == goalsFilter) &&
                (!successOnly || e.Success)
            ).OrderByDescending(e => e.Timestamp).ToList();

            _filteredExperiments.Clear();
            foreach (var exp in filtered)
            {
                _filteredExperiments.Add(exp);
            }

            CalculateStatistics(filtered);
        }

        public List<string> GetAvailableAlgorithms()
        {
            var all = _experimentService.GetAllExperiments();
            return all.Select(e => e.AlgorithmName).Distinct().OrderBy(a => a).ToList();
        }
        #endregion

        #region Private Methods
        private void CalculateStatistics(List<ExperimentData> experiments)
        {
            if (!experiments.Any())
            {
                Statistics = new ExperimentStatistics();
                return;
            }

            var byAlgorithm = experiments.GroupBy(e => e.AlgorithmName);
            var bestAlgorithm = byAlgorithm
                .Select(g => new { Algorithm = g.Key, AvgTime = g.Average(e => e.SearchTimeMs) })
                .OrderBy(x => x.AvgTime)
                .FirstOrDefault();

            Statistics = new ExperimentStatistics
            {
                TotalCount = experiments.Count,
                SuccessRate = experiments.Average(e => e.Success ? 1 : 0) * 100,
                AvgPathLength = experiments.Average(e => e.PathLengthCells),
                AvgSearchTime = experiments.Average(e => e.SearchTimeMs),
                AvgStaticObstacles = experiments.Average(e => e.StaticObstaclesPercentage),
                AvgSemiStaticObstacles = experiments.Average(e => e.SemiStaticObstaclesPercentage),
                AvgDynamicObstacles = experiments.Average(e => e.DynamicObstaclesCount),
                BestAlgorithm = bestAlgorithm?.Algorithm ?? "N/A"
            };
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class ExperimentStatistics
    {
        public int TotalCount { get; set; }
        public double SuccessRate { get; set; }
        public double AvgPathLength { get; set; }
        public double AvgSearchTime { get; set; }
        public double AvgStaticObstacles { get; set; }
        public double AvgSemiStaticObstacles { get; set; }
        public double AvgDynamicObstacles { get; set; }
        public string BestAlgorithm { get; set; }
    }
}
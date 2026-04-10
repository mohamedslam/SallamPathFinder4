#region File Header
/// <summary>
/// File: frmStatisticsViewer.cs
/// Description: Advanced statistics viewer for experiment results with charts
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.WinForms.Forms.Experiments.frmStatisticsViewer.Core;
using SallamPathFinder4.WinForms.Models;
using System.Windows.Forms.DataVisualization.Charting;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmStatisticsViewer
{
    /// <summary>
    /// Form for viewing advanced statistics and charts from experiment results
    /// </summary>
    public sealed partial class frmStatisticsViewer : Form
    {
        #region Constants
        private const int FORM_WIDTH = 1100;
        private const int FORM_HEIGHT = 750;
        #endregion

        #region Private Fields
        private readonly List<ExperimentResultItem> _results;
        private readonly StatisticsViewerLogic _logic;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the statistics viewer form
        /// </summary>
        /// <param name="results">List of experiment results to analyze</param>
        public frmStatisticsViewer(List<ExperimentResultItem> results)
        {
            _results = results ?? new List<ExperimentResultItem>();
            _logic = new StatisticsViewerLogic();

            InitializeComponent();
            WireEvents();
            LoadStatistics();
        }
        #endregion

        #region Private Methods - Initialization
        /// <summary>
        /// Wires up all event handlers
        /// </summary>
        private void WireEvents()
        {
            _btnExportCharts.Click += (s, e) => ExportCharts();
            _btnExportData.Click += (s, e) => ExportData();
            _btnClose.Click += (s, e) => Close();
        }

        /// <summary>
        /// Loads all statistics and charts
        /// </summary>
        private void LoadStatistics()
        {
            if (_results == null || _results.Count == 0)
            {
                _lblSummary.Text = "No data available.";
                return;
            }

            LoadAlgorithmStats();
            LoadMetricStats();
            LoadCollisionStats();
            LoadSummary();
            LoadCharts();
            LoadTrends();
        }
        #endregion

        #region Private Methods - Data Loading
        /// <summary>
        /// Loads algorithm performance statistics
        /// </summary>
        private void LoadAlgorithmStats()
        {
            var algorithms = _results.Select(r => r.Algorithm).Distinct().OrderBy(a => a).ToList();

            foreach (var algo in algorithms)
            {
                var stats = _logic.CalculateAlgorithmStats(_results, algo);

                _dgvAlgorithmStats.Rows.Add(
                    algo,
                    stats.TotalRuns,
                    stats.SuccessCount,
                    stats.SuccessRate.ToString("F1"),
                    stats.AvgTimeMs.ToString("F2"),
                    stats.MinTimeMs.ToString("F2"),
                    stats.MaxTimeMs.ToString("F2"),
                    stats.StdDevTimeMs.ToString("F2"),
                    stats.AvgLength.ToString("F0"),
                    stats.MinLength,
                    stats.MaxLength,
                    stats.AvgBattery.ToString("F1"),
                    stats.AvgCollisions.ToString("F1"),
                    stats.AvgErrors.ToString("F1"),
                    stats.AvgSpeed.ToString("F1")
                );
            }
        }

        /// <summary>
        /// Loads metric-based statistics
        /// </summary>
        private void LoadMetricStats()
        {
            var metrics = _results.GroupBy(r => r.Metric);

            foreach (var group in metrics)
            {
                var stats = _logic.CalculateMetricStats(group.ToList());

                _dgvMetricStats.Rows.Add(
                    group.Key,
                    stats.TotalRuns,
                    stats.SuccessRate.ToString("F1"),
                    stats.AvgTimeMs.ToString("F2"),
                    stats.AvgLength.ToString("F0"),
                    stats.AvgBattery.ToString("F1"),
                    stats.AvgCollisions.ToString("F1")
                );
            }
        }

        /// <summary>
        /// Loads collision and error statistics
        /// </summary>
        private void LoadCollisionStats()
        {
            var algorithms = _results.Select(r => r.Algorithm).Distinct().OrderBy(a => a).ToList();

            foreach (var algo in algorithms)
            {
                var stats = _logic.CalculateCollisionStats(_results, algo);

                _dgvCollisionStats.Rows.Add(
                    algo,
                    stats.TotalCollisions,
                    stats.AvgCollisions.ToString("F1"),
                    stats.MaxCollisions,
                    stats.TotalErrors,
                    stats.AvgErrors.ToString("F1"),
                    stats.FailureRate.ToString("F1")
                );
            }
        }

        /// <summary>
        /// Loads summary statistics
        /// </summary>
        private void LoadSummary()
        {
            var summary = _logic.CalculateSummaryStats(_results);

            _lblSummary.Text =
                $"📊 EXPERIMENT SUMMARY\n\n" +
                $"Total Experiments: {summary.TotalExperiments}\n" +
                $"Successful: {summary.SuccessCount} ({summary.SuccessRate:F1}%)\n" +
                $"Failed: {summary.TotalExperiments - summary.SuccessCount}\n\n" +
                $"📈 AVERAGE METRICS\n" +
                $"├─ Computation Time: {summary.AvgTimeMs:F2} ms\n" +
                $"├─ Path Length: {summary.AvgLength:F0} cells\n" +
                $"├─ Battery Remaining: {summary.AvgBattery:F1}%\n" +
                $"├─ Collisions: {summary.AvgCollisions:F1}\n" +
                $"└─ Average Speed: {summary.AvgSpeed:F1} cm/s\n\n" +
                $"🏆 BEST PERFORMERS\n" +
                $"├─ Fastest: {summary.FastestAlgorithm} ({summary.FastestTimeMs:F2} ms)\n" +
                $"├─ Shortest Path: {summary.ShortestPathAlgorithm} ({summary.ShortestPathLength} cells)\n" +
                $"└─ Safest: {summary.SafestAlgorithm} ({summary.SafestCollisions} collisions)";
        }

        /// <summary>
        /// Loads all charts
        /// </summary>
        private void LoadCharts()
        {
            LoadTimeChart();
            LoadSuccessRateChart();
            LoadCollisionsChart();
            LoadSpeedChart();
        }

        /// <summary>
        /// Loads time comparison chart
        /// </summary>
        private void LoadTimeChart()
        {
            var algorithms = _results.Select(r => r.Algorithm).Distinct().ToList();
            var timeSeries = new Series("Computation Time (ms)")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true
            };

            foreach (var algo in algorithms)
            {
                var avg = _logic.GetAverageTimeForAlgorithm(_results, algo);
                timeSeries.Points.AddXY(algo, avg);
            }

            _chartTime.Series.Clear();
            _chartTime.Series.Add(timeSeries);
            _chartTime.Titles[0].Text = "Average Computation Time by Algorithm";
        }

        /// <summary>
        /// Loads success rate chart
        /// </summary>
        private void LoadSuccessRateChart()
        {
            var algorithms = _results.Select(r => r.Algorithm).Distinct().ToList();
            var successSeries = new Series("Success Rate (%)")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true
            };

            foreach (var algo in algorithms)
            {
                var rate = _logic.GetSuccessRateForAlgorithm(_results, algo);
                successSeries.Points.AddXY(algo, rate);
            }

            _chartSuccess.Series.Clear();
            _chartSuccess.Series.Add(successSeries);
            _chartSuccess.Titles[0].Text = "Success Rate by Algorithm";
        }

        /// <summary>
        /// Loads collisions chart
        /// </summary>
        private void LoadCollisionsChart()
        {
            var algorithms = _results.Select(r => r.Algorithm).Distinct().ToList();
            var collisionSeries = new Series("Average Collisions")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true
            };

            foreach (var algo in algorithms)
            {
                var avg = _logic.GetAverageCollisionsForAlgorithm(_results, algo);
                collisionSeries.Points.AddXY(algo, avg);
            }

            _chartCollisions.Series.Clear();
            _chartCollisions.Series.Add(collisionSeries);
            _chartCollisions.Titles[0].Text = "Average Collisions by Algorithm";
        }

        /// <summary>
        /// Loads speed chart
        /// </summary>
        private void LoadSpeedChart()
        {
            var algorithms = _results.Select(r => r.Algorithm).Distinct().ToList();
            var speedSeries = new Series("Average Speed (cm/s)")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true
            };

            foreach (var algo in algorithms)
            {
                var avg = _logic.GetAverageSpeedForAlgorithm(_results, algo);
                speedSeries.Points.AddXY(algo, avg);
            }

            _chartSpeed.Series.Clear();
            _chartSpeed.Series.Add(speedSeries);
            _chartSpeed.Titles[0].Text = "Average Speed by Algorithm";
        }

        /// <summary>
        /// Loads performance trends chart
        /// </summary>
        private void LoadTrends()
        {
            var grouped = _results.GroupBy(r => r.Iteration).OrderBy(g => g.Key);

            _chartTrend.Series["Time"].Points.Clear();
            _chartTrend.Series["Length"].Points.Clear();

            foreach (var group in grouped)
            {
                var avgTime = group.Where(r => r.Success).Select(r => r.ComputationTimeMs).DefaultIfEmpty(0).Average();
                var avgLength = group.Where(r => r.Success).Select(r => (double)r.PathLength).DefaultIfEmpty(0).Average();

                _chartTrend.Series["Time"].Points.AddXY(group.Key, avgTime);
                _chartTrend.Series["Length"].Points.AddXY(group.Key, avgLength);
            }

            _chartTrend.Series["Time"].ToolTip = "Computation Time (ms)";
            _chartTrend.Series["Length"].ToolTip = "Path Length (cells)";
            _chartTrend.Titles[0].Text = "Performance Trends by Iteration";
        }
        #endregion

        #region Private Methods - Export
        /// <summary>
        /// Exports charts as images
        /// </summary>
        private void ExportCharts()
        {
            using var fbd = new FolderBrowserDialog();
            fbd.Description = "Select folder to save charts";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                _logic.SaveChartAsImage(_chartTime, Path.Combine(fbd.SelectedPath, "Chart_Time.png"));
                _logic.SaveChartAsImage(_chartSuccess, Path.Combine(fbd.SelectedPath, "Chart_Success.png"));
                _logic.SaveChartAsImage(_chartCollisions, Path.Combine(fbd.SelectedPath, "Chart_Collisions.png"));
                _logic.SaveChartAsImage(_chartSpeed, Path.Combine(fbd.SelectedPath, "Chart_Speed.png"));
                _logic.SaveChartAsImage(_chartTrend, Path.Combine(fbd.SelectedPath, "Chart_Trend.png"));

                MessageBox.Show($"Charts saved to:\n{fbd.SelectedPath}", "Export Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Exports statistics data to CSV
        /// </summary>
        private void ExportData()
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.FileName = $"Statistics_Data_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _logic.ExportStatisticsToCsv(_results, sfd.FileName);
                    MessageBox.Show($"Data exported to:\n{sfd.FileName}", "Export Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}
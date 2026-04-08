#region File Header
/// <summary>
/// File: frmExperimentResults.cs
/// Description: Form to display experiment results with advanced features
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentResults.Core;
using SallamPathFinder4.WinForms.Models;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentResults
{
    /// <summary>
    /// Form for displaying and analyzing experiment results
    /// </summary>
    public sealed partial class frmExperimentResults : Form
    {
        #region Constants
        private const int FORM_WIDTH = 1300;
        private const int FORM_HEIGHT = 750;
        private const int SCREENSHOT_SIZE = 100;
        #endregion

        #region Private Fields
        private readonly List<ExperimentResultItem> _allResults;
        private readonly string _resultsFolderPath;
        private ExperimentResultItem _selectedResult;
        private readonly ExperimentResultsLogic _logic;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the experiment results form
        /// </summary>
        /// <param name="results">List of experiment results to display</param>
        /// <param name="folderPath">Path to the folder containing screenshots</param>
        public frmExperimentResults(List<ExperimentResultItem> results, string folderPath)
        {
            _allResults = results ?? new List<ExperimentResultItem>();
            _resultsFolderPath = folderPath ?? string.Empty;
            _logic = new ExperimentResultsLogic();

            InitializeComponent();
            WireEvents();
            InitializeFilters();
            ApplyFilters();
        }
        #endregion

        #region Private Methods - Initialization
        /// <summary>
        /// Wires up all event handlers
        /// </summary>
        private void WireEvents()
        {
            _cboAlgorithmFilter.SelectedIndexChanged += (s, e) => ApplyFilters();
            _cboMetricFilter.SelectedIndexChanged += (s, e) => ApplyFilters();
            _cboResultFilter.SelectedIndexChanged += (s, e) => ApplyFilters();
            _txtSearch.TextChanged += (s, e) => ApplyFilters();
            _btnClearFilters.Click += (s, e) => ClearFilters();
            _btnExportCSV.Click += (s, e) => ExportToCSV();
            _btnExportExcel.Click += (s, e) => ExportToExcel();
            _btnExportPDF.Click += (s, e) => ExportToPDF();
            _btnSaveReplay.Click += (s, e) => SaveReplay();
            _btnPlayReplay.Click += (s, e) => PlayReplay();
            _btnAdvancedStats.Click += (s, e) => ShowAdvancedStatistics();
            _btnNewExperiment.Click += (s, e) => { this.DialogResult = DialogResult.Retry; this.Close(); };
            _btnClose.Click += (s, e) => Close();
            _dgvResults.CellClick += DgvResults_CellClick;
            _dgvResults.CellFormatting += DgvResults_CellFormatting;
        }

        /// <summary>
        /// Initializes filter dropdowns with available values
        /// </summary>
        private void InitializeFilters()
        {
            // Algorithm filter
            var algorithms = _allResults.Select(r => r.Algorithm).Distinct().OrderBy(a => a).ToList();
            foreach (var algo in algorithms)
            {
                _cboAlgorithmFilter.Items.Add(algo);
            }
            _cboAlgorithmFilter.SelectedIndex = 0;

            // Metric filter
            var metrics = _allResults.Select(r => r.Metric).Distinct().OrderBy(m => m).ToList();
            foreach (var metric in metrics)
            {
                _cboMetricFilter.Items.Add(metric);
            }
            _cboMetricFilter.SelectedIndex = 0;
        }

        /// <summary>
        /// Applies all active filters to the results
        /// </summary>
        private void ApplyFilters()
        {
            var filtered = _allResults.AsEnumerable();

            string algorithmFilter = _cboAlgorithmFilter.SelectedItem?.ToString();
            if (algorithmFilter != null && algorithmFilter != "All Algorithms")
                filtered = filtered.Where(r => r.Algorithm == algorithmFilter);

            string metricFilter = _cboMetricFilter.SelectedItem?.ToString();
            if (metricFilter != null && metricFilter != "All Metrics")
                filtered = filtered.Where(r => r.Metric == metricFilter);

            string resultFilter = _cboResultFilter.SelectedItem?.ToString();
            if (resultFilter == "Success Only")
                filtered = filtered.Where(r => r.Success);
            else if (resultFilter == "Failure Only")
                filtered = filtered.Where(r => !r.Success);

            string searchText = _txtSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(r =>
                    r.Algorithm.ToLower().Contains(searchText) ||
                    r.Metric.ToLower().Contains(searchText) ||
                    r.Iteration.ToString().Contains(searchText));
            }

            DisplayResults(filtered.ToList());
            UpdateStatistics(filtered.ToList());
        }

        /// <summary>
        /// Displays results in the data grid view
        /// </summary>
        private void DisplayResults(List<ExperimentResultItem> results)
        {
            _dgvResults.Rows.Clear();
            int id = 1;

            foreach (var r in results)
            {
                if (r == null) continue;

                bool hasScreenshot = !string.IsNullOrEmpty(r.ScreenshotPath) && File.Exists(r.ScreenshotPath);
                string avgSpeed = r.AverageActualSpeed > 0 ? $"{r.AverageActualSpeed:F1}" : "N/A";

                _dgvResults.Rows.Add(
                    id++,
                    r.Algorithm ?? "Unknown",
                    r.Metric ?? "Unknown",
                    r.Iteration,
                    r.PathLength,
                    r.ComputationTimeMs.ToString("F2"),
                    r.RemainingBattery.ToString("F1"),
                    r.CollisionCount,
                    r.InvalidMoveCount,
                    avgSpeed,
                    r.Success ? "✓" : "✗",
                    hasScreenshot ? "📷" : ""
                );

                var row = _dgvResults.Rows[_dgvResults.Rows.Count - 1];
                if (!r.Success)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230);
                else
                    row.DefaultCellStyle.BackColor = Color.FromArgb(230, 255, 230);
            }
        }

        /// <summary>
        /// Updates the statistics display
        /// </summary>
        private void UpdateStatistics(List<ExperimentResultItem> results)
        {
            int total = results.Count;
            int successCount = results.Count(r => r.Success);
            double successRate = total > 0 ? (double)successCount / total * 100 : 0;
            double avgTime = results.Any() ? results.Average(r => r.ComputationTimeMs) : 0;
            double avgLength = results.Any() ? results.Average(r => (double)r.PathLength) : 0;
            double avgBattery = results.Any() ? results.Average(r => r.RemainingBattery) : 0;
            double avgCollisions = results.Any() ? results.Average(r => r.CollisionCount) : 0;

            var bestAlgorithm = results
                .Where(r => r.Success)
                .GroupBy(r => r.Algorithm)
                .Select(g => new { Algorithm = g.Key, AvgTime = g.Average(r => r.ComputationTimeMs) })
                .OrderBy(a => a.AvgTime)
                .FirstOrDefault();

            _lblStatistics.Text = $"📊 Total: {total} | ✅ Success: {successCount} ({successRate:F1}%) | " +
                                 $"⏱️ Avg Time: {avgTime:F2} ms | 📏 Avg Length: {avgLength:F0} | " +
                                 $"🔋 Battery: {avgBattery:F1}% | 💥 Collisions: {avgCollisions:F1} | " +
                                 $"🏆 Best: {(bestAlgorithm?.Algorithm ?? "N/A")}";
        }

        /// <summary>
        /// Clears all filters and refreshes the display
        /// </summary>
        private void ClearFilters()
        {
            _cboAlgorithmFilter.SelectedIndex = 0;
            _cboMetricFilter.SelectedIndex = 0;
            _cboResultFilter.SelectedIndex = 0;
            _txtSearch.Text = "";
            ApplyFilters();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles cell click to load screenshots for selected result
        /// </summary>
        private void DgvResults_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string algorithm = _dgvResults.Rows[e.RowIndex].Cells["Algorithm"].Value?.ToString();
            string metric = _dgvResults.Rows[e.RowIndex].Cells["Metric"].Value?.ToString();
            int iteration = int.Parse(_dgvResults.Rows[e.RowIndex].Cells["Iteration"].Value?.ToString() ?? "0");

            _selectedResult = _allResults.FirstOrDefault(r =>
                r.Algorithm == algorithm &&
                r.Metric == metric &&
                r.Iteration == iteration);

            if (_selectedResult != null)
            {
                LoadScreenshots();
                UpdateDetailsPanel();
            }
        }

        /// <summary>
        /// Handles cell formatting for success column
        /// </summary>
        private void DgvResults_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var successColumn = _dgvResults.Columns["Success"];
            if (successColumn == null || e.ColumnIndex != successColumn.Index) return;

            if (e.ColumnIndex == _dgvResults.Columns["Success"].Index)
            {
                e.CellStyle.ForeColor = e.Value?.ToString() == "✓" ? Color.Green : Color.Red;
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }

        /// <summary>
        /// Loads screenshots for the selected result
        /// </summary>
        private void LoadScreenshots()
        {
            string basePath = Path.Combine(_resultsFolderPath, "Screenshots", _selectedResult.Algorithm);

            string initialPath = Path.Combine(basePath,
                $"{_selectedResult.Algorithm}_{_selectedResult.Metric}_Initial_Iter{_selectedResult.Iteration}.png");

            string pathPath = Path.Combine(basePath,
                $"{_selectedResult.Algorithm}_{_selectedResult.Metric}_Path_Iter{_selectedResult.Iteration}.png");

            string completedPath = Path.Combine(basePath,
                $"{_selectedResult.Algorithm}_{_selectedResult.Metric}_Completed_Iter{_selectedResult.Iteration}.png");

            if (File.Exists(initialPath))
                _picInitial.Image = System.Drawing.Image.FromFile(initialPath);

            if (File.Exists(pathPath))
                _picPath.Image = System.Drawing.Image.FromFile(pathPath);

            if (File.Exists(completedPath))
                _picCompleted.Image = System.Drawing.Image.FromFile(completedPath);
        }

        /// <summary>
        /// Updates the details panel with selected result information
        /// </summary>
        private void UpdateDetailsPanel()
        {
            if (_selectedResult == null) return;

            _lblDetails.Text =
                $"📋 DETAILS\n\n" +
                $"Algorithm: {_selectedResult.Algorithm}\n" +
                $"Metric: {_selectedResult.Metric}\n" +
                $"Iteration: {_selectedResult.Iteration}\n" +
                $"Success: {(_selectedResult.Success ? "✓ Yes" : "✗ No")}\n" +
                $"Path Length: {_selectedResult.PathLength} cells\n" +
                $"Time: {_selectedResult.ComputationTimeMs:F2} ms\n" +
                $"Battery: {_selectedResult.RemainingBattery:F1}%\n" +
                $"Collisions: {_selectedResult.CollisionCount}\n" +
                $"Errors: {_selectedResult.InvalidMoveCount}\n" +
                $"Avg Speed: {(_selectedResult.AverageActualSpeed > 0 ? $"{_selectedResult.AverageActualSpeed:F1} cm/s" : "N/A")}";
        }
        #endregion

        #region Export Methods
        /// <summary>
        /// Exports results to CSV file
        /// </summary>
        private void ExportToCSV()
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.FileName = $"Experiment_Results_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _logic.ExportToCsv(_allResults, sfd.FileName);
                    MessageBox.Show($"Exported to:\n{sfd.FileName}", "Export Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Exports results to Excel (CSV format)
        /// </summary>
        private void ExportToExcel() => ExportToCSV();

        /// <summary>
        /// Exports results to HTML for PDF printing
        /// </summary>
        private void ExportToPDF()
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "HTML files (*.html)|*.html";
            sfd.FileName = $"Experiment_Results_{DateTime.Now:yyyyMMdd_HHmmss}.html";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _logic.ExportToHtml(_allResults, sfd.FileName);
                    MessageBox.Show($"Exported to:\n{sfd.FileName}", "Export Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Saves replay for selected result
        /// </summary>
        private void SaveReplay()
        {
            if (_selectedResult == null)
            {
                MessageBox.Show("Please select a result first.", "Save Replay",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "Replay files (*.sreplay)|*.sreplay";
            sfd.FileName = $"Replay_{_selectedResult.Algorithm}_{_selectedResult.Iteration}.sreplay";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show($"Replay saved to:\n{sfd.FileName}", "Save Replay",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Plays a saved replay
        /// </summary>
        private void PlayReplay()
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Replay files (*.sreplay)|*.sreplay";
            ofd.Title = "Select Replay File";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show($"Loading replay from:\n{ofd.FileName}", "Play Replay",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Shows advanced statistics viewer
        /// </summary>
        private void ShowAdvancedStatistics()
        {
            var statsViewer = new frmStatisticsViewer.frmStatisticsViewer(_allResults);
            statsViewer.ShowDialog();
        }

        private void PicInitial_DoubleClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedResult?.InitialScreenshotPath) && File.Exists(_selectedResult.InitialScreenshotPath))
            {
                var viewer = new Experiments.frmScreenshotViewer.frmScreenshotViewer(_selectedResult, _resultsFolderPath);
                viewer.ShowDialog();
            }
            else
            {
                MessageBox.Show("No initial screenshot available.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void PicPath_DoubleClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedResult?.PathScreenshotPath) && File.Exists(_selectedResult.PathScreenshotPath))
            {
                var viewer = new Experiments.frmScreenshotViewer.frmScreenshotViewer(_selectedResult, _resultsFolderPath);
                viewer.ShowDialog();
            }
            else
            {
                MessageBox.Show("No path screenshot available.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void PicCompleted_DoubleClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedResult?.CompletedScreenshotPath) && File.Exists(_selectedResult.CompletedScreenshotPath))
            {
                var viewer = new Experiments.frmScreenshotViewer.frmScreenshotViewer(_selectedResult, _resultsFolderPath);
                viewer.ShowDialog();
            }
            else
            {
                MessageBox.Show("No completed screenshot available.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion
    }
}
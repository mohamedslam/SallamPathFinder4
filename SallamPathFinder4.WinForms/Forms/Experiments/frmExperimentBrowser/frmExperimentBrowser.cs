#region File Header
/// <summary>
/// File: frmExperimentBrowser.cs
/// Description: Browse, compare, and manage saved experiments
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-09
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SallamPathFinder4.Core.Models.Experiments;
using SallamPathFinder4.WinForms.Models;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentBrowser
{
    #region Class Documentation
    /// <summary>
    /// Form for browsing, comparing, and managing saved experiments
    /// </summary>
    #endregion
    public sealed partial class frmExperimentBrowser : Form
    {
        #region Constants
        private const string EXPERIMENTS_SUBFOLDER = "Experiments";
        #endregion

        #region Private Fields
        private string _experimentsPath;
        private List<ExperimentData> _loadedExperiments;
        private ExperimentBrowserLogic _logic;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the experiment browser form
        /// </summary>
        public frmExperimentBrowser()
        {
            _logic = new ExperimentBrowserLogic();
            InitializeComponent();
            LoadExperimentsPath();
            LoadExperimentFolders();
            WireEvents();
        }
        #endregion

        #region Private Methods - Initialization
        /// <summary>
        /// Loads the experiments path from logic
        /// </summary>
        private void LoadExperimentsPath()
        {
            _experimentsPath = _logic.GetExperimentsPath();

            if (!Directory.Exists(_experimentsPath))
            {
                Directory.CreateDirectory(_experimentsPath);
            }
        }

        /// <summary>
        /// Wires up all event handlers
        /// </summary>
        private void WireEvents()
        {
            this.btnSelectAll.Click += BtnSelectAll_Click;
            this.btnClearAll.Click += BtnClearAll_Click;
            this.btnCompareSelected.Click += BtnCompareSelected_Click;
            this.btnDeleteSelected.Click += BtnDeleteSelected_Click;
            this.btnRefresh.Click += BtnRefresh_Click;
            this.btnExportCsv.Click += BtnExportCsv_Click;
            this.btnViewDetails.Click += BtnViewDetails_Click;
            this.btnClose.Click += (s, e) => this.Close();
            this.checkedListFolders.ItemCheck += CheckedListFolders_ItemCheck;
            this.dgvResults.CellDoubleClick += DgvResults_CellDoubleClick;
        }

        /// <summary>
        /// Loads experiment folders from the experiments directory
        /// </summary>
        private void LoadExperimentFolders()
        {
            if (checkedListFolders == null)
            {
                return;
            }

            var directories = Directory.GetDirectories(_experimentsPath)
                .OrderByDescending(d => Directory.GetCreationTime(d))
                .ToList();

            System.Diagnostics.Debug.WriteLine($"Found {directories.Count} directories");

            checkedListFolders.Items.Clear();
            _loadedExperiments = new List<ExperimentData>();

            foreach (var dir in directories)
            {
                string folderName = Path.GetFileName(dir);
                string displayName = $"📁 {folderName}";
                var experiment = _logic.LoadExperimentFromFolder(dir);

                System.Diagnostics.Debug.WriteLine($"Loading {folderName}: experiment = {(experiment != null ? experiment.AlgorithmName : "NULL")}");

                _loadedExperiments.Add(experiment);
                checkedListFolders.Items.Add(displayName, false);
            }

            UpdateStatusLabel($"{directories.Count} experiment(s) found");
        }

        /// <summary>
        /// Updates the status label text
        /// </summary>
        private void UpdateStatusLabel(string message)
        {
            if (lblStats != null)
            {
                lblStats.Text = message;
            }
        }
        #endregion

        #region Private Methods - Display Results
        /// <summary>
        /// Displays comparison results in the data grid view
        /// </summary>
        private void DisplayComparisonResults(List<ExperimentData> experiments)
        {
            dgvResults.Rows.Clear();

            foreach (var exp in experiments)
            {
                if (exp == null)
                {
                    continue;
                }

                dgvResults.Rows.Add(
                    exp.AlgorithmName ?? "Unknown",
                    exp.SearchTimeMs.ToString("F2"),
                    exp.PathLengthCells,
                    exp.Success ? "✓" : "✗",
                    exp.CollisionCount,
                    exp.Timestamp.ToString("yyyy-MM-dd HH:mm")
                );

                var row = dgvResults.Rows[dgvResults.Rows.Count - 1];

                if (!exp.Success)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230);
                }
            }

            UpdateStatistics(experiments);
        }

        /// <summary>
        /// Updates the statistics display
        /// </summary>
        private void UpdateStatistics(List<ExperimentData> experiments)
        {
            var stats = _logic.CalculateStatistics(experiments);
            lblStats.Text = stats;
        }
        #endregion

        #region Private Methods - Get Selected Items
        /// <summary>
        /// Gets the selected experiments
        /// </summary>
        private List<ExperimentData> GetSelectedExperiments()
        {
            var selected = new List<ExperimentData>();

            System.Diagnostics.Debug.WriteLine($"GetSelectedExperiments: _loadedExperiments count = {_loadedExperiments?.Count ?? 0}");

            for (int i = 0; i < checkedListFolders.Items.Count; i++)
            {
                if (checkedListFolders.GetItemChecked(i))
                {
                    System.Diagnostics.Debug.WriteLine($"Item {i} is checked");

                    if (i < _loadedExperiments.Count && _loadedExperiments[i] != null)
                    {
                        selected.Add(_loadedExperiments[i]);
                        System.Diagnostics.Debug.WriteLine($"Added experiment: {_loadedExperiments[i].AlgorithmName}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"No experiment data for index {i}");
                    }
                }
            }

            return selected;
        }

        /// <summary>
        /// Gets the selected indices
        /// </summary>
        private List<int> GetSelectedIndices()
        {
            var indices = new List<int>();

            for (int i = 0; i < checkedListFolders.Items.Count; i++)
            {
                if (checkedListFolders.GetItemChecked(i))
                {
                    indices.Add(i);
                }
            }

            return indices;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles select all button click
        /// </summary>
        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListFolders.Items.Count; i++)
            {
                checkedListFolders.SetItemChecked(i, true);
            }
        }

        /// <summary>
        /// Handles clear all button click
        /// </summary>
        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListFolders.Items.Count; i++)
            {
                checkedListFolders.SetItemChecked(i, false);
            }

            dgvResults.Rows.Clear();
            UpdateStatusLabel("Selection cleared");
        }

        /// <summary>
        /// Handles compare selected button click
        /// </summary>
        private void BtnCompareSelected_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedExperiments();

            if (selected.Count == 0)
            {
                MessageBox.Show("Please select at least one experiment to compare.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DisplayComparisonResults(selected);
        }

        /// <summary>
        /// Handles delete selected button click
        /// </summary>
        private async void BtnDeleteSelected_Click(object sender, EventArgs e)
        {
            var selectedIndices = GetSelectedIndices();

            if (selectedIndices.Count == 0)
            {
                MessageBox.Show("Please select experiments to delete.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Delete {selectedIndices.Count} experiment(s)?\n\nThis action cannot be undone.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                int deletedCount = await _logic.DeleteExperimentsAsync(_experimentsPath, selectedIndices, checkedListFolders);

                LoadExperimentFolders();
                dgvResults.Rows.Clear();
                UpdateStatusLabel($"Deleted {deletedCount} experiment(s)");
            }
        }

        /// <summary>
        /// Handles refresh button click
        /// </summary>
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadExperimentFolders();
            dgvResults.Rows.Clear();
            UpdateStatusLabel("Refreshed");
        }

        /// <summary>
        /// Handles export CSV button click
        /// </summary>
        private void BtnExportCsv_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedExperiments();

            if (selected.Count == 0)
            {
                MessageBox.Show("Please select experiments to export.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.FileName = $"Experiments_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _logic.ExportToCsv(selected, sfd.FileName);
                MessageBox.Show($"Exported to:\n{sfd.FileName}", "Export Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Handles view details button click
        /// </summary>
        private void BtnViewDetails_Click(object sender, EventArgs e)
        {
            if (dgvResults.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a result to view details.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int rowIndex = dgvResults.SelectedRows[0].Index;
            string algorithm = dgvResults.Rows[rowIndex].Cells[1].Value?.ToString();
            string time = dgvResults.Rows[rowIndex].Cells[3].Value?.ToString();
            string length = dgvResults.Rows[rowIndex].Cells[4].Value?.ToString();

            MessageBox.Show($"Algorithm: {algorithm}\nTime: {time} ms\nPath Length: {length} cells",
                "Experiment Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Handles item check event in the checked list box
        /// </summary>
        private void CheckedListFolders_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                System.Diagnostics.Debug.WriteLine("=== ItemCheck called ===");

                var selected = GetSelectedExperiments();

                System.Diagnostics.Debug.WriteLine($"Selected count: {selected.Count}");

                if (selected.Count > 0)
                {
                    foreach (var exp in selected)
                    {
                        System.Diagnostics.Debug.WriteLine($"Selected: {exp.AlgorithmName} - {exp.SearchTimeMs}ms");
                    }

                    DisplayComparisonResults(selected);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No experiments selected");
                    dgvResults.Rows.Clear();
                    UpdateStatusLabel("No experiments selected");
                }
            }));
        }

        /// <summary>
        /// Handles double-click on result row
        /// </summary>
        private void DgvResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            var selected = GetSelectedExperiments();

            if (e.RowIndex < selected.Count)
            {
                var experiment = selected[e.RowIndex];
                ShowExperimentDetails(experiment);
            }
        }

        /// <summary>
        /// Shows detailed experiment information
        /// </summary>
        private void ShowExperimentDetails(ExperimentData experiment)
        {
            try
            {
                string folderPath = Path.Combine(_experimentsPath,
                    $"{experiment.ExperimentId}_{experiment.Timestamp:yyyyMMdd_HHmmss}");

                if (!Directory.Exists(folderPath))
                {
                    // Try to find the folder by scanning
                    var dirs = Directory.GetDirectories(_experimentsPath);

                    foreach (var dir in dirs)
                    {
                        var exp = _logic.LoadExperimentFromFolder(dir);

                        if (exp != null && exp.ExperimentId == experiment.ExperimentId)
                        {
                            folderPath = dir;
                            break;
                        }
                    }
                }

                var resultItem = new ExperimentResultItem
                {
                    Algorithm = experiment.AlgorithmName,
                    Metric = experiment.DistanceMetric,
                    Iteration = 1,
                    PathLength = experiment.PathLengthCells,
                    ComputationTimeMs = experiment.SearchTimeMs,
                    Success = experiment.Success,
                    RemainingBattery = experiment.BatteryConsumption,
                    CollisionCount = experiment.CollisionCount,
                    InvalidMoveCount = experiment.InvalidMoveCount,
                    InitialScreenshotPath = Path.Combine(folderPath, "Screenshots",
                        $"{experiment.AlgorithmName}_{experiment.DistanceMetric}_Initial.png"),
                    PathScreenshotPath = Path.Combine(folderPath, "Screenshots",
                        $"{experiment.AlgorithmName}_{experiment.DistanceMetric}_Path.png"),
                    CompletedScreenshotPath = Path.Combine(folderPath, "Screenshots",
                        $"{experiment.AlgorithmName}_{experiment.DistanceMetric}_Completed.png")
                };

                var resultsList = new List<ExperimentResultItem> { resultItem };
                var resultsForm = new frmExperimentResults.frmExperimentResults(resultsList, folderPath);
                resultsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing details: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
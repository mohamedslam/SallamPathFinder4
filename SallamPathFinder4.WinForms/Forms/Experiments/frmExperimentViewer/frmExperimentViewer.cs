#region File Header
/// <summary>
/// File: frmExperimentViewer.cs
/// Description: Form to view and manage saved experiments
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Experiments;
using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentViewer.Core;

#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentViewer
{
    /// <summary>
    /// Form for viewing and managing saved experiment results
    /// </summary>
    public sealed partial class frmExperimentViewer : Form
    {
        #region Constants
        private const int FORM_WIDTH = 1200;
        private const int FORM_HEIGHT = 700;
        #endregion

        #region Private Fields
        private readonly IExperimentService _experimentService;
        private readonly ExperimentViewerLogic _logic;
        private List<ExperimentData> _allExperiments;
        private List<ExperimentData> _filteredExperiments;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the experiment viewer form
        /// </summary>
        /// <param name="experimentService">Service for accessing experiment data</param>
        public frmExperimentViewer(IExperimentService experimentService)
        {
            _experimentService = experimentService ?? throw new ArgumentNullException(nameof(experimentService));
            _logic = new ExperimentViewerLogic();
            _allExperiments = new List<ExperimentData>();
            _filteredExperiments = new List<ExperimentData>();

            InitializeComponent();  
            InitializeGoalsFilter();
            WireEvents();
            LoadExperiments();
        }
        #endregion

        #region Private Methods - Initialization
        /// <summary>
        /// Wires up all event handlers
        /// </summary>
        /// /// <summary>
        /// Initializes the goals filter dropdown
        /// </summary>
        private void InitializeGoalsFilter()
        {
            for (int i = 1; i <= 20; i++)
            {
                _cboGoalsFilter.Items.Add(i.ToString());
            }
            _cboGoalsFilter.SelectedIndex = 0;
        }
        private void WireEvents()
        {
            _cboAlgorithmFilter.SelectedIndexChanged += (s, e) => ApplyFilters();
            _dtpFrom.ValueChanged += (s, e) => ApplyFilters();
            _dtpTo.ValueChanged += (s, e) => ApplyFilters();
            _cboGoalsFilter.SelectedIndexChanged += (s, e) => ApplyFilters();
            _chkSuccessOnly.CheckedChanged += (s, e) => ApplyFilters();
            _txtSearch.TextChanged += (s, e) => ApplyFilters();
            _btnRefresh.Click += (s, e) => LoadExperiments();
            _btnExportSelected.Click += BtnExportSelected_Click;
            _btnExportAll.Click += BtnExportAll_Click;
            _btnDeleteSelected.Click += BtnDeleteSelected_Click;
            _btnViewDetails.Click += BtnViewDetails_Click;
            _btnClose.Click += (s, e) => Close();
            _dgvExperiments.CellDoubleClick += DgvExperiments_CellDoubleClick;
        }

        /// <summary>
        /// Loads experiments from the service
        /// </summary>
        private void LoadExperiments()
        {
            try
            {
                _lblStatus.Text = "Loading experiments...";
                Application.DoEvents();

                _allExperiments = _experimentService.GetAllExperiments() ?? new List<ExperimentData>();
                UpdateAlgorithmFilter();
                ApplyFilters();

                _lblStatus.Text = $"Ready - {_allExperiments.Count} experiments loaded";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading experiments: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _lblStatus.Text = "Failed to load experiments";
            }
        }

        /// <summary>
        /// Updates the algorithm filter dropdown with available algorithms
        /// </summary>
        private void UpdateAlgorithmFilter()
        {
            var algorithms = _allExperiments.Select(e => e.AlgorithmName).Distinct().OrderBy(a => a).ToList();
            foreach (var algo in algorithms)
            {
                if (!_cboAlgorithmFilter.Items.Contains(algo))
                    _cboAlgorithmFilter.Items.Add(algo);
            }
        }

        /// <summary>
        /// Applies all active filters to the experiment list
        /// </summary>
        private void ApplyFilters()
        {
            if (_allExperiments == null) return;

            var filtered = _allExperiments.AsEnumerable();

            // Algorithm filter
            string algorithmFilter = _cboAlgorithmFilter.SelectedItem?.ToString();
            if (algorithmFilter != null && algorithmFilter != "All Algorithms")
                filtered = filtered.Where(e => e.AlgorithmName == algorithmFilter);

            // Date range filter
            DateTime fromDate = _dtpFrom.Value.Date;
            DateTime toDate = _dtpTo.Value.Date.AddDays(1).AddSeconds(-1);
            filtered = filtered.Where(e => e.Timestamp >= fromDate && e.Timestamp <= toDate);

            // Goals count filter
            string goalsValue = _cboGoalsFilter.SelectedItem?.ToString();
            if (goalsValue != null && goalsValue != "All" && int.TryParse(goalsValue, out int goalCount))
                filtered = filtered.Where(e => e.GoalCount == goalCount);

            // Success only filter
            if (_chkSuccessOnly.Checked)
                filtered = filtered.Where(e => e.Success);

            // Search text filter
            string searchText = _txtSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(e =>
                    e.ExperimentId.ToLower().Contains(searchText) ||
                    e.ExperimentName?.ToLower().Contains(searchText) == true ||
                    e.MapName?.ToLower().Contains(searchText) == true ||
                    e.AlgorithmName.ToLower().Contains(searchText));
            }

            _filteredExperiments = filtered.OrderByDescending(e => e.Timestamp).ToList();
            DisplayExperiments();
            UpdateStatistics();
        }

        /// <summary>
        /// Displays experiments in the data grid view
        /// </summary>
        private void DisplayExperiments()
        {
            _dgvExperiments.Rows.Clear();

            foreach (var exp in _filteredExperiments)
            {
                _dgvExperiments.Rows.Add(
                    exp.ExperimentId,
                    exp.Timestamp.ToString("yyyy-MM-dd HH:mm"),
                    exp.ExperimentName ?? "-",
                    exp.MapName ?? "-",
                    exp.AlgorithmName,
                    exp.DistanceMetric,
                    exp.GoalCount,
                    exp.SearchTimeMs.ToString("F2"),
                    exp.PathLengthCells,
                    exp.CollisionCount,
                    exp.Success ? "✓" : "✗"
                );

                var row = _dgvExperiments.Rows[_dgvExperiments.Rows.Count - 1];
                if (!exp.Success)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230);
            }
        }

        /// <summary>
        /// Updates the statistics display
        /// </summary>
        private void UpdateStatistics()
        {
            int total = _filteredExperiments.Count;
            int successCount = _filteredExperiments.Count(e => e.Success);
            double successRate = total > 0 ? (double)successCount / total * 100 : 0;
            double avgTime = _filteredExperiments.Any() ? _filteredExperiments.Average(e => e.SearchTimeMs) : 0;
            double avgLength = _filteredExperiments.Any() ? _filteredExperiments.Average(e => (double)e.PathLengthCells) : 0;
            double avgCollisions = _filteredExperiments.Any() ? _filteredExperiments.Average(e => (double)e.CollisionCount) : 0;

            _lblStats.Text = $"📊 Total: {total} | ✅ Success: {successCount} ({successRate:F1}%) | " +
                           $"⏱️ Avg Time: {avgTime:F2} ms | 📏 Avg Length: {avgLength:F0} | " +
                           $"💥 Avg Collisions: {avgCollisions:F1}";
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles double-click on experiment row to view details
        /// </summary>
        private void DgvExperiments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            ViewExperimentDetails(e.RowIndex);
        }

        /// <summary>
        /// Handles export selected button click
        /// </summary>
        private async void BtnExportSelected_Click(object sender, EventArgs e)
        {
            if (_dgvExperiments.SelectedRows.Count == 0)
            {
                MessageBox.Show("No experiments selected.", "Export",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedIds = new List<string>();
            foreach (DataGridViewRow row in _dgvExperiments.SelectedRows)
            {
                if (row.Cells[0].Value != null)
                    selectedIds.Add(row.Cells[0].Value.ToString());
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.FileName = $"Experiments_Selected_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _lblStatus.Text = "Exporting...";
                    await _experimentService.ExportSelectedAsync(selectedIds, sfd.FileName);
                    _lblStatus.Text = "Export completed";
                    MessageBox.Show($"Exported to:\n{sfd.FileName}", "Export Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _lblStatus.Text = "Export failed";
                }
            }
        }

        /// <summary>
        /// Handles export all button click
        /// </summary>
        private async void BtnExportAll_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.FileName = $"Experiments_All_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _lblStatus.Text = "Exporting all...";
                    await _experimentService.ExportAllAsync(sfd.FileName);
                    _lblStatus.Text = "Export completed";
                    MessageBox.Show($"Exported to:\n{sfd.FileName}", "Export Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _lblStatus.Text = "Export failed";
                }
            }
        }

        /// <summary>
        /// Handles delete selected button click
        /// </summary>
        private async void BtnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (_dgvExperiments.SelectedRows.Count == 0)
            {
                MessageBox.Show("No experiments selected.", "Delete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedIds = new List<string>();
            foreach (DataGridViewRow row in _dgvExperiments.SelectedRows)
            {
                if (row.Cells[0].Value != null)
                    selectedIds.Add(row.Cells[0].Value.ToString());
            }

            var result = MessageBox.Show(
                $"Delete {selectedIds.Count} selected experiment(s)?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _lblStatus.Text = "Deleting...";
                    int deletedCount = await _experimentService.DeleteExperimentsAsync(selectedIds);
                    _lblStatus.Text = $"Deleted {deletedCount} experiment(s)";
                    LoadExperiments();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _lblStatus.Text = "Delete failed";
                }
            }
        }

        /// <summary>
        /// Handles view details button click
        /// </summary>
        private void BtnViewDetails_Click(object sender, EventArgs e)
        {
            if (_dgvExperiments.SelectedRows.Count == 0)
            {
                MessageBox.Show("No experiment selected.", "View Details",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ViewExperimentDetails(_dgvExperiments.SelectedRows[0].Index);
        }

        /// <summary>
        /// Displays detailed information about an experiment
        /// </summary>
        private void ViewExperimentDetails(int rowIndex)
        {
            // Use column index instead of name (Column 0 = ExperimentId)
            string experimentId = _dgvExperiments.Rows[rowIndex].Cells[0].Value?.ToString();
            var experiment = _filteredExperiments.FirstOrDefault(e => e.ExperimentId == experimentId);

            if (experiment != null)
            {
                MessageBox.Show(
                    $"Experiment Details:\n\n" +
                    $"ID: {experiment.ExperimentId}\n" +
                    $"Name: {experiment.ExperimentName}\n" +
                    $"Date: {experiment.Timestamp}\n" +
                    $"Algorithm: {experiment.AlgorithmName}\n" +
                    $"Metric: {experiment.DistanceMetric}\n" +
                    $"Success: {experiment.Success}\n" +
                    $"Path Length: {experiment.PathLengthCells}\n" +
                    $"Time: {experiment.SearchTimeMs:F2} ms\n" +
                    $"Collisions: {experiment.CollisionCount}\n" +
                    $"Map: {experiment.MapName}",
                    "Experiment Details",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        #endregion
    }
}
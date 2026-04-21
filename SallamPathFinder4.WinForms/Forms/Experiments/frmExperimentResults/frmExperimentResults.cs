#region File Header
/// <summary>
/// File: frmExperimentResults.cs
/// Description: Form to display experiment results with advanced battery and path statistics
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-19
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentResults.Core;
using SallamPathFinder4.WinForms.Forms.Experiments.frmScreenshotViewer;
using SallamPathFinder4.WinForms.Forms.Experiments.frmStatisticsViewer;
using SallamPathFinder4.WinForms.Models;
using System.Text;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentResults
{
    public sealed partial class frmExperimentResults : Form
    {
        #region Constants
        private const int FORM_WIDTH = 1600;
        private const int FORM_HEIGHT = 850;
        #endregion

        #region Private Fields
        private readonly List<ExperimentResultItem> _allResults;
        private readonly string _resultsFolderPath;
        private ExperimentResultItem _selectedResult;
        private readonly ExperimentResultsLogic _logic;
        #endregion

        #region Constructor
        public frmExperimentResults(List<ExperimentResultItem> results, string folderPath)
        {
            System.Diagnostics.Debug.WriteLine("=== frmExperimentResults CONSTRUCTOR START ===");
            System.Diagnostics.Debug.WriteLine($"Results count: {results?.Count ?? 0}");
            System.Diagnostics.Debug.WriteLine($"Folder path: {folderPath}");

            try
            {
                _allResults = results ?? new List<ExperimentResultItem>();
                _resultsFolderPath = folderPath ?? string.Empty;
                _logic = new ExperimentResultsLogic();

                System.Diagnostics.Debug.WriteLine("Calling InitializeComponent...");
                InitializeComponent();

                System.Diagnostics.Debug.WriteLine("Calling WireEvents...");
                WireEvents();

                System.Diagnostics.Debug.WriteLine("Calling InitializeFilters...");
                InitializeFilters();

                System.Diagnostics.Debug.WriteLine("Calling SetupDataGridViewColumns...");
                SetupDataGridViewColumns();

                System.Diagnostics.Debug.WriteLine("Calling ApplyFilters...");
                ApplyFilters();

                System.Diagnostics.Debug.WriteLine("=== frmExperimentResults CONSTRUCTOR END ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION in constructor: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
        #endregion

        #region Private Methods - Initialization
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
            _btnNewExperiment.Click += (s, e) => { DialogResult = DialogResult.Retry; Close(); };
            _btnClose.Click += (s, e) => Close();
            _dgvResults.CellClick += DgvResults_CellClick;
            _dgvResults.CellDoubleClick += DgvResults_CellDoubleClick;
            _dgvResults.CellFormatting += DgvResults_CellFormatting;
        }

        private void SetupDataGridViewColumns()
        {
            _dgvResults.Columns.Clear();

            // Basic columns
            _dgvResults.Columns.Add("Id", "#");
            _dgvResults.Columns.Add("Algorithm", "Algorithm");
            _dgvResults.Columns.Add("Metric", "Metric");
            _dgvResults.Columns.Add("Iteration", "Iter");
            _dgvResults.Columns.Add("PathLength", "Length");
            _dgvResults.Columns.Add("TimeMs", "Time (ms)");

            // Battery columns
            _dgvResults.Columns.Add("InitialBattery", "Init Bat %");
            _dgvResults.Columns.Add("FinalBattery", "Final Bat %");
            _dgvResults.Columns.Add("ConsumedBattery", "Consumed %");
            _dgvResults.Columns.Add("ChargingUnits", "Chg Units");
            _dgvResults.Columns.Add("ChargingCycles", "Chg Cycles");
            _dgvResults.Columns.Add("ChargingTime", "Chg Time (s)");

            // Time columns
            _dgvResults.Columns.Add("TravelTime", "Travel (s)");
            _dgvResults.Columns.Add("OverheadTime", "Overhead (s)");
            _dgvResults.Columns.Add("TotalTime", "Total (s)");

            // Path columns
            _dgvResults.Columns.Add("StartPoint", "Start");
            _dgvResults.Columns.Add("EndPoint", "End");
            _dgvResults.Columns.Add("GoalOrder", "Goal Order");

            // Error columns
            _dgvResults.Columns.Add("Collisions", "Collisions");
            _dgvResults.Columns.Add("Errors", "Errors");
            _dgvResults.Columns.Add("Speed", "Speed");
            _dgvResults.Columns.Add("Success", "✓");
            _dgvResults.Columns.Add("HasScreenshot", "📷");

            // Set column widths
            _dgvResults.Columns["GoalOrder"].Width = 250;
            _dgvResults.Columns["GoalOrder"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _dgvResults.Columns["Algorithm"].Width = 100;
            _dgvResults.Columns["Metric"].Width = 100;
        }

        private void InitializeFilters()
        {
            var algorithms = _allResults.Select(r => r.Algorithm).Distinct().OrderBy(a => a).ToList();
            _cboAlgorithmFilter.Items.Clear();
            _cboAlgorithmFilter.Items.Add("All Algorithms");
            foreach (var algo in algorithms) _cboAlgorithmFilter.Items.Add(algo);
            _cboAlgorithmFilter.SelectedIndex = 0;

            var metrics = _allResults.Select(r => r.Metric).Distinct().OrderBy(m => m).ToList();
            _cboMetricFilter.Items.Clear();
            _cboMetricFilter.Items.Add("All Metrics");
            foreach (var metric in metrics) _cboMetricFilter.Items.Add(metric);
            _cboMetricFilter.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            System.Diagnostics.Debug.WriteLine($"ApplyFilters: _allResults.Count = {_allResults?.Count ?? 0}");

            if (_allResults == null || _allResults.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("ApplyFilters: No results to filter");
                _dgvResults.Rows.Clear();
                _lblStatistics.Text = "No results available.";
                return;
            }

            var filtered = _allResults.AsEnumerable();

            // ... باقي الفلاتر ...

            var filteredList = filtered.ToList();
            System.Diagnostics.Debug.WriteLine($"ApplyFilters: Filtered to {filteredList.Count} results");

            DisplayResults(filteredList);
            UpdateStatistics(filteredList);
        }
 
        private void DisplayResults(List<ExperimentResultItem> results)
        {
            _dgvResults.Rows.Clear();
            int id = 1;

            foreach (var r in results)
            {
                if (r == null) continue;

                bool hasScreenshot = (!string.IsNullOrEmpty(r.InitialScreenshotPath) && File.Exists(r.InitialScreenshotPath)) ||
                                    (!string.IsNullOrEmpty(r.PathScreenshotPath) && File.Exists(r.PathScreenshotPath)) ||
                                    (!string.IsNullOrEmpty(r.CompletedScreenshotPath) && File.Exists(r.CompletedScreenshotPath));

                _dgvResults.Rows.Add(
                    id++,
                    r.Algorithm ?? "Unknown",
                    r.Metric ?? "Unknown",
                    r.Iteration,
                    r.PathLength,
                    r.ComputationTimeMs.ToString("F2"),
                    r.InitialBatteryPercent.ToString("F1"),
                    r.FinalBatteryPercent.ToString("F1"),
                    r.TotalBatteryConsumedPercent.ToString("F1"),
                    r.TotalChargingUnits.ToString("F2"),
                    r.TotalChargingCycles,
                    r.TotalChargingTimeSeconds.ToString("F0"),
                    r.TotalTravelTimeSeconds.ToString("F2"),
                    r.TotalOverheadTimeSeconds.ToString("F2"),
                    r.TotalTimeSeconds.ToString("F2"),
                    $"({r.StartPointUsed.X},{r.StartPointUsed.Y})",
                    $"({r.EndPointReached.X},{r.EndPointReached.Y})",
                    r.GoalOrder?.Length > 40 ? r.GoalOrder.Substring(0, 40) + "..." : r.GoalOrder ?? "-",
                    r.CollisionCount,
                    r.InvalidMoveCount,
                    r.AverageActualSpeed.ToString("F1"),
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

        private void UpdateStatistics(List<ExperimentResultItem> results)
        {
            int total = results.Count;
            int successCount = results.Count(r => r.Success);
            double successRate = total > 0 ? (double)successCount / total * 100 : 0;
            double avgTime = results.Any() ? results.Average(r => r.ComputationTimeMs) : 0;
            double avgLength = results.Any() ? results.Average(r => (double)r.PathLength) : 0;
            double avgFinalBattery = results.Any() ? results.Average(r => r.FinalBatteryPercent) : 0;
            double avgChargingUnits = results.Any() ? results.Average(r => r.TotalChargingUnits) : 0;
            double avgTotalTime = results.Any() ? results.Average(r => r.TotalTimeSeconds) : 0;
            double avgCollisions = results.Any() ? results.Average(r => r.CollisionCount) : 0;

            _lblStatistics.Text = $"📊 Total: {total} | ✅ Success: {successCount} ({successRate:F1}%) | " +
                                 $"⏱️ Avg Time: {avgTime:F2} ms | 📏 Avg Length: {avgLength:F0} | " +
                                 $"🔋 Avg Final Battery: {avgFinalBattery:F1}% | " +
                                 $"⚡ Avg Charging Units: {avgChargingUnits:F2} | " +
                                 $"⏰ Avg Total Time: {avgTotalTime:F0}s | " +
                                 $"💥 Avg Collisions: {avgCollisions:F1}";
        }

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
        private void DgvResults_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            _selectedResult = GetResultFromRow(e.RowIndex);
            if (_selectedResult != null)
            {
                LoadScreenshots();
                UpdateDetailsPanel();
            }
        }

        private void DgvResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var selected = GetResultFromRow(e.RowIndex);
            if (selected != null) ShowFullPathDetails(selected);
        }

        private void DgvResults_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (_dgvResults.Columns["Success"] != null && e.ColumnIndex == _dgvResults.Columns["Success"].Index)
            {
                e.CellStyle.ForeColor = e.Value?.ToString() == "✓" ? Color.Green : Color.Red;
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }

        private ExperimentResultItem GetResultFromRow(int rowIndex)
        {
            string algorithm = _dgvResults.Rows[rowIndex].Cells["Algorithm"].Value?.ToString();
            string metric = _dgvResults.Rows[rowIndex].Cells["Metric"].Value?.ToString();
            int iteration = int.Parse(_dgvResults.Rows[rowIndex].Cells["Iteration"].Value?.ToString() ?? "0");

            return _allResults.FirstOrDefault(r => r.Algorithm == algorithm && r.Metric == metric && r.Iteration == iteration);
        }
        #endregion

        #region Private Methods - Display
        private void ShowFullPathDetails(ExperimentResultItem result)
        {
            var detailsForm = new Form
            {
                Text = $"Path Details - {result.Algorithm} - Iter {result.Iteration}",
                Size = new Size(700, 600),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White
            };

            var splitContainer = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 200 };

            // Info panel
            var infoText = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                Font = new Font("Consolas", 10),
                BackColor = Color.FromArgb(248, 249, 250),
                Text = $"═══════════════════════════════════════════════════════════\n" +
                       $"                    PATH DETAILS\n" +
                       $"═══════════════════════════════════════════════════════════\n\n" +
                       $"Algorithm:        {result.Algorithm}\n" +
                       $"Metric:           {result.Metric}\n" +
                       $"Iteration:        {result.Iteration}\n" +
                       $"Success:          {(result.Success ? "✓ Yes" : "✗ No")}\n\n" +
                       $"{new string('─', 55)}\n" +
                       $"                    BATTERY STATISTICS\n" +
                       $"{new string('─', 55)}\n\n" +
                       result.GetBatteryStatsText() + "\n\n" +
                       $"{new string('─', 55)}\n" +
                       $"                    TIME STATISTICS\n" +
                       $"{new string('─', 55)}\n\n" +
                       result.GetTimeStatsText() + "\n\n" +
                       $"{new string('─', 55)}\n" +
                       $"                    PATH INFORMATION\n" +
                       $"{new string('─', 55)}\n\n" +
                       result.GetPathInfoText() + "\n\n" +
                       $"Goal Order:       {result.GoalOrder}"
            };

            // Path points panel
            var pathList = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                IntegralHeight = false
            };
            for (int i = 0; i < result.Path.Count; i++)
            {
                var point = result.Path[i];
                pathList.Items.Add($"{i + 1,4}. ({point.X,3}, {point.Y,3})");
            }

            var pathPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            pathPanel.Controls.Add(pathList);
            pathPanel.Controls.Add(new Label { Text = $"📍 Path Points ({result.PathLength} cells)", Dock = DockStyle.Top, Font = new Font("Segoe UI", 10, FontStyle.Bold), Height = 30 });

            splitContainer.Panel1.Controls.Add(infoText);
            splitContainer.Panel2.Controls.Add(pathPanel);
            detailsForm.Controls.Add(splitContainer);
            detailsForm.ShowDialog();
        }

        private void LoadScreenshots()
        {
            if (_selectedResult == null) return;

            string basePath = Path.Combine(_resultsFolderPath, "Screenshots", _selectedResult.Algorithm);

            TryLoadImage(_picInitial, _selectedResult.InitialScreenshotPath);
            TryLoadImage(_picPath, _selectedResult.PathScreenshotPath);
            TryLoadImage(_picCompleted, _selectedResult.CompletedScreenshotPath);
        }

        private void TryLoadImage(PictureBox pictureBox, string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try { pictureBox.Image = Image.FromFile(path); }
                catch { pictureBox.Image = null; }
            }
            else pictureBox.Image = null;
        }

        private void UpdateDetailsPanel()
        {
            if (_selectedResult == null) return;

            _lblDetails.Text =
                $"═══════════════════════════════════\n" +
                $"           EXPERIMENT DETAILS\n" +
                $"═══════════════════════════════════\n\n" +
                $"Algorithm:        {_selectedResult.Algorithm}\n" +
                $"Metric:           {_selectedResult.Metric}\n" +
                $"Iteration:        {_selectedResult.Iteration}\n" +
                $"Success:          {(_selectedResult.Success ? "✓ Yes" : "✗ No")}\n\n" +
                $"───────────────────────────────────\n" +
                $"BATTERY STATISTICS\n" +
                $"───────────────────────────────────\n" +
                _selectedResult.GetBatteryStatsText() + "\n\n" +
                $"───────────────────────────────────\n" +
                $"TIME STATISTICS\n" +
                $"───────────────────────────────────\n" +
                _selectedResult.GetTimeStatsText() + "\n\n" +
                $"───────────────────────────────────\n" +
                $"PATH INFORMATION\n" +
                $"───────────────────────────────────\n" +
                _selectedResult.GetPathInfoText() + "\n\n" +
                $"───────────────────────────────────\n" +
                $"COLLISION STATISTICS\n" +
                $"───────────────────────────────────\n" +
                $"Collisions:       {_selectedResult.CollisionCount}\n" +
                $"Invalid Moves:    {_selectedResult.InvalidMoveCount}\n" +
                $"Avg Speed:        {(_selectedResult.AverageActualSpeed > 0 ? $"{_selectedResult.AverageActualSpeed:F1} cm/s" : "N/A")}";
        }
        #endregion

        #region Export Methods
        private void ExportToCSV()
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.FileName = $"Experiment_Results_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using var writer = new StreamWriter(sfd.FileName, false, Encoding.UTF8);
                    writer.WriteLine("Algorithm,Metric,Iteration,Success,PathLength,TimeMs," +
                        "InitialBattery%,FinalBattery%,TotalConsumed%,ChargingUnits,ChargingCycles,ChargingTimeSec," +
                        "TravelTimeSec,OverheadTimeSec,TotalTimeSec," +
                        "StartX,StartY,EndX,EndY,GoalOrder," +
                        "Collisions,Errors,AvgSpeed,ErrorMessage");

                    foreach (var r in _allResults)
                    {
                        writer.WriteLine($"{r.Algorithm},{r.Metric},{r.Iteration},{r.Success},{r.PathLength},{r.ComputationTimeMs:F2}," +
                            $"{r.InitialBatteryPercent:F1},{r.FinalBatteryPercent:F1},{r.TotalBatteryConsumedPercent:F1}," +
                            $"{r.TotalChargingUnits:F2},{r.TotalChargingCycles},{r.TotalChargingTimeSeconds:F0}," +
                            $"{r.TotalTravelTimeSeconds:F2},{r.TotalOverheadTimeSeconds:F2},{r.TotalTimeSeconds:F2}," +
                            $"{r.StartPointUsed.X},{r.StartPointUsed.Y},{r.EndPointReached.X},{r.EndPointReached.Y}," +
                            $"\"{r.GoalOrder?.Replace("\"", "\"\"")}\"," +
                            $"{r.CollisionCount},{r.InvalidMoveCount},{r.AverageActualSpeed:F1}," +
                            $"\"{r.ErrorMessage?.Replace("\"", "\"\"")}\"");
                    }

                    MessageBox.Show($"Exported to:\n{sfd.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void ExportToExcel() => ExportToCSV();
        private void ExportToPDF() => ExportToCSV();
        private void SaveReplay() { }
        private void PlayReplay() { }
        private void ShowAdvancedStatistics()
        {
            var statsViewer = new  frmStatisticsViewer.frmStatisticsViewer(_allResults);
            statsViewer.ShowDialog();
        }
        #endregion

        #region Picture Double-Click Handlers
        private void PicInitial_DoubleClick(object sender, EventArgs e) => ShowScreenshot(_selectedResult?.InitialScreenshotPath);
        private void PicPath_DoubleClick(object sender, EventArgs e) => ShowScreenshot(_selectedResult?.PathScreenshotPath);
        private void PicCompleted_DoubleClick(object sender, EventArgs e) => ShowScreenshot(_selectedResult?.CompletedScreenshotPath);

        private void ShowScreenshot(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                new frmScreenshotViewer.frmScreenshotViewer(_selectedResult, _resultsFolderPath).ShowDialog();
            else
                MessageBox.Show("No screenshot available.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
    }
}
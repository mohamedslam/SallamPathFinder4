#region File Header
/// <summary>
/// File: frmExperimentResults.Designer.cs
/// Description: Designer file for experiment results form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentResults
{
    partial class frmExperimentResults
    {
        #region Private Fields - UI Components
        private System.ComponentModel.IContainer _components;
        private System.Windows.Forms.DataGridView _dgvResults;
        private System.Windows.Forms.ComboBox _cboAlgorithmFilter;
        private System.Windows.Forms.ComboBox _cboMetricFilter;
        private System.Windows.Forms.ComboBox _cboResultFilter;
        private System.Windows.Forms.TextBox _txtSearch;
        private System.Windows.Forms.Label _lblStatistics;
        private System.Windows.Forms.Button _btnExportCSV;
        private System.Windows.Forms.Button _btnExportExcel;
        private System.Windows.Forms.Button _btnExportPDF;
        private System.Windows.Forms.Button _btnClearFilters;
        private System.Windows.Forms.Button _btnAdvancedStats;
        private System.Windows.Forms.Button _btnSaveReplay;
        private System.Windows.Forms.Button _btnPlayReplay;
        private System.Windows.Forms.Button _btnNewExperiment;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.Panel _filterPanel;
        private System.Windows.Forms.Panel _bottomPanel;
        private System.Windows.Forms.Panel _screenshotPanel;
        private System.Windows.Forms.PictureBox _picInitial;
        private System.Windows.Forms.PictureBox _picPath;
        private System.Windows.Forms.PictureBox _picCompleted;
        private System.Windows.Forms.Label _lblInitial;
        private System.Windows.Forms.Label _lblPath;
        private System.Windows.Forms.Label _lblCompleted;
        private System.Windows.Forms.Label _lblDetails;
        #endregion
 
        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Initialize Component
       private void InitializeComponent()
{
    filterPanel = new Panel();
    lblAlgorithm = new Label();
    _cboAlgorithmFilter = new ComboBox();
    lblMetric = new Label();
    _cboMetricFilter = new ComboBox();
    lblResult = new Label();
    _cboResultFilter = new ComboBox();
    lblSearch = new Label();
    _txtSearch = new TextBox();
    _btnClearFilters = new Button();
    _dgvResults = new DataGridView();
    screenshotPanel = new Panel();
    lblScreenshotTitle = new Label();
    _picInitial = new PictureBox();
    _picPath = new PictureBox();
    _picCompleted = new PictureBox();
    lblInitial = new Label();
    lblPath = new Label();
    lblCompleted = new Label();
    _lblDetails = new Label();
    bottomPanel = new Panel();
    _lblStatistics = new Label();
    btnPanel = new Panel();
    _btnExportCSV = new Button();
    _btnExportExcel = new Button();
    _btnExportPDF = new Button();
    _btnSaveReplay = new Button();
    _btnPlayReplay = new Button();
    _btnAdvancedStats = new Button();
    _btnNewExperiment = new Button();
    _btnClose = new Button();
    
    filterPanel.SuspendLayout();
    ((System.ComponentModel.ISupportInitialize)_dgvResults).BeginInit();
    screenshotPanel.SuspendLayout();
    ((System.ComponentModel.ISupportInitialize)_picInitial).BeginInit();
    ((System.ComponentModel.ISupportInitialize)_picPath).BeginInit();
    ((System.ComponentModel.ISupportInitialize)_picCompleted).BeginInit();
    bottomPanel.SuspendLayout();
    SuspendLayout();

    // filterPanel
    filterPanel.BackColor = Color.FromArgb(240, 242, 245);
    filterPanel.Controls.Add(lblAlgorithm);
    filterPanel.Controls.Add(_cboAlgorithmFilter);
    filterPanel.Controls.Add(lblMetric);
    filterPanel.Controls.Add(_cboMetricFilter);
    filterPanel.Controls.Add(lblResult);
    filterPanel.Controls.Add(_cboResultFilter);
    filterPanel.Controls.Add(lblSearch);
    filterPanel.Controls.Add(_txtSearch);
    filterPanel.Controls.Add(_btnClearFilters);
    filterPanel.Dock = DockStyle.Top;
    filterPanel.Location = new Point(0, 0);
    filterPanel.Name = "filterPanel";
    filterPanel.Padding = new Padding(5);
    filterPanel.Size = new Size(1284, 50);
    filterPanel.TabIndex = 3;

    // lblAlgorithm
    lblAlgorithm.Location = new Point(10, 15);
    lblAlgorithm.Name = "lblAlgorithm";
    lblAlgorithm.Size = new Size(70, 20);
    lblAlgorithm.TabIndex = 0;
    lblAlgorithm.Text = "Algorithm:";

    // _cboAlgorithmFilter
    _cboAlgorithmFilter.DropDownStyle = ComboBoxStyle.DropDownList;
    _cboAlgorithmFilter.Items.AddRange(new object[] { "All Algorithms" });
    _cboAlgorithmFilter.Location = new Point(80, 12);
    _cboAlgorithmFilter.Name = "_cboAlgorithmFilter";
    _cboAlgorithmFilter.Size = new Size(120, 23);
    _cboAlgorithmFilter.TabIndex = 1;

    // lblMetric
    lblMetric.Location = new Point(220, 15);
    lblMetric.Name = "lblMetric";
    lblMetric.Size = new Size(60, 20);
    lblMetric.TabIndex = 2;
    lblMetric.Text = "Metric:";

    // _cboMetricFilter
    _cboMetricFilter.DropDownStyle = ComboBoxStyle.DropDownList;
    _cboMetricFilter.Items.AddRange(new object[] { "All Metrics" });
    _cboMetricFilter.Location = new Point(280, 12);
    _cboMetricFilter.Name = "_cboMetricFilter";
    _cboMetricFilter.Size = new Size(120, 23);
    _cboMetricFilter.TabIndex = 3;

    // lblResult
    lblResult.Location = new Point(420, 15);
    lblResult.Name = "lblResult";
    lblResult.Size = new Size(50, 20);
    lblResult.TabIndex = 4;
    lblResult.Text = "Result:";

    // _cboResultFilter
    _cboResultFilter.DropDownStyle = ComboBoxStyle.DropDownList;
    _cboResultFilter.Items.AddRange(new object[] { "All Results", "Success Only", "Failure Only" });
    _cboResultFilter.Location = new Point(470, 12);
    _cboResultFilter.Name = "_cboResultFilter";
    _cboResultFilter.Size = new Size(100, 23);
    _cboResultFilter.TabIndex = 5;

    // lblSearch
    lblSearch.Location = new Point(590, 15);
    lblSearch.Name = "lblSearch";
    lblSearch.Size = new Size(50, 20);
    lblSearch.TabIndex = 6;
    lblSearch.Text = "Search:";

    // _txtSearch
    _txtSearch.Location = new Point(640, 12);
    _txtSearch.Name = "_txtSearch";
    _txtSearch.Size = new Size(150, 23);
    _txtSearch.TabIndex = 7;

    // _btnClearFilters
    _btnClearFilters.BackColor = Color.FromArgb(149, 165, 166);
    _btnClearFilters.Cursor = Cursors.Hand;
    _btnClearFilters.FlatStyle = FlatStyle.Flat;
    _btnClearFilters.ForeColor = Color.White;
    _btnClearFilters.Location = new Point(800, 10);
    _btnClearFilters.Name = "_btnClearFilters";
    _btnClearFilters.Size = new Size(70, 28);
    _btnClearFilters.TabIndex = 8;
    _btnClearFilters.Text = "Clear";
    _btnClearFilters.UseVisualStyleBackColor = false;

    // _dgvResults
    _dgvResults.AllowUserToAddRows = false;
    _dgvResults.AllowUserToDeleteRows = false;
    _dgvResults.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
    _dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
    _dgvResults.BackgroundColor = Color.White;
    _dgvResults.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
    _dgvResults.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
    _dgvResults.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
    _dgvResults.EnableHeadersVisualStyles = false;
    _dgvResults.Location = new Point(0, 50);
    _dgvResults.MultiSelect = false;
    _dgvResults.Name = "_dgvResults";
    _dgvResults.ReadOnly = true;
    _dgvResults.RowHeadersVisible = false;
    _dgvResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    _dgvResults.Size = new Size(910, 600);
    _dgvResults.TabIndex = 0;

    // Add columns to _dgvResults
    _dgvResults.Columns.Add("Id", "#");
    _dgvResults.Columns.Add("Algorithm", "Algorithm");
    _dgvResults.Columns.Add("Metric", "Metric");
    _dgvResults.Columns.Add("Iteration", "Iter");
    _dgvResults.Columns.Add("PathLength", "Length");
    _dgvResults.Columns.Add("TimeMs", "Time (ms)");
    _dgvResults.Columns.Add("BatteryRemaining", "Battery %");
    _dgvResults.Columns.Add("Collisions", "Collisions");
    _dgvResults.Columns.Add("Errors", "Errors");
    _dgvResults.Columns.Add("AvgSpeed", "Speed");
    _dgvResults.Columns.Add("Success", "✓");
    _dgvResults.Columns.Add("HasScreenshot", "📷");
    // ========== BATTERY STATISTICS COLUMNS ==========
    _dgvResults.Columns.Add("InitialBattery", "Init Bat %");
    _dgvResults.Columns.Add("FinalBattery", "Final Bat %");
    _dgvResults.Columns.Add("ConsumedBattery", "Consumed %");
    _dgvResults.Columns.Add("ChargingUnits", "Chg Units");
    _dgvResults.Columns.Add("ChargingCycles", "Chg Cycles");
    _dgvResults.Columns.Add("ChargingTime", "Chg Time (s)");

    // ========== TIME STATISTICS COLUMNS ==========
    _dgvResults.Columns.Add("TravelTime", "Travel (s)");
    _dgvResults.Columns.Add("OverheadTime", "Overhead (s)");
    _dgvResults.Columns.Add("TotalTime", "Total (s)");

    // ========== PATH INFORMATION COLUMNS ==========
    _dgvResults.Columns.Add("StartPoint", "Start");
    _dgvResults.Columns.Add("EndPoint", "End");
    _dgvResults.Columns.Add("GoalOrder", "Goal Order");

    // اضبط عرض الأعمدة
    _dgvResults.Columns["GoalOrder"].Width = 200;
    _dgvResults.Columns["GoalOrder"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            // screenshotPanel
            screenshotPanel.BackColor = Color.FromArgb(248, 249, 250);
    screenshotPanel.BorderStyle = BorderStyle.FixedSingle;
    screenshotPanel.Controls.Add(lblScreenshotTitle);
    screenshotPanel.Controls.Add(_picInitial);
    screenshotPanel.Controls.Add(_picPath);
    screenshotPanel.Controls.Add(_picCompleted);
    screenshotPanel.Controls.Add(lblInitial);
    screenshotPanel.Controls.Add(lblPath);
    screenshotPanel.Controls.Add(lblCompleted);
    screenshotPanel.Location = new Point(944, 50);
    screenshotPanel.Name = "screenshotPanel";
    screenshotPanel.Size = new Size(300, 180);
    screenshotPanel.TabIndex = 1;

    // lblScreenshotTitle
    lblScreenshotTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
    lblScreenshotTitle.Location = new Point(10, 5);
    lblScreenshotTitle.Name = "lblScreenshotTitle";
    lblScreenshotTitle.Size = new Size(80, 20);
    lblScreenshotTitle.TabIndex = 0;
    lblScreenshotTitle.Text = "Screenshots";

    // _picInitial
    _picInitial.BackColor = Color.White;
    _picInitial.BorderStyle = BorderStyle.FixedSingle;
    _picInitial.Location = new Point(10, 30);
    _picInitial.Name = "_picInitial";
    _picInitial.Size = new Size(90, 90);
    _picInitial.SizeMode = PictureBoxSizeMode.Zoom;
    _picInitial.TabIndex = 1;
    _picInitial.TabStop = false;
    _picInitial.DoubleClick += PicInitial_DoubleClick;

    // _picPath
    _picPath.BackColor = Color.White;
    _picPath.BorderStyle = BorderStyle.FixedSingle;
    _picPath.Location = new Point(105, 30);
    _picPath.Name = "_picPath";
    _picPath.Size = new Size(90, 90);
    _picPath.SizeMode = PictureBoxSizeMode.Zoom;
    _picPath.TabIndex = 2;
    _picPath.TabStop = false;
    _picPath.DoubleClick += PicPath_DoubleClick;

    // _picCompleted
    _picCompleted.BackColor = Color.White;
    _picCompleted.BorderStyle = BorderStyle.FixedSingle;
    _picCompleted.Location = new Point(200, 30);
    _picCompleted.Name = "_picCompleted";
    _picCompleted.Size = new Size(90, 90);
    _picCompleted.SizeMode = PictureBoxSizeMode.Zoom;
    _picCompleted.TabIndex = 3;
    _picCompleted.TabStop = false;
    _picCompleted.DoubleClick += PicCompleted_DoubleClick;

    // lblInitial
    lblInitial.Location = new Point(10, 125);
    lblInitial.Name = "lblInitial";
    lblInitial.Size = new Size(90, 20);
    lblInitial.TabIndex = 4;
    lblInitial.Text = "Initial";
    lblInitial.TextAlign = ContentAlignment.MiddleCenter;

    // lblPath
    lblPath.Location = new Point(105, 125);
    lblPath.Name = "lblPath";
    lblPath.Size = new Size(90, 20);
    lblPath.TabIndex = 5;
    lblPath.Text = "Path";
    lblPath.TextAlign = ContentAlignment.MiddleCenter;

    // lblCompleted
    lblCompleted.Location = new Point(200, 125);
    lblCompleted.Name = "lblCompleted";
    lblCompleted.Size = new Size(90, 20);
    lblCompleted.TabIndex = 6;
    lblCompleted.Text = "Completed";
    lblCompleted.TextAlign = ContentAlignment.MiddleCenter;

    // _lblDetails
    _lblDetails.BackColor = Color.FromArgb(248, 249, 250);
    _lblDetails.BorderStyle = BorderStyle.FixedSingle;
    _lblDetails.Font = new Font("Segoe UI", 9F);
    _lblDetails.ForeColor = Color.FromArgb(52, 73, 94);
    _lblDetails.Location = new Point(935, 250);
    _lblDetails.Name = "_lblDetails";
    _lblDetails.Padding = new Padding(5);
    _lblDetails.Size = new Size(309, 120);
    _lblDetails.TabIndex = 2;

    // bottomPanel
    bottomPanel.BackColor = Color.FromArgb(240, 242, 245);
    bottomPanel.Controls.Add(_lblStatistics);
    bottomPanel.Controls.Add(btnPanel);
    bottomPanel.Dock = DockStyle.Bottom;
    bottomPanel.Location = new Point(0, 651);
    bottomPanel.Name = "bottomPanel";
    bottomPanel.Padding = new Padding(5);
    bottomPanel.Size = new Size(1284, 60);
    bottomPanel.TabIndex = 4;

    // _lblStatistics
    _lblStatistics.AutoSize = true;
    _lblStatistics.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
    _lblStatistics.ForeColor = Color.FromArgb(52, 73, 94);
    _lblStatistics.Location = new Point(10, 5);
    _lblStatistics.Name = "_lblStatistics";
    _lblStatistics.Size = new Size(0, 15);
    _lblStatistics.TabIndex = 0;

    // btnPanel
    btnPanel.Location = new Point(10, 28);
    btnPanel.Name = "btnPanel";
    btnPanel.Size = new Size(900, 28);
    btnPanel.TabIndex = 1;

    // _btnExportCSV
    _btnExportCSV.Text = "CSV";
    _btnExportCSV.Location = new Point(0, 0);
    _btnExportCSV.Size = new Size(70, 28);
    _btnExportCSV.FlatStyle = FlatStyle.Flat;
    _btnExportCSV.BackColor = Color.FromArgb(52, 152, 219);
    _btnExportCSV.ForeColor = Color.White;
    _btnExportCSV.Cursor = Cursors.Hand;
    _btnExportCSV.Font = new Font("Segoe UI", 8, FontStyle.Bold);

    // _btnExportExcel
    _btnExportExcel.Text = "Excel";
    _btnExportExcel.Location = new Point(75, 0);
    _btnExportExcel.Size = new Size(80, 28);
    _btnExportExcel.FlatStyle = FlatStyle.Flat;
    _btnExportExcel.BackColor = Color.FromArgb(52, 152, 219);
    _btnExportExcel.ForeColor = Color.White;
    _btnExportExcel.Cursor = Cursors.Hand;
    _btnExportExcel.Font = new Font("Segoe UI", 8, FontStyle.Bold);

    // _btnExportPDF
    _btnExportPDF.Text = "PDF";
    _btnExportPDF.Location = new Point(160, 0);
    _btnExportPDF.Size = new Size(70, 28);
    _btnExportPDF.FlatStyle = FlatStyle.Flat;
    _btnExportPDF.BackColor = Color.FromArgb(52, 152, 219);
    _btnExportPDF.ForeColor = Color.White;
    _btnExportPDF.Cursor = Cursors.Hand;
    _btnExportPDF.Font = new Font("Segoe UI", 8, FontStyle.Bold);

    // _btnSaveReplay
    _btnSaveReplay.Text = "Save Replay";
    _btnSaveReplay.Location = new Point(235, 0);
    _btnSaveReplay.Size = new Size(100, 28);
    _btnSaveReplay.FlatStyle = FlatStyle.Flat;
    _btnSaveReplay.BackColor = Color.FromArgb(52, 152, 219);
    _btnSaveReplay.ForeColor = Color.White;
    _btnSaveReplay.Cursor = Cursors.Hand;
    _btnSaveReplay.Font = new Font("Segoe UI", 8, FontStyle.Bold);

    // _btnPlayReplay
    _btnPlayReplay.Text = "Play Replay";
    _btnPlayReplay.Location = new Point(340, 0);
    _btnPlayReplay.Size = new Size(100, 28);
    _btnPlayReplay.FlatStyle = FlatStyle.Flat;
    _btnPlayReplay.BackColor = Color.FromArgb(52, 152, 219);
    _btnPlayReplay.ForeColor = Color.White;
    _btnPlayReplay.Cursor = Cursors.Hand;
    _btnPlayReplay.Font = new Font("Segoe UI", 8, FontStyle.Bold);

    // _btnAdvancedStats
    _btnAdvancedStats.Text = "Advanced Stats";
    _btnAdvancedStats.Location = new Point(445, 0);
    _btnAdvancedStats.Size = new Size(120, 28);
    _btnAdvancedStats.FlatStyle = FlatStyle.Flat;
    _btnAdvancedStats.BackColor = Color.FromArgb(155, 89, 182);
    _btnAdvancedStats.ForeColor = Color.White;
    _btnAdvancedStats.Cursor = Cursors.Hand;
    _btnAdvancedStats.Font = new Font("Segoe UI", 8, FontStyle.Bold);

    // _btnNewExperiment
    _btnNewExperiment.Text = "New Experiment";
    _btnNewExperiment.Location = new Point(570, 0);
    _btnNewExperiment.Size = new Size(120, 28);
    _btnNewExperiment.FlatStyle = FlatStyle.Flat;
    _btnNewExperiment.BackColor = Color.FromArgb(46, 204, 113);
    _btnNewExperiment.ForeColor = Color.White;
    _btnNewExperiment.Cursor = Cursors.Hand;
    _btnNewExperiment.Font = new Font("Segoe UI", 8, FontStyle.Bold);

    // _btnClose
    _btnClose.Text = "Close";
    _btnClose.Location = new Point(695, 0);
    _btnClose.Size = new Size(80, 28);
    _btnClose.FlatStyle = FlatStyle.Flat;
    _btnClose.BackColor = Color.FromArgb(231, 76, 60);
    _btnClose.ForeColor = Color.White;
    _btnClose.Cursor = Cursors.Hand;
    _btnClose.Font = new Font("Segoe UI", 8, FontStyle.Bold);

    // Add buttons to btnPanel
    btnPanel.Controls.Add(_btnExportCSV);
    btnPanel.Controls.Add(_btnExportExcel);
    btnPanel.Controls.Add(_btnExportPDF);
    btnPanel.Controls.Add(_btnSaveReplay);
    btnPanel.Controls.Add(_btnPlayReplay);
    btnPanel.Controls.Add(_btnAdvancedStats);
    btnPanel.Controls.Add(_btnNewExperiment);
    btnPanel.Controls.Add(_btnClose);

    // frmExperimentResults
    BackColor = Color.White;
    ClientSize = new Size(1284, 711);
    Controls.Add(_dgvResults);
    Controls.Add(screenshotPanel);
    Controls.Add(_lblDetails);
    Controls.Add(filterPanel);
    Controls.Add(bottomPanel);
    MinimumSize = new Size(1000, 600);
    Name = "frmExperimentResults";
    StartPosition = FormStartPosition.CenterParent;
    Text = "Experiment Results Viewer";

    filterPanel.ResumeLayout(false);
    filterPanel.PerformLayout();
    ((System.ComponentModel.ISupportInitialize)_dgvResults).EndInit();
    screenshotPanel.ResumeLayout(false);
    ((System.ComponentModel.ISupportInitialize)_picInitial).EndInit();
    ((System.ComponentModel.ISupportInitialize)_picPath).EndInit();
    ((System.ComponentModel.ISupportInitialize)_picCompleted).EndInit();
    bottomPanel.ResumeLayout(false);
    bottomPanel.PerformLayout();
    ResumeLayout(false);
}

        private Button CreateButton(string text, int x, int width)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, 0);
            btn.Size = new Size(width, 28);
            btn.FlatStyle = FlatStyle.Flat;
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btn.BackColor = GetButtonColor(text);
            return btn;
        }

        private Color GetButtonColor(string buttonText)
        {
            switch (buttonText)
            {
                case "CSV":
                case "Excel":
                case "PDF":
                    return Color.FromArgb(52, 152, 219);
                case "Save Replay":
                case "Play Replay":
                    return Color.FromArgb(52, 152, 219);
                case "Advanced Stats":
                    return Color.FromArgb(155, 89, 182);
                case "New Experiment":
                    return Color.FromArgb(46, 204, 113);
                case "Close":
                    return Color.FromArgb(231, 76, 60);
                default:
                    return Color.FromArgb(52, 73, 94);
            }
        }
        #endregion

        private Label lblAlgorithm;
        private Label lblMetric;
        private Label lblResult;
        private Label lblSearch;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private Label lblScreenshotTitle;
        private Panel btnPanel;
        private Panel filterPanel;
        private Panel screenshotPanel;
        private Label lblInitial;
        private Label lblPath;
        private Label lblCompleted;
        private Panel bottomPanel;
    }
}
#region File Header
/// <summary>
/// File: frmExperimentViewer.Designer.cs
/// Description: Designer file for experiment viewer form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentViewer
{
    partial class frmExperimentViewer
    {
        #region Private Fields - UI Components
        private System.ComponentModel.IContainer _components;
        private System.Windows.Forms.DataGridView _dgvExperiments;
        private System.Windows.Forms.ComboBox _cboAlgorithmFilter;
        private System.Windows.Forms.DateTimePicker _dtpFrom;
        private System.Windows.Forms.DateTimePicker _dtpTo;
        private System.Windows.Forms.ComboBox _cboGoalsFilter;
        private System.Windows.Forms.CheckBox _chkSuccessOnly;
        private System.Windows.Forms.TextBox _txtSearch;
        private System.Windows.Forms.Button _btnRefresh;
        private System.Windows.Forms.Button _btnExportSelected;
        private System.Windows.Forms.Button _btnExportAll;
        private System.Windows.Forms.Button _btnDeleteSelected;
        private System.Windows.Forms.Button _btnViewDetails;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.Label _lblStats; 
        private System.Windows.Forms.Panel _filterPanel;
        private System.Windows.Forms.Panel _bottomPanel;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _tsStatus;


        #endregion

        #region Constructor
        public frmExperimentViewer()
        {
            InitializeComponent();
        }
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
            this._lblStatus = new System.Windows.Forms.Label();
            this._lblStatus.Text = "Ready";
            this._lblStatus.Dock = DockStyle.Fill;
            this._lblStatus.ForeColor = System.Drawing.Color.White;
            this._lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            this._lblStatus.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this._dgvExperiments = new System.Windows.Forms.DataGridView();

            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            this._dgvExperiments = new System.Windows.Forms.DataGridView();

            _filterPanel = new Panel();
            lblAlgorithm = new Label();
            _cboAlgorithmFilter = new ComboBox();
            lblFrom = new Label();
            _dtpFrom = new DateTimePicker();
            lblTo = new Label();
            _dtpTo = new DateTimePicker();
            lblGoals = new Label();
            _cboGoalsFilter = new ComboBox();
            _chkSuccessOnly = new CheckBox();
            lblSearch = new Label();
            _txtSearch = new TextBox();
            _btnRefresh = new Button();
            _dgvExperiments = new DataGridView();
            _bottomPanel = new Panel();
            _lblStats = new Label();
            btnPanel = new Panel();
            _btnExportSelected = new Button();
            _btnExportAll = new Button();
            _btnDeleteSelected = new Button();
            _btnViewDetails = new Button();
            _btnClose = new Button();
            _statusStrip = new StatusStrip();
            _tsStatus = new ToolStripStatusLabel();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn9 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn10 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn11 = new DataGridViewTextBoxColumn();
            _filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_dgvExperiments).BeginInit();
            _bottomPanel.SuspendLayout();
            btnPanel.SuspendLayout();
            _statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // _filterPanel
            // 
            _filterPanel.BackColor = Color.FromArgb(240, 242, 245);
            _filterPanel.Controls.Add(lblAlgorithm);
            _filterPanel.Controls.Add(_cboAlgorithmFilter);
            _filterPanel.Controls.Add(lblFrom);
            _filterPanel.Controls.Add(_dtpFrom);
            _filterPanel.Controls.Add(lblTo);
            _filterPanel.Controls.Add(_dtpTo);
            _filterPanel.Controls.Add(lblGoals);
            _filterPanel.Controls.Add(_cboGoalsFilter);
            _filterPanel.Controls.Add(_chkSuccessOnly);
            _filterPanel.Controls.Add(lblSearch);
            _filterPanel.Controls.Add(_txtSearch);
            _filterPanel.Controls.Add(_btnRefresh);
            _filterPanel.Dock = DockStyle.Top;
            _filterPanel.Location = new Point(0, 0);
            _filterPanel.Name = "_filterPanel";
            _filterPanel.Padding = new Padding(10);
            _filterPanel.Size = new Size(884, 80);
            _filterPanel.TabIndex = 1;
            // 
            // lblAlgorithm
            // 
            lblAlgorithm.Location = new Point(10, 15);
            lblAlgorithm.Name = "lblAlgorithm";
            lblAlgorithm.Size = new Size(70, 20);
            lblAlgorithm.TabIndex = 0;
            lblAlgorithm.Text = "Algorithm:";
            // 
            // _cboAlgorithmFilter
            // 
            _cboAlgorithmFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboAlgorithmFilter.Items.AddRange(new object[] { "All Algorithms" });
            _cboAlgorithmFilter.Location = new Point(80, 12);
            _cboAlgorithmFilter.Name = "_cboAlgorithmFilter";
            _cboAlgorithmFilter.Size = new Size(120, 23);
            _cboAlgorithmFilter.TabIndex = 1;
            // 
            // lblFrom
            // 
            lblFrom.Location = new Point(220, 15);
            lblFrom.Name = "lblFrom";
            lblFrom.Size = new Size(40, 20);
            lblFrom.TabIndex = 2;
            lblFrom.Text = "From:";
            // 
            // _dtpFrom
            // 
            _dtpFrom.Format = DateTimePickerFormat.Short;
            _dtpFrom.Location = new Point(260, 12);
            _dtpFrom.Name = "_dtpFrom";
            _dtpFrom.Size = new Size(120, 23);
            _dtpFrom.TabIndex = 3;
            _dtpFrom.Value = new DateTime(2026, 3, 9, 0, 10, 42, 217);
            // 
            // lblTo
            // 
            lblTo.Location = new Point(400, 15);
            lblTo.Name = "lblTo";
            lblTo.Size = new Size(30, 20);
            lblTo.TabIndex = 4;
            lblTo.Text = "To:";
            // 
            // _dtpTo
            // 
            _dtpTo.Format = DateTimePickerFormat.Short;
            _dtpTo.Location = new Point(430, 12);
            _dtpTo.Name = "_dtpTo";
            _dtpTo.Size = new Size(120, 23);
            _dtpTo.TabIndex = 5;
            _dtpTo.Value = new DateTime(2026, 4, 8, 0, 10, 42, 221);
            // 
            // lblGoals
            // 
            lblGoals.Location = new Point(570, 15);
            lblGoals.Name = "lblGoals";
            lblGoals.Size = new Size(40, 20);
            lblGoals.TabIndex = 6;
            lblGoals.Text = "Goals:";
            // 
            // _cboGoalsFilter
            // 
            _cboGoalsFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboGoalsFilter.Items.AddRange(new object[] { "All" });
            _cboGoalsFilter.Location = new Point(610, 12);
            _cboGoalsFilter.Name = "_cboGoalsFilter";
            _cboGoalsFilter.Size = new Size(80, 23);
            _cboGoalsFilter.TabIndex = 7;
            // 
            // _chkSuccessOnly
            // 
            _chkSuccessOnly.Location = new Point(710, 13);
            _chkSuccessOnly.Name = "_chkSuccessOnly";
            _chkSuccessOnly.Size = new Size(90, 20);
            _chkSuccessOnly.TabIndex = 8;
            _chkSuccessOnly.Text = "Success Only";
            // 
            // lblSearch
            // 
            lblSearch.Location = new Point(10, 45);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(50, 20);
            lblSearch.TabIndex = 9;
            lblSearch.Text = "Search:";
            // 
            // _txtSearch
            // 
            _txtSearch.Location = new Point(60, 42);
            _txtSearch.Name = "_txtSearch";
            _txtSearch.Size = new Size(200, 23);
            _txtSearch.TabIndex = 10;
            // 
            // _btnRefresh
            // 
            _btnRefresh.BackColor = Color.FromArgb(52, 152, 219);
            _btnRefresh.FlatStyle = FlatStyle.Flat;
            _btnRefresh.ForeColor = Color.White;
            _btnRefresh.Location = new Point(280, 40);
            _btnRefresh.Name = "_btnRefresh";
            _btnRefresh.Size = new Size(80, 25);
            _btnRefresh.TabIndex = 11;
            _btnRefresh.Text = "Refresh";
            _btnRefresh.UseVisualStyleBackColor = false;
            // 
            // _dgvExperiments
            // 
            _dgvExperiments.AllowUserToAddRows = false;
            _dgvExperiments.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(248, 249, 250);
            _dgvExperiments.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            _dgvExperiments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            _dgvExperiments.BackgroundColor = Color.White;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(52, 73, 94);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            _dgvExperiments.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            _dgvExperiments.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5, dataGridViewTextBoxColumn6, dataGridViewTextBoxColumn7, dataGridViewTextBoxColumn8, dataGridViewTextBoxColumn9, dataGridViewTextBoxColumn10, dataGridViewTextBoxColumn11 });
            _dgvExperiments.Dock = DockStyle.Fill;
            _dgvExperiments.EnableHeadersVisualStyles = false;
            _dgvExperiments.Location = new Point(0, 80);
            _dgvExperiments.Name = "_dgvExperiments";
            _dgvExperiments.ReadOnly = true;
            _dgvExperiments.RowHeadersVisible = false;
            _dgvExperiments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvExperiments.Size = new Size(884, 489);
            _dgvExperiments.TabIndex = 0;
            // 
            // _bottomPanel
            // 
            _bottomPanel.BackColor = Color.FromArgb(240, 242, 245);
            _bottomPanel.Controls.Add(_lblStats);
            _bottomPanel.Controls.Add(btnPanel);
            _bottomPanel.Dock = DockStyle.Bottom;
            _bottomPanel.Location = new Point(0, 569);
            _bottomPanel.Name = "_bottomPanel";
            _bottomPanel.Padding = new Padding(10);
            _bottomPanel.Size = new Size(884, 70);
            _bottomPanel.TabIndex = 2;
            // 
            // _lblStats
            // 
            _lblStats.AutoSize = true;
            _lblStats.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblStats.ForeColor = Color.FromArgb(52, 73, 94);
            _lblStats.Location = new Point(10, 10);
            _lblStats.Name = "_lblStats";
            _lblStats.Size = new Size(0, 15);
            _lblStats.TabIndex = 0;
            // 
            // btnPanel
            // 
            btnPanel.Controls.Add(_btnExportSelected);
            btnPanel.Controls.Add(_btnExportAll);
            btnPanel.Controls.Add(_btnDeleteSelected);
            btnPanel.Controls.Add(_btnViewDetails);
            btnPanel.Controls.Add(_btnClose);
            btnPanel.Location = new Point(10, 28);
            btnPanel.Name = "btnPanel";
            btnPanel.Size = new Size(600, 35);
            btnPanel.TabIndex = 1;
            // 
            // _btnExportSelected
            // 
            _btnExportSelected.BackColor = Color.FromArgb(46, 204, 113);
            _btnExportSelected.FlatStyle = FlatStyle.Flat;
            _btnExportSelected.ForeColor = Color.White;
            _btnExportSelected.Location = new Point(0, 0);
            _btnExportSelected.Name = "_btnExportSelected";
            _btnExportSelected.Size = new Size(110, 28);
            _btnExportSelected.TabIndex = 0;
            _btnExportSelected.Text = "Export Selected";
            _btnExportSelected.UseVisualStyleBackColor = false;
            // 
            // _btnExportAll
            // 
            _btnExportAll.BackColor = Color.FromArgb(52, 152, 219);
            _btnExportAll.FlatStyle = FlatStyle.Flat;
            _btnExportAll.ForeColor = Color.White;
            _btnExportAll.Location = new Point(115, 0);
            _btnExportAll.Name = "_btnExportAll";
            _btnExportAll.Size = new Size(100, 28);
            _btnExportAll.TabIndex = 1;
            _btnExportAll.Text = "Export All";
            _btnExportAll.UseVisualStyleBackColor = false;
            // 
            // _btnDeleteSelected
            // 
            _btnDeleteSelected.BackColor = Color.FromArgb(231, 76, 60);
            _btnDeleteSelected.FlatStyle = FlatStyle.Flat;
            _btnDeleteSelected.ForeColor = Color.White;
            _btnDeleteSelected.Location = new Point(220, 0);
            _btnDeleteSelected.Name = "_btnDeleteSelected";
            _btnDeleteSelected.Size = new Size(110, 28);
            _btnDeleteSelected.TabIndex = 2;
            _btnDeleteSelected.Text = "Delete Selected";
            _btnDeleteSelected.UseVisualStyleBackColor = false;
            // 
            // _btnViewDetails
            // 
            _btnViewDetails.BackColor = Color.FromArgb(155, 89, 182);
            _btnViewDetails.FlatStyle = FlatStyle.Flat;
            _btnViewDetails.ForeColor = Color.White;
            _btnViewDetails.Location = new Point(335, 0);
            _btnViewDetails.Name = "_btnViewDetails";
            _btnViewDetails.Size = new Size(100, 28);
            _btnViewDetails.TabIndex = 3;
            _btnViewDetails.Text = "View Details";
            _btnViewDetails.UseVisualStyleBackColor = false;
            // 
            // _btnClose
            // 
            _btnClose.BackColor = Color.FromArgb(149, 165, 166);
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.ForeColor = Color.White;
            _btnClose.Location = new Point(440, 0);
            _btnClose.Name = "_btnClose";
            _btnClose.Size = new Size(100, 28);
            _btnClose.TabIndex = 4;
            _btnClose.Text = "Close";
            _btnClose.UseVisualStyleBackColor = false;
            // 
            // _statusStrip
            // 
            _statusStrip.Items.AddRange(new ToolStripItem[] { _tsStatus });
            _statusStrip.Location = new Point(0, 639);
            _statusStrip.Name = "_statusStrip";
            _statusStrip.Size = new Size(884, 22);
            _statusStrip.TabIndex = 3;
            // 
            // _tsStatus
            // 
            _tsStatus.Name = "_tsStatus";
            _tsStatus.Size = new Size(39, 17);
            _tsStatus.Text = "Ready";
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "ID";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            dataGridViewTextBoxColumn1.Width = 45;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Date";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            dataGridViewTextBoxColumn2.Width = 59;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Name";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            dataGridViewTextBoxColumn3.Width = 65;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.HeaderText = "Map";
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.ReadOnly = true;
            dataGridViewTextBoxColumn4.Width = 56;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.HeaderText = "Algorithm";
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.ReadOnly = true;
            dataGridViewTextBoxColumn5.Width = 88;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewTextBoxColumn6.HeaderText = "Metric";
            dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            dataGridViewTextBoxColumn6.ReadOnly = true;
            dataGridViewTextBoxColumn6.Width = 69;
            // 
            // dataGridViewTextBoxColumn7
            // 
            dataGridViewTextBoxColumn7.HeaderText = "Goals";
            dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            dataGridViewTextBoxColumn7.ReadOnly = true;
            dataGridViewTextBoxColumn7.Width = 62;
            // 
            // dataGridViewTextBoxColumn8
            // 
            dataGridViewTextBoxColumn8.HeaderText = "Time (ms)";
            dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            dataGridViewTextBoxColumn8.ReadOnly = true;
            dataGridViewTextBoxColumn8.Width = 87;
            // 
            // dataGridViewTextBoxColumn9
            // 
            dataGridViewTextBoxColumn9.HeaderText = "Length";
            dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            dataGridViewTextBoxColumn9.ReadOnly = true;
            dataGridViewTextBoxColumn9.Width = 71;
            // 
            // dataGridViewTextBoxColumn10
            // 
            dataGridViewTextBoxColumn10.HeaderText = "Collisions";
            dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            dataGridViewTextBoxColumn10.ReadOnly = true;
            dataGridViewTextBoxColumn10.Width = 82;
            // 
            // dataGridViewTextBoxColumn11
            // 
            dataGridViewTextBoxColumn11.HeaderText = "✓";
            dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            dataGridViewTextBoxColumn11.ReadOnly = true;
            dataGridViewTextBoxColumn11.Width = 42;
            // 
            // frmExperimentViewer
            // 
            BackColor = Color.White;
            ClientSize = new Size(884, 661);
            Controls.Add(_dgvExperiments);
            Controls.Add(_filterPanel);
            Controls.Add(_bottomPanel);
            Controls.Add(_statusStrip);
            MinimumSize = new Size(900, 500);
            Name = "frmExperimentViewer";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Experiment Viewer - Saved Experiments";
            _filterPanel.ResumeLayout(false);
            _filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_dgvExperiments).EndInit();
            _bottomPanel.ResumeLayout(false);
            _bottomPanel.PerformLayout();
            btnPanel.ResumeLayout(false);
            _statusStrip.ResumeLayout(false);
            _statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private Label lblAlgorithm;
        private Label lblFrom;
        private Label lblTo;
        private Label lblGoals;
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
        private System.Windows.Forms.Label _lblStatus;

        private Panel btnPanel;
    }
}
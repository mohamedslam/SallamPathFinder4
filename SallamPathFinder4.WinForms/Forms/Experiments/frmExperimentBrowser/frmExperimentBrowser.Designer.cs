#region File Header
/// <summary>
/// File: frmExperimentBrowser.Designer.cs
/// Description: Designer file for experiment browser form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-09
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentBrowser
{
    partial class frmExperimentBrowser
    {
        #region Private Fields - UI Components
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.Label lblFoldersTitle;
        private System.Windows.Forms.CheckedListBox checkedListFolders;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Button btnCompareSelected;
        private System.Windows.Forms.Button btnDeleteSelected;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblResultsTitle;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.Label lblStats;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.Button btnViewDetails;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlgorithm;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTimeMs;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPathLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSuccess;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCollisions;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Initialize Component
        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.lblFoldersTitle = new System.Windows.Forms.Label();
            this.checkedListFolders = new System.Windows.Forms.CheckedListBox();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.btnCompareSelected = new System.Windows.Forms.Button();
            this.btnDeleteSelected = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblResultsTitle = new System.Windows.Forms.Label();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.colAlgorithm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTimeMs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPathLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSuccess = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCollisions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblStats = new System.Windows.Forms.Label();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.btnViewDetails = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.rightPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();

            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.leftPanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.rightPanel);
            this.splitContainer.Size = new System.Drawing.Size(1030, 650);
            this.splitContainer.SplitterDistance = 250;
            this.splitContainer.TabIndex = 0;

            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.checkedListFolders);
            this.leftPanel.Controls.Add(this.lblFoldersTitle);
            this.leftPanel.Controls.Add(this.btnSelectAll);
            this.leftPanel.Controls.Add(this.btnClearAll);
            this.leftPanel.Controls.Add(this.btnCompareSelected);
            this.leftPanel.Controls.Add(this.btnDeleteSelected);
            this.leftPanel.Controls.Add(this.btnRefresh);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Padding = new System.Windows.Forms.Padding(5);
            this.leftPanel.Size = new System.Drawing.Size(250, 650);
            this.leftPanel.TabIndex = 0;

            // 
            // lblFoldersTitle
            // 
            this.lblFoldersTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblFoldersTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblFoldersTitle.Location = new System.Drawing.Point(5, 5);
            this.lblFoldersTitle.Name = "lblFoldersTitle";
            this.lblFoldersTitle.Size = new System.Drawing.Size(240, 30);
            this.lblFoldersTitle.TabIndex = 0;
            this.lblFoldersTitle.Text = "📁 Experiment Folders";
            this.lblFoldersTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // checkedListFolders
            // 
            this.checkedListFolders.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkedListFolders.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkedListFolders.FormattingEnabled = true;
            this.checkedListFolders.IntegralHeight = false;
            this.checkedListFolders.Location = new System.Drawing.Point(5, 35);
            this.checkedListFolders.Name = "checkedListFolders";
            this.checkedListFolders.Size = new System.Drawing.Size(240, 450);
            this.checkedListFolders.TabIndex = 1;

            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectAll.Location = new System.Drawing.Point(5, 485);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(240, 30);
            this.btnSelectAll.TabIndex = 2;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;

            // 
            // btnClearAll
            // 
            this.btnClearAll.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnClearAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearAll.Location = new System.Drawing.Point(5, 515);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(240, 30);
            this.btnClearAll.TabIndex = 3;
            this.btnClearAll.Text = "Clear All";
            this.btnClearAll.UseVisualStyleBackColor = true;

            // 
            // btnCompareSelected
            // 
            this.btnCompareSelected.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnCompareSelected.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnCompareSelected.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCompareSelected.ForeColor = System.Drawing.Color.White;
            this.btnCompareSelected.Location = new System.Drawing.Point(5, 545);
            this.btnCompareSelected.Name = "btnCompareSelected";
            this.btnCompareSelected.Size = new System.Drawing.Size(240, 35);
            this.btnCompareSelected.TabIndex = 4;
            this.btnCompareSelected.Text = "Compare Selected";
            this.btnCompareSelected.UseVisualStyleBackColor = false;

            // 
            // btnDeleteSelected
            // 
            this.btnDeleteSelected.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnDeleteSelected.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnDeleteSelected.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteSelected.ForeColor = System.Drawing.Color.White;
            this.btnDeleteSelected.Location = new System.Drawing.Point(5, 580);
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.btnDeleteSelected.Size = new System.Drawing.Size(240, 30);
            this.btnDeleteSelected.TabIndex = 5;
            this.btnDeleteSelected.Text = "Delete Selected";
            this.btnDeleteSelected.UseVisualStyleBackColor = false;

            // 
            // btnRefresh
            // 
            this.btnRefresh.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Location = new System.Drawing.Point(5, 610);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(240, 30);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;

            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add(this.dgvResults);
            this.rightPanel.Controls.Add(this.lblResultsTitle);
            this.rightPanel.Controls.Add(this.lblStats);
            this.rightPanel.Controls.Add(this.btnExportCsv);
            this.rightPanel.Controls.Add(this.btnViewDetails);
            this.rightPanel.Controls.Add(this.btnClose);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(0, 0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Padding = new System.Windows.Forms.Padding(5);
            this.rightPanel.Size = new System.Drawing.Size(775, 650);
            this.rightPanel.TabIndex = 1;

            // 
            // lblResultsTitle
            // 
            this.lblResultsTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResultsTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblResultsTitle.Location = new System.Drawing.Point(5, 5);
            this.lblResultsTitle.Name = "lblResultsTitle";
            this.lblResultsTitle.Size = new System.Drawing.Size(765, 30);
            this.lblResultsTitle.TabIndex = 0;
            this.lblResultsTitle.Text = "📊 Comparison Results";
            this.lblResultsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // dgvResults
            // 
            this.dgvResults.AllowUserToAddRows = false;
            this.dgvResults.AllowUserToDeleteRows = false;
            this.dgvResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvResults.BackgroundColor = System.Drawing.Color.White;
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colAlgorithm,
                this.colTimeMs,
                this.colPathLength,
                this.colSuccess,
                this.colCollisions,
                this.colDate});
            this.dgvResults.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvResults.Location = new System.Drawing.Point(5, 35);
            this.dgvResults.MultiSelect = false;
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.ReadOnly = true;
            this.dgvResults.RowHeadersVisible = false;
            this.dgvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResults.Size = new System.Drawing.Size(765, 450);
            this.dgvResults.TabIndex = 1;

            // 
            // colAlgorithm
            // 
            this.colAlgorithm.HeaderText = "Algorithm";
            this.colAlgorithm.Name = "colAlgorithm";
            this.colAlgorithm.ReadOnly = true;
            this.colAlgorithm.Width = 86;

            // 
            // colTimeMs
            // 
            this.colTimeMs.HeaderText = "Time (ms)";
            this.colTimeMs.Name = "colTimeMs";
            this.colTimeMs.ReadOnly = true;
            this.colTimeMs.Width = 85;

            // 
            // colPathLength
            // 
            this.colPathLength.HeaderText = "Path Length";
            this.colPathLength.Name = "colPathLength";
            this.colPathLength.ReadOnly = true;
            this.colPathLength.Width = 96;

            // 
            // colSuccess
            // 
            this.colSuccess.HeaderText = "Success";
            this.colSuccess.Name = "colSuccess";
            this.colSuccess.ReadOnly = true;
            this.colSuccess.Width = 73;

            // 
            // colCollisions
            // 
            this.colCollisions.HeaderText = "Collisions";
            this.colCollisions.Name = "colCollisions";
            this.colCollisions.ReadOnly = true;
            this.colCollisions.Width = 83;

            // 
            // colDate
            // 
            this.colDate.HeaderText = "Date";
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            this.colDate.Width = 56;

            // 
            // lblStats
            // 
            this.lblStats.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStats.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStats.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblStats.Location = new System.Drawing.Point(5, 585);
            this.lblStats.Name = "lblStats";
            this.lblStats.Size = new System.Drawing.Size(765, 40);
            this.lblStats.TabIndex = 2;
            this.lblStats.Text = "Ready";
            this.lblStats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // btnExportCsv
            // 
            this.btnExportCsv.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnExportCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportCsv.ForeColor = System.Drawing.Color.White;
            this.btnExportCsv.Location = new System.Drawing.Point(5, 490);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(120, 30);
            this.btnExportCsv.TabIndex = 3;
            this.btnExportCsv.Text = "Export CSV";
            this.btnExportCsv.UseVisualStyleBackColor = false;

            // 
            // btnViewDetails
            // 
            this.btnViewDetails.BackColor = System.Drawing.Color.FromArgb(155, 89, 182);
            this.btnViewDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewDetails.ForeColor = System.Drawing.Color.White;
            this.btnViewDetails.Location = new System.Drawing.Point(130, 490);
            this.btnViewDetails.Name = "btnViewDetails";
            this.btnViewDetails.Size = new System.Drawing.Size(120, 30);
            this.btnViewDetails.TabIndex = 4;
            this.btnViewDetails.Text = "View Details";
            this.btnViewDetails.UseVisualStyleBackColor = false;

            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(260, 490);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 30);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;

            // 
            // frmExperimentBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1030, 650);
            this.Controls.Add(this.splitContainer);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "frmExperimentBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Experiment Browser";

            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);
        }
        #endregion
    }
}
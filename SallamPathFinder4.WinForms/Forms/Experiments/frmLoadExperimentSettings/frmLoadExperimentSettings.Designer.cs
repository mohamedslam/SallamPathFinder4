#region File Header
/// <summary>
/// File: frmLoadExperimentSettings.Designer.cs
/// Description: Designer file for load experiment settings dialog
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmLoadExperimentSettings
{
    partial class frmLoadExperimentSettings
    {
        #region Private Fields - UI Components
        private System.ComponentModel.IContainer _components;
        private System.Windows.Forms.ListBox _lstSettings;
        private System.Windows.Forms.TextBox _txtPreview;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.Button _btnLoad;
        private System.Windows.Forms.Button _btnDelete;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Button _btnRefresh;
        private System.Windows.Forms.Label _lblTitle;
        private System.Windows.Forms.Label _lblPreview;
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
            _lblTitle = new Label();
            _lstSettings = new ListBox();
            _lblPreview = new Label();
            _txtPreview = new TextBox();
            _lblStatus = new Label();
            _btnRefresh = new Button();
            _btnLoad = new Button();
            _btnDelete = new Button();
            _btnCancel = new Button();
            SuspendLayout();
            // 
            // _lblTitle
            // 
            _lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            _lblTitle.ForeColor = Color.FromArgb(52, 73, 94);
            _lblTitle.Location = new Point(20, 15);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new Size(200, 25);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "Saved Experiment Settings";
            // 
            // _lstSettings
            // 
            _lstSettings.Font = new Font("Segoe UI", 10F);
            _lstSettings.ItemHeight = 17;
            _lstSettings.Location = new Point(20, 50);
            _lstSettings.Name = "_lstSettings";
            _lstSettings.Size = new Size(250, 344);
            _lstSettings.TabIndex = 1;
            // 
            // _lblPreview
            // 
            _lblPreview.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblPreview.Location = new Point(290, 20);
            _lblPreview.Name = "_lblPreview";
            _lblPreview.Size = new Size(60, 20);
            _lblPreview.TabIndex = 2;
            _lblPreview.Text = "Preview:";
            // 
            // _txtPreview
            // 
            _txtPreview.BackColor = Color.FromArgb(248, 249, 250);
            _txtPreview.Location = new Point(290, 50);
            _txtPreview.Multiline = true;
            _txtPreview.Name = "_txtPreview";
            _txtPreview.ReadOnly = true;
            _txtPreview.Size = new Size(320, 344);
            _txtPreview.TabIndex = 3;
            // 
            // _lblStatus
            // 
            _lblStatus.ForeColor = Color.Gray;
            _lblStatus.Location = new Point(20, 410);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new Size(300, 20);
            _lblStatus.TabIndex = 4;
            _lblStatus.Text = "Ready";
            // 
            // _btnRefresh
            // 
            _btnRefresh.BackColor = Color.FromArgb(52, 152, 219);
            _btnRefresh.Cursor = Cursors.Hand;
            _btnRefresh.FlatStyle = FlatStyle.Flat;
            _btnRefresh.ForeColor = Color.White;
            _btnRefresh.Location = new Point(20, 435);
            _btnRefresh.Name = "_btnRefresh";
            _btnRefresh.Size = new Size(100, 30);
            _btnRefresh.TabIndex = 5;
            _btnRefresh.Text = "Refresh";
            _btnRefresh.UseVisualStyleBackColor = false;
            // 
            // _btnLoad
            // 
            _btnLoad.BackColor = Color.FromArgb(46, 204, 113);
            _btnLoad.Cursor = Cursors.Hand;
            _btnLoad.Enabled = false;
            _btnLoad.FlatStyle = FlatStyle.Flat;
            _btnLoad.ForeColor = Color.White;
            _btnLoad.Location = new Point(130, 435);
            _btnLoad.Name = "_btnLoad";
            _btnLoad.Size = new Size(100, 30);
            _btnLoad.TabIndex = 6;
            _btnLoad.Text = "Load";
            _btnLoad.UseVisualStyleBackColor = false;
            // 
            // _btnDelete
            // 
            _btnDelete.BackColor = Color.FromArgb(231, 76, 60);
            _btnDelete.Cursor = Cursors.Hand;
            _btnDelete.Enabled = false;
            _btnDelete.FlatStyle = FlatStyle.Flat;
            _btnDelete.ForeColor = Color.White;
            _btnDelete.Location = new Point(240, 435);
            _btnDelete.Name = "_btnDelete";
            _btnDelete.Size = new Size(100, 30);
            _btnDelete.TabIndex = 7;
            _btnDelete.Text = "Delete";
            _btnDelete.UseVisualStyleBackColor = false;
            // 
            // _btnCancel
            // 
            _btnCancel.BackColor = Color.FromArgb(149, 165, 166);
            _btnCancel.Cursor = Cursors.Hand;
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.ForeColor = Color.White;
            _btnCancel.Location = new Point(510, 435);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(100, 30);
            _btnCancel.TabIndex = 8;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // frmLoadExperimentSettings
            // 
            BackColor = Color.White;
            ClientSize = new Size(634, 474);
            Controls.Add(_lblTitle);
            Controls.Add(_lstSettings);
            Controls.Add(_lblPreview);
            Controls.Add(_txtPreview);
            Controls.Add(_lblStatus);
            Controls.Add(_btnRefresh);
            Controls.Add(_btnLoad);
            Controls.Add(_btnDelete);
            Controls.Add(_btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimumSize = new Size(650, 500);
            Name = "frmLoadExperimentSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Load Experiment Settings";
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion
    }
}
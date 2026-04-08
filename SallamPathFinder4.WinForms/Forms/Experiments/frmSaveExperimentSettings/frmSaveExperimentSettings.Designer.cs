#region File Header
/// <summary>
/// File: frmSaveExperimentSettings.Designer.cs
/// Description: Designer file for save experiment settings dialog
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmSaveExperimentSettings
{
    partial class frmSaveExperimentSettings
    {
        #region Private Fields - UI Components
        private System.ComponentModel.IContainer _components;
        private System.Windows.Forms.Label _lblFileName;
        private System.Windows.Forms.TextBox _txtFileName;
        private System.Windows.Forms.Label _lblDescription;
        private System.Windows.Forms.TextBox _txtDescription;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Button _btnCancel;
        #endregion

        #region Constructor
        public frmSaveExperimentSettings()
        {
            InitializeComponent();
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Clean up any resources being used
        /// </summary>
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
        /// <summary>
        /// Required method for Designer support - do not modify
        /// </summary>
        private void InitializeComponent()
        {
            _components = new System.ComponentModel.Container();

            // Form Settings
            this.Text = "Save Experiment Settings";
            this.Size = new System.Drawing.Size(450, 220);
            this.MinimumSize = new System.Drawing.Size(450, 220);
            this.MaximumSize = new System.Drawing.Size(450, 220);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.White;

            // File Name Label
            _lblFileName = new System.Windows.Forms.Label();
            _lblFileName.Text = "File Name:";
            _lblFileName.Location = new System.Drawing.Point(20, 20);
            _lblFileName.Size = new System.Drawing.Size(70, 23);
            _lblFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // File Name TextBox
            _txtFileName = new System.Windows.Forms.TextBox();
            _txtFileName.Location = new System.Drawing.Point(95, 20);
            _txtFileName.Size = new System.Drawing.Size(310, 23);
            _txtFileName.TabIndex = 1;

            // Description Label
            _lblDescription = new System.Windows.Forms.Label();
            _lblDescription.Text = "Description:";
            _lblDescription.Location = new System.Drawing.Point(20, 55);
            _lblDescription.Size = new System.Drawing.Size(70, 23);
            _lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // Description TextBox (multiline)
            _txtDescription = new System.Windows.Forms.TextBox();
            _txtDescription.Location = new System.Drawing.Point(95, 55);
            _txtDescription.Size = new System.Drawing.Size(310, 80);
            _txtDescription.Multiline = true;
            _txtDescription.TabIndex = 2;

            // Save Button
            _btnSave = new System.Windows.Forms.Button();
            _btnSave.Text = "Save";
            _btnSave.Location = new System.Drawing.Point(95, 150);
            _btnSave.Size = new System.Drawing.Size(100, 30);
            _btnSave.FlatStyle = FlatStyle.Flat;
            _btnSave.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            _btnSave.ForeColor = System.Drawing.Color.White;
            _btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            _btnSave.TabIndex = 3;

            // Cancel Button
            _btnCancel = new System.Windows.Forms.Button();
            _btnCancel.Text = "Cancel";
            _btnCancel.Location = new System.Drawing.Point(205, 150);
            _btnCancel.Size = new System.Drawing.Size(100, 30);
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            _btnCancel.ForeColor = System.Drawing.Color.White;
            _btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            _btnCancel.TabIndex = 4;

            // Add controls to form
            this.Controls.Add(_lblFileName);
            this.Controls.Add(_txtFileName);
            this.Controls.Add(_lblDescription);
            this.Controls.Add(_txtDescription);
            this.Controls.Add(_btnSave);
            this.Controls.Add(_btnCancel);
        }
        #endregion
    }
}
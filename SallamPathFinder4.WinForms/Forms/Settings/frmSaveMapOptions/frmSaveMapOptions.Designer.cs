#region File Header
/// <summary>
/// File: frmSaveMapOptions.Designer.cs
/// Description: Designer file for save map options dialog
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Settings.frmSaveMapOptions
{
    partial class frmSaveMapOptions
    {
        #region Private Fields - UI Components
        private System.ComponentModel.IContainer _components;
        private System.Windows.Forms.CheckBox _chkMap;
        private System.Windows.Forms.CheckBox _chkElements;
        private System.Windows.Forms.CheckBox _chkParking;
        private System.Windows.Forms.CheckBox _chkGoals;
        private System.Windows.Forms.CheckBox _chkObstacles;
        private System.Windows.Forms.CheckBox _chkPath;
        private System.Windows.Forms.CheckBox _chkRobot;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Label _lblTitle;
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
            _components = new System.ComponentModel.Container();

            // Form Settings
            this.Text = "Save Map Options";
            this.Size = new System.Drawing.Size(320, 350);
            this.MinimumSize = new System.Drawing.Size(320, 350);
            this.MaximumSize = new System.Drawing.Size(320, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.White;

            int y = 15;

            // Title
            _lblTitle = new System.Windows.Forms.Label();
            _lblTitle.Text = "Select items to save:";
            _lblTitle.Location = new System.Drawing.Point(15, y);
            _lblTitle.Size = new System.Drawing.Size(150, 25);
            _lblTitle.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            y += 35;

            // Checkboxes
            _chkMap = new System.Windows.Forms.CheckBox();
            _chkMap.Text = "Map Grid & Surface Weights";
            _chkMap.Location = new System.Drawing.Point(15, y);
            _chkMap.Size = new System.Drawing.Size(200, 23);
            y += 28;

            _chkElements = new System.Windows.Forms.CheckBox();
            _chkElements.Text = "Static Elements (Wall, Door, Window, Ramp)";
            _chkElements.Location = new System.Drawing.Point(15, y);
            _chkElements.Size = new System.Drawing.Size(280, 23);
            y += 28;

            _chkParking = new System.Windows.Forms.CheckBox();
            _chkParking.Text = "Parking Points";
            _chkParking.Location = new System.Drawing.Point(15, y);
            _chkParking.Size = new System.Drawing.Size(150, 23);
            y += 28;

            _chkGoals = new System.Windows.Forms.CheckBox();
            _chkGoals.Text = "Goal Points";
            _chkGoals.Location = new System.Drawing.Point(15, y);
            _chkGoals.Size = new System.Drawing.Size(150, 23);
            y += 28;

            _chkObstacles = new System.Windows.Forms.CheckBox();
            _chkObstacles.Text = "Dynamic Obstacles";
            _chkObstacles.Location = new System.Drawing.Point(15, y);
            _chkObstacles.Size = new System.Drawing.Size(150, 23);
            y += 28;

            _chkPath = new System.Windows.Forms.CheckBox();
            _chkPath.Text = "Current Path";
            _chkPath.Location = new System.Drawing.Point(15, y);
            _chkPath.Size = new System.Drawing.Size(150, 23);
            y += 28;

            _chkRobot = new System.Windows.Forms.CheckBox();
            _chkRobot.Text = "Robot Position & Angle";
            _chkRobot.Location = new System.Drawing.Point(15, y);
            _chkRobot.Size = new System.Drawing.Size(180, 23);
            y += 45;

            // Buttons
            _btnSave = new System.Windows.Forms.Button();
            _btnSave.Text = "Save";
            _btnSave.Location = new System.Drawing.Point(50, y);
            _btnSave.Size = new System.Drawing.Size(90, 35);
            _btnSave.FlatStyle = FlatStyle.Flat;
            _btnSave.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            _btnSave.ForeColor = System.Drawing.Color.White;
            _btnSave.Cursor = Cursors.Hand;

            _btnCancel = new System.Windows.Forms.Button();
            _btnCancel.Text = "Cancel";
            _btnCancel.Location = new System.Drawing.Point(160, y);
            _btnCancel.Size = new System.Drawing.Size(90, 35);
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            _btnCancel.ForeColor = System.Drawing.Color.White;
            _btnCancel.Cursor = Cursors.Hand;

            // Add controls to form
            this.Controls.Add(_lblTitle);
            this.Controls.Add(_chkMap);
            this.Controls.Add(_chkElements);
            this.Controls.Add(_chkParking);
            this.Controls.Add(_chkGoals);
            this.Controls.Add(_chkObstacles);
            this.Controls.Add(_chkPath);
            this.Controls.Add(_chkRobot);
            this.Controls.Add(_btnSave);
            this.Controls.Add(_btnCancel);
        }
        #endregion
    }
}
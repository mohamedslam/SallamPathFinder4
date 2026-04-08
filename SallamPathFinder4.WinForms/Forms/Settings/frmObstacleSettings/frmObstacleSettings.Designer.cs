#region File Header
/// <summary>
/// File: frmObstacleSettings.Designer.cs
/// Description: Designer file for obstacle settings form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Settings.frmObstacleSettings
{
    partial class frmObstacleSettings
    {
        #region Private Fields - UI Components
        private System.ComponentModel.IContainer _components;
        private System.Windows.Forms.DataGridView _dgvTypeSettings;
        private System.Windows.Forms.CheckBox _chkRandomMovement;
        private System.Windows.Forms.CheckBox _chkFollowWaypoints;
        private System.Windows.Forms.CheckBox _chkAvoidRobot;
        private System.Windows.Forms.CheckBox _chkAttractToRobot;
        private System.Windows.Forms.TrackBar trbDirectionChange;
        private System.Windows.Forms.TrackBar trbMaxTurnAngle;
        private System.Windows.Forms.Label lblDirectionValue;
        private System.Windows.Forms.Label lblTurnValue;
        private System.Windows.Forms.ComboBox cboCollisionResponse;
        private System.Windows.Forms.NumericUpDown nudInitialCount;
        private System.Windows.Forms.NumericUpDown nudMaxCount;
        private System.Windows.Forms.NumericUpDown nudSpawnInterval;
        private System.Windows.Forms.CheckBox chkDynamicSpawning;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblTypes;
        private System.Windows.Forms.Label lblDirection;
        private System.Windows.Forms.Label lblTurn;
        private System.Windows.Forms.GroupBox grpMovement;
        private System.Windows.Forms.GroupBox grpCollision;
        private System.Windows.Forms.GroupBox grpSpawning;
        private System.Windows.Forms.Label lblInitial;
        private System.Windows.Forms.Label lblMax;
        private System.Windows.Forms.Label lblInterval;
        #endregion

        #region Constructor
        public frmObstacleSettings()
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
            _components = new System.ComponentModel.Container();

            // Form Settings
            this.Text = "Dynamic Obstacles Settings";
            this.Size = new System.Drawing.Size(700, 650);
            this.MinimumSize = new System.Drawing.Size(700, 650);
            this.MaximumSize = new System.Drawing.Size(700, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.White;

            int y = 15;

            // Types Label
            lblTypes = new System.Windows.Forms.Label();
            lblTypes.Text = "Obstacle Types";
            lblTypes.Location = new System.Drawing.Point(15, y);
            lblTypes.Size = new System.Drawing.Size(150, 25);
            lblTypes.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            y += 30;

            // DataGridView for obstacle types
            _dgvTypeSettings = new System.Windows.Forms.DataGridView();
            _dgvTypeSettings.Location = new System.Drawing.Point(15, y);
            _dgvTypeSettings.Size = new System.Drawing.Size(660, 200);
            _dgvTypeSettings.AllowUserToAddRows = false;
            _dgvTypeSettings.AllowUserToDeleteRows = false;
            _dgvTypeSettings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            _dgvTypeSettings.Columns.Add("Type", "Type");
            _dgvTypeSettings.Columns.Add("Speed", "Speed (cells/s)");
            _dgvTypeSettings.Columns.Add("Randomness", "Randomness (0-1)");
            _dgvTypeSettings.Columns.Add("Radius", "Radius (cells)");
            _dgvTypeSettings.Columns.Add("Weight", "Weight (0-1)");

            _dgvTypeSettings.Columns["Speed"].DefaultCellStyle.Format = "F2";
            _dgvTypeSettings.Columns["Randomness"].DefaultCellStyle.Format = "F2";
            _dgvTypeSettings.Columns["Radius"].DefaultCellStyle.Format = "F2";
            _dgvTypeSettings.Columns["Weight"].DefaultCellStyle.Format = "F2";

            y += 210;

            // Movement Group
            grpMovement = new System.Windows.Forms.GroupBox();
            grpMovement.Text = "Movement Behavior";
            grpMovement.Location = new System.Drawing.Point(15, y);
            grpMovement.Size = new System.Drawing.Size(320, 140);

            int my = 20;
            _chkRandomMovement = new System.Windows.Forms.CheckBox();
            _chkRandomMovement.Text = "Enable Random Movement";
            _chkRandomMovement.Location = new System.Drawing.Point(10, my);
            _chkRandomMovement.Size = new System.Drawing.Size(180, 23);
            _chkRandomMovement.Checked = true;
            my += 28;

            _chkFollowWaypoints = new System.Windows.Forms.CheckBox();
            _chkFollowWaypoints.Text = "Follow Waypoints";
            _chkFollowWaypoints.Location = new System.Drawing.Point(10, my);
            _chkFollowWaypoints.Size = new System.Drawing.Size(150, 23);
            my += 28;

            _chkAvoidRobot = new System.Windows.Forms.CheckBox();
            _chkAvoidRobot.Text = "Avoid Robot";
            _chkAvoidRobot.Location = new System.Drawing.Point(10, my);
            _chkAvoidRobot.Size = new System.Drawing.Size(120, 23);
            my += 28;

            _chkAttractToRobot = new System.Windows.Forms.CheckBox();
            _chkAttractToRobot.Text = "Attract to Robot (Aggressive)";
            _chkAttractToRobot.Location = new System.Drawing.Point(10, my);
            _chkAttractToRobot.Size = new System.Drawing.Size(200, 23);

            grpMovement.Controls.Add(_chkRandomMovement);
            grpMovement.Controls.Add(_chkFollowWaypoints);
            grpMovement.Controls.Add(_chkAvoidRobot);
            grpMovement.Controls.Add(_chkAttractToRobot);

            // Direction Change
            lblDirection = new System.Windows.Forms.Label();
            lblDirection.Text = "Direction Change Probability:";
            lblDirection.Location = new System.Drawing.Point(350, y + 25);
            lblDirection.Size = new System.Drawing.Size(180, 23);

            trbDirectionChange = new System.Windows.Forms.TrackBar();
            trbDirectionChange.Location = new System.Drawing.Point(350, y + 50);
            trbDirectionChange.Size = new System.Drawing.Size(200, 45);
            trbDirectionChange.Minimum = 0;
            trbDirectionChange.Maximum = 100;
            trbDirectionChange.Value = 30;

            lblDirectionValue = new System.Windows.Forms.Label();
            lblDirectionValue.Text = "30%";
            lblDirectionValue.Location = new System.Drawing.Point(560, y + 60);
            lblDirectionValue.Size = new System.Drawing.Size(40, 23);

            // Max Turn Angle
            lblTurn = new System.Windows.Forms.Label();
            lblTurn.Text = "Max Turn Angle:";
            lblTurn.Location = new System.Drawing.Point(350, y + 95);
            lblTurn.Size = new System.Drawing.Size(120, 23);

            trbMaxTurnAngle = new System.Windows.Forms.TrackBar();
            trbMaxTurnAngle.Location = new System.Drawing.Point(350, y + 120);
            trbMaxTurnAngle.Size = new System.Drawing.Size(200, 45);
            trbMaxTurnAngle.Minimum = 0;
            trbMaxTurnAngle.Maximum = 180;
            trbMaxTurnAngle.Value = 90;

            lblTurnValue = new System.Windows.Forms.Label();
            lblTurnValue.Text = "90°";
            lblTurnValue.Location = new System.Drawing.Point(560, y + 130);
            lblTurnValue.Size = new System.Drawing.Size(40, 23);

            y += 160;

            // Collision Response Group
            grpCollision = new System.Windows.Forms.GroupBox();
            grpCollision.Text = "Collision Response";
            grpCollision.Location = new System.Drawing.Point(15, y);
            grpCollision.Size = new System.Drawing.Size(320, 80);

            cboCollisionResponse = new System.Windows.Forms.ComboBox();
            cboCollisionResponse.Location = new System.Drawing.Point(15, 25);
            cboCollisionResponse.Size = new System.Drawing.Size(280, 25);
            cboCollisionResponse.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCollisionResponse.Items.AddRange(new object[] { "Stop Robot", "Reduce Battery", "Wait and Re-route", "Log Only" });
            cboCollisionResponse.SelectedIndex = 3;

            grpCollision.Controls.Add(cboCollisionResponse);

            // Spawning Group
            grpSpawning = new System.Windows.Forms.GroupBox();
            grpSpawning.Text = "Spawning";
            grpSpawning.Location = new System.Drawing.Point(350, y);
            grpSpawning.Size = new System.Drawing.Size(320, 120);

            int sy = 20;
            lblInitial = new System.Windows.Forms.Label();
            lblInitial.Text = "Initial Count:";
            lblInitial.Location = new System.Drawing.Point(15, sy);
            lblInitial.Size = new System.Drawing.Size(80, 23);

            nudInitialCount = new System.Windows.Forms.NumericUpDown();
            nudInitialCount.Location = new System.Drawing.Point(100, sy);
            nudInitialCount.Size = new System.Drawing.Size(80, 23);
            nudInitialCount.Minimum = 0;
            nudInitialCount.Maximum = 50;
            nudInitialCount.Value = 5;
            sy += 30;

            lblMax = new System.Windows.Forms.Label();
            lblMax.Text = "Max Count:";
            lblMax.Location = new System.Drawing.Point(15, sy);
            lblMax.Size = new System.Drawing.Size(80, 23);

            nudMaxCount = new System.Windows.Forms.NumericUpDown();
            nudMaxCount.Location = new System.Drawing.Point(100, sy);
            nudMaxCount.Size = new System.Drawing.Size(80, 23);
            nudMaxCount.Minimum = 1;
            nudMaxCount.Maximum = 100;
            nudMaxCount.Value = 20;
            sy += 30;

            chkDynamicSpawning = new System.Windows.Forms.CheckBox();
            chkDynamicSpawning.Text = "Spawn dynamically during simulation";
            chkDynamicSpawning.Location = new System.Drawing.Point(15, sy);
            chkDynamicSpawning.Size = new System.Drawing.Size(250, 23);
            sy += 30;

            lblInterval = new System.Windows.Forms.Label();
            lblInterval.Text = "Spawn Interval (s):";
            lblInterval.Location = new System.Drawing.Point(15, sy);
            lblInterval.Size = new System.Drawing.Size(100, 23);

            nudSpawnInterval = new System.Windows.Forms.NumericUpDown();
            nudSpawnInterval.Location = new System.Drawing.Point(120, sy);
            nudSpawnInterval.Size = new System.Drawing.Size(80, 23);
            nudSpawnInterval.Minimum = 1;
            nudSpawnInterval.Maximum = 60;
            nudSpawnInterval.Value = 10;
            nudSpawnInterval.DecimalPlaces = 1;

            grpSpawning.Controls.Add(lblInitial);
            grpSpawning.Controls.Add(nudInitialCount);
            grpSpawning.Controls.Add(lblMax);
            grpSpawning.Controls.Add(nudMaxCount);
            grpSpawning.Controls.Add(chkDynamicSpawning);
            grpSpawning.Controls.Add(lblInterval);
            grpSpawning.Controls.Add(nudSpawnInterval);

            y += 140;

            // Buttons
            btnSave = new System.Windows.Forms.Button();
            btnSave.Text = "Save";
            btnSave.Location = new System.Drawing.Point(50, y);
            btnSave.Size = new System.Drawing.Size(100, 35);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = System.Drawing.Color.White;
            btnSave.Cursor = Cursors.Hand;

            btnCancel = new System.Windows.Forms.Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new System.Drawing.Point(170, y);
            btnCancel.Size = new System.Drawing.Size(100, 35);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            btnCancel.ForeColor = System.Drawing.Color.White;
            btnCancel.Cursor = Cursors.Hand;

            btnReset = new System.Windows.Forms.Button();
            btnReset.Text = "Reset to Defaults";
            btnReset.Location = new System.Drawing.Point(290, y);
            btnReset.Size = new System.Drawing.Size(120, 35);
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnReset.ForeColor = System.Drawing.Color.White;
            btnReset.Cursor = Cursors.Hand;

            // Add controls to form
            this.Controls.Add(lblTypes);
            this.Controls.Add(_dgvTypeSettings);
            this.Controls.Add(grpMovement);
            this.Controls.Add(lblDirection);
            this.Controls.Add(trbDirectionChange);
            this.Controls.Add(lblDirectionValue);
            this.Controls.Add(lblTurn);
            this.Controls.Add(trbMaxTurnAngle);
            this.Controls.Add(lblTurnValue);
            this.Controls.Add(grpCollision);
            this.Controls.Add(grpSpawning);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnReset);
        }
        #endregion
    }
}
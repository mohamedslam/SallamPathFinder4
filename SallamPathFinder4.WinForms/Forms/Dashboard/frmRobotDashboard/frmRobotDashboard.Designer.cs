#region File Header
/// <summary>
/// File: frmRobotDashboard.Designer.cs
/// Description: Designer file for robot dashboard form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard
{
    partial class frmRobotDashboard
    {
        #region Private Fields - UI Components
        private System.ComponentModel.IContainer _components;
        private System.Windows.Forms.PictureBox cameraView;
        private SallamPathFinder4.WinForms.Controls.SensorGauge frontSensor;
        private SallamPathFinder4.WinForms.Controls.SensorGauge leftSensor;
        private SallamPathFinder4.WinForms.Controls.SensorGauge rightSensor;
        private SallamPathFinder4.WinForms.Controls.SensorGauge backSensor;
        private System.Windows.Forms.Label lblPitch;
        private System.Windows.Forms.Label lblRoll;
        private System.Windows.Forms.Label lblYaw;
        private System.Windows.Forms.ProgressBar batteryBar;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnRotLeft;
        private System.Windows.Forms.Button btnRotRight;
        private System.Windows.Forms.Button btnStrafeLeft;
        private System.Windows.Forms.Button btnStrafeRight;
        private System.Windows.Forms.Label lblImuTitle;
        private System.Windows.Forms.Label lblBatteryTitle;
        private System.Windows.Forms.TextBox _txtIpAddress;
        private System.Windows.Forms.NumericUpDown _nudPort;
        private System.Windows.Forms.Button _btnConnect;
        private System.Windows.Forms.Button _btnDisconnect;
        private System.Windows.Forms.Label _lblConnectionTitle;
        private System.Windows.Forms.GroupBox _grpConnection;
        private System.Windows.Forms.GroupBox _grpControls;
        private System.Windows.Forms.GroupBox _grpSensors;
        private System.Windows.Forms.GroupBox _grpIMU;
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
            this.Text = "Robot Dashboard - Live View";
            this.Size = new System.Drawing.Size(850, 650);
            this.MinimumSize = new System.Drawing.Size(850, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor = System.Drawing.Color.FromArgb(240, 242, 245);

            // ============================================================
            // CAMERA VIEW
            // ============================================================
            cameraView = new System.Windows.Forms.PictureBox();
            cameraView.Location = new System.Drawing.Point(220, 20);
            cameraView.Size = new System.Drawing.Size(400, 300);
            cameraView.BackColor = System.Drawing.Color.Black;
            cameraView.SizeMode = PictureBoxSizeMode.Zoom;
            cameraView.BorderStyle = BorderStyle.FixedSingle;

            // ============================================================
            // SENSOR GAUGES
            // ============================================================
            frontSensor = new SallamPathFinder4.WinForms.Controls.SensorGauge("Front", "Top");
            frontSensor.Location = new System.Drawing.Point(340, 10);
            frontSensor.Size = new System.Drawing.Size(80, 80);

            leftSensor = new SallamPathFinder4.WinForms.Controls.SensorGauge("Left", "Left");
            leftSensor.Location = new System.Drawing.Point(130, 130);
            leftSensor.Size = new System.Drawing.Size(80, 80);

            rightSensor = new SallamPathFinder4.WinForms.Controls.SensorGauge("Right", "Right");
            rightSensor.Location = new System.Drawing.Point(560, 130);
            rightSensor.Size = new System.Drawing.Size(80, 80);

            backSensor = new SallamPathFinder4.WinForms.Controls.SensorGauge("Back", "Bottom");
            backSensor.Location = new System.Drawing.Point(340, 340);
            backSensor.Size = new System.Drawing.Size(80, 80);

            // ============================================================
            // IMU GROUP
            // ============================================================
            _grpIMU = new System.Windows.Forms.GroupBox();
            _grpIMU.Text = "IMU / Gyro";
            _grpIMU.Location = new System.Drawing.Point(20, 20);
            _grpIMU.Size = new System.Drawing.Size(180, 100);

            lblPitch = new System.Windows.Forms.Label();
            lblPitch.Text = "Pitch: 0.0°";
            lblPitch.Location = new System.Drawing.Point(15, 25);
            lblPitch.Size = new System.Drawing.Size(80, 23);

            lblRoll = new System.Windows.Forms.Label();
            lblRoll.Text = "Roll: 0.0°";
            lblRoll.Location = new System.Drawing.Point(15, 50);
            lblRoll.Size = new System.Drawing.Size(80, 23);

            lblYaw = new System.Windows.Forms.Label();
            lblYaw.Text = "Yaw: 0.0°";
            lblYaw.Location = new System.Drawing.Point(15, 75);
            lblYaw.Size = new System.Drawing.Size(80, 23);

            _grpIMU.Controls.Add(lblPitch);
            _grpIMU.Controls.Add(lblRoll);
            _grpIMU.Controls.Add(lblYaw);

            // ============================================================
            // BATTERY
            // ============================================================
            lblBatteryTitle = new System.Windows.Forms.Label();
            lblBatteryTitle.Text = "Battery:";
            lblBatteryTitle.Location = new System.Drawing.Point(20, 135);
            lblBatteryTitle.Size = new System.Drawing.Size(50, 23);

            batteryBar = new System.Windows.Forms.ProgressBar();
            batteryBar.Location = new System.Drawing.Point(75, 135);
            batteryBar.Size = new System.Drawing.Size(120, 23);
            batteryBar.Maximum = 100;
            batteryBar.Value = 0;

            lblSpeed = new System.Windows.Forms.Label();
            lblSpeed.Text = "Speed: 0.0 cm/s";
            lblSpeed.Location = new System.Drawing.Point(20, 170);
            lblSpeed.Size = new System.Drawing.Size(120, 23);

            // ============================================================
            // CONNECTION GROUP
            // ============================================================
            _grpConnection = new System.Windows.Forms.GroupBox();
            _grpConnection.Text = "Connection";
            _grpConnection.Location = new System.Drawing.Point(20, 210);
            _grpConnection.Size = new System.Drawing.Size(180, 120);

            var lblIp = new System.Windows.Forms.Label();
            lblIp.Text = "IP Address:";
            lblIp.Location = new System.Drawing.Point(10, 25);
            lblIp.Size = new System.Drawing.Size(70, 23);

            _txtIpAddress = new System.Windows.Forms.TextBox();
            _txtIpAddress.Text = "192.168.1.100";
            _txtIpAddress.Location = new System.Drawing.Point(85, 22);
            _txtIpAddress.Size = new System.Drawing.Size(85, 23);

            var lblPort = new System.Windows.Forms.Label();
            lblPort.Text = "Port:";
            lblPort.Location = new System.Drawing.Point(10, 55);
            lblPort.Size = new System.Drawing.Size(40, 23);

            _nudPort = new System.Windows.Forms.NumericUpDown();
            _nudPort.Location = new System.Drawing.Point(55, 52);
            _nudPort.Size = new System.Drawing.Size(60, 23);
            _nudPort.Minimum = 1;
            _nudPort.Maximum = 65535;
            _nudPort.Value = 8080;

            _btnConnect = new System.Windows.Forms.Button();
            _btnConnect.Text = "Connect";
            _btnConnect.Location = new System.Drawing.Point(10, 85);
            _btnConnect.Size = new System.Drawing.Size(75, 25);
            _btnConnect.FlatStyle = FlatStyle.Flat;
            _btnConnect.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            _btnConnect.ForeColor = System.Drawing.Color.White;

            _btnDisconnect = new System.Windows.Forms.Button();
            _btnDisconnect.Text = "Disconnect";
            _btnDisconnect.Location = new System.Drawing.Point(95, 85);
            _btnDisconnect.Size = new System.Drawing.Size(75, 25);
            _btnDisconnect.FlatStyle = FlatStyle.Flat;
            _btnDisconnect.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            _btnDisconnect.ForeColor = System.Drawing.Color.White;
            _btnDisconnect.Enabled = false;

            _grpConnection.Controls.Add(lblIp);
            _grpConnection.Controls.Add(_txtIpAddress);
            _grpConnection.Controls.Add(lblPort);
            _grpConnection.Controls.Add(_nudPort);
            _grpConnection.Controls.Add(_btnConnect);
            _grpConnection.Controls.Add(_btnDisconnect);

            // ============================================================
            // CONTROL BUTTONS
            // ============================================================
            _grpControls = new System.Windows.Forms.GroupBox();
            _grpControls.Text = "Robot Control";
            _grpControls.Location = new System.Drawing.Point(20, 350);
            _grpControls.Size = new System.Drawing.Size(600, 200);

            // Forward
            btnForward = new System.Windows.Forms.Button();
            btnForward.Text = "▲ Forward";
            btnForward.Location = new System.Drawing.Point(240, 30);
            btnForward.Size = new System.Drawing.Size(80, 40);
            btnForward.FlatStyle = FlatStyle.Flat;
            btnForward.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnForward.ForeColor = System.Drawing.Color.White;

            // Back
            btnBack = new System.Windows.Forms.Button();
            btnBack.Text = "▼ Back";
            btnBack.Location = new System.Drawing.Point(240, 130);
            btnBack.Size = new System.Drawing.Size(80, 40);
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnBack.ForeColor = System.Drawing.Color.White;

            // Left
            btnLeft = new System.Windows.Forms.Button();
            btnLeft.Text = "◀ Left";
            btnLeft.Location = new System.Drawing.Point(150, 80);
            btnLeft.Size = new System.Drawing.Size(80, 40);
            btnLeft.FlatStyle = FlatStyle.Flat;
            btnLeft.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnLeft.ForeColor = System.Drawing.Color.White;

            // Right
            btnRight = new System.Windows.Forms.Button();
            btnRight.Text = "▶ Right";
            btnRight.Location = new System.Drawing.Point(330, 80);
            btnRight.Size = new System.Drawing.Size(80, 40);
            btnRight.FlatStyle = FlatStyle.Flat;
            btnRight.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnRight.ForeColor = System.Drawing.Color.White;

            // Stop
            btnStop = new System.Windows.Forms.Button();
            btnStop.Text = "■ Stop";
            btnStop.Location = new System.Drawing.Point(240, 80);
            btnStop.Size = new System.Drawing.Size(80, 40);
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            btnStop.ForeColor = System.Drawing.Color.White;

            // Rotate Left
            btnRotLeft = new System.Windows.Forms.Button();
            btnRotLeft.Text = "↺ Rot Left";
            btnRotLeft.Location = new System.Drawing.Point(60, 80);
            btnRotLeft.Size = new System.Drawing.Size(70, 40);
            btnRotLeft.FlatStyle = FlatStyle.Flat;
            btnRotLeft.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnRotLeft.ForeColor = System.Drawing.Color.White;

            // Rotate Right
            btnRotRight = new System.Windows.Forms.Button();
            btnRotRight.Text = "↻ Rot Right";
            btnRotRight.Location = new System.Drawing.Point(430, 80);
            btnRotRight.Size = new System.Drawing.Size(70, 40);
            btnRotRight.FlatStyle = FlatStyle.Flat;
            btnRotRight.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnRotRight.ForeColor = System.Drawing.Color.White;

            // Strafe Left
            btnStrafeLeft = new System.Windows.Forms.Button();
            btnStrafeLeft.Text = "⤹ Strafe L";
            btnStrafeLeft.Location = new System.Drawing.Point(60, 130);
            btnStrafeLeft.Size = new System.Drawing.Size(70, 40);
            btnStrafeLeft.FlatStyle = FlatStyle.Flat;
            btnStrafeLeft.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnStrafeLeft.ForeColor = System.Drawing.Color.White;

            // Strafe Right
            btnStrafeRight = new System.Windows.Forms.Button();
            btnStrafeRight.Text = "⤸ Strafe R";
            btnStrafeRight.Location = new System.Drawing.Point(430, 130);
            btnStrafeRight.Size = new System.Drawing.Size(70, 40);
            btnStrafeRight.FlatStyle = FlatStyle.Flat;
            btnStrafeRight.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnStrafeRight.ForeColor = System.Drawing.Color.White;

            _grpControls.Controls.Add(btnForward);
            _grpControls.Controls.Add(btnBack);
            _grpControls.Controls.Add(btnLeft);
            _grpControls.Controls.Add(btnRight);
            _grpControls.Controls.Add(btnStop);
            _grpControls.Controls.Add(btnRotLeft);
            _grpControls.Controls.Add(btnRotRight);
            _grpControls.Controls.Add(btnStrafeLeft);
            _grpControls.Controls.Add(btnStrafeRight);

            // ============================================================
            // STATUS BAR
            // ============================================================
            lblStatus = new System.Windows.Forms.Label();
            lblStatus.Text = "Status: Disconnected";
            lblStatus.Location = new System.Drawing.Point(20, 570);
            lblStatus.Size = new System.Drawing.Size(600, 25);
            lblStatus.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            lblStatus.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);

            // Add controls to form
            this.Controls.Add(cameraView);
            this.Controls.Add(frontSensor);
            this.Controls.Add(leftSensor);
            this.Controls.Add(rightSensor);
            this.Controls.Add(backSensor);
            this.Controls.Add(_grpIMU);
            this.Controls.Add(lblBatteryTitle);
            this.Controls.Add(batteryBar);
            this.Controls.Add(lblSpeed);
            this.Controls.Add(_grpConnection);
            this.Controls.Add(_grpControls);
            this.Controls.Add(lblStatus);
        }
        #endregion
    }
}
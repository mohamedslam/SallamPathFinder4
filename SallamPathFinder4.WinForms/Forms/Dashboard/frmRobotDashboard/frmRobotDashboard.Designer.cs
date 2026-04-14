#region File Header
/// <summary>
/// File: frmRobotDashboard.Designer.cs
/// Description: Designer file for robot dashboard form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-14
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard
{
    partial class frmRobotDashboard
    {
        private System.ComponentModel.IContainer components = null;

        #region UI Components - Camera
        private System.Windows.Forms.PictureBox cameraView;
        private System.Windows.Forms.Button btnTakePhoto;
        private System.Windows.Forms.Button btnStartVideo;
        private System.Windows.Forms.Button btnStopVideo;
        #endregion

        #region UI Components - Sensors
        private SallamPathFinder4.WinForms.Controls.SensorGauge frontSensor;
        private SallamPathFinder4.WinForms.Controls.SensorGauge leftSensor;
        private SallamPathFinder4.WinForms.Controls.SensorGauge rightSensor;
        private SallamPathFinder4.WinForms.Controls.SensorGauge backSensor;
        #endregion

        #region UI Components - IMU
        private System.Windows.Forms.GroupBox grpIMU;
        private System.Windows.Forms.Label lblPitch;
        private System.Windows.Forms.Label lblRoll;
        private System.Windows.Forms.Label lblYaw;
        #endregion

        #region UI Components - Battery & Speed
        private System.Windows.Forms.ProgressBar batteryBar;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblSpeedValue;
        private System.Windows.Forms.TrackBar trackSpeed;
        private System.Windows.Forms.Button btnSpeedUp;
        private System.Windows.Forms.Button btnSpeedDown;
        #endregion

        #region UI Components - Connection
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        #endregion

        #region UI Components - Network Status
        private System.Windows.Forms.GroupBox grpNetwork;
        private System.Windows.Forms.Label lblNetworkStatus;
        private System.Windows.Forms.Label lblTxPackets;
        private System.Windows.Forms.Label lblRxPackets;
        private System.Windows.Forms.Label lblLatency;
        #endregion

        #region UI Components - Control Buttons
        private System.Windows.Forms.GroupBox grpControls;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnRotLeft;
        private System.Windows.Forms.Button btnRotRight;
        private System.Windows.Forms.Button btnStrafeLeft;
        private System.Windows.Forms.Button btnStrafeRight;
        #endregion

        #region UI Components - Command Log
        private System.Windows.Forms.GroupBox grpCommandLog;
        private System.Windows.Forms.ListView lvCommandLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnExportLog;
        private System.Windows.Forms.Button btnSendCommands;
        private System.Windows.Forms.Label lblCommandCount;
        #endregion

        #region UI Components - Bottom Buttons
        private System.Windows.Forms.Button btnStartAuto;
        private System.Windows.Forms.Button btnPauseAuto;
        private System.Windows.Forms.Button btnStopAuto;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnClose;
        #endregion

        #region UI Components - Status
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblModeStatus;
        private System.Windows.Forms.Label lblBattery;
        private System.Windows.Forms.Label lblMotorTemp;
        private System.Windows.Forms.Label lblCpuLoad;
        private System.Windows.Forms.Label lblWifiSignal;
        private System.Windows.Forms.Label lblUptime;
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
            cameraView = new PictureBox();
            btnTakePhoto = new Button();
            btnStartVideo = new Button();
            btnStopVideo = new Button();
            grpConnection = new GroupBox();
            lblIp = new Label();
            txtIpAddress = new TextBox();
            lblPort = new Label();
            nudPort = new NumericUpDown();
            btnConnect = new Button();
            btnDisconnect = new Button();
            grpNetwork = new GroupBox();
            lblNetworkStatus = new Label();
            lblTxPackets = new Label();
            lblRxPackets = new Label();
            lblLatency = new Label();
            grpIMU = new GroupBox();
            lblPitch = new Label();
            lblRoll = new Label();
            lblYaw = new Label();
            lblBattery = new Label();
            batteryBar = new ProgressBar();
            lblSpeed = new Label();
            lblSpeedValue = new Label();
            trackSpeed = new TrackBar();
            btnSpeedUp = new Button();
            btnSpeedDown = new Button();
            grpControls = new GroupBox();
            btnForward = new Button();
            btnBack = new Button();
            btnLeft = new Button();
            btnRight = new Button();
            btnStop = new Button();
            btnRotLeft = new Button();
            btnRotRight = new Button();
            btnStrafeLeft = new Button();
            btnStrafeRight = new Button();
            grpCommandLog = new GroupBox();
            lvCommandLog = new ListView();
            btnClearLog = new Button();
            btnExportLog = new Button();
            btnSendCommands = new Button();
            lblCommandCount = new Label();
            btnStartAuto = new Button();
            btnPauseAuto = new Button();
            btnStopAuto = new Button();
            btnReset = new Button();
            btnSettings = new Button();
            btnClose = new Button();
            lblStatus = new Label();
            lblModeStatus = new Label();
            lblMotorTemp = new Label();
            lblCpuLoad = new Label();
            lblWifiSignal = new Label();
            lblUptime = new Label();
            ((System.ComponentModel.ISupportInitialize)cameraView).BeginInit();
            grpConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudPort).BeginInit();
            grpNetwork.SuspendLayout();
            grpIMU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackSpeed).BeginInit();
            grpControls.SuspendLayout();
            grpCommandLog.SuspendLayout();
            SuspendLayout();
            // 
            // cameraView
            // 
            cameraView.BackColor = Color.Black;
            cameraView.BorderStyle = BorderStyle.FixedSingle;
            cameraView.Location = new Point(12, 12);
            cameraView.Name = "cameraView";
            cameraView.Size = new Size(608, 336);
            cameraView.SizeMode = PictureBoxSizeMode.Zoom;
            cameraView.TabIndex = 0;
            cameraView.TabStop = false;
            // 
            // btnTakePhoto
            // 
            btnTakePhoto.BackColor = Color.FromArgb(52, 152, 219);
            btnTakePhoto.FlatStyle = FlatStyle.Flat;
            btnTakePhoto.ForeColor = Color.White;
            btnTakePhoto.Location = new Point(626, 149);
            btnTakePhoto.Name = "btnTakePhoto";
            btnTakePhoto.Size = new Size(80, 39);
            btnTakePhoto.TabIndex = 1;
            btnTakePhoto.Text = "📸 Photo";
            btnTakePhoto.UseVisualStyleBackColor = false;
            // 
            // btnStartVideo
            // 
            btnStartVideo.BackColor = Color.FromArgb(46, 204, 113);
            btnStartVideo.FlatStyle = FlatStyle.Flat;
            btnStartVideo.ForeColor = Color.White;
            btnStartVideo.Location = new Point(706, 149);
            btnStartVideo.Name = "btnStartVideo";
            btnStartVideo.Size = new Size(80, 39);
            btnStartVideo.TabIndex = 2;
            btnStartVideo.Text = "▶️ Video";
            btnStartVideo.UseVisualStyleBackColor = false;
            // 
            // btnStopVideo
            // 
            btnStopVideo.BackColor = Color.FromArgb(231, 76, 60);
            btnStopVideo.FlatStyle = FlatStyle.Flat;
            btnStopVideo.ForeColor = Color.White;
            btnStopVideo.Location = new Point(626, 194);
            btnStopVideo.Name = "btnStopVideo";
            btnStopVideo.Size = new Size(160, 46);
            btnStopVideo.TabIndex = 3;
            btnStopVideo.Text = "⏹️ Stop";
            btnStopVideo.UseVisualStyleBackColor = false;
            // 
            // grpConnection
            // 
            grpConnection.Controls.Add(lblIp);
            grpConnection.Controls.Add(txtIpAddress);
            grpConnection.Controls.Add(lblPort);
            grpConnection.Controls.Add(nudPort);
            grpConnection.Controls.Add(btnConnect);
            grpConnection.Controls.Add(btnDisconnect);
            grpConnection.ForeColor = Color.White;
            grpConnection.Location = new Point(792, 12);
            grpConnection.Name = "grpConnection";
            grpConnection.Size = new Size(280, 120);
            grpConnection.TabIndex = 4;
            grpConnection.TabStop = false;
            grpConnection.Text = "🔌 Connection";
            // 
            // lblIp
            // 
            lblIp.ForeColor = Color.White;
            lblIp.Location = new Point(10, 25);
            lblIp.Name = "lblIp";
            lblIp.Size = new Size(70, 23);
            lblIp.TabIndex = 0;
            lblIp.Text = "IP Address:";
            // 
            // txtIpAddress
            // 
            txtIpAddress.Location = new Point(90, 22);
            txtIpAddress.Name = "txtIpAddress";
            txtIpAddress.Size = new Size(100, 23);
            txtIpAddress.TabIndex = 1;
            txtIpAddress.Text = "192.168.1.100";
            // 
            // lblPort
            // 
            lblPort.ForeColor = Color.White;
            lblPort.Location = new Point(10, 55);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(40, 23);
            lblPort.TabIndex = 2;
            lblPort.Text = "Port:";
            // 
            // nudPort
            // 
            nudPort.Location = new Point(90, 52);
            nudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            nudPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudPort.Name = "nudPort";
            nudPort.Size = new Size(100, 23);
            nudPort.TabIndex = 3;
            nudPort.Value = new decimal(new int[] { 8080, 0, 0, 0 });
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.FromArgb(46, 204, 113);
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.ForeColor = Color.White;
            btnConnect.Location = new Point(10, 85);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(120, 28);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "● Connect";
            btnConnect.UseVisualStyleBackColor = false;
            // 
            // btnDisconnect
            // 
            btnDisconnect.BackColor = Color.FromArgb(231, 76, 60);
            btnDisconnect.Enabled = false;
            btnDisconnect.FlatStyle = FlatStyle.Flat;
            btnDisconnect.ForeColor = Color.White;
            btnDisconnect.Location = new Point(153, 85);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(121, 28);
            btnDisconnect.TabIndex = 5;
            btnDisconnect.Text = "○ Disconnect";
            btnDisconnect.UseVisualStyleBackColor = false;
            // 
            // grpNetwork
            // 
            grpNetwork.Controls.Add(lblNetworkStatus);
            grpNetwork.Controls.Add(lblTxPackets);
            grpNetwork.Controls.Add(lblRxPackets);
            grpNetwork.Controls.Add(lblLatency);
            grpNetwork.Controls.Add(lblWifiSignal);
            grpNetwork.ForeColor = Color.White;
            grpNetwork.Location = new Point(626, 12);
            grpNetwork.Name = "grpNetwork";
            grpNetwork.Size = new Size(160, 120);
            grpNetwork.TabIndex = 5;
            grpNetwork.TabStop = false;
            grpNetwork.Text = "📡 Command Network";
            // 
            // lblNetworkStatus
            // 
            lblNetworkStatus.Location = new Point(10, 22);
            lblNetworkStatus.Name = "lblNetworkStatus";
            lblNetworkStatus.Size = new Size(120, 23);
            lblNetworkStatus.TabIndex = 0;
            lblNetworkStatus.Text = "○ Disconnected";
            // 
            // lblTxPackets
            // 
            lblTxPackets.Location = new Point(10, 45);
            lblTxPackets.Name = "lblTxPackets";
            lblTxPackets.Size = new Size(80, 23);
            lblTxPackets.TabIndex = 1;
            lblTxPackets.Text = "TX: 0";
            // 
            // lblRxPackets
            // 
            lblRxPackets.Location = new Point(90, 42);
            lblRxPackets.Name = "lblRxPackets";
            lblRxPackets.Size = new Size(64, 23);
            lblRxPackets.TabIndex = 2;
            lblRxPackets.Text = "RX: 0";
            // 
            // lblLatency
            // 
            lblLatency.Location = new Point(10, 68);
            lblLatency.Name = "lblLatency";
            lblLatency.Size = new Size(120, 23);
            lblLatency.TabIndex = 3;
            lblLatency.Text = "Latency: 0 ms";
            // 
            // frontSensor
            // 
            frontSensor = new Controls.SensorGauge("front", "front");
            frontSensor.BackColor = Color.Transparent;
            frontSensor.LabelText = "Front";
            frontSensor.Location = new Point(897, 239);
            frontSensor.MaxValue = 5D;
            frontSensor.Name = "frontSensor";
            frontSensor.Size = new Size(70, 70);
            frontSensor.TabIndex = 6;
            frontSensor.Value = 0D;
            // 
            // leftSensor
            // 
            leftSensor =new Controls.SensorGauge("", "");
            leftSensor.BackColor = Color.Transparent;
            leftSensor.LabelText = "Left";
            leftSensor.Location = new Point(832, 282);
            leftSensor.MaxValue = 5D;
            leftSensor.Name = "leftSensor";
            leftSensor.Size = new Size(70, 70);
            leftSensor.TabIndex = 7;
            leftSensor.Value = 0D;
            // 
            // rightSensor
            // 
            rightSensor = new Controls.SensorGauge("", "");
            rightSensor.BackColor = Color.Transparent;
            rightSensor.LabelText = "Right";
            rightSensor.Location = new Point(966, 282);
            rightSensor.MaxValue = 5D;
            rightSensor.Name = "rightSensor";
            rightSensor.Size = new Size(70, 70);
            rightSensor.TabIndex = 8;
            rightSensor.Value = 0D;
            // 
            // backSensor
            // 
            backSensor = new Controls.SensorGauge("", "");
            backSensor.BackColor = Color.Transparent;
            backSensor.LabelText = "Back";
            backSensor.Location = new Point(896, 315);
            backSensor.MaxValue = 5D;
            backSensor.Name = "backSensor";
            backSensor.Size = new Size(70, 70);
            backSensor.TabIndex = 9;
            backSensor.Value = 0D;
            // 
            // grpIMU
            // 
            grpIMU.Controls.Add(lblPitch);
            grpIMU.Controls.Add(lblRoll);
            grpIMU.Controls.Add(lblYaw);
            grpIMU.Controls.Add(lblCpuLoad);
            grpIMU.Controls.Add(lblMotorTemp);
            grpIMU.Controls.Add(lblUptime);
            grpIMU.ForeColor = Color.White;
            grpIMU.Location = new Point(792, 140);
            grpIMU.Name = "grpIMU";
            grpIMU.Size = new Size(280, 100);
            grpIMU.TabIndex = 10;
            grpIMU.TabStop = false;
            grpIMU.Text = "\U0001f9ed IMU / Gyro";
            // 
            // lblPitch
            // 
            lblPitch.Location = new Point(10, 25);
            lblPitch.Name = "lblPitch";
            lblPitch.Size = new Size(120, 23);
            lblPitch.TabIndex = 0;
            lblPitch.Text = "Pitch: 0.0°";
            // 
            // lblRoll
            // 
            lblRoll.Location = new Point(10, 50);
            lblRoll.Name = "lblRoll";
            lblRoll.Size = new Size(120, 23);
            lblRoll.TabIndex = 1;
            lblRoll.Text = "Roll: 0.0°";
            // 
            // lblYaw
            // 
            lblYaw.Location = new Point(10, 75);
            lblYaw.Name = "lblYaw";
            lblYaw.Size = new Size(120, 23);
            lblYaw.TabIndex = 2;
            lblYaw.Text = "Yaw: 0.0°";
            // 
            // lblBattery
            // 
            lblBattery.ForeColor = Color.FromArgb(255, 255, 192);
            lblBattery.Location = new Point(788, 384);
            lblBattery.Name = "lblBattery";
            lblBattery.Size = new Size(98, 23);
            lblBattery.TabIndex = 5;
            lblBattery.Text = "Battery: --%";
            // 
            // batteryBar
            // 
            batteryBar.Location = new Point(623, 378);
            batteryBar.Name = "batteryBar";
            batteryBar.Size = new Size(160, 23);
            batteryBar.TabIndex = 6;
            // 
            // lblSpeed
            // 
            lblSpeed.Location = new Point(6, 21);
            lblSpeed.Name = "lblSpeed";
            lblSpeed.Size = new Size(50, 23);
            lblSpeed.TabIndex = 3;
            lblSpeed.Text = "Speed:";
            // 
            // lblSpeedValue
            // 
            lblSpeedValue.Location = new Point(55, 21);
            lblSpeedValue.Name = "lblSpeedValue";
            lblSpeedValue.Size = new Size(40, 23);
            lblSpeedValue.TabIndex = 4;
            lblSpeedValue.Text = "50%";
            // 
            // trackSpeed
            // 
            trackSpeed.Location = new Point(90, 21);
            trackSpeed.Maximum = 100;
            trackSpeed.Name = "trackSpeed";
            trackSpeed.Size = new Size(255, 45);
            trackSpeed.TabIndex = 1;
            trackSpeed.Value = 50;
            // 
            // btnSpeedUp
            // 
            btnSpeedUp.BackColor = Color.Green;
            btnSpeedUp.Location = new Point(391, 21);
            btnSpeedUp.Name = "btnSpeedUp";
            btnSpeedUp.Size = new Size(35, 30);
            btnSpeedUp.TabIndex = 2;
            btnSpeedUp.Text = "+";
            btnSpeedUp.UseVisualStyleBackColor = false;
            // 
            // btnSpeedDown
            // 
            btnSpeedDown.BackColor = Color.FromArgb(255, 128, 0);
            btnSpeedDown.Location = new Point(351, 21);
            btnSpeedDown.Name = "btnSpeedDown";
            btnSpeedDown.Size = new Size(35, 30);
            btnSpeedDown.TabIndex = 0;
            btnSpeedDown.Text = "-";
            btnSpeedDown.UseVisualStyleBackColor = false;
            // 
            // grpControls
            // 
            grpControls.Controls.Add(btnSpeedDown);
            grpControls.Controls.Add(trackSpeed);
            grpControls.Controls.Add(btnSpeedUp);
            grpControls.Controls.Add(lblSpeed);
            grpControls.Controls.Add(lblSpeedValue);
            grpControls.Controls.Add(btnForward);
            grpControls.Controls.Add(btnBack);
            grpControls.Controls.Add(btnLeft);
            grpControls.Controls.Add(btnRight);
            grpControls.Controls.Add(btnStop);
            grpControls.Controls.Add(btnRotLeft);
            grpControls.Controls.Add(btnRotRight);
            grpControls.Controls.Add(btnStrafeLeft);
            grpControls.Controls.Add(btnStrafeRight);
            grpControls.ForeColor = Color.White;
            grpControls.Location = new Point(626, 405);
            grpControls.Name = "grpControls";
            grpControls.Size = new Size(446, 352);
            grpControls.TabIndex = 11;
            grpControls.TabStop = false;
            grpControls.Text = "🎮 Manual Control";
            // 
            // btnForward
            // 
            btnForward.BackColor = Color.FromArgb(52, 152, 219);
            btnForward.FlatStyle = FlatStyle.Flat;
            btnForward.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnForward.ForeColor = Color.White;
            btnForward.Location = new Point(180, 88);
            btnForward.Name = "btnForward";
            btnForward.Size = new Size(80, 50);
            btnForward.TabIndex = 7;
            btnForward.Text = "▲\nForward";
            btnForward.UseVisualStyleBackColor = false;
            // 
            // btnBack
            // 
            btnBack.BackColor = Color.FromArgb(52, 152, 219);
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnBack.ForeColor = Color.White;
            btnBack.Location = new Point(180, 198);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(80, 50);
            btnBack.TabIndex = 8;
            btnBack.Text = "▼\nBack";
            btnBack.UseVisualStyleBackColor = false;
            // 
            // btnLeft
            // 
            btnLeft.BackColor = Color.FromArgb(52, 152, 219);
            btnLeft.FlatStyle = FlatStyle.Flat;
            btnLeft.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnLeft.ForeColor = Color.White;
            btnLeft.Location = new Point(90, 143);
            btnLeft.Name = "btnLeft";
            btnLeft.Size = new Size(80, 50);
            btnLeft.TabIndex = 9;
            btnLeft.Text = "◀\nLeft";
            btnLeft.UseVisualStyleBackColor = false;
            // 
            // btnRight
            // 
            btnRight.BackColor = Color.FromArgb(52, 152, 219);
            btnRight.FlatStyle = FlatStyle.Flat;
            btnRight.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRight.ForeColor = Color.White;
            btnRight.Location = new Point(270, 143);
            btnRight.Name = "btnRight";
            btnRight.Size = new Size(80, 50);
            btnRight.TabIndex = 10;
            btnRight.Text = "▶\nRight";
            btnRight.UseVisualStyleBackColor = false;
            // 
            // btnStop
            // 
            btnStop.BackColor = Color.FromArgb(231, 76, 60);
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnStop.ForeColor = Color.White;
            btnStop.Location = new Point(180, 143);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(80, 50);
            btnStop.TabIndex = 11;
            btnStop.Text = "■\nSTOP";
            btnStop.UseVisualStyleBackColor = false;
            // 
            // btnRotLeft
            // 
            btnRotLeft.BackColor = SystemColors.ActiveCaption;
            btnRotLeft.FlatStyle = FlatStyle.Flat;
            btnRotLeft.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnRotLeft.ForeColor = Color.White;
            btnRotLeft.Location = new Point(14, 113);
            btnRotLeft.Name = "btnRotLeft";
            btnRotLeft.Size = new Size(70, 50);
            btnRotLeft.TabIndex = 12;
            btnRotLeft.Text = "↺\nRot L";
            btnRotLeft.UseVisualStyleBackColor = false;
            // 
            // btnRotRight
            // 
            btnRotRight.BackColor = SystemColors.ActiveCaption;
            btnRotRight.FlatStyle = FlatStyle.Flat;
            btnRotRight.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnRotRight.ForeColor = Color.White;
            btnRotRight.Location = new Point(356, 113);
            btnRotRight.Name = "btnRotRight";
            btnRotRight.Size = new Size(70, 50);
            btnRotRight.TabIndex = 13;
            btnRotRight.Text = "↻\nRot R";
            btnRotRight.UseVisualStyleBackColor = false;
            // 
            // btnStrafeLeft
            // 
            btnStrafeLeft.BackColor = SystemColors.ActiveCaption;
            btnStrafeLeft.FlatStyle = FlatStyle.Flat;
            btnStrafeLeft.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnStrafeLeft.ForeColor = Color.White;
            btnStrafeLeft.Location = new Point(14, 173);
            btnStrafeLeft.Name = "btnStrafeLeft";
            btnStrafeLeft.Size = new Size(70, 50);
            btnStrafeLeft.TabIndex = 14;
            btnStrafeLeft.Text = "⤹\nStr L";
            btnStrafeLeft.UseVisualStyleBackColor = false;
            // 
            // btnStrafeRight
            // 
            btnStrafeRight.BackColor = SystemColors.ActiveCaption;
            btnStrafeRight.FlatStyle = FlatStyle.Flat;
            btnStrafeRight.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnStrafeRight.ForeColor = Color.White;
            btnStrafeRight.Location = new Point(356, 173);
            btnStrafeRight.Name = "btnStrafeRight";
            btnStrafeRight.Size = new Size(70, 50);
            btnStrafeRight.TabIndex = 15;
            btnStrafeRight.Text = "⤸\nStr R";
            btnStrafeRight.UseVisualStyleBackColor = false;
            // 
            // grpCommandLog
            // 
            grpCommandLog.Controls.Add(lvCommandLog);
            grpCommandLog.Controls.Add(btnExportLog);
            grpCommandLog.Controls.Add(btnSendCommands);
            grpCommandLog.Controls.Add(btnClearLog);
            grpCommandLog.Controls.Add(lblCommandCount);
            grpCommandLog.ForeColor = Color.White;
            grpCommandLog.Location = new Point(12, 354);
            grpCommandLog.Name = "grpCommandLog";
            grpCommandLog.Size = new Size(608, 468);
            grpCommandLog.TabIndex = 12;
            grpCommandLog.TabStop = false;
            grpCommandLog.Text = "📋 Command Log";
            // 
            // lvCommandLog
            // 
            lvCommandLog.Dock = DockStyle.Top;
            lvCommandLog.Font = new Font("Consolas", 9F);
            lvCommandLog.FullRowSelect = true;
            lvCommandLog.GridLines = true;
            lvCommandLog.Location = new Point(3, 19);
            lvCommandLog.Name = "lvCommandLog";
            lvCommandLog.Size = new Size(602, 447);
            lvCommandLog.TabIndex = 0;
            lvCommandLog.UseCompatibleStateImageBehavior = false;
            lvCommandLog.View = View.Details;
            // 
            // btnClearLog
            // 
            btnClearLog.BackColor = Color.FromArgb(149, 165, 166);
            btnClearLog.FlatStyle = FlatStyle.Flat;
            btnClearLog.ForeColor = Color.White;
            btnClearLog.Location = new Point(535, 472);
            btnClearLog.Name = "btnClearLog";
            btnClearLog.Size = new Size(67, 30);
            btnClearLog.TabIndex = 1;
            btnClearLog.Text = "🗑 Clear";
            btnClearLog.UseVisualStyleBackColor = false;
            // 
            // btnExportLog
            // 
            btnExportLog.BackColor = Color.FromArgb(52, 152, 219);
            btnExportLog.FlatStyle = FlatStyle.Flat;
            btnExportLog.ForeColor = Color.White;
            btnExportLog.Location = new Point(10, 472);
            btnExportLog.Name = "btnExportLog";
            btnExportLog.Size = new Size(67, 30);
            btnExportLog.TabIndex = 2;
            btnExportLog.Text = "💾 Export";
            btnExportLog.UseVisualStyleBackColor = false;
            // 
            // btnSendCommands
            // 
            btnSendCommands.BackColor = Color.FromArgb(46, 204, 113);
            btnSendCommands.FlatStyle = FlatStyle.Flat;
            btnSendCommands.ForeColor = Color.White;
            btnSendCommands.Location = new Point(83, 472);
            btnSendCommands.Name = "btnSendCommands";
            btnSendCommands.Size = new Size(67, 30);
            btnSendCommands.TabIndex = 3;
            btnSendCommands.Text = "📤 Send";
            btnSendCommands.UseVisualStyleBackColor = false;
            // 
            // lblCommandCount
            // 
            lblCommandCount.ForeColor = Color.White;
            lblCommandCount.Location = new Point(166, 480);
            lblCommandCount.Name = "lblCommandCount";
            lblCommandCount.Size = new Size(90, 23);
            lblCommandCount.TabIndex = 4;
            lblCommandCount.Text = "Commands: 0";
            // 
            // btnStartAuto
            // 
            btnStartAuto.BackColor = Color.FromArgb(46, 204, 113);
            btnStartAuto.FlatStyle = FlatStyle.Flat;
            btnStartAuto.ForeColor = Color.White;
            btnStartAuto.Location = new Point(626, 783);
            btnStartAuto.Name = "btnStartAuto";
            btnStartAuto.Size = new Size(85, 35);
            btnStartAuto.TabIndex = 13;
            btnStartAuto.Text = "▶ Start Auto";
            btnStartAuto.UseVisualStyleBackColor = false;
            // 
            // btnPauseAuto
            // 
            btnPauseAuto.BackColor = Color.FromArgb(241, 196, 15);
            btnPauseAuto.FlatStyle = FlatStyle.Flat;
            btnPauseAuto.ForeColor = Color.White;
            btnPauseAuto.Location = new Point(715, 783);
            btnPauseAuto.Name = "btnPauseAuto";
            btnPauseAuto.Size = new Size(85, 35);
            btnPauseAuto.TabIndex = 14;
            btnPauseAuto.Text = "⏸ Pause";
            btnPauseAuto.UseVisualStyleBackColor = false;
            // 
            // btnStopAuto
            // 
            btnStopAuto.BackColor = Color.FromArgb(231, 76, 60);
            btnStopAuto.FlatStyle = FlatStyle.Flat;
            btnStopAuto.ForeColor = Color.White;
            btnStopAuto.Location = new Point(806, 783);
            btnStopAuto.Name = "btnStopAuto";
            btnStopAuto.Size = new Size(80, 35);
            btnStopAuto.TabIndex = 15;
            btnStopAuto.Text = "⏹ Stop";
            btnStopAuto.UseVisualStyleBackColor = false;
            // 
            // btnReset
            // 
            btnReset.BackColor = Color.FromArgb(52, 152, 219);
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.ForeColor = Color.White;
            btnReset.Location = new Point(897, 783);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(79, 35);
            btnReset.TabIndex = 16;
            btnReset.Text = "🔄 Reset";
            btnReset.UseVisualStyleBackColor = false;
            // 
            // btnSettings
            // 
            btnSettings.BackColor = Color.FromArgb(52, 73, 94);
            btnSettings.FlatStyle = FlatStyle.Flat;
            btnSettings.ForeColor = Color.White;
            btnSettings.Location = new Point(982, 783);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(90, 35);
            btnSettings.TabIndex = 17;
            btnSettings.Text = "⚙️ Settings";
            btnSettings.UseVisualStyleBackColor = false;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.FromArgb(149, 165, 166);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(972, 835);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 26);
            btnClose.TabIndex = 18;
            btnClose.Text = "❌ Close";
            btnClose.UseVisualStyleBackColor = false;
            // 
            // lblStatus
            // 
            lblStatus.BackColor = Color.FromArgb(52, 73, 94);
            lblStatus.ForeColor = Color.White;
            lblStatus.Location = new Point(12, 835);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(5, 0, 0, 0);
            lblStatus.Size = new Size(954, 25);
            lblStatus.TabIndex = 19;
            lblStatus.Text = "Status: Disconnected";
            // 
            // lblModeStatus
            // 
            lblModeStatus.ForeColor = Color.White;
            lblModeStatus.Location = new Point(945, 384);
            lblModeStatus.Name = "lblModeStatus";
            lblModeStatus.Size = new Size(120, 25);
            lblModeStatus.TabIndex = 20;
            lblModeStatus.Text = "Mode: Manual";
            // 
            // lblMotorTemp
            // 
            lblMotorTemp.ForeColor = Color.White;
            lblMotorTemp.Location = new Point(174, 44);
            lblMotorTemp.Name = "lblMotorTemp";
            lblMotorTemp.Size = new Size(100, 25);
            lblMotorTemp.TabIndex = 21;
            lblMotorTemp.Text = "Motor: --°C";
            // 
            // lblCpuLoad
            // 
            lblCpuLoad.ForeColor = Color.White;
            lblCpuLoad.Location = new Point(174, 19);
            lblCpuLoad.Name = "lblCpuLoad";
            lblCpuLoad.Size = new Size(80, 25);
            lblCpuLoad.TabIndex = 22;
            lblCpuLoad.Text = "CPU: --%";
            // 
            // lblWifiSignal
            // 
            lblWifiSignal.ForeColor = Color.White;
            lblWifiSignal.Location = new Point(10, 90);
            lblWifiSignal.Name = "lblWifiSignal";
            lblWifiSignal.Size = new Size(100, 25);
            lblWifiSignal.TabIndex = 23;
            lblWifiSignal.Text = "WiFi: -- dBm";
            // 
            // lblUptime
            // 
            lblUptime.ForeColor = Color.White;
            lblUptime.Location = new Point(174, 69);
            lblUptime.Name = "lblUptime";
            lblUptime.Size = new Size(99, 25);
            lblUptime.TabIndex = 24;
            lblUptime.Text = "Uptime: --";
            // 
            // frmRobotDashboard
            // 
            BackColor = Color.FromArgb(30, 30, 35);
            ClientSize = new Size(1084, 871);
            Controls.Add(batteryBar);
            Controls.Add(cameraView);
            Controls.Add(btnTakePhoto);
            Controls.Add(btnStartVideo);
            Controls.Add(btnStopVideo);
            Controls.Add(grpConnection);
            Controls.Add(lblBattery);
            Controls.Add(grpNetwork);
            Controls.Add(frontSensor);
            Controls.Add(leftSensor);
            Controls.Add(rightSensor);
            Controls.Add(backSensor);
            Controls.Add(grpIMU);
            Controls.Add(grpControls);
            Controls.Add(grpCommandLog);
            Controls.Add(btnStartAuto);
            Controls.Add(btnPauseAuto);
            Controls.Add(btnStopAuto);
            Controls.Add(btnReset);
            Controls.Add(btnSettings);
            Controls.Add(btnClose);
            Controls.Add(lblStatus);
            Controls.Add(lblModeStatus);
            Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            MinimumSize = new Size(1100, 750);
            Name = "frmRobotDashboard";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Robot Command Center - Real Robot Dashboard";
            ((System.ComponentModel.ISupportInitialize)cameraView).EndInit();
            grpConnection.ResumeLayout(false);
            grpConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudPort).EndInit();
            grpNetwork.ResumeLayout(false);
            grpIMU.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)trackSpeed).EndInit();
            grpControls.ResumeLayout(false);
            grpControls.PerformLayout();
            grpCommandLog.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion

        private Label lblIp;
        private Label lblPort;
    }
}
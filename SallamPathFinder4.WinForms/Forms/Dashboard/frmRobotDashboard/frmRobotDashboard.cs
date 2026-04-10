#region File Header
/// <summary>
/// File: frmRobotDashboard.cs
/// Description: Robot dashboard form for live sensor and camera data
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard.Core;
using SallamPathFinder4.WinForms.ViewModels;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard
{
    /// <summary>
    /// Dashboard form for monitoring robot status and sending commands
    /// </summary>
    public sealed partial class frmRobotDashboard : Form
    {
        #region Private Fields
        private readonly DashboardViewModel _viewModel;
        private readonly RobotDashboardLogic _logic;
        private bool _isConnected;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the robot dashboard form
        /// </summary>
        public frmRobotDashboard()
        {
            _viewModel = new DashboardViewModel(null, null);
            _logic = new RobotDashboardLogic();

            InitializeComponent();
            WireEvents();
            InitializeUI();
        }

        /// <summary>
        /// Initializes a new instance with a ViewModel
        /// </summary>
        public frmRobotDashboard(DashboardViewModel viewModel)
        {
            _viewModel = viewModel ?? new DashboardViewModel(null, null);
            _logic = new RobotDashboardLogic();

            InitializeComponent();
            WireEvents();
            InitializeUI();
        }
        #endregion

        #region Private Methods - Initialization
        /// <summary>
        /// Wires up all event handlers
        /// </summary>
        private void WireEvents()
        {
            // Connection events
            _btnConnect.Click += BtnConnect_Click;
            _btnDisconnect.Click += BtnDisconnect_Click;

            // Movement commands
            btnForward.Click += (s, e) => SendCommand(RobotCommand.Forward);
            btnBack.Click += (s, e) => SendCommand(RobotCommand.Backward);
            btnLeft.Click += (s, e) => SendCommand(RobotCommand.TurnLeft);
            btnRight.Click += (s, e) => SendCommand(RobotCommand.TurnRight);
            btnStop.Click += (s, e) => SendCommand(RobotCommand.Stop);
            btnRotLeft.Click += (s, e) => SendCommand(RobotCommand.TurnLeft);
            btnRotRight.Click += (s, e) => SendCommand(RobotCommand.TurnRight);
            btnStrafeLeft.Click += (s, e) => SendCommand(RobotCommand.TurnLeft);
            btnStrafeRight.Click += (s, e) => SendCommand(RobotCommand.TurnRight);

            // ViewModel events
            if (_viewModel != null)
            {
                _viewModel.CameraFrameReceived += OnCameraFrameReceived;
                _viewModel.SensorDataReceived += OnSensorDataReceived;
                _viewModel.StatusChanged += OnStatusChanged;
            }
        }

        /// <summary>
        /// Initializes UI with default values
        /// </summary>
        private void InitializeUI()
        {
            _isConnected = false;
            UpdateConnectionUI();
            batteryBar.Maximum = 100;
            batteryBar.Value = 0;
            lblSpeed.Text = "Speed: 0.0 cm/s";
            lblStatus.Text = "Status: Disconnected";
        }

        private void UpdateConnectionUI()
        {
            _btnConnect.Enabled = !_isConnected;
            _btnDisconnect.Enabled = _isConnected;

            // Enable/disable command buttons based on connection
            bool enabled = _isConnected;
            btnForward.Enabled = enabled;
            btnBack.Enabled = enabled;
            btnLeft.Enabled = enabled;
            btnRight.Enabled = enabled;
            btnStop.Enabled = enabled;
            btnRotLeft.Enabled = enabled;
            btnRotRight.Enabled = enabled;
            btnStrafeLeft.Enabled = enabled;
            btnStrafeRight.Enabled = enabled;
        }
        #endregion

        #region Private Methods - Commands
        /// <summary>
        /// Sends a command to the robot
        /// </summary>
        private void SendCommand(RobotCommand command)
        {
            if (!_isConnected)
            {
                OnStatusChanged("Not connected. Please connect first.");
                return;
            }

            _viewModel?.SendCommand(command);
        }

        /// <summary>
        /// Connects to the robot
        /// </summary>
        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            string ip = _txtIpAddress.Text.Trim();
            int port = (int)_nudPort.Value;

            if (string.IsNullOrEmpty(ip))
            {
                MessageBox.Show("Please enter a valid IP address.", "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await _viewModel.ConnectAsync(ip, port);
        }

        /// <summary>
        /// Disconnects from the robot
        /// </summary>
        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            _viewModel.Disconnect();
        }
        #endregion

        #region Private Methods - UI Updates
        /// <summary>
        /// Updates sensor displays
        /// </summary>
        private void UpdateSensors(SensorData data)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateSensors(data)));
                return;
            }

            frontSensor.Value = data.FrontDistance;
            leftSensor.Value = data.LeftDistance;
            rightSensor.Value = data.RightDistance;
            backSensor.Value = data.BackDistance;

            lblPitch.Text = $"Pitch: {data.Pitch:F1}°";
            lblRoll.Text = $"Roll: {data.Roll:F1}°";
            lblYaw.Text = $"Yaw: {data.Yaw:F1}°";
        }

        /// <summary>
        /// Updates battery display
        /// </summary>
        public void UpdateBattery(double level)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateBattery(level)));
                return;
            }

            int batteryValue = (int)Math.Round(level);
            batteryBar.Value = Math.Clamp(batteryValue, 0, 100);

            // Change color based on battery level
            if (level < 10)
                batteryBar.ForeColor = Color.FromArgb(231, 76, 60);
            else if (level < 20)
                batteryBar.ForeColor = Color.FromArgb(241, 196, 15);
            else
                batteryBar.ForeColor = Color.FromArgb(46, 204, 113);
        }

        /// <summary>
        /// Updates speed display
        /// </summary>
        public void UpdateSpeed(double speed)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateSpeed(speed)));
                return;
            }

            lblSpeed.Text = $"Speed: {speed:F1} cm/s";
        }
        #endregion

        #region Event Handlers - ViewModel
        /// <summary>
        /// Handles camera frame received event
        /// </summary>
        private void OnCameraFrameReceived(Image img)
        {
            if (cameraView.InvokeRequired)
            {
                cameraView.Invoke(new Action(() => cameraView.Image = img));
            }
            else
            {
                cameraView.Image = img;
            }
        }

        /// <summary>
        /// Handles sensor data received event
        /// </summary>
        private void OnSensorDataReceived(SensorData data)
        {
            UpdateSensors(data);
        }

        /// <summary>
        /// Handles status changed event
        /// </summary>
        private void OnStatusChanged(string status)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => lblStatus.Text = $"Status: {status}"));
            }
            else
            {
                lblStatus.Text = $"Status: {status}";
            }

            // Update connection state based on status
            if (status == "Connected")
            {
                _isConnected = true;
                UpdateConnectionUI();
            }
            else if (status == "Disconnected" || status == "Connection Failed")
            {
                _isConnected = false;
                UpdateConnectionUI();
            }
        }
        #endregion

        #region Protected Methods
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isConnected)
            {
                _viewModel?.Disconnect();
            }
            _viewModel?.Dispose();
            base.OnFormClosing(e);
        }
        #endregion
    }
}
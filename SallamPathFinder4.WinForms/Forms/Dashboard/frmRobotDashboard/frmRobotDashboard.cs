#region File Header
/// <summary>
/// File: frmRobotDashboard.cs
/// Description: Complete robot dashboard with ESP32-CAM, sensors, and command network
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-14
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard.Core;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard
{
    #region Class Documentation
    /// <summary>
    /// Complete robot dashboard with ESP32-CAM, sensors, and command network
    /// </summary>
    #endregion
    public sealed partial class frmRobotDashboard : Form
    {
        #region Constants
        private const int TELEMETRY_UPDATE_INTERVAL_MS = 100;
        private const int COMMAND_LOG_MAX_ITEMS = 100;
        private const string ESP32_CAM_STREAM_URL = "http://{0}:81/stream";
        private const string ESP32_CAM_CAPTURE_URL = "http://{0}:81/capture";
        #endregion

        #region Private Fields - Core
        private readonly RobotDashboardLogic _logic;
        private bool _isConnected;
        private bool _isExecuting;
        private bool _isPaused;
        private HttpClient _httpClient;
        private CancellationTokenSource _cameraCts;
        private Task _cameraStreamTask;
        #endregion

        #region Private Fields - Telemetry
        private System.Windows.Forms. Timer _telemetryTimer;
        private RobotTelemetry _currentTelemetry;
        private Point _robotPosition;
        private float _robotAngle;
        #endregion

        #region Private Fields - Command Network
        private int _txPackets;
        private int _rxPackets;
        private int _networkLatencyMs;
        #endregion

        #region Constructor
        public frmRobotDashboard()
        {
            _logic = new RobotDashboardLogic();
            _httpClient = new HttpClient();
            _currentTelemetry = new RobotTelemetry();
            _robotPosition = new Point(0, 0);
            _robotAngle = 0;

            InitializeComponent();
            WireEvents();
            InitializeUI();
        }
        #endregion

        #region Private Methods - Initialization
        /// <summary>
        /// Initializes UI with default values
        /// </summary>
        private void InitializeUI()
        {
            _isConnected = false;
            UpdateConnectionUI();
            UpdateNetworkStatusUI();
            batteryBar.Maximum = 100;
            batteryBar.Value = 0;
            lblSpeed.Text = "Speed: 0.0 cm/s";
            lblStatus.Text = "Status: Disconnected";

            // Initialize telemetry timer
            _telemetryTimer = new System.Windows.Forms. Timer();
            _telemetryTimer.Interval = TELEMETRY_UPDATE_INTERVAL_MS;
            _telemetryTimer.Tick += OnTelemetryTimerTick;
        }

        /// <summary>
        /// Updates connection UI state
        /// </summary>
        private void UpdateConnectionUI()
        {
            btnConnect.Enabled = !_isConnected;
            btnDisconnect.Enabled = _isConnected;

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
            btnSpeedUp.Enabled = enabled;
            btnSpeedDown.Enabled = enabled;
            trackSpeed.Enabled = enabled;
        }

        /// <summary>
        /// Updates network status UI
        /// </summary>
        private void UpdateNetworkStatusUI()
        {
            if (lblNetworkStatus != null)
            {
                lblNetworkStatus.Text = _isConnected ? "● Connected" : "○ Disconnected";
                lblNetworkStatus.ForeColor = _isConnected ? Color.Green : Color.Red;
            }

            if (lblTxPackets != null)
            {
                lblTxPackets.Text = $"TX: {_txPackets}";
            }

            if (lblRxPackets != null)
            {
                lblRxPackets.Text = $"RX: {_rxPackets}";
            }

            if (lblLatency != null)
            {
                lblLatency.Text = $"Latency: {_networkLatencyMs} ms";
                lblLatency.ForeColor = _networkLatencyMs < 50 ? Color.Green :
                                      _networkLatencyMs < 100 ? Color.Orange : Color.Red;
            }
        }
        #endregion

        #region Private Methods - Events Wiring
        /// <summary>
        /// Wires up all event handlers
        /// </summary>
        private void WireEvents()
        {
            // Connection events
            btnConnect.Click += BtnConnect_Click;
            btnDisconnect.Click += BtnDisconnect_Click;

            // Movement commands
            btnForward.Click += (s, e) => SendCommand("F,1,0");
            btnBack.Click += (s, e) => SendCommand("B,1,0");
            btnLeft.Click += (s, e) => SendCommand("TL,90");
            btnRight.Click += (s, e) => SendCommand("TR,90");
            btnStop.Click += (s, e) => SendCommand("S");
            btnRotLeft.Click += (s, e) => SendCommand("TL,45");
            btnRotRight.Click += (s, e) => SendCommand("TR,45");
            btnStrafeLeft.Click += (s, e) => SendCommand("SL,10");
            btnStrafeRight.Click += (s, e) => SendCommand("SR,10");

            // Speed control
            btnSpeedUp.Click += (s, e) => AdjustSpeed(10);
            btnSpeedDown.Click += (s, e) => AdjustSpeed(-10);
            trackSpeed.Scroll += (s, e) => UpdateSpeed();

            // Camera buttons
            btnTakePhoto.Click += (s, e) => TakePhoto();
            btnStartVideo.Click += (s, e) => StartVideo();
            btnStopVideo.Click += (s, e) => StopVideo();

            // Command log buttons
            btnClearLog.Click += (s, e) => ClearCommandLog();
            btnExportLog.Click += (s, e) => ExportCommandLog();
            btnSendCommands.Click += (s, e) => SendCommandsToRobot();

            // Auto execution
            btnStartAuto.Click += (s, e) => StartAutoExecution();
            btnPauseAuto.Click += (s, e) => PauseExecution();
            btnStopAuto.Click += (s, e) => StopExecution();
            btnReset.Click += (s, e) => ResetRobot();
            btnSettings.Click += (s, e) => ShowSettings();
            btnClose.Click += (s, e) => Close();

            // Form events
            this.FormClosing += OnFormClosingEventHandler;
        }
        #endregion
        private void OnFormClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            _telemetryTimer?.Stop();
            _telemetryTimer?.Dispose();
            StopCameraStream();
            _httpClient?.Dispose();
        }
        #region Private Methods - Connection
        /// <summary>
        /// Connects to robot and ESP32-CAM
        /// </summary>
        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            string ip = txtIpAddress.Text.Trim();
            int port = (int)nudPort.Value;

            if (string.IsNullOrEmpty(ip))
            {
                MessageBox.Show("Please enter a valid IP address.", "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblStatus.Text = "Connecting to robot...";

            // Simulate connection (replace with actual TCP connection)
            await Task.Delay(500);

            _isConnected = true;
            _txPackets = 0;
            _rxPackets = 0;
            _networkLatencyMs = 12;

            UpdateConnectionUI();
            UpdateNetworkStatusUI();

            // Start telemetry
            _telemetryTimer.Start();

            // Start ESP32-CAM stream
            StartCameraStream(ip);

            lblStatus.Text = $"Connected to {ip}:{port}";
            AddCommandLog("CONNECT", $"Connected to {ip}:{port}", "✅");
        }

        /// <summary>
        /// Disconnects from robot
        /// </summary>
        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            StopCameraStream();
            _telemetryTimer.Stop();

            _isConnected = false;
            UpdateConnectionUI();
            UpdateNetworkStatusUI();

            lblStatus.Text = "Disconnected";
            AddCommandLog("DISCONNECT", "Disconnected from robot", "✅");
        }

        /// <summary>
        /// Starts ESP32-CAM video stream
        /// </summary>
        private async void StartCameraStream(string ip)
        {
            string streamUrl = string.Format(ESP32_CAM_STREAM_URL, ip);

            _cameraCts = new CancellationTokenSource();
            _cameraStreamTask = Task.Run(async () =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(5);
                        var response = await client.GetAsync(streamUrl, _cameraCts.Token);

                        if (response.IsSuccessStatusCode)
                        {
                            var stream = await response.Content.ReadAsStreamAsync();
                            var buffer = new byte[1024];

                            while (!_cameraCts.Token.IsCancellationRequested)
                            {
                                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, _cameraCts.Token);
                                // Process JPEG stream (simplified)
                                await Task.Delay(33); // ~30 FPS
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Camera stream error: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Stops ESP32-CAM video stream
        /// </summary>
        private void StopCameraStream()
        {
            _cameraCts?.Cancel();
            _cameraStreamTask?.Wait(1000);
            _cameraCts?.Dispose();
        }

        /// <summary>
        /// Takes photo from ESP32-CAM
        /// </summary>
        private async void TakePhoto()
        {
            string ip = txtIpAddress.Text.Trim();
            string captureUrl = string.Format(ESP32_CAM_CAPTURE_URL, ip);

            try
            {
                var response = await _httpClient.GetAsync(captureUrl);
                if (response.IsSuccessStatusCode)
                {
                    var imageData = await response.Content.ReadAsByteArrayAsync();
                    using (var ms = new System.IO.MemoryStream(imageData))
                    {
                        var img = Image.FromStream(ms);
                        cameraView.Image = img;
                    }
                    AddCommandLog("PHOTO", "Photo captured", "✅");
                }
            }
            catch (Exception ex)
            {
                AddCommandLog("PHOTO", $"Failed: {ex.Message}", "❌");
            }
        }

        private void StartVideo() => AddCommandLog("VIDEO", "Video recording started", "▶️");
        private void StopVideo() => AddCommandLog("VIDEO", "Video recording stopped", "⏹️");
        #endregion

        #region Private Methods - Commands
        /// <summary>
        /// Sends a command to the robot
        /// </summary>
        private void SendCommand(string command)
        {
            if (!_isConnected)
            {
                lblStatus.Text = "Not connected. Please connect first.";
                return;
            }

            _txPackets++;
            UpdateNetworkStatusUI();
            AddCommandLog(command.Split(',')[0], command, "📤");

            // Simulate command execution (replace with actual TCP send)
            System.Diagnostics.Debug.WriteLine($"Sending command: {command}");
        }

        /// <summary>
        /// Adds command to log
        /// </summary>
        private void AddCommandLog(string type, string details, string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddCommandLog(type, details, status)));
                return;
            }

            var item = new ListViewItem(DateTime.Now.ToString("HH:mm:ss"));
            item.SubItems.Add(type);
            item.SubItems.Add(details);
            item.SubItems.Add(status);

            lvCommandLog.Items.Insert(0, item);

            // Limit log size
            while (lvCommandLog.Items.Count > COMMAND_LOG_MAX_ITEMS)
            {
                lvCommandLog.Items.RemoveAt(lvCommandLog.Items.Count - 1);
            }

            lblCommandCount.Text = $"Commands: {lvCommandLog.Items.Count}";
        }

        /// <summary>
        /// Clears command log
        /// </summary>
        private void ClearCommandLog()
        {
            lvCommandLog.Items.Clear();
            lblCommandCount.Text = "Commands: 0";
        }

        /// <summary>
        /// Exports command log to file
        /// </summary>
        private void ExportCommandLog()
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = $"RobotCommands_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var writer = new System.IO.StreamWriter(sfd.FileName))
                    {
                        writer.WriteLine("Time,Type,Details,Status");
                        foreach (ListViewItem item in lvCommandLog.Items)
                        {
                            writer.WriteLine($"{item.Text},{item.SubItems[1].Text},{item.SubItems[2].Text},{item.SubItems[3].Text}");
                        }
                    }
                    MessageBox.Show($"Exported to {sfd.FileName}", "Export Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Sends all commands to robot
        /// </summary>
        private void SendCommandsToRobot()
        {
            if (!_isConnected)
            {
                MessageBox.Show("Not connected to robot.", "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Implement batch command sending
            MessageBox.Show("Sending commands to robot...", "Send Commands",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Adjusts robot speed
        /// </summary>
        private void AdjustSpeed(int delta)
        {
            int newSpeed = trackSpeed.Value + delta;
            trackSpeed.Value = Math.Max(0, Math.Min(100, newSpeed));
            UpdateSpeed();
        }

        /// <summary>
        /// Updates robot speed
        /// </summary>
        private void UpdateSpeed()
        {
            int speed = trackSpeed.Value;
            lblSpeedValue.Text = $"{speed}%";
            SendCommand($"SP,{speed}");
        }
        #endregion

        #region Private Methods - Telemetry
        private void OnTelemetryTimerTick(object sender, EventArgs e)
        {
            if (!_isConnected) return;

            _rxPackets++;
            UpdateNetworkStatusUI();
            UpdateTelemetryDisplay();
        }

        private void UpdateTelemetryDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateTelemetryDisplay));
                return;
            }

            // Simulate telemetry data (replace with actual data)
            Random rand = new Random();
            _currentTelemetry.BatteryPercent = 85;
            _currentTelemetry.MotorTemp = 45;
            _currentTelemetry.CpuLoad = 35;
            _currentTelemetry.WifiSignal = -45;
            _currentTelemetry.FrontDistance = 25;
            _currentTelemetry.LeftDistance = 30;
            _currentTelemetry.RightDistance = 28;
            _currentTelemetry.BackDistance = 15;
            _currentTelemetry.Pitch = 0;
            _currentTelemetry.Roll = 0;
            _currentTelemetry.Yaw = 45;

            // Update UI
            _logic.UpdateTelemetryDisplay(_currentTelemetry, lblBattery, lblMotorTemp,
                lblCpuLoad, lblWifiSignal, lblUptime);

            frontSensor.Value = _currentTelemetry.FrontDistance;
            leftSensor.Value = _currentTelemetry.LeftDistance;
            rightSensor.Value = _currentTelemetry.RightDistance;
            backSensor.Value = _currentTelemetry.BackDistance;

            lblPitch.Text = $"Pitch: {_currentTelemetry.Pitch:F1}°";
            lblRoll.Text = $"Roll: {_currentTelemetry.Roll:F1}°";
            lblYaw.Text = $"Yaw: {_currentTelemetry.Yaw:F1}°";

            batteryBar.Value = (int)_currentTelemetry.BatteryPercent;

            // Update battery color
            if (_currentTelemetry.BatteryPercent < 10)
                batteryBar.ForeColor = Color.FromArgb(231, 76, 60);
            else if (_currentTelemetry.BatteryPercent < 20)
                batteryBar.ForeColor = Color.FromArgb(241, 196, 15);
            else
                batteryBar.ForeColor = Color.FromArgb(46, 204, 113);
        }
        #endregion

        #region Private Methods - Auto Execution
        private void StartAutoExecution()
        {
            _isExecuting = true;
            _isPaused = false;
            lblModeStatus.Text = "Mode: Auto (Running)";
            lblModeStatus.ForeColor = Color.Green;
        }

        private void PauseExecution()
        {
            if (_isExecuting)
            {
                _isPaused = true;
                lblModeStatus.Text = "Mode: Auto (Paused)";
                lblModeStatus.ForeColor = Color.Orange;
            }
        }

        private void StopExecution()
        {
            _isExecuting = false;
            _isPaused = false;
            SendCommand("S");
            lblModeStatus.Text = "Mode: Manual";
            lblModeStatus.ForeColor = Color.White;
        }

        private void ResetRobot()
        {
            SendCommand("R");
            AddCommandLog("RESET", "Robot reset", "🔄");
        }

        private void ShowSettings()
        {
            MessageBox.Show("Settings dialog will be implemented in future version.",
                "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Protected Methods
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _telemetryTimer?.Stop();
            _telemetryTimer?.Dispose();
            StopCameraStream();
            _httpClient?.Dispose();
            base.OnFormClosing(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.W:
                case Keys.Up:
                    SendCommand("F,1,0");
                    return true;
                case Keys.S:
                case Keys.Down:
                    SendCommand("B,1,0");
                    return true;
                case Keys.A:
                case Keys.Left:
                    SendCommand("TL,90");
                    return true;
                case Keys.D:
                case Keys.Right:
                    SendCommand("TR,90");
                    return true;
                case Keys.Space:
                    SendCommand("S");
                    return true;
                case Keys.Q:
                    SendCommand("TL,45");
                    return true;
                case Keys.E:
                    SendCommand("TR,45");
                    return true;
                case Keys.Oemplus:
                    AdjustSpeed(10);
                    return true;
                case Keys.OemMinus:
                    AdjustSpeed(-10);
                    return true;
                case Keys.F5:
                    StartAutoExecution();
                    return true;
                case Keys.F6:
                    PauseExecution();
                    return true;
                case Keys.F7:
                    StopExecution();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion
    }
}
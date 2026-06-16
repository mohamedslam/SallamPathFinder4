#region File Header
/// <summary>
/// File: RobotPanel.cs
/// Description: Panel for robot control and status display with dynamic charging support
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-12
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Services.Battery;
using SallamPathFinder4.Services.Simulation;
using System;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace SallamPathFinder4.WinForms.Panels
{
    #region Class Documentation
    /// <summary>
    /// Panel for robot control and status display
    /// Includes dynamic charging settings with countdown timer
    /// </summary>
    #endregion
    public sealed class RobotPanel : Panel
    {
        #region Constants
        private const int PANEL_HEIGHT = 380;
        private const int SPACING = 35;
        private const int CONTROL_WIDTH = 100;
        #endregion

        #region Private Fields - Basic Controls
        private Label _lblTitle;
        private Label _lblSpeed;
        private NumericUpDown _nudSpeed;
        private Label _lblWidth;
        private NumericUpDown _nudWidth;
        private Label _lblLength;
        private NumericUpDown _nudLength;
        private Label _lblBatterySet;
        private TrackBar _trbBattery;
        private Label _lblBatteryValue;
        private Button _btnSimulate;
        private Button _btnPause;
        private Button _btnStop;
        #endregion

        #region Private Fields - Obstacle Detection Settings
        private GroupBox _grpObstacleSettings;
        private CheckBox _chkEnableLearning;
        private CheckBox _chkEnableObstacleAvoidance;
        private Button _btnExportObstacleLog;
        private Button _btnClearObstacleMemory;
        private Label _lblObstacleStats;
        private NumericUpDown _nudSafeDistance;
        private NumericUpDown _nudCriticalDistance;
        private ComboBox _cboObstacleType;
        private NumericUpDown _nudWaitTime;
        private NumericUpDown _nudMaxWaitTime;
        private Button _btnApplyWaitTime;
        #endregion

        #region Private Fields - Dynamic Charging
        private CheckBox _chkDynamicCharging;
        private Label _lblChargingTime;
        private NumericUpDown _nudChargingMinutes;
        private NumericUpDown _nudChargingSeconds;
        private Label _lblMinutes;
        private Label _lblSeconds;
        private Label _lblSafetyMargin;
        private NumericUpDown _nudSafetyMargin;
        private Label _lblSafetyMarginUnit;
        private Label _lblChargingCountdown;
        private System.Windows.Forms. Timer _countdownTimer;
        private int _remainingChargingSeconds;
        #endregion

        #region Events
        public event EventHandler SimulateClick;
        public event EventHandler PauseClick;
        public event EventHandler StopClick;
        public event EventHandler BatteryLevelChanged;
        public event EventHandler ChargingSettingsChanged;
        public event Action ChargingCompleted;
        public event Action<double> SpeedChanged;
        public event Action<int, int, int> RobotDimensionsChanged;  // width, length, height

        #endregion

        #region Constructor
        public RobotPanel()
        {
            InitializeComponents();
            SetDefaultValues();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            this.Dock = DockStyle.Top;
            this.Height = PANEL_HEIGHT;
            this.Padding = new Padding(5);
            this.BackColor = Color.White;

            int y = 5;

            // Title
            _lblTitle = new Label
            {
                Text = "🤖 ROBOT CONTROL",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(5, y),
                AutoSize = true
            };
            this.Controls.Add(_lblTitle);
            y += 25;

            // Speed
            _lblSpeed = new Label { Text = "Speed (cm/s):", Location = new Point(5, y), AutoSize = true };
            _nudSpeed = new NumericUpDown
            {
                Location = new Point(120, y - 3),
                Width = CONTROL_WIDTH,
                Minimum = 1,
                Maximum = 100,
                Value = 10,
                DecimalPlaces = 1
            };
            _nudSpeed.ValueChanged += _nudSpeed_ValueChanged;
            this.Controls.Add(_lblSpeed);
            this.Controls.Add(_nudSpeed);
            y += SPACING;

            // Width
            _lblWidth = new Label { Text = "Width (cm):", Location = new Point(5, y), AutoSize = true };
            _nudWidth = new NumericUpDown
            {
                Location = new Point(120, y - 3),
                Width = CONTROL_WIDTH,
                Minimum = 1,
                Maximum = 200,
                Value = 20
            };
            this.Controls.Add(_lblWidth);
            this.Controls.Add(_nudWidth);
            y += SPACING;

            // Length
            _lblLength = new Label { Text = "Length (cm):", Location = new Point(5, y), AutoSize = true };
            _nudLength = new NumericUpDown
            {
                Location = new Point(120, y - 3),
                Width = CONTROL_WIDTH,
                Minimum = 1,
                Maximum = 200,
                Value = 20
            };
            this.Controls.Add(_lblLength);
            this.Controls.Add(_nudLength);
            y += SPACING;

            // ========== DYNAMIC CHARGING SECTION ==========
            var lblChargingSection = new Label
            {
                Text = "⚡ DYNAMIC CHARGING",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Location = new Point(5, y),
                AutoSize = true
            };
            this.Controls.Add(lblChargingSection);
            y += 22;

            // CheckBox Enable Dynamic Charging
            _chkDynamicCharging = new CheckBox
            {
                Text = "Enable Automatic Charging",
                Location = new Point(5, y),
                AutoSize = true,
                Checked = false
            };
            _chkDynamicCharging.CheckedChanged += OnDynamicChargingCheckedChanged;
            this.Controls.Add(_chkDynamicCharging);
            y += 25;

            // Charging Time (Minutes & Seconds)
            _lblChargingTime = new Label
            {
                Text = "Charging Time:",
                Location = new Point(5, y),
                AutoSize = true,
                Enabled = false
            };
            this.Controls.Add(_lblChargingTime);

            _nudChargingMinutes = new NumericUpDown
            {
                Location = new Point(120, y - 3),
                Width = 55,
                Minimum = 0,
                Maximum = 60,
                Value = 0,
                Enabled = false
            };
            this.Controls.Add(_nudChargingMinutes);
            _nudChargingMinutes.ValueChanged += OnChargingTimeChanged;

            _lblMinutes = new Label
            {
                Text = "min",
                Location = new Point(180, y),
                AutoSize = true,
                Enabled = false
            };
            this.Controls.Add(_lblMinutes);

            _nudChargingSeconds = new NumericUpDown
            {
                Location = new Point(215, y - 3),
                Width = 55,
                Minimum = 0,
                Maximum = 59,
                Value = 15,
                Enabled = false
            };
            this.Controls.Add(_nudChargingSeconds);
            _nudChargingSeconds.ValueChanged += OnChargingTimeChanged;

            _lblSeconds = new Label
            {
                Text = "sec",
                Location = new Point(275, y),
                AutoSize = true,
                Enabled = false
            };
            this.Controls.Add(_lblSeconds);
            y += 25;

            // Charging Countdown Display
            _lblChargingCountdown = new Label
            {
                Text = "⏳ Remaining: --:--",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Enabled = false
            };
            this.Controls.Add(_lblChargingCountdown);
            y += 22;

            // Safety Margin
            _lblSafetyMargin = new Label
            {
                Text = "Safety Margin:",
                Location = new Point(5, y),
                AutoSize = true,
                Enabled = false
            };
            this.Controls.Add(_lblSafetyMargin);

            _nudSafetyMargin = new NumericUpDown
            {
                Location = new Point(120, y - 3),
                Width = 60,
                Minimum = 5,
                Maximum = 20,
                Value = 10,
                DecimalPlaces = 0,
                Enabled = false
            };
            this.Controls.Add(_nudSafetyMargin);
            _nudSafetyMargin.ValueChanged += OnSafetyMarginChanged;

            _lblSafetyMarginUnit = new Label
            {
                Text = "%",
                Location = new Point(185, y),
                AutoSize = true,
                Enabled = false
            };
            this.Controls.Add(_lblSafetyMarginUnit);
            y += SPACING + 5;

            // Battery TrackBar
            _lblBatterySet = new Label { Text = "Set Battery:", Location = new Point(5, y), AutoSize = true };
            _trbBattery = new TrackBar
            {
                Location = new Point(120, y - 3),
                Width = CONTROL_WIDTH,
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                TickFrequency = 10
            };
            _lblBatteryValue = new Label { Text = "100%", Location = new Point(225, y - 3), AutoSize = true };
            _trbBattery.ValueChanged += OnBatteryTrackBarChanged;

            this.Controls.Add(_lblBatterySet);
            this.Controls.Add(_trbBattery);
            this.Controls.Add(_lblBatteryValue);
            y += SPACING + 10;

            // Control Buttons
            _btnSimulate = new Button
            {
                Text = "▶ Simulate",
                Location = new Point(5, y),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnSimulate.Click += OnSimulateClick;

            _btnPause = new Button
            {
                Text = "⏸ Pause",
                Location = new Point(100, y),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnPause.Click += OnPauseClick;

            _btnStop = new Button
            {
                Text = "⏹ Stop",
                Location = new Point(195, y),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnStop.Click += OnStopClick;

            this.Controls.Add(_btnSimulate);
            this.Controls.Add(_btnPause);
            this.Controls.Add(_btnStop);
            _nudWidth.ValueChanged += (s, e) => UpdateDimensions();
            _nudLength.ValueChanged += (s, e) => UpdateDimensions();

            // Create obstacle detection UI
            CreateObstacleSettingsPanel();
            CreateWaitTimeConfigPanel();
        }
        private void UpdateDimensions()
        {
            int width = (int)_nudWidth.Value;
            int length = (int)_nudLength.Value;
            int height = 30;  // قيمة ثابتة

            // استدعاء الحدث بأمان
            var handler = RobotDimensionsChanged;
            if (handler != null)
            {
                handler(width, length, height);
            }
        }
        private void _nudSpeed_ValueChanged(object? sender, EventArgs e)
        {
            decimal speed = _nudSpeed.Value;   
            double actualSpeed =(double ) speed;    
            SpeedChanged?.Invoke(actualSpeed); 
        }
        private void SetDefaultValues()
        {
            _chkDynamicCharging.Checked = false;
            _nudChargingMinutes.Value = 0;
            _nudChargingSeconds.Value = 10;
            _nudSafetyMargin.Value = 10;
        }
        #region Obstacle Detection UI

        /// <summary>
        /// Creates the obstacle detection settings panel
        /// </summary>
        private void CreateObstacleSettingsPanel()
        {
            int y = _btnStop.Bottom + 15;

            _grpObstacleSettings = new GroupBox
            {
                Text = "🚧 Obstacle Detection & Learning",
                Location = new Point(5, y),
                Size = new Size(310, 180),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            int gy = 20;

            // Enable Learning
            _chkEnableLearning = new CheckBox
            {
                Text = "Enable Learning Memory (SPPA-DL)",
                Location = new Point(10, gy),
                AutoSize = true,
                Checked = true
            };
            _grpObstacleSettings.Controls.Add(_chkEnableLearning);
            gy += 25;

            // Enable Obstacle Avoidance
            _chkEnableObstacleAvoidance = new CheckBox
            {
                Text = "Enable Obstacle Avoidance",
                Location = new Point(10, gy),
                AutoSize = true,
                Checked = true
            };
            _grpObstacleSettings.Controls.Add(_chkEnableObstacleAvoidance);
            gy += 25;

            // Safe Distance
            Label lblSafeDist = new Label
            {
                Text = "Safe Distance (cm):",
                Location = new Point(10, gy),
                Size = new Size(110, 23)
            };
            _nudSafeDistance = new NumericUpDown
            {
                Location = new Point(130, gy - 3),
                Size = new Size(60, 23),
                Minimum = 10,
                Maximum = 200,
                Value = 30,
                Increment = 5
            };
            _grpObstacleSettings.Controls.Add(lblSafeDist);
            _grpObstacleSettings.Controls.Add(_nudSafeDistance);
            gy += 28;

            // Critical Distance
            Label lblCriticalDist = new Label
            {
                Text = "Critical Distance (cm):",
                Location = new Point(10, gy),
                Size = new Size(110, 23)
            };
            _nudCriticalDistance = new NumericUpDown
            {
                Location = new Point(130, gy - 3),
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 50,
                Value = 10,
                Increment = 2
            };
            _grpObstacleSettings.Controls.Add(lblCriticalDist);
            _grpObstacleSettings.Controls.Add(_nudCriticalDistance);
            gy += 28;

            // Export Button
            _btnExportObstacleLog = new Button
            {
                Text = "📊 Export Obstacle Log",
                Location = new Point(10, gy),
                Size = new Size(140, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            _grpObstacleSettings.Controls.Add(_btnExportObstacleLog);

            // Clear Memory Button
            _btnClearObstacleMemory = new Button
            {
                Text = "🗑 Clear Learning Memory",
                Location = new Point(160, gy),
                Size = new Size(140, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            _grpObstacleSettings.Controls.Add(_btnClearObstacleMemory);
            gy += 35;

            // Statistics Label
            _lblObstacleStats = new Label
            {
                Text = "📈 Stats: Loading...",
                Location = new Point(10, gy),
                Size = new Size(290, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            _grpObstacleSettings.Controls.Add(_lblObstacleStats);

            this.Controls.Add(_grpObstacleSettings);
        }

        /// <summary>
        /// Creates wait time configuration panel for obstacle types
        /// </summary>
        private void CreateWaitTimeConfigPanel()
        {
            int y = _grpObstacleSettings.Bottom + 5;

            var grpWaitTimes = new GroupBox
            {
                Text = "⏱️ Wait Times by Obstacle Type",
                Location = new Point(5, y),
                Size = new Size(310, 100),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            int gy = 20;

            // Obstacle Type Combo
            Label lblType = new Label
            {
                Text = "Obstacle Type:",
                Location = new Point(10, gy),
                Size = new Size(90, 23)
            };
            _cboObstacleType = new ComboBox
            {
                Location = new Point(110, gy - 3),
                Size = new Size(120, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboObstacleType.Items.AddRange(new string[] { "Child", "Adult", "Animal", "OtherRobot", "Equipment" });
            _cboObstacleType.SelectedIndex = 0;
            grpWaitTimes.Controls.Add(lblType);
            grpWaitTimes.Controls.Add(_cboObstacleType);
            gy += 28;

            // Wait Time
            Label lblWait = new Label
            {
                Text = "Wait Time (sec):",
                Location = new Point(10, gy),
                Size = new Size(90, 23)
            };
            _nudWaitTime = new NumericUpDown
            {
                Location = new Point(110, gy - 3),
                Size = new Size(60, 23),
                Minimum = 0,
                Maximum = 30,
                Value = 3,
                DecimalPlaces = 1,
                Increment = 0.5M
            };
            grpWaitTimes.Controls.Add(lblWait);
            grpWaitTimes.Controls.Add(_nudWaitTime);

            // Max Wait Time
            Label lblMaxWait = new Label
            {
                Text = "Max Wait (sec):",
                Location = new Point(180, gy),
                Size = new Size(90, 23)
            };
            _nudMaxWaitTime = new NumericUpDown
            {
                Location = new Point(260, gy - 3),
                Size = new Size(40, 23),
                Minimum = 0,
                Maximum = 60,
                Value = 8,
                DecimalPlaces = 1,
                Increment = 0.5M
            };
            grpWaitTimes.Controls.Add(lblMaxWait);
            grpWaitTimes.Controls.Add(_nudMaxWaitTime);
            gy += 28;

            // Apply Button
            _btnApplyWaitTime = new Button
            {
                Text = "Apply Wait Time",
                Location = new Point(110, gy),
                Size = new Size(100, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            grpWaitTimes.Controls.Add(_btnApplyWaitTime);

            this.Controls.Add(grpWaitTimes);
        }

        /// <summary>
        /// Updates obstacle statistics display
        /// </summary>
        public void UpdateObstacleStats(string stats)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateObstacleStats(stats)));
                return;
            }

            if (_lblObstacleStats != null)
            {
                _lblObstacleStats.Text = stats;
            }
        }

        #endregion
        #endregion

        #region Public Properties - Basic
        public double RobotSpeed => (double)_nudSpeed.Value;
        public double RobotWidth => (double)_nudWidth.Value;
        public double RobotLength => (double)_nudLength.Value;
        public int SetBatteryLevel => _trbBattery.Value;
        #endregion

        #region Public Properties - Dynamic Charging
        public bool IsDynamicChargingEnabled => _chkDynamicCharging?.Checked ?? false;

        public int ChargingTimeSeconds
        {
            get
            {
                int minutes = (int)(_nudChargingMinutes?.Value ?? 0);
                int seconds = (int)(_nudChargingSeconds?.Value ?? 0);
                return (minutes * 60) + seconds;
            }
        }

        public double SafetyMarginPercent => (double)(_nudSafetyMargin?.Value ?? 10);
        #endregion

        #region Public Methods - Charging Countdown
        public void StartChargingCountdown(int totalSeconds)
        {
            _remainingChargingSeconds = totalSeconds;
            _lblChargingCountdown.Enabled = true;
            UpdateCountdownDisplay();

            if (_countdownTimer == null)
            {
                _countdownTimer = new System.Windows.Forms.Timer();
                _countdownTimer.Interval = 1000;
                _countdownTimer.Tick += OnCountdownTick;
            }
            _countdownTimer.Start();
        }

        public void StopChargingCountdown()
        {
            _countdownTimer?.Stop();
            _lblChargingCountdown.Text = "⏳ Remaining: --:--";
            _lblChargingCountdown.Enabled = false;
        }

        private void UpdateCountdownDisplay()
        {
            TimeSpan ts = TimeSpan.FromSeconds(_remainingChargingSeconds);
            _lblChargingCountdown.Text = $"⏳ Remaining: {ts:mm\\:ss}";
        }

        private void OnCountdownTick(object sender, EventArgs e)
        {
            if (_remainingChargingSeconds > 0)
            {
                _remainingChargingSeconds--;
                UpdateCountdownDisplay();

                if (_remainingChargingSeconds == 0)
                {
                    _countdownTimer.Stop();
                    ChargingCompleted?.Invoke();   
                }
            }
        }
        #endregion

        #region Public Methods - UI Updates
        

        /// <summary>
        /// Updates the battery display with percentage and battery count
        /// </summary>
        /// <param name="percentage">Current battery percentage (0-100)</param>
        public void UpdateBattery(double percentage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateBattery(percentage)));
                return;
            }

            // Use battery service to format with battery units (assuming 3 batteries = 100%)
            var batteryService = new BatteryService();
            string batteryText = batteryService.FormatBatteryWithBatteries(percentage, 3.0);

            if (_lblBatteryValue != null)
            {
                _lblBatteryValue.Text = batteryText;
            }

            // Change color based on battery level
            if (percentage < 10)
            {
                if (_lblBatteryValue != null) _lblBatteryValue.ForeColor = Color.FromArgb(231, 76, 60);
            }
            else if (percentage < 20)
            {
                if (_lblBatteryValue != null) _lblBatteryValue.ForeColor = Color.FromArgb(241, 196, 15);
            }
            else
            {
                if (_lblBatteryValue != null) _lblBatteryValue.ForeColor = Color.Gray;
            }
        }

       

        public void SetButtonStates(bool isSimulating, bool isPaused)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetButtonStates(isSimulating, isPaused)));
                return;
            }

            _btnSimulate.Enabled = !isSimulating;
            _btnPause.Enabled = isSimulating && !isPaused;
            _btnStop.Enabled = isSimulating;
        }

        public void SetDynamicChargingEnabled(bool enabled)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetDynamicChargingEnabled(enabled)));
                return;
            }

            _chkDynamicCharging.Checked = enabled;
        }
        #endregion

        #region Private Methods - Event Handlers
   
        private void OnSimulateClick(object sender, EventArgs e)
        {
            SimulateClick?.Invoke(this, EventArgs.Empty);
        }

        private void OnPauseClick(object sender, EventArgs e)
        {
            PauseClick?.Invoke(this, EventArgs.Empty);
        }

        private void OnStopClick(object sender, EventArgs e)
        {
            StopClick?.Invoke(this, EventArgs.Empty);
        }

        private void OnBatteryTrackBarChanged(object sender, EventArgs e)
        {
            int value = _trbBattery.Value;
            _lblBatteryValue.Text = $"{value}%";
            BatteryLevelChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnDynamicChargingCheckedChanged(object sender, EventArgs e)
        {
            bool enabled = _chkDynamicCharging.Checked;

            _lblChargingTime.Enabled = enabled;
            _nudChargingMinutes.Enabled = enabled;
            _lblMinutes.Enabled = enabled;
            _nudChargingSeconds.Enabled = enabled;
            _lblSeconds.Enabled = enabled;
            _lblSafetyMargin.Enabled = enabled;
            _nudSafetyMargin.Enabled = enabled;
            _lblSafetyMarginUnit.Enabled = enabled;

            if (!enabled)
            {
                StopChargingCountdown();
            }

            OnChargingSettingsChanged();
        }

        private void OnChargingTimeChanged(object sender, EventArgs e)
        {
            OnChargingSettingsChanged();
        }

        private void OnSafetyMarginChanged(object sender, EventArgs e)
        {
            OnChargingSettingsChanged();
        }

        private void OnChargingSettingsChanged()
        {
            ChargingSettingsChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

     
        /// <summary>
        /// Public method to trigger dimensions update from external code
        /// </summary>
        public void UpdateDimensionsExternally(int width, int length, int height)
        {
            // تحديث قيم الـ NumericUpDown
            if (_nudWidth != null) _nudWidth.Value = width;
            if (_nudLength != null) _nudLength.Value = length;

            // استدعاء الدالة الداخلية التي تطلق الحدث
            UpdateDimensions();
        }
      
        /// <summary>
        /// Event args for robot dimensions
        /// </summary>
        public class RobotDimensionsEventArgs : EventArgs
        {
            public int WidthCm { get; }
            public int LengthCm { get; }
            public int HeightCm { get; }

            public RobotDimensionsEventArgs(int widthCm, int lengthCm, int heightCm)
            {
                WidthCm = widthCm;
                LengthCm = lengthCm;
                HeightCm = heightCm;
            }
        }

        #region Obstacle Wait State Display

        /// <summary>
        /// Updates the wait state display when robot is waiting for an obstacle
        /// </summary>
        /// <param name="waitState">Current wait state</param>
        public void UpdateWaitStateDisplay(ObstacleWaitState waitState)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateWaitStateDisplay(waitState)));
                return;
            }

            if (waitState != null && waitState.IsWaiting)
            {
                // Update status display
                string waitText = $"⏱️ Waiting for {waitState.Type} - {waitState.RemainingWaitTime:F1}s remaining";

                // Find or create wait label
                Label lblWait = this.Controls.Find("_lblWaitStatus", true).FirstOrDefault() as Label;
                if (lblWait == null)
                {
                    lblWait = new Label
                    {
                        Name = "_lblWaitStatus",
                        Location = new Point(5, _btnStop.Bottom + 5),
                        Size = new Size(300, 25),
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        ForeColor = Color.FromArgb(241, 196, 15),
                        BackColor = Color.FromArgb(50, 0, 0, 0),
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    this.Controls.Add(lblWait);
                }

                lblWait.Text = waitText;
                lblWait.Visible = true;

                // Update battery display with wait info
                var batteryLabel = this.Controls.Find("_lblBatteryValue", true).FirstOrDefault() as Label;
                if (batteryLabel != null)
                {
                    batteryLabel.Text = $"⏱️ Wait: {waitState.RemainingWaitTime:F1}s";
                }
            }
            else
            {
                // Hide wait label when not waiting
                var lblWait = this.Controls.Find("_lblWaitStatus", true).FirstOrDefault() as Label;
                if (lblWait != null)
                {
                    lblWait.Visible = false;
                }
            }
        }

        #endregion
    }
}
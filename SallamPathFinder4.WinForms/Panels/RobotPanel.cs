#region File Header
/// <summary>
/// File: RobotPanel.cs
/// Description: Panel for robot control and status display with dynamic charging support
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-12
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Services.Battery;
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
            this.Controls.Add(_lblSpeed);
            this.Controls.Add(_nudSpeed);
            y += SPACING;

            // Width
            _lblWidth = new Label { Text = "Width (cm):", Location = new Point(5, y), AutoSize = true };
            _nudWidth = new NumericUpDown
            {
                Location = new Point(120, y - 3),
                Width = CONTROL_WIDTH,
                Minimum = 20,
                Maximum = 200,
                Value = 60
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
                Minimum = 20,
                Maximum = 200,
                Value = 60
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
        }

        private void SetDefaultValues()
        {
            _chkDynamicCharging.Checked = false;
            _nudChargingMinutes.Value = 0;
            _nudChargingSeconds.Value = 15;
            _nudSafetyMargin.Value = 10;
        }
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
    }
}
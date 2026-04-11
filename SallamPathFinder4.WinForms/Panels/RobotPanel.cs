#region File Header
/// <summary>
/// File: RobotPanel.cs
/// Description: Panel for robot control and status display
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
#endregion

namespace SallamPathFinder4.WinForms.Panels
{
    public sealed class RobotPanel : Panel
    {
        #region Private Fields
        private NumericUpDown _nudSpeed;
        private NumericUpDown _nudWidth;
        private NumericUpDown _nudLength; 
         private Label _lblTitle;
        private Button _btnSimulate;
        private Button _btnPause;
        private Button _btnStop;
        private TrackBar _trbBattery;
        private Label _lblBatterySet;
        private Label _lblBatteryValue;

        #endregion
        #region Private Fields - Dynamic Charging
        private CheckBox _chkDynamicCharging;
        private Label _lblChargingTime;
        private DateTimePicker _dtpChargingTime;
        private Label _lblSafetyMargin;
        private NumericUpDown _nudSafetyMargin;
        private Label _lblSafetyMarginUnit;
        #endregion
        #region Constants
        private const int PANEL_HEIGHT = 320;   
        private const int SPACING = 35;
        private const int CONTROL_WIDTH = 100;
        private const int TIME_PICKER_WIDTH = 80;
        #endregion

        #region Events
        public event EventHandler SimulateClick;
        public event EventHandler PauseClick;
        public event EventHandler StopClick;   
        public event EventHandler BatteryLevelChanged;
        public event EventHandler ResumeClick;

        public int SetBatteryLevel => _trbBattery.Value;

        #endregion

        #region Properties
        public double RobotSpeed => (double)_nudSpeed.Value;
        public double RobotWidth => (double)_nudWidth.Value;
        public double RobotLength => (double)_nudLength.Value;
        #endregion
        #region Public Properties - Dynamic Charging
        public bool IsDynamicChargingEnabled => _chkDynamicCharging?.Checked ?? false;

        public int ChargingTimeSeconds
        {
            get
            {
                if (_dtpChargingTime == null) return 1800;
                TimeSpan ts = _dtpChargingTime.Value.TimeOfDay;
                return (int)ts.TotalSeconds;
            }
        }

        public double SafetyMarginPercent => (double)(_nudSafetyMargin?.Value ?? 10);
        #endregion
        #region Constructor
        public RobotPanel()
        {
            InitializeComponents();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            this.Dock = DockStyle.Top;
            this.Height = PANEL_HEIGHT;
            this.Padding = new Padding(5);

            int y = 5;

            _lblTitle = new Label
            {
                Text = "🤖 ROBOT CONTROL",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(5, y),
                AutoSize = true
            };
            this.Controls.Add(_lblTitle);
            y += 25;

            var lblSpeed = new Label { Text = "Speed (cm/s):", Location = new Point(5, y), AutoSize = true };
            _nudSpeed = new NumericUpDown
            {
                Location = new Point(120, y - 3),
                Width = CONTROL_WIDTH,
                Minimum = 1,
                Maximum = 100,
                Value = 10,
                DecimalPlaces = 1
            };
            this.Controls.Add(lblSpeed);
            this.Controls.Add(_nudSpeed);
            y += SPACING;

            var lblWidth = new Label { Text = "Width (cm):", Location = new Point(5, y), AutoSize = true };
            _nudWidth = new NumericUpDown
            {
                Location = new Point(120, y - 3),
                Width = CONTROL_WIDTH,
                Minimum = 20,
                Maximum = 200,
                Value = 60
            };
            this.Controls.Add(lblWidth);
            this.Controls.Add(_nudWidth);
            y += SPACING;

            var lblLength = new Label { Text = "Length (cm):", Location = new Point(5, y), AutoSize = true };
            _nudLength = new NumericUpDown
            {
                Location = new Point(120, y - 3),
                Width = CONTROL_WIDTH,
                Minimum = 20,
                Maximum = 200,
                Value = 60
            };
            this.Controls.Add(lblLength);
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

            // Charging Time (HH:MM:SS)
            _lblChargingTime = new Label
            {
                Text = "Charging Time:",
                Location = new Point(5, y),
                AutoSize = true,
                Enabled = false
            };
            this.Controls.Add(_lblChargingTime);

            _dtpChargingTime = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = DateTime.Today.AddHours(0).AddMinutes(30), // 00:30:00
                Location = new Point(120, y - 3),
                Width = TIME_PICKER_WIDTH,
                Enabled = false
            };
            this.Controls.Add(_dtpChargingTime);

            var lblHoursHint = new Label
            {
                Text = "(HH:MM:SS)",
                Location = new Point(205, y),
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 7),
                Enabled = false
            };
            this.Controls.Add(lblHoursHint);
            y += SPACING - 10;

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

            _lblSafetyMarginUnit = new Label
            {
                Text = "%",
                Location = new Point(185, y),
                AutoSize = true,
                Enabled = false
            };
            this.Controls.Add(_lblSafetyMarginUnit);
            y += SPACING;

            // ========== Battery TrackBar (الموجود) ==========
            _trbBattery = new TrackBar
            {
                Location = new Point(120, y - 3),
                Width = CONTROL_WIDTH,
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                TickFrequency = 10
            };
            _lblBatterySet = new Label { Text = "Set Battery:", Location = new Point(5, y), AutoSize = true };
            _lblBatteryValue = new Label { Text = "100%", Location = new Point(225, y - 3), AutoSize = true };
            _trbBattery.ValueChanged += (s, e) => BatteryLevelChanged?.Invoke(this, EventArgs.Empty);

            this.Controls.Add(_lblBatterySet);
            this.Controls.Add(_trbBattery);
            this.Controls.Add(_lblBatteryValue);
            y += SPACING + 10; 

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

            _btnSimulate.Click += (s, e) => SimulateClick?.Invoke(s, e);
            _btnPause.Click += (s, e) => PauseClick?.Invoke(s, e);
            _btnStop.Click += (s, e) => StopClick?.Invoke(s, e);

            this.Controls.Add(_btnSimulate);
            this.Controls.Add(_btnPause);
            this.Controls.Add(_btnStop);
        }
        #endregion

        #region Public Methods
        public void UpdateBattery(double percentage)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateBattery(percentage)));
                return;
            }

            _trbBattery.Value = (int)percentage;
            _lblBatterySet.Text = $"{percentage:F1}%";

            if (percentage < 10)
                _lblBatterySet.ForeColor = Color.FromArgb(231, 76, 60);
            else if (percentage < 20)
                _lblBatterySet.ForeColor = Color.FromArgb(241, 196, 15);
            else
                _lblBatterySet.ForeColor = Color.Gray;
        }

        public int GetBatteryLevel()
        {
            return _trbBattery?.Value ?? 100;
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
        #endregion
        #region Private Methods - Event Handlers
        private void OnDynamicChargingCheckedChanged(object sender, EventArgs e)
        {
            bool enabled = _chkDynamicCharging.Checked;

            _lblChargingTime.Enabled = enabled;
            _dtpChargingTime.Enabled = enabled;
            _lblSafetyMargin.Enabled = enabled;
            _nudSafetyMargin.Enabled = enabled;
            _lblSafetyMarginUnit.Enabled = enabled;

            // تحديث النص في شريط الحالة
            if (enabled)
            {
                System.Diagnostics.Debug.WriteLine("[RobotPanel] Dynamic Charging ENABLED");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[RobotPanel] Dynamic Charging DISABLED (manual battery replacement mode)");
            }
        }
        #endregion
    }
}
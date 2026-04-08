#region File Header
/// <summary>
/// File: RobotPanel.cs
/// Description: Panel for robot control and status display
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.Windows.Forms;
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
        #endregion

        #region Constants
        private const int PANEL_HEIGHT = 220;
        private const int SPACING = 35;
        private const int CONTROL_WIDTH = 100;
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
            _lblBatterySet = new Label { Text = "100%", Location = new Point(225, y - 3), AutoSize = true };
            _trbBattery.ValueChanged += (s, e) => BatteryLevelChanged?.Invoke(this, EventArgs.Empty);

            this.Controls.Add(_lblBatterySet);
            this.Controls.Add(_trbBattery);
            this.Controls.Add(_lblBatterySet);
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
    }
}
#region File Header
/// <summary>
/// File: frmRobotDesigner.cs
/// Description: Professional robot designer form with shape selection, color picker, and sensor management
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-29
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.WinForms.Controls;

namespace SallamPathFinder4.WinForms.Forms.RobotDesigner
{
    public partial class frmRobotDesigner : Form
    {
        #region Private Fields
        private RobotDefinition _robot;
        private RobotDesignerCanvas _canvas;
        private bool _isNewRobot;
        private SimpleSensor _selectedSensor;
        // Panels
        private Panel _propertiesPanel;
        private Panel _sensorsPanel;

        // Properties Controls
        private TextBox _txtRobotName;
        private TextBox _txtDescription;
        private ComboBox _cboRobotType;
        private ComboBox _cboBodyShape;
        private NumericUpDown _nudWidth;
        private NumericUpDown _nudHeight;
        private NumericUpDown _nudLength;
        private NumericUpDown _nudMaxSpeed;
        private NumericUpDown _nudTurnRate;
        private NumericUpDown _nudTurnRadius;
        private NumericUpDown _nudAcceleration;
        private ColorDialog _colorDialog;
        private Button _btnBodyColor;
        private Button _btnSecondaryColor;
        private Panel _pnlColorPreview;
        private Panel _pnlSecondaryColorPreview;
        private CheckBox _chkShowWheels;
        private CheckBox _chkShowDirectionArrow;
        private CheckBox _chkShowSensorFOV;
        private TrackBar _trbOpacity;
        private Label _lblOpacityValue;

        // Sensors Controls
        private ListBox _lstSensors;
        private ComboBox _cboSensorType;
        private NumericUpDown _nudSensorX;
        private NumericUpDown _nudSensorY;
        private NumericUpDown _nudSensorAngle;
        private NumericUpDown _nudSensorFOV;
        private NumericUpDown _nudSensorRange;
        private TrackBar _trackSensorAngle;
        private Label _lblSensorAngleValue;
        private CheckBox _chkSensorEnabled;
        private Button _btnAddSensor;
        private Button _btnRemoveSensor;
        private Button _btnEditSensor;

        // Bottom Buttons
        private Button _btnSave;
        private Button _btnCancel;
        private Button _btnExport;
        private Button _btnImport;
        private Button _btnDuplicate;
        #endregion

        #region Properties
        public RobotDefinition Robot => _robot;
        #endregion

        #region Constructor
        public frmRobotDesigner()
        {
            _robot = new RobotDefinition();
            _robot.RobotName = "New Robot";
            _isNewRobot = true;
            InitializeComponent();
            InitializeCanvas();
            WireEvents();
            UpdateRobotDisplay();
        }

        public frmRobotDesigner(RobotDefinition existingRobot)
        {
            _robot = existingRobot?.Clone() ?? new RobotDefinition();
            _isNewRobot = existingRobot == null;
            InitializeComponent();
            InitializeCanvas();
            LoadRobotData();
            UpdateRobotDisplay();
        }
        #endregion

        #region Initialization - Layout
        private void InitializeComponent()
        {
            this.Text = "Robot Designer - Professional Edition";
            this.Size = new Size(1400, 850);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.MinimumSize = new Size(1100, 700);

            _colorDialog = new ColorDialog();

            CreateMainLayout();
            CreatePropertiesPanel();
            CreateSensorsPanel();
            CreateBottomButtons();
        }

        private void CreateMainLayout()
        {
            var mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 800,
                SplitterWidth = 6
            };

            // Canvas Panel with Toolbar
            var canvasPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(240, 242, 245)
            };

            var toolbar = CreateToolbar();
            canvasPanel.Controls.Add(toolbar);

            _canvas = new RobotDesignerCanvas
            {
                Dock = DockStyle.Fill,
                CurrentRobot = _robot,
                BackColor = Color.White,
                Location = new Point(0, 50)
            };
            canvasPanel.Controls.Add(_canvas);

            // Right Panel with Tabs
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            var tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            var tabProperties = new TabPage("⚙️ Properties");
            _propertiesPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), AutoScroll = true };
            tabProperties.Controls.Add(_propertiesPanel);

            var tabSensors = new TabPage("📡 Sensors");
            _sensorsPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), AutoScroll = true };
            tabSensors.Controls.Add(_sensorsPanel);

            var tabStats = new TabPage("📊 Statistics");
            tabStats.Controls.Add(CreateStatsPanel());

            tabControl.TabPages.Add(tabProperties);
            tabControl.TabPages.Add(tabSensors);
            tabControl.TabPages.Add(tabStats);
            rightPanel.Controls.Add(tabControl);

            mainSplit.Panel1.Controls.Add(canvasPanel);
            mainSplit.Panel2.Controls.Add(rightPanel);

            this.Controls.Add(mainSplit);
        }

        private Panel CreateStatsPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), AutoScroll = true };
            int y = 10;

            var lblStats = new Label
            {
                Text = "Robot Statistics",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(10, y),
                Size = new Size(300, 30)
            };
            panel.Controls.Add(lblStats);
            y += 40;

            var lblManeuverability = new Label
            {
                Text = "Maneuverability Score: --",
                Location = new Point(10, y),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10)
            };
            panel.Controls.Add(lblManeuverability);
            y += 30;

            var lblSensorCount = new Label
            {
                Text = "Sensors: 0",
                Location = new Point(10, y),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10)
            };
            panel.Controls.Add(lblSensorCount);
            y += 30;

            var lblDimensions = new Label
            {
                Text = "Dimensions: -- x -- x -- cm",
                Location = new Point(10, y),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10)
            };
            panel.Controls.Add(lblDimensions);
            y += 30;

            var lblSpeed = new Label
            {
                Text = "Max Speed: -- m/s",
                Location = new Point(10, y),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10)
            };
            panel.Controls.Add(lblSpeed);
            y += 30;

            var lblTurnRate = new Label
            {
                Text = "Turn Rate: -- °/s",
                Location = new Point(10, y),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10)
            };
            panel.Controls.Add(lblTurnRate);

            panel.Tag = new { lblManeuverability, lblSensorCount, lblDimensions, lblSpeed, lblTurnRate };

            return panel;
        }

        private void UpdateStatsPanel()
        {
            var statsPanel = FindStatsPanel();
            if (statsPanel?.Tag == null) return;

            var data = statsPanel.Tag;
            var lblManeuverability = data.GetType().GetProperty("lblManeuverability")?.GetValue(data) as Label;
            var lblSensorCount = data.GetType().GetProperty("lblSensorCount")?.GetValue(data) as Label;
            var lblDimensions = data.GetType().GetProperty("lblDimensions")?.GetValue(data) as Label;
            var lblSpeed = data.GetType().GetProperty("lblSpeed")?.GetValue(data) as Label;
            var lblTurnRate = data.GetType().GetProperty("lblTurnRate")?.GetValue(data) as Label;

            if (lblManeuverability != null)
                lblManeuverability.Text = $"Maneuverability Score: {_robot.Kinematics.GetManeuverabilityScore():F0}%";
            if (lblSensorCount != null)
                lblSensorCount.Text = $"Sensors: {_robot.Sensors.Count}";
            if (lblDimensions != null)
                lblDimensions.Text = $"Dimensions: {_robot.Appearance.Width:F0} x {_robot.Appearance.Height:F0} x {_robot.Appearance.Length:F0} cm";
            if (lblSpeed != null)
                lblSpeed.Text = $"Max Speed: {_robot.Kinematics.MaxForwardSpeed:F1} m/s";
            if (lblTurnRate != null)
                lblTurnRate.Text = $"Turn Rate: {_robot.Kinematics.MaxTurnRate:F0} °/s";
        }

        private Panel FindStatsPanel()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is SplitContainer split)
                {
                    foreach (Control panelCtrl in split.Panel2.Controls)
                    {
                        if (panelCtrl is TabControl tabs)
                        {
                            foreach (TabPage page in tabs.TabPages)
                            {
                                if (page.Text == "📊 Statistics")
                                {
                                    return page.Controls.OfType<Panel>().FirstOrDefault();
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private Panel CreateToolbar()
        {
            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(5)
            };

            // Zoom buttons
            var btnZoomIn = new Button { Text = "🔍+", Location = new Point(10, 10), Size = new Size(40, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(44, 62, 80), ForeColor = Color.White };
            btnZoomIn.Click += (s, e) => _canvas.Zoom += 0.1f;

            var btnZoomOut = new Button { Text = "🔍-", Location = new Point(55, 10), Size = new Size(40, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(44, 62, 80), ForeColor = Color.White };
            btnZoomOut.Click += (s, e) => _canvas.Zoom -= 0.1f;

            var btnResetView = new Button { Text = "Reset View", Location = new Point(105, 10), Size = new Size(85, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White };
            btnResetView.Click += (s, e) => _canvas.ResetView();

            // Angle control
            var lblAngle = new Label { Text = "Angle:", Location = new Point(210, 15), Size = new Size(45, 25), ForeColor = Color.White };
            var nudAngle = new NumericUpDown { Location = new Point(260, 12), Size = new Size(60, 25), Minimum = -360, Maximum = 360, Value = 0, Increment = 15 };
            nudAngle.ValueChanged += (s, e) => _canvas.PreviewAngle = (float)nudAngle.Value;

            var btnRotateLeft = new Button { Text = "◀", Location = new Point(330, 10), Size = new Size(35, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(44, 62, 80), ForeColor = Color.White };
            btnRotateLeft.Click += (s, e) => { nudAngle.Value -= 15; };

            var btnRotateRight = new Button { Text = "▶", Location = new Point(370, 10), Size = new Size(35, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(44, 62, 80), ForeColor = Color.White };
            btnRotateRight.Click += (s, e) => { nudAngle.Value += 15; };

            // View options
            var btnToggleGrid = new Button { Text = "Toggle Grid", Location = new Point(430, 10), Size = new Size(85, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(44, 62, 80), ForeColor = Color.White };
            btnToggleGrid.Click += (s, e) => _canvas.ShowGrid = !_canvas.ShowGrid;

            var btnToggleFOV = new Button { Text = "Toggle FOV", Location = new Point(525, 10), Size = new Size(85, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(44, 62, 80), ForeColor = Color.White };
            btnToggleFOV.Click += (s, e) => _canvas.ShowSensorFOV = !_canvas.ShowSensorFOV;

            toolbar.Controls.Add(btnZoomIn);
            toolbar.Controls.Add(btnZoomOut);
            toolbar.Controls.Add(btnResetView);
            toolbar.Controls.Add(lblAngle);
            toolbar.Controls.Add(nudAngle);
            toolbar.Controls.Add(btnRotateLeft);
            toolbar.Controls.Add(btnRotateRight);
            toolbar.Controls.Add(btnToggleGrid);
            toolbar.Controls.Add(btnToggleFOV);

            return toolbar;
        }
        #endregion

        #region Initialization - Properties Panel
        private void CreatePropertiesPanel()
        {
            int y = 10;

            // Robot Name
            var lblName = new Label { Text = "Robot Name:", Location = new Point(10, y), Size = new Size(100, 25), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            _txtRobotName = new TextBox { Location = new Point(115, y), Size = new Size(200, 25), Text = _robot.RobotName };
            _propertiesPanel.Controls.Add(lblName);
            _propertiesPanel.Controls.Add(_txtRobotName);
            y += 35;

            // Description
            var lblDesc = new Label { Text = "Description:", Location = new Point(10, y), Size = new Size(100, 25), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            _txtDescription = new TextBox { Location = new Point(115, y), Size = new Size(200, 25), Text = _robot.Description };
            _propertiesPanel.Controls.Add(lblDesc);
            _propertiesPanel.Controls.Add(_txtDescription);
            y += 35;

            // Separator
            _propertiesPanel.Controls.Add(CreateSeparator(y));
            y += 15;

            // Quick Type Buttons
            var grpQuickTypes = new GroupBox { Text = "Quick Select", Location = new Point(10, y), Size = new Size(310, 45) };
            AddQuickTypeButton(grpQuickTypes, "Wheeled", 10);
            AddQuickTypeButton(grpQuickTypes, "Tracked", 85);
            AddQuickTypeButton(grpQuickTypes, "Drone", 160);
            AddQuickTypeButton(grpQuickTypes, "Omni", 235);
            _propertiesPanel.Controls.Add(grpQuickTypes);
            y += 55;

            // Robot Type Combo
            var lblType = new Label { Text = "Robot Type:", Location = new Point(10, y), Size = new Size(100, 25), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            _cboRobotType = new ComboBox { Location = new Point(115, y), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            _cboRobotType.Items.AddRange(new[] { "Wheeled", "Tracked", "Flying", "Humanoid", "Omnidirectional", "Custom" });
            _cboRobotType.SelectedItem = _robot.RobotType.ToString();
            _propertiesPanel.Controls.Add(lblType);
            _propertiesPanel.Controls.Add(_cboRobotType);
            y += 35;

            // Body Shape
            var lblShape = new Label { Text = "Body Shape:", Location = new Point(10, y), Size = new Size(100, 25), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            _cboBodyShape = new ComboBox { Location = new Point(115, y), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            _cboBodyShape.Items.AddRange(new[] { "Rectangle", "Square", "Circle", "Rounded Rect", "Triangle", "Hexagon" });
            _cboBodyShape.SelectedIndex = (int)_robot.Appearance.ShapeType;
            _propertiesPanel.Controls.Add(lblShape);
            _propertiesPanel.Controls.Add(_cboBodyShape);
            y += 35;

            // Separator
            _propertiesPanel.Controls.Add(CreateSeparator(y));
            y += 15;

            // Dimensions Group
            var grpDimensions = new GroupBox { Text = "Dimensions (cm)", Location = new Point(10, y), Size = new Size(310, 110) };

            var lblWidth = new Label { Text = "Width:", Location = new Point(15, 28), Size = new Size(60, 25) };
            _nudWidth = new NumericUpDown { Location = new Point(80, 26), Size = new Size(80, 23), Minimum = 20, Maximum = 200, Value = (decimal)_robot.Appearance.Width, DecimalPlaces = 1 };

            var lblHeight = new Label { Text = "Height:", Location = new Point(180, 28), Size = new Size(60, 25) };
            _nudHeight = new NumericUpDown { Location = new Point(240, 26), Size = new Size(60, 23), Minimum = 20, Maximum = 200, Value = (decimal)_robot.Appearance.Height, DecimalPlaces = 1 };

            var lblLength = new Label { Text = "Length:", Location = new Point(15, 65), Size = new Size(60, 25) };
            _nudLength = new NumericUpDown { Location = new Point(80, 63), Size = new Size(80, 23), Minimum = 20, Maximum = 200, Value = (decimal)_robot.Appearance.Length, DecimalPlaces = 1 };

            grpDimensions.Controls.Add(lblWidth);
            grpDimensions.Controls.Add(_nudWidth);
            grpDimensions.Controls.Add(lblHeight);
            grpDimensions.Controls.Add(_nudHeight);
            grpDimensions.Controls.Add(lblLength);
            grpDimensions.Controls.Add(_nudLength);
            _propertiesPanel.Controls.Add(grpDimensions);
            y += 120;

            // Colors
            var lblColor = new Label { Text = "Body Color:", Location = new Point(10, y), Size = new Size(80, 25), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            _btnBodyColor = new Button { Text = "Choose", Location = new Point(95, y), Size = new Size(70, 25), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, Cursor = Cursors.Hand };
            _pnlColorPreview = new Panel { Location = new Point(170, y), Size = new Size(30, 25), BackColor = Color.FromArgb(52, 152, 219), BorderStyle = BorderStyle.FixedSingle };
            _propertiesPanel.Controls.Add(lblColor);
            _propertiesPanel.Controls.Add(_btnBodyColor);
            _propertiesPanel.Controls.Add(_pnlColorPreview);
            y += 35;

            // Opacity - using TrackBar
            var lblOpacity = new Label { Text = "Opacity:", Location = new Point(10, y), Size = new Size(60, 25), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            _trbOpacity = new TrackBar { Location = new Point(75, y - 5), Size = new Size(150, 30), Minimum = 0, Maximum = 100, Value = (int)(_robot.Appearance.Opacity * 100) };
            _lblOpacityValue = new Label { Text = $"{_robot.Appearance.Opacity * 100:F0}%", Location = new Point(235, y), Size = new Size(50, 25), TextAlign = ContentAlignment.MiddleLeft };
            _propertiesPanel.Controls.Add(lblOpacity);
            _propertiesPanel.Controls.Add(_trbOpacity);
            _propertiesPanel.Controls.Add(_lblOpacityValue);
            y += 40;

            // Separator
            _propertiesPanel.Controls.Add(CreateSeparator(y));
            y += 15;

            // Movement Group
            var grpMovement = new GroupBox { Text = "Movement", Location = new Point(10, y), Size = new Size(310, 105) };

            var lblSpeed = new Label { Text = "Max Speed (m/s):", Location = new Point(15, 28), Size = new Size(100, 25) };
            _nudMaxSpeed = new NumericUpDown { Location = new Point(120, 26), Size = new Size(80, 23), Minimum = 0.5m, Maximum = 5, Value = (decimal)_robot.Kinematics.MaxForwardSpeed, DecimalPlaces = 1, Increment = 0.1m };

            var lblTurnRate = new Label { Text = "Turn Rate (°/s):", Location = new Point(15, 55), Size = new Size(100, 25) };
            _nudTurnRate = new NumericUpDown { Location = new Point(120, 53), Size = new Size(80, 23), Minimum = 30, Maximum = 360, Value = (decimal)_robot.Kinematics.MaxTurnRate };

            var lblTurnRadius = new Label { Text = "Turn Radius (cm):", Location = new Point(15, 82), Size = new Size(100, 25) };
            _nudTurnRadius = new NumericUpDown { Location = new Point(120, 80), Size = new Size(80, 23), Minimum = 10, Maximum = 200, Value = (decimal)_robot.Kinematics.MinTurnRadius };

            grpMovement.Controls.Add(lblSpeed);
            grpMovement.Controls.Add(_nudMaxSpeed);
            grpMovement.Controls.Add(lblTurnRate);
            grpMovement.Controls.Add(_nudTurnRate);
            grpMovement.Controls.Add(lblTurnRadius);
            grpMovement.Controls.Add(_nudTurnRadius);
            _propertiesPanel.Controls.Add(grpMovement);
            y += 115;

            // Visual Options
            var grpVisual = new GroupBox { Text = "Visual Options", Location = new Point(10, y), Size = new Size(310, 65) };

            _chkShowWheels = new CheckBox { Text = "Show Wheels", Location = new Point(15, 28), Size = new Size(100, 25), Checked = _robot.Appearance.ShowWheels };
            _chkShowDirectionArrow = new CheckBox { Text = "Show Direction", Location = new Point(130, 28), Size = new Size(120, 25), Checked = _robot.Appearance.ShowDirectionArrow };

            grpVisual.Controls.Add(_chkShowWheels);
            grpVisual.Controls.Add(_chkShowDirectionArrow);
            _propertiesPanel.Controls.Add(grpVisual);
            y += 75;
        }
        private void AddLabelAndControl(string labelText, ref int y, out Label label, out Control control, Control ctrl)
        {
            label = new Label { Text = labelText, Location = new Point(10, y), Size = new Size(100, 25), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            control = ctrl;
            control.Location = new Point(115, y);
            _propertiesPanel.Controls.Add(label);
            _propertiesPanel.Controls.Add(control);
            y += 35;
        }

        private void AddNumericRow(GroupBox parent, string labelText, int x, int y, out Label label, out Control control, decimal min, decimal max, decimal value, decimal increment)
        {
            label = new Label { Text = labelText, Location = new Point(x, y), Size = new Size(90, 25) };
            var nud = new NumericUpDown
            {
                Location = new Point(x + 95, y),
                Size = new Size(80, 23),
                Minimum = min,
                Maximum = max,
                Value = value,
                DecimalPlaces = increment < 1 ? 1 : 0,
                Increment = increment
            };
            control = nud;
            parent.Controls.Add(label);
            parent.Controls.Add(nud);
        }
        private void AddTrackBarRow(GroupBox parent, string labelText, int x, int y, out Label label, out TrackBar trackBar, int min, int max, int value)
        {
            label = new Label { Text = labelText, Location = new Point(x, y), Size = new Size(60, 25) };
            trackBar = new TrackBar
            {
                Location = new Point(x + 65, y - 5),
                Size = new Size(150, 30),
                Minimum = min,
                Maximum = max,
                Value = value
            };
            parent.Controls.Add(label);
            parent.Controls.Add(trackBar);
        }
        private void AddQuickTypeButton(GroupBox parent, string text, int x)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, 16),
                Size = new Size(70, 23),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btn.Click += (s, e) => { _cboRobotType.SelectedItem = text; };
            parent.Controls.Add(btn);
        }

        private Label CreateSeparator(int y)
        {
            return new Label
            {
                Text = "─────────────────────────────────────",
                Location = new Point(10, y),
                Size = new Size(300, 15),
                ForeColor = Color.Gray
            };
        }
        #endregion

        #region Initialization - Sensors Panel
        private void CreateSensorsPanel()
        {
            int y = 10;

            // Sensors List
            var lblSensors = new Label { Text = "Sensors List:", Location = new Point(10, y), Size = new Size(100, 25), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            _lstSensors = new ListBox { Location = new Point(10, y + 30), Size = new Size(280, 120) };
            _sensorsPanel.Controls.Add(lblSensors);
            _sensorsPanel.Controls.Add(_lstSensors);
            y += 160;

            // Selected Sensor Controls
            var grpSelected = new GroupBox { Text = "Selected Sensor", Location = new Point(10, y), Size = new Size(310, 100) };

            var lblAngle = new Label { Text = "Angle:", Location = new Point(10, 28), Size = new Size(45, 25) };
            _trackSensorAngle = new TrackBar { Location = new Point(60, 22), Size = new Size(150, 30), Minimum = -180, Maximum = 180, Value = 0 };
            _lblSensorAngleValue = new Label { Text = "0°", Location = new Point(215, 28), Size = new Size(40, 25) };

            var lblFOV = new Label { Text = "FOV:", Location = new Point(10, 58), Size = new Size(45, 25) };
            _nudSensorFOV = new NumericUpDown { Location = new Point(60, 56), Size = new Size(80, 23), Minimum = 10, Maximum = 180, Value = 30 };

            var lblRange = new Label { Text = "Range:", Location = new Point(160, 58), Size = new Size(50, 25) };
            _nudSensorRange = new NumericUpDown { Location = new Point(210, 56), Size = new Size(80, 23), Minimum = 10, Maximum = 500, Value = 100 };

            _chkSensorEnabled = new CheckBox { Text = "Enabled", Location = new Point(250, 28), Size = new Size(70, 25), Checked = true };

            grpSelected.Controls.Add(lblAngle);
            grpSelected.Controls.Add(_trackSensorAngle);
            grpSelected.Controls.Add(_lblSensorAngleValue);
            grpSelected.Controls.Add(lblFOV);
            grpSelected.Controls.Add(_nudSensorFOV);
            grpSelected.Controls.Add(lblRange);
            grpSelected.Controls.Add(_nudSensorRange);
            grpSelected.Controls.Add(_chkSensorEnabled);
            _sensorsPanel.Controls.Add(grpSelected);
            y += 110;

            // Add Sensor Group
            var grpAddSensor = new GroupBox { Text = "Add New Sensor", Location = new Point(10, y), Size = new Size(310, 130) };

            var lblSensorType = new Label { Text = "Type:", Location = new Point(10, 28), Size = new Size(45, 25) };
            _cboSensorType = new ComboBox { Location = new Point(60, 26), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            _cboSensorType.Items.AddRange(new[] { "Ultrasonic", "Infrared", "Lidar", "Camera", "Proximity", "Temperature", "Pressure", "Humidity", "GPS", "IMU" });
            _cboSensorType.SelectedIndex = 0;

            var lblX = new Label { Text = "X:", Location = new Point(200, 28), Size = new Size(30, 25) };
            _nudSensorX = new NumericUpDown { Location = new Point(230, 26), Size = new Size(60, 23), Minimum = -150, Maximum = 150, Value = 0 };

            var lblY = new Label { Text = "Y:", Location = new Point(10, 60), Size = new Size(30, 25) };
            _nudSensorY = new NumericUpDown { Location = new Point(45, 58), Size = new Size(60, 23), Minimum = -150, Maximum = 150, Value = 0 };

            var lblAngleAdd = new Label { Text = "Angle:", Location = new Point(120, 60), Size = new Size(45, 25) };
            _nudSensorAngle = new NumericUpDown { Location = new Point(170, 58), Size = new Size(60, 23), Minimum = -180, Maximum = 180, Value = 0 };

            _btnAddSensor = new Button { Text = "Add", Location = new Point(10, 95), Size = new Size(80, 28), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            _btnRemoveSensor = new Button { Text = "Remove", Location = new Point(100, 95), Size = new Size(80, 28), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            grpAddSensor.Controls.Add(lblSensorType);
            grpAddSensor.Controls.Add(_cboSensorType);
            grpAddSensor.Controls.Add(lblX);
            grpAddSensor.Controls.Add(_nudSensorX);
            grpAddSensor.Controls.Add(lblY);
            grpAddSensor.Controls.Add(_nudSensorY);
            grpAddSensor.Controls.Add(lblAngleAdd);
            grpAddSensor.Controls.Add(_nudSensorAngle);
            grpAddSensor.Controls.Add(_btnAddSensor);
            grpAddSensor.Controls.Add(_btnRemoveSensor);
            _sensorsPanel.Controls.Add(grpAddSensor);

            RefreshSensorList();
        }
        #endregion

        #region Initialization - Bottom Buttons
        private void CreateBottomButtons()
        {
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                BackColor = Color.FromArgb(240, 242, 245),
                Padding = new Padding(10)
            };

            _btnSave = new Button { Text = "Save Robot", Location = new Point(10, 10), Size = new Size(120, 35), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            _btnCancel = new Button { Text = "Cancel", Location = new Point(140, 10), Size = new Size(100, 35), BackColor = Color.FromArgb(149, 165, 166), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            _btnExport = new Button { Text = "Export JSON", Location = new Point(250, 10), Size = new Size(100, 35), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            _btnImport = new Button { Text = "Import", Location = new Point(360, 10), Size = new Size(90, 35), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            _btnDuplicate = new Button { Text = "Duplicate", Location = new Point(460, 10), Size = new Size(100, 35), BackColor = Color.FromArgb(155, 89, 182), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

            bottomPanel.Controls.Add(_btnSave);
            bottomPanel.Controls.Add(_btnCancel);
            bottomPanel.Controls.Add(_btnExport);
            bottomPanel.Controls.Add(_btnImport);
            bottomPanel.Controls.Add(_btnDuplicate);

            this.Controls.Add(bottomPanel);
        }
        #endregion

        #region Events Wiring
        private void WireEvents()
        {
            // Properties events
            _txtRobotName.TextChanged += (s, e) => { _robot.RobotName = _txtRobotName.Text; UpdateRobotDisplay(); };
            _txtDescription.TextChanged += (s, e) => { _robot.Description = _txtDescription.Text; };
            _cboRobotType.SelectedIndexChanged += (s, e) => { _robot.RobotType = (RobotType)Enum.Parse(typeof(RobotType), _cboRobotType.SelectedItem.ToString()); UpdateRobotDisplay(); UpdateStatsPanel(); };
            _cboBodyShape.SelectedIndexChanged += (s, e) => { _robot.Appearance.ShapeType = (RobotShapeType)_cboBodyShape.SelectedIndex; UpdateRobotDisplay(); };

            _nudWidth.ValueChanged += (s, e) => { _robot.Appearance.Width = (double)_nudWidth.Value; UpdateRobotDisplay(); UpdateStatsPanel(); };
            _nudHeight.ValueChanged += (s, e) => { _robot.Appearance.Height = (double)_nudHeight.Value; UpdateRobotDisplay(); UpdateStatsPanel(); };
            _nudLength.ValueChanged += (s, e) => { _robot.Appearance.Length = (double)_nudLength.Value; UpdateRobotDisplay(); UpdateStatsPanel(); };

            _nudMaxSpeed.ValueChanged += (s, e) => { _robot.Kinematics.MaxForwardSpeed = (double)_nudMaxSpeed.Value; UpdateStatsPanel(); };
            _nudTurnRate.ValueChanged += (s, e) => { _robot.Kinematics.MaxTurnRate = (double)_nudTurnRate.Value; UpdateStatsPanel(); };
            _nudTurnRadius.ValueChanged += (s, e) => { _robot.Kinematics.MinTurnRadius = (double)_nudTurnRadius.Value; UpdateStatsPanel(); };

            _chkShowWheels.CheckedChanged += (s, e) => { _robot.Appearance.ShowWheels = _chkShowWheels.Checked; UpdateRobotDisplay(); };
            _chkShowDirectionArrow.CheckedChanged += (s, e) => { _robot.Appearance.ShowDirectionArrow = _chkShowDirectionArrow.Checked; UpdateRobotDisplay(); };

            _btnBodyColor.Click += (s, e) =>
            {
                if (_colorDialog.ShowDialog() == DialogResult.OK)
                {
                    _pnlColorPreview.BackColor = _colorDialog.Color;
                    _robot.Appearance.Color = $"#{_colorDialog.Color.R:X2}{_colorDialog.Color.G:X2}{_colorDialog.Color.B:X2}";
                    UpdateRobotDisplay();
                }
            };

            _trbOpacity.ValueChanged += (s, e) => { _robot.Appearance.Opacity = _trbOpacity.Value / 100.0; _lblOpacityValue.Text = $"{_trbOpacity.Value}%"; UpdateRobotDisplay(); };

            // Sensor events
            _lstSensors.SelectedIndexChanged += (s, e) => OnSensorSelectedFromList();
            _trackSensorAngle.ValueChanged += (s, e) => { if (_selectedSensor != null) { _selectedSensor.MountAngle = _trackSensorAngle.Value; _lblSensorAngleValue.Text = $"{_trackSensorAngle.Value}°"; UpdateRobotDisplay(); RefreshSensorList(); } };
            _nudSensorFOV.ValueChanged += (s, e) => { if (_selectedSensor != null) { _selectedSensor.FieldOfView = (double)_nudSensorFOV.Value; UpdateRobotDisplay(); } };
            _nudSensorRange.ValueChanged += (s, e) => { if (_selectedSensor != null) { _selectedSensor.MaxRange = (double)_nudSensorRange.Value; UpdateRobotDisplay(); } };
            _chkSensorEnabled.CheckedChanged += (s, e) => { if (_selectedSensor != null) { _selectedSensor.IsEnabled = _chkSensorEnabled.Checked; UpdateRobotDisplay(); RefreshSensorList(); } };

            _btnAddSensor.Click += (s, e) => AddSensor();
            _btnRemoveSensor.Click += (s, e) => RemoveSensor();

            // Bottom buttons
            _btnSave.Click += BtnSave_Click;
            _btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            _btnExport.Click += (s, e) => ExportRobot();
            _btnImport.Click += (s, e) => ImportRobot();
            _btnDuplicate.Click += (s, e) => DuplicateRobot();

            // Canvas events
            _canvas.SensorSelected += OnCanvasSensorSelected;
            _canvas.SensorAdded += (s, e) => RefreshSensorList();
            _canvas.SensorRemoved += (s, e) => RefreshSensorList();
            _canvas.RobotChanged += (s, e) => UpdateStatsPanel();
        }

        private void OnSensorSelectedFromList()
        {
            if (_lstSensors.SelectedItem != null)
            {
                var selectedText = _lstSensors.SelectedItem.ToString();
                var sensor = _robot.Sensors.FirstOrDefault(s => $"{s.SensorName} ({s.PositionX}, {s.PositionY})" == selectedText ||
                    $"{s.SensorName} ({s.PositionX}, {s.PositionY}) @ {s.MountAngle}°" == selectedText);
                if (sensor != null)
                {
                    _selectedSensor = sensor;
                    _trackSensorAngle.Value = (int)sensor.MountAngle;
                    _nudSensorFOV.Value = (decimal)sensor.FieldOfView;
                    _nudSensorRange.Value = (decimal)sensor.MaxRange;
                    _chkSensorEnabled.Checked = sensor.IsEnabled;
                    _lblSensorAngleValue.Text = $"{sensor.MountAngle}°";
                }
            }
        }

        private void OnCanvasSensorSelected(object sender, SimpleSensor sensor)
        {
            _selectedSensor = sensor;
            if (sensor != null)
            {
                _trackSensorAngle.Value = (int)sensor.MountAngle;
                _nudSensorFOV.Value = (decimal)sensor.FieldOfView;
                _nudSensorRange.Value = (decimal)sensor.MaxRange;
                _chkSensorEnabled.Checked = sensor.IsEnabled;
                _lblSensorAngleValue.Text = $"{sensor.MountAngle}°";

                // Select in listbox
                for (int i = 0; i < _lstSensors.Items.Count; i++)
                {
                    if (_lstSensors.Items[i].ToString().Contains($"{sensor.PositionX}, {sensor.PositionY}"))
                    {
                        _lstSensors.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void AddSensor()
        {
            var newSensor = new SimpleSensor
            {
                SensorId = Guid.NewGuid().ToString(),
                SensorName = _cboSensorType.SelectedItem.ToString(),
                SensorType = _cboSensorType.SelectedItem.ToString(),
                PositionX = (int)_nudSensorX.Value,
                PositionY = (int)_nudSensorY.Value,
                MountAngle = (double)_nudSensorAngle.Value,
                FieldOfView = 30,
                MaxRange = 100,
                IsEnabled = true
            };

            _robot.Sensors.Add(newSensor);
            RefreshSensorList();
            UpdateRobotDisplay();
            UpdateStatsPanel();

            _lstSensors.SelectedIndex = _lstSensors.Items.Count - 1;
        }

        private void RemoveSensor()
        {
            if (_lstSensors.SelectedIndex >= 0 && _selectedSensor != null)
            {
                _robot.Sensors.Remove(_selectedSensor);
                _selectedSensor = null;
                RefreshSensorList();
                UpdateRobotDisplay();
                UpdateStatsPanel();
            }
            else
            {
                MessageBox.Show("Please select a sensor to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region Robot Methods
        private void InitializeCanvas()
        {
            if (_canvas != null)
            {
                _canvas.CurrentRobot = _robot;
            }
        }

        private void LoadRobotData()
        {
            _txtRobotName.Text = _robot.RobotName;
            _txtDescription.Text = _robot.Description;
            _cboRobotType.SelectedItem = _robot.RobotType.ToString();
            _cboBodyShape.SelectedIndex = (int)_robot.Appearance.ShapeType;
            _nudWidth.Value = (decimal)_robot.Appearance.Width;
            _nudHeight.Value = (decimal)_robot.Appearance.Height;
            _nudLength.Value = (decimal)_robot.Appearance.Length;
            _nudMaxSpeed.Value = (decimal)_robot.Kinematics.MaxForwardSpeed;
            _nudTurnRate.Value = (decimal)_robot.Kinematics.MaxTurnRate;
            _nudTurnRadius.Value = (decimal)_robot.Kinematics.MinTurnRadius;
            _chkShowWheels.Checked = _robot.Appearance.ShowWheels;
            _chkShowDirectionArrow.Checked = _robot.Appearance.ShowDirectionArrow;
            _trbOpacity.Value = (int)(_robot.Appearance.Opacity * 100);

            if (!string.IsNullOrEmpty(_robot.Appearance.Color))
            {
                try
                {
                    var color = ColorTranslator.FromHtml(_robot.Appearance.Color);
                    _pnlColorPreview.BackColor = color;
                }
                catch { }
            }

            RefreshSensorList();
            UpdateStatsPanel();
        }

        private void RefreshSensorList()
        {
            _lstSensors.Items.Clear();
            foreach (var sensor in _robot.Sensors)
            {
                string status = sensor.IsEnabled ? "✓" : "✗";
                _lstSensors.Items.Add($"{status} {sensor.SensorName} ({sensor.PositionX}, {sensor.PositionY}) @ {sensor.MountAngle}°");
            }
        }

        private void UpdateRobotDisplay()
        {
            _canvas?.RefreshCanvas();
        }

        private void ExportRobot()
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "JSON files (*.json)|*.json";
            sfd.FileName = $"{_robot.RobotName}_export.json";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _robot.SaveToFile(sfd.FileName);
                MessageBox.Show($"Robot exported to:\n{sfd.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ImportRobot()
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "JSON files (*.json)|*.json|Robot files (*.robot)|*.robot";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var imported = RobotDefinition.LoadFromFile(ofd.FileName);
                    if (imported != null)
                    {
                        _robot = imported;
                        LoadRobotData();
                        _canvas.CurrentRobot = _robot;
                        UpdateRobotDisplay();
                        MessageBox.Show($"Robot imported: {_robot.RobotName}", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing robot: {ex.Message}", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DuplicateRobot()
        {
            var clone = _robot.Clone();
            clone.RobotName = $"{_robot.RobotName} (Copy)";
            clone.RobotId = Guid.NewGuid().ToString();

            _robot = clone;
            LoadRobotData();
            _canvas.CurrentRobot = _robot;
            UpdateRobotDisplay();

            MessageBox.Show($"Robot duplicated as: {_robot.RobotName}", "Duplicate Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "Robot Definition (*.robot)|*.robot";
            sfd.FileName = $"{_robot.RobotName}_{DateTime.Now:yyyyMMdd_HHmmss}.robot";

            string robotsDir = Path.Combine(Application.StartupPath, "Robots");
            if (!Directory.Exists(robotsDir)) Directory.CreateDirectory(robotsDir);
            sfd.InitialDirectory = robotsDir;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    _robot.ModifiedAt = DateTime.Now;
                    _robot.SaveToFile(sfd.FileName);
                    MessageBox.Show($"Robot saved to:\n{sfd.FileName}", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving robot: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }
        #endregion
    }
}
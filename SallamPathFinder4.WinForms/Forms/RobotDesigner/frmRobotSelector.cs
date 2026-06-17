#region File Header
/// <summary>
/// File: frmRobotSelector.cs
/// Description: Form for selecting a robot from available robots
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-16
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Robot;

namespace SallamPathFinder4.WinForms.Forms.RobotDesigner
{
    public partial class frmRobotSelector : Form
    {
        #region Private Fields
        private List<RobotDefinition> _availableRobots;
        private RobotDefinition _selectedRobot;
        private bool _isLoading;
        private ListBox _lstRobots;
        private Label _lblLoading;
        private Button _btnSelect;
        private Button _btnCancel;
        private Button _btnCreate;
        private Button _btnEdit;
        private Button _btnDelete;
        private Panel _previewPanel;
        private PictureBox _previewPicture;
        private Label _lblRobotInfo;

        #endregion
        #region Properties
        public RobotDefinition SelectedRobot => _selectedRobot;
        #endregion

        #region Constructor
        public frmRobotSelector()
        {
            InitializeComponent();
            _availableRobots = new List<RobotDefinition>();

            // ✅ تحميل الروبوتات بعد ظهور النافذة (لتجنب تجمد العرض)
            this.Shown += async (s, e) => await LoadRobotsAsync();
        }
        #endregion

        #region Initialization
        private void InitializeComponent()
        {
            this.Text = "Robot Selector";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.MinimumSize = new Size(650, 500);

            // Title
            var lblTitle = new Label
            {
                Text = "Select Robot",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(200, 40),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            // Main Split Container
            var splitContainer = new SplitContainer
            {
                Location = new Point(20, 70),
                Size = new Size(640, 380),
                Orientation = Orientation.Vertical,
                SplitterDistance = 280
            };

            // Left Panel - Robots List
            var leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(5) };

            _lblLoading = new Label
            {
                Text = "Loading robots...",
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray
            };

            _lstRobots = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                Enabled = false
            };
            _lstRobots.SelectedIndexChanged += OnRobotSelected;

            leftPanel.Controls.Add(_lstRobots);
            leftPanel.Controls.Add(_lblLoading);

            // Right Panel - Preview
            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            _previewPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                BackColor = Color.FromArgb(240, 242, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            _previewPicture = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            _previewPanel.Controls.Add(_previewPicture);

            _lblRobotInfo = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 80),
                Text = "Select a robot to see details",
                Padding = new Padding(5)
            };

            rightPanel.Controls.Add(_lblRobotInfo);
            rightPanel.Controls.Add(_previewPanel);

            splitContainer.Panel1.Controls.Add(leftPanel);
            splitContainer.Panel2.Controls.Add(rightPanel);

            // Buttons Panel
            var btnPanel = new Panel
            {
                Location = new Point(20, 460),
                Size = new Size(640, 45)
            };

            _btnSelect = new Button
            {
                Text = "Select",
                Location = new Point(0, 5),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _btnSelect.Click += (s, e) => { if (_selectedRobot != null) { this.DialogResult = DialogResult.OK; this.Close(); } };

            _btnCreate = new Button
            {
                Text = "Create New",
                Location = new Point(110, 5),
                Size = new Size(110, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnCreate.Click += (s, e) => CreateNewRobot();

            _btnEdit = new Button
            {
                Text = "Edit",
                Location = new Point(230, 5),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _btnEdit.Click += (s, e) => EditSelectedRobot();

            _btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(330, 5),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _btnDelete.Click += (s, e) => DeleteSelectedRobot();

            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(540, 5),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            btnPanel.Controls.Add(_btnSelect);
            btnPanel.Controls.Add(_btnCreate);
            btnPanel.Controls.Add(_btnEdit);
            btnPanel.Controls.Add(_btnDelete);
            btnPanel.Controls.Add(_btnCancel);

            this.Controls.Add(lblTitle);
            this.Controls.Add(splitContainer);
            this.Controls.Add(btnPanel);
        }

        private async Task LoadRobotsAsync()
        {
            try
            {
                if (_lstRobots == null || _lblLoading == null) return;

                _lstRobots.Enabled = false;
                if (_btnSelect != null) _btnSelect.Enabled = false;
                if (_btnEdit != null) _btnEdit.Enabled = false;
                if (_btnDelete != null) _btnDelete.Enabled = false;

                _lblLoading.Visible = true;
                _lblLoading.Text = "Loading robots...";
                _lblLoading.ForeColor = Color.Gray;

                _lstRobots.Items.Clear();
                _availableRobots.Clear();

                string robotsDirectory = Path.Combine(Application.StartupPath, "Robots");
                if (!Directory.Exists(robotsDirectory))
                {
                    Directory.CreateDirectory(robotsDirectory);
                }

                var files = Directory.GetFiles(robotsDirectory, "*.robot", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    if (_lblLoading != null)
                    {
                        _lblLoading.Text = $"Loading: {Path.GetFileName(file)}...";
                        Application.DoEvents();
                    }

                    try
                    {
                        var robot = RobotDefinition.LoadFromFile(file);
                        if (robot != null)
                        {
                            _availableRobots.Add(robot);
                            _lstRobots.Items.Add(robot);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading robot {file}: {ex.Message}");
                    }
                }

                if (_lstRobots.Items.Count == 0)
                {
                    var defaultRobot = CreateDefaultRobot();
                    if (defaultRobot != null)
                    {
                        _availableRobots.Add(defaultRobot);
                        _lstRobots.Items.Add(defaultRobot);
                        if (_lblLoading != null) _lblLoading.Text = "No robots found. Default robot created.";
                        await Task.Delay(1500);
                    }
                }
                else
                {
                    if (_lblLoading != null) _lblLoading.Text = $"Ready - {_lstRobots.Items.Count} robot(s) available";
                }

                if (_lstRobots.Items.Count > 0)
                {
                    _lstRobots.SelectedIndex = 0;
                    _lstRobots.Enabled = true;
                }

                if (_lblLoading != null)
                {
                    await Task.Delay(500);
                    _lblLoading.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadRobotsAsync: {ex.Message}");

                if (_lblLoading != null)
                {
                    _lblLoading.Text = "Error loading robots. Please try again.";
                    _lblLoading.ForeColor = Color.Red;
                }

                try
                {
                    var defaultRobot = CreateDefaultRobot();
                    if (defaultRobot != null && _lstRobots != null)
                    {
                        _lstRobots.Items.Clear();
                        _lstRobots.Items.Add(defaultRobot);
                        _lstRobots.SelectedIndex = 0;
                        _lstRobots.Enabled = true;
                    }
                }
                catch { }
            }
        }
        private RobotDefinition CreateDefaultRobot()
        {
            try
            {
                var robot = new RobotDefinition
                {
                    RobotId = "robot_default_001",
                    RobotName = "Default Wheeled Robot",
                    RobotType = RobotType.Wheeled,
                    Description = "Standard wheeled robot for testing",
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now
                };
                robot.Appearance.Width = 50;
                robot.Appearance.Height = 30;
                robot.Appearance.Length = 60;
                robot.Appearance.Color = "#3498db";
                robot.Kinematics.MaxForwardSpeed = 1.5;
                robot.Kinematics.MaxTurnRate = 90;
                robot.Kinematics.MinTurnRadius = 30;
                return robot;
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region Event Handlers
        private void OnRobotSelected(object sender, EventArgs e)
        {
            if (_lstRobots.SelectedItem is RobotDefinition robot)
            {
                _selectedRobot = robot;
                _btnSelect.Enabled = true;
                _btnEdit.Enabled = true;
                _btnDelete.Enabled = true;
                UpdatePreview(robot);
            }
        }

        private void UpdatePreview(RobotDefinition robot)
        {
            if (robot == null) return;

            // Update info label
            if (_lblRobotInfo != null)
            {
                _lblRobotInfo.Text = $"📋 {robot.RobotName}\n" +
                                    $"Type: {robot.RobotType}\n" +
                                    $"Dimensions: {robot.Appearance.Width:F0} x {robot.Appearance.Height:F0} x {robot.Appearance.Length:F0} cm\n" +
                                    $"Speed: {robot.Kinematics.MaxForwardSpeed:F1} m/s\n" +
                                    $"Turn Rate: {robot.Kinematics.MaxTurnRate:F0}°/s\n" +
                                    $"Sensors: {robot.Sensors.Count}\n" +
                                    $"Created: {robot.CreatedAt:yyyy-MM-dd HH:mm}\n" +
                                    $"Modified: {robot.ModifiedAt:yyyy-MM-dd HH:mm}";
            }

            // Draw preview
            DrawRobotPreview(robot);
        }

        private void DrawRobotPreview(RobotDefinition robot)
        {
            if (_previewPicture == null) return;

            if (_previewPicture.Image != null)
            {
                _previewPicture.Image.Dispose();
            }

            int size = 180;
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                 
                float maxDimension = Math.Max((float)robot.Appearance.Width, (float)robot.Appearance.Height);
                float scale = size / maxDimension * 0.7f;

                var center = new Point(size / 2, size / 2);

                robot.Draw(g, center, 0, scale);
            }

            _previewPicture.Image = bmp;
        }

        private void CreateNewRobot()
        {
            var designer = new frmRobotDesigner();
            if (designer.ShowDialog() == DialogResult.OK && designer.Robot != null)
            {
                _ = LoadRobotsAsync();
            }
        }

        private void EditSelectedRobot()
        {
            if (_selectedRobot != null)
            {
                var designer = new frmRobotDesigner(_selectedRobot);
                if (designer.ShowDialog() == DialogResult.OK)
                {
                    _ = LoadRobotsAsync();
                }
            }
        }

        private void DeleteSelectedRobot()
        {
            if (_selectedRobot == null) return;

            var result = MessageBox.Show($"Delete robot '{_selectedRobot.RobotName}'?\n\nThis action cannot be undone.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string robotsDir = Path.Combine(Application.StartupPath, "Robots");

                // Find and delete file
                foreach (var file in Directory.GetFiles(robotsDir, "*.robot"))
                {
                    try
                    {
                        var robot = RobotDefinition.LoadFromFile(file);
                        if (robot != null && robot.RobotId == _selectedRobot.RobotId)
                        {
                            File.Delete(file);
                            break;
                        }
                    }
                    catch { }
                }

                _ = LoadRobotsAsync();
            }
        }
        #endregion
    }
}
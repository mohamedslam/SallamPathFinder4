#region File Header
#endregion

using SallamPathFinder4.Core.Enums;
#region 
/// <summary>
/// File: frmEnvironment.Designer.cs
/// Description: Designer file for main environment form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms
{
    partial class frmEnvironment
    {
        #region Private Fields - Container
        private System.ComponentModel.IContainer components = null;
        #endregion

        #region Private Fields - Layout Components
        private System.Windows.Forms.TableLayoutPanel tlpMapArea;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabAlgorithmRobot;
        private System.Windows.Forms.TabPage tabGoalsParking;
        private System.Windows.Forms.TabPage tabPathResults;
        private System.Windows.Forms.TabPage tabObstacleLog;
        #endregion

        #region Private Fields - Controls
        private SallamPathFinder4.WinForms.Controls.MapControl mapControl;
        private SallamPathFinder4.WinForms.Controls.RulerControl rulerTop;
        private SallamPathFinder4.WinForms.Controls.RulerControl rulerLeft;
        public  SallamPathFinder4.WinForms.Panels.RobotPanel robotPanel;
        private SallamPathFinder4.WinForms.Panels.AlgorithmSettingsPanel algorithmSettingsPanel;
        private SallamPathFinder4.WinForms.Panels.GoalsPanel goalsPanel;
        private SallamPathFinder4.WinForms.Panels.ParkingPanel parkingPanel;
        private SallamPathFinder4.WinForms.Panels.PathDisplayPanel pathDisplayPanel;
        private SallamPathFinder4.WinForms.Panels.ObstacleLogPanel obstacleLogPanel;
        #endregion

        #region Private Fields - Menu and Toolbar
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripDropDownButton experimentsMenu;
        private System.Windows.Forms.ToolStripDropDownButton testMenu;
        private System.Windows.Forms.ToolStripDropDownButton obstacleMenu;
        #endregion

        #region Private Fields - Status Strip Items
        public  System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblMousePos;
        private System.Windows.Forms.ToolStripStatusLabel lblCellPos;
        private System.Windows.Forms.ToolStripStatusLabel lblRealPos;
        private System.Windows.Forms.ToolStripStatusLabel lblRobotPos;
        public  System.Windows.Forms.ToolStripStatusLabel lblBattery;
        private System.Windows.Forms.ToolStripStatusLabel lblAlgoTime;
        private System.Windows.Forms.ToolStripStatusLabel lblTravelTime;
        #endregion

        #region Private Fields - Menu Items
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem robotMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGridItem;
        private System.Windows.Forms.ToolStripMenuItem showCoordsItem;

        #endregion

        #region Private Fields - Toolbar Items
        private System.Windows.Forms.ToolStripButton btnFindPath;
        private System.Windows.Forms.ToolStripDropDownButton mapMenu;
        #endregion
         #region Constants
        private const int RIGHT_PANEL_WIDTH = 340;
        private const int RULER_SIZE = 30;
        private const int DEFAULT_CELL_SIZE = 30;
        #endregion

        #region Protected Methods
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Windows Form Designer generated code

        private void CreateMenuAndToolbar()
        {
            // Create MenuStrip
            mainMenuStrip = new MenuStrip();
            mainMenuStrip.Dock = DockStyle.Top;

            // Create ToolStrip
            toolStrip = new ToolStrip();
            toolStrip.Dock = DockStyle.Top;
            toolStrip.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);

            // ========== FILE MENU ==========
            var fileMenuItem = new ToolStripMenuItem("File");
            fileMenuItem.DropDownItems.Add("New Map", null, (s, e) => NewMap());
            fileMenuItem.DropDownItems.Add("Open Map...", null, (s, e) => OpenMap());
            fileMenuItem.DropDownItems.Add("Save Map", null, (s, e) => SaveMap());
            fileMenuItem.DropDownItems.Add(new ToolStripSeparator());
            fileMenuItem.DropDownItems.Add("Exit", null, (s, e) => Application.Exit());

            // ========== VIEW MENU ==========
            var viewMenuItem = new ToolStripMenuItem("View");
            viewMenuItem.DropDownItems.Add("Zoom In", null, (s, e) => mapControl.ZoomLevel += 0.1f);
            viewMenuItem.DropDownItems.Add("Zoom Out", null, (s, e) => mapControl.ZoomLevel -= 0.1f);
            viewMenuItem.DropDownItems.Add("Reset Zoom", null, (s, e) => ResetView());
            viewMenuItem.DropDownItems.Add(new ToolStripSeparator());

            // Add Map Settings
            viewMenuItem.DropDownItems.Add("Map Settings...", null, (s, e) => ShowMapSettings());

            viewMenuItem.DropDownItems.Add(new ToolStripSeparator());

            var showGridItem = new ToolStripMenuItem("Show Grid");
            showGridItem.Checked = true;
            showGridItem.Click += (s, e) => ToggleGrid();
            viewMenuItem.DropDownItems.Add(showGridItem);

            var showCoordsItem = new ToolStripMenuItem("Show Coordinates");
            showCoordsItem.Checked = false;
            showCoordsItem.Click += (s, e) => ToggleCoordinates();
            viewMenuItem.DropDownItems.Add(showCoordsItem);

            // Store references
            this.showGridItem = showGridItem;
            this.showCoordsItem = showCoordsItem;
            // ========== ROBOT MENU ==========
            var robotMenuItem = new ToolStripMenuItem("Robot");
            robotMenuItem.DropDownItems.Add("Robot Dashboard...", null, (s, e) => ShowDashboard());

            // View menu - add separator and order goals option
            var orderGoalsItem = new ToolStripMenuItem("Order Goals by Distance");
            orderGoalsItem.ShortcutKeyDisplayString = "Ctrl+Shift+G";
            orderGoalsItem.Click += (s, e) => ToggleOrderGoalsByDistance();
            viewMenuItem.DropDownItems.Add(orderGoalsItem);

            // ========== ADD MENUS TO MENUSTRIP ==========
            mainMenuStrip.Items.Add(fileMenuItem);
            mainMenuStrip.Items.Add(viewMenuItem);
            mainMenuStrip.Items.Add(robotMenuItem);

            // ========== TOOLBAR BUTTONS ==========
            var btnFindPath = new ToolStripButton("🔍 Find Path");
            btnFindPath.Click += (s, e) => _viewModel.FindPathAsync();

            var experimentsMenu = new ToolStripDropDownButton("📊 Experiments");
            experimentsMenu.DropDownItems.Add("Experiment Designer...", null, (s, e) => ShowExperimentDesigner());
            experimentsMenu.DropDownItems.Add("Browse Experiments...", null, (s, e) => ShowExperimentBrowser());
            experimentsMenu.DropDownItems.Add("View Results...", null, (s, e) => _viewModel.ShowExperimentViewer());

            var testMenu = new ToolStripDropDownButton("🧪 Test");
            testMenu.DropDownItems.Add("Test All Algorithms", null, async (s, e) => await TestAllAlgorithms());
            testMenu.DropDownItems.Add(new ToolStripSeparator());
            testMenu.DropDownItems.Add("Test A* Only", null, async (s, e) => await TestSingleAlgorithm(AlgorithmType.AStar));
            testMenu.DropDownItems.Add("Test SPPA Only", null, async (s, e) => await TestSingleAlgorithm(AlgorithmType.SPPA));
            testMenu.DropDownItems.Add("Test SPPA-DL Only", null, async (s, e) => await TestSingleAlgorithm(AlgorithmType.SPPA_DL));
            testMenu.DropDownItems.Add(new ToolStripSeparator());
            testMenu.DropDownItems.Add("Clear Test Results", null, (s, e) => ClearTestResults());
           
            

            // Add Dynamic Obstacles submenu
            var obstacleMenu = new ToolStripDropDownButton("🚧 Obstacles");

            // ========== 1. Static Obstacles ==========
            var staticMenu = new ToolStripMenuItem("🧱 Static Obstacles");
            obstacleMenu.DropDownItems.Add("⚙️ Obstacle Settings...", null, (s, e) => ShowObstacleSettings());
            obstacleMenu.DropDownItems.Add(new ToolStripSeparator());
            staticMenu.DropDownItems.Add("🧱 Wall", null, (s, e) => SetStaticElement(MapElementType.Wall));
            staticMenu.DropDownItems.Add("📐 Ramp", null, (s, e) => SetStaticElement(MapElementType.Ramp));
            obstacleMenu.DropDownItems.Add(staticMenu);
            obstacleMenu.DropDownItems.Add(new ToolStripSeparator());

            // ========== 2. Semi-Static Obstacles ==========
            var semiStaticMenu = new ToolStripMenuItem("🪟 Semi-Static Obstacles");
            semiStaticMenu.DropDownItems.Add("🚪 Door", null, (s, e) => SetStaticElement(MapElementType.Door));
            semiStaticMenu.DropDownItems.Add("🪟 Window", null, (s, e) => SetStaticElement(MapElementType.Window));
            obstacleMenu.DropDownItems.Add(semiStaticMenu);
            obstacleMenu.DropDownItems.Add(new ToolStripSeparator());

            // ========== 3. Dynamic Obstacles ==========
            var dynamicMenu = new ToolStripMenuItem("👤 Dynamic Obstacles");
            dynamicMenu.DropDownItems.Add("👤 Adult", null, (s, e) => SetDynamicObstacleType(ObstacleType.Adult));
            dynamicMenu.DropDownItems.Add("🧒 Child", null, (s, e) => SetDynamicObstacleType(ObstacleType.Child));
            dynamicMenu.DropDownItems.Add("🐕 Animal", null, (s, e) => SetDynamicObstacleType(ObstacleType.Animal));
            dynamicMenu.DropDownItems.Add("🤖 Other Robot", null, (s, e) => SetDynamicObstacleType(ObstacleType.OtherRobot));
            dynamicMenu.DropDownItems.Add("🔧 Equipment", null, (s, e) => SetDynamicObstacleType(ObstacleType.Equipment));
            obstacleMenu.DropDownItems.Add(dynamicMenu);

            obstacleMenu.DropDownItems.Add(new ToolStripSeparator());

            // ========== 4. Surface Weights ==========
            var weightMenu = new ToolStripMenuItem("📊 Surface Weights");
            for (int weight = 0; weight <= 100; weight += 5)
            {
                int w = weight;
                var item = new ToolStripMenuItem($"{weight}%");
                int intensity = 255 - (int)((weight / 100.0) * 255);
                item.BackColor = System.Drawing.Color.FromArgb(intensity, intensity, intensity);
                item.Click += (s, e) => SetSurfaceWeight((byte)w);
                weightMenu.DropDownItems.Add(item);
            }
            obstacleMenu.DropDownItems.Add(weightMenu);
            obstacleMenu.DropDownItems.Add(new ToolStripSeparator());

            // ========== 5. Clear All Obstacles ==========
            obstacleMenu.DropDownItems.Add("🗑 Clear All Obstacles", null, (s, e) => ClearAllObstacles());

            toolStrip.Items.Add(obstacleMenu);



            toolStrip.Items.Add(btnFindPath);
            toolStrip.Items.Add(experimentsMenu);
            toolStrip.Items.Add(testMenu);
            toolStrip.Items.Add(obstacleMenu);
            this.Controls.Add(toolStrip);
            this.Controls.Add(mainMenuStrip);

        }
        private void InitializeComponent()
        {
   
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEnvironment));
            tlpMapArea = new TableLayoutPanel();
            mapControl = new Controls.MapControl();
            rightPanel = new Panel();
            tabControl = new TabControl();
            tabAlgorithmRobot = new TabPage();
            tabGoalsParking = new TabPage();
            tabPathResults = new TabPage();
            tabObstacleLog = new TabPage();
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            lblMousePos = new ToolStripStatusLabel();
            lblCellPos = new ToolStripStatusLabel();
            lblRealPos = new ToolStripStatusLabel();
            lblRobotPos = new ToolStripStatusLabel();
            lblBattery = new ToolStripStatusLabel();
            lblAlgoTime = new ToolStripStatusLabel();
            lblTravelTime = new ToolStripStatusLabel();
            tlpMapArea.SuspendLayout();
            rightPanel.SuspendLayout();
            tabControl.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // Table Layout Panel
            // 
            this.tlpMapArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMapArea.ColumnCount = 2;
            this.tlpMapArea.RowCount = 2;
            this.tlpMapArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, RULER_SIZE));
            this.tlpMapArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
            this.tlpMapArea.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, RULER_SIZE));
            this.tlpMapArea.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));

            // Rulers
            this.rulerTop = new SallamPathFinder4.WinForms.Controls.RulerControl(SallamPathFinder4.WinForms.Controls.RulerControl.RulerOrientation.Horizontal);
            this.rulerTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.rulerTop.CellSize = DEFAULT_CELL_SIZE;
            this.rulerTop.Scale = 1.0f;

            this.rulerLeft = new SallamPathFinder4.WinForms.Controls.RulerControl(SallamPathFinder4.WinForms.Controls.RulerControl.RulerOrientation.Vertical);
            this.rulerLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.rulerLeft.CellSize = DEFAULT_CELL_SIZE;
            this.rulerLeft.Scale = 1.0f;

            // Map Control
            this.mapControl = new SallamPathFinder4.WinForms.Controls.MapControl();
            this.mapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapControl.CellSize = DEFAULT_CELL_SIZE;
            this.mapControl.ShowRobot = true;
            this.mapControl.ZoomLevel = 1.0f;

            this.tlpMapArea.Controls.Add(this.rulerLeft, 0, 1);
            this.tlpMapArea.Controls.Add(this.rulerTop, 1, 0);
            this.tlpMapArea.Controls.Add(this.mapControl, 1, 1);
            // 
            // mapControl
            // 
            mapControl.BackColor = Color.White;
            mapControl.CellSize = 30;
            mapControl.CurrentDrawMode = WinForms.Controls.MapControl.DrawMode.None;
            mapControl.CurrentElement = MapElementType.Wall;
            mapControl.CurrentObstacleType = ObstacleType.Adult;
            mapControl.CurrentWeight = 1;
            mapControl.DetectionZoneColor = Color.FromArgb(80, 52, 152, 219);
            mapControl.Dock = DockStyle.Fill;
            mapControl.Location = new Point(899, 761);
            mapControl.MapGrid = null;
            mapControl.Name = "mapControl";
            mapControl.RobotAngle = 0F;
            mapControl.RobotPosition = new Point(10, 10);
            mapControl.ScaleCmPerCell = 0D;
            mapControl.ShowCoordinates = false;
            mapControl.ShowDetectionZone = true;
            mapControl.ShowGrid = true;
            mapControl.ShowRobot = true;
            mapControl.Size = new Size(14, 14);
            mapControl.TabIndex = 2;
            mapControl.ZoomLevel = 1F;
            // 
            // rightPanel
            // 
            rightPanel.BackColor = Color.FromArgb(248, 249, 250);
            rightPanel.Controls.Add(tabControl);
            rightPanel.Dock = DockStyle.Right;
            rightPanel.Location = new Point(916, 0);
            rightPanel.Name = "rightPanel";
            rightPanel.Padding = new Padding(5);
            rightPanel.Size = new Size(284, 778);
            rightPanel.TabIndex = 1;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabAlgorithmRobot);
            tabControl.Controls.Add(tabGoalsParking);
            tabControl.Controls.Add(tabPathResults);
            tabControl.Controls.Add(tabObstacleLog);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(5, 5);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(274, 768);
            tabControl.TabIndex = 0;
            // 
            // tabAlgorithmRobot
            // 
            tabAlgorithmRobot.Location = new Point(4, 24);
            tabAlgorithmRobot.Name = "tabAlgorithmRobot";
            tabAlgorithmRobot.Size = new Size(266, 740);
            tabAlgorithmRobot.TabIndex = 0;
            tabAlgorithmRobot.Text = "⚙️ Algorithm & Robot";
            // 
            // tabGoalsParking
            // 
            tabGoalsParking.Location = new Point(4, 24);
            tabGoalsParking.Name = "tabGoalsParking";
            tabGoalsParking.Size = new Size(182, 62);
            tabGoalsParking.TabIndex = 1;
            tabGoalsParking.Text = "🎯 Goals & Parking";
            // 
            // tabPathResults
            // 
            tabPathResults.Location = new Point(4, 24);
            tabPathResults.Name = "tabPathResults";
            tabPathResults.Size = new Size(182, 62);
            tabPathResults.TabIndex = 2;
            tabPathResults.Text = "📊 Path Results";
            // 
            // tabObstacleLog
            // 
            tabObstacleLog.Location = new Point(4, 24);
            tabObstacleLog.Name = "tabObstacleLog";
            tabObstacleLog.Size = new Size(182, 62);
            tabObstacleLog.TabIndex = 3;
            tabObstacleLog.Text = "⚠️ Collision Log";
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, lblMousePos, lblCellPos, lblRealPos, lblRobotPos, lblBattery, lblAlgoTime, lblTravelTime });
            statusStrip.Location = new Point(0, 778);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1200, 22);
            statusStrip.TabIndex = 2;
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(54, 17);
            lblStatus.Text = "\U0001f7e2 Ready";
            // 
            // lblMousePos
            // 
            lblMousePos.Name = "lblMousePos";
            lblMousePos.Size = new Size(72, 17);
            lblMousePos.Text = "Mouse: (0,0)";
            // 
            // lblCellPos
            // 
            lblCellPos.Name = "lblCellPos";
            lblCellPos.Size = new Size(56, 17);
            lblCellPos.Text = "Cell: (0,0)";
            // 
            // lblRealPos
            // 
            lblRealPos.Name = "lblRealPos";
            lblRealPos.Size = new Size(95, 17);
            lblRealPos.Text = "Real: (0cm, 0cm)";
            // 
            // lblRobotPos
            // 
            lblRobotPos.Name = "lblRobotPos";
            lblRobotPos.Size = new Size(82, 17);
            lblRobotPos.Text = "Robot: (0,0) 0°";
            // 
            // lblBattery
            // 
            lblBattery.Name = "lblBattery";
            lblBattery.Size = new Size(90, 17);
            lblBattery.Text = "🔋 Battery: 100%";
            // 
            // lblAlgoTime
            // 
            lblAlgoTime.Name = "lblAlgoTime";
            lblAlgoTime.Size = new Size(74, 17);
            lblAlgoTime.Text = "⏱️ Algo: 0ms";
            // 
            // lblTravelTime
            // 
            lblTravelTime.Name = "lblTravelTime";
            lblTravelTime.Size = new Size(69, 17);
            lblTravelTime.Text = "🕒 Travel: 0s";
            // 
            // frmEnvironment
            // 
            BackColor = Color.FromArgb(248, 249, 250);
            ClientSize = new Size(1200, 800);
            Controls.Add(tlpMapArea);
            Controls.Add(rightPanel);
            Controls.Add(statusStrip);
            KeyPreview = true;
            Name = "frmEnvironment";
            Text = "SallamPathFinder 4: Cognitive Mobility Robot";
            WindowState = FormWindowState.Maximized;
            tlpMapArea.ResumeLayout(false);
            rightPanel.ResumeLayout(false);
            tabControl.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
            this.Controls.Add(this.mainMenuStrip);
            this.Controls.Add(this.toolStrip);

            // Bring them to front

        }
        #endregion
    }
}
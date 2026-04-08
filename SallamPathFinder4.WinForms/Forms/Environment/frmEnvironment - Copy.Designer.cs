#region File Header
/// <summary>
/// File: frmEnvironment.Designer.cs
/// Description: Designer file for main environment form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-08
/// </summary>
#endregion

using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Goals;
using SallamPathFinder4.WinForms.Controls;
using SallamPathFinder4.WinForms.Panels;

namespace SallamPathFinder4.WinForms.Forms
{
    partial class frmEnvironmentCopy
    {
        private System.ComponentModel.IContainer components = null;

        // Layout Components - Public للوصول من Environment Modules
        internal System.Windows.Forms.TableLayoutPanel tlpMapArea;
        internal System.Windows.Forms.Panel rightPanel;
        internal System.Windows.Forms.TabControl tabControl;
        internal System.Windows.Forms.TabPage tabAlgorithmRobot;
        internal System.Windows.Forms.TabPage tabGoalsParking;
        internal System.Windows.Forms.TabPage tabPathResults;
        internal System.Windows.Forms.TabPage tabObstacleLog;

        // Controls - Public للوصول من Environment Modules
        internal MapControl mapControl;
        internal RulerControl rulerTop;
        internal RulerControl rulerLeft;
        internal RobotPanel robotPanel;
        internal AlgorithmSettingsPanel algorithmSettingsPanel;
        internal GoalsPanel goalsPanel;
        internal ParkingPanel parkingPanel;
        internal PathDisplayPanel pathDisplayPanel;
        internal ObstacleLogPanel obstacleLogPanel;

        // MenuStrip - Public للوصول
        internal System.Windows.Forms.MenuStrip mainMenuStrip;

        // File Menu - Public للوصول
        internal System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem newMapMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem openMapMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem saveMapMenuItem;
        internal System.Windows.Forms.ToolStripSeparator fileSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem exitMenuItem;

        // View Menu - Public للوصول
        internal System.Windows.Forms.ToolStripMenuItem viewMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem zoomInMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem zoomOutMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem zoomResetMenuItem;
        internal System.Windows.Forms.ToolStripSeparator viewSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem mapSettingsMenuItem;
        internal System.Windows.Forms.ToolStripSeparator viewSeparator2;
        internal System.Windows.Forms.ToolStripMenuItem showGridItem;
        internal System.Windows.Forms.ToolStripMenuItem showCoordsItem;

        // Robot Menu - Public للوصول
        internal System.Windows.Forms.ToolStripMenuItem robotMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem robotDashboardMenuItem;
        internal System.Windows.Forms.ToolStripSeparator robotSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem createRobotMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem manageRobotsMenuItem;
        internal System.Windows.Forms.ToolStripSeparator robotSeparator2;
        internal System.Windows.Forms.ToolStripMenuItem robotSettingsMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem exportRobotMenuItem;

        // Experiments Menu - Public للوصول
        internal System.Windows.Forms.ToolStripMenuItem experimentsMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem experimentDesignerMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem experimentResultsMenuItem;

        // Help Menu - Public للوصول
        internal System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem helpContentMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem keyboardShortcutsMenuItem;
        internal System.Windows.Forms.ToolStripSeparator helpSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem documentationMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem checkUpdatesMenuItem;
        internal System.Windows.Forms.ToolStripSeparator helpSeparator2;
        internal System.Windows.Forms.ToolStripMenuItem aboutMenuItem;

        // ToolStrip - Public للوصول
        internal System.Windows.Forms.ToolStrip toolStrip;
        internal System.Windows.Forms.ToolStripButton btnFindPath;
        internal System.Windows.Forms.ToolStripDropDownButton experimentsMenu;
        internal System.Windows.Forms.ToolStripDropDownButton testMenu;
        internal System.Windows.Forms.ToolStripDropDownButton obstacleMenu;
        //internal System.Windows.Forms.ToolStripDropDownButton robotMenuButton;

        // Test Menu Items - Public للوصول
        internal System.Windows.Forms.ToolStripMenuItem testAllMenuItem;
        internal System.Windows.Forms.ToolStripSeparator testSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem testAStarMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem testSPPAMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem testSPPA_DLMenuItem;
        internal System.Windows.Forms.ToolStripSeparator testSeparator2;
        internal System.Windows.Forms.ToolStripMenuItem clearTestResultsMenuItem;

        // Obstacle Menu Items - Public للوصول
        internal System.Windows.Forms.ToolStripMenuItem staticMenu;
        internal System.Windows.Forms.ToolStripMenuItem wallMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem rampMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem semiStaticMenu;
        internal System.Windows.Forms.ToolStripMenuItem doorMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem windowMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem dynamicMenu;
        internal System.Windows.Forms.ToolStripMenuItem adultMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem childMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem animalMenuItem;
        //internal System.Windows.Forms.ToolStripMenuItem otherRobotMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem equipmentMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem weightMenu;
        internal System.Windows.Forms.ToolStripSeparator obstacleSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem clearAllObstaclesMenuItem;
        internal System.Windows.Forms.ToolStripSeparator obstacleSeparator2;
        internal System.Windows.Forms.ToolStripMenuItem obstacleSettingsMenuItem;

        // Robot Toolbar Menu Items - Public للوصول
        
        internal System.Windows.Forms.ToolStripMenuItem robotToolbarDashboardMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem robotToolbarCreateMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem robotToolbarManageMenuItem;

        // StatusStrip - Public للوصول
        internal System.Windows.Forms.StatusStrip statusStrip;
        internal System.Windows.Forms.ToolStripStatusLabel lblStatus;
        internal System.Windows.Forms.ToolStripStatusLabel lblMousePos;
        internal System.Windows.Forms.ToolStripStatusLabel lblCellPos;
        internal System.Windows.Forms.ToolStripStatusLabel lblRealPos;
        internal System.Windows.Forms.ToolStripStatusLabel lblRobotPos;
        internal System.Windows.Forms.ToolStripStatusLabel lblBattery;
        internal System.Windows.Forms.ToolStripStatusLabel lblAlgoTime;
        internal System.Windows.Forms.ToolStripStatusLabel lblTravelTime;

        // State Properties
        public bool IsAddingGoal { get; private set; }
        public bool IsAddingParking { get; private set; }
        public bool IsMovingGoal { get; private set; }
        public bool IsMovingParking { get; private set; }
        public GoalPoint MovingGoal { get; private set; }
        public ParkingPoint MovingParking { get; private set; }

        public void SetAddingGoal(bool value) => IsAddingGoal = value;
        public void SetAddingParking(bool value) => IsAddingParking = value;
        public void SetMovingGoal(GoalPoint goal) { MovingGoal = goal; IsMovingGoal = goal != null; }
        public void SetMovingParking(ParkingPoint parking) { MovingParking = parking; IsMovingParking = parking != null; }
        public void ClearMovingGoal() { MovingGoal = null; IsMovingGoal = false; }
        public void ClearMovingParking() { MovingParking = null; IsMovingParking = false; }

        private const int RIGHT_PANEL_WIDTH = 340;
        private const int RULER_SIZE = 30;
        private const int DEFAULT_CELL_SIZE = 30;


        private void InitializeComponent()
        {
            // ========== CREATE LAYOUT CONTROLS ==========
            this.tlpMapArea = new System.Windows.Forms.TableLayoutPanel();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabAlgorithmRobot = new System.Windows.Forms.TabPage();
            this.tabGoalsParking = new System.Windows.Forms.TabPage();
            this.tabPathResults = new System.Windows.Forms.TabPage();
            this.tabObstacleLog = new System.Windows.Forms.TabPage();
            this.rulerTop = new RulerControl(RulerControl.RulerOrientation.Horizontal);
            this.rulerLeft = new RulerControl(RulerControl.RulerOrientation.Vertical);

            // ========== CREATE PANELS ==========
            this.robotPanel = new RobotPanel();
            this.algorithmSettingsPanel = new AlgorithmSettingsPanel();
            this.goalsPanel = new GoalsPanel(null);
            this.parkingPanel = new ParkingPanel(null);
            this.pathDisplayPanel = new PathDisplayPanel();
            this.obstacleLogPanel = new ObstacleLogPanel(null);

            // ========== CREATE MENU STRIP ==========
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();

            // File Menu
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem("File");
            this.newMapMenuItem = new System.Windows.Forms.ToolStripMenuItem("New Map");
            this.openMapMenuItem = new System.Windows.Forms.ToolStripMenuItem("Open Map...");
            this.saveMapMenuItem = new System.Windows.Forms.ToolStripMenuItem("Save Map");
            this.fileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem("Exit");

            // View Menu
            this.viewMenuItem = new System.Windows.Forms.ToolStripMenuItem("View");
            this.zoomInMenuItem = new System.Windows.Forms.ToolStripMenuItem("Zoom In");
            this.zoomOutMenuItem = new System.Windows.Forms.ToolStripMenuItem("Zoom Out");
            this.zoomResetMenuItem = new System.Windows.Forms.ToolStripMenuItem("Reset Zoom");
            this.viewSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mapSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem("Map Settings...");
            this.viewSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.showGridItem = new System.Windows.Forms.ToolStripMenuItem("Show Grid");
            this.showCoordsItem = new System.Windows.Forms.ToolStripMenuItem("Show Coordinates");

            // Robot Menu
            this.robotMenuItem = new System.Windows.Forms.ToolStripMenuItem("Robot");
            this.robotDashboardMenuItem = new System.Windows.Forms.ToolStripMenuItem("Robot Dashboard...");
            this.robotSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.createRobotMenuItem = new System.Windows.Forms.ToolStripMenuItem("Create New Robot...");
            this.manageRobotsMenuItem = new System.Windows.Forms.ToolStripMenuItem("Manage Robots...");
            this.robotSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.robotSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem("Robot Settings...");
            this.exportRobotMenuItem = new System.Windows.Forms.ToolStripMenuItem("Export Robot Profile...");

            // Experiments Menu
            this.experimentsMenuItem = new System.Windows.Forms.ToolStripMenuItem("Experiments");
            this.experimentDesignerMenuItem = new System.Windows.Forms.ToolStripMenuItem("Experiment Designer...");
            this.experimentResultsMenuItem = new System.Windows.Forms.ToolStripMenuItem("View Results...");

            // Help Menu
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem("Help");
            this.helpContentMenuItem = new System.Windows.Forms.ToolStripMenuItem("Help");
            this.keyboardShortcutsMenuItem = new System.Windows.Forms.ToolStripMenuItem("Keyboard Shortcuts");
            this.helpSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.documentationMenuItem = new System.Windows.Forms.ToolStripMenuItem("Documentation");
            this.checkUpdatesMenuItem = new System.Windows.Forms.ToolStripMenuItem("Check for Updates");
            this.helpSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem("About");

            // ========== BUILD MENU DROP DOWN ITEMS ==========
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.newMapMenuItem, this.openMapMenuItem, this.saveMapMenuItem, this.fileSeparator1, this.exitMenuItem});

            this.viewMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.zoomInMenuItem, this.zoomOutMenuItem, this.zoomResetMenuItem, this.viewSeparator1,
                this.mapSettingsMenuItem, this.viewSeparator2, this.showGridItem, this.showCoordsItem});

            this.robotMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.robotDashboardMenuItem, this.robotSeparator1, this.createRobotMenuItem, this.manageRobotsMenuItem,
                this.robotSeparator2, this.robotSettingsMenuItem, this.exportRobotMenuItem});

            this.experimentsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.experimentDesignerMenuItem, this.experimentResultsMenuItem});

            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.helpContentMenuItem, this.keyboardShortcutsMenuItem, this.helpSeparator1,
                this.documentationMenuItem, this.checkUpdatesMenuItem, this.helpSeparator2, this.aboutMenuItem});

            // ========== ADD MENUS TO MENU STRIP ==========
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.fileMenuItem, this.viewMenuItem, this.robotMenuItem,  this.helpMenuItem});

            // ========== CREATE TOOL STRIP ==========
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStrip.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);

            this.btnFindPath = new System.Windows.Forms.ToolStripButton("🔍 Find Path");
            this.experimentsMenu = new System.Windows.Forms.ToolStripDropDownButton("📊 Experiments");
            this.testMenu = new System.Windows.Forms.ToolStripDropDownButton("🧪 Test");
            this.obstacleMenu = new System.Windows.Forms.ToolStripDropDownButton("🚧 Obstacles");

            // Test Menu Items
            this.testAllMenuItem = new System.Windows.Forms.ToolStripMenuItem("Test All Algorithms");
            this.testSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.testAStarMenuItem = new System.Windows.Forms.ToolStripMenuItem("Test A* Only");
            this.testSPPAMenuItem = new System.Windows.Forms.ToolStripMenuItem("Test SPPA Only");
            this.testSPPA_DLMenuItem = new System.Windows.Forms.ToolStripMenuItem("Test SPPA-DL Only");
            this.testSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.clearTestResultsMenuItem = new System.Windows.Forms.ToolStripMenuItem("Clear Test Results");

            this.testMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.testAllMenuItem, this.testSeparator1, this.testAStarMenuItem, this.testSPPAMenuItem,
                this.testSPPA_DLMenuItem, this.testSeparator2, this.clearTestResultsMenuItem});

            // Obstacle Menu Items
            this.staticMenu = new System.Windows.Forms.ToolStripMenuItem("🧱 Static Obstacles");
            this.wallMenuItem = new System.Windows.Forms.ToolStripMenuItem("🧱 Wall");
            this.rampMenuItem = new System.Windows.Forms.ToolStripMenuItem("📐 Ramp");
            this.staticMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.wallMenuItem, this.rampMenuItem });

            this.semiStaticMenu = new System.Windows.Forms.ToolStripMenuItem("🪟 Semi-Static Obstacles");
            this.doorMenuItem = new System.Windows.Forms.ToolStripMenuItem("🚪 Door");
            this.windowMenuItem = new System.Windows.Forms.ToolStripMenuItem("🪟 Window");
            this.semiStaticMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.doorMenuItem, this.windowMenuItem });

            this.dynamicMenu = new System.Windows.Forms.ToolStripMenuItem("👤 Dynamic Obstacles");
            this.adultMenuItem = new System.Windows.Forms.ToolStripMenuItem("👤 Adult");
            this.childMenuItem = new System.Windows.Forms.ToolStripMenuItem("🧒 Child");
            this.animalMenuItem = new System.Windows.Forms.ToolStripMenuItem("🐕 Animal");
            this.equipmentMenuItem = new System.Windows.Forms.ToolStripMenuItem("🔧 Equipment");
            this.dynamicMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.adultMenuItem, this.childMenuItem, this.animalMenuItem, this.equipmentMenuItem});
            this.weightMenu = new System.Windows.Forms.ToolStripMenuItem("📊 Surface Weights");
            
            this.obstacleSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllObstaclesMenuItem = new System.Windows.Forms.ToolStripMenuItem("🗑 Clear All Obstacles");
            this.obstacleSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.obstacleSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem("⚙️ Obstacle Settings...");

            this.obstacleMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.staticMenu, this.semiStaticMenu, this.dynamicMenu, this.weightMenu,
                this.obstacleSeparator1, this.clearAllObstaclesMenuItem, this.obstacleSeparator2, this.obstacleSettingsMenuItem});

            // Robot Toolbar Menu Items
            
            this.experimentsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.experimentDesignerMenuItem, this.experimentResultsMenuItem});

            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnFindPath, this.experimentsMenu, this.testMenu, this.obstacleMenu});

            // ========== CREATE STATUS STRIP ==========
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel("🟢 Ready");
            this.lblMousePos = new System.Windows.Forms.ToolStripStatusLabel("Mouse: (0,0)");
            this.lblCellPos = new System.Windows.Forms.ToolStripStatusLabel("Cell: (0,0)");
            this.lblRealPos = new System.Windows.Forms.ToolStripStatusLabel("Real: (0cm, 0cm)");
            this.lblRobotPos = new System.Windows.Forms.ToolStripStatusLabel("Robot: (0,0) 0°");
            this.lblBattery = new System.Windows.Forms.ToolStripStatusLabel("🔋 Battery: 100%");
            this.lblAlgoTime = new System.Windows.Forms.ToolStripStatusLabel("⏱️ Algo: 0ms");
            this.lblTravelTime = new System.Windows.Forms.ToolStripStatusLabel("🕒 Travel: 0s");

            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblStatus, this.lblMousePos, this.lblCellPos, this.lblRealPos,
                this.lblRobotPos, this.lblBattery, this.lblAlgoTime, this.lblTravelTime});

            // ========== CONFIGURE LAYOUT ==========
            this.tlpMapArea.ColumnCount = 2;
            this.tlpMapArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, RULER_SIZE));
            this.tlpMapArea.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
            this.tlpMapArea.Controls.Add(this.rulerLeft, 0, 1);
            this.tlpMapArea.Controls.Add(this.rulerTop, 1, 0);
            this.tlpMapArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMapArea.RowCount = 2;
            this.tlpMapArea.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, RULER_SIZE));
            this.tlpMapArea.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));

            this.rulerTop.CellSize = DEFAULT_CELL_SIZE;
            this.rulerTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.rulerTop.Scale = 1F;

            this.rulerLeft.CellSize = DEFAULT_CELL_SIZE;
            this.rulerLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.rulerLeft.Scale = 1F;
 
           
            this.rightPanel.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.rightPanel.Controls.Add(this.tabControl);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPanel.Padding = new System.Windows.Forms.Padding(5);
            this.rightPanel.Size = new System.Drawing.Size(RIGHT_PANEL_WIDTH, 778);

            this.tabControl.Controls.Add(this.tabAlgorithmRobot);
            this.tabControl.Controls.Add(this.tabGoalsParking);
            this.tabControl.Controls.Add(this.tabPathResults);
            this.tabControl.Controls.Add(this.tabObstacleLog);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.SelectedIndex = 0;

            this.tabAlgorithmRobot.Controls.Add(this.robotPanel);
            this.tabAlgorithmRobot.Controls.Add(this.algorithmSettingsPanel);
            this.tabAlgorithmRobot.Text = "⚙️ Algorithm & Robot";

            this.tabGoalsParking.Controls.Add(this.goalsPanel);
            this.tabGoalsParking.Controls.Add(this.parkingPanel);
            this.tabGoalsParking.Text = "🎯 Goals & Parking";

            this.tabPathResults.Controls.Add(this.pathDisplayPanel);
            this.tabPathResults.Text = "📊 Path Results";

            this.tabObstacleLog.Controls.Add(this.obstacleLogPanel);
            this.tabObstacleLog.Text = "⚠️ Collision Log";

            this.robotPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.robotPanel.Height = 220;
            this.algorithmSettingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.goalsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.goalsPanel.Height = 250;
            this.parkingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathDisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.obstacleLogPanel.Dock = System.Windows.Forms.DockStyle.Fill;

            // ========== ADD CONTROLS TO FORM ==========
            this.Controls.Add(this.tlpMapArea);
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.mainMenuStrip);

            // ========== FORM SETTINGS ==========
            this.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.KeyPreview = true;
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "frmEnvironment";
            this.Text = "SallamPathFinder 4: Cognitive Mobility Robot";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }
           
    }
}
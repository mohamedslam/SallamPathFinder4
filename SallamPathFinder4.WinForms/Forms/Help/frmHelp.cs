#region File Header
/// <summary>
/// File: frmHelp.cs
/// Description: Help form showing user manual and keyboard shortcuts
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-05
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Help
{
    public partial class frmHelp : Form
    {
        private TabControl _tabControl;
        private RichTextBox _txtShortcuts;
        private RichTextBox _txtManual;
        private RichTextBox _txtAbout;
        private Button _btnClose;

        public frmHelp()
        {
            InitializeComponent();
            CreateHelpContent();
        }

        private void CreateHelpContent()
        {
            this.Text = "Help - SallamPathFinder 4";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(600, 400);
            this.BackColor = Color.White;

            _tabControl = new TabControl();
            _tabControl.Dock = DockStyle.Fill;

            // Tab 1: Keyboard Shortcuts
            var tabShortcuts = new TabPage("⌨️ Keyboard Shortcuts");
            _txtShortcuts = new RichTextBox();
            _txtShortcuts.Dock = DockStyle.Fill;
            _txtShortcuts.Font = new Font("Consolas", 9);
            _txtShortcuts.ReadOnly = true;
            _txtShortcuts.BackColor = Color.FromArgb(248, 249, 250);
            _txtShortcuts.Text = GetShortcutsText();
            tabShortcuts.Controls.Add(_txtShortcuts);

            // Tab 2: User Manual
            var tabManual = new TabPage("📖 User Manual");
            _txtManual = new RichTextBox();
            _txtManual.Dock = DockStyle.Fill;
            _txtManual.Font = new Font("Segoe UI", 9);
            _txtManual.ReadOnly = true;
            _txtManual.BackColor = Color.FromArgb(248, 249, 250);
            _txtManual.Text = GetManualText();
            tabManual.Controls.Add(_txtManual);

            // Tab 3: About
            var tabAbout = new TabPage("ℹ️ About");
            _txtAbout = new RichTextBox();
            _txtAbout.Dock = DockStyle.Fill;
            _txtAbout.Font = new Font("Segoe UI", 9);
            _txtAbout.ReadOnly = true;
            _txtAbout.BackColor = Color.FromArgb(248, 249, 250);
            _txtAbout.Text = GetAboutText();
            tabAbout.Controls.Add(_txtAbout);

            _tabControl.TabPages.Add(tabShortcuts);
            _tabControl.TabPages.Add(tabManual);
            _tabControl.TabPages.Add(tabAbout);

            // Close button
            _btnClose = new Button();
            _btnClose.Text = "Close";
            _btnClose.Size = new Size(100, 35);
            _btnClose.Location = new Point(this.Width - 120, this.Height - 55);
            _btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _btnClose.BackColor = Color.FromArgb(52, 73, 94);
            _btnClose.ForeColor = Color.White;
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(_tabControl);
            this.Controls.Add(_btnClose);
        }

        private string GetShortcutsText()
        {
            return
"═══════════════════════════════════════════════════════════════════════\n" +
"                          KEYBOARD SHORTCUTS                              \n" +
"═══════════════════════════════════════════════════════════════════════\n\n" +
"FILE\n" +
"  Ctrl+N          New Map\n" +
"  Ctrl+O          Open Map\n" +
"  Ctrl+S          Save Map\n\n" +
"VIEW\n" +
"  Ctrl+Plus (+)   Zoom In\n" +
"  Ctrl+Minus (-)  Zoom Out\n" +
"  Ctrl+0          Reset Zoom\n" +
"  F11             Full Screen\n" +
"  Ctrl+Shift+R    Reset Window Layout\n\n" +
"ROBOT\n" +
"  Ctrl+Shift+S    Set Start Point\n" +
"  Ctrl+Shift+G    Order Goals by Distance\n" +
"  Ctrl+D          Robot Dashboard\n\n" +
"SIMULATION\n" +
"  F5              Start Simulation\n" +
"  F6              Pause Simulation\n" +
"  F7              Stop Simulation\n\n" +
"ALGORITHMS\n" +
"  Ctrl+1          A* Algorithm\n" +
"  Ctrl+2          SPPA Algorithm\n" +
"  Ctrl+3          SPPA-DL Algorithm\n" +
"  Ctrl+4          ACO Algorithm\n" +
"  Ctrl+5          D* Algorithm\n" +
"  Ctrl+6          KNN Algorithm\n" +
"  Ctrl+7          Brute Force\n\n" +
"EXPERIMENTS\n" +
"  Ctrl+Shift+D    Experiment Designer\n" +
"  Ctrl+Shift+B    Experiment Browser\n" +
"  Ctrl+Shift+V    View Results\n\n" +
"MANUAL CONTROL\n" +
"  W               Forward\n" +
"  S               Backward\n" +
"  A               Turn Left (Tank)\n" +
"  D               Turn Right (Tank)\n" +
"  Q               Pivot Left\n" +
"  E               Pivot Right\n" +
"  Z               Strafe Left\n" +
"  C               Strafe Right\n" +
"  Space           Emergency Stop\n\n" +
"VISUALIZATION\n" +
"  (when enabled)  Pause/Resume/Stop buttons control search speed\n\n" +
"HELP\n" +
"  Ctrl+Shift+K    Show this window\n" +
"  F1              User Manual\n\n" +
"═══════════════════════════════════════════════════════════════════════\n" +
"  ESC             Cancel Current Operation\n" +
"  Ctrl+F          Find Path\n" +
"  Ctrl+Shift+N    New Experiment\n" +
"═══════════════════════════════════════════════════════════════════════";
        }

        private string GetManualText()
        {
            return
"═══════════════════════════════════════════════════════════════════════\n" +
"                          USER MANUAL                                   \n" +
"═══════════════════════════════════════════════════════════════════════\n\n" +
"1. GETTING STARTED\n" +
"   ────────────────────────────────────────────────────────────────────\n" +
"   • Use 'File → New Map' or Ctrl+N to create a new map\n" +
"   • Add goals by clicking 'Add Goal' in the Goals panel, then click on map\n" +
"   • Add parking points by clicking 'Add Parking' in the Parking panel\n" +
"   • Select an algorithm from the Algorithm Settings panel\n\n" +
"2. FINDING A PATH\n" +
"   ────────────────────────────────────────────────────────────────────\n" +
"   • Click 'Find Path' or press Ctrl+F\n" +
"   • The algorithm will search for the optimal path visiting all goals\n" +
"   • The path will be displayed in gold color\n" +
"   • You can order goals by distance using Ctrl+Shift+G\n\n" +
"3. SIMULATION\n" +
"   ────────────────────────────────────────────────────────────────────\n" +
"   • Click 'Start' or press F5 to begin simulation\n" +
"   • Watch the robot follow the calculated path\n" +
"   • Robot speed can be adjusted in Robot Settings\n" +
"   • Dynamic charging will send robot to parking when battery is low\n\n" +
"4. VISUALIZATION\n" +
"   ────────────────────────────────────────────────────────────────────\n" +
"   • Enable 'Search Visualization' in Algorithm Settings\n" +
"   • Green cells = Open (to be explored)\n" +
"   • Red cells = Closed (already explored)\n" +
"   • Blue cell = Current (being examined)\n" +
"   • Yellow cells = Final path\n\n" +
"5. EXPERIMENTS\n" +
"   ────────────────────────────────────────────────────────────────────\n" +
"   • Use Experiment Designer to create batch experiments\n" +
"   • Results are saved in CSV format\n" +
"   • Use Experiment Browser to view past experiments\n\n" +
"6. TROUBLESHOOTING\n" +
"   ────────────────────────────────────────────────────────────────────\n" +
"   • If path not found: Increase Search Limit in Algorithm Settings\n" +
"   • If simulation is slow: Reduce visualization speed or disable it\n" +
"   • If robot won't move: Check battery level and start point\n" +
"═══════════════════════════════════════════════════════════════════════";
        }

        private string GetAboutText()
        {
            return
"═══════════════════════════════════════════════════════════════════════\n" +
"                              ABOUT                                      \n" +
"═══════════════════════════════════════════════════════════════════════\n\n" +
"                    SALLAMPATHFINDER 4\n" +
"                    Cognitive Mobility Robot\n\n" +
"═══════════════════════════════════════════════════════════════════════\n\n" +
"DEVELOPER\n" +
"  ────────────────────────────────────────────────────────────────────\n" +
"  Name:     Mohamed ElSayed Sallam\n" +
"  Email:    mohamedslam2000@yahoo.com\n" +
"  Role:     PhD Student at South Ural State University (SUSU)\n\n" +
"SUPERVISOR\n" +
"  ────────────────────────────────────────────────────────────────────\n" +
"  Name:     Prof. Tatiana Anatolyevna Makarovskikh\n" +
"  University: South Ural State University (SUSU)\n\n" +
"ALGORITHMS IMPLEMENTED\n" +
"  ────────────────────────────────────────────────────────────────────\n" +
"  • A* (A-Star)              • SPPA\n" +
"  • SPPA-DL                   • ACO (Ant Colony Optimization)\n" +
"  • D* (Dynamic A*)          • KNN (K-Nearest Neighbors)\n" +
"  • Brute Force              • RRT\n" +
"  • PRM                      • PSO\n" +
"  • GA                       • RRT*\n\n" +
"TECHNOLOGIES USED\n" +
"  ────────────────────────────────────────────────────────────────────\n" +
"  • .NET 8.0\n" +
"  • Windows Forms\n" +
"  • ML.NET for obstacle prediction\n" +
"  • TCP/IP for real robot communication\n" +
"  • ESP32-CAM support\n" +
"  • WeifenLuo.WinFormsUI.Docking\n\n" +
"VERSION\n" +
"  ────────────────────────────────────────────────────────────────────\n" +
"  Version 4.0.0\n" +
"  Build Date: 2026\n\n" +
"═══════════════════════════════════════════════════════════════════════\n" +
"                    © 2026 SallamPathFinder 4\n" +
"═══════════════════════════════════════════════════════════════════════";
        }
    }
}
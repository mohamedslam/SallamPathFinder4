//#region File Header
///// <summary>
///// File: EnvironmentDialogs.cs
///// Description: Dialog management for main environment form
///// Author: Mohamed ElSayed Sallam
///// Date: 2026-04-08
///// </summary>
//#endregion

//#region Namespace Imports
//using System;
//using System.Diagnostics;
//using System.Drawing;
//using System.Windows.Forms;
//using SallamPathFinder4.Core.Interfaces.Services;
//using SallamPathFinder4.WinForms.Container;
//using SallamPathFinder4.WinForms.Controls;
//using SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard;
//using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentDesigner;
//using SallamPathFinder4.WinForms.Forms.Help;
//using SallamPathFinder4.WinForms.Forms.Robot;
//using SallamPathFinder4.WinForms.Forms.RobotManager;
//using SallamPathFinder4.WinForms.Forms.Settings.frmObstacleSettings;
//using SallamPathFinder4.WinForms.ViewModels;
//#endregion

//namespace SallamPathFinder4.WinForms.Forms.Environment
//{
//    public sealed class EnvironmentDialogs
//    {
//        #region Private Fields
//        private readonly frmEnvironment _form; 
//        private readonly MapControl _mapControl;
//        private readonly MainViewModel _viewModel;
//        private readonly EnvironmentLogic _logic;
//        private readonly EnvironmentUI _ui;
//        #endregion

//        #region Constructor
//        public EnvironmentDialogs(
//            frmEnvironment form,
//            MapControl mapControl,
//            MainViewModel viewModel,
//            EnvironmentLogic logic,
//            EnvironmentUI ui)
//        {
//            _form = form ?? throw new ArgumentNullException(nameof(form));
//            _mapControl = mapControl ?? throw new ArgumentNullException(nameof(mapControl));
//            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
//            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
//            _ui = ui ?? throw new ArgumentNullException(nameof(ui));
//        }
   
//        #endregion

//        #region Public Methods - Robot Dialogs
//        public void ShowRobotDashboard()
//        {
//            try
//            {
//                var dashboard = new frmRobotDashboard();
//                dashboard.Show();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error opening Robot Dashboard: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        public void ShowRobotCreator()
//        {
//            try
//            {
//                var creator = new frmRobotCreator();
//                creator.ShowDialog();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error opening Robot Creator: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        public void ShowRobotManager()
//        {
//            try
//            {
//                var manager = new frmRobotManager();
//                manager.ShowDialog();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error opening Robot Manager: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        public void ShowRobotSettings()
//        {
//            // TODO: Implement Robot Settings dialog
//            MessageBox.Show("Robot Settings dialog will be implemented in future version.",
//                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        }

//        public void ExportRobotProfile()
//        {
//            using var sfd = new SaveFileDialog();
//            sfd.Filter = "Robot Profile (*.robot)|*.robot";
//            sfd.DefaultExt = "robot";
//            sfd.Title = "Export Robot Profile";

//            if (sfd.ShowDialog() == DialogResult.OK)
//            {
//                // TODO: Implement export logic
//                MessageBox.Show($"Robot profile exported to:\n{sfd.FileName}",
//                    "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//        }
//        #endregion

//        #region Public Methods - Help Dialogs
//        public void ShowHelp()
//        {
//            try
//            {
//                var helpForm = new frmHelp(true );
//                helpForm.ShowDialog();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error opening Help: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        public void ShowKeyboardShortcuts()
//        {
//            try
//            {
//                var shortcutsForm = new frmHelp(true);
//                shortcutsForm.ShowDialog();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error opening Keyboard Shortcuts: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        public void ShowAboutDialog()
//        {
//            var aboutForm = new Form
//            {
//                Text = _logic.GetAboutTitle(),
//                Size = new Size(550, 500),
//                StartPosition = FormStartPosition.CenterParent,
//                FormBorderStyle = FormBorderStyle.FixedDialog,
//                MaximizeBox = false,
//                MinimizeBox = false,
//                BackColor = EnvironmentLogic.COLOR_DARK,
//                ForeColor = Color.White
//            };

//            // Title Label
//            var lblAppName = new Label
//            {
//                Text = _logic.GetAboutAppName(),
//                Font = new Font("Segoe UI", 20, FontStyle.Bold),
//                ForeColor = EnvironmentLogic.COLOR_INFO,
//                BackColor = Color.Transparent,
//                TextAlign = ContentAlignment.MiddleCenter,
//                Dock = DockStyle.Top,
//                Height = 60,
//                Padding = new Padding(0, 15, 0, 0)
//            };

//            // Version Label
//            var lblVersion = new Label
//            {
//                Text = _logic.GetAboutVersion(),
//                Font = new Font("Segoe UI", 10, FontStyle.Italic),
//                ForeColor = EnvironmentLogic.COLOR_LIGHT,
//                BackColor = Color.Transparent,
//                TextAlign = ContentAlignment.MiddleCenter,
//                Dock = DockStyle.Top,
//                Height = 30
//            };

//            // Description Label
//            var lblDescription = new Label
//            {
//                Text = _logic.GetAboutDescription(),
//                Font = new Font("Segoe UI", 10, FontStyle.Regular),
//                ForeColor = EnvironmentLogic.COLOR_LIGHT,
//                BackColor = Color.Transparent,
//                TextAlign = ContentAlignment.MiddleCenter,
//                Dock = DockStyle.Top,
//                Height = 40,
//                Padding = new Padding(10, 10, 10, 0)
//            };

//            // Info Panel
//            var infoPanel = new Panel
//            {
//                Dock = DockStyle.Top,
//                Height = 150,
//                BackColor = Color.FromArgb(52, 73, 94),
//                Padding = new Padding(10)
//            };

//            var lblDevelopedBy = new Label
//            {
//                Text = _logic.GetAboutDevelopedBy(),
//                Font = new Font("Segoe UI", 9, FontStyle.Regular),
//                ForeColor = Color.White,
//                Location = new Point(10, 10),
//                AutoSize = true
//            };

//            var lblEmail = new Label
//            {
//                Text = _logic.GetAboutEmail(),
//                Font = new Font("Segoe UI", 9, FontStyle.Regular),
//                ForeColor = Color.White,
//                Location = new Point(10, 35),
//                AutoSize = true
//            };

//            var lblUniversity = new Label
//            {
//                Text = _logic.GetAboutUniversity(),
//                Font = new Font("Segoe UI", 9, FontStyle.Regular),
//                ForeColor = Color.White,
//                Location = new Point(10, 60),
//                AutoSize = true
//            };

//            var lblSupervisor = new Label
//            {
//                Text = _logic.GetAboutSupervisor(),
//                Font = new Font("Segoe UI", 9, FontStyle.Bold),
//                ForeColor = EnvironmentLogic.COLOR_INFO,
//                Location = new Point(10, 85),
//                AutoSize = true
//            };

//            var lblCopyright = new Label
//            {
//                Text = _logic.GetAboutCopyright(),
//                Font = new Font("Segoe UI", 8, FontStyle.Italic),
//                ForeColor = EnvironmentLogic.COLOR_LIGHT,
//                Location = new Point(10, 115),
//                AutoSize = true
//            };

//            infoPanel.Controls.Add(lblDevelopedBy);
//            infoPanel.Controls.Add(lblEmail);
//            infoPanel.Controls.Add(lblUniversity);
//            infoPanel.Controls.Add(lblSupervisor);
//            infoPanel.Controls.Add(lblCopyright);

//            // Algorithms Panel
//            var algoPanel = new Panel
//            {
//                Dock = DockStyle.Top,
//                Height = 160,
//                BackColor = EnvironmentLogic.COLOR_PRIMARY,
//                Padding = new Padding(10)
//            };

//            var lblAlgorithms = new Label
//            {
//                Text = "Supported Algorithms",
//                Font = new Font("Segoe UI", 11, FontStyle.Bold),
//                ForeColor = EnvironmentLogic.COLOR_INFO,
//                Location = new Point(10, 10),
//                AutoSize = true
//            };

//            var txtAlgorithms = new TextBox
//            {
//                Text = _logic.GetAboutAlgorithms(),
//                Font = new Font("Consolas", 9, FontStyle.Regular),
//                ForeColor = Color.White,
//                BackColor = EnvironmentLogic.COLOR_PRIMARY,
//                BorderStyle = BorderStyle.None,
//                Location = new Point(10, 35),
//                Size = new Size(500, 110),
//                Multiline = true,
//                ReadOnly = true
//            };

//            algoPanel.Controls.Add(lblAlgorithms);
//            algoPanel.Controls.Add(txtAlgorithms);

//            // OK Button
//            var btnOK = new Button
//            {
//                Text = "OK",
//                Size = new Size(100, 35),
//                BackColor = EnvironmentLogic.COLOR_INFO,
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat,
//                Cursor = Cursors.Hand
//            };
//            btnOK.Click += (s, e) => aboutForm.Close();

//            // Position button at bottom center
//            btnOK.Location = new Point((aboutForm.Width - btnOK.Width) / 2, aboutForm.Height - 55);

//            aboutForm.Controls.Add(lblAppName);
//            aboutForm.Controls.Add(lblVersion);
//            aboutForm.Controls.Add(lblDescription);
//            aboutForm.Controls.Add(infoPanel);
//            aboutForm.Controls.Add(algoPanel);
//            aboutForm.Controls.Add(btnOK);

//            aboutForm.ShowDialog();
//        }
//        #endregion

//        #region Public Methods - Experiment Dialogs
//        public void ShowExperimentDesigner()
//        {
//            try
//            {
//                var designer = new frmExperimentDesigner(_mapControl.MapGrid, _mapControl, _viewModel);
//                designer.ShowDialog();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error opening Experiment Designer: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        public void ShowExperimentViewer()
//        {
//            _viewModel?.ShowExperimentViewer();
//        }
//        #endregion

//        #region Public Methods - Settings Dialogs
//        public void ShowMapSettings()
//        {
//            // Implemented in EnvironmentMapOperations
//        }

//        public void ShowObstacleSettings()
//        {
//            try
//            {
//                var settingsService = ServiceContainer.Resolve<IObstacleSettingsService>();
//                var viewModel = new ObstacleSettingsViewModel(settingsService);
//                var settingsForm = new frmObstacleSettings(viewModel);
//                settingsForm.ShowDialog();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error opening Obstacle Settings: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }
//        #endregion

//        #region Public Methods - Documentation
//        public void ShowDocumentation()
//        {
//            // Open online documentation or local PDF
//            const string docUrl = "https://github.com/yourrepo/SallamPathFinder4/wiki";

//            try
//            {
//                Process.Start(new ProcessStartInfo(docUrl) { UseShellExecute = true });
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Cannot open documentation: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        public void CheckForUpdates()
//        {
//            // TODO: Implement update check
//            MessageBox.Show("Update check will be implemented in future version.\n\nCurrent version: 4.0.0",
//                "Check for Updates", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        }
//        #endregion
//    }
//}
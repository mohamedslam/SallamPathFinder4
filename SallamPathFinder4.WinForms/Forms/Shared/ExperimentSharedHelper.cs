#region File Header
/// <summary>
/// File: ExperimentSharedHelper.cs
/// Description: Shared helper methods for UI operations across experiment forms
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
#endregion

namespace SallamPathFinder4.WinForms.Forms.Shared
{
    /// <summary>
    /// Provides shared helper methods for UI operations
    /// </summary>
    public static class ExperimentSharedHelper
    {
        #region Button Creation
        /// <summary>
        /// Creates a styled button with consistent appearance
        /// </summary>
        public static Button CreateStyledButton(string text, int x, int y, int width, int height, Color backColor)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
        }

        /// <summary>
        /// Creates a styled button with default colors
        /// </summary>
        public static Button CreateDefaultButton(string text, int x, int y)
        {
            return CreateStyledButton(text, x, y, 100, 30, Color.FromArgb(52, 152, 219));
        }

        /// <summary>
        /// Creates a success button (green)
        /// </summary>
        public static Button CreateSuccessButton(string text, int x, int y)
        {
            return CreateStyledButton(text, x, y, 100, 30, Color.FromArgb(46, 204, 113));
        }

        /// <summary>
        /// Creates a danger button (red)
        /// </summary>
        public static Button CreateDangerButton(string text, int x, int y)
        {
            return CreateStyledButton(text, x, y, 100, 30, Color.FromArgb(231, 76, 60));
        }

        /// <summary>
        /// Creates a warning button (yellow/orange)
        /// </summary>
        public static Button CreateWarningButton(string text, int x, int y)
        {
            return CreateStyledButton(text, x, y, 100, 30, Color.FromArgb(241, 196, 15));
        }
        #endregion

        #region Dialog Helpers
        /// <summary>
        /// Shows a confirmation dialog
        /// </summary>
        public static DialogResult ShowConfirmation(string message, string title = "Confirm")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Shows an information dialog
        /// </summary>
        public static void ShowInfo(string message, string title = "Information")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows a warning dialog
        /// </summary>
        public static void ShowWarning(string message, string title = "Warning")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Shows an error dialog
        /// </summary>
        public static void ShowError(string message, string title = "Error")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows a validation warning with option to continue
        /// </summary>
        public static DialogResult ShowValidationWarning(string message)
        {
            return MessageBox.Show(message, "Validation Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }
        #endregion

        #region DataGridView Helpers
        /// <summary>
        /// Creates a configured DataGridView
        /// </summary>
        public static DataGridView CreateDataGridView()
        {
            return new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(248, 249, 250) },
                GridColor = Color.FromArgb(230, 230, 230),
                BorderStyle = BorderStyle.Fixed3D
            };
        }

        /// <summary>
        /// Configures DataGridView headers with consistent style
        /// </summary>
        public static void ConfigureDataGridViewHeaders(DataGridView dgv)
        {
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
        }

        /// <summary>
        /// Colors a success row (green)
        /// </summary>
        public static void ColorSuccessRow(DataGridViewRow row)
        {
            row.DefaultCellStyle.BackColor = Color.FromArgb(230, 255, 230);
        }

        /// <summary>
        /// Colors a failure row (red)
        /// </summary>
        public static void ColorFailureRow(DataGridViewRow row)
        {
            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230);
        }
        #endregion

        #region Control Helpers
        /// <summary>
        /// Safely updates a control's text on the UI thread
        /// </summary>
        public static void SafeSetText(Control control, string text)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() => control.Text = text));
            }
            else
            {
                control.Text = text;
            }
        }

        /// <summary>
        /// Safely updates a control's enabled state on the UI thread
        /// </summary>
        public static void SafeSetEnabled(Control control, bool enabled)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() => control.Enabled = enabled));
            }
            else
            {
                control.Enabled = enabled;
            }
        }

        /// <summary>
        /// Safely updates a ProgressBar value on the UI thread
        /// </summary>
        public static void SafeSetProgressValue(ProgressBar progressBar, int value)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() => progressBar.Value = value));
            }
            else
            {
                progressBar.Value = value;
            }
        }
        #endregion

        #region File Dialog Helpers
        /// <summary>
        /// Shows a save file dialog for CSV files
        /// </summary>
        public static string ShowSaveCsvDialog(string defaultName = null)
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.DefaultExt = "csv";
            if (!string.IsNullOrEmpty(defaultName))
                sfd.FileName = defaultName;

            return sfd.ShowDialog() == DialogResult.OK ? sfd.FileName : null;
        }

        /// <summary>
        /// Shows a save file dialog for JSON files
        /// </summary>
        public static string ShowSaveJsonDialog(string defaultName = null)
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "JSON files (*.json)|*.json";
            sfd.DefaultExt = "json";
            if (!string.IsNullOrEmpty(defaultName))
                sfd.FileName = defaultName;

            return sfd.ShowDialog() == DialogResult.OK ? sfd.FileName : null;
        }

        /// <summary>
        /// Shows a save file dialog for experiment settings
        /// </summary>
        public static string ShowSaveExpSettingsDialog(string defaultName = null)
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "Experiment Settings (*.expcfg)|*.expcfg";
            sfd.DefaultExt = "expcfg";
            if (!string.IsNullOrEmpty(defaultName))
                sfd.FileName = defaultName;

            return sfd.ShowDialog() == DialogResult.OK ? sfd.FileName : null;
        }

        /// <summary>
        /// Shows an open file dialog for experiment settings
        /// </summary>
        public static string ShowOpenExpSettingsDialog()
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Experiment Settings (*.expcfg)|*.expcfg";

            return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : null;
        }

        /// <summary>
        /// Shows a folder browser dialog
        /// </summary>
        public static string ShowFolderBrowserDialog(string description = "Select folder", string selectedPath = null)
        {
            using var fbd = new FolderBrowserDialog();
            fbd.Description = description;
            if (!string.IsNullOrEmpty(selectedPath))
                fbd.SelectedPath = selectedPath;

            return fbd.ShowDialog() == DialogResult.OK ? fbd.SelectedPath : null;
        }
        #endregion

        #region Color Helpers
        /// <summary>
        /// Gets color for algorithm display
        /// </summary>
        public static Color GetAlgorithmColor(string algorithm)
        {
            return algorithm switch
            {
                "AStar" => Color.FromArgb(52, 152, 219),
                "SPPA" => Color.FromArgb(46, 204, 113),
                "SPPA_DL" => Color.FromArgb(155, 89, 182),
                "ACO" => Color.FromArgb(241, 196, 15),
                "DStar" => Color.FromArgb(230, 126, 34),
                "KNN" => Color.FromArgb(26, 188, 156),
                "BruteForce" => Color.FromArgb(231, 76, 60),
                _ => Color.FromArgb(52, 73, 94)
            };
        }

        /// <summary>
        /// Gets color for success/failure status
        /// </summary>
        public static Color GetStatusColor(bool success)
        {
            return success ? Color.FromArgb(46, 204, 113) : Color.FromArgb(231, 76, 60);
        }

        /// <summary>
        /// Gets color for battery level
        /// </summary>
        public static Color GetBatteryColor(double percentage)
        {
            if (percentage <= 10) return Color.FromArgb(231, 76, 60);
            if (percentage <= 20) return Color.FromArgb(241, 196, 15);
            return Color.FromArgb(46, 204, 113);
        }
        #endregion

        #region Formatting Helpers
        /// <summary>
        /// Formats time in milliseconds to readable string
        /// </summary>
        public static string FormatTimeMs(double ms)
        {
            if (ms < 1) return $"{ms * 1000:F2} μs";
            if (ms < 1000) return $"{ms:F2} ms";
            return $"{ms / 1000:F2} s";
        }

        /// <summary>
        /// Formats battery percentage with icon
        /// </summary>
        public static string FormatBattery(double percentage)
        {
            string icon = percentage <= 10 ? "🔴" : percentage <= 20 ? "🟡" : "🟢";
            return $"{icon} {percentage:F1}%";
        }

        /// <summary>
        /// Formats path length with icon
        /// </summary>
        public static string FormatPathLength(int length)
        {
            return $"📏 {length} cells";
        }

        /// <summary>
        /// Formats computation time with icon
        /// </summary>
        public static string FormatComputationTime(double ms)
        {
            return $"⏱️ {FormatTimeMs(ms)}";
        }
        #endregion
    }
}
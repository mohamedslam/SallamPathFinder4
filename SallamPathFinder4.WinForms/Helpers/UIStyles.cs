#region File Header
/// <summary>
/// File: UIStyles.cs
/// Description: Centralized UI styling constants and helper methods
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace SallamPathFinder4.WinForms.Helpers
{
    public static class UIStyles
    {
        #region Color Constants
        public static readonly Color PrimaryColor = Color.FromArgb(52, 73, 94);
        public static readonly Color SecondaryColor = Color.FromArgb(41, 128, 185);
        public static readonly Color AccentColor = Color.FromArgb(46, 204, 113);
        public static readonly Color WarningColor = Color.FromArgb(241, 196, 15);
        public static readonly Color DangerColor = Color.FromArgb(231, 76, 60);
        public static readonly Color DarkColor = Color.FromArgb(44, 62, 80);
        public static readonly Color LightColor = Color.FromArgb(236, 240, 241);
        public static readonly Color PanelBackColor = Color.FromArgb(248, 249, 250);
        public static readonly Color ToolStripBackColor = Color.FromArgb(236, 240, 241);
        #endregion

        #region Font Constants
        public static Font TitleFont => new Font("Segoe UI", 12, FontStyle.Bold);
        public static Font SubtitleFont => new Font("Segoe UI", 10, FontStyle.Regular);
        public static Font SmallFont => new Font("Segoe UI", 8, FontStyle.Regular);
        public static Font StatusFont => new Font("Consolas", 9, FontStyle.Regular);
        public static Font ButtonFont => new Font("Segoe UI", 9, FontStyle.Bold);
        #endregion

        #region Size Constants
        public static readonly Size IconButtonSize = new Size(32, 32);
        public static readonly Size SmallButtonSize = new Size(75, 23);
        public static readonly Size MediumButtonSize = new Size(100, 30);
        public static readonly Size LargeButtonSize = new Size(120, 35);
        #endregion

        #region Padding Constants
        public static readonly Padding DefaultPadding = new Padding(5);
        public static readonly Padding FormPadding = new Padding(10);
        public static readonly Padding PanelPadding = new Padding(5, 5, 5, 5);
        #endregion

        #region Methods
        public static void ApplyPrimaryButton(Button button)
        {
            button.BackColor = PrimaryColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = ButtonFont;
            button.FlatAppearance.BorderSize = 0;
        }

        public static void ApplySuccessButton(Button button)
        {
            button.BackColor = AccentColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = ButtonFont;
            button.FlatAppearance.BorderSize = 0;
        }

        public static void ApplyDangerButton(Button button)
        {
            button.BackColor = DangerColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = ButtonFont;
            button.FlatAppearance.BorderSize = 0;
        }

        public static void ApplyPanel(Panel panel)
        {
            panel.BackColor = PanelBackColor;
            panel.Padding = PanelPadding;
        }

        public static void ApplyForm(Form form)
        {
            form.BackColor = PanelBackColor;
            form.Font = SubtitleFont;
        }
        #endregion
    }
}
#region File Header
/// <summary>
/// File: Program.cs
/// Description: Application entry point with service registration
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Windows.Forms;
using SallamPathFinder4.WinForms.Container;
using SallamPathFinder4.WinForms.Forms;
#endregion

namespace SallamPathFinder4.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ServiceContainer.InitializeServices();

            using (var splash = new frmSplashScreen())
            {
                splash.ShowDialog();
            }

            try
            {
                Application.Run(new frmEnvironment());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fatal Error: {ex.Message}\n\n{ex.StackTrace}",
                    "SallamPathFinder 4", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
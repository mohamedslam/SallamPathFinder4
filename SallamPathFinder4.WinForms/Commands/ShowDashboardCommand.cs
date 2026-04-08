#region File Header
/// <summary>
/// File: ShowDashboardCommand.cs
/// Description: Command to show robot dashboard window
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Threading.Tasks;
using System.Windows.Forms;
using SallamPathFinder4.WinForms.Forms;
using SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard;
using SallamPathFinder4.WinForms.ViewModels;
#endregion

namespace SallamPathFinder4.WinForms.Commands
{
    public sealed class ShowDashboardCommand : CommandBase
    {
        #region Private Fields
        private readonly MainViewModel _viewModel;
        #endregion

        #region Constructor
        public ShowDashboardCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        #endregion

        #region Properties
        public override string Name => "Robot Dashboard";
        public override string Shortcut => "Ctrl+D";
        public override bool CanExecute => true;
        #endregion

        #region Public Methods
        public override void Execute()
        {
            var dashboard = new frmRobotDashboard();
            dashboard.Show();
        }

        public override Task ExecuteAsync()
        {
            Execute();
            return Task.CompletedTask;
        }
        #endregion
    }
}
#region File Header
/// <summary>
/// File: ShowExperimentViewerCommand.cs
/// Description: Command to show experiment viewer window
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Threading.Tasks;
using System.Windows.Forms;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.WinForms.Container;
using SallamPathFinder4.WinForms.Forms;
using SallamPathFinder4.WinForms.ViewModels;
using SallamPathFinder4.WinForms.Forms.Experiments;
using SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentViewer;

#endregion

namespace SallamPathFinder4.WinForms.Commands
{
    public sealed class ShowExperimentViewerCommand : CommandBase
    {
        #region Private Fields
        private readonly MainViewModel _viewModel;
        #endregion

        #region Constructor
        public ShowExperimentViewerCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        #endregion

        #region Properties
        public override string Name => "View Experiments";
        public override string Shortcut => "Ctrl+R";
        public override bool CanExecute => true;
        #endregion

        #region Public Methods
        public override void Execute()
        {
            var viewer = new frmExperimentViewer(ServiceContainer.Resolve<IExperimentService>());
            viewer.ShowDialog();
        }

        public override Task ExecuteAsync()
        {
            Execute();
            return Task.CompletedTask;
        }
        #endregion
    }
}
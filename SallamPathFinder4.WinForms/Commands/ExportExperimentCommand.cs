#region File Header
/// <summary>
/// File: ExportExperimentCommand.cs
/// Description: Command to export experiment results
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Threading.Tasks;
using System.Windows.Forms;
using SallamPathFinder4.WinForms.ViewModels;
#endregion

namespace SallamPathFinder4.WinForms.Commands
{
    public sealed class ExportExperimentCommand : CommandBase
    {
        #region Private Fields
        private readonly MainViewModel _viewModel;
        #endregion

        #region Constructor
        public ExportExperimentCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        #endregion

        #region Properties
        public override string Name => "Export Experiment";
        public override string Shortcut => "Ctrl+E";
        public override bool CanExecute => _viewModel.LastExperimentData != null;
        #endregion

        #region Public Methods
        public override void Execute()
        {
            Task.Run(async () => await ExecuteAsync());
        }

        public override async Task ExecuteAsync()
        {
            if (_viewModel.LastExperimentData == null) return;

            using var sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.DefaultExt = "csv";
            sfd.FileName = $"Experiment_{_viewModel.LastExperimentData.ExperimentId}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Experiment exported successfully.", "Export",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion
    }
}
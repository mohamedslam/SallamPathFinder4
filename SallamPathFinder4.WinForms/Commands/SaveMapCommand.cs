#region File Header
/// <summary>
/// File: SaveMapCommand.cs
/// Description: Command to save current map to file
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
    public sealed class SaveMapCommand : CommandBase
    {
        #region Private Fields
        private readonly MainViewModel _viewModel;
        #endregion

        #region Constructor
        public SaveMapCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        #endregion

        #region Properties
        public override string Name => "Save Map";
        public override string Shortcut => "Ctrl+S";
        public override bool CanExecute => true;
        #endregion

        #region Public Methods
        public override void Execute()
        {
            Task.Run(async () => await ExecuteAsync());
        }

        public override async Task ExecuteAsync()
        {
            using var sfd = new SaveFileDialog();
            sfd.Filter = "Sallam Map (*.smap)|*.smap";
            sfd.DefaultExt = "smap";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                await _viewModel.SaveMapAsync(sfd.FileName);
            }
        }
        #endregion
    }
}
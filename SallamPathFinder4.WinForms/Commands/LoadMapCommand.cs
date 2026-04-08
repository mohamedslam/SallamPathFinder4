#region File Header
/// <summary>
/// File: LoadMapCommand.cs
/// Description: Command to load map from file
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
    public sealed class LoadMapCommand : CommandBase
    {
        #region Private Fields
        private readonly MainViewModel _viewModel;
        #endregion

        #region Constructor
        public LoadMapCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        #endregion

        #region Properties
        public override string Name => "Load Map";
        public override string Shortcut => "Ctrl+O";
        public override bool CanExecute => true;
        #endregion

        #region Public Methods
        public override void Execute()
        {
            Task.Run(async () => await ExecuteAsync());
        }

        public override async Task ExecuteAsync()
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Sallam Map (*.smap)|*.smap";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                await _viewModel.LoadMapAsync(ofd.FileName);
            }
        }
        #endregion
    }
}
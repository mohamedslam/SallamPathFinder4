#region File Header
/// <summary>
/// File: FindPathCommand.cs
/// Description: Command to find path using selected algorithm
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Threading.Tasks;
using SallamPathFinder4.WinForms.ViewModels;
#endregion

namespace SallamPathFinder4.WinForms.Commands
{
    public sealed class FindPathCommand : CommandBase
    {
        #region Private Fields
        private readonly MainViewModel _viewModel;
        #endregion

        #region Constructor
        public FindPathCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        #endregion

        #region Properties
        public override string Name => "Find Path";
        public override string Shortcut => "Ctrl+F";
        public override bool CanExecute => !_viewModel.IsSearching && !_viewModel.IsSimulating && _viewModel.HasGoals;
        #endregion

        #region Public Methods
        public override void Execute()
        {
            Task.Run(async () => await ExecuteAsync());
        }

        public override async Task ExecuteAsync()
        {
            if (!CanExecute) return;
            await _viewModel.FindPathAsync();
        }
        #endregion
    }
}
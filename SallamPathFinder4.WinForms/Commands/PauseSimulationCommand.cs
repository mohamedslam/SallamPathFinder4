#region File Header
/// <summary>
/// File: PauseSimulationCommand.cs
/// Description: Command to pause robot simulation
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.WinForms.ViewModels;
#endregion

namespace SallamPathFinder4.WinForms.Commands
{
    public sealed class PauseSimulationCommand : CommandBase
    {
        #region Private Fields
        private readonly MainViewModel _viewModel;
        #endregion

        #region Constructor
        public PauseSimulationCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        #endregion

        #region Properties
        public override string Name => "Pause Simulation";
        public override string Shortcut => "F6";
        public override bool CanExecute => _viewModel.IsSimulating && !_viewModel.IsPaused;
        #endregion

        #region Public Methods
        public override void Execute()
        {
            _viewModel.PauseSimulation();
        }

        public override Task ExecuteAsync()
        {
            Execute();
            return Task.CompletedTask;
        }
        #endregion
    }
}
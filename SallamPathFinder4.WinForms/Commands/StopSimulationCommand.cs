#region File Header
/// <summary>
/// File: StopSimulationCommand.cs
/// Description: Command to stop robot simulation
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.WinForms.ViewModels;
#endregion

namespace SallamPathFinder4.WinForms.Commands
{
    public sealed class StopSimulationCommand : CommandBase
    {
        #region Private Fields
        private readonly MainViewModel _viewModel;
        #endregion

        #region Constructor
        public StopSimulationCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        #endregion

        #region Properties
        public override string Name => "Stop Simulation";
        public override string Shortcut => "F7";
        public override bool CanExecute => _viewModel.IsSimulating;
        #endregion

        #region Public Methods
        public override void Execute()
        {
            _viewModel.StopSimulation();
        }

        public override Task ExecuteAsync()
        {
            Execute();
            return Task.CompletedTask;
        }
        #endregion
    }
}
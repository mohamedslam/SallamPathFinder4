#region File Header
/// <summary>
/// File: StartSimulationCommand.cs
/// Description: Command to start robot simulation
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.WinForms.ViewModels;
#endregion

namespace SallamPathFinder4.WinForms.Commands
{
    public sealed class StartSimulationCommand : CommandBase
    {
        #region Private Fields
        private readonly MainViewModel _viewModel;
        #endregion

        #region Constructor
        public StartSimulationCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        #endregion

        #region Properties
        public override string Name => "Start Simulation";
        public override string Shortcut => "F5";
        public override bool CanExecute => !_viewModel.IsSimulating && _viewModel.HasPath;
        #endregion

        #region Public Methods
        public override void Execute()
        {
            _viewModel.StartSimulation();
        }

        public override Task ExecuteAsync()
        {
            Execute();
            return Task.CompletedTask;
        }
        #endregion
    }
}
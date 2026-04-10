#region File Header
/// <summary>
/// File: ICommand.cs
/// Description: Base command interface for MVVM pattern
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
#endregion

namespace SallamPathFinder4.WinForms.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Shortcut { get; }
        bool CanExecute { get; }
        void Execute();
        Task ExecuteAsync();
        event EventHandler CanExecuteChanged;
    }
}
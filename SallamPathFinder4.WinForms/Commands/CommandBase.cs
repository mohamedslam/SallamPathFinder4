#region File Header
/// <summary>
/// File: CommandBase.cs
/// Description: Base implementation for commands
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Threading.Tasks;
#endregion

namespace SallamPathFinder4.WinForms.Commands
{
    public abstract class CommandBase : ICommand
    {
        #region Private Fields
        private bool _canExecute = true;
        #endregion

        #region Properties
        public abstract string Name { get; }
        public abstract string Shortcut { get; }

        public virtual bool CanExecute
        {
            get => _canExecute;
            protected set
            {
                if (_canExecute != value)
                {
                    _canExecute = value;
                    OnCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler CanExecuteChanged;
        #endregion

        #region Public Methods
        public abstract void Execute();
        public abstract Task ExecuteAsync();

        public void RefreshCanExecute()
        {
            OnCanExecuteChanged();
        }
        #endregion

        #region Protected Methods
        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
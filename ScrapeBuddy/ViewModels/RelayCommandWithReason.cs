using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Interface for reporting the reason an action cannot be executed.
    /// </summary>
    public interface IReportReasonCantExecute
    {
        /// <summary>
        /// Gets or sets the reason why the action cannot be executed.
        /// </summary>
        string ReasonCantExecute { set; }
    }

    /// <summary>
    /// A relay command that supports reporting a reason why it cannot execute.
    /// </summary>
    public class RelayCommandWithReason : 
        IRelayCommand,  // from CommunityToolkit.MVVM
        IReportReasonCantExecute, 
        INotifyPropertyChanged, 
        INotifyPropertyChanging
    {
        #region INotifyPropertyChange interfaces

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        public event PropertyChangingEventHandler? PropertyChanging;

        #endregion

        #region IRelayCommand interface

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Notifies listeners that the ability to execute the command has changed.
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Executes the command's action.
        /// </summary>
        /// <param name="parameter">The parameter to pass to the action.</param>
        public void Execute(object? parameter)
        {
            _execute?.Invoke(parameter);
        }

        /// <summary>
        /// Determines whether the command can execute, reporting the reason if it cannot.
        /// </summary>
        /// <param name="parameter">The parameter to evaluate for execution.</param>
        /// <returns>
        /// <c>true</c> if the command can execute; otherwise, <c>false</c>.
        /// </returns>
        public bool CanExecute(object? parameter)
        {
            if (_canExecuteWithReason != null)
            {
                bool canExecute = _canExecuteWithReason(parameter, this);

                if (canExecute)
                {
                    ReasonCantExecute = null;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReasonCantExecute)));
                return canExecute;
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Delegate for determining whether the command can execute, with a reason if it cannot.
        /// </summary>
        /// <param name="parameter">The parameter to evaluate.</param>
        /// <param name="reporter">The reporter to set the reason why execution cannot occur.</param>
        /// <returns>
        /// <c>true</c> if the command can execute; otherwise, <c>false</c>.
        /// </returns>
        public delegate bool CanExecuteWithReason(object? parameter, IReportReasonCantExecute reporter);

        private string? _reasonCantExecute;

        /// <summary>
        /// Gets or sets the reason why the command cannot execute.
        /// </summary>
        public string? ReasonCantExecute
        {
            get => _reasonCantExecute;
            set
            {
                if (_reasonCantExecute != value)
                {
                    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(ReasonCantExecute)));
                    _reasonCantExecute = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReasonCantExecute)));
                }
            }
        }

        private readonly Action<object?>? _execute;
        private readonly CanExecuteWithReason? _canExecuteWithReason;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommandWithReason"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecuteWithReason">The delegate to determine if the command can execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="execute"/> is null.</exception>
        public RelayCommandWithReason(Action<object?> execute, CanExecuteWithReason canExecuteWithReason)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecuteWithReason = canExecuteWithReason;
        }
    }
}

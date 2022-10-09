using System;
using System.Windows.Input;

namespace DiskObserver.Utils {
    public class RelayCommand : ICommand, IDisposable {
        private Action<object> execute;

        private Predicate<object> canExecute;
        public bool IsDisposed { get; private set; }

        private event EventHandler CanExecuteChangedInternal;

        public RelayCommand(Action<object> execute)
            : this(execute, DefaultCanExecute) {
        }

        public RelayCommand(Action action)
            : this(_ => action(), DefaultCanExecute) {

        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute) {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public event EventHandler CanExecuteChanged {
            add
            {
                CommandManager.RequerySuggested += value;
                this.CanExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                this.CanExecuteChangedInternal -= value;
            }
        }

        public bool CanExecute(object parameter) {
            return this.canExecute != null && this.canExecute(parameter);
        }

        public void Execute(object parameter) {
            this.execute(parameter);
        }

        public void OnCanExecuteChanged() {
            EventHandler handler = CanExecuteChangedInternal;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private static bool DefaultCanExecute(object parameter) {
            return true;
        }

        protected virtual void Dispose(bool disposing) {
            if (!IsDisposed) {
                if (disposing) {
                    canExecute = _ => false;
                    execute = _ => { };
                }
                IsDisposed = true;
            }
        }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

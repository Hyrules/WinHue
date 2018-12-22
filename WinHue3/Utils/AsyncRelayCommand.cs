using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WinHue3.Utils
{
    [Obsolete("Replaced by AsyncCommand")]
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object, Task> execute;
        private readonly Func<object, bool> canExecute;

        private long isExecuting;

        public AsyncRelayCommand(Func<object, Task> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute ?? (o => true);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;          
        }


        private static void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
            
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (Interlocked.Read(ref isExecuting) != 0)
                return false;

            return canExecute(parameter);
        }

   
        public async void Execute(object parameter)
        {
            Interlocked.Exchange(ref isExecuting, 1);
            RaiseCanExecuteChanged();

            try
            {
                await execute(parameter);
            }
            finally
            {
                Interlocked.Exchange(ref isExecuting, 0);
                RaiseCanExecuteChanged();
            }
        }

    }

    public class CanExecuteEventArgs : EventArgs
    {

    }
 
}

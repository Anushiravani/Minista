using System;
using System.Windows.Input;

namespace Minista
{
    public class BaseCommand : ICommand
    {
#pragma warning disable CS0067 // The event 'BaseCommand.CanExecuteChanged' is never used
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067 // The event 'BaseCommand.CanExecuteChanged' is never used
        public static BaseCommand GetInstance()
        {
            return new BaseCommand() { CanExecuteFunc = obj => true };
        }
        public Predicate<object> CanExecuteFunc
        {
            get;
            set;
        }

        public Action<object> ExecuteFunc
        {
            get;
            set;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteFunc(parameter);
        }
    }
}

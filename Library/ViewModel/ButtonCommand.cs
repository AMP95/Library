using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Library.ViewModel
{
    class ButtonCommand :ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        Action<object?> _action;
        Func<object?,bool>? _func;
        public ButtonCommand(Action<object?> action, Func<object?,bool>? func = null)
        {
            _action = action;
            _func = func;
        }
        public bool CanExecute(object? parameter)
        {
            return _func == null || _func(parameter);
        }

        public void Execute(object? parameter)
        {
            _action(parameter);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Core.ViewModel;

namespace Agent.ViewModel
{
    public class ToastViewModel : VM
    {
        public ToastViewModel(string caption, string text, IReadOnlyCollection<ToastCommand> commands)
        {
            Caption = caption;
            Text = text;
            Commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public ToastViewModel(string caption, string text, params ToastCommand[] commands) :
            this(caption, text, commands.ToList().AsReadOnly())
        {
        }

        public string Caption { get; }
        public string Text { get; }
        // icon?
        public IReadOnlyCollection<ToastCommand> Commands { get; }
    }

    public class ToastCommand : VM
    {
        public ToastCommand(ICommand command, string name)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public ICommand Command { get; }
        public string Name { get; }
    }
}

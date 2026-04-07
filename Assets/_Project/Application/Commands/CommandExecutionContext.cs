using _Project.Application.Interfaces;

namespace _Project.Application.Commands
{
    public sealed class CommandExecutionContext
    {
        public readonly ICommand Command;
        public ValidationResult ValidationResult { get; set; }
        public CommandResult Result { get; set; }

        public CommandExecutionContext(ICommand command)
        {
            Command = command;
        }
    }
}



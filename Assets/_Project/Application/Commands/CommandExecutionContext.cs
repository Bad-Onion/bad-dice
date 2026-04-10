using _Project.Application.Interfaces;

namespace _Project.Application.Commands
{
    /// <summary>
    /// The CommandExecutionContext class encapsulates the context of a command execution, including the command itself,
    /// its validation result, and the final execution result. It is used by the CommandProcessor to manage the state of
    /// command execution and to allow middlewares to access and modify this state as needed.
    /// </summary>
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



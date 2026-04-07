using _Project.Application.Interfaces;

namespace _Project.Application.Commands
{
    /// <summary>
    /// The CommandProcessor is part of the Command design pattern, it is responsible for executing commands that implement
    /// the ICommand interface. It checks if the command is valid before executing it, ensuring that only valid commands are processed.
    /// </summary>
    public class CommandProcessor
    {
        public CommandResult ExecuteCommand(ICommand command)
        {
            if (command == null)
            {
                return CommandResult.Failure("NullCommand", "Cannot execute a null command instance.");
            }

            ValidationResult validationResult = command.Validate();
            if (!validationResult.IsValid)
            {
                return CommandResult.Invalid(validationResult);
            }

            return command.Execute();
        }
    }
}
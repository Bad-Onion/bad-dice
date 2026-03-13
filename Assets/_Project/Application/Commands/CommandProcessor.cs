using _Project.Application.Interfaces;

namespace _Project.Application.Commands
{
    public class CommandProcessor
    {
        public void ExecuteCommand(ICommand command)
        {
            if (command.IsValid())
            {
                command.Execute();
            }
            // Add 'else' in the future for logging failed commands
        }
    }
}
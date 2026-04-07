namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Interface for commands that can be executed. This is part of the Command pattern.
    /// </summary>
    public interface ICommand
    {
        ValidationResult Validate();
        CommandResult Execute();
    }
}
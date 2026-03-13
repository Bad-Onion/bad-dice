namespace _Project.Application.Interfaces
{
    public interface ICommand
    {
        bool IsValid();
        void Execute();
    }
}
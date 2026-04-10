namespace _Project.Application.Interfaces
{
    public interface IDiceSessionPresenterLifecycle
    {
        void Attach(IDiceSessionView view);
        void Detach();
    }
}

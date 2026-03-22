using _Project.Domain.Entities.Session;

namespace _Project.Application.Interfaces
{
    public interface IRunRepository
    {
        PlayerRunState LoadRun();
        void SaveRun(PlayerRunState state);
        bool HasActiveRun();
    }
}
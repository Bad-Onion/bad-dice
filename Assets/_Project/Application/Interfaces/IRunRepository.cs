using _Project.Domain.Entities;

namespace _Project.Application.Interfaces
{
    public interface IRunRepository
    {
        PlayerRunState LoadRun();
        void SaveRun(PlayerRunState state);
        bool HasActiveRun();
    }
}
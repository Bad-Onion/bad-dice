using _Project.Domain.Features.Run.Session;

namespace _Project.Application.Interfaces
{
    public interface IRunRepository
    {
        PlayerRunState LoadRun();
        void SaveRun(PlayerRunState state);
        bool HasActiveRun();
    }
}
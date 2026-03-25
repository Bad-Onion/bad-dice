using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.Run.Session;

namespace _Project.Application.Interfaces
{
    public interface IRunRepository
    {
        PlayerRunState LoadRun(CombatSessionState combatSessionState);
        void SaveRun(PlayerRunState state, CombatSessionState combatSessionState);
        bool HasActiveRun();
    }
}
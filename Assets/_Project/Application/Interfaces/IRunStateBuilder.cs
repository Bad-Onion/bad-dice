using _Project.Domain.Features.Run.ScriptableObjects.Settings;
using _Project.Domain.Features.Run.Session;

namespace _Project.Application.Interfaces
{
    public interface IRunStateBuilder
    {
        void BuildFromExisting(PlayerRunState targetState, PlayerRunState sourceState, RunDefinitions runDefinitions);
        void BuildNew(PlayerRunState targetState, RunDefinitions runDefinitions);
    }
}


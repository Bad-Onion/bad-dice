using _Project.Domain.Entities.Session;
using _Project.Domain.ScriptableObjects.GameSettings;

namespace _Project.Application.Interfaces
{
    public interface IRunStateBuilder
    {
        void BuildFromExisting(PlayerRunState targetState, PlayerRunState sourceState, RunDefinitions runDefinitions);
        void BuildNew(PlayerRunState targetState, RunDefinitions runDefinitions);
    }
}


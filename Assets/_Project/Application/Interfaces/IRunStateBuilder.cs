using _Project.Domain.Features.Run.ScriptableObjects.Settings;
using _Project.Domain.Features.Run.Session;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Builder interface for constructing and initializing player run state.
    /// Supports both creating new runs and building from existing runs with custom configurations.
    /// </summary>
    public interface IRunStateBuilder
    {
        /// <summary>
        /// Builds a player run state from an existing run, copying data to a target state with optional modifications.
        /// </summary>
        /// <param name="targetState">The target player run state to build into.</param>
        /// <param name="sourceState">The source player run state to copy from.</param>
        /// <param name="runDefinitions">The run definitions containing configuration and settings.</param>
        void BuildFromExisting(PlayerRunState targetState, PlayerRunState sourceState, RunDefinitions runDefinitions);

        /// <summary>
        /// Builds a completely new player run state based on the provided run definitions.
        /// </summary>
        /// <param name="targetState">The target player run state to initialize.</param>
        /// <param name="runDefinitions">The run definitions containing configuration and settings.</param>
        void BuildNew(PlayerRunState targetState, RunDefinitions runDefinitions);
    }
}


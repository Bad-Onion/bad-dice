using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.Run.Session;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Repository interface for managing player run persistence and state management.
    /// Handles loading and saving player run data, including combat session information.
    /// </summary>
    public interface IRunRepository
    {
    /// <summary>
    /// Loads an existing player run state from storage.
    /// </summary>
    /// <returns>The loaded player run state, or null if no active run exists.</returns>
    PlayerRunState LoadRun();

    /// <summary>
    /// Restores the combat session state from the last saved run data.
    /// Should be called after loading the run to restore encounter progression.
    /// </summary>
    /// <param name="combatSessionState">The combat session state to restore with saved progression data.</param>
    void RestoreCombatProgression(CombatSessionState combatSessionState);

        /// <summary>
        /// Saves the current player run state and associated combat session state to storage.
        /// </summary>
        /// <param name="state">The player run state to save.</param>
        /// <param name="combatSessionState">The associated combat session state to save.</param>
        void SaveRun(PlayerRunState state, CombatSessionState combatSessionState);

        /// <summary>
        /// Checks whether an active run currently exists in storage.
        /// </summary>
        /// <returns>True if an active run exists; otherwise, false.</returns>
        bool HasActiveRun();
    }
}
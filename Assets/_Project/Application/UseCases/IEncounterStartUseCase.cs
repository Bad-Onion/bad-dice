using _Project.Domain.Features.Combat.Session;

namespace _Project.Application.UseCases
{
    /// <summary>
    /// Starts combat encounter state for a prepared fight.
    /// </summary>
    public interface IEncounterStartUseCase
    {
        /// <summary>
        /// Initializes the combat-turn state for the active encounter.
        /// </summary>
        /// <returns>The updated encounter snapshot when combat can start; otherwise, null.</returns>
        EncounterSnapshot StartEncounter();
    }
}



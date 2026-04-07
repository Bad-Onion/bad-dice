namespace _Project.Application.UseCases
{
    /// <summary>
    /// Use case interface for managing encounter progression throughout a run.
    /// Handles initialization of run progression, encounter preparation, and advancement between encounters.
    /// </summary>
    public interface IEncounterProgressionUseCase
    {
        /// <summary>
        /// Gets a value indicating whether the run progression has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initializes the run progression state, preparing it for encounter management.
        /// </summary>
        void InitializeRunProgression();

        /// <summary>
        /// Starts the prepared encounter flow for combat by initializing dice-session turn state.
        /// </summary>
        void StartEncounter();

        /// <summary>
        /// Prepares the current encounter for combat, setting up enemy and encounter-specific data.
        /// </summary>
        void PrepareCurrentEncounter();

        /// <summary>
        /// Attempts to advance to the next encounter in the run.
        /// </summary>
        /// <returns>True if advancement was successful; otherwise, false.</returns>
        bool TryAdvanceEncounter();
    }
}


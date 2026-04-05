namespace _Project.Application.UseCases
{
    /// <summary>
    /// Use case interface for managing overall game flow and transitions.
    /// Handles game initialization, state transitions, pause management, and application shutdown.
    /// </summary>
    public interface IGameFlowUseCase
    {
        /// <summary>
        /// Starts a new game from the main menu, initializing all necessary systems.
        /// </summary>
        void StartGameFromMenu();

        /// <summary>
        /// Returns to the main menu from the current game state.
        /// </summary>
        void ReturnToMenu();

        /// <summary>
        /// Resumes the game from a paused state.
        /// </summary>
        void ResumeGame();

        /// <summary>
        /// Toggles between paused and playing states.
        /// </summary>
        void TogglePause();

        /// <summary>
        /// Quits the application.
        /// </summary>
        void QuitGame();
    }
}
namespace _Project.Application.UseCases
{
    public interface IGameFlowUseCase
    {
        void StartGameFromMenu();
        void ReturnToMenu();
        void PauseGame();
        void ResumeGame();
        void TogglePause();
        void QuitGame();
    }
}
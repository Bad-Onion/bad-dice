namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Interface for a game state in a game state machine pattern. Each game state should implement this interface to
    /// define its behavior when entering and exiting the state.
    /// </summary>
    public interface IGameState
    {
        void Enter();
        void Exit();
    }
}
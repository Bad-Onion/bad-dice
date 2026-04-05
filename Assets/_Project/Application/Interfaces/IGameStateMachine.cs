using System;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Interface for a game state machine pattern that manages the current state of the game and allows for changing states.
    /// </summary>
    public interface IGameStateMachine
    {
        /// <summary>
        /// Gets the type of the current game state. This allows other parts of the application to query what state the game is currently in.
        /// </summary>
        Type CurrentStateType { get; }

        /// <summary>
        /// Changes the current game state to a new state of type TState. The new state must implement the IGameState interface.
        /// </summary>
        /// <typeparam name="TState">The type of the new game state to transition to, which must be a class that implements IGameState.</typeparam>
        void ChangeState<TState>() where TState : class, IGameState;
    }
}
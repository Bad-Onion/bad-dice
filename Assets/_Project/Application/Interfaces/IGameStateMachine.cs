using System;

namespace _Project.Application.Interfaces
{
    public interface IGameStateMachine
    {
        Type CurrentStateType { get; }

        void ChangeState<TState>() where TState : class, IGameState;
    }
}
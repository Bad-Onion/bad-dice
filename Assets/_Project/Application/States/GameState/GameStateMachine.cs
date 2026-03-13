using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Application.Interfaces;


namespace _Project.Application.States.GameState
{
    public class GameStateMachine : IGameStateMachine
    {
        private readonly Dictionary<Type, IGameState> _states;
        private IGameState _currentState;

        public Type CurrentStateType { get; private set; }

        public GameStateMachine(List<IGameState> states)
        {
            _states = states.ToDictionary(s => s.GetType(), s => s);
        }

        public void ChangeState<TState>() where TState : class, IGameState
        {
            if (!_states.TryGetValue(typeof(TState), out var nextState)) return;

            _currentState?.Exit();
            CurrentStateType = typeof(TState);
            _currentState = nextState;
            _currentState.Enter();
        }
    }
}
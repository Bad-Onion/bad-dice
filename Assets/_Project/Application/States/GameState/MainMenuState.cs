using _Project.Application.Interfaces;
using _Project.Application.Events;

namespace _Project.Application.States.GameState
{
    public class MainMenuState : IGameState
    {
        private readonly GameStateEventChannel _eventChannel;

        public MainMenuState(GameStateEventChannel eventChannel)
        {
            _eventChannel = eventChannel;
        }

        public void Enter() => _eventChannel.RaiseEvent(GetType());

        public void Exit() { }
    }
}
using _Project.Application.Events.EventChannels;
using _Project.Application.Interfaces;

namespace _Project.Application.States.GameState
{
    public class BootstrapState : IGameState
    {
        private readonly GameStateEventChannel _eventChannel;

        public BootstrapState(GameStateEventChannel eventChannel)
        {
            _eventChannel = eventChannel;
        }

        public void Enter() => _eventChannel.RaiseEvent(GetType());

        public void Exit() { }
    }
}
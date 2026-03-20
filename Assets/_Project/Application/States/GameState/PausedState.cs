using _Project.Application.Interfaces;
using _Project.Application.Events.EventChannels;

namespace _Project.Application.States.GameState
{
    public class PausedState : IGameState
    {
        private readonly GameStateEventChannel _eventChannel;
        private readonly ITimeService _timeService;

        public PausedState(GameStateEventChannel eventChannel, ITimeService timeService)
        {
            _eventChannel = eventChannel;
            _timeService = timeService;
        }

        public void Enter()
        {
            _timeService.SetTimeScale(0f);
            _eventChannel.RaiseEvent(GetType());
        }

        public void Exit() { }
    }
}
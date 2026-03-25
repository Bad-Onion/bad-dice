using _Project.Application.Interfaces;

namespace _Project.Application.Events.EncounterState
{
    public struct TurnChangedEvent : IEvent
    {
        public int CurrentTurn;
        public int MaxTurns;
    }
}


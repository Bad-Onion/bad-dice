using _Project.Application.Interfaces;

namespace _Project.Application.Events.DiceInput
{
    public struct DiceRerollSelectionRequestedEvent : IEvent
    {
        public string DiceId;
    }
}


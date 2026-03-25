using _Project.Application.Interfaces;

namespace _Project.Application.Events.DiceInput
{
    public struct DiceHoverChangedEvent : IEvent
    {
        public string DiceId;
        public bool IsHovered;
    }
}


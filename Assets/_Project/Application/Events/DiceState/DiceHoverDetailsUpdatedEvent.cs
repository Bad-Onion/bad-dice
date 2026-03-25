using _Project.Application.Interfaces;

namespace _Project.Application.Events.DiceState
{
    public struct DiceHoverDetailsUpdatedEvent : IEvent
    {
        public bool HasDetails;
        public string DiceId;
        public int CurrentValue;
        public int Level;
        public int Damage;
    }
}


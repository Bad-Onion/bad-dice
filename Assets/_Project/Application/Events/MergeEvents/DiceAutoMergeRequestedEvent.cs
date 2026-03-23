using _Project.Application.Interfaces;

namespace _Project.Application.Events.MergeEvents
{
    public struct DiceAutoMergeRequestedEvent : IEvent
    {
        public string DiceId;
    }
}


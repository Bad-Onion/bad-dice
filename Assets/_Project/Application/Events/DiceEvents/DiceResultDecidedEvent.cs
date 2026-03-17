using _Project.Application.Interfaces;

namespace _Project.Application.Events.DiceEvents
{
    public struct DiceResultDecidedEvent : IEvent
    {
        public int[] TargetFaceIndices { get; set; }
    }
}
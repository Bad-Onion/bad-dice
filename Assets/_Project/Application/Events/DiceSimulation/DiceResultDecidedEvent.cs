using _Project.Application.Interfaces;

namespace _Project.Application.Events.DiceSimulation
{
    public struct DiceResultDecidedEvent : IEvent
    {
        public int[] TargetFaceIndices { get; set; }
    }
}
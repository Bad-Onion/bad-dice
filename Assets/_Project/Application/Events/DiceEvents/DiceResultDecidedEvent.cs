using _Project.Application.Interfaces;

namespace _Project.Application.Events.DiceEvents
{
    // TODO: Move event to a more appropriate location like inside a "Events/DiceSimulation" folder
    public struct DiceResultDecidedEvent : IEvent
    {
        public int[] TargetFaceIndices { get; set; }
    }
}
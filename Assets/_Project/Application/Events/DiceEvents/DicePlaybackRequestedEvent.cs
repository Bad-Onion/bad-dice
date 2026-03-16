using _Project.Application.Interfaces;
using _Project.Domain.Entities;

namespace _Project.Application.Events.DiceEvents
{
    public struct DicePlaybackRequestedEvent : IEvent
    {
        public DiceSimulationResult SimulationResult;
    }
}
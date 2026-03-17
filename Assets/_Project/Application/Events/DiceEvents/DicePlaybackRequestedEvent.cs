using System.Collections.Generic;
using _Project.Application.Interfaces;
using _Project.Domain.Entities;

namespace _Project.Application.Events.DiceEvents
{
    public struct DicePlaybackRequestedEvent : IEvent
    {
        public DiceSimulationResult SimulationResult { get; set; }
        public List<string> RolledDiceIds { get; set; }
    }
}
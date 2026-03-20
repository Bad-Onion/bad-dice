using System.Collections.Generic;
using _Project.Application.Interfaces;
using _Project.Domain.Entities;

namespace _Project.Application.Events.DiceEvents
{
    // TODO: Move event to a more appropriate location like inside a "Events/DiceSimulation" folder
    public struct DicePlaybackRequestedEvent : IEvent
    {
        public DiceSimulationResult SimulationResult { get; set; }
        public List<string> RolledDiceIds { get; set; }
    }
}
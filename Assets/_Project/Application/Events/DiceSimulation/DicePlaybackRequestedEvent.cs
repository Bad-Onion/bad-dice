using System.Collections.Generic;
using _Project.Application.Interfaces;
using _Project.Domain.Entities;
using _Project.Domain.Entities.DiceSimulation;

namespace _Project.Application.Events.DiceSimulation
{
    public struct DicePlaybackRequestedEvent : IEvent
    {
        public DiceSimulationResult SimulationResult { get; set; }
        public List<string> RolledDiceIds { get; set; }
    }
}
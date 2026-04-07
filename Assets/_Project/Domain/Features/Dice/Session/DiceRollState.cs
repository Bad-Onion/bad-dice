using _Project.Application.States.DiceSession;
using _Project.Domain.Features.Dice.Simulation;
using System;
using System.Collections.Generic;

namespace _Project.Domain.Features.Dice.Session
{
    public class DiceRollState
    {
        public bool IsRolling { get; set; }
        public DiceRollPhase RollPhase { get; set; } = DiceRollPhase.Idle;
        public int[] CurrentTargetFaceIndices { get; set; } = Array.Empty<int>();
        public DiceSimulationResult CurrentSimulationResult { get; set; }
        public List<string> CurrentRolledDiceIds { get; set; } = new();
    }
}


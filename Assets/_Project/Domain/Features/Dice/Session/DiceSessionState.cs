using System.Collections.Generic;
using _Project.Application.States.DiceSession;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Dice.Simulation;

namespace _Project.Domain.Features.Dice.Session
{
    public class DiceSessionState
    {
        public List<DiceState> ActiveDice { get; set; } = new();
        public bool IsRolling { get; set; }
        public DiceRollPhase RollPhase { get; set; } = DiceRollPhase.Idle;
        public int RerollsLeft { get; set; }
        public int CurrentTurn { get; set; } = 1;
        public int MaxTurns { get; set; } = 1;
        public bool HasDealtThisTurn { get; set; }
        public int[] CurrentTargetFaceIndices { get; set; } = System.Array.Empty<int>();
        public DiceSimulationResult CurrentSimulationResult { get; set; }
        public List<string> CurrentRolledDiceIds { get; set; } = new();
        public MergeState MergeState { get; set; } = MergeState.None;
        public List<string> MergeableDiceIds { get; set; } = new();
    }
}
using System.Collections.Generic;
using _Project.Domain.Features.Dice.Entities;

namespace _Project.Domain.Features.Dice.Session
{
    public class DiceSessionState
    {
        public List<DiceState> ActiveDice { get; set; } = new();
        public bool IsRolling { get; set; }
        public int RerollsLeft { get; set; }
        public int CurrentTurn { get; set; } = 1;
        public int MaxTurns { get; set; } = 1;
        public bool HasDealtThisTurn { get; set; }
        public List<string> MergeableDiceIds { get; set; } = new();
    }
}
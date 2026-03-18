using System.Collections.Generic;

namespace _Project.Domain.Entities
{
    public class DiceSession
    {
        public List<DiceState> ActiveDice { get; set; } = new List<DiceState>();
        public bool IsRolling { get; set; }
        public int RerollsLeft { get; set; } = 3;
        public bool IsMergeModeActive { get; set; }
        public string MergeTargetDiceId { get; set; }
        public List<string> MergeableDiceIds { get; set; } = new List<string>();
    }
}
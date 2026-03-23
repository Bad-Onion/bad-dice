using System.Collections.Generic;
using _Project.Domain.Entities.DiceData;

namespace _Project.Domain.Entities.Session
{
    public class DiceSessionState
    {
        public List<DiceState> ActiveDice { get; set; } = new();
        public bool IsRolling { get; set; }
        public int RerollsLeft { get; set; }
        public List<string> MergeableDiceIds { get; set; } = new();
    }
}
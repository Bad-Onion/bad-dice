using System.Collections.Generic;
using _Project.Domain.Entities.DiceData;

namespace _Project.Domain.Entities.Session
{
    public class DiceSessionState
    {
        public List<DiceState> ActiveDice { get; set; } = new List<DiceState>();
        public bool IsRolling { get; set; }
        public int RerollsLeft { get; set; } = 3;
        public List<string> MergeableDiceIds { get; set; } = new List<string>();
    }
}
using System.Collections.Generic;

namespace _Project.Domain.Entities
{
    // TODO: Move to "Entities/Session"
    // TODO: Rename to something more descriptive like "DiceSessionState"
    public class DiceSession
    {
        public List<DiceState> ActiveDice { get; set; } = new List<DiceState>();
        public bool IsRolling { get; set; }
        public int RerollsLeft { get; set; } = 3;
        public List<string> MergeableDiceIds { get; set; } = new List<string>();
    }
}
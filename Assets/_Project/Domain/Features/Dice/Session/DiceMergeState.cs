using _Project.Application.States.DiceSession;
using System.Collections.Generic;

namespace _Project.Domain.Features.Dice.Session
{
    public class DiceMergeState
    {
        public MergeState MergeState { get; set; } = MergeState.None;
        public List<string> MergeableDiceIds { get; set; } = new();
    }
}


using System.Collections.Generic;
using _Project.Domain.Entities.DiceData;

namespace _Project.Domain.Entities.Session
{
    public class PlayerRunState
    {
        public List<OwnedDiceData> Inventory { get; set; } = new();
        public int MaxEquippedDice { get; set; } = 5;
    }
}
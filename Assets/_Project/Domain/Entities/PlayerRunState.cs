using System.Collections.Generic;

namespace _Project.Domain.Entities
{
    public class PlayerRunState
    {
        public List<OwnedDiceData> Inventory { get; set; } = new();
        public int MaxEquippedDice { get; set; } = 5;
    }
}
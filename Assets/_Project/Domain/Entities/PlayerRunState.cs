using System.Collections.Generic;

namespace _Project.Domain.Entities
{
    // TODO: Move to "Entities/Session"
    public class PlayerRunState
    {
        public List<OwnedDiceData> Inventory { get; set; } = new();
        public int MaxEquippedDice { get; set; } = 5;
    }
}
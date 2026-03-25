using System.Collections.Generic;
using _Project.Domain.Features.Dice.Entities;

namespace _Project.Domain.Features.Run.Session
{
    public class PlayerRunState
    {
        public List<OwnedDiceData> DiceInventory { get; set; } = new();
        public int MaxEquippedDice { get; set; }
        public int RerollsPerTurn { get; set; }
        public int TurnsPerFight { get; set; }
    }
}
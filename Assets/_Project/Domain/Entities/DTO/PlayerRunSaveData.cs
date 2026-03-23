using System;
using System.Collections.Generic;

namespace _Project.Domain.Entities.DTO
{
    [Serializable]
    public class PlayerRunSaveData
    {
        public List<OwnedDiceSaveData> diceInventory = new();
        public int maxEquippedDice;
        public int rerollsPerTurn;
        public int turnsPerFight;
    }
}
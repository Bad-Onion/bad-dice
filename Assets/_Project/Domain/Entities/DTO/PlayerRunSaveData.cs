using System;
using System.Collections.Generic;

namespace _Project.Domain.Entities.DTO
{
    [Serializable]
    public class PlayerRunSaveData
    {
        public List<OwnedDiceSaveData> diceInventory = new();
        // TODO: This should be set by an scriptable object
        public int maxEquippedDice = 5;
    }
}
using System;
using System.Collections.Generic;

namespace _Project.Domain.Entities
{
    [Serializable]
    public class PlayerRunSaveData
    {
        public List<OwnedDiceSaveData> Inventory = new();
        public int MaxEquippedDice = 5;
    }
}
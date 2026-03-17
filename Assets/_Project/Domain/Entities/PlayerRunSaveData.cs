using System;
using System.Collections.Generic;

namespace _Project.Domain.Entities
{
    [Serializable]
    public class PlayerRunSaveData
    {
        public List<OwnedDiceSaveData> inventory = new();
        public int maxEquippedDice = 5;
    }
}
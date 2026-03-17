using System;

namespace _Project.Domain.Entities
{
    [Serializable]
    public class OwnedDiceSaveData
    {
        public string id;
        public string definitionName;
        public int level;
        public bool isEquipped;
    }
}
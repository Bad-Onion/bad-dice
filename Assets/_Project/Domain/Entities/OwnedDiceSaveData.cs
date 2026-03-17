using System;

namespace _Project.Domain.Entities
{
    [Serializable]
    public class OwnedDiceSaveData
    {
        public string Id;
        public string DefinitionName;
        public int Level;
        public bool IsEquipped;
    }
}
using System;

namespace _Project.Domain.Entities.DTO
{
    [Serializable]
    public class OwnedDiceSaveData
    {
        public string id;
        public string definitionName;
        public bool isEquipped;
    }
}
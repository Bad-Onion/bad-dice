using System;

namespace _Project.Domain.Features.Run.DTO
{
    [Serializable]
    public class OwnedDiceSaveData
    {
        public string id;
        public string definitionName;
        public bool isEquipped;
    }
}
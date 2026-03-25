using System;

namespace _Project.Domain.Features.Run.DTO
{
    [Serializable]
    public class EncounterPlanSaveData
    {
        public string enemyId;
        public string enemyName;
        public int maxHealth;
        public int encounterType;
        public int cycleNumber;
        public int encounterIndexInCycle;
    }
}



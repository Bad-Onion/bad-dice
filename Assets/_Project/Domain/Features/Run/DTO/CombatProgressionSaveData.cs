using System;
using System.Collections.Generic;

namespace _Project.Domain.Features.Run.DTO
{
    [Serializable]
    public class CombatProgressionSaveData
    {
        public List<EncounterPlanSaveData> plannedEncounters = new();
        public int currentEncounterIndex;
        public int currentCycleNumber;
        public string currentEnemyId;
    }
}



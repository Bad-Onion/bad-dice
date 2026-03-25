using _Project.Domain.Features.Combat.Enums;

namespace _Project.Domain.Features.Combat.Entities
{
    public class EncounterPlanEntry
    {
        public string EnemyId { get; set; }
        public string EnemyName { get; set; }
        public int MaxHealth { get; set; }
        public EnemyEncounterType EncounterType { get; set; }
        public int CycleNumber { get; set; }
        public int EncounterIndexInCycle { get; set; }
    }
}


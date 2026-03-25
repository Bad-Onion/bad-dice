using _Project.Domain.Features.Combat.Entities;

namespace _Project.Domain.Features.Combat.Session
{
    public class EnemyEncounterState
    {
        public EncounterPlanEntry CurrentEncounter { get; set; }
        public int CurrentHealth { get; set; }
        public bool IsPrepared { get; set; }
        public bool IsDefeated { get; set; }
    }
}


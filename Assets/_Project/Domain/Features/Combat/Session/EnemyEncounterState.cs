using _Project.Application.States.Encounter;
using _Project.Domain.Features.Combat.Entities;

namespace _Project.Domain.Features.Combat.Session
{
    public class EnemyEncounterState
    {
        public EncounterPlanEntry CurrentEncounter { get; set; }
        public int CurrentHealth { get; set; }
        public bool IsPrepared { get; set; }
        public bool IsDefeated { get; set; }
        public EncounterPhase Phase { get; set; } = EncounterPhase.Prepared;
        public EncounterSnapshot Snapshot { get; set; } = new();
    }
}


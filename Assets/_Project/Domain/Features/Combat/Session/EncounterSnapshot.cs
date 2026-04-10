using _Project.Application.States.Encounter;

namespace _Project.Domain.Features.Combat.Session
{
    public class EncounterSnapshot
    {
        public EncounterPhase Phase { get; set; }
        public string EnemyId { get; set; }
        public string EnemyName { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public int CycleNumber { get; set; }
        public int EncounterIndexInCycle { get; set; }
        public bool IsBoss { get; set; }
        public bool IsFinalBoss { get; set; }
    }
}


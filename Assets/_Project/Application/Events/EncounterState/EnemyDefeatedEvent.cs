using _Project.Application.Interfaces;

namespace _Project.Application.Events.EncounterState
{
    public struct EnemyDefeatedEvent : IEvent
    {
        public string EnemyId;
        public string EnemyName;
        public int CycleNumber;
        public int EncounterIndexInCycle;
        public bool IsBoss;
        public bool IsFinalBoss;
    }
}


using _Project.Application.Interfaces;

namespace _Project.Application.Events.EncounterState
{
    public struct EncounterPreparedEvent : IEvent
    {
        public string EnemyId;
        public string EnemyName;
        public int CurrentHealth;
        public int MaxHealth;
        public int CycleNumber;
        public int EncounterIndexInCycle;
        public bool IsBoss;
        public bool IsFinalBoss;
    }
}


using _Project.Application.Interfaces;

namespace _Project.Application.Events.EncounterState
{
    public struct EnemyDamagedEvent : IEvent
    {
        public string EnemyName;
        public int DamageApplied;
        public int RemainingHealth;
        public int MaxHealth;
    }
}


using _Project.Application.Events.Core;
using _Project.Application.Events.EncounterState;
using _Project.Application.UseCases;
using _Project.Domain.Features.Combat.Entities;
using _Project.Domain.Features.Combat.Enums;
using _Project.Domain.Features.Combat.Session;
using UnityEngine;

namespace _Project.Infrastructure.Features.Combat.Health
{
    public class EnemyHealthService : IEnemyHealthUseCase
    {
        private readonly EnemyEncounterState _enemyEncounterState;

        public EnemyHealthService(EnemyEncounterState enemyEncounterState)
        {
            _enemyEncounterState = enemyEncounterState;
        }

        public void ApplyDamage(int amount)
        {
            if (!TryGetDamageContext(amount, out EncounterPlanEntry currentEncounter, out int damageApplied))
                return;

            ApplyDamageToCurrentEncounter(damageApplied);
            RaiseEnemyDamagedEvent(currentEncounter, damageApplied);

            if (_enemyEncounterState.CurrentHealth > 0) return;

            MarkEnemyDefeated(currentEncounter);
        }

        private bool TryGetDamageContext(int amount, out EncounterPlanEntry currentEncounter, out int damageApplied)
        {
            currentEncounter = _enemyEncounterState.CurrentEncounter;
            damageApplied = 0;

            if (!_enemyEncounterState.IsPrepared || currentEncounter == null || _enemyEncounterState.CurrentHealth <= 0)
                return false;

            int clampedDamage = Mathf.Max(0, amount);
            if (clampedDamage <= 0) return false;

            damageApplied = Mathf.Min(clampedDamage, _enemyEncounterState.CurrentHealth);
            return damageApplied > 0;
        }

        private void ApplyDamageToCurrentEncounter(int damageApplied)
        {
            _enemyEncounterState.CurrentHealth -= damageApplied;
        }

        private void RaiseEnemyDamagedEvent(EncounterPlanEntry currentEncounter, int damageApplied)
        {
            Bus<EnemyDamagedEvent>.Raise(new EnemyDamagedEvent
            {
                EnemyName = currentEncounter.EnemyName,
                DamageApplied = damageApplied,
                RemainingHealth = _enemyEncounterState.CurrentHealth,
                MaxHealth = currentEncounter.MaxHealth
            });
        }

        private void MarkEnemyDefeated(EncounterPlanEntry currentEncounter)
        {
            _enemyEncounterState.IsDefeated = true;

            Bus<EnemyDefeatedEvent>.Raise(new EnemyDefeatedEvent
            {
                EnemyId = currentEncounter.EnemyId,
                EnemyName = currentEncounter.EnemyName,
                CycleNumber = currentEncounter.CycleNumber,
                EncounterIndexInCycle = currentEncounter.EncounterIndexInCycle,
                IsBoss = currentEncounter.EncounterType == EnemyEncounterType.Boss || currentEncounter.EncounterType == EnemyEncounterType.FinalBoss,
                IsFinalBoss = currentEncounter.EncounterType == EnemyEncounterType.FinalBoss
            });
        }
    }
}


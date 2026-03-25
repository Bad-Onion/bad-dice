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
            if (!_enemyEncounterState.IsPrepared) return;

            EncounterPlanEntry currentEncounter = _enemyEncounterState.CurrentEncounter;
            if (currentEncounter == null || _enemyEncounterState.CurrentHealth <= 0) return;

            int clampedDamage = Mathf.Max(0, amount);
            if (clampedDamage <= 0) return;

            int damageApplied = Mathf.Min(clampedDamage, _enemyEncounterState.CurrentHealth);
            _enemyEncounterState.CurrentHealth -= damageApplied;

            Bus<EnemyDamagedEvent>.Raise(new EnemyDamagedEvent
            {
                EnemyName = currentEncounter.EnemyName,
                DamageApplied = damageApplied,
                RemainingHealth = _enemyEncounterState.CurrentHealth,
                MaxHealth = currentEncounter.MaxHealth
            });

            if (_enemyEncounterState.CurrentHealth > 0) return;

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


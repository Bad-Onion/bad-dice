using System.Collections.Generic;
using System.Linq;
using _Project.Application.Events.Core;
using _Project.Application.Events.EncounterState;
using _Project.Application.UseCases;
using _Project.Domain.Features.Combat.Entities;
using _Project.Domain.Features.Combat.Enums;
using _Project.Domain.Features.Combat.ScriptableObjects.Definitions;
using _Project.Domain.Features.Combat.ScriptableObjects.Settings;
using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;
using UnityEngine;

namespace _Project.Infrastructure.Features.Combat.Progression
{
    public class EncounterProgressionService : IEncounterProgressionUseCase
    {
        private readonly CombatSessionState _combatSessionState;
        private readonly EnemyEncounterState _enemyEncounterState;
        private readonly GameConfiguration _gameConfiguration;

        public EncounterProgressionService(
            CombatSessionState combatSessionState,
            EnemyEncounterState enemyEncounterState,
            GameConfiguration gameConfiguration)
        {
            _combatSessionState = combatSessionState;
            _enemyEncounterState = enemyEncounterState;
            _gameConfiguration = gameConfiguration;
        }

        public bool IsInitialized => _combatSessionState.IsInitialized;

        public void InitializeRunProgression()
        {
            if (_combatSessionState.IsInitialized) return;

            EnemyDatabase enemyDatabase = _gameConfiguration.enemyDatabase;
            EnemyProgressionConfiguration progressionConfiguration = _gameConfiguration.enemyProgressionConfiguration;

            if (enemyDatabase == null || progressionConfiguration == null)
            {
                Debug.LogError("Enemy progression is not configured in GameConfiguration.");
                return;
            }

            _combatSessionState.PlannedEncounters.Clear();
            _combatSessionState.CurrentEncounterIndex = 0;

            BuildEncounterPlan(enemyDatabase, progressionConfiguration);
            _combatSessionState.IsInitialized = _combatSessionState.PlannedEncounters.Count > 0;
        }

        public void PrepareCurrentEncounter()
        {
            if (!_combatSessionState.IsInitialized) return;
            if (_combatSessionState.CurrentEncounterIndex < 0 || _combatSessionState.CurrentEncounterIndex >= _combatSessionState.PlannedEncounters.Count) return;

            EncounterPlanEntry encounterPlanEntry = _combatSessionState.PlannedEncounters[_combatSessionState.CurrentEncounterIndex];
            _enemyEncounterState.CurrentEncounter = encounterPlanEntry;
            _enemyEncounterState.CurrentHealth = encounterPlanEntry.MaxHealth;
            _enemyEncounterState.IsPrepared = true;
            _enemyEncounterState.IsDefeated = false;

            Bus<EncounterPreparedEvent>.Raise(new EncounterPreparedEvent
            {
                EnemyId = encounterPlanEntry.EnemyId,
                EnemyName = encounterPlanEntry.EnemyName,
                CurrentHealth = _enemyEncounterState.CurrentHealth,
                MaxHealth = encounterPlanEntry.MaxHealth,
                CycleNumber = encounterPlanEntry.CycleNumber,
                EncounterIndexInCycle = encounterPlanEntry.EncounterIndexInCycle,
                IsBoss = encounterPlanEntry.EncounterType == EnemyEncounterType.Boss || encounterPlanEntry.EncounterType == EnemyEncounterType.FinalBoss,
                IsFinalBoss = encounterPlanEntry.EncounterType == EnemyEncounterType.FinalBoss
            });
        }

        public bool TryAdvanceEncounter()
        {
            if (!_combatSessionState.IsInitialized) return false;

            int nextEncounterIndex = _combatSessionState.CurrentEncounterIndex + 1;
            if (nextEncounterIndex >= _combatSessionState.PlannedEncounters.Count)
            {
                return false;
            }

            _combatSessionState.CurrentEncounterIndex = nextEncounterIndex;
            return true;
        }

        private void BuildEncounterPlan(EnemyDatabase enemyDatabase, EnemyProgressionConfiguration progressionConfiguration)
        {
            for (int cycleNumber = 1; cycleNumber <= progressionConfiguration.TotalCycles; cycleNumber++)
            {
                AddMinorEncounters(enemyDatabase.MinorEnemies, progressionConfiguration.MinorEncountersPerCycle, cycleNumber);
                AddBossEncounter(enemyDatabase, progressionConfiguration.TotalCycles, cycleNumber, progressionConfiguration.MinorEncountersPerCycle + 1);
            }
        }

        private void AddMinorEncounters(IReadOnlyList<MinorEnemyDefinition> minorEnemies, int minorEncountersPerCycle, int cycleNumber)
        {
            if (minorEnemies == null || minorEnemies.Count == 0) return;

            List<MinorEnemyDefinition> shuffledCyclePool = minorEnemies.OrderBy(_ => Random.value).ToList();

            for (int encounterIndexInCycle = 1; encounterIndexInCycle <= minorEncountersPerCycle; encounterIndexInCycle++)
            {
                MinorEnemyDefinition selectedEnemy = shuffledCyclePool.Count > 0
                    ? PopFirst(shuffledCyclePool)
                    : minorEnemies[Random.Range(0, minorEnemies.Count)];

                _combatSessionState.PlannedEncounters.Add(new EncounterPlanEntry
                {
                    EnemyId = selectedEnemy.EnemyId,
                    EnemyName = selectedEnemy.EnemyName,
                    MaxHealth = selectedEnemy.MaxHealth,
                    EncounterType = EnemyEncounterType.Minor,
                    CycleNumber = cycleNumber,
                    EncounterIndexInCycle = encounterIndexInCycle
                });
            }
        }

        private void AddBossEncounter(EnemyDatabase enemyDatabase, int totalCycles, int cycleNumber, int encounterIndexInCycle)
        {
            BossEnemyDefinition selectedBoss = cycleNumber == totalCycles
                ? enemyDatabase.FinalBoss
                : GetRandomBoss(enemyDatabase);

            if (selectedBoss == null) return;

            _combatSessionState.PlannedEncounters.Add(new EncounterPlanEntry
            {
                EnemyId = selectedBoss.EnemyId,
                EnemyName = selectedBoss.EnemyName,
                MaxHealth = selectedBoss.MaxHealth,
                EncounterType = cycleNumber == totalCycles ? EnemyEncounterType.FinalBoss : EnemyEncounterType.Boss,
                CycleNumber = cycleNumber,
                EncounterIndexInCycle = encounterIndexInCycle
            });
        }

        private static MinorEnemyDefinition PopFirst(List<MinorEnemyDefinition> entries)
        {
            MinorEnemyDefinition selectedEnemy = entries[0];
            entries.RemoveAt(0);
            return selectedEnemy;
        }

        private static BossEnemyDefinition GetRandomBoss(EnemyDatabase enemyDatabase)
        {
            if (enemyDatabase.Bosses == null || enemyDatabase.Bosses.Count == 0) return null;

            List<BossEnemyDefinition> candidateBosses = enemyDatabase.Bosses
                .Where(boss => boss != null && boss != enemyDatabase.FinalBoss)
                .ToList();

            if (candidateBosses.Count == 0)
            {
                candidateBosses = enemyDatabase.Bosses.Where(boss => boss != null).ToList();
            }

            if (candidateBosses.Count == 0) return null;

            int randomIndex = Random.Range(0, candidateBosses.Count);
            return candidateBosses[randomIndex];
        }
    }
}


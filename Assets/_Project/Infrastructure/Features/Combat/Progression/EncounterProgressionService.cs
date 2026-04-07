using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Application.Events.Core;
using _Project.Application.Events.GameState;
using _Project.Application.Interfaces;
using _Project.Application.States.DiceSession;
using _Project.Application.States.Encounter;
using _Project.Application.UseCases;
using _Project.Domain.Features.Combat.Entities;
using _Project.Domain.Features.Combat.Enums;
using _Project.Domain.Features.Combat.ScriptableObjects.Definitions;
using _Project.Domain.Features.Combat.ScriptableObjects.Settings;
using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;
using _Project.Domain.Features.Dice.Session;
using _Project.Domain.Features.Run.Session;
using Random = UnityEngine.Random;

namespace _Project.Infrastructure.Features.Combat.Progression
{
    public class EncounterProgressionService : IEncounterProgressionUseCase
    {
        public event Action<EncounterSnapshot> EncounterSnapshotUpdated;

        private readonly CombatSessionState _combatSessionState;
        private readonly EnemyEncounterState _enemyEncounterState;
        private readonly GameConfiguration _gameConfiguration;
        private readonly IRunRepository _runRepository;
        private readonly PlayerRunState _runState;
        private readonly DiceSessionState _diceSessionState;

        public EncounterProgressionService(
            CombatSessionState combatSessionState,
            EnemyEncounterState enemyEncounterState,
            GameConfiguration gameConfiguration,
            IRunRepository runRepository,
            PlayerRunState runState,
            DiceSessionState diceSessionState)
        {
            _combatSessionState = combatSessionState;
            _enemyEncounterState = enemyEncounterState;
            _gameConfiguration = gameConfiguration;
            _runRepository = runRepository;
            _runState = runState;
            _diceSessionState = diceSessionState;
        }

        public bool IsInitialized => _combatSessionState.IsInitialized;

        public void StartEncounter()
        {
            if (!_runState.DiceInventory.Any(diceData => diceData.IsEquipped)) return;

            _diceSessionState.ActiveDice.Clear();
            _diceSessionState.RerollsLeft = _runState.RerollsPerTurn;
            _diceSessionState.CurrentTurn = 1;
            _diceSessionState.MaxTurns = _runState.TurnsPerFight;
            _diceSessionState.HasDealtThisTurn = false;
            _diceSessionState.RollPhase = DiceRollPhase.Idle;
            _diceSessionState.MergeableDiceIds.Clear();
            _diceSessionState.MergeState = MergeState.None;

            var equippedDice = _runState.DiceInventory.Where(diceData => diceData.IsEquipped);
            foreach (var ownedDice in equippedDice)
            {
                _diceSessionState.ActiveDice.Add(new DiceState
                {
                    Dice = ownedDice.Dice,
                    Level = 1,
                    CurrentFaceIndex = -1,
                    IsSelectedForReroll = false
                });
            }

            _runRepository.SaveRun(_runState, _combatSessionState);

            Bus<TurnChangedEvent>.Raise(new TurnChangedEvent
            {
                CurrentTurn = _diceSessionState.CurrentTurn,
                MaxTurns = _diceSessionState.MaxTurns
            });

            _enemyEncounterState.Phase = EncounterPhase.Active;
            if (_enemyEncounterState.Snapshot == null)
            {
                _enemyEncounterState.Snapshot = new EncounterSnapshot();
            }

            _enemyEncounterState.Snapshot.Phase = EncounterPhase.Active;
            EncounterSnapshotUpdated?.Invoke(_enemyEncounterState.Snapshot);
        }

        public void InitializeRunProgression()
        {
            if (_combatSessionState.IsInitialized) return;

            EnemyDatabase enemyDatabase = _gameConfiguration.enemyDatabase;
            EnemyProgressionConfiguration progressionConfiguration = _gameConfiguration.enemyProgressionConfiguration;

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

            _enemyEncounterState.Phase = EncounterPhase.Prepared;
            _enemyEncounterState.Snapshot = new EncounterSnapshot
            {
                Phase = EncounterPhase.Prepared,
                EnemyId = encounterPlanEntry.EnemyId,
                EnemyName = encounterPlanEntry.EnemyName,
                CurrentHealth = _enemyEncounterState.CurrentHealth,
                MaxHealth = encounterPlanEntry.MaxHealth,
                CycleNumber = encounterPlanEntry.CycleNumber,
                EncounterIndexInCycle = encounterPlanEntry.EncounterIndexInCycle,
                IsBoss = encounterPlanEntry.EncounterType == EnemyEncounterType.Boss || encounterPlanEntry.EncounterType == EnemyEncounterType.FinalBoss,
                IsFinalBoss = encounterPlanEntry.EncounterType == EnemyEncounterType.FinalBoss
            };

            EncounterSnapshotUpdated?.Invoke(_enemyEncounterState.Snapshot);
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

                if (selectedEnemy == null) continue;

                AddEncounterPlanEntry(
                    selectedEnemy.EnemyId,
                    selectedEnemy.EnemyName,
                    selectedEnemy.MaxHealth,
                    EnemyEncounterType.Minor,
                    cycleNumber,
                    encounterIndexInCycle);
            }
        }

        private void AddBossEncounter(EnemyDatabase enemyDatabase, int totalCycles, int cycleNumber, int encounterIndexInCycle)
        {
            BossEnemyDefinition selectedBoss = cycleNumber == totalCycles
                ? enemyDatabase.FinalBoss
                : GetRandomBoss(enemyDatabase);

            if (selectedBoss == null) return;

            AddEncounterPlanEntry(
                selectedBoss.EnemyId,
                selectedBoss.EnemyName,
                selectedBoss.MaxHealth,
                cycleNumber == totalCycles ? EnemyEncounterType.FinalBoss : EnemyEncounterType.Boss,
                cycleNumber,
                encounterIndexInCycle);
        }

        private void AddEncounterPlanEntry(
            string enemyId,
            string enemyName,
            int maxHealth,
            EnemyEncounterType encounterType,
            int cycleNumber,
            int encounterIndexInCycle)
        {
            _combatSessionState.PlannedEncounters.Add(new EncounterPlanEntry
            {
                EnemyId = enemyId,
                EnemyName = enemyName,
                MaxHealth = maxHealth,
                EncounterType = encounterType,
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


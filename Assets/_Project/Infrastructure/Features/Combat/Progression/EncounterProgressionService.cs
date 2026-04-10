using System;
using _Project.Application.Interfaces;
using _Project.Application.States.Encounter;
using _Project.Application.UseCases;
using _Project.Domain.Features.Combat.Entities;
using _Project.Domain.Features.Combat.Enums;
using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;

namespace _Project.Infrastructure.Features.Combat.Progression
{
    public class EncounterProgressionService : IEncounterProgressionUseCase
    {
        public event Action<EncounterSnapshot> EncounterSnapshotUpdated;

        private readonly CombatSessionState _combatSessionState;
        private readonly EnemyEncounterState _enemyEncounterState;
        private readonly IEncounterPlanBuilder _encounterPlanBuilder;
        private readonly IEncounterStartUseCase _encounterStartUseCase;
        private readonly GameConfiguration _gameConfiguration;

        public EncounterProgressionService(
            CombatSessionState combatSessionState,
            EnemyEncounterState enemyEncounterState,
            IEncounterPlanBuilder encounterPlanBuilder,
            IEncounterStartUseCase encounterStartUseCase,
            GameConfiguration gameConfiguration)
        {
            _combatSessionState = combatSessionState;
            _enemyEncounterState = enemyEncounterState;
            _encounterPlanBuilder = encounterPlanBuilder;
            _encounterStartUseCase = encounterStartUseCase;
            _gameConfiguration = gameConfiguration;
        }

        public bool IsInitialized => _combatSessionState.IsInitialized;

        public void StartEncounter()
        {
            EncounterSnapshot encounterSnapshot = _encounterStartUseCase.StartEncounter();

            if (encounterSnapshot == null) return;

            EncounterSnapshotUpdated?.Invoke(encounterSnapshot);
        }

        public void InitializeRunProgression()
        {
            if (_combatSessionState.IsInitialized) return;

            ResetRunProgressionState();

            _combatSessionState.PlannedEncounters.AddRange(
                _encounterPlanBuilder.BuildPlan(_gameConfiguration.enemyDatabase,
                    _gameConfiguration.enemyProgressionConfiguration));
            _combatSessionState.IsInitialized = HasPlannedEncounters();
        }

        public void PrepareCurrentEncounter()
        {
            if (!_combatSessionState.IsInitialized) return;
            if (!IsCurrentEncounterIndexValid()) return;

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
                IsBoss = IsBossEncounter(encounterPlanEntry.EncounterType),
                IsFinalBoss = IsFinalBossEncounter(encounterPlanEntry.EncounterType)
            };

            EncounterSnapshotUpdated?.Invoke(_enemyEncounterState.Snapshot);
        }

        public bool TryAdvanceEncounter()
        {
            if (!_combatSessionState.IsInitialized) return false;

            int nextEncounterIndex = _combatSessionState.CurrentEncounterIndex + 1;

            if (nextEncounterIndex >= _combatSessionState.PlannedEncounters.Count) return false;

            _combatSessionState.CurrentEncounterIndex = nextEncounterIndex;
            return true;
        }

        private void ResetRunProgressionState()
        {
            _combatSessionState.PlannedEncounters.Clear();
            _combatSessionState.CurrentEncounterIndex = 0;
        }

        private bool HasPlannedEncounters()
        {
            return _combatSessionState.PlannedEncounters.Count > 0;
        }

        private bool IsCurrentEncounterIndexValid()
        {
            return _combatSessionState.CurrentEncounterIndex >= 0 &&
                   _combatSessionState.CurrentEncounterIndex < _combatSessionState.PlannedEncounters.Count;
        }

        private static bool IsBossEncounter(EnemyEncounterType encounterType)
        {
            return encounterType == EnemyEncounterType.Boss ||
                   encounterType == EnemyEncounterType.FinalBoss;
        }

        private static bool IsFinalBossEncounter(EnemyEncounterType encounterType)
        {
            return encounterType == EnemyEncounterType.FinalBoss;
        }
    }
}
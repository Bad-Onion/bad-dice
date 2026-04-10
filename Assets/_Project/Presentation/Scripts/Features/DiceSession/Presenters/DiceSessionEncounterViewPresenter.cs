using _Project.Application.Interfaces;
using _Project.Application.States.Encounter;
using _Project.Application.UseCases;
using _Project.Domain.Features.Combat.Entities;
using _Project.Domain.Features.Combat.Enums;
using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;

namespace _Project.Presentation.Scripts.Features.DiceSession.Presenters
{
    public class DiceSessionEncounterViewPresenter : IDiceSessionPresenterLifecycle
    {
        private readonly EnemyEncounterState _enemyEncounterState;
        private readonly GameConfiguration _gameConfiguration;
        private readonly IEncounterProgressionUseCase _encounterProgressionUseCase;

        private IDiceSessionView _view;

        public DiceSessionEncounterViewPresenter(
            EnemyEncounterState enemyEncounterState,
            GameConfiguration gameConfiguration,
            IEncounterProgressionUseCase encounterProgressionUseCase)
        {
            _enemyEncounterState = enemyEncounterState;
            _gameConfiguration = gameConfiguration;
            _encounterProgressionUseCase = encounterProgressionUseCase;
        }

        public void Attach(IDiceSessionView view)
        {
            _view = view;
            _encounterProgressionUseCase.EncounterSnapshotUpdated += OnEncounterSnapshotUpdated;
        }

        public void Detach()
        {
            _encounterProgressionUseCase.EncounterSnapshotUpdated -= OnEncounterSnapshotUpdated;
            _view = null;
        }

        public void RefreshEnemyPanelFromState()
        {
            if (_view == null || _enemyEncounterState?.CurrentEncounter == null)
            {
                return;
            }

            EncounterPlanEntry currentEncounter = _enemyEncounterState.CurrentEncounter;
            int maxEncountersPerCycle = GetMaxEncountersPerCycle();

            _view.SetEnemyInfo($"Enemy: {currentEncounter.EnemyName} ({GetEncounterTypeText(currentEncounter.EncounterType)})");
            _view.SetEnemyHealth($"HP: {_enemyEncounterState.CurrentHealth}/{currentEncounter.MaxHealth}");
            _view.SetCycleInfo($"Cycle {currentEncounter.CycleNumber} - Encounter {currentEncounter.EncounterIndexInCycle}/{maxEncountersPerCycle}");
            _view.SetDicePanelVisible(_enemyEncounterState.Phase == EncounterPhase.Active);
        }

        private void OnEncounterSnapshotUpdated(EncounterSnapshot snapshot)
        {
            if (_view == null || snapshot == null)
            {
                return;
            }

            _view.SetEnemyInfo($"Enemy: {snapshot.EnemyName} ({GetEncounterTypeText(GetEncounterType(snapshot.IsBoss, snapshot.IsFinalBoss))})");
            _view.SetEnemyHealth($"HP: {snapshot.CurrentHealth}/{snapshot.MaxHealth}");
            _view.SetCycleInfo($"Cycle {snapshot.CycleNumber} - Encounter {snapshot.EncounterIndexInCycle}/{GetMaxEncountersPerCycle()}");
            _view.SetDicePanelVisible(snapshot.Phase == EncounterPhase.Active);
        }

        private int GetMaxEncountersPerCycle()
        {
            int minorEncountersPerCycle = _gameConfiguration?.enemyProgressionConfiguration?.MinorEncountersPerCycle ?? 0;
            return minorEncountersPerCycle + 1;
        }

        private static EnemyEncounterType GetEncounterType(bool isBoss, bool isFinalBoss)
        {
            if (isFinalBoss)
            {
                return EnemyEncounterType.FinalBoss;
            }

            if (isBoss)
            {
                return EnemyEncounterType.Boss;
            }

            return EnemyEncounterType.Minor;
        }

        private static string GetEncounterTypeText(EnemyEncounterType encounterType)
        {
            switch (encounterType)
            {
                case EnemyEncounterType.FinalBoss:
                    return "FINAL BOSS";
                case EnemyEncounterType.Boss:
                    return "BOSS";
                default:
                    return "MINOR";
            }
        }
    }
}


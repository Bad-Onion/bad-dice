using System;
using System.Linq;
using _Project.Application.Events.Core;
using _Project.Application.Events.EncounterState;
using _Project.Application.Events.GameState;
using _Project.Application.Events.Load;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Dice.Session;
using _Project.Domain.Features.GameFlow.Session;
using _Project.Domain.Features.Run.Session;
using Zenject;

namespace _Project.Infrastructure.Features.Combat.Orchestration
{
    public class EncounterFlowCoordinator : IInitializable, IDisposable
    {
        private readonly IEncounterProgressionUseCase _encounterProgressionUseCase;
        private readonly ISceneLoader _sceneLoader;
        private readonly GameSession _gameSession;
        private readonly PlayerRunState _runState;
        private readonly DiceSessionState _diceSessionState;

        private bool _isReloadInProgress;

        public EncounterFlowCoordinator(
            IEncounterProgressionUseCase encounterProgressionUseCase,
            ISceneLoader sceneLoader,
            GameSession gameSession,
            PlayerRunState runState,
            DiceSessionState diceSessionState)
        {
            _encounterProgressionUseCase = encounterProgressionUseCase;
            _sceneLoader = sceneLoader;
            _gameSession = gameSession;
            _runState = runState;
            _diceSessionState = diceSessionState;
        }

        public void Initialize()
        {
            Bus<LevelLoadedEvent>.OnEvent += HandleLevelLoaded;
            Bus<EnemyDefeatedEvent>.OnEvent += HandleEnemyDefeated;
            Bus<EncounterStartRequestedEvent>.OnEvent += HandleEncounterStartRequested;
        }

        public void Dispose()
        {
            Bus<LevelLoadedEvent>.OnEvent -= HandleLevelLoaded;
            Bus<EnemyDefeatedEvent>.OnEvent -= HandleEnemyDefeated;
            Bus<EncounterStartRequestedEvent>.OnEvent -= HandleEncounterStartRequested;
        }

        private void HandleEncounterStartRequested(EncounterStartRequestedEvent evt)
        {
            StartEncounter();
        }

        private void HandleLevelLoaded(LevelLoadedEvent evt)
        {
            if (!_encounterProgressionUseCase.IsInitialized)
            {
                _encounterProgressionUseCase.InitializeRunProgression();
            }

            _encounterProgressionUseCase.PrepareCurrentEncounter();
        }

        private void HandleEnemyDefeated(EnemyDefeatedEvent evt)
        {
            if (_isReloadInProgress) return;

            bool hasMoreEncounters = _encounterProgressionUseCase.TryAdvanceEncounter();
            if (!hasMoreEncounters)
            {
                Bus<RunCompletedEvent>.Raise(new RunCompletedEvent());
                return;
            }

            ReloadEncounterScene();
        }

        private void ReloadEncounterScene()
        {
            if (_gameSession.CurrentLevelData == null || string.IsNullOrEmpty(_gameSession.CurrentLevelData.SceneName))
            {
                _encounterProgressionUseCase.PrepareCurrentEncounter();
                return;
            }

            string sceneName = _gameSession.CurrentLevelData.SceneName;
            _isReloadInProgress = true;

            _sceneLoader.UnloadScene(sceneName, () =>
            {
                _sceneLoader.LoadSceneAdditive(sceneName, () =>
                {
                    _isReloadInProgress = false;
                    _encounterProgressionUseCase.PrepareCurrentEncounter();
                });
            });
        }

        private void StartEncounter()
        {
            if (!IsAnyDiceEquipped()) return;

            _diceSessionState.ActiveDice.Clear();
            _diceSessionState.RerollsLeft = _runState.RerollsPerTurn;
            _diceSessionState.CurrentTurn = 1;
            _diceSessionState.MaxTurns = _runState.TurnsPerFight;
            _diceSessionState.HasDealtThisTurn = false;
            _diceSessionState.MergeableDiceIds.Clear();

            SetActiveDiceFromInventory();

            Bus<TurnChangedEvent>.Raise(new TurnChangedEvent
            {
                CurrentTurn = _diceSessionState.CurrentTurn,
                MaxTurns = _diceSessionState.MaxTurns
            });

            Bus<EncounterStartedEvent>.Raise(new EncounterStartedEvent());
        }

        private bool IsAnyDiceEquipped()
        {
            return _runState.DiceInventory.Any(diceData => diceData.IsEquipped);
        }

        private void SetActiveDiceFromInventory()
        {
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
        }
    }
}


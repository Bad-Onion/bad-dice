using System;
using _Project.Application.Events.Core;
using _Project.Application.Events.EncounterState;
using _Project.Application.Events.GameState;
using _Project.Application.Events.Load;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Features.GameFlow.Session;
using Zenject;

namespace _Project.Infrastructure.Features.Combat.Orchestration
{
    public class EncounterFlowCoordinator : IInitializable, IDisposable
    {
        private readonly IEncounterProgressionUseCase _encounterProgressionUseCase;
        private readonly ISceneLoader _sceneLoader;
        private readonly GameSession _gameSession;

        private bool _isReloadInProgress;

        public EncounterFlowCoordinator(
            IEncounterProgressionUseCase encounterProgressionUseCase,
            ISceneLoader sceneLoader,
            GameSession gameSession)
        {
            _encounterProgressionUseCase = encounterProgressionUseCase;
            _sceneLoader = sceneLoader;
            _gameSession = gameSession;
        }

        public void Initialize()
        {
            Bus<LevelLoadedEvent>.OnEvent += HandleLevelLoaded;
            Bus<EnemyDefeatedEvent>.OnEvent += HandleEnemyDefeated;
        }

        public void Dispose()
        {
            Bus<LevelLoadedEvent>.OnEvent -= HandleLevelLoaded;
            Bus<EnemyDefeatedEvent>.OnEvent -= HandleEnemyDefeated;
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
    }
}


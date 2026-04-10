using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;
using _Project.Domain.Features.Run.Session;
using UnityEngine;

namespace _Project.Infrastructure.Features.Run.Persistence
{
    public class RunGameInitializationService : IRunInitializationUseCase
    {
        private readonly IRunRepository _repository;
        private readonly IRunStateBuilder _runStateBuilder;
        private readonly GameConfiguration _gameConfiguration;
        private readonly PlayerRunState _runState;
        private readonly CombatSessionState _combatSessionState;

        public RunGameInitializationService(
            IRunRepository repository,
            IRunStateBuilder runStateBuilder,
            GameConfiguration gameConfiguration,
            PlayerRunState runState,
            CombatSessionState combatSessionState)
        {
            _repository = repository;
            _runStateBuilder = runStateBuilder;
            _gameConfiguration = gameConfiguration;
            _runState = runState;
            _combatSessionState = combatSessionState;
        }

        public void EnsureRunInitialized()
        {
            // Don't remove this, it's useful for debugging and testing to reset the run state without having to clear PlayerPrefs manually
            PlayerPrefs.DeleteAll();

            if (_repository.HasActiveRun())
            {
                LoadRun();
                return;
            }

            InitializeRun();
        }

        private void LoadRun()
        {
            var loadedState = _repository.LoadRun();
            _runStateBuilder.BuildFromExisting(_runState, loadedState, _gameConfiguration.runDefinitions);
            _repository.RestoreCombatProgression(_combatSessionState);
        }

        private void InitializeRun()
        {
            _runStateBuilder.BuildNew(_runState, _gameConfiguration.runDefinitions);
            _repository.SaveRun(_runState, _combatSessionState);
        }
    }
}
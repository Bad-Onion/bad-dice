using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities.Session;
using _Project.Domain.ScriptableObjects.GameSettings;

namespace _Project.Infrastructure.Services
{
    public class RunGameInitializationService : IRunInitializationUseCase
    {
        private readonly IRunRepository _repository;
        private readonly IRunStateBuilder _runStateBuilder;
        private readonly GameConfiguration _gameConfiguration;
        private readonly PlayerRunState _runState;

        public RunGameInitializationService(
            IRunRepository repository,
            IRunStateBuilder runStateBuilder,
            GameConfiguration gameConfiguration,
            PlayerRunState runState)
        {
            _repository = repository;
            _runStateBuilder = runStateBuilder;
            _gameConfiguration = gameConfiguration;
            _runState = runState;
        }

        public void EnsureRunInitialized()
        {
            // Don't remove this, it's useful for debugging and testing to reset the run state without having to clear PlayerPrefs manually
            // PlayerPrefs.DeleteAll();

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
        }

        private void InitializeRun()
        {
            _runStateBuilder.BuildNew(_runState, _gameConfiguration.runDefinitions);
            _repository.SaveRun(_runState);
        }
    }
}
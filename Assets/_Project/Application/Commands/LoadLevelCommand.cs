using System;
using Zenject;
using _Project.Application.Events.Core;
using _Project.Application.Events.Load;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;

namespace _Project.Application.Commands
{
    /// <summary>
    /// Command to load a level based on the provided LevelData. It uses an ISceneLoader to load the scene additively and raises
    /// a LevelLoadedEvent upon completion. It also ensures that the initialization use case has been run before loading the level.
    /// </summary>
    public class LoadLevelCommand : ICommand
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly LevelData _levelData;
        private readonly Action _onComplete;
        private readonly IRunInitializationUseCase _runInitializationUseCase;

        public LoadLevelCommand(ISceneLoader sceneLoader, LevelData levelData, Action onComplete, IRunInitializationUseCase runInitializationUseCase)
        {
            _sceneLoader = sceneLoader;
            _levelData = levelData;
            _onComplete = onComplete;
            _runInitializationUseCase = runInitializationUseCase;
        }

        public ValidationResult Validate()
        {
            if (_levelData == null)
            {
                return ValidationResult.Failure("LevelDataMissing", "Cannot load a level without LevelData.");
            }

            return !string.IsNullOrEmpty(_levelData.SceneName)
                ? ValidationResult.Success()
                : ValidationResult.Failure("SceneNameMissing", "Cannot load a level with an empty scene name.");
        }

        public CommandResult Execute()
        {
            try
            {
                _runInitializationUseCase.EnsureRunInitialized();

                _sceneLoader.LoadSceneAdditive(_levelData.SceneName, () =>
                {
                    Bus<LevelLoadedEvent>.Raise(new LevelLoadedEvent(_levelData));
                    _onComplete?.Invoke();
                });

                return CommandResult.Success();
            }
            catch (Exception exception)
            {
                return CommandResult.Failure("LoadLevelFailed", exception.Message);
            }
        }

        public class Factory : PlaceholderFactory<LevelData, Action, LoadLevelCommand> { }
    }
}
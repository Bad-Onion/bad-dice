using System;
using Zenject;
using _Project.Application.Interfaces;
using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;

namespace _Project.Application.Commands
{
    /// <summary>
    /// Command to unload a level based on the provided LevelData. It uses an ISceneLoader to unload the scene and invokes a callback upon completion.
    /// </summary>
    public class UnloadLevelCommand : ICommand
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly LevelData _levelData;
        private readonly Action _onComplete;

        public UnloadLevelCommand(ISceneLoader sceneLoader, LevelData levelData, Action onComplete)
        {
            _sceneLoader = sceneLoader;
            _levelData = levelData;
            _onComplete = onComplete;
        }

        public ValidationResult Validate()
        {
            if (_levelData == null)
            {
                return ValidationResult.Failure("LevelDataMissing", "Cannot unload a level without LevelData.");
            }

            return !string.IsNullOrEmpty(_levelData.SceneName)
                ? ValidationResult.Success()
                : ValidationResult.Failure("SceneNameMissing", "Cannot unload a level with an empty scene name.");
        }

        public CommandResult Execute()
        {
            try
            {
                _sceneLoader.UnloadScene(_levelData.SceneName, _onComplete);
                return CommandResult.Success();
            }
            catch (Exception exception)
            {
                return CommandResult.Failure("UnloadLevelFailed", exception.Message);
            }
        }

        public class Factory : PlaceholderFactory<LevelData, Action, UnloadLevelCommand> { }
    }
}
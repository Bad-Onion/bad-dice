using System;
using Zenject;
using _Project.Application.Interfaces;
using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;

namespace _Project.Application.Commands
{
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

        public bool IsValid() => _levelData != null && !string.IsNullOrEmpty(_levelData.SceneName);

        public void Execute()
        {
            _sceneLoader.UnloadScene(_levelData.SceneName, _onComplete);
        }

        public class Factory : PlaceholderFactory<LevelData, Action, UnloadLevelCommand> { }
    }
}
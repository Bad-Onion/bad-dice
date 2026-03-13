using System;
using Zenject;
using _Project.Application.Events;
using _Project.Application.Interfaces;
using _Project.Domain.ScriptableObjects;

namespace _Project.Application.Commands
{
    public class LoadLevelCommand : ICommand
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly LevelData _levelData;
        private readonly Action _onComplete;

        public LoadLevelCommand(ISceneLoader sceneLoader, LevelData levelData, Action onComplete)
        {
            _sceneLoader = sceneLoader;
            _levelData = levelData;
            _onComplete = onComplete;
        }

        public bool IsValid() => _levelData != null && !string.IsNullOrEmpty(_levelData.SceneName);

        public void Execute()
        {
            _sceneLoader.LoadSceneAdditive(_levelData.SceneName, () =>
            {
                Bus<LevelLoadedEvent>.Raise(new LevelLoadedEvent(_levelData));
                _onComplete?.Invoke();
            });
        }

        public class Factory : PlaceholderFactory<LevelData, Action, LoadLevelCommand> { }
    }
}
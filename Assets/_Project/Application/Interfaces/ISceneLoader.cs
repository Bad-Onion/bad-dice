using System;

namespace _Project.Application.Interfaces
{
    public interface ISceneLoader
    {
        void LoadSceneAdditive(string sceneName, Action onComplete);
        void UnloadScene(string sceneName, Action onComplete);
    }
}
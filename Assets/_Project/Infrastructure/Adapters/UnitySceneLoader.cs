using System;
using UnityEngine.SceneManagement;
using _Project.Application.Interfaces;

namespace _Project.Infrastructure.Adapters
{
    public class UnitySceneLoader : ISceneLoader
    {
        public void LoadSceneAdditive(string sceneName, Action onComplete)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (asyncLoad == null)
            {
                onComplete?.Invoke();
                return;
            }

            asyncLoad.completed += (op) =>
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
                onComplete?.Invoke();
            };
        }

        public void UnloadScene(string sceneName, Action onComplete)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                onComplete?.Invoke();
                return;
            }

            var asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
            if (asyncUnload == null)
            {
                onComplete?.Invoke();
                return;
            }

            asyncUnload.completed += (op) => onComplete?.Invoke();
        }
    }
}
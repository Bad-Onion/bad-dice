using System;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Service interface for managing scene loading and unloading operations.
    /// Supports additive scene loading with completion callbacks for managing scene transitions.
    /// </summary>
    public interface ISceneLoader
    {
        /// <summary>
        /// Loads a scene additively (without unloading other scenes) and invokes a callback upon completion.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="onComplete">The callback to invoke when the scene loading is complete.</param>
        void LoadSceneAdditive(string sceneName, Action onComplete);

        /// <summary>
        /// Unloads a scene and invokes a callback upon completion.
        /// </summary>
        /// <param name="sceneName">The name of the scene to unload.</param>
        /// <param name="onComplete">The callback to invoke when the scene unloading is complete.</param>
        void UnloadScene(string sceneName, Action onComplete);
    }
}
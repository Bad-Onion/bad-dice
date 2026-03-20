using UnityEngine;

namespace _Project.Domain.ScriptableObjects
{
    // TODO: Move to "ScriptableObjects/GameSettings"
    [CreateAssetMenu(menuName = "Project/Data/Level Data", fileName = "NewLevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("Scene Settings")]
        [Tooltip("The name of the scene to be loaded when this level is loaded.")]
        public string SceneName { get; private set; }
    }
}
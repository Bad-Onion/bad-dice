using UnityEngine;

namespace _Project.Domain.Features.GameFlow.ScriptableObjects.Settings
{
    [CreateAssetMenu(menuName = "Project/Data/Level Data", fileName = "NewLevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("Scene Settings")]
        [Tooltip("The name of the scene to be loaded when this level is loaded.")]
        [field:SerializeField] public string SceneName { get; private set; }
    }
}
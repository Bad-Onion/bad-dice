using UnityEngine;

namespace _Project.Domain.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Project/Data/Level Data", fileName = "NewLevelData")]
    public class LevelData : ScriptableObject
    {
        [field: SerializeField] public string SceneName { get; private set; }
        // Future Player Spawn Position, Level Music, etc.
    }
}
using UnityEngine;

namespace _Project.Domain.ScriptableObjects
{
    // TODO: Move to "ScriptableObjects/GameSettings"
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Domain/GameConfiguration")]
    public class GameConfiguration : ScriptableObject
    {
        [Header("Level Settings")]
        [Tooltip("The main level data to be loaded upon starting the game.")]
        public LevelData defaultLevelData;
    }
}
using UnityEngine;

namespace _Project.Domain.ScriptableObjects.GameSettings
{
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Domain/GameConfiguration")]
    public class GameConfiguration : ScriptableObject
    {
        [Header("Run Settings")]
        [Tooltip("The starting run definitions to be used by the game.")]
        public RunDefinitions runDefinitions;

        [Header("Level Settings")]
        [Tooltip("The main level data to be loaded upon starting the game.")]
        public LevelData defaultLevelData;
    }
}
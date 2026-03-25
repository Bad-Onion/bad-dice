using _Project.Domain.Features.Run.ScriptableObjects.Settings;
using _Project.Domain.Features.Combat.ScriptableObjects.Definitions;
using _Project.Domain.Features.Combat.ScriptableObjects.Settings;
using UnityEngine;

namespace _Project.Domain.Features.GameFlow.ScriptableObjects.Settings
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

        [Header("Combat Settings")]
        [Tooltip("Database containing all available minor enemies, bosses and the fixed final boss.")]
        public EnemyDatabase enemyDatabase;

        [Tooltip("Progression rules that define cycle and encounter counts.")]
        public EnemyProgressionConfiguration enemyProgressionConfiguration;
    }
}
using UnityEngine;

namespace _Project.Domain.ScriptableObjects.GameSettings
{
    [CreateAssetMenu(fileName = "NewRunDefinition", menuName = "Domain/GameConfiguration/RunDefinition")]
    public class RunDefinitions : ScriptableObject
    {
        [Header("Run Settings")]
        [Tooltip("Maximum number of dice that can be equipped at the same time.")]
        [Range(1, 10)]
        public int maxEquippedDice = 5;

        [Tooltip("Number of rerolls available per turn.")]
        [Range(0, 5)]
        public int rerollsPerTurn = 3;

        [Tooltip("Number of turns per fight.")]
        [Range(1, 10)]
        public int turnsPerFight = 3;

        [Tooltip("Starting dice pool configuration for a new run.")]
        public StartingDicePool startingDicePool;
    }
}
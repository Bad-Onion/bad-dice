using UnityEngine;

namespace _Project.Domain.Features.Combat.ScriptableObjects.Settings
{
    [CreateAssetMenu(fileName = "EnemyProgressionConfiguration", menuName = "Domain/Combat/Enemy Progression Configuration")]
    public class EnemyProgressionConfiguration : ScriptableObject
    {
        [Header("Cycle Settings")]
        [Tooltip("Total number of cycles in a full run.")]
        [Range(1, 20)]
        [SerializeField] private int totalCycles = 9;

        [Tooltip("How many minor encounters happen before the cycle boss.")]
        [Range(1, 10)]
        [SerializeField] private int minorEncountersPerCycle = 3;

        public int TotalCycles => totalCycles;
        public int MinorEncountersPerCycle => minorEncountersPerCycle;
    }
}


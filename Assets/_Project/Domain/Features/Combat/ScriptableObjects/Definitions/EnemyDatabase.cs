using System.Collections.Generic;
using UnityEngine;

namespace _Project.Domain.Features.Combat.ScriptableObjects.Definitions
{
    [CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Domain/Combat/Enemy Database")]
    public class EnemyDatabase : ScriptableObject
    {
        [Header("Minor Enemies")]
        [Tooltip("Pool used for the 3 random non-boss encounters in each cycle.")]
        [SerializeField] private List<MinorEnemyDefinition> minorEnemies = new();

        [Header("Bosses")]
        [Tooltip("Pool used for random bosses from cycles 1 to 8.")]
        [SerializeField] private List<BossEnemyDefinition> bosses = new();

        [Tooltip("Fixed final boss used in the last encounter of cycle 9.")]
        [SerializeField] private BossEnemyDefinition finalBoss;

        public IReadOnlyList<MinorEnemyDefinition> MinorEnemies => minorEnemies;
        public IReadOnlyList<BossEnemyDefinition> Bosses => bosses;
        public BossEnemyDefinition FinalBoss => finalBoss;
    }
}


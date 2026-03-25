using UnityEngine;

namespace _Project.Domain.Features.Combat.ScriptableObjects.Definitions
{
    [CreateAssetMenu(fileName = "MinorEnemyDefinition", menuName = "Domain/Combat/Minor Enemy")]
    public class MinorEnemyDefinition : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique identifier for save/load and analytics.")]
        [SerializeField] private string enemyId;

        [Tooltip("Display name used in the encounter UI.")]
        [SerializeField] private string enemyName;

        [Header("Combat")]
        [Tooltip("Maximum health for this enemy.")]
        [Min(1)]
        [SerializeField] private int maxHealth = 20;

        public string EnemyId => enemyId;
        public string EnemyName => enemyName;
        public int MaxHealth => maxHealth;
    }
}


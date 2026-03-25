using UnityEngine;

namespace _Project.Domain.Features.Combat.ScriptableObjects.Definitions
{
    [CreateAssetMenu(fileName = "BossEnemyDefinition", menuName = "Domain/Combat/Boss Enemy")]
    public class BossEnemyDefinition : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique identifier for save/load and analytics.")]
        [SerializeField] private string enemyId;

        [Tooltip("Display name used in the encounter UI.")]
        [SerializeField] private string enemyName;

        [Header("Combat")]
        [Tooltip("Maximum health for this boss.")]
        [Min(1)]
        [SerializeField] private int maxHealth = 80;

        public string EnemyId => enemyId;
        public string EnemyName => enemyName;
        public int MaxHealth => maxHealth;
    }
}


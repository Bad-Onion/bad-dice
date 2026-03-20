using UnityEngine;

namespace _Project.Domain.ScriptableObjects.Configuration
{
    [CreateAssetMenu(fileName = "DiceRollConfiguration", menuName = "Domain/Dice/DiceRollConfiguration")]
    public class DiceRollConfiguration : ScriptableObject
    {
        [Header("Physics Settings")]
        [Tooltip("Distance between each dice spawn point to avoid immediate clipping.")]
        [Range(0.1f, 3f)] public float spawnSpacing = 1.2f;

        [Tooltip("The force applied to the dice. The force is randomly generated between minForce and maxForce.")]
        [Range(1f, 10f)] public float minForce = 3f;

        [Tooltip("The force applied to the dice. The force is randomly generated between minForce and maxForce.")]
        [Range(5f, 20f)] public float maxForce = 8f;

        [Tooltip("The torque is the force of rotation applied to the dice.")]
        [Range(10f, 100f)] public float torqueMultiplier = 50f;

        [Header("Simulation")]
        [Tooltip("The spawn position of the dices. The dice will be spawned in a grid pattern around this point based on the number of dice and spawn spacing.")]
        public Vector3 spawnCenter = new Vector3(0, 5, 0);
    }
}
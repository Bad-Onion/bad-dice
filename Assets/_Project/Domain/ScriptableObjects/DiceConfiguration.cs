using UnityEngine;

namespace _Project.Domain.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DiceConfiguration", menuName = "Domain/DiceConfiguration")]
    public class DiceConfiguration : ScriptableObject
    {
        [Header("Physics Settings")]
        [Tooltip("Minimum force applied to the dice.")]
        [Range(1f, 10f)] public float minForce = 3f;

        [Tooltip("Maximum force applied to the dice.")]
        [Range(5f, 20f)] public float maxForce = 8f;

        [Tooltip("Amount of random torque applied.")]
        [Range(10f, 100f)] public float torqueMultiplier = 50f;

        [Header("Initial Spawn")]
        [Tooltip("Where the dice spawns/resets above the table.")]
        public Vector3 spawnPosition = new Vector3(0, 5, 0);

        [Tooltip("Prefab used purely for the hidden physics simulation. Must contain a Rigidbody and Collider matching the visual dice.")]
        public GameObject physicsPrefab;

        public Vector3 GetLocalUpForFace(int faceNumber)
        {
            return faceNumber switch
            {
                1 => Vector3.up,
                2 => Vector3.forward,
                3 => Vector3.right,
                4 => Vector3.left,
                5 => Vector3.back,
                6 => Vector3.down,
                _ => Vector3.up
            };
        }
    }
}
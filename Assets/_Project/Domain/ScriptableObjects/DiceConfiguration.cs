using UnityEngine;

namespace _Project.Domain.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DiceConfiguration", menuName = "Domain/DiceConfiguration")]
    public class DiceConfiguration : ScriptableObject
    {
        [Header("Pool & Count Settings")]
        [Tooltip("Prefab used for the visual dice representation (no colliders).")]
        public GameObject VisualPrefab;

        [Tooltip("Number of dice to roll simultaneously.")]
        [Range(1, 10)] public int DiceCount = 5;

        [Tooltip("Distance between each dice spawn point to avoid immediate clipping.")]
        [Range(0.1f, 3f)] public float SpawnSpacing = 1.2f;

        [Header("Physics Settings")]
        [Range(1f, 10f)] public float MinForce = 3f;
        [Range(5f, 20f)] public float MaxForce = 8f;
        [Range(10f, 100f)] public float TorqueMultiplier = 50f;

        [Header("Simulation")]
        public Vector3 SpawnCenter = new Vector3(0, 5, 0);
        public GameObject PhysicsPrefab;

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
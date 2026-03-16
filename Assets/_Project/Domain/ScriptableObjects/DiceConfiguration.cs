using UnityEngine;

namespace _Project.Domain.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DiceConfiguration", menuName = "Domain/DiceConfiguration")]
    public class DiceConfiguration : ScriptableObject
    {
        [Header("Dice Pool Settings")]
        [Tooltip("Array of dice definitions to roll. The amount of dice rolled equals the length of this array.")]
        public DiceDefinition[] diceDefinitions;

        [Tooltip("Distance between each dice spawn point to avoid immediate clipping.")]
        [Range(0.1f, 3f)] public float spawnSpacing = 1.2f;

        [Header("Physics Settings")]
        [Range(1f, 10f)] public float minForce = 3f;
        [Range(5f, 20f)] public float maxForce = 8f;
        [Range(10f, 100f)] public float torqueMultiplier = 50f;

        [Header("Simulation")]
        public Vector3 spawnCenter = new Vector3(0, 5, 0);
        public GameObject physicsPrefab;

        public int DiceCount => diceDefinitions != null ? diceDefinitions.Length : 0;

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
using _Project.Domain.Features.Dice.DTO;
using UnityEngine;

namespace _Project.Domain.Features.Dice.ScriptableObjects.Definitions
{
    [CreateAssetMenu(fileName = "NewDiceDefinition", menuName = "Domain/Dice/DiceDefinition")]
    public class DiceDefinition : ScriptableObject
    {
        [Header("Prefabs")]
        [Tooltip("The visual root prefab used by runtime systems to spawn this die.")]
        public GameObject visualPrefab;

        [Tooltip("The physics dummy for this specific dice (Rigidbody and Colliders only).")]
        public GameObject physicsPrefab;

        [Header("Visual Configuration")]
        [Tooltip("Visual data used to configure the runtime visual prefab for this die.")]
        public DiceVisualConfigurationData visualConfiguration;

        [Header("Faces Setup")]
        [Tooltip("Define the faces of this dice. For a D6, this array should have 6 elements.")]
        public DiceFaceData[] faces;

        public int GetRandomFaceIndex()
        {
            if (faces == null || faces.Length == 0) return 0;
            return Random.Range(0, faces.Length);
        }

        public DiceFaceData GetFaceData(int faceIndex)
        {
            if (faces == null || faceIndex < 0 || faceIndex >= faces.Length) return default;
            return faces[faceIndex];
        }
    }
}
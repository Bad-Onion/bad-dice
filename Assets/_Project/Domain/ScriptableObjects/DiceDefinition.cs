using System;
using _Project.Domain.Enums;
using UnityEngine;

namespace _Project.Domain.ScriptableObjects
{
    [Serializable]
    public struct DiceFaceData
    {
        [Tooltip("The numerical value or effect ID of this face.")]
        public int value;

        [Tooltip("The local direction this face points to when resting. Choose the direction that aligns with the face in the 3D model.")]
        public DiceFaceDirection localDirection;
    }

    [CreateAssetMenu(fileName = "NewDiceDefinition", menuName = "Domain/Dice/DiceDefinition")]
    public class DiceDefinition : ScriptableObject
    {
        [Header("Prefabs")]
        [Tooltip("The visual representation of the dice (no colliders).")]
        public GameObject visualPrefab;

        [Tooltip("The physics dummy for this specific dice (Rigidbody and Colliders only).")]
        public GameObject physicsPrefab;

        [Header("Faces Setup")]
        [Tooltip("Define the faces of this dice. For a D6, this array should have 6 elements.")]
        public DiceFaceData[] faces;

        public int GetRandomFaceIndex()
        {
            if (faces == null || faces.Length == 0) return 0;
            return UnityEngine.Random.Range(0, faces.Length);
        }

        public DiceFaceData GetFaceData(int faceIndex)
        {
            if (faces == null || faceIndex < 0 || faceIndex >= faces.Length) return default;
            return faces[faceIndex];
        }
    }
}
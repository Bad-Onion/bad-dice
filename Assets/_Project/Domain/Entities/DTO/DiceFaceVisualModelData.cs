using System;
using _Project.Domain.Enums;
using UnityEngine;

namespace _Project.Domain.Entities.DTO
{
    [Serializable]
    public struct DiceFaceVisualModelData
    {
        [Tooltip("The local direction where this face model should be attached.")]
        public DiceFaceDirection localDirection;

        [Tooltip("Model prefab exported from Blender for this face.")]
        public GameObject modelPrefab;
    }
}
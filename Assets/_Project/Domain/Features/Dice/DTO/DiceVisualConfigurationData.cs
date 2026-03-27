using System;
using UnityEngine;

namespace _Project.Domain.Features.Dice.DTO
{
    [Serializable]
    public struct DiceVisualConfigurationData
    {
        [Header("Base Model")]
        [Tooltip("Optional base model prefab (usually a cube). If assigned, it is instantiated inside the visual root.")]
        public GameObject baseModelPrefab;

        [Tooltip("Optional mesh override for the base model MeshFilter.")]
        public Mesh baseMesh;

        [Tooltip("Optional shared material override used by the base model.")]
        public Material diceMaterial;

        [Tooltip("If enabled, the dice material override is also applied to spawned face models. Keep disabled for TMP/text-based face prefabs.")]
        public bool applyDiceMaterialToFaceModels;

        [Header("Face Models")]
        [Tooltip("Face model prefabs mapped by local direction.")]
        public DiceFaceVisualModelData[] faceModels;
    }
}


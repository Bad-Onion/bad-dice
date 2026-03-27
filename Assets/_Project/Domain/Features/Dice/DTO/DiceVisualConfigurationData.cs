using System;
using UnityEngine.Serialization;
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

        [Tooltip("Optional shared material override used as the default material for the base body.")]
        [FormerlySerializedAs("diceMaterial")]
        public Material baseMaterial;


        [Tooltip("Optional default material used by value slots (for example, Value_Default in the base model).")]
        public Material defaultFaceValueMaterial;

        [Tooltip("If enabled, the base material fallback is also applied to spawned face models when they have no explicit face material.")]
        [FormerlySerializedAs("applyDiceMaterialToFaceModels")]
        public bool applyBaseMaterialToFaceModels;

        [Header("Face Models")]
        [Tooltip("Face model prefabs mapped by local direction.")]
        public DiceFaceVisualModelData[] faceModels;
    }
}


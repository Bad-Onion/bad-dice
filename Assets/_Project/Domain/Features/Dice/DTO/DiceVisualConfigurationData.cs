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

        [Tooltip("Optional shared material override used as the default material for the base body.")]
        public Material baseMaterial;

        [Header("Face Materials")]
        [Tooltip("Material applied to each face. Maps local direction to material.")]
        public DiceFaceMaterialData[] faceMaterials;
    }
}


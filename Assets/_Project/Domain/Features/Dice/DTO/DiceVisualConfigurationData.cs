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

        [Header("Shader & Base Layer")]
        [Tooltip("The master material using the custom Shader Graph (e.g., Mat_DiceMaster).")]
        public Material shaderMaterial;

        [Tooltip("The base texture applied to all faces as the absolute background.")]
        public Texture2D baseTexture;

        [Header("Face Layers")]
        [Tooltip("Per-face texture definitions for direction and value.")]
        public DiceFaceTextureData[] faceTextures;
    }
}


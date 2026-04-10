using System;
using UnityEngine;

namespace _Project.Domain.Features.Dice.DTO
{
    /// <summary>
    /// Encapsulates all visual configuration data for a die.
    /// </summary>
    [Serializable]
    public struct DiceVisualConfigurationData
    {
        [Header("Base Model")]
        [Tooltip("Optional base die model. If assigned, it is instantiated inside the visual root. If not, the fallback is a basic cube model.")]
        public GameObject baseModel;

        [Tooltip("Optional mesh override for the base model MeshFilter. Useful for custom die shapes without needing a full model.")]
        public Mesh baseMesh;

        [Header("Shader & Base Layer")]
        [Tooltip("The master material using the custom Shader Graph (e.g., Mat_DiceMaster), this shader graph should be designed to support the layered texturing approach for the die faces.")]
        public Material shaderMaterial;

        [Tooltip("The base texture applied to the entire die model including all faces as the absolute background.")]
        public Texture2D baseTexture;

        [Header("Face Layers")]
        [Tooltip("Per-face texture definitions for direction and value.")]
        public DiceFaceTextureData[] faceTextures;
    }
}


using _Project.Domain.Features.Dice.DTO;
using UnityEngine;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Manages material assignment for the dice body model.
    /// Handles base material and per-face materials with proper fallback logic.
    /// Applies shader properties to ensure correct rendering of textures visibility on the dice faces.
    /// </summary>
    public interface IDiceMaterialManager
    {
        /// <summary>
        /// Instantiates the base shader material per face and injects the corresponding textures.
        /// </summary>
        void ApplyMaterials(
            Material shaderMaterial,
            Texture2D baseTexture,
            DiceFaceTextureData[] faceTextures);
    }
}


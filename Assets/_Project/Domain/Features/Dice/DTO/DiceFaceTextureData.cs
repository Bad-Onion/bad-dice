using System;
using _Project.Domain.Features.Dice.Enums;
using UnityEngine;

namespace _Project.Domain.Features.Dice.DTO
{
    /// <summary>
    /// Maps a die face direction to its specific textures.
    /// </summary>
    [Serializable]
    public struct DiceFaceTextureData
    {
        [Tooltip("The local direction of this face.")]
        public DiceFaceDirection localDirection;

        [Tooltip("Texture for the face background. Transparent areas fall back to the Base Texture.")]
        public Texture2D faceTexture;

        [Tooltip("Texture for the face value/icon. Transparent areas fall back to the Face Texture.")]
        public Texture2D valueTexture;
    }
}
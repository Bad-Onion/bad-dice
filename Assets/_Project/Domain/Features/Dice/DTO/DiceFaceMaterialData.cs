using System;
using _Project.Domain.Features.Dice.Enums;
using UnityEngine;

namespace _Project.Domain.Features.Dice.DTO
{
    /// <summary>
    /// Maps a dice face direction to its material.
    /// </summary>
    [Serializable]
    public struct DiceFaceMaterialData
    {
        [Tooltip("The local direction of this face.")]
        public DiceFaceDirection localDirection;

        [Tooltip("Material to apply to this face. If null, falls back to the base material.")]
        public Material faceMaterial;
    }
}
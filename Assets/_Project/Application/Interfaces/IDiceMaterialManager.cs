using _Project.Domain.Features.Dice.DTO;
using UnityEngine;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Manages material assignment for the dice body model.
    /// Handles base material and per-face materials with proper fallback logic.
    /// </summary>
    public interface IDiceMaterialManager
    {
        /// <summary>
        /// Applies base material and per-face materials to the dice body based on face directions and material slots.
        /// </summary>
        void ApplyMaterials(
            Material baseMaterial,
            DiceFaceMaterialData[] faceMaterials);
    }
}


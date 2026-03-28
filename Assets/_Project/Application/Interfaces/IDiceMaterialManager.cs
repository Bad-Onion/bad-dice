using _Project.Domain.Features.Dice.DTO;
using _Project.Domain.Features.Dice.Enums;
using UnityEngine;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Manages material assignment for both the base model and face models.
    /// </summary>
    public interface IDiceMaterialManager
    {
        /// <summary>
        /// Applies materials to the base model's MeshRenderer based on face directions and slots.
        /// </summary>
        void ApplyBaseMaterials(
            Material baseMaterial,
            Material defaultFaceValueMaterial,
            DiceFaceVisualModelData[] faceModels);

        /// <summary>
        /// Applies materials to a spawned face model.
        /// </summary>
        void ApplyFaceModelMaterials(
            GameObject spawnedFaceModel,
            DiceFaceDirection faceDirection,
            DiceFaceVisualModelData faceModel,
            Material baseMaterial,
            Material defaultFaceValueMaterial,
            bool applyBaseMaterialToFaceModels);
    }
}


using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.DTO;
using _Project.Domain.Features.Dice.Enums;
using UnityEngine;
using System.Collections.Generic;

namespace _Project.Infrastructure.Features.DiceSession.VisualServices
{
    /// <summary>
    /// Manages material assignment for dice models.
    /// Handles both base model materials and per-face materials with proper fallback logic.
    /// Follows Single Responsibility Principle by focusing only on material management.
    /// </summary>
    public class DiceMaterialManager : IDiceMaterialManager
    {
        private const string BodyBaseSlotName = "Body_Base";
        private const string ValueDefaultSlotName = "Value_Default";

        private readonly IDiceBaseModelManager _baseModelManager;

        public DiceMaterialManager(IDiceBaseModelManager baseModelManager)
        {
            _baseModelManager = baseModelManager;
        }

        public void ApplyBaseMaterials(
            Material baseMaterial,
            Material defaultFaceValueMaterial,
            DiceFaceVisualModelData[] faceModels)
        {
            MeshRenderer meshRenderer = _baseModelManager.GetMeshRenderer();
            if (meshRenderer == null) return;

            Material resolvedValueDefaultMaterial = defaultFaceValueMaterial != null
                ? defaultFaceValueMaterial
                : baseMaterial;

            bool updatedAnySlot = false;
            updatedAnySlot |= TrySetMaterialBySlotName(meshRenderer, BodyBaseSlotName, baseMaterial);
            updatedAnySlot |= TrySetMaterialBySlotName(meshRenderer, ValueDefaultSlotName, resolvedValueDefaultMaterial);

            Dictionary<DiceFaceDirection, Material> faceMaterialsByDirection = BuildFaceModelMaterialByDirectionMap(faceModels);

            foreach (DiceFaceDirection direction in System.Enum.GetValues(typeof(DiceFaceDirection)))
            {
                string slotName = GetBodyFaceSlotName(direction);
                if (string.IsNullOrEmpty(slotName)) continue;

                Material resolvedFaceMaterial = faceMaterialsByDirection.TryGetValue(direction, out Material directionalMaterial) && directionalMaterial != null
                    ? directionalMaterial
                    : baseMaterial;
                updatedAnySlot |= TrySetMaterialBySlotName(meshRenderer, slotName, resolvedFaceMaterial);
            }

            if (!updatedAnySlot && baseMaterial != null)
            {
                meshRenderer.sharedMaterial = baseMaterial;
            }
        }

        public void ApplyFaceModelMaterials(
            GameObject spawnedFaceModel,
            DiceFaceDirection faceDirection,
            DiceFaceVisualModelData faceModel,
            Material baseMaterial,
            Material defaultFaceValueMaterial,
            bool applyBaseMaterialToFaceModels)
        {
            if (spawnedFaceModel == null) return;

            Material fallbackFaceModelMaterial = applyBaseMaterialToFaceModels ? baseMaterial : null;
            Material resolvedFaceModelMaterial = faceModel.faceModelMaterial != null
                ? faceModel.faceModelMaterial
                : fallbackFaceModelMaterial;

            Material resolvedFaceValueMaterial = faceModel.faceValueMaterial != null
                ? faceModel.faceValueMaterial
                : (defaultFaceValueMaterial != null ? defaultFaceValueMaterial : resolvedFaceModelMaterial);

            ApplyMaterialsToRenderers(spawnedFaceModel, resolvedFaceModelMaterial, resolvedFaceValueMaterial);
        }

        private static void ApplyMaterialsToRenderers(
            GameObject spawnedFaceModel,
            Material faceModelMaterial,
            Material faceValueMaterial)
        {
            MeshRenderer[] renderers = spawnedFaceModel.GetComponentsInChildren<MeshRenderer>(true);
            for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
            {
                Material[] sharedMaterials = renderers[rendererIndex].sharedMaterials;
                bool isValueRenderer = IsValueRenderer(renderers[rendererIndex]);

                if (isValueRenderer && TryFillAllSlotsWithMaterial(sharedMaterials, faceValueMaterial))
                {
                    renderers[rendererIndex].sharedMaterials = sharedMaterials;
                    continue;
                }

                bool updatedByNamedSlots = TryApplyMaterialsBySlotNames(sharedMaterials, faceModelMaterial, faceValueMaterial);
                bool updatedBySingleSlotFallback = !updatedByNamedSlots && TryApplySingleSlotFallback(sharedMaterials, faceModelMaterial, faceValueMaterial);

                if (updatedByNamedSlots || updatedBySingleSlotFallback)
                {
                    renderers[rendererIndex].sharedMaterials = sharedMaterials;
                }
            }
        }

        private static bool IsValueRenderer(MeshRenderer meshRenderer)
        {
            return meshRenderer != null && meshRenderer.transform.name
                .IndexOf("Value", System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool TryFillAllSlotsWithMaterial(Material[] sharedMaterials, Material material)
        {
            if (sharedMaterials == null || sharedMaterials.Length == 0 || material == null) return false;

            for (int index = 0; index < sharedMaterials.Length; index++)
            {
                sharedMaterials[index] = material;
            }

            return true;
        }

        private static bool TryApplyMaterialsBySlotNames(
            Material[] sharedMaterials,
            Material faceModelMaterial,
            Material faceValueMaterial)
        {
            if (sharedMaterials == null || sharedMaterials.Length == 0) return false;

            bool updatedRenderer = false;
            for (int materialIndex = 0; materialIndex < sharedMaterials.Length; materialIndex++)
            {
                string materialName = sharedMaterials[materialIndex] != null
                    ? sharedMaterials[materialIndex].name.Replace(" (Instance)", string.Empty)
                    : string.Empty;
                bool isValueSlot = materialName.IndexOf("Value", System.StringComparison.OrdinalIgnoreCase) >= 0;

                if (isValueSlot && faceValueMaterial != null)
                {
                    sharedMaterials[materialIndex] = faceValueMaterial;
                    updatedRenderer = true;
                    continue;
                }

                if (!isValueSlot && faceModelMaterial != null)
                {
                    sharedMaterials[materialIndex] = faceModelMaterial;
                    updatedRenderer = true;
                }
            }

            return updatedRenderer;
        }

        private static bool TryApplySingleSlotFallback(
            Material[] sharedMaterials,
            Material faceModelMaterial,
            Material faceValueMaterial)
        {
            if (sharedMaterials == null || sharedMaterials.Length != 1) return false;

            if (faceValueMaterial != null)
            {
                sharedMaterials[0] = faceValueMaterial;
                return true;
            }

            if (faceModelMaterial != null)
            {
                sharedMaterials[0] = faceModelMaterial;
                return true;
            }

            return false;
        }

        private static string GetBodyFaceSlotName(DiceFaceDirection direction)
        {
            return direction switch
            {
                DiceFaceDirection.Up => "Body_Face_Up",
                DiceFaceDirection.Down => "Body_Face_Down",
                DiceFaceDirection.Forward => "Body_Face_Forward",
                DiceFaceDirection.Back => "Body_Face_Back",
                DiceFaceDirection.Right => "Body_Face_Right",
                DiceFaceDirection.Left => "Body_Face_Left",
                _ => string.Empty
            };
        }

        private static Dictionary<DiceFaceDirection, Material> BuildFaceModelMaterialByDirectionMap(DiceFaceVisualModelData[] faceModels)
        {
            var materialsByDirection = new Dictionary<DiceFaceDirection, Material>();
            if (faceModels == null) return materialsByDirection;

            for (int index = 0; index < faceModels.Length; index++)
            {
                DiceFaceVisualModelData faceModel = faceModels[index];
                if (faceModel.faceModelMaterial == null) continue;

                materialsByDirection[faceModel.localDirection] = faceModel.faceModelMaterial;
            }

            return materialsByDirection;
        }

        private static bool TrySetMaterialBySlotName(MeshRenderer meshRenderer, string slotName, Material material)
        {
            if (meshRenderer == null || string.IsNullOrWhiteSpace(slotName) || material == null) return false;

            Material[] sharedMaterials = meshRenderer.sharedMaterials;
            bool updatedSlot = false;

            for (int index = 0; index < sharedMaterials.Length; index++)
            {
                string currentMaterialName = sharedMaterials[index] != null
                    ? sharedMaterials[index].name.Replace(" (Instance)", string.Empty)
                    : string.Empty;

                if (!string.Equals(currentMaterialName, slotName, System.StringComparison.OrdinalIgnoreCase)) continue;

                sharedMaterials[index] = material;
                updatedSlot = true;
            }

            if (updatedSlot)
            {
                meshRenderer.sharedMaterials = sharedMaterials;
            }

            return updatedSlot;
        }
    }
}


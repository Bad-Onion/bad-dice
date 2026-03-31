using System;
using System.Collections.Generic;
using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.DTO;
using _Project.Domain.Features.Dice.Enums;
using UnityEngine;

namespace _Project.Presentation.Scripts.Features.DiceSession.VisualServices
{
    /// <summary>
    /// Manages material assignment for dice models.
    /// Handles base model materials and per-face materials with proper fallback logic.
    /// </summary>
    public class DiceMaterialManager : IDiceMaterialManager
    {
        // Material Slot Names
        private const string BodyBaseSlotName = "Body_Base";
        private const string DiceFaceSlotNameUp = "Body_Face_Up";
        private const string DiceFaceSlotNameDown = "Body_Face_Down";
        private const string DiceFaceSlotNameForward = "Body_Face_Forward";
        private const string DiceFaceSlotNameBack = "Body_Face_Back";
        private const string DiceFaceSlotNameRight = "Body_Face_Right";
        private const string DiceFaceSlotNameLeft = "Body_Face_Left";

        private readonly IDiceBaseModelManager _baseModelManager;

        public DiceMaterialManager(IDiceBaseModelManager baseModelManager)
        {
            _baseModelManager = baseModelManager;
        }

        public void ApplyMaterials(
            Material baseMaterial,
            DiceFaceMaterialData[] faceMaterials)
        {
            MeshRenderer meshRenderer = _baseModelManager.GetMeshRenderer();
            if (meshRenderer == null) return;

            bool updatedAnySlot = false;
            updatedAnySlot |= TrySetMaterialBySlotName(meshRenderer, BodyBaseSlotName, baseMaterial);

            Dictionary<DiceFaceDirection, Material> faceMaterialsByDirection = BuildFaceMaterialByDirectionMap(faceMaterials);

            foreach (DiceFaceDirection direction in Enum.GetValues(typeof(DiceFaceDirection)))
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

        private static string GetBodyFaceSlotName(DiceFaceDirection direction)
        {
            return direction switch
            {
                DiceFaceDirection.Up => DiceFaceSlotNameUp,
                DiceFaceDirection.Down => DiceFaceSlotNameDown,
                DiceFaceDirection.Forward => DiceFaceSlotNameForward,
                DiceFaceDirection.Back => DiceFaceSlotNameBack,
                DiceFaceDirection.Right => DiceFaceSlotNameRight,
                DiceFaceDirection.Left => DiceFaceSlotNameLeft,
                _ => string.Empty
            };
        }

        private static Dictionary<DiceFaceDirection, Material> BuildFaceMaterialByDirectionMap(DiceFaceMaterialData[] faceMaterials)
        {
            var materialsByDirection = new Dictionary<DiceFaceDirection, Material>();
            if (faceMaterials == null) return materialsByDirection;

            foreach (DiceFaceMaterialData faceMaterial in faceMaterials)
            {
                if (faceMaterial.faceMaterial == null) continue;

                materialsByDirection[faceMaterial.localDirection] = faceMaterial.faceMaterial;
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
                if (!DoesMaterialSlotNameMatch(sharedMaterials[index], slotName)) continue;

                sharedMaterials[index] = material;
                updatedSlot = true;
            }

            if (updatedSlot)
            {
                meshRenderer.sharedMaterials = sharedMaterials;
            }

            return updatedSlot;
        }

        private static bool DoesMaterialSlotNameMatch(Material material, string slotName)
        {
            string currentMaterialName = RemovePrefabInstanceSuffix(material);
            return string.Equals(currentMaterialName, slotName, StringComparison.OrdinalIgnoreCase);
        }

        private static string RemovePrefabInstanceSuffix(Material material)
        {
            return material != null
                ? material.name.Replace(" (Instance)", string.Empty)
                : string.Empty;
        }
    }
}


using System;
using System.Collections.Generic;
using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.DTO;
using _Project.Domain.Features.Dice.Enums;
using UnityEngine;

namespace _Project.Presentation.Scripts.Features.DicePrefab.VisualServices
{
    /// <summary>
    /// Manages material assignment for dice models.
    /// Handles base model materials and per-face materials with proper fallback logic.
    /// </summary>
    public class DiceMaterialManager : IDiceMaterialManager
    {
        // Material Slot Names
        private const string DiceFaceSlotNameUp = "Body_Face_Up";
        private const string DiceFaceSlotNameDown = "Body_Face_Down";
        private const string DiceFaceSlotNameForward = "Body_Face_Forward";
        private const string DiceFaceSlotNameBack = "Body_Face_Back";
        private const string DiceFaceSlotNameRight = "Body_Face_Right";
        private const string DiceFaceSlotNameLeft = "Body_Face_Left";

        // Shader Property IDs
        private static readonly int BaseTexId = Shader.PropertyToID("_BaseTexture");
        private static readonly int FaceTexId = Shader.PropertyToID("_FaceTexture");
        private static readonly int ValueTexId = Shader.PropertyToID("_ValueTexture");

        private readonly IDiceBaseModelManager _baseModelManager;

        private static Texture2D _transparentFallbackTexture;
        private static Texture2D TransparentFallbackTexture
        {
            get
            {
                if (_transparentFallbackTexture == null)
                {
                    _transparentFallbackTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                    _transparentFallbackTexture.SetPixel(0, 0, Color.clear);
                    _transparentFallbackTexture.Apply();
                }
                return _transparentFallbackTexture;
            }
        }

        public DiceMaterialManager(IDiceBaseModelManager baseModelManager)
        {
            _baseModelManager = baseModelManager;
        }

        public void ApplyMaterials(
            Material shaderMaterial,
            Texture2D baseTexture,
            DiceFaceTextureData[] faceTextures)
        {
            MeshRenderer meshRenderer = _baseModelManager.GetMeshRenderer();
            if (meshRenderer == null || shaderMaterial == null) return;

            Dictionary<DiceFaceDirection, DiceFaceTextureData> faceDataMap = BuildFaceDataMap(faceTextures);
            Material[] currentMaterials = meshRenderer.sharedMaterials;
            bool materialsUpdated = false;

            for (int i = 0; i < currentMaterials.Length; i++)
            {
                DiceFaceDirection? mappedDirection = GetDirectionFromSlotName(currentMaterials[i]);

                if (mappedDirection.HasValue)
                {
                    Material faceMaterialInstance = new Material(shaderMaterial);

                    Texture2D appliedBaseTex = baseTexture != null ? baseTexture : TransparentFallbackTexture;
                    faceMaterialInstance.SetTexture(BaseTexId, appliedBaseTex);

                    Texture2D appliedFaceTex = TransparentFallbackTexture;
                    Texture2D appliedValueTex = TransparentFallbackTexture;

                    if (faceDataMap.TryGetValue(mappedDirection.Value, out DiceFaceTextureData specificFaceData))
                    {
                        if (specificFaceData.faceTexture != null) appliedFaceTex = specificFaceData.faceTexture;
                        if (specificFaceData.valueTexture != null) appliedValueTex = specificFaceData.valueTexture;
                    }

                    faceMaterialInstance.SetTexture(FaceTexId, appliedFaceTex);
                    faceMaterialInstance.SetTexture(ValueTexId, appliedValueTex);

                    currentMaterials[i] = faceMaterialInstance;
                    materialsUpdated = true;
                }
            }

            if (materialsUpdated)
            {
                meshRenderer.sharedMaterials = currentMaterials;
            }
        }

        private static Dictionary<DiceFaceDirection, DiceFaceTextureData> BuildFaceDataMap(DiceFaceTextureData[] faceTextures)
        {
            var map = new Dictionary<DiceFaceDirection, DiceFaceTextureData>();
            if (faceTextures == null) return map;

            foreach (var faceTexture in faceTextures)
            {
                map[faceTexture.localDirection] = faceTexture;
            }

            return map;
        }

        private static DiceFaceDirection? GetDirectionFromSlotName(Material material)
        {
            string slotName = RemovePrefabInstanceSuffix(material);

            if (string.Equals(slotName, DiceFaceSlotNameUp, StringComparison.OrdinalIgnoreCase)) return DiceFaceDirection.Up;
            if (string.Equals(slotName, DiceFaceSlotNameDown, StringComparison.OrdinalIgnoreCase)) return DiceFaceDirection.Down;
            if (string.Equals(slotName, DiceFaceSlotNameForward, StringComparison.OrdinalIgnoreCase)) return DiceFaceDirection.Forward;
            if (string.Equals(slotName, DiceFaceSlotNameBack, StringComparison.OrdinalIgnoreCase)) return DiceFaceDirection.Back;

            // Inverted mapping to correct Blender to Unity axis translation
            if (string.Equals(slotName, DiceFaceSlotNameRight, StringComparison.OrdinalIgnoreCase)) return DiceFaceDirection.Left;
            if (string.Equals(slotName, DiceFaceSlotNameLeft, StringComparison.OrdinalIgnoreCase)) return DiceFaceDirection.Right;

            return null;
        }

        private static string RemovePrefabInstanceSuffix(Material material)
        {
            return material != null ? material.name.Replace(" (Instance)", string.Empty) : string.Empty;
        }
    }
}


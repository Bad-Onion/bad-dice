using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.DTO;
using _Project.Domain.Features.Dice.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Infrastructure.Features.DiceSession.VisualServices
{
    /// <summary>
    /// Manages spawning and configuration of face value models for a dice.
    /// 
    /// Key design principles:
    /// 1. Each face model is placed at its corresponding anchor position
    /// 2. Prefab origin transforms are stripped and replaced with configured transforms
    /// 3. localPositionOffset and localRotationEuler apply directly (zero = anchor position/identity)
    /// 4. This ensures all prefabs (even identical ones like Five dice) are positioned correctly
    /// 5. Gameplay face values and visual model selection don't interfere with positioning
    /// 6. The Value_X GameObjects in the base model are hidden to prevent visual duplication
    /// 
    /// This ensures consistency across all dice definitions while allowing maximum flexibility
    /// for custom positioning requirements (e.g., TMP text needing offset from faces).
    /// 
    /// Follows Single Responsibility Principle by focusing only on face model management.
    /// </summary>
    public class DiceFaceModelManager : IDiceFaceModelManager
    {
        private readonly IDiceFaceAnchorResolver _anchorResolver;
        private readonly IDiceMaterialManager _materialManager;
        private readonly IDiceFaceValueTextController _textController;
        private readonly Material _baseMaterial;
        private readonly Material _defaultFaceValueMaterial;
        private readonly bool _applyBaseMaterialToFaceModels;

        private readonly List<GameObject> _spawnedFaceModels = new();

        public DiceFaceModelManager(
            IDiceFaceAnchorResolver anchorResolver,
            IDiceMaterialManager materialManager,
            IDiceFaceValueTextController textController,
            Material baseMaterial,
            Material defaultFaceValueMaterial,
            bool applyBaseMaterialToFaceModels)
        {
            _anchorResolver = anchorResolver;
            _materialManager = materialManager;
            _textController = textController;
            _baseMaterial = baseMaterial;
            _defaultFaceValueMaterial = defaultFaceValueMaterial;
            _applyBaseMaterialToFaceModels = applyBaseMaterialToFaceModels;
        }

        public void SetupFaceModels(
            DiceFaceVisualModelData[] faceModels,
            DiceFaceData[] gameplayFaces)
        {
            ClearSpawnedFaceModels();
            if (faceModels == null || faceModels.Length == 0) return;

            Dictionary<DiceFaceDirection, int> gameplayFaceValues = BuildFaceValueByDirectionMap(gameplayFaces);
            var spawnedDirections = new HashSet<DiceFaceDirection>();

            for (int index = 0; index < faceModels.Length; index++)
            {
                DiceFaceVisualModelData faceModel = faceModels[index];
                if (faceModel.faceValuePrefab == null) continue;
                if (!spawnedDirections.Add(faceModel.localDirection)) continue;

                Transform anchor = _anchorResolver.GetAnchorForDirection(faceModel.localDirection);
                if (anchor == null) continue;

                GameObject spawnedFaceModel = Object.Instantiate(faceModel.faceValuePrefab, anchor);

                spawnedFaceModel.transform.localPosition = faceModel.localPositionOffset;
                spawnedFaceModel.transform.localRotation = Quaternion.Euler(faceModel.localRotationEuler);
                spawnedFaceModel.transform.localScale = Vector3.one;
                
                _spawnedFaceModels.Add(spawnedFaceModel);

                ConfigureSpawnedFaceModel(
                    spawnedFaceModel,
                    faceModel,
                    gameplayFaceValues);
            }
        }

        public void Cleanup()
        {
            ClearSpawnedFaceModels();
        }

        private void ConfigureSpawnedFaceModel(
            GameObject spawnedFaceModel,
            DiceFaceVisualModelData faceModel,
            Dictionary<DiceFaceDirection, int> gameplayFaceValues)
        {
            // Apply materials
            _materialManager.ApplyFaceModelMaterials(
                spawnedFaceModel,
                faceModel.localDirection,
                faceModel,
                _baseMaterial,
                _defaultFaceValueMaterial,
                _applyBaseMaterialToFaceModels);

            // Determine text to display
            string resolvedText = ResolveDisplayText(faceModel, gameplayFaceValues);

            // Apply text
            Material resolvedFaceValueMaterial = faceModel.faceValueMaterial != null
                ? faceModel.faceValueMaterial
                : _defaultFaceValueMaterial;

            _textController.ApplyFaceTexts(spawnedFaceModel, resolvedText, resolvedFaceValueMaterial);
        }

        private string ResolveDisplayText(
            DiceFaceVisualModelData faceModel,
            Dictionary<DiceFaceDirection, int> gameplayFaceValues)
        {
            if (faceModel.useGameplayFaceValueAsText)
            {
                if (gameplayFaceValues.TryGetValue(faceModel.localDirection, out int gameplayFaceValue))
                {
                    return gameplayFaceValue.ToString();
                }
            }

            if (!string.IsNullOrWhiteSpace(faceModel.customFaceValueText))
            {
                return faceModel.customFaceValueText;
            }

            return null;
        }

        private static Dictionary<DiceFaceDirection, int> BuildFaceValueByDirectionMap(DiceFaceData[] gameplayFaces)
        {
            var valuesByDirection = new Dictionary<DiceFaceDirection, int>();
            if (gameplayFaces == null) return valuesByDirection;

            for (int index = 0; index < gameplayFaces.Length; index++)
            {
                valuesByDirection[gameplayFaces[index].localDirection] = gameplayFaces[index].value;
            }

            return valuesByDirection;
        }


        private void ClearSpawnedFaceModels()
        {
            foreach (GameObject spawnedFaceModel in _spawnedFaceModels)
            {
                if (spawnedFaceModel != null)
                {
                    Object.Destroy(spawnedFaceModel);
                }
            }

            _spawnedFaceModels.Clear();
        }
    }
}


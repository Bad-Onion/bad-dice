using System.Collections.Generic;
using _Project.Domain.Features.Dice.DTO;
using _Project.Domain.Features.Dice.Enums;
using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;
using TMPro;
using UnityEngine;

namespace _Project.Presentation.Scripts.Features.DiceSession.VisualControllers
{
    public class DiceVisualRuntimeConfigurator : MonoBehaviour
    {
        private const string BodyBaseSlotName = "Body_Base";
        private const string ValueDefaultSlotName = "Value_Default";

        [Header("Base Visual Targets")]
        [Tooltip("Parent transform used to spawn the optional base model prefab.")]
        [SerializeField] private Transform baseModelRoot;

        [Tooltip("Fallback MeshFilter used when no base model prefab is assigned.")]
        [SerializeField] private MeshFilter fallbackBaseMeshFilter;

        [Tooltip("Fallback MeshRenderer used when no base model prefab is assigned.")]
        [SerializeField] private MeshRenderer fallbackBaseMeshRenderer;

        [Header("Face Visual Targets")]
        [Tooltip("Bindings between die local directions and face anchors.")]
        [SerializeField] private FaceAnchorBinding[] faceAnchors;

        private readonly List<GameObject> _spawnedFaceModels = new();
        private GameObject _spawnedBaseModel;

        public void ApplyFromDefinition(DiceDefinition diceDefinition)
        {
            if (diceDefinition == null) return;

            ApplyVisualConfiguration(diceDefinition.visualConfiguration, diceDefinition.faces);
        }

        private void ApplyVisualConfiguration(DiceVisualConfigurationData visualConfiguration, DiceFaceData[] gameplayFaces)
        {
            SetupBaseModel(visualConfiguration.baseModelPrefab);
            UpdateFallbackBaseVisibility();
            ApplyBaseMesh(visualConfiguration.baseMesh);
            SyncAnchorsFromEmbeddedValueGuides(gameplayFaces);
            DisableEmbeddedValueGuideObjects();

            var faceMaterialsByDirection = BuildFaceModelMaterialByDirectionMap(visualConfiguration.faceModels);
            ApplyBaseMaterials(
                visualConfiguration.baseMaterial,
                faceMaterialsByDirection,
                visualConfiguration.defaultFaceValueMaterial);

            SetupFaceModels(
                visualConfiguration.faceModels,
                gameplayFaces,
                visualConfiguration.baseMaterial,
                visualConfiguration.defaultFaceValueMaterial,
                visualConfiguration.applyBaseMaterialToFaceModels,
                faceMaterialsByDirection);
        }

        private void SetupBaseModel(GameObject baseModelPrefab)
        {
            if (_spawnedBaseModel != null)
            {
                Destroy(_spawnedBaseModel);
                _spawnedBaseModel = null;
            }

            if (baseModelRoot == null || baseModelPrefab == null) return;

            _spawnedBaseModel = Instantiate(baseModelPrefab, baseModelRoot);
            _spawnedBaseModel.transform.localPosition = Vector3.zero;
            _spawnedBaseModel.transform.localRotation = Quaternion.identity;
            _spawnedBaseModel.transform.localScale = Vector3.one;
        }

        private void UpdateFallbackBaseVisibility()
        {
            if (fallbackBaseMeshRenderer == null)
            {
                return;
            }

            bool shouldShowFallback = _spawnedBaseModel == null;
            fallbackBaseMeshRenderer.enabled = shouldShowFallback;
        }

        private void ApplyBaseMesh(Mesh baseMesh)
        {
            if (baseMesh == null) return;

            MeshFilter targetMeshFilter = ResolveBaseMeshFilter();
            if (targetMeshFilter == null) return;

            targetMeshFilter.sharedMesh = baseMesh;
        }

        private void ApplyBaseMaterials(
            Material baseMaterial,
            Dictionary<DiceFaceDirection, Material> faceMaterialsByDirection,
            Material defaultFaceValueMaterial)
        {
            MeshRenderer targetRenderer = ResolveBaseMeshRenderer();
            if (targetRenderer == null) return;

            Material resolvedValueDefaultMaterial = defaultFaceValueMaterial != null
                ? defaultFaceValueMaterial
                : baseMaterial;

            bool updatedAnySlot = false;
            updatedAnySlot |= TrySetMaterialBySlotName(targetRenderer, BodyBaseSlotName, baseMaterial);
            updatedAnySlot |= TrySetMaterialBySlotName(targetRenderer, ValueDefaultSlotName, resolvedValueDefaultMaterial);

            foreach (DiceFaceDirection direction in System.Enum.GetValues(typeof(DiceFaceDirection)))
            {
                string slotName = GetBodyFaceSlotName(direction);
                if (string.IsNullOrEmpty(slotName)) continue;

                Material resolvedFaceMaterial = faceMaterialsByDirection.TryGetValue(direction, out Material directionalMaterial) && directionalMaterial != null
                    ? directionalMaterial
                    : baseMaterial;
                updatedAnySlot |= TrySetMaterialBySlotName(targetRenderer, slotName, resolvedFaceMaterial);
            }

            if (!updatedAnySlot && baseMaterial != null)
            {
                targetRenderer.sharedMaterial = baseMaterial;
            }
        }

        private void SetupFaceModels(
            DiceFaceVisualModelData[] faceModels,
            DiceFaceData[] gameplayFaces,
            Material baseMaterial,
            Material defaultFaceValueMaterial,
            bool applyBaseMaterialToFaceModels,
            Dictionary<DiceFaceDirection, Material> bodyFaceMaterials)
        {
            ClearSpawnedFaceModels();
            if (faceModels == null || faceModels.Length == 0) return;

            Dictionary<DiceFaceDirection, Transform> anchorsByDirection = GetAnchorsByDirection();
            Dictionary<DiceFaceDirection, int> gameplayFaceValues = BuildFaceValueByDirectionMap(gameplayFaces);
            var spawnedDirections = new HashSet<DiceFaceDirection>();

            for (int index = 0; index < faceModels.Length; index++)
            {
                DiceFaceVisualModelData faceModel = faceModels[index];
                if (faceModel.faceValuePrefab == null) continue;
                if (!spawnedDirections.Add(faceModel.localDirection)) continue;
                if (!anchorsByDirection.TryGetValue(faceModel.localDirection, out Transform anchor) ||
                    anchor == null) continue;

                GameObject spawnedFaceModel = Instantiate(faceModel.faceValuePrefab, anchor);
                spawnedFaceModel.transform.localPosition = Vector3.zero;
                spawnedFaceModel.transform.localRotation = Quaternion.identity;
                spawnedFaceModel.transform.localScale = Vector3.one;
                _spawnedFaceModels.Add(spawnedFaceModel);

                ConfigureSpawnedFaceModel(
                    spawnedFaceModel,
                    faceModel,
                    gameplayFaceValues,
                    bodyFaceMaterials,
                    baseMaterial,
                    defaultFaceValueMaterial,
                    applyBaseMaterialToFaceModels);
            }
        }

        private static void ConfigureSpawnedFaceModel(
            GameObject spawnedFaceModel,
            DiceFaceVisualModelData faceModel,
            Dictionary<DiceFaceDirection, int> gameplayFaceValues,
            Dictionary<DiceFaceDirection, Material> bodyFaceMaterials,
            Material baseMaterial,
            Material defaultFaceValueMaterial,
            bool applyBaseMaterialToFaceModels)
        {
            Material fallbackFaceModelMaterial = ResolveFaceBodyMaterial(
                faceModel.localDirection,
                bodyFaceMaterials,
                baseMaterial,
                applyBaseMaterialToFaceModels);

            Material resolvedFaceModelMaterial = faceModel.faceModelMaterial != null
                ? faceModel.faceModelMaterial
                : fallbackFaceModelMaterial;
            Material resolvedFaceValueMaterial = ResolveFaceValueMaterial(
                faceModel.faceValueMaterial,
                defaultFaceValueMaterial,
                resolvedFaceModelMaterial);

            ApplyFaceModelMaterials(spawnedFaceModel, resolvedFaceModelMaterial, resolvedFaceValueMaterial);

            gameplayFaceValues.TryGetValue(faceModel.localDirection, out int gameplayFaceValue);
            int? faceValue = gameplayFaceValues.ContainsKey(faceModel.localDirection)
                ? gameplayFaceValue
                : null;
            ApplyFaceTexts(spawnedFaceModel, faceModel, faceValue, resolvedFaceValueMaterial);
        }

        private static Material ResolveFaceValueMaterial(
            Material explicitFaceValueMaterial,
            Material defaultFaceValueMaterial,
            Material resolvedFaceModelMaterial)
        {
            if (explicitFaceValueMaterial != null)
            {
                return explicitFaceValueMaterial;
            }

            if (defaultFaceValueMaterial != null)
            {
                return defaultFaceValueMaterial;
            }

            return resolvedFaceModelMaterial;
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

        private static Dictionary<int, DiceFaceDirection> BuildDirectionByFaceValueMap(DiceFaceData[] gameplayFaces)
        {
            var directionByValue = new Dictionary<int, DiceFaceDirection>();
            if (gameplayFaces == null) return directionByValue;

            for (int index = 0; index < gameplayFaces.Length; index++)
            {
                int faceValue = gameplayFaces[index].value;
                if (faceValue < 1 || faceValue > 6) continue;

                directionByValue[faceValue] = gameplayFaces[index].localDirection;
            }

            return directionByValue;
        }

        private void SyncAnchorsFromEmbeddedValueGuides(DiceFaceData[] gameplayFaces)
        {
            if (_spawnedBaseModel == null) return;

            Dictionary<DiceFaceDirection, Transform> anchorsByDirection = GetAnchorsByDirection();
            if (anchorsByDirection.Count == 0) return;

            Dictionary<int, DiceFaceDirection> directionByValue = BuildDirectionByFaceValueMap(gameplayFaces);
            if (directionByValue.Count == 0) return;

            for (int faceValue = 1; faceValue <= 6; faceValue++)
            {
                if (!directionByValue.TryGetValue(faceValue, out DiceFaceDirection direction)) continue;
                if (!anchorsByDirection.TryGetValue(direction, out Transform anchor) || anchor == null) continue;

                Transform guide = FindTransformByName(_spawnedBaseModel.transform, $"Value_{faceValue}");
                if (guide == null) continue;

                Transform anchorParent = anchor.parent;
                if (anchorParent == null)
                {
                    anchor.position = guide.position;
                    anchor.rotation = guide.rotation;
                    continue;
                }

                anchor.localPosition = anchorParent.InverseTransformPoint(guide.position);
                anchor.localRotation = Quaternion.Inverse(anchorParent.rotation) * guide.rotation;
            }
        }

        private void DisableEmbeddedValueGuideObjects()
        {
            if (_spawnedBaseModel == null) return;

            Transform[] transforms = _spawnedBaseModel.GetComponentsInChildren<Transform>(true);
            for (int index = 0; index < transforms.Length; index++)
            {
                Transform current = transforms[index];
                if (current == _spawnedBaseModel.transform) continue;
                if (!IsEmbeddedValueObjectName(current.name)) continue;

                current.gameObject.SetActive(false);
            }
        }

        private static bool IsEmbeddedValueObjectName(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName)) return false;
            if (!objectName.StartsWith("Value_", System.StringComparison.OrdinalIgnoreCase)) return false;

            string suffix = objectName.Substring("Value_".Length);
            return int.TryParse(suffix, out int parsedValue) && parsedValue >= 1 && parsedValue <= 6;
        }

        private static Transform FindTransformByName(Transform root, string targetName)
        {
            if (root == null || string.IsNullOrWhiteSpace(targetName)) return null;

            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            for (int index = 0; index < transforms.Length; index++)
            {
                if (string.Equals(transforms[index].name, targetName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return transforms[index];
                }
            }

            return null;
        }

        private static Material ResolveFaceBodyMaterial(
            DiceFaceDirection faceDirection,
            Dictionary<DiceFaceDirection, Material> bodyFaceMaterials,
            Material baseMaterial,
            bool allowBaseMaterialFallback)
        {
            if (bodyFaceMaterials != null && bodyFaceMaterials.TryGetValue(faceDirection, out Material faceMaterial) && faceMaterial != null)
            {
                return faceMaterial;
            }

            return allowBaseMaterialFallback ? baseMaterial : null;
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

        private static void ApplyFaceModelMaterials(GameObject spawnedFaceModel, Material faceModelMaterial, Material faceValueMaterial)
        {
            if (spawnedFaceModel == null) return;

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

        private static void ApplyFaceTexts(
            GameObject spawnedFaceModel,
            DiceFaceVisualModelData faceModel,
            int? gameplayFaceValue,
            Material faceValueMaterial)
        {
            TMP_Text[] texts = spawnedFaceModel.GetComponentsInChildren<TMP_Text>(true);
            if (texts.Length == 0) return;

            string resolvedText = null;
            if (faceModel.useGameplayFaceValueAsText && gameplayFaceValue.HasValue)
            {
                resolvedText = gameplayFaceValue.Value.ToString();
            }
            else if (!string.IsNullOrWhiteSpace(faceModel.customFaceValueText))
            {
                resolvedText = faceModel.customFaceValueText;
            }

            for (int index = 0; index < texts.Length; index++)
            {
                if (resolvedText != null)
                {
                    texts[index].text = resolvedText;
                }

                if (faceValueMaterial != null)
                {
                    texts[index].fontSharedMaterial = faceValueMaterial;
                }
            }
        }

        private void ClearSpawnedFaceModels()
        {
            foreach (GameObject spawnedFaceModel in _spawnedFaceModels)
            {
                if (spawnedFaceModel != null)
                {
                    Destroy(spawnedFaceModel);
                }
            }

            _spawnedFaceModels.Clear();
        }

        private Dictionary<DiceFaceDirection, Transform> GetAnchorsByDirection()
        {
            var anchorMap = new Dictionary<DiceFaceDirection, Transform>();
            if (faceAnchors == null) return anchorMap;

            foreach (FaceAnchorBinding binding in faceAnchors)
            {
                if (binding.anchor == null) continue;

                anchorMap[binding.localDirection] = binding.anchor;
            }

            return anchorMap;
        }

        private MeshFilter ResolveBaseMeshFilter()
        {
            return ResolveBaseComponent(fallbackBaseMeshFilter);
        }

        private MeshRenderer ResolveBaseMeshRenderer()
        {
            return ResolveBaseComponent(fallbackBaseMeshRenderer);
        }

        private T ResolveBaseComponent<T>(T fallbackComponent) where T : Component
        {
            if (_spawnedBaseModel == null)
            {
                return fallbackComponent;
            }

            T component = _spawnedBaseModel.GetComponentInChildren<T>(true);
            return component != null ? component : fallbackComponent;
        }
    }
}
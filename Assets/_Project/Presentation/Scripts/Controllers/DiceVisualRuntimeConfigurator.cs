using System.Collections.Generic;
using _Project.Domain.Entities.DTO;
using _Project.Domain.Enums;
using _Project.Domain.ScriptableObjects.DiceDefinitions;
using UnityEngine;

namespace _Project.Presentation.Scripts.Controllers
{
    // TODO: Check the expensive method invocations
    public class DiceVisualRuntimeConfigurator : MonoBehaviour
    {
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

            ApplyVisualConfiguration(diceDefinition.visualConfiguration);
        }

        private void ApplyVisualConfiguration(DiceVisualConfigurationData visualConfiguration)
        {
            SetupBaseModel(visualConfiguration.baseModelPrefab);
            ApplyBaseMesh(visualConfiguration.baseMesh);
            ApplyMaterial(visualConfiguration.diceMaterial);
            SetupFaceModels(
                visualConfiguration.faceModels,
                visualConfiguration.diceMaterial,
                visualConfiguration.applyDiceMaterialToFaceModels);
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

        private void ApplyBaseMesh(Mesh baseMesh)
        {
            if (baseMesh == null) return;

            MeshFilter targetMeshFilter = ResolveBaseMeshFilter();
            if (targetMeshFilter == null) return;

            targetMeshFilter.sharedMesh = baseMesh;
        }

        private void ApplyMaterial(Material diceMaterial)
        {
            if (diceMaterial == null) return;

            MeshRenderer targetRenderer = ResolveBaseMeshRenderer();
            if (targetRenderer != null)
            {
                targetRenderer.sharedMaterial = diceMaterial;
            }

            foreach (GameObject faceModel in _spawnedFaceModels)
            {
                MeshRenderer[] renderers = faceModel.GetComponentsInChildren<MeshRenderer>(true);

                foreach (MeshRenderer meshRenderer in renderers)
                {
                    meshRenderer.sharedMaterial = diceMaterial;
                }
            }
        }

        private void SetupFaceModels(DiceFaceVisualModelData[] faceModels, Material overrideMaterial,
            bool applyMaterialOverrideToFaceModels)
        {
            ClearSpawnedFaceModels();
            if (faceModels == null || faceModels.Length == 0) return;

            Dictionary<DiceFaceDirection, Transform> anchorsByDirection = GetAnchorsByDirection();

            for (int index = 0; index < faceModels.Length; index++)
            {
                DiceFaceVisualModelData faceModel = faceModels[index];
                if (faceModel.modelPrefab == null) continue;
                if (!anchorsByDirection.TryGetValue(faceModel.localDirection, out Transform anchor) ||
                    anchor == null) continue;

                GameObject spawnedFaceModel = Instantiate(faceModel.modelPrefab, anchor);
                spawnedFaceModel.transform.localPosition = Vector3.zero;
                spawnedFaceModel.transform.localRotation = Quaternion.identity;
                spawnedFaceModel.transform.localScale = Vector3.one;
                _spawnedFaceModels.Add(spawnedFaceModel);
            }

            if (applyMaterialOverrideToFaceModels && overrideMaterial != null)
            {
                ApplyMaterial(overrideMaterial);
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

        // TODO: Reuse duplicated code
        private MeshFilter ResolveBaseMeshFilter()
        {
            if (_spawnedBaseModel == null)
            {
                return fallbackBaseMeshFilter;
            }

            MeshFilter meshFilter = _spawnedBaseModel.GetComponentInChildren<MeshFilter>(true);
            if (meshFilter != null) return meshFilter;

            return fallbackBaseMeshFilter;
        }

        // TODO: Reuse duplicated code
        private MeshRenderer ResolveBaseMeshRenderer()
        {
            if (_spawnedBaseModel == null)
            {
                return fallbackBaseMeshRenderer;
            }

            MeshRenderer meshRenderer = _spawnedBaseModel.GetComponentInChildren<MeshRenderer>(true);
            if (meshRenderer != null) return meshRenderer;

            return fallbackBaseMeshRenderer;
        }
    }
}
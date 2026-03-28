using _Project.Application.Interfaces;
using UnityEngine;

namespace _Project.Infrastructure.Features.DiceSession.VisualServices
{
    /// <summary>
    /// Manages the base model (body) of a dice, handling mesh and visibility.
    /// Follows Single Responsibility Principle by focusing only on base model concerns.
    /// </summary>
    public class DiceBaseModelManager : IDiceBaseModelManager
    {
        private readonly Transform _baseModelRoot;
        private readonly MeshFilter _fallbackMeshFilter;
        private readonly MeshRenderer _fallbackMeshRenderer;

        private GameObject _spawnedBaseModel;

        public DiceBaseModelManager(
            Transform baseModelRoot,
            MeshFilter fallbackMeshFilter,
            MeshRenderer fallbackMeshRenderer)
        {
            _baseModelRoot = baseModelRoot;
            _fallbackMeshFilter = fallbackMeshFilter;
            _fallbackMeshRenderer = fallbackMeshRenderer;
        }

        public MeshFilter GetMeshFilter()
        {
            return ResolveBaseComponent(_fallbackMeshFilter);
        }

        public MeshRenderer GetMeshRenderer()
        {
            return ResolveBaseComponent(_fallbackMeshRenderer);
        }

        public void SetupBaseModel(GameObject baseModelPrefab)
        {
            if (_spawnedBaseModel != null)
            {
                Object.Destroy(_spawnedBaseModel);
                _spawnedBaseModel = null;
            }

            if (_baseModelRoot == null || baseModelPrefab == null) return;

            _spawnedBaseModel = Object.Instantiate(baseModelPrefab, _baseModelRoot);
            _spawnedBaseModel.transform.localPosition = Vector3.zero;
            _spawnedBaseModel.transform.localRotation = Quaternion.identity;
            _spawnedBaseModel.transform.localScale = Vector3.one;

            UpdateFallbackVisibility();
        }

        public void ApplyMesh(Mesh baseMesh)
        {
            if (baseMesh == null) return;

            MeshFilter meshFilter = GetMeshFilter();
            if (meshFilter == null) return;

            meshFilter.sharedMesh = baseMesh;
        }

        public void HideBaseModelFaceValueObjects()
        {
            // Hide Value_X GameObjects in the fallback model first
            HideValueObjectsInTransform(_fallbackMeshRenderer?.gameObject.transform);

            // Hide Value_X GameObjects in the spawned model
            if (_spawnedBaseModel != null)
            {
                HideValueObjectsInTransform(_spawnedBaseModel.transform);
            }
        }

        private static void HideValueObjectsInTransform(Transform root)
        {
            if (root == null) return;

            // Search all children recursively
            Transform[] allChildren = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                // Check if the transform name contains "Value" (case-insensitive)
                if (child.name.IndexOf("Value", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        public void Cleanup()
        {
            if (_spawnedBaseModel != null)
            {
                Object.Destroy(_spawnedBaseModel);
                _spawnedBaseModel = null;
            }

            UpdateFallbackVisibility();
        }

        private void UpdateFallbackVisibility()
        {
            if (_fallbackMeshRenderer == null) return;

            bool shouldShowFallback = _spawnedBaseModel == null;
            _fallbackMeshRenderer.enabled = shouldShowFallback;
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


using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.DTO;
using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;
using _Project.Infrastructure.Features.DiceSession.VisualServices;
using UnityEngine;

namespace _Project.Presentation.Scripts.Features.DiceSession.VisualControllers
{
    /// <summary>
    /// Orchestrates visual configuration of a dice prefab instance.
    /// Applies visual data from DiceDefinition using delegated services for each concern.
    /// This class follows the Single Responsibility Principle by delegating concerns:
    /// - IDiceBaseModelManager: Handles base model setup
    /// - IDiceMaterialManager: Handles material application
    /// - IDiceFaceAnchorResolver: Resolves anchor positions
    /// - IDiceFaceModelManager: Handles face model spawning and configuration
    /// 
    /// Key improvements over the original implementation:
    /// 1. No embedded guide synchronization: Face models are placed at anchors without Value_X guide dependency
    /// 2. Each service has a single, well-defined responsibility
    /// 3. Easy to test, extend, or replace individual components
    /// 4. Better composition over inheritance
    /// 5. Each dice configuration gets its own set of services for complete isolation
    /// </summary>
    public class DiceVisualRuntimeConfigurator : MonoBehaviour, IDiceVisualConfigurator
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

        private IDiceBaseModelManager _baseModelManager;
        private IDiceMaterialManager _materialManager;
        private IDiceFaceAnchorResolver _anchorResolver;
        private IDiceFaceModelManager _faceModelManager;

        public void ApplyFromDefinition(DiceDefinition diceDefinition)
        {
            if (diceDefinition == null) return;

            ApplyVisualConfiguration(diceDefinition.visualConfiguration, diceDefinition.faces);
        }

        private void ApplyVisualConfiguration(
            DiceVisualConfigurationData visualConfiguration,
            DiceFaceData[] gameplayFaces)
        {
            InitializeServices();

            // Setup base model
            _baseModelManager.SetupBaseModel(visualConfiguration.baseModelPrefab);
            _baseModelManager.ApplyMesh(visualConfiguration.baseMesh);
            _baseModelManager.HideBaseModelFaceValueObjects();

            // Setup materials
            _materialManager.ApplyBaseMaterials(
                visualConfiguration.baseMaterial,
                visualConfiguration.defaultFaceValueMaterial,
                visualConfiguration.faceModels);

            // Create a new face model manager with the current configuration
            // This ensures each visual configuration gets fresh material context
            IDiceFaceModelManager currentFaceModelManager = new DiceFaceModelManager(
                _anchorResolver,
                _materialManager,
                new DiceFaceValueTextController(),
                visualConfiguration.baseMaterial,
                visualConfiguration.defaultFaceValueMaterial,
                visualConfiguration.applyBaseMaterialToFaceModels);

            // Clean up previous face models
            _faceModelManager?.Cleanup();
            _faceModelManager = currentFaceModelManager;

            // Setup face models
            _faceModelManager.SetupFaceModels(
                visualConfiguration.faceModels,
                gameplayFaces);
        }

        private void InitializeServices()
        {
            // Initialize base model manager
            if (_baseModelManager == null)
            {
                _baseModelManager = new DiceBaseModelManager(
                    baseModelRoot,
                    fallbackBaseMeshFilter,
                    fallbackBaseMeshRenderer);
            }

            // Initialize material manager (depends on base model manager)
            if (_materialManager == null)
            {
                _materialManager = new DiceMaterialManager(_baseModelManager);
            }

            // Initialize face anchor resolver
            if (_anchorResolver == null)
            {
                _anchorResolver = new DiceFaceAnchorResolver(faceAnchors);
            }
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _baseModelManager?.Cleanup();
            _faceModelManager?.Cleanup();
        }
    }
}


using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.DTO;
using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;
using _Project.Presentation.Scripts.Features.DiceSession.VisualServices;
using UnityEngine;

namespace _Project.Presentation.Scripts.Features.DiceSession.VisualControllers
{
    /// <summary>
    /// Orchestrates visual configuration of a die prefab instance.
    /// Applies visual data from DiceDefinition using delegated services for each concern.
    /// - IDiceBaseModelManager: Handles base model setup
    /// - IDiceMaterialManager: Handles material application
    /// </summary>
    public class DiceVisualRuntimeConfigurator : MonoBehaviour, IDiceVisualConfigurator
    {
        [Header("Base Visual Targets")]
        [Tooltip("Parent transform used to spawn the optional base model prefab.")]
        [SerializeField] private Transform baseModelRoot;

        [Tooltip("Fallback MeshFilter used when no base model prefab is assigned.")]
        [SerializeField] private MeshFilter fallbackBaseMeshFilter;

        [Tooltip("Fallback MeshRenderer used when no base model is assigned (eg. a simple cube).")]
        [SerializeField] private MeshRenderer fallbackBaseMeshRenderer;

        private IDiceBaseModelManager _baseModelManager;
        private IDiceMaterialManager _materialManager;

        public void ApplyFromDefinition(DiceDefinition diceDefinition)
        {
            if (diceDefinition == null) return;

            ApplyVisualConfiguration(diceDefinition.visualConfiguration);
        }

        private void ApplyVisualConfiguration(DiceVisualConfigurationData visualConfiguration)
        {
            InitializeServices();

            _baseModelManager.SetupBaseModel(visualConfiguration.baseModel);
            _baseModelManager.ApplyMesh(visualConfiguration.baseMesh);

            _materialManager.ApplyMaterials(
                visualConfiguration.shaderMaterial,
                visualConfiguration.baseTexture,
                visualConfiguration.faceTextures);
        }

        // TODO: If services ever scale to require more parameters or more services, implement Dependency Injection to manage dependencies and lifecycle more cleanly.
        private void InitializeServices()
        {
            _baseModelManager ??= new DiceBaseModelManager(
                baseModelRoot,
                fallbackBaseMeshFilter,
                fallbackBaseMeshRenderer);

            _materialManager ??= new DiceMaterialManager(_baseModelManager);
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _baseModelManager?.Cleanup();
        }
    }
}


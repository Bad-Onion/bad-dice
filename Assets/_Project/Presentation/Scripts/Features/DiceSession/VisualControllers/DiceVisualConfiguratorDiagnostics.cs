using _Project.Domain.Features.Dice.Enums;
using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;
using UnityEngine;

namespace _Project.Presentation.Scripts.Features.DiceSession.VisualControllers
{
    /// <summary>
    /// Helper class for testing and verifying the refactored DiceVisualRuntimeConfigurator.
    /// Provides diagnostic information about the dice visual configuration and services.
    /// 
    /// Usage:
    /// 1. Attach this script to a GameObject with DiceVisualRuntimeConfigurator
    /// 2. Assign the configurator reference in inspector
    /// 3. Call diagnostic methods from editor buttons or code
    /// 4. Check Console output for diagnostic information
    /// </summary>
    public class DiceVisualConfiguratorDiagnostics : MonoBehaviour
    {
        [SerializeField] private DiceVisualRuntimeConfigurator configurator;
        [SerializeField] private DiceDefinition diceDefinition;

        [ContextMenu("Verify Configuration")]
        public void VerifyConfiguration()
        {
            if (configurator == null)
            {
                Debug.LogError("DiceVisualRuntimeConfigurator is not assigned!", this);
                return;
            }

            if (diceDefinition == null)
            {
                Debug.LogError("DiceDefinition is not assigned!", this);
                return;
            }

            Debug.Log("=== DiceVisualConfigurator Diagnostics ===", this);
            Debug.Log($"Configurator: {configurator.name}", this);
            Debug.Log($"Dice Definition: {diceDefinition.name}", this);

            // Verify visual configuration
            var visualConfig = diceDefinition.visualConfiguration;
            Debug.Log($"\n--- Visual Configuration ---", this);
            Debug.Log($"Base Model Prefab: {(visualConfig.baseModelPrefab != null ? visualConfig.baseModelPrefab.name : "NONE (using fallback)")}");
            Debug.Log($"Base Mesh: {(visualConfig.baseMesh != null ? visualConfig.baseMesh.name : "NONE")}");
            Debug.Log($"Base Material: {(visualConfig.baseMaterial != null ? visualConfig.baseMaterial.name : "NONE")}");
            Debug.Log($"Default Face Value Material: {(visualConfig.defaultFaceValueMaterial != null ? visualConfig.defaultFaceValueMaterial.name : "NONE")}");
            Debug.Log($"Apply Base Material To Face Models: {visualConfig.applyBaseMaterialToFaceModels}");

            // Verify face models
            Debug.Log($"\n--- Face Models Configuration ---", this);
            if (visualConfig.faceModels != null && visualConfig.faceModels.Length > 0)
            {
                for (int i = 0; i < visualConfig.faceModels.Length; i++)
                {
                    var faceModel = visualConfig.faceModels[i];
                    Debug.Log($"Face {i}: Direction={faceModel.localDirection}", this);
                    Debug.Log($"  Prefab: {(faceModel.faceValuePrefab != null ? faceModel.faceValuePrefab.name : "NONE")}", this);
                    Debug.Log($"  Model Material: {(faceModel.faceModelMaterial != null ? faceModel.faceModelMaterial.name : "NONE")}", this);
                    Debug.Log($"  Value Material: {(faceModel.faceValueMaterial != null ? faceModel.faceValueMaterial.name : "NONE")}", this);
                    Debug.Log($"  Use Gameplay Face Value As Text: {faceModel.useGameplayFaceValueAsText}", this);
                    Debug.Log($"  Custom Face Value Text: {(string.IsNullOrEmpty(faceModel.customFaceValueText) ? "NONE" : faceModel.customFaceValueText)}", this);
                }
            }
            else
            {
                Debug.LogWarning("No face models configured!", this);
            }

            // Verify gameplay faces
            Debug.Log($"\n--- Gameplay Faces ---", this);
            if (diceDefinition.faces != null && diceDefinition.faces.Length > 0)
            {
                for (int i = 0; i < diceDefinition.faces.Length; i++)
                {
                    var face = diceDefinition.faces[i];
                    Debug.Log($"Face {i}: Value={face.value}, Direction={face.localDirection}", this);
                }
            }
            else
            {
                Debug.LogWarning("No gameplay faces configured!", this);
            }

            Debug.Log($"\n=== End Diagnostics ===\n", this);
        }

        [ContextMenu("Test Apply Configuration")]
        public void TestApplyConfiguration()
        {
            if (configurator == null)
            {
                Debug.LogError("DiceVisualRuntimeConfigurator is not assigned!", this);
                return;
            }

            if (diceDefinition == null)
            {
                Debug.LogError("DiceDefinition is not assigned!", this);
                return;
            }

            Debug.Log($"Applying configuration from {diceDefinition.name}...", this);
            configurator.ApplyFromDefinition(diceDefinition);
            Debug.Log("Configuration applied successfully!", this);

            // Wait a frame then verify spawned objects
            StartCoroutine(VerifySpawnedObjectsNextFrame());
        }

        private System.Collections.IEnumerator VerifySpawnedObjectsNextFrame()
        {
            yield return null;
            VerifySpawnedObjects();
        }

        [ContextMenu("Verify Spawned Objects")]
        public void VerifySpawnedObjects()
        {
            Debug.Log("=== Spawned Objects Verification ===", this);

            Transform root = configurator.transform;
            Debug.Log($"Configurator Root: {root.name}", this);
            Debug.Log($"Children Count: {root.childCount}", this);

            foreach (Transform child in root)
            {
                Debug.Log($"  Child: {child.name} (Position: {child.localPosition}, Rotation: {child.localRotation.eulerAngles})", this);

                if (child.childCount > 0)
                {
                    foreach (Transform grandchild in child)
                    {
                        Debug.Log($"    Grandchild: {grandchild.name}", this);
                    }
                }
            }

            // Check for face value models (they should be somewhere in the hierarchy)
            DiceFaceDirection[] directions = { 
                DiceFaceDirection.Up, DiceFaceDirection.Down, 
                DiceFaceDirection.Forward, DiceFaceDirection.Back,
                DiceFaceDirection.Right, DiceFaceDirection.Left
            };

            Debug.Log($"\n--- Face Value Models ---", this);
            foreach (var direction in directions)
            {
                // Try to find anchors
                Transform anchor = FindChildByName(root, $"Anchor_{direction}");
                if (anchor == null)
                {
                    anchor = FindChildByName(root, direction.ToString());
                }

                if (anchor != null)
                {
                    Debug.Log($"{direction}: Anchor found, Children: {anchor.childCount}", this);
                    foreach (Transform child in anchor)
                    {
                        Debug.Log($"  - Spawned Model: {child.name}", this);
                    }
                }
                else
                {
                    Debug.LogWarning($"{direction}: No anchor found", this);
                }
            }

            Debug.Log($"\n=== End Verification ===\n", this);
        }

        [ContextMenu("Check Materials")]
        public void CheckMaterials()
        {
            Debug.Log("=== Materials Check ===", this);

            // Find MeshRenderer on base model
            MeshRenderer[] renderers = configurator.GetComponentsInChildren<MeshRenderer>(true);
            Debug.Log($"Total MeshRenderers found: {renderers.Length}", this);

            foreach (var renderer in renderers)
            {
                Debug.Log($"\nRenderer: {renderer.name}", this);
                Debug.Log($"  Materials count: {renderer.sharedMaterials.Length}", this);

                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    Material mat = renderer.sharedMaterials[i];
                    if (mat != null)
                    {
                        Debug.Log($"  [{i}] {mat.name}", this);
                    }
                    else
                    {
                        Debug.Log($"  [{i}] NULL", this);
                    }
                }
            }

            Debug.Log($"\n=== End Materials Check ===\n", this);
        }

        [ContextMenu("Check Text Components")]
        public void CheckTextComponents()
        {
            Debug.Log("=== Text Components Check ===", this);

            TMPro.TMP_Text[] texts = configurator.GetComponentsInChildren<TMPro.TMP_Text>(true);
            Debug.Log($"Total TMP_Text components found: {texts.Length}", this);

            foreach (var text in texts)
            {
                Debug.Log($"Text: {text.gameObject.name}", this);
                Debug.Log($"  Content: '{text.text}'", this);
                Debug.Log($"  Font Material: {(text.fontSharedMaterial != null ? text.fontSharedMaterial.name : "NONE")}", this);
            }

            Debug.Log($"\n=== End Text Check ===\n", this);
        }

        private Transform FindChildByName(Transform parent, string name)
        {
            Transform[] children = parent.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child.name.Contains(name, System.StringComparison.OrdinalIgnoreCase))
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// Quick reference for expected face anchor transforms.
        /// Place this info in a note for your team.
        /// </summary>
        [ContextMenu("Print Expected Anchors")]
        public void PrintExpectedAnchors()
        {
            Debug.Log(@"
                    === Expected Face Anchors Configuration ===

                    Face anchors should be empty GameObjects positioned at:

                    Up       → Position: (0,  0.51,  0)
                    Down     → Position: (0, -0.51,  0)
                    Forward  → Position: (0,  0,    0.51)
                    Back     → Position: (0,  0,   -0.51)
                    Right    → Position: (0.51, 0,  0)
                    Left     → Position: (-0.51, 0, 0)

                    All rotations should be identity (0, 0, 0).
                    All scales should be (1, 1, 1).

                    Configure these in DiceVisualRuntimeConfigurator's Face Anchors array.
                    ==="
            );
        }
    }
}


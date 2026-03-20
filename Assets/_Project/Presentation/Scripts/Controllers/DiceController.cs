using System.Collections;
using _Project.Domain.Entities.DiceSimulation;
using UnityEngine;

namespace _Project.Presentation.Scripts.Controllers
{
    // Class dedicated to handle a Dice Prefab (a singular dice)
    // TODO: Separate into "DiceTrajectoryRoutineController" and "DiceVisualFeedbackController" components
    public class DiceController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Assign the child GameObject that holds all the visual numbers and meshes.")]
        [SerializeField] private Transform visualsRoot;

        [Header("Visual Feedback")]
        [Tooltip("Assign the Visuals child object here to animate it upon selection.")]
        [SerializeField] private Transform visualsTransform;

        [Header("Merge Visuals")]
        [Tooltip("Assign an object/renderer here to act as the merge outline indicator.")]
        [SerializeField] private GameObject outlineVisual;

        private Coroutine _playbackCoroutine;

        public string DiceId { get; private set; }

        public void Initialize(string id)
        {
            DiceId = id;
            SetSelectionVisual(false);
        }

        public void PlayTrajectory(DicePoseSimulationResultPath poseSimulationResultPath)
        {
            if (_playbackCoroutine != null) StopCoroutine(_playbackCoroutine);

            _playbackCoroutine = StartCoroutine(PlaybackRoutine(poseSimulationResultPath));
        }

        public void StopPlayback()
        {
            if (_playbackCoroutine == null) return;

            StopCoroutine(_playbackCoroutine);
            _playbackCoroutine = null;
        }

        private IEnumerator PlaybackRoutine(DicePoseSimulationResultPath poseSimulationResultPath)
        {
            visualsRoot.localRotation = poseSimulationResultPath.VisualCorrection;

            for (int i = 0; i < poseSimulationResultPath.Frames.Count; i++)
            {
                transform.position = poseSimulationResultPath.Frames[i].Position;
                transform.rotation = poseSimulationResultPath.Frames[i].Rotation;

                yield return new WaitForFixedUpdate();
            }
        }

        public void SetSelectionVisual(bool isSelected)
        {
            if (visualsTransform == null) return;

            // Simple visual feedback: Elevate the mesh slightly to show it's selected for a reroll
            float targetY = isSelected ? 0.5f : 0f;
            visualsTransform.localPosition = new Vector3(
                visualsTransform.localPosition.x,
                targetY,
                visualsTransform.localPosition.z
            );
        }

        public void SetHoverVisual(bool isHovered)
        {
            if (visualsTransform == null) return;

            float targetScale = isHovered ? 1.15f : 1.0f;
            visualsTransform.localScale = Vector3.one * targetScale;
        }

        public void SetMergeableOutline(bool isMergeable)
        {
            if (outlineVisual != null) outlineVisual.SetActive(isMergeable);
        }
    }
}

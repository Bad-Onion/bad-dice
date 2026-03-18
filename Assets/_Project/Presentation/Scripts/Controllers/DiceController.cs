using System.Collections;
using _Project.Domain.Entities;
using UnityEngine;

namespace _Project.Presentation.Scripts.Controllers
{
    // Dice Prefab Controller
    public class DiceController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Assign the child GameObject that holds all the visual numbers and meshes.")]
        [SerializeField] private Transform visualsRoot;

        [Header("Visual Feedback")]
        [Tooltip("Assign the Visuals child object here to animate it upon selection.")]
        [SerializeField] private Transform visualsTransform;

        private Coroutine _playbackCoroutine;

        public string DiceId { get; private set; }

        public void Initialize(string id)
        {
            DiceId = id;
            SetSelectionVisual(false); // Ensure it starts unselected
        }

        public void PlayTrajectory(DicePath path)
        {
            if (_playbackCoroutine != null) StopCoroutine(_playbackCoroutine);

            _playbackCoroutine = StartCoroutine(PlaybackRoutine(path));
        }

        public void StopPlayback()
        {
            if (_playbackCoroutine == null) return;

            StopCoroutine(_playbackCoroutine);
            _playbackCoroutine = null;
        }

        private IEnumerator PlaybackRoutine(DicePath path)
        {
            visualsRoot.localRotation = path.VisualCorrection;

            for (int i = 0; i < path.Frames.Count; i++)
            {
                transform.position = path.Frames[i].Position;
                transform.rotation = path.Frames[i].Rotation;

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

            // Simple hover feedback: Scale the dice up slightly
            float targetScale = isHovered ? 1.15f : 1.0f;
            visualsTransform.localScale = Vector3.one * targetScale;
        }
    }
}

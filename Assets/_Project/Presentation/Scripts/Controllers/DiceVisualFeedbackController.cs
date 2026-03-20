using UnityEngine;

namespace _Project.Presentation.Scripts.Controllers
{
    public class DiceVisualFeedbackController : MonoBehaviour
    {
        [Header("Visual Feedback")]
        [Tooltip("Assign the Visuals child object here to animate it upon selection.")]
        [SerializeField] private Transform visualsTransform;

        [Header("Merge Visuals")]
        [Tooltip("Assign an object/renderer here to act as the merge outline indicator.")]
        [SerializeField] private GameObject outlineVisual;

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


using _Project.Domain.Entities.DiceSimulation;
using _Project.Application.Interfaces;
using UnityEngine;

namespace _Project.Presentation.Scripts.Controllers
{
    // Coordinator component for managing a Dice Prefab (a singular dice)
    public class DiceController : MonoBehaviour, IHoverablePointerTarget
    {
        [Header("Components")]
        [Tooltip("Assign the DiceTrajectoryRoutineController here to control the dice trajectory.")]
        [SerializeField] private DiceTrajectoryRoutineController trajectoryController;
        [Tooltip("Assign the DiceVisualFeedbackController here to control the dice visual feedback.")]
        [SerializeField] private DiceVisualFeedbackController visualFeedbackController;

        public string DiceId { get; private set; }

        public void Initialize(string id)
        {
            DiceId = id;
            visualFeedbackController.SetSelectionVisual(false);
        }

        public void PlayTrajectory(DicePoseSimulationResultPath poseSimulationResultPath)
        {
            trajectoryController.PlayTrajectory(poseSimulationResultPath);
        }

        public void StopPlayback()
        {
            trajectoryController.StopPlayback();
        }

        public void SetSelectionVisual(bool isSelected)
        {
            visualFeedbackController.SetSelectionVisual(isSelected);
        }

        public void SetHoverVisual(bool isHovered)
        {
            visualFeedbackController.SetHoverVisual(isHovered);
        }

        public void SetMergeableOutline(bool isMergeable)
        {
            visualFeedbackController.SetMergeableOutline(isMergeable);
        }
    }
}

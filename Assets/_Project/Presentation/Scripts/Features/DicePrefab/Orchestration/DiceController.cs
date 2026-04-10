using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;
using _Project.Domain.Features.Dice.Simulation;
using _Project.Presentation.Scripts.Features.DicePrefab.Physics;
using _Project.Presentation.Scripts.Features.DicePrefab.VisualControllers;
using UnityEngine;

namespace _Project.Presentation.Scripts.Features.DicePrefab.Orchestration
{
    // Coordinator component for managing a Dice Prefab (a singular dice)
    public class DiceController : MonoBehaviour, IHoverablePointerTarget
    {
        [Header("Components")]
        [Tooltip("Assign the DiceTrajectoryRoutineController here to control the dice trajectory.")]
        [SerializeField] private DiceTrajectoryRoutineController trajectoryController;
        [Tooltip("Assign the DiceVisualFeedbackController here to control the dice visual feedback.")]
        [SerializeField] private DiceVisualFeedbackController visualFeedbackController;
        [Tooltip("Assign the runtime visual configurator used to apply visual data from DiceDefinition.")]
        [SerializeField] private DiceVisualRuntimeConfigurator visualRuntimeConfigurator;

        public string DiceId { get; private set; }

        public void Initialize(string id, DiceDefinition definition)
        {
            DiceId = id;
            visualFeedbackController.SetSelectionVisual(false);

            if (visualRuntimeConfigurator != null)
            {
                visualRuntimeConfigurator.ApplyFromDefinition(definition);
            }
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

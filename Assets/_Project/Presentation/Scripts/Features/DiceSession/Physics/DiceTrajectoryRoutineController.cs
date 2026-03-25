using System.Collections;
using _Project.Domain.Features.Dice.Simulation;
using UnityEngine;

namespace _Project.Presentation.Scripts.Features.DiceSession.Physics
{
    public class DiceTrajectoryRoutineController : MonoBehaviour
    {
        [Header("Visual Correction")]
        [Tooltip("Assign the child GameObject that holds all the visual numbers and meshes.")]
        [SerializeField] private Transform visualsRoot;

        private Coroutine _playbackCoroutine;

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
    }
}


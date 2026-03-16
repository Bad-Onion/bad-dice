using System.Collections;
using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Controllers
{
    public class DiceController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Assign the child GameObject that holds all the visual numbers and meshes.")]
        [SerializeField] private Transform visualsRoot;

        private DiceConfiguration _config;
        private Coroutine _playbackCoroutine;

        [Inject]
        public void Construct(DiceConfiguration config)
        {
            _config = config;
        }

        private void OnEnable()
        {
            Bus<DicePlaybackRequestedEvent>.OnEvent += HandlePlaybackRequested;
            Bus<DiceResetEvent>.OnEvent += HandleReset;
        }

        private void OnDisable()
        {
            Bus<DicePlaybackRequestedEvent>.OnEvent -= HandlePlaybackRequested;
            Bus<DiceResetEvent>.OnEvent -= HandleReset;
        }

        private void HandlePlaybackRequested(DicePlaybackRequestedEvent evt)
        {
            if (_playbackCoroutine != null) StopCoroutine(_playbackCoroutine);
            _playbackCoroutine = StartCoroutine(PlaybackRoutine(evt.SimulationResult));
        }

        private IEnumerator PlaybackRoutine(DiceSimulationResult result)
        {
            visualsRoot.localRotation = result.VisualCorrection;

            for (int i = 0; i < result.Frames.Count; i++)
            {
                transform.position = result.Frames[i].Position;
                transform.rotation = result.Frames[i].Rotation;
                yield return new WaitForFixedUpdate();
            }
        }

        private void HandleReset(DiceResetEvent evt)
        {
            if (_playbackCoroutine != null) StopCoroutine(_playbackCoroutine);

            transform.position = _config.spawnPosition;
            transform.rotation = Quaternion.identity;
            visualsRoot.localRotation = Quaternion.identity;
        }
    }
}

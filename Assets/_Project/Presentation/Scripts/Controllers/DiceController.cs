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

        private Coroutine _playbackCoroutine;

        public void PlayTrajectory(DicePath path)
        {
            if (_playbackCoroutine != null) StopCoroutine(_playbackCoroutine);
            _playbackCoroutine = StartCoroutine(PlaybackRoutine(path));
        }

        public void StopPlayback()
        {
            if (_playbackCoroutine != null)
            {
                StopCoroutine(_playbackCoroutine);
                _playbackCoroutine = null;
            }
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

        // Zenject memory pool declaration
        public class Pool : MonoMemoryPool<DiceController>
        {
            protected override void OnDespawned(DiceController item)
            {
                item.StopPlayback();
                base.OnDespawned(item);
            }
        }
    }
}

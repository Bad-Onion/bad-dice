
using System.Collections;
using System.Collections.Generic;
using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    public class DiceController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Assign the child GameObject that holds all the visual numbers and meshes.")]
        [SerializeField] private Transform visualsRoot;

        private Rigidbody _rigidbody;
        private DiceConfiguration _config;
        private Coroutine _playbackCoroutine;

        [Inject]
        public void Construct(DiceConfiguration config)
        {
            _config = config;
            _rigidbody = GetComponent<Rigidbody>();

            // Default to kinematic until we need physics
            _rigidbody.isKinematic = true;
        }

        private void OnEnable()
        {
            Bus<DiceSimulationRequestedEvent>.OnEvent += HandleSimulationRequested;
            Bus<DiceResetEvent>.OnEvent += HandleReset;
        }

        private void OnDisable()
        {
            Bus<DiceSimulationRequestedEvent>.OnEvent -= HandleSimulationRequested;
            Bus<DiceResetEvent>.OnEvent -= HandleReset;
        }

        private void HandleSimulationRequested(DiceSimulationRequestedEvent evt)
        {
            if (_playbackCoroutine != null) StopCoroutine(_playbackCoroutine);

            // 1. Setup Simulation Environment
            SimulationMode originalMode = Physics.simulationMode;
            Physics.simulationMode = SimulationMode.Script;

            _rigidbody.isKinematic = false;
            _rigidbody.position = evt.StartPosition;
            _rigidbody.rotation = evt.StartRotation;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            _rigidbody.AddForce(evt.Force, ForceMode.Impulse);
            _rigidbody.AddTorque(evt.Torque, ForceMode.Impulse);

            List<DiceFrame> recordedFrames = new List<DiceFrame>(300);

            // 2. Run and Record Simulation instantly
            for (int i = 0; i < 300; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
                recordedFrames.Add(new DiceFrame(_rigidbody.position, _rigidbody.rotation));

                if (_rigidbody.IsSleeping() || (_rigidbody.linearVelocity.sqrMagnitude < 0.001f && _rigidbody.angularVelocity.sqrMagnitude < 0.001f))
                    break;
            }

            // 3. Calculate exact Landed Face
            Vector3 landedLocalUp = transform.InverseTransformDirection(Vector3.up);
            Vector3 exactLandedFace = GetClosestExactFace(landedLocalUp);

            // 4. Calculate Visual Correction Offset
            Vector3 targetLocalUp = _config.GetLocalUpForFace(evt.TargetResult);
            Quaternion visualCorrection = Quaternion.FromToRotation(targetLocalUp, exactLandedFace);

            // 5. Restore Environment to start Playback
            Physics.simulationMode = originalMode;
            _rigidbody.isKinematic = true;

            _playbackCoroutine = StartCoroutine(PlaybackRoutine(recordedFrames, visualCorrection));
        }

        private IEnumerator PlaybackRoutine(List<DiceFrame> frames, Quaternion visualCorrection)
        {
            // Apply the visual offset. Now the target face is perfectly aligned with the naturally landed face
            visualsRoot.localRotation = visualCorrection;

            // Playback physical frames
            for (int i = 0; i < frames.Count; i++)
            {
                _rigidbody.position = frames[i].Position;
                _rigidbody.rotation = frames[i].Rotation;
                yield return new WaitForFixedUpdate();
            }
        }

        private Vector3 GetClosestExactFace(Vector3 localDirection)
        {
            Vector3[] exactFaces = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
            Vector3 bestFace = Vector3.up;
            float maxDot = -Mathf.Infinity;

            foreach (var face in exactFaces)
            {
                float dot = Vector3.Dot(localDirection, face);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    bestFace = face;
                }
            }
            return bestFace;
        }

        private void HandleReset(DiceResetEvent evt)
        {
            if (_playbackCoroutine != null) StopCoroutine(_playbackCoroutine);

            _rigidbody.isKinematic = true;
            _rigidbody.position = _config.SpawnPosition;
            _rigidbody.rotation = Quaternion.identity;

            // Reset visuals cheat
            visualsRoot.localRotation = Quaternion.identity;
        }
    }
}

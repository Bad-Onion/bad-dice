using System.Collections.Generic;
using _Project.Application.Interfaces;
using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;
using UnityEngine;

namespace _Project.Infrastructure.Services
{
    public class DiceSimulationService : IDiceSimulationService
    {
        private readonly DiceConfiguration _config;

        public DiceSimulationService(DiceConfiguration config)
        {
            _config = config;
        }

        public DiceSimulationResult SimulateTrajectory(int targetResult, Vector3 startPos, Quaternion startRot, Vector3 force, Vector3 torque)
        {
            SimulationMode originalMode = Physics.simulationMode;
            Physics.simulationMode = SimulationMode.Script;

            GameObject dummy = Object.Instantiate(_config.physicsPrefab, startPos, startRot);
            Rigidbody rb = dummy.GetComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);

            List<DiceFrame> recordedFrames = new List<DiceFrame>(300);

            for (int i = 0; i < 300; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
                recordedFrames.Add(new DiceFrame(rb.position, rb.rotation));

                if (rb.IsSleeping() || (rb.linearVelocity.sqrMagnitude < 0.001f && rb.angularVelocity.sqrMagnitude < 0.001f))
                    break;
            }

            Vector3 exactLandedFace = GetClosestExactFace(dummy.transform.InverseTransformDirection(Vector3.up));
            Vector3 targetLocalUp = _config.GetLocalUpForFace(targetResult);
            Quaternion visualCorrection = Quaternion.FromToRotation(targetLocalUp, exactLandedFace);

            Object.Destroy(dummy);
            Physics.simulationMode = originalMode;

            return new DiceSimulationResult
            {
                Frames = recordedFrames,
                VisualCorrection = visualCorrection
            };
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
    }
}
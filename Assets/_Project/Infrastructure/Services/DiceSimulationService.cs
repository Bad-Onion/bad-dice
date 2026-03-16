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

        private const int MaxRecordCapacity = 300;
        private const float MaxMotionThreshold = 0.001f;
        private static readonly Vector3[] ExactFaces = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        public DiceSimulationService(DiceConfiguration config)
        {
            _config = config;
        }

        public DiceSimulationResult SimulateTrajectory(int targetResult, Vector3 startPos, Quaternion startRot, Vector3 force, Vector3 torque)
        {
            SimulationMode originalMode = Physics.simulationMode;
            
            try
            {
                Physics.simulationMode = SimulationMode.Script;
                
                GameObject dummyPhysicsObject = CreateAndInitializeDice(startPos, startRot, force, torque);
                Rigidbody rb = dummyPhysicsObject.GetComponent<Rigidbody>();

                List<DiceFrame> recordedFrames = SimulateUntilSettled(rb);
                Quaternion visualCorrection = CalculateVisualCorrection(targetResult, dummyPhysicsObject);

                Object.Destroy(dummyPhysicsObject);

                return new DiceSimulationResult
                {
                    Frames = recordedFrames,
                    VisualCorrection = visualCorrection
                };
            }
            finally
            {
                Physics.simulationMode = originalMode;
            }
        }

        private GameObject CreateAndInitializeDice(Vector3 position, Quaternion rotation, Vector3 force, Vector3 torque)
        {
            GameObject dummyPhysicsObject = Object.Instantiate(_config.physicsPrefab, position, rotation);
            Rigidbody rb = dummyPhysicsObject.GetComponent<Rigidbody>();

            rb.isKinematic = false;
            ApplyForces(rb, force, torque);

            return dummyPhysicsObject;
        }

        private void ApplyForces(Rigidbody rb, Vector3 force, Vector3 torque)
        {
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);
        }

        private List<DiceFrame> SimulateUntilSettled(Rigidbody rb)
        {
            List<DiceFrame> recordedFrames = new List<DiceFrame>(MaxRecordCapacity);

            for (int i = 0; i < MaxRecordCapacity; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
                recordedFrames.Add(new DiceFrame(rb.position, rb.rotation));

                if (HasDiceSettled(rb))
                    break;
            }

            return recordedFrames;
        }

        private bool HasDiceSettled(Rigidbody rb)
        {
            return rb.IsSleeping() || IsMotionBelowThreshold(rb);
        }

        private bool IsMotionBelowThreshold(Rigidbody rb)
        {
            return rb.linearVelocity.sqrMagnitude < MaxMotionThreshold && 
                   rb.angularVelocity.sqrMagnitude < MaxMotionThreshold;
        }

        private Quaternion CalculateVisualCorrection(int targetResult, GameObject diceObject)
        {
            Vector3 exactLandedFace = GetExactLandedFace(diceObject.transform);
            Vector3 targetLocalUp = _config.GetLocalUpForFace(targetResult);
            
            return Quaternion.FromToRotation(targetLocalUp, exactLandedFace);
        }

        private Vector3 GetExactLandedFace(Transform diceTransform)
        {
            Vector3 localUpDirection = diceTransform.InverseTransformDirection(Vector3.up);
            return GetClosestExactFace(localUpDirection);
        }

        private Vector3 GetClosestExactFace(Vector3 localDirection)
        {
            Vector3 bestFace = ExactFaces[0];
            float maxDot = Vector3.Dot(localDirection, bestFace);

            for (int i = 1; i < ExactFaces.Length; i++)
            {
                float dot = Vector3.Dot(localDirection, ExactFaces[i]);

                if (dot <= maxDot)
                    continue;

                maxDot = dot;
                bestFace = ExactFaces[i];
            }

            return bestFace;
        }
    }
}
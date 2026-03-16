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

        public DiceSimulationResult SimulateTrajectory(int[] targetResults, Vector3[] startPos, Quaternion[] startRot, Vector3[] forces, Vector3[] torques)
        {
            SimulationMode originalMode = Physics.simulationMode;

            try
            {
                Physics.simulationMode = SimulationMode.Script;

                int diceCount = targetResults.Length;
                GameObject[] dummyPhysicsObjects = CreateAndInitializeDiceArray(diceCount, startPos, startRot, forces, torques);
                Rigidbody[] rbs = GetRigidbodies(dummyPhysicsObjects);

                List<DicePath> dicePaths = SimulateUntilAllSettled(rbs);
                ApplyVisualCorrections(dicePaths, targetResults, dummyPhysicsObjects);

                CleanupDummies(dummyPhysicsObjects);

                return new DiceSimulationResult
                {
                    DicePaths = dicePaths
                };
            }
            finally
            {
                Physics.simulationMode = originalMode;
            }
        }

        private GameObject[] CreateAndInitializeDiceArray(int count, Vector3[] positions, Quaternion[] rotations, Vector3[] forces, Vector3[] torques)
        {
            GameObject[] dummies = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                // Fetch the specific physics prefab for this dice
                GameObject physicsPrefab = _config.diceDefinitions[i].physicsPrefab;

                dummies[i] = Object.Instantiate(physicsPrefab, positions[i], rotations[i]);
                Rigidbody rb = dummies[i].GetComponent<Rigidbody>();

                rb.isKinematic = false;
                ApplyForces(rb, forces[i], torques[i]);
            }
            return dummies;
        }

        private Rigidbody[] GetRigidbodies(GameObject[] dummies)
        {
            Rigidbody[] rbs = new Rigidbody[dummies.Length];
            for (int i = 0; i < dummies.Length; i++)
            {
                rbs[i] = dummies[i].GetComponent<Rigidbody>();
            }
            return rbs;
        }

        private void ApplyForces(Rigidbody rb, Vector3 force, Vector3 torque)
        {
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);
        }

        private List<DicePath> SimulateUntilAllSettled(Rigidbody[] rbs)
        {
            int count = rbs.Length;
            List<DicePath> paths = new List<DicePath>(count);

            for (int i = 0; i < count; i++)
            {
                paths.Add(new DicePath { Frames = new List<DiceFrame>(MaxRecordCapacity) });
            }

            for (int frame = 0; frame < MaxRecordCapacity; frame++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
                bool allSettled = true;

                for (int i = 0; i < count; i++)
                {
                    paths[i].Frames.Add(new DiceFrame(rbs[i].position, rbs[i].rotation));

                    if (!HasDiceSettled(rbs[i]))
                    {
                        allSettled = false;
                    }
                }

                if (allSettled) break;
            }

            return paths;
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

        private void ApplyVisualCorrections(List<DicePath> paths, int[] targetResults, GameObject[] diceObjects)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                DicePath path = paths[i];
                path.VisualCorrection = CalculateVisualCorrection(targetResults[i], diceObjects[i]);
                paths[i] = path;
            }
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

        private void CleanupDummies(GameObject[] dummies)
        {
            foreach (var dummy in dummies)
            {
                Object.Destroy(dummy);
            }
        }
    }
}
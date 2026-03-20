using System.Collections.Generic;
using System.Linq;
using _Project.Application.Interfaces;
using _Project.Domain.Entities.DiceSimulation;
using _Project.Domain.Enums;
using _Project.Domain.ScriptableObjects.DiceDefinitions;
using UnityEngine;

namespace _Project.Infrastructure.Services
{
    public class DiceSimulationService : IDiceSimulationService
    {
        private const int MaxRecordCapacity = 300;
        private const float MaxMotionThreshold = 0.001f;
        // TODO: Use DiceFaceDirection instead of hardcoded vectors
        private static readonly Vector3[] ExactFaces = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        public DiceSimulationResult SimulateTrajectory(
            DiceDefinition[] definitions,
            int[] targetFaceIndices,
            Vector3[] startPos,
            Quaternion[] startRot,
            Vector3[] forces,
            Vector3[] torques)
        {
            SimulationMode originalMode = Physics.simulationMode;

            try
            {
                Physics.simulationMode = SimulationMode.Script;

                GameObject[] dummyPhysicsObjects = CreateAndInitializeDiceArray(definitions, startPos, startRot, forces, torques);
                Rigidbody[] rigidBodies = GetRigidbodies(dummyPhysicsObjects);

                List<DicePoseSimulationResultPath> dicePaths = SimulateUntilAllSettled(rigidBodies);
                ApplyVisualCorrections(dicePaths, definitions, targetFaceIndices, dummyPhysicsObjects);

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

        private GameObject[] CreateAndInitializeDiceArray(DiceDefinition[] definitions, Vector3[] positions, Quaternion[] rotations, Vector3[] forces, Vector3[] torques)
        {
            int count = definitions.Length;
            GameObject[] dummyDice = new GameObject[count];

            for (int i = 0; i < count; i++)
            {
                GameObject physicsPrefab = definitions[i].physicsPrefab;

                dummyDice[i] = Object.Instantiate(physicsPrefab, positions[i], rotations[i]);
                Rigidbody rigidBody = dummyDice[i].GetComponent<Rigidbody>();

                rigidBody.isKinematic = false;
                ApplyForces(rigidBody, forces[i], torques[i]);
            }

            return dummyDice;
        }

        private Rigidbody[] GetRigidbodies(GameObject[] dummies)
        {
            Rigidbody[] dummyRigidbodies = new Rigidbody[dummies.Length];

            for (int i = 0; i < dummies.Length; i++)
            {
                dummyRigidbodies[i] = dummies[i].GetComponent<Rigidbody>();
            }

            return dummyRigidbodies;
        }

        private void ApplyForces(Rigidbody rb, Vector3 force, Vector3 torque)
        {
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);
        }

        private List<DicePoseSimulationResultPath> SimulateUntilAllSettled(Rigidbody[] rigidBodies)
        {
            int count = rigidBodies.Length;
            List<DicePoseSimulationResultPath> paths = Enumerable.Range(0, count).Select(_ => new DicePoseSimulationResultPath { Frames = new List<DicePoseFrame>(MaxRecordCapacity) }).ToList();

            for (int frame = 0; frame < MaxRecordCapacity; frame++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
                bool allSettled = true;

                for (int i = 0; i < count; i++)
                {
                    paths[i].Frames.Add(new DicePoseFrame(rigidBodies[i].position, rigidBodies[i].rotation));

                    if (!HasDiceSettled(rigidBodies[i]))
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

        private void ApplyVisualCorrections(List<DicePoseSimulationResultPath> paths, DiceDefinition[] definitions, int[] targetFaceIndices, GameObject[] diceObjects)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                DicePoseSimulationResultPath poseSimulationResultPath = paths[i];
                poseSimulationResultPath.VisualCorrection = CalculateVisualCorrection(definitions[i], targetFaceIndices[i], diceObjects[i]);
                paths[i] = poseSimulationResultPath;
            }
        }

        private Quaternion CalculateVisualCorrection(DiceDefinition definition, int targetFaceIndex, GameObject diceObject)
        {
            Vector3 exactLandedFace = GetExactLandedFace(diceObject.transform);
            Vector3 targetLocalUp = definition.GetFaceData(targetFaceIndex).localDirection.ToVector3();

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
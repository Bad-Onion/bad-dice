using System.Collections.Generic;
using System.Linq;
using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.Enums;
using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;
using _Project.Domain.Features.Dice.Simulation;
using UnityEngine;

namespace _Project.Infrastructure.Features.DiceSession.UseCases
{
    public class DiceSimulationService : IDiceSimulationService
    {
        private const int MaxRecordCapacity = 300;
        private const float MaxMotionThreshold = 0.001f;
        private static readonly DiceFaceDirection[] ExactFaces =
        {
            DiceFaceDirection.Up,
            DiceFaceDirection.Down,
            DiceFaceDirection.Left,
            DiceFaceDirection.Right,
            DiceFaceDirection.Forward,
            DiceFaceDirection.Back
        };

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

        private static GameObject[] CreateAndInitializeDiceArray(DiceDefinition[] definitions, Vector3[] positions, Quaternion[] rotations, Vector3[] forces, Vector3[] torques)
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

        private static Rigidbody[] GetRigidbodies(GameObject[] dummies)
        {
            Rigidbody[] dummyRigidbodies = new Rigidbody[dummies.Length];

            for (int i = 0; i < dummies.Length; i++)
            {
                dummyRigidbodies[i] = dummies[i].GetComponent<Rigidbody>();
            }

            return dummyRigidbodies;
        }

        private static void ApplyForces(Rigidbody rb, Vector3 force, Vector3 torque)
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

        private static bool HasDiceSettled(Rigidbody rb)
        {
            return rb.IsSleeping() || IsMotionBelowThreshold(rb);
        }

        private static bool IsMotionBelowThreshold(Rigidbody rb)
        {
            return rb.linearVelocity.sqrMagnitude < MaxMotionThreshold &&
                   rb.angularVelocity.sqrMagnitude < MaxMotionThreshold;
        }

        private static void ApplyVisualCorrections(List<DicePoseSimulationResultPath> paths, DiceDefinition[] definitions, int[] targetFaceIndices, GameObject[] diceObjects)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                DicePoseSimulationResultPath poseSimulationResultPath = paths[i];
                poseSimulationResultPath.VisualCorrection = CalculateVisualCorrection(definitions[i], targetFaceIndices[i], diceObjects[i]);
                paths[i] = poseSimulationResultPath;
            }
        }

        private static Quaternion CalculateVisualCorrection(DiceDefinition definition, int targetFaceIndex, GameObject diceObject)
        {
            Vector3 exactLandedFace = GetExactLandedFace(diceObject.transform);
            Vector3 targetLocalUp = definition.GetFaceData(targetFaceIndex).localDirection.ToVector3();

            return Quaternion.FromToRotation(targetLocalUp, exactLandedFace);
        }

        private static Vector3 GetExactLandedFace(Transform diceTransform)
        {
            Vector3 localUpDirection = diceTransform.InverseTransformDirection(Vector3.up);
            return GetClosestExactFace(localUpDirection);
        }

        private static Vector3 GetClosestExactFace(Vector3 localDirection)
        {
            DiceFaceDirection bestFaceDirection = ExactFaces[0];
            float maxDot = Vector3.Dot(localDirection, bestFaceDirection.ToVector3());

            for (int i = 1; i < ExactFaces.Length; i++)
            {
                DiceFaceDirection currentFaceDirection = ExactFaces[i];
                float dot = Vector3.Dot(localDirection, currentFaceDirection.ToVector3());

                if (dot <= maxDot)
                    continue;

                maxDot = dot;
                bestFaceDirection = currentFaceDirection;
            }

            return bestFaceDirection.ToVector3();
        }

        private static void CleanupDummies(GameObject[] dummies)
        {
            foreach (var dummy in dummies)
            {
                Object.Destroy(dummy);
            }
        }
    }
}
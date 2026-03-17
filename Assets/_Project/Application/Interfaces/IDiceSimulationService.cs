using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;
using UnityEngine;

namespace _Project.Application.Interfaces
{
    public interface IDiceSimulationService
    {
        DiceSimulationResult SimulateTrajectory(
            DiceDefinition[] definitions,
            int[] targetFaceIndices,
            Vector3[] startPos,
            Quaternion[] startRot,
            Vector3[] forces,
            Vector3[] torques);
    }
}
using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;
using _Project.Domain.Features.Dice.Simulation;
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
using _Project.Domain.Entities;
using UnityEngine;

namespace _Project.Application.Interfaces
{
    public interface IDiceSimulationService
    {
        DiceSimulationResult SimulateTrajectory(int[] targetResults, Vector3[] startPos, Quaternion[] startRot, Vector3[] forces, Vector3[] torques);
    }
}
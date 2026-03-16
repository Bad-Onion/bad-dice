using _Project.Domain.Entities;
using UnityEngine;

namespace _Project.Application.Interfaces
{
    public interface IDiceSimulationService
    {
        DiceSimulationResult SimulateTrajectory(int targetResult, Vector3 startPos, Quaternion startRot, Vector3 force, Vector3 torque);
    }
}
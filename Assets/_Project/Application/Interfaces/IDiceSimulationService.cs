using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;
using _Project.Domain.Features.Dice.Simulation;
using UnityEngine;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Service for simulating dice trajectories based on given parameters.
    /// </summary>
    public interface IDiceSimulationService
    {
        /// <summary>
        /// Simulates the trajectory of dice based on their definitions, target face indices, initial positions and rotations, applied forces, and torques.
        /// </summary>
        /// <param name="definitions">An array of dice definitions that describe the properties of each die being simulated.</param>
        /// <param name="targetFaceIndices">An array of integers representing the target face indices for each die, indicating which face
        /// should be facing up at the end of the simulation.</param>
        /// <param name="startPositions">An array of Vector3 representing the initial positions of each die at the start of the simulation.</param>
        /// <param name="startRotations">An array of Quaternions representing the initial rotations of each die at the start of the simulation.</param>
        /// <param name="forces">An array of Vector3 representing the forces applied to each die at the start of the simulation.</param>
        /// <param name="torques">An array of Vector3 representing the torques applied to each die at the start of the simulation.</param>
        /// <returns>Returns a DiceSimulationResult containing the outcomes of the simulation, including the final positions, rotations,
        /// and face indices of each die.</returns>
        DiceSimulationResult SimulateTrajectory(
            DiceDefinition[] definitions,
            int[] targetFaceIndices,
            Vector3[] startPositions,
            Quaternion[] startRotations,
            Vector3[] forces,
            Vector3[] torques);
    }
}
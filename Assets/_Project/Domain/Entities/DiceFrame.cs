using UnityEngine;

namespace _Project.Domain.Entities
{
    // TODO: Move to "Entities/DiceSimulation"
    // TODO: Rename to something more descriptive like "DicePoseSimulationResult" or "DicePoseFrame"
    public struct DiceFrame
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public DiceFrame(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
using UnityEngine;

namespace _Project.Domain.Entities.DiceSimulation
{
    public struct DicePoseFrame
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public DicePoseFrame(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
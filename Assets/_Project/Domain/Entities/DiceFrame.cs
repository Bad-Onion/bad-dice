using UnityEngine;

namespace _Project.Domain.Entities
{
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
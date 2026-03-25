using UnityEngine;

namespace _Project.Domain.Features.Dice.Enums
{
    public enum DiceFaceDirection
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Back
    }

    public static class DiceFaceDirectionExtensions
    {
        public static Vector3 ToVector3(this DiceFaceDirection direction) => direction switch
        {
            DiceFaceDirection.Up => Vector3.up,
            DiceFaceDirection.Down => Vector3.down,
            DiceFaceDirection.Left => Vector3.left,
            DiceFaceDirection.Right => Vector3.right,
            DiceFaceDirection.Forward => Vector3.forward,
            DiceFaceDirection.Back => Vector3.back,
            _ => Vector3.up
        };
    }
}
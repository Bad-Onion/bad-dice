using _Project.Domain.Features.Dice.Enums;
using UnityEngine;

namespace _Project.Infrastructure.Features.DiceSession.Utility
{
    public class DiceFacesService
    {
        private static readonly DiceFaceDirection[] ExactFaces =
        {
            DiceFaceDirection.Up,
            DiceFaceDirection.Down,
            DiceFaceDirection.Left,
            DiceFaceDirection.Right,
            DiceFaceDirection.Forward,
            DiceFaceDirection.Back
        };

        public static Vector3 GetExactLandedFace(Transform diceTransform)
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
    }
}
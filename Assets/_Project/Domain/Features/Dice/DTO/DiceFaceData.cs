using System;
using _Project.Domain.Features.Dice.Enums;
using UnityEngine;

namespace _Project.Domain.Features.Dice.DTO
{
    [Serializable]
    public struct DiceFaceData
    {
        [Header("Gameplay")]
        [Tooltip("The numerical value or effect ID of this face.")]
        public int value;

        [Tooltip("The local direction this face points to when resting. Choose the direction that aligns with the face in the 3D model.")]
        public DiceFaceDirection localDirection;
    }
}
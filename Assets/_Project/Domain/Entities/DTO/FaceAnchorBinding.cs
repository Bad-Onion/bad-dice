using System;
using _Project.Domain.Enums;
using UnityEngine;

namespace _Project.Domain.Entities.DTO
{
    [Serializable]
    public struct FaceAnchorBinding
    {
        [Tooltip("Face direction used to map runtime face model placement.")]
        public DiceFaceDirection localDirection;

        [Tooltip("Transform used as spawn anchor for this face model.")]
        public Transform anchor;
    }
}
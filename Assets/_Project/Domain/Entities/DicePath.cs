using System.Collections.Generic;
using UnityEngine;

namespace _Project.Domain.Entities
{
    public struct DicePath
    {
        public List<DiceFrame> Frames;
        public Quaternion VisualCorrection;
    }
}
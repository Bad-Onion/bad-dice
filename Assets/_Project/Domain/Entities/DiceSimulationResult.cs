using System.Collections.Generic;
using UnityEngine;

namespace _Project.Domain.Entities
{
    public struct DiceSimulationResult
    {
        public List<DiceFrame> Frames;
        public Quaternion VisualCorrection;
    }
}
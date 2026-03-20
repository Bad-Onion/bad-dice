using System.Collections.Generic;
using UnityEngine;

namespace _Project.Domain.Entities.DiceSimulation
{
    public struct DicePoseSimulationResultPath
    {
        public List<DicePoseFrame> Frames;
        public Quaternion VisualCorrection;
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Domain.Features.Dice.Simulation
{
    public struct DicePoseSimulationResultPath
    {
        public List<DicePoseFrame> Frames;
        public Quaternion VisualCorrection;
    }
}
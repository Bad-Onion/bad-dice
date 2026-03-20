using System.Collections.Generic;
using UnityEngine;

namespace _Project.Domain.Entities
{
    // TODO: Move to "Entities/DiceSimulation"
    // TODO: Rename to something more descriptive like "DicePoseSimulationResultPath"
    public struct DicePath
    {
        public List<DiceFrame> Frames;
        public Quaternion VisualCorrection;
    }
}
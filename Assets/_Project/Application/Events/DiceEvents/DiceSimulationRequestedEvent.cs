using _Project.Application.Interfaces;
using UnityEngine;

namespace _Project.Application.Events.DiceEvents
{
    public struct DiceSimulationRequestedEvent : IEvent
    {
        public int TargetResult;
        public Vector3 StartPosition;
        public Quaternion StartRotation;
        public Vector3 Force;
        public Vector3 Torque;
    }
}
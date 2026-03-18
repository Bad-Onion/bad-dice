using System.Collections.Generic;
using _Project.Application.Interfaces;

namespace _Project.Application.Events.MergeEvents
{
    public struct MergePossibilitiesEvaluatedEvent : IEvent
    {
        public bool CanMerge { get; set; }
        public List<string> MergeableDiceIds { get; set; }
    }
}
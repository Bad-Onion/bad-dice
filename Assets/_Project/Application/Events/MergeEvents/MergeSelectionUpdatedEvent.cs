using System.Collections.Generic;
using _Project.Application.Interfaces;

namespace _Project.Application.Events.MergeEvents
{
    public struct MergeSelectionUpdatedEvent : IEvent
    {
        public string TargetDiceId { get; set; }
        public List<string> SelectedDiceIds { get; set; }
    }
}
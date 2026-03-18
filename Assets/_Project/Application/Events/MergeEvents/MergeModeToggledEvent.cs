using _Project.Application.Interfaces;

namespace _Project.Application.Events.MergeEvents
{
    public struct MergeModeToggledEvent : IEvent
    {
        public bool IsActive { get; set; }
    }
}
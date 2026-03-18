using _Project.Application.Interfaces;

namespace _Project.Application.Events.DiceEvents
{
    public struct DiceRerollToggledEvent : IEvent
    {
        public string DiceId { get; set; }
        public bool IsSelected { get; set; }
    }
}
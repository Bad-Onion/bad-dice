using _Project.Application.Interfaces;

namespace _Project.Application.Events.DiceInput
{
    public struct DiceRerollToggledEvent : IEvent
    {
        public string DiceId { get; set; }
        public bool IsSelected { get; set; }
    }
}
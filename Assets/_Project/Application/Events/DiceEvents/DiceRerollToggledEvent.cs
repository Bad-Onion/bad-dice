using _Project.Application.Interfaces;

namespace _Project.Application.Events.DiceEvents
{
    // TODO: Move event to a more appropriate location like inside a "Events/DiceInput" folder
    public struct DiceRerollToggledEvent : IEvent
    {
        public string DiceId { get; set; }
        public bool IsSelected { get; set; }
    }
}
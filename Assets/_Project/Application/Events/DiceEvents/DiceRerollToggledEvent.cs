namespace _Project.Application.Events.DiceEvents
{
    public struct DiceRerollToggledEvent
    {
        public string DiceId { get; set; }
        public bool IsSelected { get; set; }
    }
}
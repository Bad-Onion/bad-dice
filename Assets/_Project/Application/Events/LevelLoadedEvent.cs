using _Project.Application.Interfaces;
using _Project.Domain.ScriptableObjects;

namespace _Project.Application.Events
{
    // TODO: Move event to a more appropriate location like inside a "Events/Load" folder
    public struct LevelLoadedEvent : IEvent
    {
        public LevelData Level;

        public LevelLoadedEvent(LevelData level)
        {
            Level = level;
        }
    }
}
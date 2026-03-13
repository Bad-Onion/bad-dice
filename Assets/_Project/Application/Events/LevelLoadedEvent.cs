using _Project.Application.Interfaces;
using _Project.Domain.ScriptableObjects;

namespace _Project.Application.Events
{
    public struct LevelLoadedEvent : IEvent
    {
        public LevelData Level;

        public LevelLoadedEvent(LevelData level)
        {
            Level = level;
        }
    }
}
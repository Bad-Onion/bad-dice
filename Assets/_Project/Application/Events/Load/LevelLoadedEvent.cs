using _Project.Application.Interfaces;
using _Project.Domain.ScriptableObjects;
using _Project.Domain.ScriptableObjects.GameSettings;

namespace _Project.Application.Events.Load
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
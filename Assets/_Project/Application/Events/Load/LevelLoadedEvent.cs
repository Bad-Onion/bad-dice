using _Project.Application.Interfaces;
using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;

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
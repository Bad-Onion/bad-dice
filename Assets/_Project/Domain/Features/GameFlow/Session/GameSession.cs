using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;

namespace _Project.Domain.Features.GameFlow.Session
{
    public class GameSession
    {
        public bool IsLevelLoaded { get; set; }
        public LevelData CurrentLevelData { get; set; }
    }
}
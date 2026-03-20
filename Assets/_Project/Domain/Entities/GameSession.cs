using _Project.Domain.ScriptableObjects;

namespace _Project.Domain.Entities
{
    // TODO: Move to "Entities/Session"
    public class GameSession
    {
        public bool IsLevelLoaded { get; set; }
        public LevelData CurrentLevelData { get; set; }
    }
}
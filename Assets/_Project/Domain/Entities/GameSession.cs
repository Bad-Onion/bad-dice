using _Project.Domain.ScriptableObjects;

namespace _Project.Domain.Entities
{
    public class GameSession
    {
        public bool IsLevelLoaded { get; set; }
        public LevelData CurrentLevelData { get; set; }
    }
}
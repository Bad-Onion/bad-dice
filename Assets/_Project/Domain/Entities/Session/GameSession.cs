using _Project.Domain.ScriptableObjects;
using _Project.Domain.ScriptableObjects.GameSettings;

namespace _Project.Domain.Entities.Session
{
    public class GameSession
    {
        public bool IsLevelLoaded { get; set; }
        public LevelData CurrentLevelData { get; set; }
    }
}
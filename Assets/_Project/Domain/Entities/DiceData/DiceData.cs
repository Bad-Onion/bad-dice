using _Project.Domain.ScriptableObjects.DiceDefinitions;

namespace _Project.Domain.Entities.DiceData
{
    public class DiceData
    {
        public string Id { get; set; }
        public DiceDefinition Definition { get; set; }
    }
}

using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;

namespace _Project.Domain.Features.Dice.Entities
{
    public class DiceData
    {
        public string Id { get; set; }
        public DiceDefinition Definition { get; set; }
    }
}

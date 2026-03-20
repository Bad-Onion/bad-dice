using _Project.Domain.ScriptableObjects;
using _Project.Domain.ScriptableObjects.DiceDefinitions;

namespace _Project.Domain.Entities.DiceData
{
    public class OwnedDiceData
    {
        public string Id { get; set; }
        public DiceDefinition Definition { get; set; }
        public int Level { get; set; }
        public bool IsEquipped { get; set; }
    }
}
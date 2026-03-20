using _Project.Domain.ScriptableObjects;

namespace _Project.Domain.Entities
{
    // TODO: Move to "Entities/DiceData"
    public class OwnedDiceData
    {
        public string Id { get; set; }
        public DiceDefinition Definition { get; set; }
        public int Level { get; set; }
        public bool IsEquipped { get; set; }
    }
}
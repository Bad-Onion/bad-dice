using _Project.Domain.ScriptableObjects;

namespace _Project.Domain.Entities
{
    public class DiceState
    {
        public string Id { get; set; }
        public DiceDefinition Definition { get; set; }
        public int Level { get; set; } = 1;

        // -1 means the dice hasn't been rolled yet in this encounter
        public int CurrentFaceIndex { get; set; } = -1;

        public bool IsSelectedForReroll { get; set; }

        public int CurrentValue => CurrentFaceIndex >= 0 ? Definition.GetFaceData(CurrentFaceIndex).value : 0;

        // TODO: Calculate damage in another layer, this is just a placeholder for now
        public int TotalDamage => CurrentValue * Level;
    }
}
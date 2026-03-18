using _Project.Domain.ScriptableObjects;

namespace _Project.Domain.Entities
{
    public class DiceState
    {
        public string Id { get; set; }
        public DiceDefinition Definition { get; set; }
        public int Level { get; set; } = 1;
        public int CurrentFaceIndex { get; set; } = -1; // -1 means the dice hasn't been rolled yet in this encounter
        public bool IsSelectedForReroll { get; set; }
        public bool IsSelectedForMerge { get; set; }

        public int CurrentValue => CurrentFaceIndex >= 0 ? Definition.GetFaceData(CurrentFaceIndex).value : 0;
    }
}
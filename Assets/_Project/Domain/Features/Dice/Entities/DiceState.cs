namespace _Project.Domain.Features.Dice.Entities
{
    public class DiceState
    {
        public DiceData Dice { get; set; }
        public int Level { get; set; } = 1;
        public int CurrentFaceIndex { get; set; } = -1; // -1 means the dice hasn't been rolled yet in this encounter
        public bool IsSelectedForReroll { get; set; }

        public int CurrentValue => CurrentFaceIndex >= 0 ? Dice.Definition.GetFaceData(CurrentFaceIndex).value : 0;
    }
}
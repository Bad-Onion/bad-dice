using _Project.Domain.ScriptableObjects.DiceDefinitions;

namespace _Project.Domain.Entities.DiceData
{
    public class DiceState
    {
        // TODO: Id and Definition are not state, they are part of a new entity DiceData that holds pure data, we should separate them in a different class, and have DiceState only contain the state of the dice during an encounter
        public string Id { get; set; }
        public DiceDefinition Definition { get; set; }
        public int Level { get; set; } = 1;
        public int CurrentFaceIndex { get; set; } = -1; // -1 means the dice hasn't been rolled yet in this encounter
        public bool IsSelectedForReroll { get; set; }

        public int CurrentValue => CurrentFaceIndex >= 0 ? Definition.GetFaceData(CurrentFaceIndex).value : 0;
    }
}
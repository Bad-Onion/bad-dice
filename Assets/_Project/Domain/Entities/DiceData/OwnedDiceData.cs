using _Project.Domain.ScriptableObjects.DiceDefinitions;

namespace _Project.Domain.Entities.DiceData
{
    public class OwnedDiceData
    {
        // TODO: Id, Definition and Level are not related to the equipped dice, Id and Definition are part of a new entity DiceData that holds pure dice data, Level is part of the DiceState as it can be changed during an encounter with upgrades, we should separate them in a different class, and have OwnedDiceData only contain the data of the dice that is owned by the player related to the Inventory, and have DiceState only contain the state of the dice during an encounter
        public string Id { get; set; }
        public DiceDefinition Definition { get; set; }
        public int Level { get; set; }
        public bool IsEquipped { get; set; }
    }
}
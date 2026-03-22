using _Project.Domain.Entities.DiceData;
using _Project.Domain.Entities.DTO;
using _Project.Domain.Entities.Session;
using _Project.Domain.ScriptableObjects.DiceDefinitions;

namespace _Project.Infrastructure.Services
{
    public static class RunDataConverter
    {
        public static PlayerRunSaveData ToSaveData(PlayerRunState state)
        {
            var saveData = new PlayerRunSaveData { maxEquippedDice = state.MaxEquippedDice };

            foreach (var ownedDice in state.Inventory)
            {
                saveData.inventory.Add(new OwnedDiceSaveData
                {
                    id = ownedDice.Dice.Id,
                    definitionName = ownedDice.Dice.Definition.name,
                    isEquipped = ownedDice.IsEquipped
                });
            }

            return saveData;
        }

        public static PlayerRunState ToRunState(PlayerRunSaveData saveData, DiceDatabase diceDatabase)
        {
            var state = new PlayerRunState { MaxEquippedDice = saveData.maxEquippedDice };

            foreach (var savedDice in saveData.inventory)
            {
                DiceDefinition def = diceDatabase.GetDefinition(savedDice.definitionName);
                if (def == null) continue;

                state.Inventory.Add(new OwnedDiceData
                {
                    Dice = new DiceData
                    {
                        Id = savedDice.id,
                        Definition = def
                    },
                    IsEquipped = savedDice.isEquipped
                });
            }

            return state;
        }
    }
}
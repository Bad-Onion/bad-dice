using _Project.Domain.Entities.DiceData;
using _Project.Domain.Entities.DTO;
using _Project.Domain.Entities.Session;
using _Project.Domain.ScriptableObjects.DiceDefinitions;

namespace _Project.Infrastructure.Services
{
    public static class RunDataConverter
    {
        public static PlayerRunSaveData ToSaveData(PlayerRunState runState)
        {
            var saveData = new PlayerRunSaveData
            {
                maxEquippedDice = runState.MaxEquippedDice,
                rerollsPerTurn = runState.RerollsPerTurn,
                turnsPerFight = runState.TurnsPerFight
            };

            foreach (var ownedDice in runState.DiceInventory)
            {
                saveData.diceInventory.Add(new OwnedDiceSaveData
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
            var runState = new PlayerRunState
            {
                MaxEquippedDice = saveData.maxEquippedDice,
                RerollsPerTurn = saveData.rerollsPerTurn,
                TurnsPerFight = saveData.turnsPerFight
            };

            foreach (var savedDice in saveData.diceInventory)
            {
                DiceDefinition definition = diceDatabase.GetDefinition(savedDice.definitionName);
                if (definition == null) continue;

                runState.DiceInventory.Add(new OwnedDiceData
                {
                    Dice = new DiceData
                    {
                        Id = savedDice.id,
                        Definition = definition
                    },
                    IsEquipped = savedDice.isEquipped
                });
            }

            return runState;
        }
    }
}
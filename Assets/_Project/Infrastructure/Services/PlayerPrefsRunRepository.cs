using _Project.Application.Interfaces;
using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;
using UnityEngine;

namespace _Project.Infrastructure.Services
{
    public class PlayerPrefsRunRepository : IRunRepository
    {
        private const string SaveKey = "BadDice_RunSave";
        private readonly DiceDatabase _diceDatabase;

        public PlayerPrefsRunRepository(DiceDatabase diceDatabase)
        {
            _diceDatabase = diceDatabase;
        }

        public bool HasActiveRun() => PlayerPrefs.HasKey(SaveKey);

        public void SaveRun(PlayerRunState state)
        {
            // TODO: Create an adaper class to handle the conversion from PlayerRunState to PlayerRunSaveData
            var saveData = new PlayerRunSaveData { maxEquippedDice = state.MaxEquippedDice };

            foreach (var dice in state.Inventory)
            {
                saveData.inventory.Add(new OwnedDiceSaveData
                {
                    id = dice.Id,
                    definitionName = dice.Definition.name,
                    level = dice.Level,
                    isEquipped = dice.IsEquipped
                });
            }

            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public PlayerRunState LoadRun()
        {
            if (!HasActiveRun()) return null;

            string json = PlayerPrefs.GetString(SaveKey);
            var saveData = JsonUtility.FromJson<PlayerRunSaveData>(json);

            // TODO: Create an adaper class to handle the conversion from PlayerRunSaveData to PlayerRunState
            var state = new PlayerRunState { MaxEquippedDice = saveData.maxEquippedDice };

            foreach (var savedDice in saveData.inventory)
            {
                DiceDefinition def = _diceDatabase.GetDefinition(savedDice.definitionName);
                if (def == null) continue;

                state.Inventory.Add(new OwnedDiceData
                {
                    Id = savedDice.id,
                    Definition = def,
                    Level = savedDice.level,
                    IsEquipped = savedDice.isEquipped
                });
            }

            return state;
        }
    }
}
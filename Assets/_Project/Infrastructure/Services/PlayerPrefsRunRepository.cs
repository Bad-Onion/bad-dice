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
            var saveData = new PlayerRunSaveData { MaxEquippedDice = state.MaxEquippedDice };

            foreach (var dice in state.Inventory)
            {
                saveData.Inventory.Add(new OwnedDiceSaveData
                {
                    Id = dice.Id,
                    DefinitionName = dice.Definition.name,
                    Level = dice.Level,
                    IsEquipped = dice.IsEquipped
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

            var state = new PlayerRunState { MaxEquippedDice = saveData.MaxEquippedDice };

            foreach (var savedDice in saveData.Inventory)
            {
                DiceDefinition def = _diceDatabase.GetDefinition(savedDice.DefinitionName);
                if (def == null) continue;

                state.Inventory.Add(new OwnedDiceData
                {
                    Id = savedDice.Id,
                    Definition = def,
                    Level = savedDice.Level,
                    IsEquipped = savedDice.IsEquipped
                });
            }

            return state;
        }
    }
}
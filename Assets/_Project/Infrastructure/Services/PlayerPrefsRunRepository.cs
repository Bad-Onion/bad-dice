using _Project.Application.Interfaces;
using _Project.Domain.Entities.DTO;
using _Project.Domain.Entities.Session;
using _Project.Domain.ScriptableObjects.DiceDefinitions;
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
            PlayerRunSaveData saveData = RunDataConverter.ToSaveData(state);

            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public PlayerRunState LoadRun()
        {
            if (!HasActiveRun()) return null;

            string json = PlayerPrefs.GetString(SaveKey);
            var saveData = JsonUtility.FromJson<PlayerRunSaveData>(json);

            return RunDataConverter.ToRunState(saveData, _diceDatabase);
        }
    }
}
using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.DTO;
using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;
using _Project.Domain.Features.Run.DTO;
using _Project.Domain.Features.Run.Session;
using UnityEngine;

namespace _Project.Infrastructure.Features.Run.Persistence
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
using _Project.Application.Interfaces;
using _Project.Domain.Features.Combat.Session;
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

        public void SaveRun(PlayerRunState state, CombatSessionState combatSessionState)
        {
            PlayerRunSaveData saveData = RunDataConverter.ToSaveData(state, combatSessionState);

            string json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public PlayerRunState LoadRun(CombatSessionState combatSessionState)
        {
            if (!HasActiveRun()) return null;

            string json = PlayerPrefs.GetString(SaveKey);
            var saveData = JsonUtility.FromJson<PlayerRunSaveData>(json);

            RunDataConverter.ApplyCombatProgression(saveData, combatSessionState);

            return RunDataConverter.ToRunState(saveData, _diceDatabase);
        }
    }
}
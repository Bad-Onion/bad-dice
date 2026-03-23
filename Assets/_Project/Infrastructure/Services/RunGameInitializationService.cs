using System;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities.DiceData;
using _Project.Domain.Entities.Session;
using _Project.Domain.ScriptableObjects.GameSettings;

namespace _Project.Infrastructure.Services
{
    public class RunGameInitializationService : IRunInitializationUseCase
    {
        private readonly IRunRepository _repository;
        private readonly GameConfiguration _gameConfiguration;
        private readonly PlayerRunState _runState;

        public RunGameInitializationService(IRunRepository repository, GameConfiguration gameConfiguration, PlayerRunState runState)
        {
            _repository = repository;
            _gameConfiguration = gameConfiguration;
            _runState = runState;
        }

        public void EnsureRunInitialized()
        {
            // Use to reset the game database
            // PlayerPrefs.DeleteAll();

            if (_repository.HasActiveRun())
            {
                LoadRun();
                return;
            }

            InitializeRun();
        }

        private void LoadRun()
        {
            var loadedState = _repository.LoadRun();
            SetRunState(loadedState);
        }

        private void SetRunState(PlayerRunState runState)
        {
            RunDefinitions runDefinitions = _gameConfiguration.runDefinitions;

            _runState.DiceInventory = runState.DiceInventory;
            _runState.MaxEquippedDice = runState.MaxEquippedDice > 0 ? runState.MaxEquippedDice : runDefinitions.maxEquippedDice;
            _runState.RerollsPerTurn = runState.RerollsPerTurn > 0 ? runState.RerollsPerTurn : runDefinitions.rerollsPerTurn;
            _runState.TurnsPerFight = runState.TurnsPerFight > 0 ? runState.TurnsPerFight : runDefinitions.turnsPerFight;
        }

        private void InitializeRun()
        {
            RunDefinitions runDefinitions = _gameConfiguration.runDefinitions;

            _runState.DiceInventory.Clear();
            _runState.MaxEquippedDice = runDefinitions.maxEquippedDice;
            _runState.RerollsPerTurn = runDefinitions.rerollsPerTurn;
            _runState.TurnsPerFight = runDefinitions.turnsPerFight;

            if (runDefinitions.startingDicePool != null)
            {
                foreach (var definition in runDefinitions.startingDicePool.diceDefinitions)
                {
                    _runState.DiceInventory.Add(new OwnedDiceData
                    {
                        Dice = new DiceData
                        {
                            Id = Guid.NewGuid().ToString(),
                            Definition = definition
                        },
                        IsEquipped = true
                    });
                }
            }

            _repository.SaveRun(_runState);
        }
    }
}
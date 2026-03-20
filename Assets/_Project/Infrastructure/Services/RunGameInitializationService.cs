using System;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;

namespace _Project.Infrastructure.Services
{
    public class RunGameInitializationService : IRunInitializationUseCase
    {
        private readonly IRunRepository _repository;
        private readonly DiceDefinition _startingDiceDefinition;
        private readonly PlayerRunState _runState;

        public RunGameInitializationService(IRunRepository repository, DiceDefinition startingDiceDefinition, PlayerRunState runState)
        {
            _repository = repository;
            _startingDiceDefinition = startingDiceDefinition;
            _runState = runState;
        }

        public void EnsureRunInitialized()
        {
            // DEBUG (use to reset game state during development)
            // PlayerPrefs.DeleteAll();
            // return;

            if (_repository.HasActiveRun())
            {
                // TODO: Move to separate function and name it "LoadRun"
                var loadedState = _repository.LoadRun();
                _runState.Inventory = loadedState.Inventory;
                _runState.MaxEquippedDice = loadedState.MaxEquippedDice;
                return;
            }

            _runState.Inventory.Clear();

            // TODO: Move to separate function and name it "InitializeRun"
            for (int i = 0; i < 5; i++)
            {
                _runState.Inventory.Add(new OwnedDiceData
                {
                    Id = Guid.NewGuid().ToString(),
                    Definition = _startingDiceDefinition,
                    Level = 1,
                    IsEquipped = true
                });
            }

            _repository.SaveRun(_runState);
        }
    }
}
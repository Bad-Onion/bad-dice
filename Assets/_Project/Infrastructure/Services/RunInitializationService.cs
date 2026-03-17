using System;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;

namespace _Project.Infrastructure.Services
{
    public class RunInitializationService : IRunInitializationUseCase
    {
        private readonly IRunRepository _repository;
        private readonly DiceDefinition _startingDiceDefinition;
        private readonly PlayerRunState _runState;

        public RunInitializationService(IRunRepository repository, DiceDefinition startingDiceDefinition, PlayerRunState runState)
        {
            _repository = repository;
            _startingDiceDefinition = startingDiceDefinition;
            _runState = runState;
        }

        public void EnsureRunInitialized()
        {
            if (_repository.HasActiveRun())
            {
                var loadedState = _repository.LoadRun();
                _runState.Inventory = loadedState.Inventory;
                _runState.MaxEquippedDice = loadedState.MaxEquippedDice;
                return;
            }

            _runState.Inventory.Clear();

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
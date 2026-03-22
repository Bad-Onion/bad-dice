using System;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities.DiceData;
using _Project.Domain.Entities.Session;
using _Project.Domain.ScriptableObjects.DiceDefinitions;

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
            _runState.Inventory = runState.Inventory;
            _runState.MaxEquippedDice = runState.MaxEquippedDice;
        }

        private void InitializeRun()
        {
            _runState.Inventory.Clear();

            // TODO: Load from a scriptable object called "Starting Dice Pool" or something like that, which can be configured per ritual or run type
            for (int i = 0; i < 5; i++)
            {
                _runState.Inventory.Add(new OwnedDiceData
                {
                    Dice = new DiceData
                    {
                        Id = Guid.NewGuid().ToString(),
                        Definition = _startingDiceDefinition
                    },
                    IsEquipped = true
                });
            }

            _repository.SaveRun(_runState);
        }
    }
}
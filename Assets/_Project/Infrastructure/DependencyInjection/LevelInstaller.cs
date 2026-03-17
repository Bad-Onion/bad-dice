using System;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;
using _Project.Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace _Project.Infrastructure.DependencyInjection
{
    public class LevelInstaller : MonoInstaller
    {
        [Header("Configurations")]
        [Tooltip("Assign the generic physics/spacing configuration.")]
        [SerializeField] private DiceConfiguration diceConfiguration;

        [Tooltip("Assign the default DiceDefinition the player starts the run with.")]
        [SerializeField] private DiceDefinition startingDiceDefinition;

        public override void InstallBindings()
        {
            // ScriptableObjects Configurations
            Container.BindInstance(diceConfiguration).AsSingle();


            // Domain Entities
            Container.Bind<PlayerRunState>().FromMethod(context => CreateMockRunState()).AsSingle();
            Container.Bind<DiceSession>().AsSingle();

            // Services
            Container.Bind<IDiceSimulationService>().To<DiceSimulationService>().AsSingle();
            Container.Bind<IDiceRollUseCase>().To<DiceRollService>().AsSingle();

            // Startup Initialization
            Container.BindInterfacesTo<GameInitializationService>().AsSingle();
        }

        private PlayerRunState CreateMockRunState()
        {
            var state = new PlayerRunState();

            // Mocking the player's inventory before the fight
            for (int i = 0; i < state.MaxEquippedDice; i++)
            {
                state.Inventory.Add(new OwnedDiceData
                {
                    Id = Guid.NewGuid().ToString(),
                    Definition = startingDiceDefinition,
                    Level = 1,
                    IsEquipped = true
                });
            }

            return state;
        }
    }
}
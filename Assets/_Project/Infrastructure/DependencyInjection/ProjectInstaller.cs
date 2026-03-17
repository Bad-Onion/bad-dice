using Zenject;
using UnityEngine;
using UnityEngine.InputSystem;
using _Project.Application.Commands;
using _Project.Application.Events;
using _Project.Application.Interfaces;
using _Project.Application.States.GameState;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;
using _Project.Infrastructure.Adapters;
using _Project.Infrastructure.Services;

namespace _Project.Infrastructure.DependencyInjection
{
    public class ProjectInstaller : MonoInstaller
    {
        [Header("Event Channels")]
        [Tooltip("Assign the global event channel for game state changes.")]
        [SerializeField] private GameStateEventChannel gameStateEventChannel;

        [Tooltip("Assign the global event channel for transitions.")]
        [SerializeField] private TransitionEventChannel transitionEventChannel;

        [Header("Input")]
        [Tooltip("Assign the input action to pause the game.")]
        [SerializeField] private InputActionReference pauseInputAction;

        [Header("Run Configuration")]
        [Tooltip("Assign the generic physics/spacing configuration.")]
        [SerializeField] private DiceConfiguration diceConfiguration;

        [Tooltip("Assign the default DiceDefinition the player starts the run with.")]
        [SerializeField] private DiceDefinition startingDiceDefinition;

        [Tooltip("Assign the DiceDatabase containing all available dice definitions.")]
        [SerializeField] private DiceDatabase diceDatabase;

        public override void InstallBindings()
        {
            // Global Event Channels
            Container.BindInstance(gameStateEventChannel).AsSingle();
            Container.BindInstance(transitionEventChannel).AsSingle();

            // Run State & Persistence (Cross-Scene)
            Container.BindInstance(diceConfiguration).AsSingle();
            Container.BindInstance(startingDiceDefinition).AsSingle();
            Container.BindInstance(diceDatabase).AsSingle();
            Container.Bind<PlayerRunState>().AsSingle();
            Container.Bind<IRunRepository>().To<PlayerPrefsRunRepository>().AsSingle();
            Container.Bind<IRunInitializationUseCase>().To<RunInitializationService>().AsSingle();

            // Core Services
            Container.Bind<ITimeService>().To<UnityTimeAdapter>().AsSingle();
            Container.Bind<ISceneLoader>().To<UnitySceneLoader>().AsSingle();

            // Commands
            Container.Bind<CommandProcessor>().AsSingle();
            Container.BindFactory<LevelData, System.Action, LoadLevelCommand, LoadLevelCommand.Factory>().AsSingle();
            Container.BindFactory<LevelData, System.Action, UnloadLevelCommand, UnloadLevelCommand.Factory>().AsSingle();

            // Game State Machine
            Container.Bind<IGameState>().To<BootstrapState>().AsSingle();
            Container.Bind<IGameState>().To<MainMenuState>().AsSingle();
            Container.Bind<IGameState>().To<PlayingState>().AsSingle();
            Container.Bind<IGameState>().To<PausedState>().AsSingle();

            Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();

            // Input
            Container.BindInstance(pauseInputAction).WhenInjectedInto<InputAdapter>();
            Container.BindInterfacesTo<InputAdapter>().AsSingle();
        }
    }
}

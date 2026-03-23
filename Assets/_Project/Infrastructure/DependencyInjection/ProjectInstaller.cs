using Zenject;
using UnityEngine;
using UnityEngine.InputSystem;
using _Project.Application.Commands;
using _Project.Application.Events.EventChannels;
using _Project.Application.Interfaces;
using _Project.Application.States.GameState;
using _Project.Application.UseCases;
using _Project.Domain.Entities.Session;
using _Project.Domain.ScriptableObjects.Configuration;
using _Project.Domain.ScriptableObjects.DiceDefinitions;
using _Project.Domain.ScriptableObjects.GameSettings;
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

        // TODO: Turn into a toggleable list of input actions for better scalability and flexibility.
        [Header("Input")]
        [Tooltip("Assign the input action to pause the game.")]
        [SerializeField] private InputActionReference pauseInputAction;
        [Tooltip("Assign the input action to interact with the game, usually the mouse left button.")]
        [SerializeField] private InputActionReference interactInputAction;
        [Tooltip("Assign the input action to get the mouse position.")]
        [SerializeField] private InputActionReference pointerPositionAction;
        [Tooltip("Assign the input action to hold the interact action.")]
        [SerializeField] private InputActionReference holdInteractAction;

        [Header("Run Configuration")]
        [Tooltip("Assign the generic physics/spacing configuration.")]
        [SerializeField] private DiceRollConfiguration diceRollConfiguration;

        [Tooltip("Assign the game configuration containing run settings.")]
        [SerializeField] private GameConfiguration gameConfiguration;

        [Tooltip("Assign the DiceDatabase containing all available dice definitions.")]
        [SerializeField] private DiceDatabase diceDatabase;

        public override void InstallBindings()
        {
            // Global Event Channels
            Container.BindInstance(gameStateEventChannel).AsSingle();
            Container.BindInstance(transitionEventChannel).AsSingle();

            // Entities
            Container.Bind<PlayerRunState>().AsSingle();
            Container.Bind<GameSession>().AsSingle();

            // Run State & Persistence (Cross-Scene)
            Container.BindInstance(diceRollConfiguration).AsSingle();
            Container.BindInstance(gameConfiguration).AsSingle();
            Container.BindInstance(diceDatabase).AsSingle();
            Container.Bind<IRunRepository>().To<PlayerPrefsRunRepository>().AsSingle();
            Container.Bind<IRunInitializationUseCase>().To<RunGameInitializationService>().AsSingle();

            // Core Services
            Container.BindInterfacesTo<GameFlowService>().AsSingle();
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
            Container.BindInterfacesTo<InputAdapter>()
                .AsSingle()
                .WithArguments(pauseInputAction, interactInputAction, holdInteractAction, pointerPositionAction);
        }
    }
}

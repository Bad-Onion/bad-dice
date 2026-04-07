using Zenject;
using UnityEngine;
using _Project.Application.Commands;
using _Project.Application.Events.EventChannels;
using _Project.Application.Interfaces;
using _Project.Application.States.GameState;
using _Project.Application.UseCases;
using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.Dice.Session;
using _Project.Domain.Features.Dice.ScriptableObjects.Configuration;
using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;
using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;
using _Project.Domain.Features.GameFlow.Session;
using _Project.Domain.Features.Run.Session;
using _Project.Infrastructure.Features.Combat.Health;
using _Project.Infrastructure.Features.Combat.Orchestration;
using _Project.Infrastructure.Features.Combat.Progression;
using _Project.Infrastructure.Features.Commands;
using _Project.Infrastructure.Features.GameFlow;
using _Project.Infrastructure.Features.Run.Persistence;
using _Project.Infrastructure.Features.Scene.Loading;
using _Project.Infrastructure.Shared.Adapters;

namespace _Project.Infrastructure.DependencyInjection
{
    /// <summary>
    /// The ProjectInstaller is responsible for setting up all dependency bindings for the project scope.
    /// </summary>
    public class ProjectInstaller : MonoInstaller
    {
        [Header("Event Channels")]
        [Tooltip("Assign the global event channel for game state changes.")]
        [SerializeField] private GameStateEventChannel gameStateEventChannel;

        [Tooltip("Assign the global event channel for transitions.")]
        [SerializeField] private TransitionEventChannel transitionEventChannel;

        [Header("Run Configuration")]
        [Tooltip("Assign the generic physics/spacing configuration.")]
        [SerializeField] private DiceRollConfiguration diceRollConfiguration;

        [Tooltip("Assign the game configuration containing run settings.")]
        [SerializeField] private GameConfiguration gameConfiguration;

        [Tooltip("Assign the DiceDatabase containing all available dice definitions.")]
        [SerializeField] private DiceDatabase diceDatabase;

        [Header("Input")]
        [Tooltip("Assign the input reader ScriptableObject for handling player input.")]
        [SerializeField] private InputReader inputReader;

        public override void InstallBindings()
        {
            // Global Event Channels
            Container.BindInstance(gameStateEventChannel).AsSingle();
            Container.BindInstance(transitionEventChannel).AsSingle();

            // Input
            Container.BindInstance(inputReader).AsSingle();

            // Entities
            Container.Bind<PlayerRunState>().AsSingle();
            Container.Bind<GameSession>().AsSingle();
            Container.Bind<CombatSessionState>().AsSingle();
            Container.Bind<EnemyEncounterState>().AsSingle();
            Container.Bind<DiceSessionState>().AsSingle();

            // Run State & Persistence (Cross-Scene)
            Container.BindInstance(diceRollConfiguration).AsSingle();
            Container.BindInstance(gameConfiguration).AsSingle();
            Container.BindInstance(diceDatabase).AsSingle();
            Container.Bind<IRunRepository>().To<PlayerPrefsRunRepository>().AsSingle();
            Container.Bind<IRunStateBuilder>().To<RunStateBuilder>().AsSingle();
            Container.Bind<IRunInitializationUseCase>().To<RunGameInitializationService>().AsSingle();
            Container.Bind<IEncounterProgressionUseCase>().To<EncounterProgressionService>().AsSingle();
            Container.Bind<IEnemyHealthUseCase>().To<EnemyHealthService>().AsSingle();
            Container.BindInterfacesTo<EncounterFlowCoordinator>().AsSingle();

            // Core Services
            Container.BindInterfacesTo<GameFlowService>().AsSingle();
            Container.Bind<ITimeService>().To<UnityTimeAdapter>().AsSingle();
            Container.Bind<ISceneLoader>().To<UnitySceneLoader>().AsSingle();

            // Commands
            Container.Bind<CommandProcessor>().AsSingle();
            Container.Bind<ICommandMiddleware>().To<CommandLoggingMiddleware>().AsSingle();
            Container.BindFactory<LevelData, System.Action, LoadLevelCommand, LoadLevelCommand.Factory>().AsSingle();
            Container.BindFactory<LevelData, System.Action, UnloadLevelCommand, UnloadLevelCommand.Factory>().AsSingle();

            // Game State Machine
            Container.Bind<IGameState>().To<BootstrapState>().AsSingle();
            Container.Bind<IGameState>().To<MainMenuState>().AsSingle();
            Container.Bind<IGameState>().To<PlayingState>().AsSingle();
            Container.Bind<IGameState>().To<PausedState>().AsSingle();

            Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();
        }
    }
}

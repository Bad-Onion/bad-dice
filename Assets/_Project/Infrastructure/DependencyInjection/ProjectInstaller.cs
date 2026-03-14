using Zenject;
using UnityEngine;
using UnityEngine.InputSystem;
using _Project.Application.Commands;
using _Project.Application.Events;
using _Project.Application.Interfaces;
using _Project.Application.States.GameState;
using _Project.Infrastructure.Adapters;
using _Project.Presentation.Scripts.Controllers;

namespace _Project.Infrastructure.DependencyInjection
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameStateEventChannel gameStateEventChannel;
        [SerializeField] private TransitionEventChannel transitionEventChannel;
        [SerializeField] private InputActionReference pauseInputAction;

        public override void InstallBindings()
        {
            // Global Event Channels
            Container.BindInstance(gameStateEventChannel).AsSingle();
            Container.BindInstance(transitionEventChannel).AsSingle();

            // Core Services
            Container.Bind<ITimeService>().To<UnityTimeAdapter>().AsSingle();
            Container.Bind<ISceneLoader>().To<UnitySceneLoader>().AsSingle();

            // Commands
            Container.Bind<CommandProcessor>().AsSingle();
            Container.BindFactory<Domain.ScriptableObjects.LevelData, System.Action, LoadLevelCommand, LoadLevelCommand.Factory>().AsSingle();
            Container.BindFactory<Domain.ScriptableObjects.LevelData, System.Action, UnloadLevelCommand, UnloadLevelCommand.Factory>().AsSingle();

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

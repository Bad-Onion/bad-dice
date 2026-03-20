using System;
using _Project.Application.Commands;
using _Project.Application.Events.Core;
using _Project.Application.Events.EventChannels;
using _Project.Application.Events.EventChannels.Payload;
using _Project.Application.Events.Load;
using _Project.Application.Interfaces;
using _Project.Application.States.GameState;
using _Project.Application.UseCases;
using _Project.Domain.Entities.Session;
using _Project.Domain.ScriptableObjects.GameSettings;
using Zenject;

namespace _Project.Infrastructure.Services
{
    public class GameFlowService : IGameFlowUseCase, IInitializable, IDisposable
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IInputProvider _inputProvider;
        private readonly TransitionEventChannel _transitionEventChannel;
        private readonly CommandProcessor _commandProcessor;
        private readonly LoadLevelCommand.Factory _loadLevelCommandFactory;
        private readonly UnloadLevelCommand.Factory _unloadLevelCommandFactory;
        private readonly GameSession _gameSession;

        public GameFlowService(
            IGameStateMachine stateMachine,
            IInputProvider inputProvider,
            TransitionEventChannel transitionEventChannel,
            LoadLevelCommand.Factory loadLevelCommandFactory,
            CommandProcessor commandProcessor,
            UnloadLevelCommand.Factory unloadLevelCommandFactory,
            GameSession gameSession,
            GameConfiguration gameConfiguration)
        {
            _gameStateMachine = stateMachine;
            _inputProvider = inputProvider;
            _transitionEventChannel = transitionEventChannel;
            _commandProcessor = commandProcessor;
            _loadLevelCommandFactory = loadLevelCommandFactory;
            _unloadLevelCommandFactory = unloadLevelCommandFactory;
            _gameSession = gameSession;

            _gameSession.CurrentLevelData = gameConfiguration.defaultLevelData;
        }

        public void Initialize()
        {
            Bus<BootstrapReadyEvent>.OnEvent += HandleBootstrapReady;
            _inputProvider.OnPauseAction += TogglePause;

            _transitionEventChannel.RaiseEvent(new TransitionPayload(true, 0f));
            _gameStateMachine.ChangeState<BootstrapState>();
        }

        public void Dispose()
        {
            Bus<BootstrapReadyEvent>.OnEvent -= HandleBootstrapReady;
            _inputProvider.OnPauseAction -= TogglePause;
        }

        private void HandleBootstrapReady(BootstrapReadyEvent evt)
        {
            RequestStateChange<MainMenuState>();
        }

        private void RequestStateChange<TState>(bool useTransition = true) where TState : class, IGameState
        {
            if (useTransition)
            {
                ExecuteWithTransition(onComplete =>
                {
                    _gameStateMachine.ChangeState<TState>();
                    onComplete?.Invoke();
                });
            }
            else
            {
                _gameStateMachine.ChangeState<TState>();
            }
        }

        public void StartGameFromMenu()
        {
            ExecuteWithTransition(onComplete =>
            {
                var loadCommand = _loadLevelCommandFactory.Create(_gameSession.CurrentLevelData, () =>
                {
                    _gameSession.IsLevelLoaded = true;
                    _gameStateMachine.ChangeState<PlayingState>();
                    onComplete?.Invoke();
                });

                _commandProcessor.ExecuteCommand(loadCommand);
            });
        }

        public void ResumeGame() => RequestStateChange<PlayingState>(false);

        public void PauseGame() => RequestStateChange<PausedState>(false);

        public void ReturnToMenu()
        {
            ExecuteWithTransition(onComplete =>
            {
                Action completeTransition = () =>
                {
                    _gameStateMachine.ChangeState<MainMenuState>();
                    onComplete?.Invoke();
                };

                if (_gameSession.IsLevelLoaded)
                {
                    var unloadCommand = _unloadLevelCommandFactory.Create(_gameSession.CurrentLevelData, () =>
                    {
                        _gameSession.IsLevelLoaded = false;
                        completeTransition();
                    });

                    _commandProcessor.ExecuteCommand(unloadCommand);
                }
                else
                {
                    completeTransition();
                }
            });
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void TogglePause()
        {
            if (_gameStateMachine.CurrentStateType == typeof(PlayingState)) PauseGame();
            else if (_gameStateMachine.CurrentStateType == typeof(PausedState)) ResumeGame();
        }

        private void ExecuteWithTransition(Action<Action> midTransitionAction)
        {
            _transitionEventChannel.RaiseEvent(new TransitionPayload(true, 0.5f, () =>
            {
                midTransitionAction?.Invoke(() =>
                    _transitionEventChannel.RaiseEvent(new TransitionPayload(false, 0.5f))
                );
            }));
        }
    }
}
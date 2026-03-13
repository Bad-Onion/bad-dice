using UnityEngine;
using Zenject;
using System;
using _Project.Application.Commands;
using _Project.Application.Events;
using _Project.Application.Events.Payload;
using _Project.Application.Interfaces;
using _Project.Application.States.GameState;
using _Project.Domain.ScriptableObjects;

namespace _Project.Presentation.Scripts.Controllers
{
    public class GameController : MonoBehaviour, IDisposable
    {
        private IGameStateMachine _gameStateMachine;
        private IInputProvider _inputProvider;
        private TransitionEventChannel _transitionEventChannel;
        private CommandProcessor _commandProcessor;
        private LoadLevelCommand.Factory _loadLevelCommandFactory;
        private UnloadLevelCommand.Factory _unloadLevelCommandFactory;

        [Header("Level Configuration")]
        [SerializeField] private LevelData currentLevelData;

        private bool _isLevelLoaded = false;

        [Inject]
        public void Construct(
            IGameStateMachine stateMachine,
            IInputProvider inputProvider,
            TransitionEventChannel transitionEventChannel,
            CommandProcessor commandProcessor,
            LoadLevelCommand.Factory loadLevelCommandFactory,
            UnloadLevelCommand.Factory unloadLevelCommandFactory)
        {
            _gameStateMachine = stateMachine;
            _inputProvider = inputProvider;
            _transitionEventChannel = transitionEventChannel;
            _commandProcessor = commandProcessor;
            _loadLevelCommandFactory = loadLevelCommandFactory;
            _unloadLevelCommandFactory = unloadLevelCommandFactory;

            _inputProvider.OnPauseAction += TogglePause;
        }

        private void OnEnable()
        {
            Bus<BootstrapReadyEvent>.OnEvent += HandleBootstrapReady;
        }

        private void OnDisable()
        {
            Bus<BootstrapReadyEvent>.OnEvent -= HandleBootstrapReady;
        }

        private void Start()
        {
            _transitionEventChannel.RaiseEvent(new TransitionPayload(true, 0f));
            _gameStateMachine.ChangeState<BootstrapState>();
        }

        private void HandleBootstrapReady(BootstrapReadyEvent evt)
        {
            RequestStateChange<MainMenuState>();
        }

        public void RequestStateChange<TState>(bool useTransition = true) where TState : class, IGameState
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
                var loadCommand = _loadLevelCommandFactory.Create(currentLevelData, () =>
                {
                    _isLevelLoaded = true;
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

                if (_isLevelLoaded)
                {
                    var unloadCommand = _unloadLevelCommandFactory.Create(currentLevelData, () =>
                    {
                        _isLevelLoaded = false;
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

        private void TogglePause()
        {
            if (_gameStateMachine.CurrentStateType == typeof(PlayingState)) PauseGame();

            else if (_gameStateMachine.CurrentStateType == typeof(PausedState)) ResumeGame();
        }

        public void Dispose()
        {
            if (_inputProvider == null) return;

            _inputProvider.OnPauseAction -= TogglePause;
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
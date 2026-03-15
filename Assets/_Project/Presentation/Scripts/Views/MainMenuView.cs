using _Project.Application.UseCases;
using UnityEngine.UIElements;
using Zenject;

namespace _Project.Presentation.Scripts.Views
{
    public class MainMenuView : BaseStateView
    {
        private Button _startButton;
        private Button _quitButton;

        private IGameFlowUseCase _gameFlowUseCase;

        [Inject]
        public void Construct(IGameFlowUseCase gameFlowUseCase)
        {
            _gameFlowUseCase = gameFlowUseCase;
        }

        protected override void BindUIElements()
        {
            if (UiContainer == null) return;

            _startButton = UiContainer.Q<Button>("start-button");
            _quitButton = UiContainer.Q<Button>("quit-button");

            if (_startButton != null) _startButton.clicked += _gameFlowUseCase.StartGameFromMenu;
            if (_quitButton != null) _quitButton.clicked += _gameFlowUseCase.QuitGame;
        }

        protected override void UnbindUIElements()
        {
            if (_startButton != null) _startButton.clicked -= _gameFlowUseCase.StartGameFromMenu;
            if (_quitButton != null) _quitButton.clicked -= _gameFlowUseCase.QuitGame;
        }
    }
}
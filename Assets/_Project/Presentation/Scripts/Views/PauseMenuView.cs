using _Project.Application.UseCases;
using UnityEngine.UIElements;
using Zenject;

namespace _Project.Presentation.Scripts.Views
{
    public class PauseMenuView : BaseStateView
    {
        private Button _resumeButton;
        private Button _mainMenuButton;

        private IGameFlowUseCase _gameFlowUseCase;

        [Inject]
        public void Construct(IGameFlowUseCase gameFlowUseCase)
        {
            _gameFlowUseCase = gameFlowUseCase;
        }

        protected override void BindUIElements()
        {
            if (UiContainer == null) return;

            _resumeButton = UiContainer.Q<Button>("resume-button");
            _mainMenuButton = UiContainer.Q<Button>("main-menu-button");

            if (_resumeButton != null) _resumeButton.clicked += _gameFlowUseCase.ResumeGame;
            if (_mainMenuButton != null) _mainMenuButton.clicked += _gameFlowUseCase.ReturnToMenu;
        }

        protected override void UnbindUIElements()
        {
            if (_resumeButton != null) _resumeButton.clicked -= _gameFlowUseCase.ResumeGame;
            if (_mainMenuButton != null) _mainMenuButton.clicked -= _gameFlowUseCase.ReturnToMenu;
        }
    }
}
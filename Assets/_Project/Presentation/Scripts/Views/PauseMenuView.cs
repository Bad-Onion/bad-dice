using _Project.Presentation.Scripts.Controllers;
using UnityEngine.UIElements;
using Zenject;

namespace _Project.Presentation.Scripts.Views
{
    public class PauseMenuView : BaseStateView
    {
        private Button _resumeButton;
        private Button _mainMenuButton;

        private readonly GameController _gameController;

        [Inject]
        public PauseMenuView(GameController gameController)
        {
            _gameController = gameController;
        }

        protected override void BindUIElements()
        {
            if (UiContainer == null) return;

            _resumeButton = UiContainer.Q<Button>("resume-button");
            _mainMenuButton = UiContainer.Q<Button>("main-menu-button");

            if (_resumeButton != null) _resumeButton.clicked += _gameController.ResumeGame;
            if (_mainMenuButton != null) _mainMenuButton.clicked += _gameController.ReturnToMenu;
        }

        protected override void UnbindUIElements()
        {
            if (_resumeButton != null) _resumeButton.clicked -= _gameController.ResumeGame;
            if (_mainMenuButton != null) _mainMenuButton.clicked -= _gameController.ReturnToMenu;
        }
    }
}
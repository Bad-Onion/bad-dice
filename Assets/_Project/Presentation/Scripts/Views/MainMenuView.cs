using UnityEngine.UIElements;
using Zenject;
using _Project.Presentation.Scripts.Controllers;

namespace _Project.Presentation.Scripts.Views
{
    public class MainMenuView : BaseStateView
    {
        private Button _startButton;
        private Button _quitButton;

        private readonly GameController _gameController;

        [Inject]
        public MainMenuView(GameController gameController)
        {
            _gameController = gameController;
        }

        protected override void BindUIElements()
        {
            if (UiContainer == null) return;

            _startButton = UiContainer.Q<Button>("start-button");
            _quitButton = UiContainer.Q<Button>("quit-button");

            if (_startButton != null) _startButton.clicked += _gameController.StartGameFromMenu;
            if (_quitButton != null) _quitButton.clicked += _gameController.QuitGame;
        }

        protected override void UnbindUIElements()
        {
            if (_startButton != null) _startButton.clicked -= _gameController.StartGameFromMenu;
            if (_quitButton != null) _quitButton.clicked -= _gameController.QuitGame;
        }
    }
}
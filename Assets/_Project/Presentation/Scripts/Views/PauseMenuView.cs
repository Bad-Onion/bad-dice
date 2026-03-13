using _Project.Presentation.Scripts.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Presentation.Scripts.Views
{
    public class PauseMenuView : BaseStateView
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private GameController gameController;

        protected override void OnEnable()
        {
            base.OnEnable();

            resumeButton.onClick.AddListener(gameController.ResumeGame);
            mainMenuButton.onClick.AddListener(gameController.ReturnToMenu);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            resumeButton.onClick.RemoveListener(gameController.ResumeGame);
            mainMenuButton.onClick.RemoveListener(gameController.ReturnToMenu);
        }
    }
}
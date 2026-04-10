using _Project.Application.Interfaces;
using _Project.Presentation.Scripts.Features.DiceSession.Presenters;
using _Project.Presentation.Scripts.Shared.AbstractViews;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace _Project.Presentation.Scripts.Features.DiceSession.Views
{
    public class DiceSessionView : BaseView, IDiceSessionView
    {
        private Button _rollButton;
        private Button _resetButton;
        private Button _dealButton;
        private Label _resultLabel;
        private Label _enemyNameLabel;
        private Label _enemyHealthLabel;
        private Label _cycleLabel;
        private Label _turnLabel;
        private VisualElement _levelContainer;

        private DiceSessionPresenterFacade _presenterFacade;
        private bool _isUiInitialized;

        [Inject]
        public void Construct(DiceSessionPresenterFacade presenterFacade)
        {
            _presenterFacade = presenterFacade;
        }

        protected override void BindUIElements()
        {
            if (UiContainer == null) return;
            if (!TryCacheUiElements()) return;

            SubscribeUiActions();
            _presenterFacade.Attach(this);
            _presenterFacade.RefreshEnemyPanelFromState();
        }

        protected override void UnbindUIElements()
        {
            if (!_isUiInitialized) return;

            UnsubscribeUiActions();
            _presenterFacade.Detach();
            _isUiInitialized = false;
        }

        private bool TryCacheUiElements()
        {
            _levelContainer = UiContainer.Q<VisualElement>("level-container");
            _rollButton = UiContainer.Q<Button>("roll-button");
            _resetButton = UiContainer.Q<Button>("reset-button");
            _dealButton = UiContainer.Q<Button>("deal-button");
            _resultLabel = UiContainer.Q<Label>("result-label");
            _enemyNameLabel = UiContainer.Q<Label>("enemy-name-label");
            _enemyHealthLabel = UiContainer.Q<Label>("enemy-health-label");
            _cycleLabel = UiContainer.Q<Label>("cycle-label");
            _turnLabel = UiContainer.Q<Label>("turn-label");

            _isUiInitialized = _levelContainer != null &&
                               _rollButton != null &&
                               _resetButton != null &&
                               _dealButton != null &&
                               _resultLabel != null &&
                               _enemyNameLabel != null &&
                               _enemyHealthLabel != null &&
                               _cycleLabel != null &&
                               _turnLabel != null;

            if (_isUiInitialized) return true;

            Debug.LogError("DiceSessionView could not bind all required UI Toolkit elements.", this);
            return false;
        }

        private void SubscribeUiActions()
        {
            _rollButton.clicked += OnRollButtonClicked;
            _resetButton.clicked += OnResetButtonClicked;
            _dealButton.clicked += OnDealButtonClicked;
        }

        private void UnsubscribeUiActions()
        {
            _rollButton.clicked -= OnRollButtonClicked;
            _resetButton.clicked -= OnResetButtonClicked;
            _dealButton.clicked -= OnDealButtonClicked;
        }

        private void OnRollButtonClicked()
        {
            _presenterFacade.RequestRoll();
        }

        private void OnResetButtonClicked()
        {
            _presenterFacade.RequestReset();
        }

        private void OnDealButtonClicked()
        {
            _presenterFacade.RequestDealDamage();
        }

        public void SetEnemyInfo(string enemyNameText)
        {
            _enemyNameLabel.text = enemyNameText;
        }

        public void SetEnemyHealth(string enemyHealthText)
        {
            _enemyHealthLabel.text = enemyHealthText;
        }

        public void SetCycleInfo(string cycleText)
        {
            _cycleLabel.text = cycleText;
        }

        public void SetTurnInfo(string turnText)
        {
            _turnLabel.text = turnText;
        }

        public void SetResultInfo(string resultText)
        {
            _resultLabel.text = resultText;
        }

        public void SetDealButtonInteractable(bool isInteractable)
        {
            _dealButton.SetEnabled(isInteractable);
        }

        public void SetDicePanelVisible(bool isVisible)
        {
            _levelContainer.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
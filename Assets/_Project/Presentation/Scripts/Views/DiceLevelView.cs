using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using _Project.Application.UseCases;
using UnityEngine.UIElements;
using Zenject;

namespace _Project.Presentation.Scripts.Views
{
    public class DiceLevelView : BaseView
    {
        private Button _rollButton;
        private Button _resetButton;
        private Label _resultLabel;

        private IDiceRollUseCase _diceRollUseCase;

        [Inject]
        public void Construct(IDiceRollUseCase diceRollUseCase)
        {
            _diceRollUseCase = diceRollUseCase;
        }

        protected override void BindUIElements()
        {
            if (UiContainer == null) return;

            _rollButton = UiContainer.Q<Button>("roll-button");
            _resetButton = UiContainer.Q<Button>("reset-button");
            _resultLabel = UiContainer.Q<Label>("result-label");

            if (_rollButton != null) _rollButton.clicked += _diceRollUseCase.RequestRoll;
            if (_resetButton != null) _resetButton.clicked += _diceRollUseCase.ResetDice;

            Bus<DiceResultDecidedEvent>.OnEvent += OnResultDecided;
            Bus<DiceResetEvent>.OnEvent += OnDiceReset;
        }

        protected override void UnbindUIElements()
        {
            if (_rollButton != null) _rollButton.clicked -= _diceRollUseCase.RequestRoll;
            if (_resetButton != null) _resetButton.clicked -= _diceRollUseCase.ResetDice;

            Bus<DiceResultDecidedEvent>.OnEvent -= OnResultDecided;
            Bus<DiceResetEvent>.OnEvent -= OnDiceReset;
        }

        private void OnResultDecided(DiceResultDecidedEvent evt)
        {
            if (_resultLabel != null) _resultLabel.text = $"Results: {string.Join(", ", evt.Results)}";
        }

        private void OnDiceReset(DiceResetEvent evt)
        {
            if (_resultLabel != null) _resultLabel.text = "Waiting to roll...";
        }
    }
}
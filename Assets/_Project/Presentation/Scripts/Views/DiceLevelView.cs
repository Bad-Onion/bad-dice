using System.Text;
using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
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
        private DiceSession _diceSession;

        [Inject]
        public void Construct(IDiceRollUseCase diceRollUseCase, DiceSession diceSession)
        {
            _diceRollUseCase = diceRollUseCase;
            _diceSession = diceSession;
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
            if (_resultLabel == null) return;

            StringBuilder sb = new StringBuilder("Results: ");
            int totalEncounterDamage = 0;

            foreach (DiceState die in _diceSession.ActiveDice)
            {
                if (die.CurrentFaceIndex >= 0)
                {
                    sb.Append($"[{die.CurrentValue} (Lv{die.Level})] ");
                    totalEncounterDamage += die.TotalDamage;
                }
            }

            sb.Append($"\nTotal Damage: {totalEncounterDamage}");
            _resultLabel.text = sb.ToString();
        }

        private void OnDiceReset(DiceResetEvent evt)
        {
            if (_resultLabel != null) _resultLabel.text = "Waiting to roll...";
        }
    }
}
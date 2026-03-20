using System.Text;
using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using _Project.Application.Events.MergeEvents;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
using UnityEngine.UIElements;
using Zenject;

namespace _Project.Presentation.Scripts.Views
{
    // TODO: Rename to something more related to the dice session, like "DiceSessionView"
    public class DiceLevelView : BaseView
    {
        private Button _rollButton;
        private Button _resetButton;
        private Label _resultLabel;
        private VisualElement _levelContainer;

        private IDiceRollUseCase _diceRollUseCase;
        private IDiceMergeUseCase _diceMergeUseCase;
        private IDiceDamageService _damageService;
        private DiceSession _diceSession;

        [Inject]
        public void Construct(IDiceRollUseCase diceRollUseCase, DiceSession diceSession, IDiceDamageService damageService, IDiceMergeUseCase diceMergeUseCase)
        {
            _diceRollUseCase = diceRollUseCase;
            _diceSession = diceSession;
            _damageService = damageService;
            _diceMergeUseCase = diceMergeUseCase;
        }

        protected override void BindUIElements()
        {
            if (UiContainer == null) return;

            _levelContainer = UiContainer.Q<VisualElement>("level-container");
            _rollButton = UiContainer.Q<Button>("roll-button");
            _resetButton = UiContainer.Q<Button>("reset-button");
            _resultLabel = UiContainer.Q<Label>("result-label");

            // TODO: Create a new event for requesting a roll instead of directly calling the use case method, to keep the view more decoupled from the application layer.
            if (_rollButton != null) _rollButton.clicked += _diceRollUseCase.RequestRoll;
            // TODO: Use the DiceResetEvent instead of directly calling the use case method, to keep the view more decoupled from the application layer.
            if (_resetButton != null) _resetButton.clicked += _diceRollUseCase.ResetDice;

            Bus<EncounterStartedEvent>.OnEvent += OnEncounterStarted;
            Bus<DiceResultDecidedEvent>.OnEvent += OnResultDecided;
            Bus<DiceRollFinishedEvent>.OnEvent += OnRollFinished;
            Bus<DiceResetEvent>.OnEvent += OnDiceReset;
            Bus<MergeCompletedEvent>.OnEvent += OnMergeCompleted;
        }

        protected override void UnbindUIElements()
        {
            // TODO: Create a new event for requesting a roll instead of directly calling the use case method, to keep the view more decoupled from the application layer.
            if (_rollButton != null) _rollButton.clicked -= _diceRollUseCase.RequestRoll;
            // TODO: Use the DiceResetEvent instead of directly calling the use case method, to keep the view more decoupled from the application layer.
            if (_resetButton != null) _resetButton.clicked -= _diceRollUseCase.ResetDice;

            Bus<EncounterStartedEvent>.OnEvent -= OnEncounterStarted;
            Bus<DiceResultDecidedEvent>.OnEvent -= OnResultDecided;
            Bus<DiceRollFinishedEvent>.OnEvent -= OnRollFinished;
            Bus<DiceResetEvent>.OnEvent -= OnDiceReset;
            Bus<MergeCompletedEvent>.OnEvent -= OnMergeCompleted;
        }

        private void OnEncounterStarted(EncounterStartedEvent evt)
        {
            if (_levelContainer != null) _levelContainer.style.display = DisplayStyle.Flex;
            if (_resultLabel != null) _resultLabel.text = "Ready to roll.";
        }

        private void OnRollFinished(DiceRollFinishedEvent evt)
        {
            if (_resultLabel == null) return;

            StringBuilder sb = new StringBuilder($"Rerolls Left: {_diceSession.RerollsLeft}\n\nResults: ");
            int totalEncounterDamage = 0;

            foreach (DiceState die in _diceSession.ActiveDice)
            {
                if (die.CurrentFaceIndex >= 0)
                {
                    int damage = _damageService.CalculateDamage(die);
                    sb.Append($"[{die.CurrentValue} (Lv{die.Level})] ");
                    totalEncounterDamage += damage;
                }
            }

            sb.Append($"\nTotal Damage: {totalEncounterDamage}");
            _resultLabel.text = sb.ToString();
        }

        private void OnResultDecided(DiceResultDecidedEvent evt)
        {
            if (_resultLabel == null) return;

            StringBuilder sb = new StringBuilder($"Rerolls Left: {_diceSession.RerollsLeft}\n\nResults: ");
            int totalEncounterDamage = 0;

            foreach (DiceState die in _diceSession.ActiveDice)
            {
                if (die.CurrentFaceIndex >= 0)
                {
                    int damage = _damageService.CalculateDamage(die);
                    sb.Append($"[{die.CurrentValue} (Lv{die.Level})] ");
                    totalEncounterDamage += damage;
                }
            }

            sb.Append($"\nTotal Damage: {totalEncounterDamage}");
            _resultLabel.text = sb.ToString();
        }

        private void OnDiceReset(DiceResetEvent evt)
        {
            if (_resultLabel != null) _resultLabel.text = "Waiting to roll...";
        }

        private void OnMergeCompleted(MergeCompletedEvent evt)
        {
            OnRollFinished(new DiceRollFinishedEvent());
            _diceMergeUseCase.EvaluateMergePossibilities();
        }
    }
}
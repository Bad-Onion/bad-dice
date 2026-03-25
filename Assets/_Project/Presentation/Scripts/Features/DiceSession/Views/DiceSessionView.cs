using _Project.Application.Events.Core;
using _Project.Application.Events.DiceInput;
using _Project.Application.Events.DiceSimulation;
using _Project.Application.Events.DiceState;
using _Project.Application.Events.EncounterState;
using _Project.Application.Events.GameState;
using _Project.Domain.Features.Combat.Enums;
using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Dice.Session;
using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;
using _Project.Presentation.Scripts.Shared.AbstractViews;
using UnityEngine.UIElements;
using Zenject;

namespace _Project.Presentation.Scripts.Features.DiceSession.Views
{
    public class DiceSessionView : BaseView
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

        private DiceSessionState _diceSessionState;
        private EnemyEncounterState _enemyEncounterState;
        private GameConfiguration _gameConfiguration;

        [Inject]
        public void Construct(
            DiceSessionState diceSessionState,
            EnemyEncounterState enemyEncounterState,
            GameConfiguration gameConfiguration)
        {
            _diceSessionState = diceSessionState;
            _enemyEncounterState = enemyEncounterState;
            _gameConfiguration = gameConfiguration;
        }

        protected override void BindUIElements()
        {
            if (UiContainer == null) return;

            _levelContainer = UiContainer.Q<VisualElement>("level-container");
            _rollButton = UiContainer.Q<Button>("roll-button");
            _resetButton = UiContainer.Q<Button>("reset-button");
            _dealButton = UiContainer.Q<Button>("deal-button");
            _resultLabel = UiContainer.Q<Label>("result-label");
            _enemyNameLabel = UiContainer.Q<Label>("enemy-name-label");
            _enemyHealthLabel = UiContainer.Q<Label>("enemy-health-label");
            _cycleLabel = UiContainer.Q<Label>("cycle-label");
            _turnLabel = UiContainer.Q<Label>("turn-label");

            if (_rollButton != null) _rollButton.clicked += RaiseRollRequested;
            if (_resetButton != null) _resetButton.clicked += RaiseResetRequested;
            if (_dealButton != null) _dealButton.clicked += HandleDealClicked;

            Bus<EncounterStartedEvent>.OnEvent += OnEncounterStarted;
            Bus<EncounterPreparedEvent>.OnEvent += OnEncounterPrepared;
            Bus<EnemyDamagedEvent>.OnEvent += OnEnemyDamaged;
            Bus<EnemyDefeatedEvent>.OnEvent += OnEnemyDefeated;
            Bus<RunCompletedEvent>.OnEvent += OnRunCompleted;
            Bus<TurnChangedEvent>.OnEvent += OnTurnChanged;
            Bus<DiceResultDecidedEvent>.OnEvent += OnResultDecided;
            Bus<DiceRollFinishedEvent>.OnEvent += OnRollFinished;
            Bus<DiceResetEvent>.OnEvent += OnDiceReset;
            Bus<MergeCompletedEvent>.OnEvent += OnMergeCompleted;
            Bus<DiceHoverDetailsUpdatedEvent>.OnEvent += OnDiceHoverDetailsUpdated;

            RefreshEnemyPanelFromState();
        }

        protected override void UnbindUIElements()
        {
            if (_rollButton != null) _rollButton.clicked -= RaiseRollRequested;
            if (_resetButton != null) _resetButton.clicked -= RaiseResetRequested;
            if (_dealButton != null) _dealButton.clicked -= HandleDealClicked;

            Bus<EncounterStartedEvent>.OnEvent -= OnEncounterStarted;
            Bus<EncounterPreparedEvent>.OnEvent -= OnEncounterPrepared;
            Bus<EnemyDamagedEvent>.OnEvent -= OnEnemyDamaged;
            Bus<EnemyDefeatedEvent>.OnEvent -= OnEnemyDefeated;
            Bus<RunCompletedEvent>.OnEvent -= OnRunCompleted;
            Bus<TurnChangedEvent>.OnEvent -= OnTurnChanged;
            Bus<DiceResultDecidedEvent>.OnEvent -= OnResultDecided;
            Bus<DiceRollFinishedEvent>.OnEvent -= OnRollFinished;
            Bus<DiceResetEvent>.OnEvent -= OnDiceReset;
            Bus<MergeCompletedEvent>.OnEvent -= OnMergeCompleted;
            Bus<DiceHoverDetailsUpdatedEvent>.OnEvent -= OnDiceHoverDetailsUpdated;
        }

        private void OnEncounterStarted(EncounterStartedEvent evt)
        {
            if (_levelContainer != null) _levelContainer.style.display = DisplayStyle.Flex;
            if (_resultLabel != null) _resultLabel.text = "Ready to roll.";

            UpdateTurnLabel(_diceSessionState.CurrentTurn, _diceSessionState.MaxTurns);
            UpdateDealButtonInteractable(false);
        }

        private void OnEncounterPrepared(EncounterPreparedEvent evt)
        {
            if (_enemyNameLabel != null)
            {
                _enemyNameLabel.text = $"Enemy: {evt.EnemyName} ({GetEncounterTypeText(GetEncounterType(evt.IsBoss, evt.IsFinalBoss))})";
            }

            if (_enemyHealthLabel != null) _enemyHealthLabel.text = $"HP: {evt.CurrentHealth}/{evt.MaxHealth}";
            if (_cycleLabel != null) _cycleLabel.text = $"Cycle {evt.CycleNumber} - Encounter {evt.EncounterIndexInCycle}/{GetMaxEncountersPerCycle()}";
        }

        private void OnEnemyDamaged(EnemyDamagedEvent evt)
        {
            if (_enemyHealthLabel != null) _enemyHealthLabel.text = $"HP: {evt.RemainingHealth}/{evt.MaxHealth}";
        }

        private void OnEnemyDefeated(EnemyDefeatedEvent evt)
        {
            if (_enemyHealthLabel != null) _enemyHealthLabel.text = "HP: 0/0";
            if (_resultLabel != null) _resultLabel.text = $"{evt.EnemyName} defeated. Preparing next encounter...";

            UpdateDealButtonInteractable(false);
        }

        private void OnRunCompleted(RunCompletedEvent evt)
        {
            if (_resultLabel != null) _resultLabel.text = "Lucifer has fallen. Run completed!";
            UpdateDealButtonInteractable(false);
        }

        private void OnTurnChanged(TurnChangedEvent evt)
        {
            UpdateTurnLabel(evt.CurrentTurn, evt.MaxTurns);
            UpdateDealButtonInteractable(false);
        }

        private void OnRollFinished(DiceRollFinishedEvent evt)
        {
            UpdateRollResultText();
            UpdateDealButtonInteractable(CanDealThisTurn());
        }

        private void OnResultDecided(DiceResultDecidedEvent evt)
        {
            UpdateRollResultText();
            UpdateDealButtonInteractable(false);
        }

        private void OnDiceReset(DiceResetEvent evt)
        {
            if (_resultLabel != null) _resultLabel.text = "Waiting to roll...";
            UpdateDealButtonInteractable(false);
        }

        private void OnMergeCompleted(MergeCompletedEvent evt)
        {
            OnRollFinished(new DiceRollFinishedEvent());
        }

        private void HandleDealClicked()
        {
            Bus<DealDamageRequestedEvent>.Raise(new DealDamageRequestedEvent());
            UpdateDealButtonInteractable(false);
        }

        private void UpdateRollResultText()
        {
            if (_resultLabel == null) return;

            _resultLabel.text = $"Rerolls Left: {_diceSessionState.RerollsLeft}\nHover a die to inspect Value, Level and Damage.";
        }

        private void OnDiceHoverDetailsUpdated(DiceHoverDetailsUpdatedEvent evt)
        {
            if (_resultLabel == null) return;

            if (!evt.HasDetails)
            {
                UpdateRollResultText();
                return;
            }

            _resultLabel.text =
                $"Dice {evt.DiceId}\n" +
                $"Value: {evt.CurrentValue}\n" +
                $"Level: {evt.Level}\n" +
                $"Damage: {evt.Damage}";
        }

        private void RefreshEnemyPanelFromState()
        {
            if (_enemyEncounterState?.CurrentEncounter == null) return;

            var currentEncounter = _enemyEncounterState.CurrentEncounter;
            int maxEncountersPerCycle = GetMaxEncountersPerCycle();

            if (_enemyNameLabel != null)
            {
                _enemyNameLabel.text = $"Enemy: {currentEncounter.EnemyName} ({GetEncounterTypeText(currentEncounter.EncounterType)})";
            }

            if (_enemyHealthLabel != null)
            {
                _enemyHealthLabel.text = $"HP: {_enemyEncounterState.CurrentHealth}/{currentEncounter.MaxHealth}";
            }

            if (_cycleLabel != null)
            {
                _cycleLabel.text = $"Cycle {currentEncounter.CycleNumber} - Encounter {currentEncounter.EncounterIndexInCycle}/{maxEncountersPerCycle}";
            }

            UpdateTurnLabel(_diceSessionState.CurrentTurn, _diceSessionState.MaxTurns);
            UpdateDealButtonInteractable(CanDealThisTurn());
        }

        private void UpdateTurnLabel(int currentTurn, int maxTurns)
        {
            if (_turnLabel == null) return;

            _turnLabel.text = $"Turn: {currentTurn}/{maxTurns}";
        }

        private bool CanDealThisTurn()
        {
            if (_diceSessionState.HasDealtThisTurn) return false;
            if (_diceSessionState.CurrentTurn > _diceSessionState.MaxTurns) return false;

            foreach (DiceState die in _diceSessionState.ActiveDice)
            {
                if (die.CurrentFaceIndex >= 0) return true;
            }

            return false;
        }

        private void UpdateDealButtonInteractable(bool isInteractable)
        {
            _dealButton?.SetEnabled(isInteractable);
        }

        private int GetMaxEncountersPerCycle()
        {
            int minorEncountersPerCycle = _gameConfiguration?.enemyProgressionConfiguration?.MinorEncountersPerCycle ?? 0;
            return minorEncountersPerCycle + 1;
        }


        private static EnemyEncounterType GetEncounterType(bool isBoss, bool isFinalBoss)
        {
            if (isFinalBoss) return EnemyEncounterType.FinalBoss;
            if (isBoss) return EnemyEncounterType.Boss;

            return EnemyEncounterType.Minor;
        }

        private static string GetEncounterTypeText(EnemyEncounterType encounterType)
        {
            switch (encounterType)
            {
                case EnemyEncounterType.FinalBoss:
                    return "FINAL BOSS";
                case EnemyEncounterType.Boss:
                    return "BOSS";
                default:
                    return "MINOR";
            }
        }

        private static void RaiseRollRequested()
        {
            Bus<DiceRollRequestedEvent>.Raise(new DiceRollRequestedEvent());
        }

        private static void RaiseResetRequested()
        {
            Bus<DiceResetRequestedEvent>.Raise(new DiceResetRequestedEvent());
        }
    }
}
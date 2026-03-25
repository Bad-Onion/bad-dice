using _Project.Application.Events.Core;
using _Project.Application.Events.DiceInput;
using _Project.Application.Events.DiceSimulation;
using _Project.Application.Events.DiceState;
using _Project.Application.Events.EncounterState;
using _Project.Application.Events.GameState;

namespace _Project.Presentation.Scripts.Features.DiceSession.Views
{
    public partial class DiceSessionView
    {
        private void OnEncounterStarted(EncounterStartedEvent evt)
        {
            _levelContainer.style.display = UnityEngine.UIElements.DisplayStyle.Flex;
            _resultLabel.text = "Ready to roll.";

            UpdateTurnLabel(_diceSessionState.CurrentTurn, _diceSessionState.MaxTurns);
            UpdateDealButtonInteractable(false);
        }

        private void OnEncounterPrepared(EncounterPreparedEvent evt)
        {
            _enemyNameLabel.text = $"Enemy: {evt.EnemyName} ({GetEncounterTypeText(GetEncounterType(evt.IsBoss, evt.IsFinalBoss))})";
            _enemyHealthLabel.text = $"HP: {evt.CurrentHealth}/{evt.MaxHealth}";
            _cycleLabel.text = $"Cycle {evt.CycleNumber} - Encounter {evt.EncounterIndexInCycle}/{GetMaxEncountersPerCycle()}";
        }

        private void OnEnemyDamaged(EnemyDamagedEvent evt)
        {
            _enemyHealthLabel.text = $"HP: {evt.RemainingHealth}/{evt.MaxHealth}";
        }

        private void OnEnemyDefeated(EnemyDefeatedEvent evt)
        {
            _enemyHealthLabel.text = "HP: 0/0";
            _resultLabel.text = $"{evt.EnemyName} defeated. Preparing next encounter...";

            UpdateDealButtonInteractable(false);
        }

        private void OnRunCompleted(RunCompletedEvent evt)
        {
            _resultLabel.text = "Lucifer has fallen. Run completed!";
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
            _resultLabel.text = "Waiting to roll...";
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

        private void OnDiceHoverDetailsUpdated(DiceHoverDetailsUpdatedEvent evt)
        {
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


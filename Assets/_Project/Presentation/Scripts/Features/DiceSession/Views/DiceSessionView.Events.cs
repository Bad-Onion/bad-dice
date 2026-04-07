using _Project.Application.Events.DiceInput;
using _Project.Application.Events.EncounterState;
using _Project.Application.Events.GameState;
using _Project.Application.Events.DiceState;
using _Project.Application.States.DiceSession;
using _Project.Application.States.Encounter;
using _Project.Domain.Features.Combat.Session;

namespace _Project.Presentation.Scripts.Features.DiceSession.Views
{
    public partial class DiceSessionView
    {
        private void OnEncounterSnapshotUpdated(EncounterSnapshot snapshot)
        {
            if (snapshot == null) return;

            _enemyNameLabel.text = $"Enemy: {snapshot.EnemyName} ({GetEncounterTypeText(GetEncounterType(snapshot.IsBoss, snapshot.IsFinalBoss))})";
            _enemyHealthLabel.text = $"HP: {snapshot.CurrentHealth}/{snapshot.MaxHealth}";
            _cycleLabel.text = $"Cycle {snapshot.CycleNumber} - Encounter {snapshot.EncounterIndexInCycle}/{GetMaxEncountersPerCycle()}";

            if (snapshot.Phase != EncounterPhase.Active) return;

            _levelContainer.style.display = UnityEngine.UIElements.DisplayStyle.Flex;
            _resultLabel.text = "Ready to roll.";
            UpdateTurnLabel(_diceSessionState.CurrentTurn, _diceSessionState.MaxTurns);
            UpdateDealButtonInteractable(false);
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

        private void OnDiceRollPhaseChanged(DiceRollPhase phase)
        {
            if (phase == DiceRollPhase.ResolvingResult)
            {
                UpdateRollResultText();
                UpdateDealButtonInteractable(false);
                return;
            }

            if (phase != DiceRollPhase.Completed) return;

            UpdateRollResultText();
            UpdateDealButtonInteractable(CanDealThisTurn());
        }

        private void OnDiceReset(DiceResetEvent evt)
        {
            _resultLabel.text = "Waiting to roll...";
            UpdateDealButtonInteractable(false);
        }

        private void OnMergeStateChanged(MergeState mergeState)
        {
            if (mergeState != MergeState.Applied) return;

            UpdateRollResultText();
            UpdateDealButtonInteractable(CanDealThisTurn());
        }

        private void HandleDealClicked()
        {
            _commandProcessor.ExecuteCommand(_dealDamageCommand);
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

        private void RaiseRollRequested()
        {
            _commandProcessor.ExecuteCommand(_requestDiceRollCommand);
        }

        private void RaiseResetRequested()
        {
            _commandProcessor.ExecuteCommand(_resetDiceCommand);
        }
    }
}


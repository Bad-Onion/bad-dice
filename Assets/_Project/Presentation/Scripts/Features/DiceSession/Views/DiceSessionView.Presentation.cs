using _Project.Domain.Features.Combat.Enums;
using _Project.Domain.Features.Dice.Entities;

namespace _Project.Presentation.Scripts.Features.DiceSession.Views
{
    public partial class DiceSessionView
    {
        private void RefreshEnemyPanelFromState()
        {
            if (_enemyEncounterState?.CurrentEncounter == null) return;

            var currentEncounter = _enemyEncounterState.CurrentEncounter;
            int maxEncountersPerCycle = GetMaxEncountersPerCycle();

            _enemyNameLabel.text = $"Enemy: {currentEncounter.EnemyName} ({GetEncounterTypeText(currentEncounter.EncounterType)})";
            _enemyHealthLabel.text = $"HP: {_enemyEncounterState.CurrentHealth}/{currentEncounter.MaxHealth}";
            _cycleLabel.text = $"Cycle {currentEncounter.CycleNumber} - Encounter {currentEncounter.EncounterIndexInCycle}/{maxEncountersPerCycle}";

            UpdateTurnLabel(_diceSessionState.CurrentTurn, _diceSessionState.MaxTurns);
            UpdateDealButtonInteractable(CanDealThisTurn());
        }

        private void UpdateRollResultText()
        {
            _resultLabel.text = $"Rerolls Left: {_diceSessionState.RerollsLeft}\nHover a die to inspect Value, Level and Damage.";
        }

        private void UpdateTurnLabel(int currentTurn, int maxTurns)
        {
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
            _dealButton.SetEnabled(isInteractable);
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
    }
}


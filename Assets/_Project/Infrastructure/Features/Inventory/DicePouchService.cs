using System.Collections.Generic;
using System.Linq;
using _Project.Application.Events.Core;
using _Project.Application.Events.EncounterState;
using _Project.Application.UseCases;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Dice.Session;
using _Project.Domain.Features.Run.Session;

namespace _Project.Infrastructure.Features.Inventory
{
    public class DicePouchService : IDicePouchUseCase
    {
        private readonly PlayerRunState _runState;
        private readonly DiceSessionState _diceSessionState;

        public DicePouchService(PlayerRunState runState, DiceSessionState diceSessionState)
        {
            _runState = runState;
            _diceSessionState = diceSessionState;
        }

        public void ToggleDiceEquip(string diceId)
        {
            var dice = GetDiceToEquip(diceId);

            if (dice == null) return;
            if (!dice.IsEquipped && IsMaxDiceEquipped()) return;

            dice.IsEquipped = !dice.IsEquipped;
        }

        // TODO: DicePouchService should not be responsible for starting encounters, this should be moved to a more appropriate service (EncounterFlowCoordinator)
        public void StartEncounter()
        {
            if (!IsAnyDiceEquipped()) return;

            _diceSessionState.ActiveDice.Clear();
            _diceSessionState.RerollsLeft = _runState.RerollsPerTurn;
            _diceSessionState.CurrentTurn = 1;
            _diceSessionState.MaxTurns = _runState.TurnsPerFight;
            _diceSessionState.HasDealtThisTurn = false;

            SetActiveDices();

            Bus<TurnChangedEvent>.Raise(new TurnChangedEvent
            {
                CurrentTurn = _diceSessionState.CurrentTurn,
                MaxTurns = _diceSessionState.MaxTurns
            });

            Bus<EncounterStartedEvent>.Raise(new EncounterStartedEvent());
        }

        private bool IsAnyDiceEquipped()
        {
            return _runState.DiceInventory.Any(d => d.IsEquipped);
        }

        private OwnedDiceData GetDiceToEquip(string diceId)
        {
            return _runState.DiceInventory.FirstOrDefault(d => d.Dice.Id == diceId);
        }

        private bool IsMaxDiceEquipped()
        {
            int currentlyEquipped = _runState.DiceInventory.Count(d => d.IsEquipped);
            return currentlyEquipped >= _runState.MaxEquippedDice;
        }

        private void SetActiveDices()
        {
            List<OwnedDiceData> equippedDice = _runState.DiceInventory.Where(diceData => diceData.IsEquipped).ToList();

            foreach (var ownedDice in equippedDice)
            {
                _diceSessionState.ActiveDice.Add(new DiceState
                {
                    Dice = ownedDice.Dice,
                    Level = 1,
                    CurrentFaceIndex = -1,
                    IsSelectedForReroll = false
                });
            }
        }
    }
}
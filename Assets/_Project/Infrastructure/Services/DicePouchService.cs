using System.Collections.Generic;
using System.Linq;
using _Project.Application.Events.Core;
using _Project.Application.Events.EncounterState;
using _Project.Application.UseCases;
using _Project.Domain.Entities.DiceData;
using _Project.Domain.Entities.Session;

namespace _Project.Infrastructure.Services
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
            // TODO: Move to a separate function and name it "GetDiceToEquip"
            var dice = _runState.Inventory.FirstOrDefault(d => d.Id == diceId);
            if (dice == null) return;

            // TODO: Move to a separate function and name it "IsMaxDiceEquipped"
            int currentlyEquipped = _runState.Inventory.Count(d => d.IsEquipped);

            if (!dice.IsEquipped && currentlyEquipped >= _runState.MaxEquippedDice) return;

            dice.IsEquipped = !dice.IsEquipped;
        }

        public bool CanStartEncounter()
        {
            // TODO: Use Any() to test whether this is empty or not.
            return _runState.Inventory.Count(d => d.IsEquipped) > 0;
        }

        public void StartEncounter()
        {
            if (!CanStartEncounter()) return;

            _diceSessionState.ActiveDice.Clear();

            // TODO: Move to a separate function and name it "SetActiveDices"
            List<OwnedDiceData> equippedDice = _runState.Inventory.Where(diceData => diceData.IsEquipped).ToList();

            foreach (var ownedDice in equippedDice)
            {
                _diceSessionState.ActiveDice.Add(new DiceState
                {
                    Id = ownedDice.Id,
                    Definition = ownedDice.Definition,
                    Level = ownedDice.Level,
                    CurrentFaceIndex = -1,
                    IsSelectedForReroll = false
                });
            }

            Bus<EncounterStartedEvent>.Raise(new EncounterStartedEvent());
        }
    }
}
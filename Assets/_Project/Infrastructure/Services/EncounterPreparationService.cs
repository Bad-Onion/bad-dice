using System.Linq;
using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using _Project.Application.UseCases;
using _Project.Domain.Entities;

namespace _Project.Infrastructure.Services
{
    public class EncounterPreparationService : IEncounterPreparationUseCase
    {
        private readonly PlayerRunState _runState;
        private readonly DiceSession _diceSession;

        public EncounterPreparationService(PlayerRunState runState, DiceSession diceSession)
        {
            _runState = runState;
            _diceSession = diceSession;
        }

        public void ToggleDiceEquip(string diceId)
        {
            var dice = _runState.Inventory.FirstOrDefault(d => d.Id == diceId);
            if (dice == null) return;

            int currentlyEquipped = _runState.Inventory.Count(d => d.IsEquipped);

            if (!dice.IsEquipped && currentlyEquipped >= _runState.MaxEquippedDice) return;

            dice.IsEquipped = !dice.IsEquipped;
        }

        public bool CanStartEncounter()
        {
            return _runState.Inventory.Count(d => d.IsEquipped) > 0;
        }

        public void StartEncounter()
        {
            if (!CanStartEncounter()) return;

            _diceSession.ActiveDice.Clear();
            var equippedDice = _runState.Inventory.Where(d => d.IsEquipped).ToList();

            foreach (var ownedDice in equippedDice)
            {
                _diceSession.ActiveDice.Add(new DiceState
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
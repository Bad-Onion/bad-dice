using System.Linq;
using _Project.Application.UseCases;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Run.Session;

namespace _Project.Infrastructure.Features.Inventory
{
    public class DicePouchService : IDicePouchUseCase
    {
        private readonly PlayerRunState _runState;

        public DicePouchService(PlayerRunState runState)
        {
            _runState = runState;
        }

        public void ToggleDiceEquip(string dieId)
        {
            var dice = GetDiceToEquip(dieId);

            if (dice == null) return;
            if (!dice.IsEquipped && IsMaxDiceEquipped()) return;

            dice.IsEquipped = !dice.IsEquipped;
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
    }
}
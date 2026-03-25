using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.Entities;

namespace _Project.Infrastructure.Features.Combat.Damage
{
    public class DiceDamageService : IDiceDamageService
    {
        // TODO: Create a DamageCalculationService as an orchestrator + many small IDamageModifierProvider modules
        public int CalculateDamage(DiceState dice)
        {
            if (dice.Level == 0) return 1;

            return dice.CurrentValue * dice.Level;
        }
    }
}
using _Project.Application.Interfaces;
using _Project.Domain.Entities.DiceData;

namespace _Project.Infrastructure.Services
{
    public class DiceDamageService : IDiceDamageService
    {
        // TODO: Should the dice itself calculate its own damage?
        public int CalculateDamage(DiceState dice)
        {
            // Future gimmick logic (Metal Dice, Medusa Dice) will be evaluated here
            // by checking dice.Definition type or checking active modifiers.
            if (dice.Level == 0) return 1;

            return dice.CurrentValue * dice.Level;
        }
    }
}
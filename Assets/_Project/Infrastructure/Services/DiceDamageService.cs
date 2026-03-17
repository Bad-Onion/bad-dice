using _Project.Application.Interfaces;
using _Project.Domain.Entities;

namespace _Project.Infrastructure.Services
{
    public class DiceDamageService : IDiceDamageService
    {
        public int CalculateDamage(DiceState dice)
        {
            // Future gimmick logic (Metal Dice, Medusa Dice) will be evaluated here
            // by checking dice.Definition type or checking active modifiers.
            return dice.CurrentValue * dice.Level;
        }
    }
}
using System.Collections.Generic;
using _Project.Domain.Features.Dice.Entities;

namespace _Project.Application.Interfaces
{
    public interface IDamageCalculationService
    {
        int CalculateTotalDamage(IReadOnlyList<DiceState> diceStates);
        bool TryCalculateDiceDamage(
            IReadOnlyList<DiceState> diceStates,
            string diceId,
            out int currentValue,
            out int level,
            out int damage);
    }
}


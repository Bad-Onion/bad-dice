using _Project.Domain.Features.Dice.Entities;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Service interface provider for calculating damage related to the dice.
    /// </summary>
    public interface IDiceDamageService
    {
        /// <summary>
        /// Calculates the damage based on the current state of the die.
        /// </summary>
        /// <param name="dice">The current state of the die, including its level and current value.</param>
        /// <returns>The calculated damage as an integer.</returns>
        int CalculateDamage(DiceState dice);
    }
}
using System.Collections.Generic;
using _Project.Domain.Features.Dice.Entities;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Orchestrates the damage calculation process, this service is responsible for coordinating the various
    /// factors that contribute to damage calculation, such as die values, levels, and any applicable modifiers, in addition
    /// to manage the damage calculation order.
    /// </summary>
    public interface IDamageCalculationService
    {
        /// <summary>
        /// Calculates the total damage based on the states of all dice. This method aggregates the damage from each die,
        /// considering their current values and levels, to provide comprehensive total damage output.
        /// </summary>
        /// <param name="diceStates">A list of the current states of all dice, including their values and levels.</param>
        /// <returns>The total damage calculated from all damage providers and modifiers.</returns>
        int CalculateTotalDamage(IReadOnlyList<DiceState> diceStates);

        /// <summary>
        /// Attempts to calculate the damage for a specific die based on its current state and modifiers (not taking into account other damage providers besides the dice damage provider).
        /// </summary>
        /// <param name="diceStates">A list of the current states of all dice.</param>
        /// <param name="diceId">The unique identifier of the die for which to calculate damage.</param>
        /// <param name="currentValue">The output parameter that will hold the current value of the die after calculation.</param>
        /// <param name="level">The output parameter that will hold the current level of the die after calculation.</param>
        /// <param name="damage">The output parameter that will hold the calculated damage for the specified die.</param>
        /// <returns>True if the damage calculation was successful and the specified die was found; otherwise, false.</returns>
        bool TryCalculateDiceDamage(
            IReadOnlyList<DiceState> diceStates,
            string diceId,
            out int currentValue,
            out int level,
            out int damage);
    }
}


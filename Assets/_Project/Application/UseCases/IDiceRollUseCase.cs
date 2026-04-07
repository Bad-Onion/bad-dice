using System;
using _Project.Application.Events.DiceInput;
using _Project.Application.States.DiceSession;

namespace _Project.Application.UseCases
{
    /// <summary>
    /// Use case interface for managing dice rolling mechanics.
    /// Handles roll requests, roll termination, dice resets, and reroll selections.
    /// </summary>
    public interface IDiceRollUseCase
    {
        event Action<DiceRollPhase> DiceRollPhaseChanged;
        event Action<DiceResetEvent> DiceReset;
        event Action<DiceRerollToggledEvent> DiceRerollToggled;

        /// <summary>
        /// Requests a new dice roll for the equipped dice.
        /// </summary>
        void RequestRoll();

        /// <summary>
        /// Ends the current dice roll operation updating its current roll state.
        /// </summary>
        void EndRoll();

        /// <summary>
        /// Resets all dice to their initial state. (THIS METHOD WILL BE DELETED, IT SERVES ONLY FOR TESTING PURPOSES)
        /// </summary>
        void ResetDice();

        /// <summary>
        /// Toggles the reroll selection status for the specified die.
        /// Marks a die as selected or deselected for rerolling.
        /// </summary>
        /// <param name="dieId">The unique identifier of the dice to toggle for reroll selection.</param>
        void ToggleDiceRerollSelection(string dieId);
    }
}
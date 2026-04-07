using System;
using _Project.Application.Events.DiceState;
using _Project.Application.Events.MergeEvents;

namespace _Project.Application.UseCases
{
    /// <summary>
    /// Use case interface for managing dice merge mechanics.
    /// Handles merging of dice and evaluation of available merge possibilities.
    /// </summary>
    public interface IDiceMergeUseCase
    {
        event Action<MergePossibilitiesEvaluatedEvent> MergePossibilitiesEvaluated;
        event Action<MergeCompletedEvent> MergeCompleted;

        /// <summary>
        /// Executes merge operation on the specified target dice.
        /// Combines the target dice with compatible dice to create higher-value results.
        /// </summary>
        /// <param name="targetDieId">The unique identifier of the dice to merge.</param>
        void ExecuteMerge(string targetDieId);

        /// <summary>
        /// Evaluates all possible merge combinations among the currently rolled dice.
        /// Updates the system state with available merge opportunities.
        /// </summary>
        void EvaluateMergePossibilities();
    }
}
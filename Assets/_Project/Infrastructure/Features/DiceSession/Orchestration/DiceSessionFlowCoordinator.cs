using System;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Application.States.DiceSession;
using Zenject;

namespace _Project.Infrastructure.Features.DiceSession.Orchestration
{
    public class DiceSessionFlowCoordinator : IInitializable, IDisposable
    {
        private readonly IDiceRollUseCase _diceRollUseCase;
        private readonly IDiceMergeUseCase _diceMergeUseCase;
        private readonly IDicePlaybackCompletionInputSource _dicePlaybackCompletionInputSource;

        public DiceSessionFlowCoordinator(
            IDiceRollUseCase diceRollUseCase,
            IDiceMergeUseCase diceMergeUseCase,
            IDicePlaybackCompletionInputSource dicePlaybackCompletionInputSource)
        {
            _diceRollUseCase = diceRollUseCase;
            _diceMergeUseCase = diceMergeUseCase;
            _dicePlaybackCompletionInputSource = dicePlaybackCompletionInputSource;
        }

        public void Initialize()
        {
            _dicePlaybackCompletionInputSource.DicePlaybackCompleted += HandleDicePlaybackCompleted;
            _diceRollUseCase.DiceRollPhaseChanged += HandleDiceRollPhaseChanged;
            _diceMergeUseCase.MergeStateChanged += HandleMergeStateChanged;
        }

        public void Dispose()
        {
            _dicePlaybackCompletionInputSource.DicePlaybackCompleted -= HandleDicePlaybackCompleted;
            _diceRollUseCase.DiceRollPhaseChanged -= HandleDiceRollPhaseChanged;
            _diceMergeUseCase.MergeStateChanged -= HandleMergeStateChanged;
        }

        private void HandleDicePlaybackCompleted()
        {
            _diceRollUseCase.EndRoll();
        }

        private void HandleDiceRollPhaseChanged(DiceRollPhase phase)
        {
            if (phase != DiceRollPhase.Completed) return;
            _diceMergeUseCase.EvaluateMergePossibilities();
        }

        private void HandleMergeStateChanged(MergeState mergeState)
        {
            if (mergeState != MergeState.Applied) return;
            _diceMergeUseCase.EvaluateMergePossibilities();
        }
    }
}

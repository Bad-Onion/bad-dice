using System;
using _Project.Application.States.DiceSession;
using _Project.Application.UseCases;
using Zenject;

namespace _Project.Infrastructure.Features.DiceSession.Orchestration
{
    public class DiceSessionFlowCoordinator : IInitializable, IDisposable
    {
        private readonly IDiceRollUseCase _diceRollUseCase;
        private readonly IDiceMergeUseCase _diceMergeUseCase;

        public DiceSessionFlowCoordinator(
            IDiceRollUseCase diceRollUseCase,
            IDiceMergeUseCase diceMergeUseCase)
        {
            _diceRollUseCase = diceRollUseCase;
            _diceMergeUseCase = diceMergeUseCase;
        }

        public void Initialize()
        {
            _diceRollUseCase.DiceRollPhaseChanged += HandleDiceRollPhaseChanged;
            _diceMergeUseCase.MergeStateChanged += HandleMergeStateChanged;
        }

        public void Dispose()
        {
            _diceRollUseCase.DiceRollPhaseChanged -= HandleDiceRollPhaseChanged;
            _diceMergeUseCase.MergeStateChanged -= HandleMergeStateChanged;
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

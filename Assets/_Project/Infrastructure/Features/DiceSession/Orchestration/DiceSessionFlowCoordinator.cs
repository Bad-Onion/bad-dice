using System;
using _Project.Application.Events.Core;
using _Project.Application.Events.DiceSimulation;
using _Project.Application.Events.DiceState;
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
            Bus<DicePlaybackCompletedEvent>.OnEvent += HandleDicePlaybackCompleted;

            _diceRollUseCase.DiceRollFinished += HandleDiceRollFinished;
            _diceMergeUseCase.MergeCompleted += HandleMergeCompleted;
        }

        public void Dispose()
        {
            Bus<DicePlaybackCompletedEvent>.OnEvent -= HandleDicePlaybackCompleted;

            _diceRollUseCase.DiceRollFinished -= HandleDiceRollFinished;
            _diceMergeUseCase.MergeCompleted -= HandleMergeCompleted;
        }


        private void HandleDicePlaybackCompleted(DicePlaybackCompletedEvent evt)
        {
            _diceRollUseCase.EndRoll();
        }

        private void HandleDiceRollFinished(DiceRollFinishedEvent evt)
        {
            _diceMergeUseCase.EvaluateMergePossibilities();
        }

        private void HandleMergeCompleted(MergeCompletedEvent evt)
        {
            _diceMergeUseCase.EvaluateMergePossibilities();
        }
    }
}

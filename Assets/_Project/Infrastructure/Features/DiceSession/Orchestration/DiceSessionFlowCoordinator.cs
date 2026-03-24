using System;
using _Project.Application.Events.Core;
using _Project.Application.Events.DiceInput;
using _Project.Application.Events.DiceSimulation;
using _Project.Application.Events.DiceState;
using _Project.Application.Events.MergeEvents;
using _Project.Application.UseCases;
using Zenject;

namespace _Project.Infrastructure.Features.DiceSession.Orchestration
{
    public class DiceSessionFlowCoordinator : IInitializable, IDisposable
    {
        private readonly IDiceRollUseCase _diceRollUseCase;
        private readonly IDiceMergeUseCase _diceMergeUseCase;

        public DiceSessionFlowCoordinator(IDiceRollUseCase diceRollUseCase, IDiceMergeUseCase diceMergeUseCase)
        {
            _diceRollUseCase = diceRollUseCase;
            _diceMergeUseCase = diceMergeUseCase;
        }

        public void Initialize()
        {
            Bus<DiceRollRequestedEvent>.OnEvent += HandleDiceRollRequested;
            Bus<DiceResetRequestedEvent>.OnEvent += HandleDiceResetRequested;
            Bus<DiceRerollSelectionRequestedEvent>.OnEvent += HandleDiceRerollSelectionRequested;
            Bus<DiceAutoMergeRequestedEvent>.OnEvent += HandleDiceAutoMergeRequested;
            Bus<DicePlaybackCompletedEvent>.OnEvent += HandleDicePlaybackCompleted;
            Bus<DiceRollFinishedEvent>.OnEvent += HandleDiceRollFinished;
            Bus<MergeCompletedEvent>.OnEvent += HandleMergeCompleted;
        }

        public void Dispose()
        {
            Bus<DiceRollRequestedEvent>.OnEvent -= HandleDiceRollRequested;
            Bus<DiceResetRequestedEvent>.OnEvent -= HandleDiceResetRequested;
            Bus<DiceRerollSelectionRequestedEvent>.OnEvent -= HandleDiceRerollSelectionRequested;
            Bus<DiceAutoMergeRequestedEvent>.OnEvent -= HandleDiceAutoMergeRequested;
            Bus<DicePlaybackCompletedEvent>.OnEvent -= HandleDicePlaybackCompleted;
            Bus<DiceRollFinishedEvent>.OnEvent -= HandleDiceRollFinished;
            Bus<MergeCompletedEvent>.OnEvent -= HandleMergeCompleted;
        }

        private void HandleDiceRollRequested(DiceRollRequestedEvent evt)
        {
            _diceRollUseCase.RequestRoll();
        }

        private void HandleDiceResetRequested(DiceResetRequestedEvent evt)
        {
            _diceRollUseCase.ResetDice();
        }

        private void HandleDiceRerollSelectionRequested(DiceRerollSelectionRequestedEvent evt)
        {
            _diceRollUseCase.ToggleDiceRerollSelection(evt.DiceId);
        }

        private void HandleDiceAutoMergeRequested(DiceAutoMergeRequestedEvent evt)
        {
            _diceMergeUseCase.ExecuteAutoMerge(evt.DiceId);
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

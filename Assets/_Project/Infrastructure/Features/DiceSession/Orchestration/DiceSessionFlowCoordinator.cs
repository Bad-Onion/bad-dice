using System;
using _Project.Application.Events.Core;
using _Project.Application.Events.DiceInput;
using _Project.Application.Events.DiceSimulation;
using _Project.Application.Events.DiceState;
using _Project.Application.Events.EncounterState;
using _Project.Application.Events.MergeEvents;
using _Project.Application.UseCases;
using Zenject;

namespace _Project.Infrastructure.Features.DiceSession.Orchestration
{
    public class DiceSessionFlowCoordinator : IInitializable, IDisposable
    {
        private readonly IDiceRollUseCase _diceRollUseCase;
        private readonly IDiceMergeUseCase _diceMergeUseCase;
        private readonly IDealDamageUseCase _dealDamageUseCase;

        public DiceSessionFlowCoordinator(
            IDiceRollUseCase diceRollUseCase,
            IDiceMergeUseCase diceMergeUseCase,
            IDealDamageUseCase dealDamageUseCase)
        {
            _diceRollUseCase = diceRollUseCase;
            _diceMergeUseCase = diceMergeUseCase;
            _dealDamageUseCase = dealDamageUseCase;
        }

        public void Initialize()
        {
            Bus<DiceRollRequestedEvent>.OnEvent += HandleDiceRollRequested;
            Bus<DiceResetRequestedEvent>.OnEvent += HandleDiceResetRequested;
            Bus<DiceRerollSelectionRequestedEvent>.OnEvent += HandleDiceRerollSelectionRequested;
            Bus<DiceAutoMergeRequestedEvent>.OnEvent += HandleDiceAutoMergeRequested;
            Bus<DicePlaybackCompletedEvent>.OnEvent += HandleDicePlaybackCompleted;
            Bus<DealDamageRequestedEvent>.OnEvent += HandleDealDamageRequested;

            _diceRollUseCase.DiceRollFinished += HandleDiceRollFinished;
            _diceMergeUseCase.MergeCompleted += HandleMergeCompleted;
        }

        public void Dispose()
        {
            Bus<DiceRollRequestedEvent>.OnEvent -= HandleDiceRollRequested;
            Bus<DiceResetRequestedEvent>.OnEvent -= HandleDiceResetRequested;
            Bus<DiceRerollSelectionRequestedEvent>.OnEvent -= HandleDiceRerollSelectionRequested;
            Bus<DiceAutoMergeRequestedEvent>.OnEvent -= HandleDiceAutoMergeRequested;
            Bus<DicePlaybackCompletedEvent>.OnEvent -= HandleDicePlaybackCompleted;
            Bus<DealDamageRequestedEvent>.OnEvent -= HandleDealDamageRequested;

            _diceRollUseCase.DiceRollFinished -= HandleDiceRollFinished;
            _diceMergeUseCase.MergeCompleted -= HandleMergeCompleted;
        }

        private void HandleDealDamageRequested(DealDamageRequestedEvent evt)
        {
            _dealDamageUseCase.DealCurrentDamage();
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
            _diceMergeUseCase.ExecuteMerge(evt.DiceId);
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

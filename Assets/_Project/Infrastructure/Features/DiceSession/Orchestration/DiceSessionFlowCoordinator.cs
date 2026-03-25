using System;
using _Project.Application.Events.Core;
using _Project.Application.Events.DiceInput;
using _Project.Application.Events.DiceSimulation;
using _Project.Application.Events.DiceState;
using _Project.Application.Events.EncounterState;
using _Project.Application.Interfaces;
using _Project.Application.Events.MergeEvents;
using _Project.Application.UseCases;
using _Project.Domain.Features.Dice.Session;
using Zenject;

namespace _Project.Infrastructure.Features.DiceSession.Orchestration
{
    public class DiceSessionFlowCoordinator : IInitializable, IDisposable
    {
        private readonly IDiceRollUseCase _diceRollUseCase;
        private readonly IDiceMergeUseCase _diceMergeUseCase;
        private readonly IDealDamageUseCase _dealDamageUseCase;
        private readonly IDamageCalculationService _damageCalculationService;
        private readonly DiceSessionState _diceSessionState;

        public DiceSessionFlowCoordinator(
            IDiceRollUseCase diceRollUseCase,
            IDiceMergeUseCase diceMergeUseCase,
            IDealDamageUseCase dealDamageUseCase,
            IDamageCalculationService damageCalculationService,
            DiceSessionState diceSessionState)
        {
            _diceRollUseCase = diceRollUseCase;
            _diceMergeUseCase = diceMergeUseCase;
            _dealDamageUseCase = dealDamageUseCase;
            _damageCalculationService = damageCalculationService;
            _diceSessionState = diceSessionState;
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
            Bus<DealDamageRequestedEvent>.OnEvent += HandleDealDamageRequested;
            Bus<DiceHoverChangedEvent>.OnEvent += HandleDiceHoverChanged;
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
            Bus<DealDamageRequestedEvent>.OnEvent -= HandleDealDamageRequested;
            Bus<DiceHoverChangedEvent>.OnEvent -= HandleDiceHoverChanged;
        }

        private void HandleDealDamageRequested(DealDamageRequestedEvent evt)
        {
            _dealDamageUseCase.DealCurrentDiceDamage();
        }

        private void HandleDiceHoverChanged(DiceHoverChangedEvent evt)
        {
            if (!evt.IsHovered)
            {
                Bus<DiceHoverDetailsUpdatedEvent>.Raise(new DiceHoverDetailsUpdatedEvent
                {
                    HasDetails = false,
                    DiceId = evt.DiceId
                });
                return;
            }

            bool hasDetails = _damageCalculationService.TryCalculateDiceDamage(
                _diceSessionState.ActiveDice,
                evt.DiceId,
                out int currentValue,
                out int level,
                out int damage);

            if (!hasDetails)
            {
                Bus<DiceHoverDetailsUpdatedEvent>.Raise(new DiceHoverDetailsUpdatedEvent
                {
                    HasDetails = false,
                    DiceId = evt.DiceId
                });
                return;
            }

            Bus<DiceHoverDetailsUpdatedEvent>.Raise(new DiceHoverDetailsUpdatedEvent
            {
                HasDetails = true,
                DiceId = evt.DiceId,
                CurrentValue = currentValue,
                Level = level,
                Damage = damage
            });
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

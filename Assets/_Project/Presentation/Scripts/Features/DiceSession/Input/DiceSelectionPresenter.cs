using System;
using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.Session;
using _Project.Infrastructure.Shared.Adapters;
using _Project.Presentation.Scripts.Features.DiceSession.Orchestration;
using _Project.Presentation.Scripts.Shared.Input;
using UnityEngine;

namespace _Project.Presentation.Scripts.Features.DiceSession.Input
{
    public class DiceSelectionPresenter
    {
        private readonly DiceRollState _diceRollState;
        private readonly PointerSelectionPresenter<DiceController> _pointerSelectionPresenter;

        public event Action<string> OnRerollRequested;
        public event Action<string> OnMergeRequested;
        public event Action<string, bool> OnDiceHoverChanged;

        public DiceSelectionPresenter(
            InputReader inputReader,
            DiceRollState diceRollState,
            IPointerTargetingService pointerTargetingService)
        {
            _diceRollState = diceRollState;
            _pointerSelectionPresenter = new PointerSelectionPresenter<DiceController>(
                inputReader,
                pointerTargetingService,
                CanProcessInteraction);

            _pointerSelectionPresenter.OnClickInteractionRequested += HandleRerollInteraction;
            _pointerSelectionPresenter.OnHoldClickInteractionRequested += HandleMergeInteraction;
            _pointerSelectionPresenter.OnHoverStarted += HandleHoverStarted;
            _pointerSelectionPresenter.OnHoverEnded += HandleHoverEnded;
        }

        public void Configure(LayerMask diceLayerMask)
        {
            _pointerSelectionPresenter.Configure(diceLayerMask);
        }

        public void Enable()
        {
            _pointerSelectionPresenter.Enable();
        }

        public void Disable()
        {
            _pointerSelectionPresenter.Disable();
        }

        public void Tick()
        {
            _pointerSelectionPresenter.Tick();
        }

        private void HandleRerollInteraction(DiceController diceController)
        {
            OnRerollRequested?.Invoke(diceController.DiceId);
        }

        private void HandleMergeInteraction(DiceController diceController)
        {
            OnMergeRequested?.Invoke(diceController.DiceId);
        }

        private bool CanProcessInteraction()
        {
            if (_diceRollState == null) return false;

            return !_diceRollState.IsRolling;
        }

        private void HandleHoverStarted(DiceController diceController)
        {
            OnDiceHoverChanged?.Invoke(diceController.DiceId, true);
        }

        private void HandleHoverEnded(DiceController diceController)
        {
            OnDiceHoverChanged?.Invoke(diceController.DiceId, false);
        }
    }
}


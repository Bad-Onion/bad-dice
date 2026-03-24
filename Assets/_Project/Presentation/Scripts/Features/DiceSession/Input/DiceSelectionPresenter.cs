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
        private readonly DiceSessionState _diceSessionState;
        private readonly PointerSelectionPresenter<DiceController> _pointerSelectionPresenter;

        public event Action<string> OnRerollRequested;
        public event Action<string> OnAutoMergeRequested;

        public DiceSelectionPresenter(
            InputReader inputReader,
            DiceSessionState diceSessionState,
            IPointerTargetingService pointerTargetingService)
        {
            _diceSessionState = diceSessionState;
            _pointerSelectionPresenter = new PointerSelectionPresenter<DiceController>(
                inputReader,
                pointerTargetingService,
                CanProcessInteraction);

            _pointerSelectionPresenter.OnClickInteractionRequested += HandleRerollInteraction;
            _pointerSelectionPresenter.OnHoldClickInteractionRequested += HandleAutoMergeInteraction;
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

        private void HandleAutoMergeInteraction(DiceController diceController)
        {
            OnAutoMergeRequested?.Invoke(diceController.DiceId);
        }

        private bool CanProcessInteraction()
        {
            if (_diceSessionState == null) return false;

            return !_diceSessionState.IsRolling;
        }
    }
}


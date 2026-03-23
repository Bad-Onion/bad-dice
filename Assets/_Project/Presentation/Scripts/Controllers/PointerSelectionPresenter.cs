using _Project.Application.Interfaces;
using _Project.Infrastructure.Adapters;
using System;
using UnityEngine;

namespace _Project.Presentation.Scripts.Controllers
{
    public class PointerSelectionPresenter<TTarget> where TTarget : Component, IHoverablePointerTarget
    {
        private readonly InputReader _inputReader;
        private readonly IPointerTargetingService _pointerTargetingService;
        private readonly Func<bool> _canProcessInteraction;

        private LayerMask _interactionLayerMask;
        private TTarget _hoveredTarget;
        private bool _isEnabled;

        public event Action<TTarget> OnPrimaryInteractionRequested;
        public event Action<TTarget> OnSecondaryInteractionRequested;

        public PointerSelectionPresenter(
            InputReader inputReader,
            IPointerTargetingService pointerTargetingService,
            Func<bool> canProcessInteraction)
        {
            _inputReader = inputReader;
            _pointerTargetingService = pointerTargetingService;
            _canProcessInteraction = canProcessInteraction;
        }

        public void Configure(LayerMask interactionLayerMask)
        {
            _interactionLayerMask = interactionLayerMask;
        }

        public void Enable()
        {
            if (_isEnabled || _inputReader == null) return;

            _inputReader.OnInteract += HandleClickInteraction;
            _inputReader.OnHoldInteract += HandleHoldClickInteraction;
            _isEnabled = true;
        }

        public void Disable()
        {
            if (!_isEnabled || _inputReader == null) return;

            _inputReader.OnInteract -= HandleClickInteraction;
            _inputReader.OnHoldInteract -= HandleHoldClickInteraction;
            ClearHoveredTarget();
            _isEnabled = false;
        }

        public void Tick()
        {
            if (!_isEnabled) return;

            HandleHover();
        }

        private void HandleHover()
        {
            if (!TryGetPointedTarget(out TTarget pointedTarget))
            {
                ClearHoveredTarget();
                return;
            }

            SetHoveredTarget(pointedTarget);
        }

        private void HandleClickInteraction()
        {
            if (!CanProcessInteraction()) return;
            if (!TryGetPointedTarget(out TTarget pointedTarget)) return;

            OnPrimaryInteractionRequested?.Invoke(pointedTarget);
        }

        private void HandleHoldClickInteraction()
        {
            if (!CanProcessInteraction()) return;
            if (!TryGetPointedTarget(out TTarget pointedTarget)) return;

            OnSecondaryInteractionRequested?.Invoke(pointedTarget);
        }

        private bool CanProcessInteraction()
        {
            if (_canProcessInteraction == null) return true;

            return _canProcessInteraction.Invoke();
        }

        private bool TryGetPointedTarget(out TTarget pointedTarget)
        {
            return _pointerTargetingService.TryGetTargetFromPointer(_interactionLayerMask, out pointedTarget);
        }

        private void SetHoveredTarget(TTarget target)
        {
            if (target == _hoveredTarget) return;

            ClearHoveredTarget();
            _hoveredTarget = target;
            _hoveredTarget.SetHoverVisual(true);
        }

        private void ClearHoveredTarget()
        {
            if (_hoveredTarget == null) return;

            _hoveredTarget.SetHoverVisual(false);
            _hoveredTarget = null;
        }
    }
}


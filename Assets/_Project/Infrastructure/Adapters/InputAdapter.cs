using System;
using UnityEngine.InputSystem;
using _Project.Application.Interfaces;
using UnityEngine;
using Zenject;

namespace _Project.Infrastructure.Adapters
{
    public class InputAdapter : IInputProvider, IInitializable, IDisposable
    {
        private readonly InputActionReference _pauseInputAction;
        private readonly InputActionReference _interactInputAction;
        private readonly InputActionReference _pointerPositionAction;

        public event Action OnPauseAction;
        public event Action OnInteract;

        public InputAdapter(
            InputActionReference pauseInputAction,
            InputActionReference interactInputAction,
            InputActionReference pointerPositionAction)
        {
            _pauseInputAction = pauseInputAction;
            _interactInputAction = interactInputAction;
            _pointerPositionAction = pointerPositionAction;
        }

        public void Initialize()
        {
            if (_pauseInputAction != null)
            {
                _pauseInputAction.action.Enable();
                _pauseInputAction.action.performed += OnPausePerformed;
            }

            if (_interactInputAction != null)
            {
                _interactInputAction.action.Enable();
                _interactInputAction.action.performed += HandleInteractPerformed;
            }

            if (_pointerPositionAction != null)
            {
                _pointerPositionAction.action.Enable();
            }
        }

        public void Dispose()
        {
            if (_pauseInputAction != null)
            {
                _pauseInputAction.action.performed -= OnPausePerformed;
            }

            if (_interactInputAction != null)
            {
                _interactInputAction.action.performed -= HandleInteractPerformed;
            }
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            OnPauseAction?.Invoke();
        }

        private void HandleInteractPerformed(InputAction.CallbackContext context)
        {
            OnInteract?.Invoke();
        }

        public Vector2 GetPointerPosition()
        {
            if (_pointerPositionAction != null && _pointerPositionAction.action.enabled)
            {
                return _pointerPositionAction.action.ReadValue<Vector2>();
            }

            return Vector2.zero;
        }
    }
}
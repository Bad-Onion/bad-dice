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
        private readonly InputActionReference _holdInteractAction;
        private readonly InputActionReference _pointerPositionAction;

        public event Action OnPauseAction;
        public event Action OnInteract;
        public event Action OnHoldInteract;

        public InputAdapter(
            InputActionReference pauseInputAction,
            InputActionReference interactInputAction,
            InputActionReference holdInteractAction,
            InputActionReference pointerPositionAction)
        {
            _pauseInputAction = pauseInputAction;
            _interactInputAction = interactInputAction;
            _holdInteractAction = holdInteractAction;
            _pointerPositionAction = pointerPositionAction;
        }

        // TODO: Change this to adhere to the Open Closed principle, right now it is not possible to add new input actions without modifying this class, which is not ideal.
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

            if (_holdInteractAction != null)
            {
                _holdInteractAction.action.Enable();
                _holdInteractAction.action.performed += HandleHoldInteractPerformed;
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

            if (_holdInteractAction != null) _holdInteractAction.action.performed -= HandleHoldInteractPerformed;
        }

        private void OnPausePerformed(InputAction.CallbackContext context) => OnPauseAction?.Invoke();
        private void HandleInteractPerformed(InputAction.CallbackContext context) => OnInteract?.Invoke();
        private void HandleHoldInteractPerformed(InputAction.CallbackContext context) => OnHoldInteract?.Invoke();

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
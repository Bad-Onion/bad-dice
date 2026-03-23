using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Infrastructure.Adapters
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Infrastructure/Input/InputReader")]
    public class InputReader : ScriptableObject, InputSystemActions.IPlayerActions, InputSystemActions.IUIActions
    {
        private InputSystemActions _inputActions;
        private Vector2 _pointerPosition;

        public event Action OnPauseAction;
        public event Action OnInteract;
        public event Action OnHoldInteract;

        public Vector2 GetPointerPosition()
        {
            return _pointerPosition;
        }

        private void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new InputSystemActions();
            }

            _inputActions.Player.AddCallbacks(this);
            _inputActions.UI.AddCallbacks(this);
            _inputActions.Player.Enable();
            _inputActions.UI.Enable();
        }

        private void OnDisable()
        {
            if (_inputActions == null)
            {
                return;
            }

            _inputActions.Player.RemoveCallbacks(this);
            _inputActions.UI.RemoveCallbacks(this);
            _inputActions.Player.Disable();
            _inputActions.UI.Disable();
        }

        void InputSystemActions.IPlayerActions.OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnInteract?.Invoke();
            }
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
        }

        public void OnNext(InputAction.CallbackContext context)
        {
        }

        void InputSystemActions.IPlayerActions.OnHoldInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnHoldInteract?.Invoke();
            }
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            _pointerPosition = context.ReadValue<Vector2>();
        }

        public void OnClick(InputAction.CallbackContext context)
        {
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnPauseAction?.Invoke();
            }
        }
    }
}
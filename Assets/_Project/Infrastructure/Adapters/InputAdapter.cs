using System;
using Zenject;
using UnityEngine.InputSystem;
using _Project.Application.Interfaces;

namespace _Project.Infrastructure.Adapters
{
    public class InputAdapter : IInputProvider, IInitializable, IDisposable
    {
        public event Action OnPauseAction;

        private readonly InputActionReference _pauseActionReference;

        public InputAdapter(InputActionReference pauseActionReference)
        {
            _pauseActionReference = pauseActionReference;
        }

        public void Initialize()
        {
            _pauseActionReference.action.performed += OnPausePerformed;
            _pauseActionReference.action.Enable();
        }

        public void Dispose()
        {
            _pauseActionReference.action.performed -= OnPausePerformed;
            _pauseActionReference.action.Disable();
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            OnPauseAction?.Invoke();
        }
    }
}
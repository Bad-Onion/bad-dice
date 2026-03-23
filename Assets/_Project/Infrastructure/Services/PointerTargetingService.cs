using _Project.Application.Interfaces;
using _Project.Infrastructure.Adapters;
using UnityEngine;

namespace _Project.Infrastructure.Services
{
    public class PointerTargetingService : IPointerTargetingService
    {
        private readonly InputReader _inputReader;
        private readonly Camera _levelCamera;

        public PointerTargetingService(InputReader inputReader, Camera levelCamera)
        {
            _inputReader = inputReader;
            _levelCamera = levelCamera;
        }

        public bool TryGetTargetFromPointer<TTarget>(LayerMask interactionLayerMask, out TTarget target)
            where TTarget : Component
        {
            target = null;
            if (_inputReader == null || _levelCamera == null) return false;

            Vector2 pointerPosition = _inputReader.GetPointerPosition();
            Ray pointerRay = _levelCamera.ScreenPointToRay(pointerPosition);

            if (!Physics.Raycast(pointerRay, out RaycastHit hit, Mathf.Infinity, interactionLayerMask))
            {
                return false;
            }

            target = hit.collider.GetComponentInParent<TTarget>();
            return target != null;
        }
    }
}


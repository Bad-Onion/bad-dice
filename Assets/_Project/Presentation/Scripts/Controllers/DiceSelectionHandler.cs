using _Project.Application.Events.Core;
using _Project.Application.Events.DiceInput;
using _Project.Application.Events.MergeEvents;
using _Project.Application.Interfaces;
using _Project.Domain.Entities.Session;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Controllers
{
    public class DiceSelectionHandler : MonoBehaviour
    {
        [Header("Physics")]
        [Tooltip("Set this to the layer your Dice are on (Currently is 'Dice').")]
        [SerializeField] private LayerMask diceLayerMask;

        private DiceSessionState _diceSessionState;
        private IInputProvider _inputProvider;
        private Camera _levelCamera;

        private DiceController _hoveredDice;

        [Inject]
        public void Construct(DiceSessionState diceSessionState, IInputProvider inputProvider, Camera levelCamera)
        {
            _diceSessionState = diceSessionState;
            _inputProvider = inputProvider;
            _levelCamera = levelCamera;
        }

        private void OnEnable()
        {
            if (_inputProvider == null) return;

            _inputProvider.OnInteract += HandleInteraction;
            _inputProvider.OnHoldInteract += HandleHoldInteraction;
        }

        private void OnDisable()
        {
            if (_inputProvider == null) return;

            _inputProvider.OnInteract -= HandleInteraction;
            _inputProvider.OnHoldInteract -= HandleHoldInteraction;
        }

        private void Update()
        {
            if (_inputProvider == null || _levelCamera == null) return;

            // TODO: Move this to a separate function and name it "GetCameraRay"
            Ray ray = _levelCamera.ScreenPointToRay(_inputProvider.GetPointerPosition());

            // TODO: Too much conditionals for an update function, move to a separate function and name it "HandleHover" and if possible find another way to do it
            // TODO: Reuse code between HandleInteraction and HandleHoldInteraction and Update in a separate function
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, diceLayerMask))
            {
                var diceController = hit.collider.GetComponentInParent<DiceController>();

                // TODO: Invert this condition to reduce nesting
                if (diceController != _hoveredDice)
                {
                    // TODO: Find a way to avoid this much expensive null comparison
                    if (_hoveredDice != null) _hoveredDice.SetHoverVisual(false);
                    _hoveredDice = diceController;
                    if (_hoveredDice != null) _hoveredDice.SetHoverVisual(true);
                }
            }
            else if (_hoveredDice != null)
            {
                _hoveredDice.SetHoverVisual(false);
                _hoveredDice = null;
            }
        }

        // TODO: Move this to a separate class and name it "InputHandler"
        // TODO: Reuse code between HandleInteraction and HandleHoldInteraction and Update in a separate function
        private void HandleInteraction()
        {
            if (_diceSessionState.IsRolling) return;

            // TODO: Move this to a separate function and name it "GetCameraRay"
            Ray ray = _levelCamera.ScreenPointToRay(_inputProvider.GetPointerPosition());

            // TODO: Invert this condition to reduce nesting
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, diceLayerMask))
            {
                // TODO: Don't use GetComponentInParent, find a way to avoid this
                var diceController = hit.collider.GetComponentInParent<DiceController>();
                if (diceController != null)
                {
                    Bus<DiceRerollSelectionRequestedEvent>.Raise(new DiceRerollSelectionRequestedEvent
                    {
                        DiceId = diceController.DiceId
                    });
                }
            }
        }

        // TODO: Move this to a separate class and name it "InputHandler"
        // TODO: Reuse code between HandleInteraction and HandleHoldInteraction and Update in a separate function
        private void HandleHoldInteraction()
        {
            if (_diceSessionState.IsRolling) return;

            Vector2 pointerPos = _inputProvider.GetPointerPosition();
            Ray ray = _levelCamera.ScreenPointToRay(pointerPos);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, diceLayerMask))
            {
                var diceController = hit.collider.GetComponentInParent<DiceController>();
                if (diceController != null)
                {
                    Bus<DiceAutoMergeRequestedEvent>.Raise(new DiceAutoMergeRequestedEvent
                    {
                        DiceId = diceController.DiceId
                    });
                }
            }
        }
    }
}
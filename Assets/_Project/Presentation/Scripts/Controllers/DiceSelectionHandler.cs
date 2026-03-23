using _Project.Application.Events.Core;
using _Project.Application.Events.DiceInput;
using _Project.Application.Events.MergeEvents;
using _Project.Domain.Entities.Session;
using _Project.Infrastructure.Adapters;
using System;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Controllers
{
    public class DiceSelectionHandler : MonoBehaviour
    {
        [Header("Physics")]
        [Tooltip("Set this to the layer your Dice are on (Currently is 'Dice').")]
        [SerializeField] private LayerMask diceLayerMask;

        private InputReader _inputReader;
        private DiceSessionState _diceSessionState;
        private Camera _levelCamera;

        private DiceController _hoveredDice;

        [Inject]
        public void Construct(DiceSessionState diceSessionState, Camera levelCamera, InputReader inputReader)
        {
            _diceSessionState = diceSessionState;
            _levelCamera = levelCamera;
            _inputReader = inputReader;
        }

        private void OnEnable()
        {
            if (_inputReader == null) return;

            _inputReader.OnInteract += HandleInteraction;
            _inputReader.OnHoldInteract += HandleHoldInteraction;
        }

        private void OnDisable()
        {
            if (_inputReader == null) return;

            _inputReader.OnInteract -= HandleInteraction;
            _inputReader.OnHoldInteract -= HandleHoldInteraction;
        }

        private void Update()
        {
            if (!TryGetPointerRay(out Ray pointerRay)) return;

            HandleHover(pointerRay);
        }

        private void HandleInteraction()
        {
            TryHandleDiceAction(RequestRerollForDice);
        }

        private void HandleHoldInteraction()
        {
            TryHandleDiceAction(RequestAutoMergeForDice);
        }

        private void HandleHover(Ray pointerRay)
        {
            if (!TryGetDiceControllerFromRay(pointerRay, out DiceController diceController))
            {
                ClearHoveredDice();
                return;
            }

            SetHoveredDice(diceController);
        }

        private void TryHandleDiceAction(Action<DiceController> onDiceSelected)
        {
            if (!CanProcessInteraction()) return;
            if (!TryGetPointedDiceController(out DiceController diceController)) return;

            onDiceSelected?.Invoke(diceController);
        }

        private bool CanProcessInteraction()
        {
            if (_inputReader == null || _levelCamera == null || _diceSessionState == null) return false;

            return !_diceSessionState.IsRolling;
        }

        private bool TryGetPointedDiceController(out DiceController diceController)
        {
            diceController = null;

            if (!TryGetPointerRay(out Ray pointerRay)) return false;

            return TryGetDiceControllerFromRay(pointerRay, out diceController);
        }

        private bool TryGetPointerRay(out Ray pointerRay)
        {
            pointerRay = default;
            if (_inputReader == null || _levelCamera == null) return false;

            pointerRay = _levelCamera.ScreenPointToRay(_inputReader.GetPointerPosition());
            return true;
        }

        private bool TryGetDiceControllerFromRay(Ray pointerRay, out DiceController diceController)
        {
            diceController = null;
            if (!Physics.Raycast(pointerRay, out RaycastHit hit, Mathf.Infinity, diceLayerMask)) return false;

            diceController = hit.collider.GetComponentInParent<DiceController>();
            return diceController != null;
        }

        private void SetHoveredDice(DiceController diceController)
        {
            if (diceController == _hoveredDice) return;

            ClearHoveredDice();
            _hoveredDice = diceController;
            _hoveredDice.SetHoverVisual(true);
        }

        private void ClearHoveredDice()
        {
            if (_hoveredDice == null) return;

            _hoveredDice.SetHoverVisual(false);
            _hoveredDice = null;
        }

        private static void RequestRerollForDice(DiceController diceController)
        {
            Bus<DiceRerollSelectionRequestedEvent>.Raise(new DiceRerollSelectionRequestedEvent
            {
                DiceId = diceController.DiceId
            });
        }

        private static void RequestAutoMergeForDice(DiceController diceController)
        {
            Bus<DiceAutoMergeRequestedEvent>.Raise(new DiceAutoMergeRequestedEvent
            {
                DiceId = diceController.DiceId
            });
        }
    }
}
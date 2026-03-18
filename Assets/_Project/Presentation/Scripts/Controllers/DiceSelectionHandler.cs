using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Controllers
{
    public class DiceSelectionHandler : MonoBehaviour
    {
        [Header("Physics")]
        [Tooltip("Set this to the layer your Dice are on (e.g., 'Dice').")]
        [SerializeField] private LayerMask diceLayerMask;

        private IDiceRollUseCase _diceRollUseCase;
        private IDiceMergeUseCase _diceMergeUseCase;
        private DiceSession _diceSession;
        private IInputProvider _inputProvider;
        private Camera _levelCamera;

        private DiceController _hoveredDice;

        [Inject]
        public void Construct(IDiceRollUseCase diceRollUseCase, IDiceMergeUseCase diceMergeUseCase, DiceSession diceSession, IInputProvider inputProvider, Camera levelCamera)
        {
            _diceRollUseCase = diceRollUseCase;
            _diceMergeUseCase = diceMergeUseCase;
            _diceSession = diceSession;
            _inputProvider = inputProvider;
            _levelCamera = levelCamera;
        }

        private void OnEnable()
        {
            if (_inputProvider != null)
                _inputProvider.OnInteract += HandleInteraction;
        }

        private void OnDisable()
        {
            if (_inputProvider != null)
                _inputProvider.OnInteract -= HandleInteraction;
        }

        private void Update()
        {
            if (_inputProvider == null || _levelCamera == null) return;

            Vector2 pointerPos = _inputProvider.GetPointerPosition();
            Ray ray = _levelCamera.ScreenPointToRay(pointerPos);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, diceLayerMask))
            {
                var diceController = hit.collider.GetComponentInParent<DiceController>();

                if (diceController != _hoveredDice)
                {
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

        private void HandleInteraction()
        {
            if (_diceSession.IsRolling) return;

            Vector2 pointerPos = _inputProvider.GetPointerPosition();
            Ray ray = _levelCamera.ScreenPointToRay(pointerPos);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, diceLayerMask))
            {
                var diceController = hit.collider.GetComponentInParent<DiceController>();
                if (diceController != null)
                {
                    if (_diceSession.IsMergeModeActive)
                    {
                        _diceMergeUseCase.ToggleDiceMergeSelection(diceController.DiceId);
                    }
                    else
                    {
                        _diceRollUseCase.ToggleDiceRerollSelection(diceController.DiceId);
                    }
                }
            }
        }
    }
}
using System;
using _Project.Application.Events.Core;
using _Project.Application.Events.DiceInput;
using _Project.Application.Events.MergeEvents;
using _Project.Application.Interfaces;
using _Project.Presentation.Scripts.Features.DiceSession.Input;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Features.DiceSession.EventHandlers
{
    public class DiceSelectionHandler : MonoBehaviour, IDiceHoverInputSource
    {
        [Header("Physics")]
        [Tooltip("Set this to the layer that contains dice colliders.")]
        [SerializeField] private LayerMask diceLayerMask;

        private DiceSelectionPresenter _diceSelectionPresenter;

        public event Action<DiceHoverChangedEvent> DiceHoverChanged;

        [Inject]
        public void Construct(DiceSelectionPresenter diceSelectionPresenter)
        {
            _diceSelectionPresenter = diceSelectionPresenter;
        }

        private void OnEnable()
        {
            if (_diceSelectionPresenter == null) return;

            _diceSelectionPresenter.Configure(diceLayerMask);
            _diceSelectionPresenter.OnRerollRequested += HandleRerollRequested;
            _diceSelectionPresenter.OnAutoMergeRequested += HandleAutoMergeRequested;
            _diceSelectionPresenter.OnDiceHoverChanged += HandleDiceHoverChanged;
            _diceSelectionPresenter.Enable();
        }

        private void OnDisable()
        {
            if (_diceSelectionPresenter == null) return;

            _diceSelectionPresenter.OnRerollRequested -= HandleRerollRequested;
            _diceSelectionPresenter.OnAutoMergeRequested -= HandleAutoMergeRequested;
            _diceSelectionPresenter.OnDiceHoverChanged -= HandleDiceHoverChanged;
            _diceSelectionPresenter.Disable();
        }

        private void Update()
        {
            _diceSelectionPresenter?.Tick();
        }

        private static void HandleRerollRequested(string diceId)
        {
            Bus<DiceRerollSelectionRequestedEvent>.Raise(new DiceRerollSelectionRequestedEvent
            {
                DiceId = diceId
            });
        }

        private static void HandleAutoMergeRequested(string diceId)
        {
            Bus<DiceAutoMergeRequestedEvent>.Raise(new DiceAutoMergeRequestedEvent
            {
                DiceId = diceId
            });
        }

        private void HandleDiceHoverChanged(string diceId, bool isHovered)
        {
            DiceHoverChanged?.Invoke(new DiceHoverChangedEvent
            {
                DiceId = diceId,
                IsHovered = isHovered
            });
        }
    }
}
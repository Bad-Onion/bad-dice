using System;
using _Project.Application.Commands;
using _Project.Application.Events.DiceInput;
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
        private CommandProcessor _commandProcessor;
        private ToggleDiceRerollSelectionCommand.Factory _toggleDiceRerollSelectionCommandFactory;
        private ExecuteDiceMergeCommand.Factory _executeDiceMergeCommandFactory;

        public event Action<DiceHoverChangedEvent> DiceHoverChanged;

        [Inject]
        public void Construct(
            DiceSelectionPresenter diceSelectionPresenter,
            CommandProcessor commandProcessor,
            ToggleDiceRerollSelectionCommand.Factory toggleDiceRerollSelectionCommandFactory,
            ExecuteDiceMergeCommand.Factory executeDiceMergeCommandFactory)
        {
            _diceSelectionPresenter = diceSelectionPresenter;
            _commandProcessor = commandProcessor;
            _toggleDiceRerollSelectionCommandFactory = toggleDiceRerollSelectionCommandFactory;
            _executeDiceMergeCommandFactory = executeDiceMergeCommandFactory;
        }

        private void OnEnable()
        {
            if (_diceSelectionPresenter == null) return;

            _diceSelectionPresenter.Configure(diceLayerMask);
            _diceSelectionPresenter.OnRerollRequested += HandleRerollRequested;
            _diceSelectionPresenter.OnMergeRequested += HandleMergeRequested;
            _diceSelectionPresenter.OnDiceHoverChanged += HandleDiceHoverChanged;
            _diceSelectionPresenter.Enable();
        }

        private void OnDisable()
        {
            if (_diceSelectionPresenter == null) return;

            _diceSelectionPresenter.OnRerollRequested -= HandleRerollRequested;
            _diceSelectionPresenter.OnMergeRequested -= HandleMergeRequested;
            _diceSelectionPresenter.OnDiceHoverChanged -= HandleDiceHoverChanged;
            _diceSelectionPresenter.Disable();
        }

        private void Update()
        {
            _diceSelectionPresenter?.Tick();
        }

        private void HandleRerollRequested(string diceId)
        {
            _commandProcessor.ExecuteCommand(_toggleDiceRerollSelectionCommandFactory.Create(diceId));
        }

        private void HandleMergeRequested(string diceId)
        {
            _commandProcessor.ExecuteCommand(_executeDiceMergeCommandFactory.Create(diceId));
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
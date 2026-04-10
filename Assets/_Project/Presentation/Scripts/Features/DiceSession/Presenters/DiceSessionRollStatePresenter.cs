using _Project.Application.Events.DiceInput;
using _Project.Application.Events.DiceState;
using _Project.Application.Interfaces;
using _Project.Application.States.DiceSession;
using _Project.Application.States.Encounter;
using _Project.Application.UseCases;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.Dice.Session;

namespace _Project.Presentation.Scripts.Features.DiceSession.Presenters
{
    public class DiceSessionRollStatePresenter : IDiceSessionPresenterLifecycle
    {
        private readonly DiceSessionState _diceSessionState;
        private readonly IDiceRollUseCase _diceRollUseCase;
        private readonly IDiceMergeUseCase _diceMergeUseCase;
        private readonly IDiceHoverUseCase _diceHoverUseCase;
        private readonly IEncounterProgressionUseCase _encounterProgressionUseCase;

        private IDiceSessionView _view;

        public DiceSessionRollStatePresenter(
            DiceSessionState diceSessionState,
            IDiceRollUseCase diceRollUseCase,
            IDiceMergeUseCase diceMergeUseCase,
            IDiceHoverUseCase diceHoverUseCase,
            IEncounterProgressionUseCase encounterProgressionUseCase)
        {
            _diceSessionState = diceSessionState;
            _diceRollUseCase = diceRollUseCase;
            _diceMergeUseCase = diceMergeUseCase;
            _diceHoverUseCase = diceHoverUseCase;
            _encounterProgressionUseCase = encounterProgressionUseCase;
        }

        public void Attach(IDiceSessionView view)
        {
            _view = view;

            _encounterProgressionUseCase.EncounterSnapshotUpdated += OnEncounterSnapshotUpdated;
            _diceRollUseCase.DiceRollPhaseChanged += OnDiceRollPhaseChanged;
            _diceRollUseCase.DiceReset += OnDiceReset;
            _diceMergeUseCase.MergeStateChanged += OnMergeStateChanged;
            _diceHoverUseCase.DiceHoverDetailsUpdated += OnDiceHoverDetailsUpdated;
        }

        public void Detach()
        {
            _encounterProgressionUseCase.EncounterSnapshotUpdated -= OnEncounterSnapshotUpdated;
            _diceRollUseCase.DiceRollPhaseChanged -= OnDiceRollPhaseChanged;
            _diceRollUseCase.DiceReset -= OnDiceReset;
            _diceMergeUseCase.MergeStateChanged -= OnMergeStateChanged;
            _diceHoverUseCase.DiceHoverDetailsUpdated -= OnDiceHoverDetailsUpdated;

            _view = null;
        }

        public void RefreshRollStateFromState()
        {
            if (_view == null)
            {
                return;
            }

            UpdateRollResultText();
            _view.SetTurnInfo($"Turn: {_diceSessionState.CurrentTurn}/{_diceSessionState.MaxTurns}");
            _view.SetDealButtonInteractable(CanDealThisTurn());
        }

        private void OnEncounterSnapshotUpdated(EncounterSnapshot snapshot)
        {
            if (_view == null || snapshot == null)
            {
                return;
            }

            if (snapshot.Phase != EncounterPhase.Active)
            {
                _view.SetResultInfo("Waiting to roll...");
                _view.SetDealButtonInteractable(false);
                return;
            }

            _view.SetResultInfo("Ready to roll.");
            _view.SetTurnInfo($"Turn: {_diceSessionState.CurrentTurn}/{_diceSessionState.MaxTurns}");
            _view.SetDealButtonInteractable(false);
        }

        private void OnDiceRollPhaseChanged(DiceRollPhase phase)
        {
            if (_view == null)
            {
                return;
            }

            if (phase == DiceRollPhase.ResolvingResult)
            {
                UpdateRollResultText();
                _view.SetDealButtonInteractable(false);
                return;
            }

            if (phase != DiceRollPhase.Completed)
            {
                return;
            }

            UpdateRollResultText();
            _view.SetDealButtonInteractable(CanDealThisTurn());
        }

        private void OnDiceReset(DiceResetEvent evt)
        {
            if (_view == null)
            {
                return;
            }

            _view.SetResultInfo("Waiting to roll...");
            _view.SetDealButtonInteractable(false);
        }

        private void OnMergeStateChanged(MergeState mergeState)
        {
            if (_view == null || mergeState != MergeState.Applied)
            {
                return;
            }

            UpdateRollResultText();
            _view.SetDealButtonInteractable(CanDealThisTurn());
        }

        private void OnDiceHoverDetailsUpdated(DiceHoverDetailsUpdatedEvent evt)
        {
            if (_view == null)
            {
                return;
            }

            if (!evt.HasDetails)
            {
                UpdateRollResultText();
                return;
            }

            _view.SetResultInfo(
                $"Dice {evt.DiceId}\n" +
                $"Value: {evt.CurrentValue}\n" +
                $"Level: {evt.Level}\n" +
                $"Damage: {evt.Damage}");
        }

        private void UpdateRollResultText()
        {
            _view?.SetResultInfo($"Rerolls Left: {_diceSessionState.RerollsLeft}\nHover a die to inspect Value, Level and Damage.");
        }

        private bool CanDealThisTurn()
        {
            if (_diceSessionState.HasDealtThisTurn)
            {
                return false;
            }

            if (_diceSessionState.CurrentTurn > _diceSessionState.MaxTurns)
            {
                return false;
            }

            foreach (DiceState die in _diceSessionState.ActiveDice)
            {
                if (die.CurrentFaceIndex >= 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}


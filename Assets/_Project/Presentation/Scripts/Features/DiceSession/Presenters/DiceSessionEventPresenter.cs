using _Project.Application.Interfaces;

namespace _Project.Presentation.Scripts.Features.DiceSession.Presenters
{
    /// <summary>
    /// Coordinates the dice session view presenters for encounter rendering, roll state, and combat feedback.
    /// </summary>
    public class DiceSessionEventPresenter : IDiceSessionPresenterLifecycle
    {
        private readonly DiceSessionEncounterViewPresenter _encounterViewPresenter;
        private readonly DiceSessionRollStatePresenter _rollStatePresenter;
        private readonly DiceSessionCombatFeedbackPresenter _combatFeedbackPresenter;

        public DiceSessionEventPresenter(
            DiceSessionEncounterViewPresenter encounterViewPresenter,
            DiceSessionRollStatePresenter rollStatePresenter,
            DiceSessionCombatFeedbackPresenter combatFeedbackPresenter)
        {
            _encounterViewPresenter = encounterViewPresenter;
            _rollStatePresenter = rollStatePresenter;
            _combatFeedbackPresenter = combatFeedbackPresenter;
        }

        public void Attach(IDiceSessionView view)
        {
            _encounterViewPresenter.Attach(view);
            _rollStatePresenter.Attach(view);
            _combatFeedbackPresenter.Attach(view);
        }

        public void Detach()
        {
            _encounterViewPresenter.Detach();
            _rollStatePresenter.Detach();
            _combatFeedbackPresenter.Detach();
        }

        public void RefreshEnemyPanelFromState()
        {
            _encounterViewPresenter.RefreshEnemyPanelFromState();
            _rollStatePresenter.RefreshRollStateFromState();
        }

        public void SetDealButtonInteractable(bool isInteractable)
        {
            _combatFeedbackPresenter.SetDealButtonInteractable(isInteractable);
        }
    }
}


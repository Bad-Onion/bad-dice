using _Project.Application.Interfaces;

namespace _Project.Presentation.Scripts.Features.DiceSession.Presenters
{
    /// <summary>
    /// Facade for the DiceSession presenters, providing a unified interface for the view to interact with both command and event presenters.
    /// </summary>
    public class DiceSessionPresenterFacade : IDiceSessionPresenterLifecycle
    {
        private readonly DiceSessionCommandPresenter _commandPresenter;
        private readonly DiceSessionEventPresenter _eventPresenter;

        public DiceSessionPresenterFacade(
            DiceSessionCommandPresenter commandPresenter,
            DiceSessionEventPresenter eventPresenter)
        {
            _commandPresenter = commandPresenter;
            _eventPresenter = eventPresenter;
        }

        public void Attach(IDiceSessionView view)
        {
            _eventPresenter.Attach(view);
        }

        public void Detach()
        {
            _eventPresenter.Detach();
        }

        public void RequestRoll()
        {
            _commandPresenter.RequestRoll();
        }

        public void RequestReset()
        {
            _commandPresenter.RequestReset();
        }

        public void RequestDealDamage()
        {
            _commandPresenter.RequestDealDamage();
            _eventPresenter.SetDealButtonInteractable(false);
        }

        public void RefreshEnemyPanelFromState()
        {
            _eventPresenter.RefreshEnemyPanelFromState();
        }
    }
}

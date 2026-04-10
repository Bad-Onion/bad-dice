using _Project.Application.Events.Core;
using _Project.Application.Events.EncounterState;
using _Project.Application.Events.GameState;
using _Project.Application.Interfaces;

namespace _Project.Presentation.Scripts.Features.DiceSession.Presenters
{
    public class DiceSessionCombatFeedbackPresenter : IDiceSessionPresenterLifecycle
    {
        private IDiceSessionView _view;

        public void Attach(IDiceSessionView view)
        {
            _view = view;

            Bus<EnemyDamagedEvent>.OnEvent += OnEnemyDamaged;
            Bus<EnemyDefeatedEvent>.OnEvent += OnEnemyDefeated;
            Bus<RunCompletedEvent>.OnEvent += OnRunCompleted;
            Bus<TurnChangedEvent>.OnEvent += OnTurnChanged;
        }

        public void Detach()
        {
            Bus<EnemyDamagedEvent>.OnEvent -= OnEnemyDamaged;
            Bus<EnemyDefeatedEvent>.OnEvent -= OnEnemyDefeated;
            Bus<RunCompletedEvent>.OnEvent -= OnRunCompleted;
            Bus<TurnChangedEvent>.OnEvent -= OnTurnChanged;

            _view = null;
        }

        public void SetDealButtonInteractable(bool isInteractable)
        {
            _view?.SetDealButtonInteractable(isInteractable);
        }

        private void OnEnemyDamaged(EnemyDamagedEvent evt)
        {
            _view?.SetEnemyHealth($"HP: {evt.RemainingHealth}/{evt.MaxHealth}");
        }

        private void OnEnemyDefeated(EnemyDefeatedEvent evt)
        {
            if (_view == null)
            {
                return;
            }

            _view.SetEnemyHealth("HP: 0/0");
            _view.SetResultInfo($"{evt.EnemyName} defeated. Preparing next encounter...");
            _view.SetDealButtonInteractable(false);
        }

        private void OnRunCompleted(RunCompletedEvent evt)
        {
            if (_view == null)
            {
                return;
            }

            _view.SetResultInfo("Lucifer has fallen. Run completed!");
            _view.SetDealButtonInteractable(false);
        }

        private void OnTurnChanged(TurnChangedEvent evt)
        {
            if (_view == null)
            {
                return;
            }

            _view.SetTurnInfo($"Turn: {evt.CurrentTurn}/{evt.MaxTurns}");
            _view.SetDealButtonInteractable(false);
        }
    }
}


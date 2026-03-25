using _Project.Application.Events.Core;
using _Project.Application.Events.DiceInput;
using _Project.Application.Events.DiceSimulation;
using _Project.Application.Events.DiceState;
using _Project.Application.Events.EncounterState;
using _Project.Application.Events.GameState;
using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.Dice.Session;
using _Project.Domain.Features.GameFlow.ScriptableObjects.Settings;
using _Project.Presentation.Scripts.Shared.AbstractViews;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace _Project.Presentation.Scripts.Features.DiceSession.Views
{
    public partial class DiceSessionView : BaseView
    {
        private Button _rollButton;
        private Button _resetButton;
        private Button _dealButton;
        private Label _resultLabel;
        private Label _enemyNameLabel;
        private Label _enemyHealthLabel;
        private Label _cycleLabel;
        private Label _turnLabel;
        private VisualElement _levelContainer;

        private DiceSessionState _diceSessionState;
        private EnemyEncounterState _enemyEncounterState;
        private GameConfiguration _gameConfiguration;
        private bool _isUiInitialized;

        [Inject]
        public void Construct(
            DiceSessionState diceSessionState,
            EnemyEncounterState enemyEncounterState,
            GameConfiguration gameConfiguration)
        {
            _diceSessionState = diceSessionState;
            _enemyEncounterState = enemyEncounterState;
            _gameConfiguration = gameConfiguration;
        }

        protected override void BindUIElements()
        {
            if (UiContainer == null) return;
            if (!TryCacheUiElements()) return;

            SubscribeUiActions();
            SubscribeEvents();

            RefreshEnemyPanelFromState();
        }

        protected override void UnbindUIElements()
        {
            if (!_isUiInitialized) return;

            UnsubscribeUiActions();
            UnsubscribeEvents();
            _isUiInitialized = false;
        }

        private bool TryCacheUiElements()
        {
            _levelContainer = UiContainer.Q<VisualElement>("level-container");
            _rollButton = UiContainer.Q<Button>("roll-button");
            _resetButton = UiContainer.Q<Button>("reset-button");
            _dealButton = UiContainer.Q<Button>("deal-button");
            _resultLabel = UiContainer.Q<Label>("result-label");
            _enemyNameLabel = UiContainer.Q<Label>("enemy-name-label");
            _enemyHealthLabel = UiContainer.Q<Label>("enemy-health-label");
            _cycleLabel = UiContainer.Q<Label>("cycle-label");
            _turnLabel = UiContainer.Q<Label>("turn-label");

            _isUiInitialized = _levelContainer != null &&
                               _rollButton != null &&
                               _resetButton != null &&
                               _dealButton != null &&
                               _resultLabel != null &&
                               _enemyNameLabel != null &&
                               _enemyHealthLabel != null &&
                               _cycleLabel != null &&
                               _turnLabel != null;

            if (_isUiInitialized) return true;

            Debug.LogError("DiceSessionView could not bind all required UI Toolkit elements.", this);
            return false;
        }

        private void SubscribeUiActions()
        {
            _rollButton.clicked += RaiseRollRequested;
            _resetButton.clicked += RaiseResetRequested;
            _dealButton.clicked += HandleDealClicked;
        }

        private void UnsubscribeUiActions()
        {
            _rollButton.clicked -= RaiseRollRequested;
            _resetButton.clicked -= RaiseResetRequested;
            _dealButton.clicked -= HandleDealClicked;
        }

        private void SubscribeEvents()
        {
            Bus<EncounterStartedEvent>.OnEvent += OnEncounterStarted;
            Bus<EncounterPreparedEvent>.OnEvent += OnEncounterPrepared;
            Bus<EnemyDamagedEvent>.OnEvent += OnEnemyDamaged;
            Bus<EnemyDefeatedEvent>.OnEvent += OnEnemyDefeated;
            Bus<RunCompletedEvent>.OnEvent += OnRunCompleted;
            Bus<TurnChangedEvent>.OnEvent += OnTurnChanged;
            Bus<DiceResultDecidedEvent>.OnEvent += OnResultDecided;
            Bus<DiceRollFinishedEvent>.OnEvent += OnRollFinished;
            Bus<DiceResetEvent>.OnEvent += OnDiceReset;
            Bus<MergeCompletedEvent>.OnEvent += OnMergeCompleted;
            Bus<DiceHoverDetailsUpdatedEvent>.OnEvent += OnDiceHoverDetailsUpdated;
        }

        private void UnsubscribeEvents()
        {
            Bus<EncounterStartedEvent>.OnEvent -= OnEncounterStarted;
            Bus<EncounterPreparedEvent>.OnEvent -= OnEncounterPrepared;
            Bus<EnemyDamagedEvent>.OnEvent -= OnEnemyDamaged;
            Bus<EnemyDefeatedEvent>.OnEvent -= OnEnemyDefeated;
            Bus<RunCompletedEvent>.OnEvent -= OnRunCompleted;
            Bus<TurnChangedEvent>.OnEvent -= OnTurnChanged;
            Bus<DiceResultDecidedEvent>.OnEvent -= OnResultDecided;
            Bus<DiceRollFinishedEvent>.OnEvent -= OnRollFinished;
            Bus<DiceResetEvent>.OnEvent -= OnDiceReset;
            Bus<MergeCompletedEvent>.OnEvent -= OnMergeCompleted;
            Bus<DiceHoverDetailsUpdatedEvent>.OnEvent -= OnDiceHoverDetailsUpdated;
        }
    }
}
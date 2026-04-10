using _Project.Application.Commands;
using _Project.Application.States.Encounter;
using _Project.Application.UseCases;
using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Run.Session;
using _Project.Presentation.Scripts.Shared.AbstractViews;
using UnityEngine.UIElements;
using Zenject;

namespace _Project.Presentation.Scripts.Features.Inventory.Views
{
    public class DicePouchSelectorView : BaseView
    {
        private IDicePouchUseCase _dicePouchUseCase;
        private PlayerRunState _runState;
        private CommandProcessor _commandProcessor;
        private StartEncounterCommand _startEncounterCommand;
        private IEncounterProgressionUseCase _encounterProgressionUseCase;

        private ScrollView _inventoryList;
        private Button _startButton;
        private VisualElement _container;

        [Inject]
        public void Construct(
            IDicePouchUseCase dicePouchUseCase,
            PlayerRunState runState,
            CommandProcessor commandProcessor,
            StartEncounterCommand startEncounterCommand,
            IEncounterProgressionUseCase encounterProgressionUseCase)
        {
            _dicePouchUseCase = dicePouchUseCase;
            _runState = runState;
            _commandProcessor = commandProcessor;
            _startEncounterCommand = startEncounterCommand;
            _encounterProgressionUseCase = encounterProgressionUseCase;
        }

        protected override void BindUIElements()
        {
            if (UiContainer == null) return;

            _container = UiContainer.Q<VisualElement>("pre-fight-container");
            _inventoryList = UiContainer.Q<ScrollView>("inventory-list");
            _startButton = UiContainer.Q<Button>("start-fight-button");

            _startButton.clicked += OnStartClicked;
            _encounterProgressionUseCase.EncounterSnapshotUpdated += HandleEncounterSnapshotUpdated;

            PopulateInventory();
        }

        protected override void UnbindUIElements()
        {
            _startButton.clicked -= OnStartClicked;
            _encounterProgressionUseCase.EncounterSnapshotUpdated -= HandleEncounterSnapshotUpdated;
        }

        private void PopulateInventory()
        {
            _inventoryList.Clear();

            foreach (var dice in _runState.DiceInventory)
            {
                AddDiceToInventoryList(dice);
            }
        }

        private void AddDiceToInventoryList(OwnedDiceData dice)
        {
            Toggle diceToggle = new Toggle($"{dice.Dice.Definition.name}");

            diceToggle.value = dice.IsEquipped;
            diceToggle.RegisterValueChangedCallback(_ =>
            {
                _dicePouchUseCase.ToggleDiceEquip(dice.Dice.Id);
                diceToggle.SetValueWithoutNotify(dice.IsEquipped);
            });

            _inventoryList.Add(diceToggle);
        }

        private void OnStartClicked()
        {
            _commandProcessor.ExecuteCommand(_startEncounterCommand);
        }

        private void HandleEncounterSnapshotUpdated(EncounterSnapshot snapshot)
        {
            if (snapshot == null || snapshot.Phase != EncounterPhase.Active) return;
            _container.style.display = DisplayStyle.None;
        }
    }
}
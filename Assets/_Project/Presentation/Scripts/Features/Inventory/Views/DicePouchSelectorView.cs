using _Project.Application.Events.Core;
using _Project.Application.Events.EncounterState;
using _Project.Application.UseCases;
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

        private ScrollView _inventoryList;
        private Button _startButton;
        private VisualElement _container;

        [Inject]
        public void Construct(IDicePouchUseCase dicePouchUseCase, PlayerRunState runState)
        {
            _dicePouchUseCase = dicePouchUseCase;
            _runState = runState;
        }

        protected override void BindUIElements()
        {
            if (UiContainer == null) return;

            _container = UiContainer.Q<VisualElement>("pre-fight-container");
            _inventoryList = UiContainer.Q<ScrollView>("inventory-list");
            _startButton = UiContainer.Q<Button>("start-fight-button");

            _startButton.clicked += OnStartClicked;

            Bus<EncounterStartedEvent>.OnEvent += HideDialog;

            PopulateInventory();
        }

        protected override void UnbindUIElements()
        {
            _startButton.clicked -= OnStartClicked;
            Bus<EncounterStartedEvent>.OnEvent -= HideDialog;
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

        private static void OnStartClicked()
        {
            Bus<EncounterStartRequestedEvent>.Raise(new EncounterStartRequestedEvent());
        }

        private void HideDialog(EncounterStartedEvent evt)
        {
            _container.style.display = DisplayStyle.None;
        }
    }
}
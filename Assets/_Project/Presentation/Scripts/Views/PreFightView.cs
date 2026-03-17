using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
using UnityEngine.UIElements;
using Zenject;

namespace _Project.Presentation.Scripts.Views
{
    public class PreFightView : BaseView
    {
        private IEncounterPreparationUseCase _preparationUseCase;
        private PlayerRunState _runState;

        private ScrollView _inventoryList;
        private Button _startButton;
        private VisualElement _container;

        [Inject]
        public void Construct(IEncounterPreparationUseCase preparationUseCase, PlayerRunState runState)
        {
            _preparationUseCase = preparationUseCase;
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

            foreach (var dice in _runState.Inventory)
            {
                Toggle diceToggle = new Toggle($"{dice.Definition.name} (Lv {dice.Level})");
                diceToggle.value = dice.IsEquipped;
                diceToggle.RegisterValueChangedCallback(evt =>
                {
                    _preparationUseCase.ToggleDiceEquip(dice.Id);
                    diceToggle.SetValueWithoutNotify(dice.IsEquipped); // Enforce max limit constraint visually
                });

                _inventoryList.Add(diceToggle);
            }
        }

        private void OnStartClicked()
        {
            _preparationUseCase.StartEncounter();
        }

        private void HideDialog(EncounterStartedEvent evt)
        {
            _container.style.display = DisplayStyle.None;
        }
    }
}
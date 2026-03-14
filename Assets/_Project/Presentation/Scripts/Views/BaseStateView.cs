using UnityEngine;
using System;
using _Project.Application.Events;
using UnityEngine.UIElements;

namespace _Project.Presentation.Scripts.Views
{
    public abstract class BaseStateView : MonoBehaviour
    {
        [Tooltip("The exact class name of the target state, e.g., MainMenuState")]
        [SerializeField] private string targetStateName;

        [SerializeField] private GameStateEventChannel eventChannel;
        [SerializeField] protected UIDocument uiDocument;
        [SerializeField] protected string containerId;

        protected VisualElement UiContainer { get; private set; }


        protected virtual void OnEnable()
        {
            AwakeUiDocument();

            eventChannel.OnEventRaised += HandleStateChanged;
        }

        protected virtual void OnDisable()
        {
            eventChannel.OnEventRaised -= HandleStateChanged;

            UnbindUIElements();
        }

        protected virtual void BindUIElements() { }

        protected virtual void UnbindUIElements() { }

        private void HandleStateChanged(Type stateType)
        {
            if (UiContainer == null) return;

            bool isActive = stateType.Name == targetStateName;
            UiContainer.style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void AwakeUiDocument()
        {
            if (uiDocument == null) return;
            if (uiDocument.rootVisualElement == null) return;

            UiContainer = uiDocument.rootVisualElement.Q<VisualElement>(containerId);

            BindUIElements();
        }
    }
}
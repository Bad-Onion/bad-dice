using UnityEngine;
using System;
using _Project.Application.Events;

namespace _Project.Presentation.Scripts.Views
{
    public abstract class BaseStateView : MonoBehaviour
    {
        [Tooltip("The exact class name of the target state, e.g., MainMenuState")]
        [SerializeField] private string targetStateName;

        [SerializeField] private GameStateEventChannel eventChannel;
        [SerializeField] private GameObject visualPanel;

        protected virtual void OnEnable()
        {
            eventChannel.OnEventRaised += HandleStateChanged;
        }

        protected virtual void OnDisable()
        {
            eventChannel.OnEventRaised -= HandleStateChanged;
        }

        private void HandleStateChanged(Type stateType)
        {
            visualPanel.SetActive(stateType.Name == targetStateName);
        }
    }
}
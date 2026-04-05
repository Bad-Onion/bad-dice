using UnityEngine;
using UnityEngine.Events;

namespace _Project.Application.Events.Core
{
    /// <summary>
    /// A generic ScriptableObject that serves as an event channel for events with a parameter of type T.
    /// </summary>
    /// <typeparam name="T">The type of the parameter that will be passed when the event is raised.</typeparam>
    public abstract class GenericEventChannelSO<T> : ScriptableObject
    {
        [Tooltip("The action to perform; Listeners subscribe to this UnityAction")]
        public UnityAction<T> OnEventRaised;

        public void RaiseEvent(T parameter)
        {
            OnEventRaised?.Invoke(parameter);
        }
    }
}
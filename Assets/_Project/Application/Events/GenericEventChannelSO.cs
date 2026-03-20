using UnityEngine;
using UnityEngine.Events;

namespace _Project.Application.Events
{
    // TODO: Move abstract event to a more appropriate location like inside a "Events/Core" folder
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
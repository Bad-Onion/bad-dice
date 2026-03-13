using UnityEngine;
using UnityEngine.Events;
using _Project.Application.Events;

namespace _Project.Presentation.Scripts.ChannelListeners
{
    // Use when we have an OnPlayerDied event to trigger an audio or else
    public abstract class AbstractEventChannelListener<TEventChannel, TEventType> : MonoBehaviour
        where TEventChannel : GenericEventChannelSO<TEventType>
    {
        [Header("Listen to Event Channels")]
        [SerializeField] protected TEventChannel eventChannel;

        [Tooltip("Responds to receiving signal from Event Channel")]
        [SerializeField] protected UnityEvent<TEventType> response;

        protected virtual void OnEnable()
        {
            if (eventChannel != null)
            {
                eventChannel.OnEventRaised += OnEventRaised;
            }
        }

        protected virtual void OnDisable()
        {
            if (eventChannel != null)
            {
                eventChannel.OnEventRaised -= OnEventRaised;
            }
        }

        public void OnEventRaised(TEventType evt)
        {
            response?.Invoke(evt);
        }
    }
}
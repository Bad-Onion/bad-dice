using System;

namespace _Project.Application.Events.EventChannels.Payload
{
    public struct TransitionPayload
    {
        public bool FadeToBlack { get; }
        public float Duration { get; }
        public Action OnComplete { get; }

        /// <param name="fadeToBlack">True to fade out to a black screen, False to fade in to the game.</param>
        /// <param name="duration">Duration of the fade in seconds.</param>
        /// <param name="onComplete">Callback executed when the fade finishes.</param>
        public TransitionPayload(bool fadeToBlack, float duration, Action onComplete = null)
        {
            FadeToBlack = fadeToBlack;
            Duration = duration;
            OnComplete = onComplete;
        }
    }
}
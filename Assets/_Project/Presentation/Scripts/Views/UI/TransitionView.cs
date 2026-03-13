using UnityEngine;
using System;
using System.Collections;
using _Project.Application.Events;
using _Project.Application.Events.Payload;

namespace _Project.Presentation.Scripts.Views.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TransitionView : MonoBehaviour
    {
        [SerializeField] private TransitionEventChannel transitionEventChannel;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 1f; // Start blacked out
        }

        private void OnEnable()
        {
            if (transitionEventChannel != null) transitionEventChannel.OnEventRaised += HandleTransition;
        }

        private void OnDisable()
        {
            if (transitionEventChannel != null) transitionEventChannel.OnEventRaised -= HandleTransition;
        }

        private void HandleTransition(TransitionPayload payload)
        {
            StopAllCoroutines();
            StartCoroutine(FadeRoutine(payload.FadeToBlack, payload.Duration, payload.OnComplete));
        }

        private IEnumerator FadeRoutine(bool fadeToBlack, float duration, Action onComplete)
        {
            float startAlpha = _canvasGroup.alpha;
            float targetAlpha = fadeToBlack ? 1f : 0f;
            float elapsed = 0f;

            BlockClicks(true);

            if (duration > 0f)
            {
                while (elapsed < duration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);

                    yield return null;
                }
            }

            _canvasGroup.alpha = targetAlpha;
            BlockClicks(fadeToBlack);

            onComplete?.Invoke();
        }

        private void BlockClicks(bool block) => _canvasGroup.blocksRaycasts = block;
    }
}
using System;
using System.Collections;
using _Project.Application.Events.EventChannels;
using _Project.Application.Events.EventChannels.Payload;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Presentation.Scripts.Features.Menu.Views
{
    public class TransitionView : MonoBehaviour
    {
        [SerializeField] private TransitionEventChannel transitionEventChannel;
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private string transitionOverlayId = "transition-overlay";

        private VisualElement _overlay;

        private void OnEnable()
        {
            BindVisualElement();
            SetOverlayInitialState();

            transitionEventChannel.OnEventRaised += HandleTransition;
        }

        private void OnDisable()
        {
            transitionEventChannel.OnEventRaised -= HandleTransition;
        }

        private void HandleTransition(TransitionPayload payload)
        {
            StopAllCoroutines();
            StartCoroutine(FadeRoutine(payload.FadeToBlack, payload.Duration, payload.OnComplete));
        }

        private IEnumerator FadeRoutine(bool fadeToBlack, float duration, Action onComplete)
        {
            if (_overlay == null) yield break;

            float startAlpha = _overlay.style.opacity.value;
            float targetAlpha = fadeToBlack ? 1f : 0f;
            float elapsed = 0f;

            BlockClicks(true);

            if (duration > 0f)
            {
                while (elapsed < duration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    _overlay.style.opacity = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);

                    yield return null;
                }
            }

            _overlay.style.opacity = targetAlpha;
            BlockClicks(fadeToBlack);

            onComplete?.Invoke();
        }

        private void BlockClicks(bool block)
        {
            if (block) _overlay.pickingMode = PickingMode.Position;
            else _overlay.pickingMode = PickingMode.Ignore;
        }

        private void BindVisualElement()
        {
            if (uiDocument == null) return;
            if (uiDocument.rootVisualElement == null) return;

            _overlay = uiDocument.rootVisualElement.Q<VisualElement>(transitionOverlayId);
        }

        private void SetOverlayInitialState()
        {
            if (_overlay == null) return;

            _overlay.style.opacity = 1f;
            BlockClicks(true);
        }
    }
}
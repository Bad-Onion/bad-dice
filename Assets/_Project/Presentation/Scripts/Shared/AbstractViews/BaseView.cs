using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Presentation.Scripts.Shared.AbstractViews
{
    public abstract class BaseView : MonoBehaviour
    {
        [SerializeField] protected UIDocument uiDocument;
        [Tooltip("Leave empty to bind to the root element")]
        [SerializeField] protected string containerId;

        protected VisualElement UiContainer { get; private set; }

        protected virtual void OnEnable()
        {
            if (uiDocument == null || uiDocument.rootVisualElement == null) return;

            UiContainer = string.IsNullOrEmpty(containerId)
                ? uiDocument.rootVisualElement
                : uiDocument.rootVisualElement.Q<VisualElement>(containerId);

            BindUIElements();
        }

        protected virtual void OnDisable()
        {
            UnbindUIElements();
        }

        protected abstract void BindUIElements();
        protected abstract void UnbindUIElements();
    }
}
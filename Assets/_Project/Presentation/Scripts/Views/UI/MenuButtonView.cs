using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace _Project.Presentation.Scripts.Views.UI
{
    [RequireComponent(typeof(Button))]
    public class MenuButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler
    {
        [SerializeField] private TMP_Text buttonText;

        [Header("Colors")] [SerializeField] private Color normalColor = Color.black;
        [SerializeField] private Color hoverColor = new Color(0.149f, 0.149f, 0.149f, 1f);
        [SerializeField] private Color pressedColor = new Color(0.275f, 0.275f, 0.275f, 1f);

        [Header("Scale")] [SerializeField] private Vector3 normalScale = Vector3.one;
        [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
        [SerializeField] private Vector3 pressedScale = new Vector3(0.95f, 0.95f, 0.95f);

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.transition = Selectable.Transition.None;
        }

        private void OnEnable() => ResetState();
        private void OnDisable() => ResetState();

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_button.interactable) return;

            buttonText.color = hoverColor;
            transform.localScale = hoverScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_button.interactable) return;
            ResetState();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_button.interactable) return;

            buttonText.color = pressedColor;
            transform.localScale = pressedScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_button.interactable) return;

            buttonText.color = hoverColor;
            transform.localScale = hoverScale;
        }

        private void ResetState()
        {
            if (buttonText != null) buttonText.color = normalColor;
            transform.localScale = normalScale;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class EventButton : MonoBehaviour
    {
        private Button _button;
        private GameObject _gameObject;
        private Action _onClick;
        private Action _additionalCallback;

        private void OnDestroy()
        {
            if (_button == null)
                return;

            _button.onClick.RemoveListener(OnClick);
        }

        public void Initialize(Action onClick)
        {
            _gameObject = gameObject;
            _button = GetComponent<Button>();
            _onClick = onClick;
            OnInitialize();

            _button.onClick.AddListener(OnClick);
        }

        public virtual void OnInitialize()
        {
        }

        public void RegisterEvent(Action callback)
        {
            _additionalCallback += callback;
        }

        public void SetInteractable(bool value)
        {
            _button.interactable = value;
        }

        public void SetActive(bool value)
        {
            _gameObject.SetActive(value);
        }

        private void OnClick()
        {
            _onClick.Invoke();
            _additionalCallback?.Invoke();
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class WindowToggleButton<T> : MonoBehaviour
        where T : Enum
    {
        [SerializeField] private Screen _screen;
        [SerializeField] private T _target;

        private Button _button;
        private Action _onInteract;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Interact);
            _screen.Initialize();
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(Interact);
        }

        public void RegisterEvent(Action onInteract)
        {
            _onInteract += onInteract;
        }

        public void Dispose()
        {
            _onInteract = null;
        }

        public virtual void OnInteract()
        {
        }

        private void Interact()
        {
            OnInteract();
            _onInteract?.Invoke();
            _screen.SetWindow(Convert.ToInt32(_target));
        }
    }
}
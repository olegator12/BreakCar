using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WindowClearButton : MonoBehaviour
    {
        [SerializeField] private Screen _screen;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Interact);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(Interact);
        }

        private void Interact()
        {
            _screen.Clear();
        }
    }
}
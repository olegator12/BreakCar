using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindow : MonoBehaviour
    {
        private const float MinValue = 0f;
        private const float MaxValue = 1f;

        [SerializeField] private List<GameObject> _enableOnOpen;
        [SerializeField] private List<GameObject> _disableOnClose;

        private CanvasGroup _group;

        public void Initialize()
        {
            _group = GetComponent<CanvasGroup>();
        }

        public void Open()
        {
            foreach (GameObject gameObject in _enableOnOpen)
                gameObject.SetActive(true);

            _group.alpha = MaxValue;
            _group.interactable = true;
            _group.blocksRaycasts = true;
        }

        public void Close()
        {
            foreach (GameObject gameObject in _disableOnClose)
                gameObject.SetActive(false);

            _group.alpha = MinValue;
            _group.interactable = false;
            _group.blocksRaycasts = false;
        }
    }
}
using UnityEngine;

namespace UI
{
    public class Screen : MonoBehaviour
    {
        [SerializeField] private UIWindow[] _windows;

        private UIWindow _current;
        private bool _didInitialize;

        public void Initialize()
        {
            if (_didInitialize == true)
                return;

            _didInitialize = true;

            foreach (UIWindow window in _windows)
                window.Initialize();
        }

        public void SetWindow(int id)
        {
            if (_current != null)
                _current.Close();

            _current = _windows[id];
            _current.Open();
        }

        public void Clear()
        {
            if (_current == null)
                return;

            _current.Close();
            _current = null;
        }
    }
}
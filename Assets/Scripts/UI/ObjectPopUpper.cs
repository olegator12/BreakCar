using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class ObjectPopUpper : MonoBehaviour
    {
        [SerializeField] private float _scale = 1.1f;
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private bool _isIndependent = true;

        private RectTransform _transform;
        private Tween _tween;
        private Vector2 _target;
        private Vector2 _startSize;

        private void Awake()
        {
            Prepare();

            if (_isIndependent == true)
                PopUp();
        }

        public void PopUp()
        {
            _tween = _transform.DOSizeDelta(_target, _animationDuration).SetLoops(-1, LoopType.Yoyo);
        }

        public void Stop()
        {
            if (_transform == null)
                return;

            _tween.Kill();
            _transform.sizeDelta = _startSize;
        }

        private void Prepare()
        {
            _transform = (RectTransform)transform;
            _startSize = _transform.sizeDelta;
            _target = _startSize * _scale;
        }
    }
}
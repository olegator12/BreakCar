using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TutorialAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform;
        [SerializeField] private RectTransform _targetPanel;
        [SerializeField] private RectTransform _targetCar;
        [SerializeField] private Image _image;
        [SerializeField] private float _firstStepSpeed = 0.7f;
        [SerializeField] private float _secondStepSpeed = 0.65f;

        private Sequence _sequence;
        private GameObject _finger;

        public bool IsActive { get; private set; }

        public void Initialize()
        {
            _finger = _transform.gameObject;
            _finger.SetActive(true);
            IsActive = true;
            _sequence = DOTween.Sequence();

            _sequence
                .Append(_transform.DOMove(_targetPanel.position, _firstStepSpeed))
                .Join(_image.DOFade(0f, _firstStepSpeed));

            _sequence
                .Append(_transform.DOMove(_targetCar.position, _secondStepSpeed))
                .Join(_image.DOFade(1f, _secondStepSpeed));

            _sequence.SetLoops(-1, LoopType.Restart);
            _sequence.Play();
        }

        public void Stop()
        {
            _sequence.Kill();
            _finger.SetActive(false);
        }
    }
}
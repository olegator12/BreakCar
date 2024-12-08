using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace UI
{
    public class CarCard : MonoBehaviour
    {
        private const float PingInterval = 0.08f;
        private const int PingCount = 2;
        private const string Dollar = "$";

        private readonly TimeSpan _pingDelay = TimeSpan.FromSeconds(PingInterval);

        [SerializeField] private Button _button;
        [SerializeField] private RawImage _image;
        [SerializeField] private Image _background;
        [SerializeField] private float _animationDuration;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Color _bought;
        [SerializeField] private Color _available;
        [SerializeField] private Color _advert;
        [SerializeField] private GameObject _advertIcon;

        private RectTransform _transform;
        private Texture2D _last;
        private GameObject _gameObject;
        private AudioSource _sound;
        private int _id;
        private Action<int> _onClick;
        private Action _onStart;

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void Initialize(AudioSource sound)
        {
            _sound = sound;
            _transform = (RectTransform)transform;
            _gameObject = gameObject;
            _transform.sizeDelta = Vector2.zero;
            _button.onClick.AddListener(OnClick);
        }

        public void SetBought()
        {
            SetDefaults(_bought, false, OnStart, true);
        }

        public void SetAvailable()
        {
            SetDefaults(_available, false, OnStart, true);
        }

        public void SetUnavailable()
        {
            SetDefaults(_available, false, () => DenyAccess(_available).Forget(), false);
        }

        public void SetAdvert(AdvertRegistration registration)
        {
            registration.RegisterCallback(AdvertName.TwoHundredMoneyCar, OnStart);
            SetDefaults(_advert, true, () => YandexGame.RewVideoShow((int)AdvertName.TwoHundredMoneyCar), true);
        }

        public void TurnOff()
        {
            _gameObject.SetActive(false);
        }

        public Tweener Activate(Texture2D texture, int price, int id, Action<int> onClick)
        {
            if (_gameObject.activeSelf == false)
                _gameObject.SetActive(true);

            _transform.localScale = Vector3.zero;
            _id = id;
            _text.SetText($"{price.ToString()}{Dollar}");
            _onClick = onClick;
            _image.texture = texture;

            if (_last != null)
                Destroy(_last);

            _last = texture;

            return _transform.DOScale(Vector3.one, _animationDuration);
        }

        private void SetDefaults(Color color, bool isAdvertActive, Action callback, bool isInteractable)
        {
            _background.color = color;
            _advertIcon.SetActive(isAdvertActive);
            _onStart = callback;
            _button.interactable = isInteractable;
        }

        private void OnClick()
        {
            _sound.Play();
            _onStart.Invoke();
        }

        private void OnStart()
        {
            _onClick.Invoke(_id);
        }

        private async UniTaskVoid DenyAccess(Color start)
        {
            for (int i = 0; i < PingCount; i++)
            {
                _background.color = Color.red;
                await UniTask.Delay(_pingDelay);
                _background.color = start;
            }
        }
    }
}
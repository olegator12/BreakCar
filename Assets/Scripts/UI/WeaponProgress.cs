using System.Collections.Generic;
using System.Threading;
using Configs;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;
using YG;

namespace UI
{
    public class WeaponProgress : MonoBehaviour
    {
        private const int OneHundred = 100;

        [SerializeField] private Screen _main;
        [SerializeField] private float _imageAnimationSpeed;
        [SerializeField] private float _textAnimationSpeed;
        [SerializeField] private WeaponObtainingConfig _weaponObtaining;
        [SerializeField] private WeaponConfiguration _weaponConfiguration;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_Text _textAdvert;
        [SerializeField] private Button _next;
        [SerializeField] private Button _advert;
        [SerializeField] private AdvertRegistration _registration;
        [SerializeField] private CardHolder _holder;

        private CancellationToken _token;
        private Dictionary<WeaponType, WeaponSettings> _config;
        private GameObject _nextObject;
        private GameObject _advertObject;
        private RectTransform _nextTransform;
        private Vector2 _nextAnchorPosition;
        private int _pointer;
        private int _current;
        private float _fillAmount;
        private string _percent;
        private bool _isSkip;

        private void OnDestroy()
        {
            _next.onClick.RemoveListener(OnNextClick);
            _advert.onClick.RemoveListener(OnRewardClick);
        }

        public void Initialize()
        {
            _registration.RegisterCallback(AdvertName.SuperWeapon, ObtainRewardProgress);
            _nextObject = _next.gameObject;
            _advertObject = _advert.gameObject;
            _nextTransform = (RectTransform)_next.transform;
            _nextAnchorPosition = _nextTransform.anchoredPosition;
            _token = destroyCancellationToken;
            _config = _weaponConfiguration.GetConfiguration();

            _next.onClick.AddListener(OnNextClick);
            _advert.onClick.AddListener(OnRewardClick);
        }

        public void Prepare()
        {
            _pointer = YandexGame.savesData.weaponLevelPointer;
            _current = YandexGame.savesData.weaponLevelProgress;

            if (_pointer >= _weaponObtaining.Queue.Count)
                return;

            SerializedPair<WeaponType, int> item = _weaponObtaining.Queue[_pointer];
            _current = Mathf.Clamp(_current, 0, item.Value);
            float currentFill = _current / (float)item.Value;
            _icon.sprite = _config[item.Key].Icon;
            _icon.fillAmount = currentFill;
            _text.SetText(JoinPercent(currentFill));
            _nextObject.SetActive(false);
            _advertObject.SetActive(false);

            YandexGame.savesData.weaponLevelProgress++;
            _current = Mathf.Clamp(YandexGame.savesData.weaponLevelProgress, 0, item.Value);
            _isSkip = YandexGame.savesData.levelCompleteCount < 2;

            _fillAmount = _current / (float)item.Value;
            _percent = JoinPercent(_fillAmount);

            if (_current >= item.Value)
            {
                YandexGame.savesData.weaponLevelPointer++;
                YandexGame.savesData.priorityWeapon = (int)item.Key;
                YandexGame.savesData.openedWeapons.Add((int)item.Key);
                YandexGame.savesData.weaponLevelProgress = 0;
                _pointer = YandexGame.savesData.weaponLevelPointer;
                _current = 0;
            }

            if (_pointer >= _weaponObtaining.Queue.Count)
                return;

            _textAdvert.SetText($"+{JoinPercent(1 / (float)_weaponObtaining.Queue[_pointer].Value)}");
        }

        public async void Play()
        {
            if (_pointer >= _weaponObtaining.Queue.Count && Mathf.Approximately(_fillAmount, 0f) == true)
            {
                OnNextClick();
                return;
            }

            _main.SetWindow((int)GameWindows.Progress);

            List<UniTask> tasks = new List<UniTask>
            {
                _icon.DOFillAmount(_fillAmount, _imageAnimationSpeed).ToUniTask(cancellationToken: _token),
                _text.DOText(_percent, _textAnimationSpeed).ToUniTask(cancellationToken: _token)
            };

            await UniTask.WhenAll(tasks);

            _fillAmount = 0f;
            _nextTransform.anchoredPosition = _nextAnchorPosition;

            if (_isSkip == false && _pointer < _weaponObtaining.Queue.Count)
                _advertObject.SetActive(true);
            else
                _nextTransform.anchoredPosition = Vector2.zero;

            _nextObject.SetActive(true);
        }

        private void OnNextClick()
        {
            _isSkip = false;
            _holder.UpdatePage();
            _main.SetWindow((int)GameWindows.Shop);
            YandexGame.FullscreenShow();
        }

        private void OnRewardClick()
        {
            YandexGame.RewVideoShow((int)AdvertName.SuperWeapon);
            _nextTransform.anchoredPosition = Vector2.zero;
            _advertObject.SetActive(false);
            _nextObject.SetActive(false);
        }

        private async void ObtainRewardProgress()
        {
            Prepare();
            YandexGame.SaveProgress();

            List<UniTask> tasks = new List<UniTask>
            {
                _icon.DOFillAmount(_fillAmount, _imageAnimationSpeed).ToUniTask(cancellationToken: _token),
                _text.DOText(_percent, _textAnimationSpeed).ToUniTask(cancellationToken: _token)
            };

            await UniTask.WhenAll(tasks);
            _nextObject.SetActive(true);
        }

        private string JoinPercent(float value)
        {
            return $"{Mathf.RoundToInt(value * OneHundred)} %";
        }
    }
}
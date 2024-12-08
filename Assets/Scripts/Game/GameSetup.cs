using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Destruction;
using Economic;
using GameAnalyticsSDK;
using TMPro;
using UI;
using UnityEngine;
using Weapons;
using YG;

namespace Game
{
    public class GameSetup : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private GeneralSettings _settings;
        [SerializeField] private AdvertRegistration _registration;
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _casterMask;
        [SerializeField] private LayerMask _casterMaskTrajectory;
        [SerializeField] private Transform _platform;
        [SerializeField] private WeaponSetup _weaponSetup;
        [SerializeField] private WeaponPanel _weaponPanel;
        [SerializeField] private WeaponConfiguration _configuration;
        [SerializeField] private Material _highlighted;
        [SerializeField] private TutorialAnimation _tutorial;

        [Header("Camera Shaker")]
        [SerializeField] private Transform _cameraPoint;
        [SerializeField] private float _shakeDuration;
        [SerializeField] private float _shakeStrength;

        [Header("UI")]
        [SerializeField] private DestructionBar _destructionBar;
        [SerializeField] private AmmunitionBar _ammunitionBar;
        [SerializeField] private PopUpEventButton _next;
        [SerializeField] private GameToggleButton _back;
        [SerializeField] private Booster _boost;
        [SerializeField] private TMP_Text _levelNumber;

        [Header("Pool")]
        [SerializeField] private Coin _coinTemplate;
        [SerializeField] private Transform _poolObjectsHolder;
        [SerializeField] private Transform _clearableHolder;
        [SerializeField] private CoinAnimation _coinAnimation;
        [SerializeField] private SpawnableSound _spawnableSoundTemplate;

        private WeaponPresenter _presenter;
        private WeaponChanger _weaponChanger;
        private DestructionAnalyzer _analyzer;
        private PlatformRotator _platformRotator;
        private EconomicService _economicService;
        private ObjectPool<SpawnableSound> _soundPool;
        private ObjectPool<Coin> _coinPool;

        private Action<int> _onDisableCallback;
        private Action _moneyReturnHandler;
        private Action<AudioSource> _soundHandler;
        private int _id;

        public Action BackCallback { get; private set; }

        private void OnDestroy()
        {
            Disable();
        }

        public PlatformRotator Initialize(
            Action moneyReturnHandler,
            Wallet wallet,
            Action<AudioSource> soundHandler)
        {
            _soundHandler = soundHandler;
            _moneyReturnHandler = moneyReturnHandler;

            _soundPool = new ObjectPool<SpawnableSound>(_spawnableSoundTemplate);
            _coinPool = new ObjectPool<Coin>(_coinTemplate);
            _soundPool.RegisterHolder(_poolObjectsHolder);
            _soundPool.RegisterOnCreate(item => _soundHandler.Invoke(item.GetSource()));
            _coinPool.RegisterHolder(_clearableHolder);
            _coinPool.RegisterOnCreate(item => _soundHandler.Invoke(item.GetSource()));

            Dictionary<WeaponType, Sprite> sprites = _configuration
                .GetConfiguration()
                .ToDictionary(item => item.Key, item => item.Value.Icon);

            _weaponChanger = new WeaponChanger(_weaponSetup.GetWeapon, _weaponPanel);
            _presenter = new WeaponPresenter(
                _settings,
                _registration,
                _boost,
                new Caster(_camera, _casterMask, _casterMaskTrajectory),
                _weaponChanger,
                _ammunitionBar,
                _tutorial);
            _platformRotator = new PlatformRotator(_platform, _settings.PlatformRotationSpeed);

            BackCallback = () =>
            {
                _moneyReturnHandler.Invoke();
                Clear();
                _presenter.Disable();
                Disable();
            };
            _back.RegisterEvent(BackCallback);
            _back.RegisterEvent(
                () =>
                {
                    GameAnalytics.NewProgressionEvent(
                        GAProgressionStatus.Fail,
                        (YandexGame.savesData.levelCompleteCount + 1).ToString());
                    Debug.Log($"GA Level {nameof(GAProgressionStatus.Fail)} {(YandexGame.savesData.levelCompleteCount + 1).ToString()}");
                });
            _next.RegisterEvent(_presenter.Disable);

            _coinAnimation.Initialize(
                wallet,
                () =>
                {
                    Clear();
                    Disable();
                    _presenter.Disable();
                },
                _moneyReturnHandler,
                GetCarId);
            _weaponPanel.Initialize(sprites);
            _weaponSetup.Initialize(_poolObjectsHolder, new CameraShaker(_camera, _cameraPoint, _shakeDuration, _shakeStrength), _soundPool);
            return _platformRotator;
        }

        public void Enable(
            int carPrice,
            CarSetup car,
            Action<int> onDisableCallback,
            int id,
            Action<int> accrualCallback)
        {
            _economicService = new EconomicService(_coinPool, carPrice);
            _onDisableCallback = onDisableCallback;
            _id = id;

            _coinAnimation.Register(_economicService);
            car.Initialize(_economicService, _soundPool).ForEach(item => _soundHandler.Invoke(item));
            _presenter.Register(car.GetHighlighter(_highlighted));

            _analyzer = new DestructionAnalyzer(
                _destructionBar,
                car.GetParts(),
                () =>
                {
                    _next.SetInteractable(true);
                    _next.PopUp();
                },
                car.MinDestroyedPercent);

            _levelNumber.SetText((_id + 1).ToString());
            _presenter.Enable();
            _weaponChanger.ShowPanel();
            GameAnalytics.NewProgressionEvent(
                GAProgressionStatus.Start,
                (YandexGame.savesData.levelCompleteCount + 1).ToString());
            Debug.Log($"GA Level {nameof(GAProgressionStatus.Start)} {(YandexGame.savesData.levelCompleteCount + 1).ToString()}");
        }

        private void Disable()
        {
            _analyzer?.Dispose();
            _analyzer = null;
            _platformRotator.StopRotation();
            _next.Stop();
            _next.SetInteractable(false);
        }

        private void Clear()
        {
            _weaponSetup.Clear();
            _onDisableCallback?.Invoke(_id);

            for (int i = 0; i < _clearableHolder.childCount; i++)
            {
                Transform item = _clearableHolder.GetChild(i);

                if (item.TryGetComponent(out SpawnableObject spawnableObject) == false)
                {
                    Destroy(item.gameObject);
                    continue;
                }

                spawnableObject.Push();
            }
        }

        private int GetCarId()
        {
            return _id;
        }
    }
}
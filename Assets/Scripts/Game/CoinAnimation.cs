using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Economic;
using GameAnalyticsSDK;
using UI;
using UnityEngine;
using YG;
using Random = UnityEngine.Random;
using Screen = UI.Screen;

namespace Game
{
    public class CoinAnimation : MonoBehaviour
    {
        private const int MinMultiplier = 2;
        private const int MaxMultiplier = 3;
        private const string Dollar = "$";

        [SerializeField] private Screen _main;
        [SerializeField] private Camera _camera;
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private RectTransform _moneyIcon;
        [SerializeField] private PopUpEventButton _next;

        [SerializeField] private LabelEventButton _victoryMobile;
        [SerializeField] private LabelEventButton _victoryDesktop;
        [SerializeField] private LabelEventButton _advertMobile;
        [SerializeField] private LabelEventButton _advertDesktop;
        [SerializeField] private AdvertRegistration _registration;
        [SerializeField] private WeaponProgress _weaponProgress;
        [SerializeField] private List<CharacterRandomizer> _character;

        [SerializeField] private CoinIcon _template;
        [SerializeField] private RectTransform _poolHolder;
        [SerializeField] private float _spawnRadius = 170f;
        [SerializeField] private int _prepareCoinCount = 32;

        private ObjectPool<CoinIcon> _pool;
        private EconomicService _economicService;
        private Wallet _wallet;
        private Action _onRestart;
        private Func<int> _carIdHandler;
        private Action _moneyReturnHandler;

        private int _reward;
        private int _advertReward;
        private bool _canInteractWithSell;

        public void Initialize(Wallet wallet, Action onRestart, Action moneyReturnHandler, Func<int> carIdHandler)
        {
            _pool = new ObjectPool<CoinIcon>(_template);
            _pool.RegisterHolder(_poolHolder);
            _wallet = wallet;
            _onRestart = onRestart;
            _carIdHandler = carIdHandler;
            _moneyReturnHandler = moneyReturnHandler;

            List<CoinIcon> preCreated = new List<CoinIcon>();

            for (int i = 0; i < _prepareCoinCount; i++)
                preCreated.Add(_pool.Pull(Vector3.zero));

            foreach (CoinIcon coin in preCreated)
                coin.Initialize().Push();

            _next.Initialize(CollectReward);
            _victoryMobile.Initialize(OnRestart);
            _victoryDesktop.Initialize(OnRestart);
            _advertMobile.Initialize(OnClickAdvert);
            _advertDesktop.Initialize(OnClickAdvert);
            _weaponProgress.Initialize();

            _registration.RegisterCallback(AdvertName.ScaleRewardAfterWin, OnAdvertRestart);
        }

        public void Register(EconomicService service)
        {
            _economicService = service;
        }

        private async void CollectReward()
        {
            _next.Stop();
            _next.SetInteractable(false);
            _canInteractWithSell = true;

            foreach (CharacterRandomizer randomizer in _character)
                randomizer.Randomize();

            _reward = _economicService.CalculateReward();
            _advertReward = _reward * Random.Range(MinMultiplier, MaxMultiplier);
            _weaponProgress.Prepare();

            YandexGame.savesData.beforeEndMoneyCount = YandexGame.savesData.moneyCount + _reward;

            if (YandexGame.savesData.didCompleteTutorial == false)
                YandexGame.savesData.didCompleteTutorial = true;

            int levelID = _carIdHandler.Invoke();

            if (YandexGame.savesData.completedLevels.Contains(levelID) == false)
                YandexGame.savesData.completedLevels.Add(levelID);

            YandexGame.savesData.levelCompleteCount++;
            YandexGame.SaveProgress();

            SetText(_reward, new[] { _victoryDesktop, _victoryMobile });
            SetText(_advertReward, new[] { _advertDesktop, _advertMobile });

            _main.Clear();

            await UniTask.WhenAll(_economicService.MoveCoin(GetIconPosition()));
            _moneyReturnHandler.Invoke();

            _main.SetWindow((int)GameWindows.Win);
        }

        private void SetText(int value, IEnumerable<LabelEventButton> targets)
        {
            foreach (LabelEventButton target in targets)
                target.SetText($"{value}{Dollar}");
        }

        private async void OnRestart()
        {
            await CollectCoins(_reward);
        }

        private async void OnAdvertRestart()
        {
            await CollectCoins(_advertReward);
        }

        private void OnClickAdvert()
        {
            YandexGame.RewVideoShow((int)AdvertName.ScaleRewardAfterWin);
        }

        private async UniTask CollectCoins(int reward)
        {
            if (_canInteractWithSell == false)
                return;

            _canInteractWithSell = false;
            List<UniTask> tasks = new List<UniTask>();

            for (int i = 0; i < _prepareCoinCount; i++)
            {
                Vector2 anchorPosition = new Vector3(
                    Random.Range(-_spawnRadius, _spawnRadius),
                    Random.Range(-_spawnRadius, _spawnRadius));

                tasks.Add(_pool.Pull(Vector3.zero).Move(anchorPosition, _moneyIcon.position));
            }

            await UniTask.WhenAll(tasks);
            _wallet.AddMoney(reward);

            GameAnalytics.NewProgressionEvent(
                GAProgressionStatus.Complete,
                YandexGame.savesData.levelCompleteCount.ToString());
            Debug.Log(
                $"GA Level {nameof(GAProgressionStatus.Complete)} {YandexGame.savesData.levelCompleteCount.ToString()}");

            _onRestart.Invoke();
            _wallet.ApplyModification();
            _weaponProgress.Play();
        }

        private Vector3 GetIconPosition()
        {
            return _camera.ViewportToWorldPoint(
                _uiCamera.WorldToViewportPoint(_moneyIcon.TransformPoint(_moneyIcon.position)));
        }
    }
}
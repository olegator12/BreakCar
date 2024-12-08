using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Cysharp.Threading.Tasks;
using Destruction;
using Economic;
using GameAnalyticsSDK;
using TMPro;
using UI;
using UnityEngine;
using Weapons;
using YG;
using Screen = UI.Screen;

namespace Game
{
    public class Bootstrap : MonoBehaviour
    {
        private const string AB = "level_cap";
        
        [SerializeField] private GameSetup _gameSetup;
        [SerializeField] private Preparer _preparer;
        [SerializeField] private CardHolder _cardHolder;
        [SerializeField] private Screen _main;
        [SerializeField] private Advert _advert;
        [SerializeField] private SoundSwitcher _soundSwitcher;
        [SerializeField] private EventButton _play;
        [SerializeField] private EventButton _additionalMoney;
        [SerializeField] private CarPricesConfiguration _configuration;
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TutorialAnimation _tutorial;
#if UNITY_EDITOR
        [SerializeField] private ProgressEditor _progressEditor;
#endif

        private readonly TimeSpan _loadDelay = TimeSpan.FromSeconds(0.01f);
        private List<CarSetup> _cars;
        private Wallet _wallet;

        private async void Awake()
        {
            GameAnalytics.Initialize();
            QualitySettings.SetQualityLevel(YandexGame.EnvironmentData.isDesktop ? 1 : 0);
            Debug.Log($"{AB}: {YandexGame.GetFlag(AB)}");
            _wallet = new Wallet(YandexGame.savesData.moneyCount, _moneyText);
            _main.Initialize();
            _soundSwitcher.Initialize();
            _advert.Initialize(_wallet);
            PlatformRotator rotator = _gameSetup.Initialize(
                _preparer.ReturnCarPrice,
                _wallet,
                _soundSwitcher.AddSound);
            _preparer.Initialize(_gameSetup.Enable, rotator, _wallet, _main);
            _cardHolder.Initialize(_configuration, OnCardClick, _wallet.HaveEnoughMoney);
            LevelManager.Register(Load, _gameSetup.BackCallback, _configuration.Cars.Count);

#if UNITY_EDITOR
            _progressEditor.Initialize(_wallet, _cardHolder);
#endif

            _advert.RegisterCallback(AdvertName.TwoHundredMoney, () => { });
            _additionalMoney.Initialize(() => YandexGame.RewVideoShow((int)AdvertName.TwoHundredMoney));
            _play.Initialize(
                () =>
                {
                    _main.SetWindow((int)GameWindows.Game);
                    YandexGame.savesData.didCompleteTutorial = true;
                });

            if (YandexGame.savesData.didCompleteTutorial == false)
            {
                Load(0, GameWindows.Tutorial);
                _tutorial.Initialize();
                LevelManager.CurrentLevel = 0;
                YandexGame.GameReadyAPI();
                return;
            }

            await UniTask.Delay(_loadDelay);

            _cardHolder.Draw();
            _main.SetWindow((int)GameWindows.Shop);
            YandexGame.GameReadyAPI();
        }

        private void OnCardClick(int carId)
        {
            _preparer.Next(
                _configuration.Cars[carId].Value,
                _cardHolder.GetCar(carId),
                _cardHolder.ReCreateCar,
                carId);
        }

        private void Load(int id, GameWindows window)
        {
            _main.SetWindow((int)window);
            _preparer.SetCarPosition(_cardHolder.GetCar(id));
            _gameSetup.Enable(
                _configuration.Cars[id].Value,
                _cardHolder.GetCar(id),
                _cardHolder.ReCreateCar,
                id,
                _wallet.AddMoney);
        }
    }
}
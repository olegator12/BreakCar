using System;
using Destruction;
using Economic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Screen = UI.Screen;

namespace Game
{
    public class Preparer : MonoBehaviour
    {
        private const float PainterMax = 1f;

        [SerializeField] private Painter _painter;
        [SerializeField] private Transform _platform;
        [SerializeField] private GameToggleButton _back;
        [SerializeField] private Slider _togglePainter;

        private Action<int, CarSetup, Action<int>, int, Action<int>> _gameStartHandler;
        private PlatformRotator _rotator;
        private Wallet _wallet;
        private Screen _main;

        private int _carPrice;
        private CarSetup _car;
        private Action<int> _disableCallback;
        private int _id;

        private void OnDisable()
        {
            _togglePainter.onValueChanged.RemoveListener(OnPainterStateSwitch);
        }

        public void Initialize(
            Action<int, CarSetup, Action<int>, int, Action<int>> gameStartHandler,
            PlatformRotator rotator,
            Wallet wallet,
            Screen main)
        {
            _togglePainter.value = YandexGame.savesData.painterTurnState;
            _gameStartHandler = gameStartHandler;
            _rotator = rotator;
            _wallet = wallet;
            _main = main;

            _painter.Initialize(PlayGame);
            _back.RegisterEvent(ReturnCarPrice);
            _back.RegisterEvent(Return);

            _togglePainter.onValueChanged.AddListener(OnPainterStateSwitch);
        }

        public void Next(int carPrice, CarSetup car, Action<int> disableCallback, int id)
        {
            _carPrice = carPrice;
            _car = car;
            _id = id;
            _disableCallback = disableCallback;

            _wallet.SpendMoney(_carPrice);

            SetCarPosition(car);

            if (_togglePainter.value < PainterMax)
            {
                PlayGame();
                _main.SetWindow((int)GameWindows.Game);
                return;
            }

            car.InitializePaint();
            _painter.Activate(car);
            _main.SetWindow((int)GameWindows.MiniGame);
        }

        public void SetCarPosition(CarSetup car)
        {
            Transform carTransform = car.transform;
            carTransform.SetParent(_platform);
            carTransform.localPosition = Vector3.zero;
            carTransform.localScale = Vector3.one;
            carTransform.localEulerAngles = Vector3.zero;

            _rotator.StartRotation();
        }

        public void ReturnCarPrice()
        {
            _wallet.AddMoney(_carPrice);
        }

        private void Return()
        {
            _disableCallback.Invoke(_id);
        }

        private void PlayGame()
        {
            _gameStartHandler.Invoke(_carPrice, _car, _disableCallback, _id, _wallet.AddMoney);
        }

        private void OnPainterStateSwitch(float value)
        {
            YandexGame.savesData.painterTurnState = value;
            YandexGame.SaveProgress();
        }
    }
}
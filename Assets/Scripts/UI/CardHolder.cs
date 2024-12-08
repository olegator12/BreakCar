using System;
using System.Collections.Generic;
using Configs;
using Destruction;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace UI
{
    public class CardHolder : MonoBehaviour
    {
        private const int MoneyPerRewarded = 200;
        private const int MinValue = 0;
        private const int DesktopColumnCount = 3;
        private const int MobileColumnCount = 2;

        private readonly Vector3 _startSpawnPosition = new (0f, 1000f, 0f);

        [SerializeField] private List<CarCard> _cards;
        [SerializeField] private int _cardCount = 6;
        [SerializeField] private Button _next;
        [SerializeField] private Button _back;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private LangYGAdditionalText _text;
        [SerializeField] private AdvertRegistration _registration;
        [SerializeField] private AudioSource _clickSound;

        private List<CarSetup> _cars;
        private List<int> _prices;
        private CarPricesConfiguration _configuration;
        private Vector3 _lastSpawnPosition;

        private Action<int> _onClick;
        private Func<int, bool> _priceHelper;
        private int _pointer;
        private int _lastId;

        private void OnDestroy()
        {
            _next.onClick.RemoveListener(Next);
            _back.onClick.RemoveListener(Back);
        }

        public void Initialize(
            CarPricesConfiguration configuration,
            Action<int> onClick,
            Func<int, bool> priceHelper)
        {
            _configuration = configuration;
            _prices = new List<int>();
            _onClick = onClick;
            _priceHelper = priceHelper;
            _cars = new List<CarSetup>();
            _lastSpawnPosition = Vector3.zero;
            _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

            if (YandexGame.EnvironmentData.isDesktop == true)
            {
                _grid.constraintCount = DesktopColumnCount;
            }
            else
            {
                Rect rect = _grid.GetComponent<RectTransform>().rect;

                _grid.constraintCount = MobileColumnCount;
                float heightModifier = _grid.cellSize.y / _grid.cellSize.x;
                float widthModifier = _grid.cellSize.x / _grid.cellSize.y;

                float width = (rect.width - _grid.spacing.x) / MobileColumnCount;
                float maxHeight = (rect.height - _grid.spacing.y * MobileColumnCount) / DesktopColumnCount;

                float cellHeight = width * heightModifier;

                if (maxHeight < cellHeight)
                {
                    cellHeight = maxHeight;
                    width = maxHeight * widthModifier;
                }

                _grid.cellSize = new Vector2(width, cellHeight);
            }

            foreach (SerializedPair<CarSetup, int> pair in _configuration.Cars)
            {
                _lastSpawnPosition += _startSpawnPosition;
                _cars.Add(Instantiate(pair.Key, _lastSpawnPosition, Quaternion.identity).Initialize());
                _prices.Add(pair.Value);
            }

            foreach (CarCard card in _cards)
                card.Initialize(_clickSound);

            _next.onClick.AddListener(Next);
            _back.onClick.AddListener(Back);
        }

        public CarSetup GetCar(int id)
        {
            _lastId = id;
            return _cars[id];
        }

        public void ReCreateCar(int id)
        {
            CarSetup toDelete = _cars[id];
            _lastSpawnPosition += _startSpawnPosition;
            _cars[id] = Instantiate(_configuration.Cars[id].Key, _lastSpawnPosition, Quaternion.identity).Initialize();
            Destroy(toDelete.gameObject);
            Draw();
        }

        public void Draw()
        {
            UpdateButtons();
            Sequence sequence = DOTween.Sequence();
            int counter = 0;
            bool isSetAdvert = false;
            int garageId = _pointer / _cardCount + 1;
            _text.additionalText = $" {garageId.ToString()}";
            List<int> completedLevels = YandexGame.savesData.completedLevels;

            foreach (CarCard card in _cards)
                card.TurnOff();

            for (int i = _pointer; i < _pointer + _cardCount; i++)
            {
                CarCard card = _cards[counter];

                if (i >= _prices.Count)
                    continue;

                if (completedLevels.Contains(i))
                {
                    card.SetBought();
                }
                else if (isSetAdvert == false &&
                         _priceHelper.Invoke(_prices[i]) == false &&
                         _priceHelper.Invoke(_prices[i] - MoneyPerRewarded) == true)
                {
                    card.SetAdvert(_registration);
                    isSetAdvert = true;
                }
                else if (_priceHelper.Invoke(_prices[i]) == true)
                {
                    card.SetAvailable();
                }
                else
                {
                    card.SetUnavailable();
                }

                sequence.Append(card.Activate(_cars[i].GetTexture(), _prices[i], i, _onClick));
                counter++;
            }

            sequence.Play();
        }

        public void UpdatePage()
        {
            if ((_lastId + 1) / 6 != 1)
                return;

            Next();
        }

        private void Next()
        {
            _pointer += _cardCount;
            Draw();
        }

        private void Back()
        {
            _pointer -= _cardCount;
            Draw();
        }

        private void UpdateButtons()
        {
            _next.interactable = _pointer + _cardCount < _cars.Count;
            _back.interactable = _pointer - _cardCount >= MinValue;
        }
    }
}
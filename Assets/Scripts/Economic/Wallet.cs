using System;
using DG.Tweening;
using TMPro;
using YG;

namespace Economic
{
    public class Wallet
    {
        private const int MinValue = 0;
        private const float AnimationSpeed = 0.6f;
        private const string ValueException = "The value must be greater than 0";
        private const string Dollar = "$";

        private readonly TMP_Text _text;

        private int _currentCount;

        public Wallet(int startValue, TMP_Text text)
        {
            _currentCount = startValue;
            _text = text;
            int before = YandexGame.savesData.beforeEndMoneyCount;

            if (before > startValue)
            {
                YandexGame.savesData.moneyCount = before;
                _currentCount = before;
            }

            _text.SetText($"{_currentCount.ToString()}{Dollar}");
        }

#if UNITY_EDITOR
        public int MoneyCount => _currentCount;
#endif

        public void AddMoney(int value)
        {
            if (value < MinValue)
                throw new ArgumentOutOfRangeException(ValueException);

            _currentCount += value;
            SetText();
        }

        public void SpendMoney(int value)
        {
            if (value < MinValue)
                throw new ArgumentOutOfRangeException(ValueException);

            if (HaveEnoughMoney(value) == false)
                return;

            _currentCount -= value;
            SetText();
        }

        public bool HaveEnoughMoney(int value)
        {
            return value <= _currentCount;
        }

        public void ApplyModification()
        {
            YandexGame.savesData.moneyCount = _currentCount;
            YandexGame.SaveProgress();
        }

        private void SetText()
        {
            _text.DOText($"{_currentCount.ToString()}{Dollar}", AnimationSpeed, scrambleMode: ScrambleMode.Numerals);
        }
    }
}
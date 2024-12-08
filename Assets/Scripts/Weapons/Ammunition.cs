using System;

namespace Weapons
{
    public class Ammunition
    {
        private const int MinValue = 0;

        private int _startUsageCount;
        private int _currentUsageCount;

        public event Action Spent;

        public event Action<int, int> Changed;

        public void Load(int usageCount)
        {
            _currentUsageCount = usageCount;
            _startUsageCount = usageCount;
            Changed?.Invoke(_currentUsageCount, _startUsageCount);
        }

        public bool CanSpend()
        {
            return _currentUsageCount > MinValue;
        }

        public void Spend()
        {
            _currentUsageCount--;
            Changed?.Invoke(_currentUsageCount, _startUsageCount);

            if (_currentUsageCount > MinValue)
                return;

            Spent?.Invoke();
        }

        public void SpendAll()
        {
            _currentUsageCount = MinValue;
            Changed?.Invoke(_currentUsageCount, _startUsageCount);
        }
    }
}
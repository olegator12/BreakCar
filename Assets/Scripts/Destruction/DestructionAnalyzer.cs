using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using YG;

namespace Destruction
{
    public class DestructionAnalyzer
    {
        private const string AB = "level_cap";

        private readonly DestructionBar _bar;
        private readonly Action _onWin;
        private readonly IReadOnlyList<VitalityPart> _parts;
        private readonly TimeSpan _winDelay = TimeSpan.FromSeconds(100f);
        private readonly CancellationTokenSource _cancellation;
        private readonly int _partsCount;

        private int _currentCount;
        private bool _didDestroy;

        public DestructionAnalyzer(
            DestructionBar bar,
            IReadOnlyList<VitalityPart> parts,
            Action onWin,
            float minDestroyedPercentage)
        {
            _bar = bar;
            _onWin = onWin;
            _cancellation = new CancellationTokenSource();
            string cap = YandexGame.GetFlag(AB);

            switch (cap)
            {
                case "0":
                    break;

                case "1":
                    minDestroyedPercentage = 0.85f;
                    break;

                case "2":
                    minDestroyedPercentage = 0.80f;
                    break;
            }

            _partsCount = Mathf.RoundToInt(parts.Count * minDestroyedPercentage);
            _currentCount = 0;
            _parts = parts;
            _bar.Change(_currentCount, _partsCount);

            foreach (VitalityPart part in _parts)
                part.Broke += OnBroke;

            WinAuto().Forget();
        }

        public void Dispose()
        {
            foreach (VitalityPart part in _parts)
                part.Broke -= OnBroke;

            if (_cancellation.IsCancellationRequested == false)
                _cancellation.Cancel();

            _cancellation.Dispose();
        }

        private void OnBroke()
        {
            if (_didDestroy == true)
                return;

            _currentCount++;
            _bar.Change(_currentCount, _partsCount);

            if (_currentCount < _partsCount)
                return;

            _didDestroy = true;
            _onWin.Invoke();
        }

        private async UniTaskVoid WinAuto()
        {
            await UniTask.Delay(_winDelay, cancellationToken: _cancellation.Token);
            _didDestroy = true;
            _bar.Change(_partsCount, _partsCount);
            _onWin.Invoke();
        }
    }
}
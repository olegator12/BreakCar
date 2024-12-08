using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Economic
{
    public class CoinIcon : SpawnableObject
    {
        private const float PrepareDuration = 0.5f;
        private const float CompleteDuration = 0.35f;

        private RectTransform _transform;

        public CoinIcon Initialize()
        {
            _transform = (RectTransform)transform;
            return this;
        }

        public async UniTask Move(Vector2 anchor, Vector3 target)
        {
            await _transform.DOAnchorPos(anchor, PrepareDuration).ToUniTask(cancellationToken: destroyCancellationToken);
            await _transform.DOMove(target, CompleteDuration).ToUniTask(cancellationToken: destroyCancellationToken);
            Push();
        }
    }
}
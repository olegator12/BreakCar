using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Destruction
{
    public interface IHighlightable
    {
        private const float HighlightInterval = 0.2f;
        private const int PingCount = 3;

        public MeshRenderer Renderer { get; }

        public async UniTaskVoid Highlight(Material highlighted)
        {
            Material start = Renderer.material;
            TimeSpan delay = TimeSpan.FromSeconds(HighlightInterval);
            await ChangeMaterial(highlighted, start, PingCount, delay);
        }

        private async UniTask ChangeMaterial(
            Material highlighted,
            Material start,
            int count,
            TimeSpan delay)
        {
            for (int i = 0; i < count; i++)
            {
                Renderer.material = highlighted;
                await UniTask.Delay(delay);
                Renderer.material = start;
                await UniTask.Delay(delay);
            }

            await UniTask.CompletedTask;
        }
    }
}
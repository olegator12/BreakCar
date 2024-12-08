using System.Collections.Generic;
using UnityEngine;

namespace Economic
{
    public class CoinFactory
    {
        private const int PreCreatedCount = 80;

        private readonly ObjectPool<Coin> _pool;

        public CoinFactory(ObjectPool<Coin> pool)
        {
            _pool = pool;

            if (_pool.Count > 0)
                return;

            List<Coin> preCreated = new List<Coin>();

            for (int i = 0; i < PreCreatedCount; i++)
                preCreated.Add(_pool.Pull(Vector3.zero, OnCreate));

            foreach (Coin coin in preCreated)
                coin.Push();
        }

        public Coin Create(Vector3 position, Vector3 normal)
        {
            return _pool.Pull(position, OnCreate).Initialize(normal);
        }

        private void OnCreate(Coin coin)
        {
            coin.Initialize();
        }
    }
}
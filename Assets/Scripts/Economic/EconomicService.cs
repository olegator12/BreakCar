using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Economic
{
    public class EconomicService
    {
        private const int Divider = 3;
        private const int MinValue = -5;
        private const int MaxValue = 10;

        private readonly CoinFactory _factory;
        private readonly List<Coin> _coins;
        private readonly int _carPrice;

        public EconomicService(ObjectPool<Coin> pool, int carPrice)
        {
            _coins = new List<Coin>();
            _factory = new CoinFactory(pool);
            _carPrice = carPrice;
        }

        public int CalculateReward()
        {
            int newCarPrice = _carPrice / Divider + Random.Range(MinValue, MaxValue);

            if (newCarPrice < 0)
                newCarPrice = 0;

            return newCarPrice;
        }

        public IEnumerable<UniTask> MoveCoin(Vector3 position)
        {
            return _coins.Select(coin => coin.Move(position));
        }

        public void SpawnCoin(Vector3 position, Vector3 normal)
        {
            _coins.Add(_factory.Create(position, normal));
        }

        public void SpawnTwoCoin(Vector3 position, Vector3 normal)
        {
            SpawnCoin(position, normal);
            SpawnCoin(position, normal);
        }
    }
}
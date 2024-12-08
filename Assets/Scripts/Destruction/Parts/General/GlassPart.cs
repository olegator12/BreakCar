using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Destruction
{
    public class GlassPart : MonoBehaviour
    {
        private const int MinForce = 3;
        private const int MaxForce = 5;
        private const float DisappearInterval = 2.2f;

        private readonly TimeSpan _delay = TimeSpan.FromSeconds(DisappearInterval);

        [SerializeField] private GameObject _gameObject;

        private Rigidbody _rigidbody;
        private bool _didBurst;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Ground _) == false)
                return;

            _gameObject.SetActive(false);
        }

        public void Initialize()
        {
            _gameObject = gameObject;
        }

        public async void Burst(Vector3 normal)
        {
            if (_didBurst == true)
                return;

            _didBurst = true;
            _rigidbody = _gameObject.AddComponent<Rigidbody>();
            _rigidbody.AddForce(normal * Random.Range(MinForce, MaxForce), ForceMode.Impulse);

            await UniTask.Delay(_delay, cancellationToken: destroyCancellationToken);

            _gameObject.SetActive(false);
        }
    }
}
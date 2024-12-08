using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Economic
{
    [RequireComponent(typeof(Rigidbody))]
    public class Coin : SpawnableObject
    {
        private const float AnimationDuration = 1f;
        private const float MinOffset = 0.1f;
        private const float MaxOffset = 0.35f;

        [SerializeField] private Collider _collider;
        [SerializeField] private AudioSource _sound;

        private Rigidbody _rigidbody;
        private Transform _transform;
        private bool _didInitialize;

        public void Initialize()
        {
            if (_didInitialize == true)
                return;

            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _didInitialize = true;
        }

        public Coin Initialize(Vector3 direction)
        {
            Initialize();
            _sound.Play();
            _rigidbody.AddForce(direction);
            return this;
        }

        public AudioSource GetSource()
        {
            return _sound;
        }

        public async UniTask Move(Vector3 position)
        {
            _rigidbody.useGravity = false;
            _transform.up = Vector3.up;
            _collider.enabled = false;

            await _transform
                .DOMove(position, AnimationDuration + Random.Range(MinOffset, MaxOffset))
                .ToUniTask(cancellationToken: destroyCancellationToken);

            Push();
        }

        public override void OnPush()
        {
            _collider.enabled = true;
            _rigidbody.useGravity = true;
        }
    }
}
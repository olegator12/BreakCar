using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Destruction;
using DG.Tweening;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public abstract class Projectile : SpawnableObject
    {
        private const float AnimationDuration = 0.1f;
        private const float PushDeep = 0.3f;
        private const float Divider = 2f;

        [SerializeField] private bool _isNotFreezingOnClear;
        [SerializeField] private bool _isNotFreezingOnContact;
        [SerializeField] private bool _isForwardRotatable;

        private Transform _originParent;
        private Rigidbody _rigidbody;
        private Collider _collider;
        private bool _didInitialize;
        private CancellationToken _token;

        private IReadOnlyList<DamageType> _damageTypes;
        private Func<float> _damageValue;
        private float _speed;
        private float _deformationModifier;
        private Action _onContact;

        private Vector3[] _trajectory;
        private Vector3 _normal;
        private bool _didContact;
        private bool _canStick = true;
        private bool _isTrajectoryMoving;

        [field: SerializeField] public ParticleSystem Effect { get; private set; }

        private void OnDestroy()
        {
            _isTrajectoryMoving = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Contact(collision);
            OnContactComplete();
        }

        public Projectile Initialize(WeaponSettings settings, Func<float> damageValue)
        {
            if (_didInitialize == true)
                return this;

            _originParent = Transform.parent;
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _damageTypes = settings.DamageTypes;
            _damageValue = damageValue;
            _speed = settings.Speed;
            _deformationModifier = settings.DeformationModifier;
            _token = destroyCancellationToken;
            return this;
        }

        public void Move(
            Vector3 direction,
            Vector3 normal,
            Action onContact = null,
            bool canStick = true)
        {
            Clear();
            _onContact = onContact;
            _normal = normal;
            _canStick = canStick;
            Transform.forward = direction;
            _rigidbody.AddForce(direction * _speed, ForceMode.Impulse);
        }

        public void Move(Vector3[] positions, Vector3 normal, bool canStick, Action onContact = null)
        {
            Clear();
            _normal = normal;
            _canStick = canStick;
            _isTrajectoryMoving = true;
            _trajectory = positions;
            _onContact = onContact;

            if (_isForwardRotatable == true)
                Transform.forward = positions[^1] - positions[0];

            if (_isNotFreezingOnClear == true)
                _rigidbody.constraints = RigidbodyConstraints.None;

            Move().Forget();
        }

        public void Freeze()
        {
            _rigidbody.useGravity = false;
        }

        public void SetParent(Transform parent = null, Vector3 euler = default)
        {
            if (parent == null)
                parent = _originParent;

            Transform.SetParent(parent);
            Transform.localEulerAngles = euler;
        }

        public abstract void OnStickPrepare(IBreakable breakable, Vector3 position, Collision collision);

        public abstract void OnContactComplete();

        public abstract void PlayEffect(bool isCarPart);

        public virtual void OnPushProjectile()
        {
        }

        public override void OnPush()
        {
            if (_rigidbody == null)
                return;

            Clear();
            OnPushProjectile();
        }

        private async void Contact(Collision collision)
        {
            if (_didContact == true)
            {
                if (_isNotFreezingOnContact == false)
                    return;

                if (collision.collider.TryGetComponent(out PlatformGround platform) == false)
                    return;

                platform.TryStick(Transform);
                return;
            }

            _didContact = true;
            _isTrajectoryMoving = false;
            _rigidbody.useGravity = true;

            if (_isNotFreezingOnClear == true)
            {
                _rigidbody.velocity = Transform.forward;
                _rigidbody.AddTorque(Vector3.one * Divider);
            }

            if (_isNotFreezingOnContact == true)
                _rigidbody.constraints = RigidbodyConstraints.None;

            if (collision.collider.TryGetComponent(out ISticky sticky) == false)
            {
                PlayEffect(false);
                return;
            }

            Vector3 position = Transform.position;

            if (_canStick == true && sticky.TryStick(Transform) == true)
            {
                OnStick();
                float maxDeep = PushDeep;

                if (_collider is BoxCollider box)
                {
                    Vector3 size = box.size;
                    maxDeep = Mathf.Max(size.x, size.y, size.z) / Divider;
                }

                await Transform.DOMove(position + -_normal * maxDeep, AnimationDuration)
                    .ToUniTask(cancellationToken: destroyCancellationToken);
            }

            if (sticky is IBreakable breakable == false)
            {
                PlayEffect(false);
                return;
            }

            PlayEffect(true);
            _onContact?.Invoke();
            OnStickPrepare(breakable, position, collision);

            if (breakable is Body { IsForwardDeformation: false } body)
            {
                body.PlaySound();
                return;
            }

            breakable.Break(
                Transform.position,
                _normal,
                -_normal,
                _deformationModifier,
                _damageTypes,
                _damageValue.Invoke());
        }

        private void Clear()
        {
            _didContact = false;
            _isTrajectoryMoving = false;
            _canStick = true;
            _rigidbody.useGravity = false;
            _rigidbody.velocity = Vector3.zero;
            _collider.enabled = true;

            if (_isNotFreezingOnClear == true)
                return;

            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void OnStick()
        {
            _rigidbody.useGravity = false;
            _collider.enabled = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        private async UniTaskVoid Move()
        {
            int count = 0;

            while (_isTrajectoryMoving == true)
            {
                Transform.position = Vector3.MoveTowards(
                    Transform.position,
                    _trajectory[count],
                    _speed * Time.deltaTime);

                if (Transform.position == _trajectory[count])
                {
                    count++;

                    if (count >= _trajectory.Length)
                    {
                        _isTrajectoryMoving = false;
                        _rigidbody.useGravity = true;
                    }
                }

                await UniTask.NextFrame(PlayerLoopTiming.Update, _token);
            }
        }
    }
}
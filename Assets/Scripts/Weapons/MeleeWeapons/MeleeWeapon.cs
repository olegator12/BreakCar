using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI;
using UnityEngine;

namespace Weapons
{
    public abstract class MeleeWeapon : Weapon, IMeleeWeapon
    {
        private const float OffSetMultiplier = 0.4f;
        private const float AngleRotationOffSet = 30f;

        [SerializeField] private Vector3 _targetRotation = new (0f, -90f, -60f);
        [SerializeField] private Transform _rotationPoint;
        [SerializeField] private ParticleSystem _effect;

        private float _animationDuration = 0.25f;
        private float _damageModifier = 1f;

        public abstract WeaponType Name { get; }

        public int Uses => _settings.Uses;

        public float DelayModifier => _settings.DelayModifier;

        public float DeformationModifier => _settings.DeformationModifier;

        public IReadOnlyList<DamageType> DamageTypes => _settings.DamageTypes;

        public float DamageValue => _settings.DamageValue * _damageModifier;

        private GameObject _gameObject;
        private Transform _transform;
        private CancellationToken _token;
        private Vector3 _startRotation;
        private List<DamageType> _damages;
        private WeaponSettings _settings;
        private ICameraShaker _cameraShaker;

        public override void Initialize(WeaponSettings settings, ICameraShaker cameraShaker)
        {
            _cameraShaker = cameraShaker;
            _gameObject = gameObject;
            _transform = transform;
            _token = destroyCancellationToken;
            _startRotation = _rotationPoint.localEulerAngles;

            _settings = settings;
            _animationDuration = _settings.Speed;
        }

        public async void Attack(Vector3 targetPosition, Vector3 normal, Func<bool> onContact)
        {
            onContact.Invoke();
            Vector3 newPosition = targetPosition + normal * OffSetMultiplier;

            _rotationPoint.localEulerAngles = _startRotation;
            _transform.position = newPosition;
            _transform.forward = Vector3.Cross(Vector3.up, normal);
            float angle = Vector3.Angle(normal, Vector3.up);

            if (Mathf.Abs(angle) > AngleRotationOffSet)
            {
                Vector3 eulerAngles = _transform.eulerAngles;
                eulerAngles.z = angle;
                _transform.eulerAngles = eulerAngles;
            }

            _gameObject.SetActive(true);
            _effect.Play();
            _cameraShaker.Shake(_settings.CameraShakeModifier);

            await _rotationPoint
                .DOLocalRotate(_targetRotation, _animationDuration)
                .ToUniTask(cancellationToken: _token);

            _gameObject.SetActive(false);
        }

        public void Prepare()
        {
            _cameraShaker.SetFOV(_settings.FOV);
        }

        public void UpdateDamage(float modifier)
        {
            _damageModifier = modifier;
        }
    }
}
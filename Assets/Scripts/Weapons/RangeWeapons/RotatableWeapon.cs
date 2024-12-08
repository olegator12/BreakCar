using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using YG;

namespace Weapons
{
    public abstract class RotatableWeapon : RangeWeapon, IRotatable
    {
        private const float Degrees = 360f;

        [SerializeField] private Transform _shootPoint;
        [SerializeField] private Transform _rotationPoint;
        [SerializeField] private Vector2 _angleYLimit = new (-30f, 30f);
        [SerializeField] private Vector2 _angleXLimit = new (-30f, 30f);
        [SerializeField] private ParticleSystem _shootEffect;
        [SerializeField] private AudioClip _shootSound;

        private bool _isRotating;
        private bool _isShooting;
        private bool _isRotatingLaunched;
        private bool _isMobile;

        private Vector2 _startInput;
        private Vector2 _lastInput;
        private Vector3 _lastPCInput;
        private Vector3 _startEulerAngles;
        private Vector3 _targetRotation;
        private float _rotationForce = 10f;

        private GameObject _gameObject;
        private Func<bool> _onContact;
        private TimeSpan _delay;
        private CancellationToken _token;
        private ObjectPool<SpawnableSound> _soundPool;

        public virtual bool CanStick => true;

        public override void OnInitialize()
        {
            _gameObject = gameObject;
            _token = destroyCancellationToken;
            _rotationForce = Settings.RotationForce;
            _isMobile = YandexGame.EnvironmentData.isDesktop == false;

            if (_isMobile == true)
                return;

            transform.eulerAngles = Vector3.zero;
        }

        public override void Attack(Vector3 _, Vector3 __, Func<bool> onContact)
        {
            _onContact = onContact;

            if (_shootEffect == true)
                _shootEffect.Play();
        }

        public override Vector3 GetSpawnPosition(Vector3 _, Vector3 __)
        {
            return _shootPoint.position;
        }

        public void UpdateDelay(TimeSpan touchDelay)
        {
            _delay = TimeSpan.FromSeconds(touchDelay.TotalSeconds * DelayModifier);
        }

        public override void Prepare()
        {
            SetFOV();
            _gameObject.SetActive(true);
        }

        public void Read(Vector2 input)
        {
            if (_isMobile == true)
            {
                if (_isRotating == false)
                {
                    _startInput = input;
                    _startEulerAngles = _rotationPoint.localEulerAngles;
                    _isRotating = true;
                    return;
                }

                if (_isShooting == true && _isRotating == true)
                {
                    _lastInput = input;
                    CalculateAngles((_lastInput - _startInput) * _rotationForce);

                    if (_isRotatingLaunched == false)
                    {
                        _isRotatingLaunched = true;
                        Rotate().Forget();
                    }

                    return;
                }
            }
            else
            {
                bool isNeedUpdate = _lastInput != input;

                _lastInput = input;

                if (isNeedUpdate == true)
                    RecalculateRotation();
            }

            if (_isShooting == true)
                return;

            _isShooting = true;
            Shoot().Forget();
        }

        public void Read(Vector3 input)
        {
            if (_isMobile == true)
                return;

            _lastPCInput = input;
        }

        public void InitializeSound(ObjectPool<SpawnableSound> soundPool)
        {
            _soundPool = soundPool;
        }

        public void Cancel()
        {
            _isRotating = false;
            _isRotatingLaunched = false;

            if (Settings.IsAutomaticGun == true)
            {
                _isShooting = false;

                if (_shootEffect == true)
                    _shootEffect.Stop();
            }


            _startInput = Vector2.zero;
            _lastInput = Vector2.zero;
        }

        public void Revert()
        {
            _isShooting = false;
            Cancel();

            if (_shootEffect == true)
                _shootEffect.Stop();

            _gameObject.SetActive(false);
        }

        private async UniTaskVoid Rotate()
        {
            while (_isRotatingLaunched == true && _token.IsCancellationRequested == false)
            {
                await _rotationPoint.DOLocalRotate(_targetRotation, 0.1f).ToUniTask(cancellationToken: _token);
            }
        }

        private async UniTaskVoid Shoot()
        {
            while (_isShooting == true && _token.IsCancellationRequested == false)
            {
                if (_onContact.Invoke() == false)
                    break;

                Vector3 direction = _shootPoint.forward;
                CreateProjectile(_shootPoint.position, -direction).Move(direction, -direction, canStick: CanStick);
                _soundPool.Pull(Vector3.zero, OnCreate).Play(_shootSound);
                Shake();
                await UniTask.Delay(_delay, cancellationToken: destroyCancellationToken);
            }
        }

        private void CalculateAngles(Vector3 result)
        {
            result = new Vector3(-result.y, result.x);

            float x = CalculateBounds(_startEulerAngles.x, _angleXLimit) + result.x;
            float y = CalculateBounds(_startEulerAngles.y, _angleYLimit) + result.y;

            x = Mathf.Clamp(x, _angleXLimit.x, _angleXLimit.y);
            y = Mathf.Clamp(y, _angleYLimit.x, _angleYLimit.y);

            _targetRotation = new Vector3(x, y);
        }

        private float CalculateBounds(float value, Vector2 limit)
        {
            value %= Degrees;

            if (value > limit.y && value - Degrees >= limit.x)
                return value - Degrees;

            if (value < limit.x && value + Degrees <= limit.y)
                return value + Degrees;

            return value;
        }


        private void RecalculateRotation()
        {
            Vector3 euler = Quaternion.LookRotation(_lastPCInput - _shootPoint.position).eulerAngles;
            float x = CalculateBounds(euler.x, _angleXLimit);
            float y = CalculateBounds(euler.y, _angleYLimit);
            _rotationPoint.rotation = Quaternion.Euler(new Vector3(x, y));
        }

        private void OnCreate(SpawnableSound sound)
        {
            sound.Initialize();
        }
    }
}
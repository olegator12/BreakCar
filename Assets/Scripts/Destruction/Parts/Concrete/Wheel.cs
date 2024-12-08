using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Destruction
{
    public class Wheel : VitalityPart, IHighlightable
    {
        private const float TearStrength = 4.2f;

        [SerializeField] private GameObject _tire;
        [SerializeField] private ParticleSystem _smokePiercing;
        [SerializeField] private ParticleSystem _smokeBurst;
        [SerializeField] private float _downOffset = -0.08f;
        [SerializeField] private float _tireBurstScale = 1.1f;
        [SerializeField] private float _animationSpeed = 0.15f;
        [SerializeField] private AudioClip _piercingSound;
        [SerializeField] private AudioClip _burstSound;
        [SerializeField] private AudioClip _hit;

        [field: SerializeField] public WheelSize Size { get; private set; }

        [field: SerializeField] public MeshRenderer Renderer { get; private set; }

        private GameObject _gameObject;
        private Transform _tireTransform;
        private Collider _collider;
        private bool _isPierced;
        private bool _didBurst;
        private bool _didTeared;

        public override CarPart Name => CarPart.Wheel;

        public override void OnInitialize()
        {
            _gameObject = gameObject;
            _collider = GetComponent<Collider>();

            if (_tire == true)
                _tireTransform = _tire.transform;
        }

        public override void Accept(IPartVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void OnBroke()
        {
        }

        public override void OnFirstTimeHitTook()
        {
        }

        public override void Break(
            Vector3 contactPosition,
            Vector3 normal,
            Vector3 relativeVelocity,
            float deformationModifier,
            IReadOnlyList<DamageType> damageTypes,
            float damageValue)
        {
            SetLastContact(contactPosition, normal);

            foreach (DamageType damage in damageTypes)
                CompareDamage(damage);
        }

        private void CompareDamage(DamageType damage)
        {
            switch (damage)
            {
                case DamageType.Piercing:
                    Pierce();
                    break;

                case DamageType.StrongPiercing:
                    Burst();
                    break;

                case DamageType.StrongCutting:
                case DamageType.StrongBlunt:
                    TearOff();
                    break;

                default:
                    PlaySound(_hit);
                    break;
            }
        }

        private void Pierce()
        {
            if (_tire == null)
            {
                PlaySound(_hit);
                return;
            }

            if (_isPierced == true || _didBurst == true)
            {
                PlaySound(_hit);
                return;
            }

            _isPierced = true;
            _smokePiercing.Play();
            TakeDamage(new[] { DamageType.Piercing }, int.MaxValue);
            Transform.DOLocalMoveY(_downOffset, _animationSpeed);
            PlaySound(_piercingSound);
        }

        private async void Burst()
        {
            if (_tire == null)
            {
                PlaySound(_hit);
                return;
            }

            if (_didBurst == true)
            {
                PlaySound(_hit);
                return;
            }

            _didBurst = true;

            await _tireTransform
                .DOScale(_tireTransform.localScale * _tireBurstScale, _animationSpeed)
                .ToUniTask(cancellationToken: destroyCancellationToken);

            TakeDamage(new[] { DamageType.StrongPiercing }, int.MaxValue);
            PlaySound(_burstSound);
            _smokeBurst.Play();
            _tire.SetActive(false);
        }

        private void TearOff()
        {
            if (_didTeared == true)
                return;

            _didTeared = true;
            PlaySound(_hit);
            TakeDamage(new[] { DamageType.StrongBlunt, DamageType.StrongCutting }, int.MaxValue);

            if (_collider is SphereCollider sphereCollider)
                sphereCollider.radius = sphereCollider.radius / 2;

            _gameObject.AddComponent<Rigidbody>().AddForce(LastNormal * TearStrength, ForceMode.Impulse);
        }
    }
}
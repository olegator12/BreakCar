using System;
using System.Collections.Generic;
using UnityEngine;

namespace Destruction
{
    public abstract class VitalityPart : VisitablePart, IBreakable
    {
        private ObjectPool<SpawnableSound> _pool;
        private BreakableHealth _health;
        private Vector3 _lastContact;

        private Action<Vector3, Vector3> _onBroke;
        private Action<Vector3, Vector3> _onFirstTimeHitTook;
        private bool _didBreak;
        private bool _didFirstTimeHit;

        public event Action Broke;

        public abstract CarPart Name { get; }

        public Transform Transform { get; private set; }

        public Vector3 LastNormal { get; private set; }

        public PartSettings Settings { get; private set; }

        private void OnDestroy()
        {
            if (_health == null)
                return;

            _health.Broke -= OnBreaking;
            _health.OneShot -= OnOneShot;
            _health.FirstTimeHitTook -= OnHitTaking;
        }

        public virtual void Initialize(
            PartSettings settings,
            ObjectPool<SpawnableSound> pool,
            Action<Vector3, Vector3> onBroke,
            Action<Vector3, Vector3> onFirstTimeHitTook)
        {
            _pool = pool;
            Transform = transform;
            Settings = settings;
            _health = new BreakableHealth(settings.HealthPointCount, settings.ImmuneTypes, settings.WeakTypes);
            _onBroke = onBroke;
            _onFirstTimeHitTook = onFirstTimeHitTook;

            _health.Broke += OnBreaking;
            _health.OneShot += OnOneShot;
            _health.FirstTimeHitTook += OnHitTaking;

            OnInitialize();
        }

        public virtual void OnInitialize()
        {
        }

        public void SetShader()
        {
            Material[] materials = GetComponent<MeshRenderer>().materials;

            foreach (Material material in materials)
                material.renderQueue = 2002;
        }

        public void SetLastContact(Vector3 contact, Vector3 normal)
        {
            _lastContact = contact;
            LastNormal = normal;
        }

        public void TakeDamage(IReadOnlyList<DamageType> damageTypes, float damageValue)
        {
            _health.TakeDamage(damageTypes, damageValue);
        }

        public void PlaySound(AudioClip clip)
        {
            _pool.Pull(Vector3.zero, OnCreate).Play(clip);
        }

        public virtual bool TryStick(Transform projectile)
        {
            projectile.SetParent(Transform);
            return true;
        }

        public abstract void OnBroke();

        public abstract void OnFirstTimeHitTook();

        public abstract void Break(
            Vector3 contactPosition,
            Vector3 normal,
            Vector3 relativeVelocity,
            float deformationModifier,
            IReadOnlyList<DamageType> damageTypes,
            float damageValue);

        private void OnBreaking()
        {
            if (_didBreak == true)
                return;

            _didBreak = true;
            OnBroke();
            _onBroke.Invoke(_lastContact, LastNormal);
            Broke?.Invoke();
        }

        private void OnHitTaking()
        {
            if (_didFirstTimeHit == true)
                return;

            _didFirstTimeHit = true;
            OnFirstTimeHitTook();
            _onFirstTimeHitTook.Invoke(_lastContact, LastNormal);
        }

        private void OnOneShot()
        {
            OnHitTaking();
            OnBreaking();
        }

        private void OnCreate(SpawnableSound sound)
        {
            sound.Initialize();
        }
    }
}
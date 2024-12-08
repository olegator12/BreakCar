using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Weapons
{
    public abstract class RangeWeapon : Weapon, IRangeWeapon
    {
        private const int PreCreatedCount = 50;

        [SerializeField] private Projectile _template;

        private ObjectPool<Projectile> _pool;
        private List<Projectile> _spawned;
        private ICameraShaker _cameraShaker;
        private float _damageModifier = 1f;

        public abstract WeaponType Name { get; }

        public int Uses => Settings.Uses;

        public float DelayModifier => Settings.DelayModifier;

        public float DeformationModifier => Settings.DeformationModifier;

        public WeaponSettings Settings { get; private set; }

        private Transform Transform { get; set; }

        private float Damage => Settings.DamageValue * _damageModifier;

        public override void Initialize(WeaponSettings settings, ICameraShaker cameraShaker)
        {
            _cameraShaker = cameraShaker;
            Transform = transform;
            _pool = new ObjectPool<Projectile>(_template);
            _spawned = new List<Projectile>();

            Settings = settings;
            OnInitialize();
        }

        public void RegisterHolder(Transform holder)
        {
            _pool.RegisterHolder(holder);

            Vector3 position = Transform.position;

            List<Projectile> preCreated = new List<Projectile>();

            for (int i = 0; i < PreCreatedCount; i++)
                preCreated.Add(_pool.Pull(position));

            foreach (Projectile projectile in preCreated)
                projectile.Push();

            OnRegisterHolder(holder);
        }

        public void Clear()
        {
            _spawned.ForEach(item => item.Push());
        }

        public Projectile CreateProjectile(Vector3 targetPosition, Vector3 normal)
        {
            Vector3 position = GetSpawnPosition(targetPosition, normal);
            Projectile projectile = _pool.Pull(position);
            _spawned.Add(projectile);
            OnProjectileCreate(projectile);
            return projectile.Initialize(Settings, () => Damage);
        }

        public void SetFOV()
        {
            _cameraShaker.SetFOV(Settings.FOV);
        }

        public void Shake()
        {
            _cameraShaker.Shake(Settings.CameraShakeModifier);
        }

        public void UpdateDamage(float modifier)
        {
            _damageModifier = modifier;
        }

        public virtual void OnInitialize()
        {
        }

        public virtual void OnRegisterHolder(Transform holder)
        {
        }

        public virtual void OnProjectileCreate(Projectile projectile)
        {
        }

        public abstract void Attack(Vector3 targetPosition, Vector3 normal, Func<bool> onContact);

        public abstract void Prepare();

        public abstract Vector3 GetSpawnPosition(Vector3 targetPosition, Vector3 normal);
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class Laser : RotatableWeapon
    {
        private const int PreCreatedCount = 30;

        [SerializeField] private VisualHole _holeTemplate;

        private ObjectPool<VisualHole> _pool;

        public override WeaponType Name => WeaponType.Laser;

        public override bool CanStick => false;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _pool = new ObjectPool<VisualHole>(_holeTemplate);
        }

        public override void OnRegisterHolder(Transform holder)
        {
            _pool.RegisterHolder(holder);

            Vector3 position = Vector3.zero;

            List<VisualHole> preCreated = new List<VisualHole>();

            for (int i = 0; i < PreCreatedCount; i++)
                preCreated.Add(_pool.Pull(position));

            foreach (VisualHole hole in preCreated)
                hole.Push();
        }

        public override void OnProjectileCreate(Projectile projectile)
        {
            if (projectile is VanishingProjectile vanishing == false)
                return;

            vanishing.Initialize(CreateHole);
        }

        private Transform CreateHole(Vector3 position, Vector3 normal)
        {
            Transform result = _pool.Pull(position).Transform;
            result.up = normal;
            return result;
        }
    }
}
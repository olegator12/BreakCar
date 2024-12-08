using System;
using UnityEngine;

namespace Weapons
{
    public interface IWeapon
    {
        public int Uses { get; }

        public WeaponType Name { get; }

        public float DelayModifier { get; }

        public float DeformationModifier { get; }

        public void Attack(Vector3 targetPosition, Vector3 normal, Func<bool> onContact);

        public void Prepare();

        public void UpdateDamage(float modifier = 1f);
    }
}
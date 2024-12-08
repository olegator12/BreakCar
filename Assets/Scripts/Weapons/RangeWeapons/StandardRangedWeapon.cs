using System;
using UnityEngine;

namespace Weapons
{
    public abstract class StandardRangedWeapon : RangeWeapon
    {
        public override void Attack(Vector3 targetPosition, Vector3 normal, Func<bool> onContact)
        {
            onContact.Invoke();
            CreateProjectile(targetPosition, normal).Move(-normal, normal, Shake);
        }

        public override void Prepare()
        {
            SetFOV();
        }
    }
}
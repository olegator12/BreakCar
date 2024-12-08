using Destruction;
using UnityEngine;

namespace Weapons
{
    public abstract class EmptyProjectile : Projectile
    {
        public override void OnStickPrepare(IBreakable _, Vector3 __, Collision ___)
        {
        }
    }
}
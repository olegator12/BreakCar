using UnityEngine;

namespace Weapons
{
    public class Knife : StandardRangedWeapon
    {
        private const float SpawnOffSet = 0.8f;

        public override WeaponType Name => WeaponType.Knife;

        public override Vector3 GetSpawnPosition(Vector3 targetPosition, Vector3 normal)
        {
            return targetPosition + normal * SpawnOffSet;
        }
    }
}
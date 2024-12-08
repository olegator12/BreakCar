using System.Collections.Generic;

namespace Weapons
{
    public interface IMeleeWeapon : IWeapon
    {
        public IReadOnlyList<DamageType> DamageTypes { get; }

        public float DamageValue { get; }
    }
}
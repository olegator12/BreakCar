using System.Collections.Generic;

namespace Weapons
{
    public interface IWeaponCollider
    {
        public IReadOnlyList<DamageType> Damages { get; }
    }
}
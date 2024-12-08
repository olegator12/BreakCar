using UnityEngine;

namespace Weapons
{
    public interface IRangeWeapon : IWeapon
    {
        public void RegisterHolder(Transform holder);

        public void Clear();
    }
}
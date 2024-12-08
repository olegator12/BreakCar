using UnityEngine;
using Weapons;

namespace Configs
{
    [CreateAssetMenu(fileName = "Weapon Configuration", menuName = "Weapon Configuration", order = 2)]
    public class WeaponConfiguration : UpdatableConfiguration<WeaponType, WeaponSettings>
    {
    }
}
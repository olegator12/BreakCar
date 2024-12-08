using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Configs
{
    [CreateAssetMenu(fileName = "Weapon Obtaining Config", menuName = "Weapon Obtaining Config", order = 6)]
    public class WeaponObtainingConfig : ScriptableObject
    {
        [field: SerializeField] public List<SerializedPair<WeaponType, int>> Queue { get; private set; }
    }
}
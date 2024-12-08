using UI;
using UnityEngine;

namespace Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        public abstract void Initialize(WeaponSettings settings, ICameraShaker cameraShaker);
    }
}
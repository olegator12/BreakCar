using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class WeaponCollider : MonoBehaviour, IWeaponCollider
    {
        private List<DamageType> _damages;
        private Collider _collider;

        public IReadOnlyList<DamageType> Damages => _damages;

        public void Initialize(List<DamageType> damages)
        {
            _damages = damages;
            _collider = GetComponent<Collider>();
        }

        public void SetColliderActive(bool value)
        {
            _collider.enabled = value;
        }
    }
}
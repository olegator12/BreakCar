using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(Collider))]
    public class HitBox : MonoBehaviour, IBreakable
    {
        [SerializeField] private List<DamageType> _accessTypes;

        private const string ParentException = "The object must have a parent";
        private const string ComponentException = "The parent element must have a IBreakable component";

        private IBreakable _breakable;

        public CarPart Name { get; private set; }

        private void Awake()
        {
            Transform parent = transform.parent;

            if (parent == false)
                throw new ArgumentNullException(ParentException);

            if (parent.TryGetComponent(out IBreakable breakable) == false)
                throw new ArgumentNullException(ComponentException);

            _breakable = breakable;
            Name = _breakable.Name;
        }

        public void Break(
            Vector3 contactPosition,
            Vector3 normal,
            Vector3 relativeVelocity,
            float deformationModifier,
            IReadOnlyList<DamageType> damageTypes,
            float damageValue)
        {
            if (_accessTypes.Count > 0)
            {
                bool result = _accessTypes.Any(damage => damageTypes.Contains(damage) == true);

                if (result == false)
                    return;
            }

            _breakable.Break(contactPosition, normal, relativeVelocity, deformationModifier, damageTypes, damageValue);
        }

        public bool TryStick(Transform projectile)
        {
            return _breakable.TryStick(projectile);
        }
    }
}
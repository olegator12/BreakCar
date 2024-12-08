using System.Collections.Generic;
using UnityEngine;

namespace Destruction
{
    public interface IBreakable : ISticky
    {
        public CarPart Name { get; }

        public void Break(
            Vector3 contactPosition,
            Vector3 normal,
            Vector3 relativeVelocity,
            float deformationModifier,
            IReadOnlyList<DamageType> damageTypes,
            float damageValue);
    }
}
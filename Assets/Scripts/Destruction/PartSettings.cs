using System;
using System.Collections.Generic;
using UnityEngine;

namespace Destruction
{
    [Serializable]
    public class PartSettings
    {
        [SerializeField] private List<DamageType> _immuneTypes = new ();
        [SerializeField] private List<DamageType> _weakTypes = new ();

        [field: SerializeField] public float DeformationRadius { get; private set; } = 0.3f;

        [field: SerializeField] public float DeformationForce { get; private set; } = 0.5f;

        [field: SerializeField] public float HealthPointCount { get; private set; } = 5f;

        [field: SerializeField] public int SkipCount { get; private set; } = 2;

        public IReadOnlyList<DamageType> ImmuneTypes => _immuneTypes;

        public IReadOnlyList<DamageType> WeakTypes => _weakTypes;
    }
}
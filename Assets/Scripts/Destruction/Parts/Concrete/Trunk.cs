using System.Collections.Generic;
using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(PaintPart))]
    public class Trunk : SimpleKnocker
    {
        [SerializeField] private Glass[] _glasses;

        public override CarPart Name => CarPart.Trunk;

        public override void OnBroke()
        {
            TearOff();

            IReadOnlyList<DamageType> damageTypes = new[] { DamageType.AntiGlass };

            foreach (Glass glass in _glasses)
                glass.TakeDamage(damageTypes, int.MaxValue);
        }
    }
}
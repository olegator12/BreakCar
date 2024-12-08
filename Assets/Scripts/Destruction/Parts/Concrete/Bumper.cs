using System.Collections.Generic;
using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(PaintPart))]
    public class Bumper : ObjectKnocker
    {
        [SerializeField] private Headlight[] _glasses;

        public override CarPart Name => CarPart.Bumper;

        public override void OnBroke()
        {
            TearOff();

            IReadOnlyList<DamageType> damageTypes = new[] { DamageType.AntiGlass };

            foreach (Headlight headlight in _glasses)
                headlight.TakeDamage(damageTypes, int.MaxValue);
        }
    }
}
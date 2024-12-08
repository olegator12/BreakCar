using System.Collections.Generic;
using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(PaintPart))]
    public class Door : ObjectKnocker
    {
        [SerializeField] private Window[] _glasses;

        public override CarPart Name => CarPart.Door;

        public override void OnBroke()
        {
            TearOff();

            IReadOnlyList<DamageType> damageTypes = new[] { DamageType.AntiGlass };

            foreach (Window window in _glasses)
                window.TakeDamage(damageTypes, int.MaxValue);
        }
    }
}
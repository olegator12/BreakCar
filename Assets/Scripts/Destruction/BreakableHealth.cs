using System;
using System.Collections.Generic;
using System.Linq;

namespace Destruction
{
    public class BreakableHealth
    {
        private const float MinValue = 0f;

        private readonly IReadOnlyList<DamageType> _immuneTypes;
        private readonly IReadOnlyList<DamageType> _weakTypes;

        private float _current;
        private bool _didHitFirstTime;
        private bool _didBreak;

        public BreakableHealth(
            float startValue,
            IReadOnlyList<DamageType> immuneTypes,
            IReadOnlyList<DamageType> weakTypes)
        {
            _current = startValue;
            _immuneTypes = immuneTypes;
            _weakTypes = weakTypes;
        }

        public event Action<IReadOnlyList<DamageType>> AdditionalCalled;

        public event Action Broke;

        public event Action FirstTimeHitTook;

        public event Action OneShot;

        public void TakeDamage(IReadOnlyList<DamageType> damageTypes, float damageValue)
        {
            if (_didBreak == true)
                return;

            if (CanReflect(damageTypes) == true)
                return;

            if (_didHitFirstTime == false)
            {
                _didHitFirstTime = true;

                if (_current <= MinValue || CanDestroy(damageTypes) == true)
                    OneShot?.Invoke();
                else
                    FirstTimeHitTook?.Invoke();
            }

            if (CanDestroy(damageTypes) == true)
                Break();

            _current -= damageValue;
            AdditionalCalled?.Invoke(damageTypes);

            if (_current > MinValue)
                return;

            Break();
        }

        private void Break()
        {
            _didBreak = true;
            _current = MinValue;
            Broke?.Invoke();
        }

        private bool CanDestroy(IReadOnlyList<DamageType> target)
        {
            return target.Any(damageType => _weakTypes.Contains(damageType) == true);
        }

        private bool CanReflect(IReadOnlyList<DamageType> target)
        {
            return (target.Count > MinValue) && target.All(damageType => _immuneTypes.Contains(damageType) == true);
        }
    }
}
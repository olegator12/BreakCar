using UnityEngine;

namespace Weapons
{
    public abstract class SpinWeapon : TrajectoryWeapon
    {
        private const int MinValue = -1;

        public override bool CanStick => true;

        public override Vector3 CalculateEvaluation(Vector3 position, float evaluation, int variant)
        {
            if (variant == MinValue)
            {
                position.x -= evaluation;
                position.z -= evaluation;
            }
            else
            {
                position.x += evaluation;
                position.z += evaluation;
            }

            return position;
        }
    }
}
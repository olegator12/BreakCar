using UnityEngine;

namespace Weapons
{
    public abstract class BallisticWeapon : TrajectoryWeapon
    {
        public override bool CanStick => false;

        public override Vector3 CalculateEvaluation(Vector3 position, float evaluation, int _)
        {
            position.y += evaluation;
            return position;
        }
    }
}
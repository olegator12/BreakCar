namespace Weapons
{
    public class Cannon : RotatableWeapon
    {
        public override WeaponType Name => WeaponType.Cannon;

        public override bool CanStick => false;
    }
}
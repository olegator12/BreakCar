namespace Weapons
{
    public class StandardProjectile : EmptyProjectile
    {
        public override void OnContactComplete()
        {
        }

        public override void PlayEffect(bool isCarPart)
        {
            if (isCarPart == false)
                return;

            Effect.Play();
        }
    }
}
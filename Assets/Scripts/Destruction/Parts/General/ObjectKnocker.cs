namespace Destruction
{
    public abstract class ObjectKnocker : DeformableMesh
    {
        private ObjectShaker _shaker;

        public override void OnInitialize()
        {
            _shaker = new ObjectShaker(Transform);
        }

        public void TearOff()
        {
            _shaker.TearOff(LastNormal);
        }

        public override void OnFirstTimeHitTook()
        {
        }

        public override void OnContact()
        {
            _shaker.Shake();
        }
    }
}
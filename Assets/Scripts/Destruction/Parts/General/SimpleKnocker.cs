namespace Destruction
{
    public abstract class SimpleKnocker : ObjectKnocker
    {
        public override void OnBroke()
        {
            TearOff();
        }
    }
}
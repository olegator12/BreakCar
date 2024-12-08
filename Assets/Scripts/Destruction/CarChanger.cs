namespace Destruction
{
    public class CarChanger
    {
        public CarSetup Current { get; private set; }

        public void Change(CarSetup car)
        {
            Current = car;
        }
    }
}
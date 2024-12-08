using System;

namespace Weapons
{
    public interface IUpdatable
    {
        public void UpdateDelay(TimeSpan touchDelay);
    }
}
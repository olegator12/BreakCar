using UnityEngine;

namespace Weapons
{
    public interface IRotatable : IRevertible, IUpdatable, ICancellable
    {
        public void Read(Vector2 input);

        public void Read(Vector3 input);

        public void InitializeSound(ObjectPool<SpawnableSound> soundPool);
    }
}
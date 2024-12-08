using DG.Tweening;
using UnityEngine;

namespace Destruction
{
    public class ObjectShaker
    {
        private const string LayerName = "Destroyed Part";
        private const float AnimationDuration = 0.12f;
        private const float ShakeStrength = 0.06f;
        private const float TearStrength = 5.2f;

        private readonly Transform _transform;
        private readonly GameObject _gameObject;
        private readonly MeshCollider _collider;
        private readonly int _layer;

        public ObjectShaker(Transform transform)
        {
            _transform = transform;
            _gameObject = _transform.gameObject;
            _layer = LayerMask.NameToLayer(LayerName);

            if (_gameObject.TryGetComponent(out MeshCollider collider) == false)
                return;

            _collider = collider;
        }

        public void Shake()
        {
            _transform.DOShakePosition(AnimationDuration, ShakeStrength);
        }

        public void TearOff(Vector3 direction)
        {
            _collider.convex = true;
            _gameObject.AddComponent<Rigidbody>().AddForce(direction * TearStrength, ForceMode.Impulse);
            _gameObject.layer = _layer;
        }
    }
}
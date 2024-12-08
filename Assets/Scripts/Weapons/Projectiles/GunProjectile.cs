using UnityEngine;

namespace Weapons
{
    public class GunProjectile : EmptyProjectile
    {
        [SerializeField] private ParticleSystem _groundEffect;
        [SerializeField] private MeshRenderer _renderer;

        public override void OnContactComplete()
        {
            Freeze();
            Transform.up = Vector3.up;
            _renderer.enabled = false;
        }

        public override void OnPushProjectile()
        {
            _renderer.enabled = true;
        }

        public override void PlayEffect(bool isCarPart)
        {
            if (isCarPart == true)
                Effect.Play();
            else
                _groundEffect.Play();
        }
    }
}
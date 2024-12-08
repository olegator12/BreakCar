using UnityEngine;

namespace Weapons
{
    public class SoundProjectile : EmptyProjectile
    {
        [SerializeField] private AudioSource _explosion;
        [SerializeField] private MeshRenderer _renderer;

        public override void OnContactComplete()
        {
            Freeze();
            _renderer.enabled = false;
        }

        public override void OnPushProjectile()
        {
            _renderer.enabled = true;
        }

        public override void PlayEffect(bool isCarPart)
        {
            Effect.Play();
            _explosion.Play();
        }
    }
}
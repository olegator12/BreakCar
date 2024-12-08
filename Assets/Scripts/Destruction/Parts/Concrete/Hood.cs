using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(PaintPart))]
    public class Hood : ObjectKnocker
    {
        [SerializeField] private ParticleSystem _smoke;

        public override CarPart Name => CarPart.Hood;

        public override void OnBroke()
        {
            _smoke.Play();
            TearOff();
        }
    }
}
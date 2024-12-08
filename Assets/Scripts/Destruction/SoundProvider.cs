using UnityEngine;

namespace Destruction
{
    public class SoundProvider : MonoBehaviour
    {
        [field: SerializeField] public AudioSource Hitting { get; private set; }

        [field: SerializeField] public AudioSource GlassHitting { get; private set; }

        [field: SerializeField] public AudioSource GlassBreaking { get; private set; }
    }
}
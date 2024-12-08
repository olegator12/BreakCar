using UnityEngine;

public class MovableParticle : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private ParticleSystem _particle;

    public void PlayAt(Vector3 position)
    {
        _transform.position = position;
        _particle.Play();
    }
}
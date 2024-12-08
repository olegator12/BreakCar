using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpawnableSound : SpawnableObject
{
    private const float MinPitch = 0.95f;
    private const float MaxPitch = 1.05f;

    private AudioSource _sound;
    private bool _didInitialize;

    public SpawnableSound Initialize()
    {
        if (_didInitialize == true)
            return this;

        _didInitialize = true;
        _sound = GetComponent<AudioSource>();
        return this;
    }

    public AudioSource GetSource()
    {
        return _sound;
    }

    public void Play(AudioClip clip)
    {
        _sound.clip = clip;
        _sound.pitch = Random.Range(MinPitch, MaxPitch);
        _sound.Play();
        PushBack().Forget();
    }

    private async UniTaskVoid PushBack()
    {
        while (_sound.isPlaying == true)
            await UniTask.NextFrame();

        Push();
    }
}
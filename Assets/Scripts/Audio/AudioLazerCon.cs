using UnityEngine;

public class AudioLazerCon : AudioController
{
    [SerializeField] private AudioSource _audioSourceBump;
    [SerializeField] private AudioClip[] _audioClips;

    public void Play() 
    {
        PlaySound(_audioSourceBump, _audioClips[Random.Range(0, _audioClips.Length - 1)]);
    }
}

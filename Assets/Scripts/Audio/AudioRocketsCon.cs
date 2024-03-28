using UnityEngine;

public class AudioRocketsCon : AudioController
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClips;

    public void Play() 
    {
        PlaySound(_audioSource, _audioClips[Random.Range(0, _audioClips.Length - 1)]);
        Disconnect();
    }
}
using UnityEngine;

public class UiSoundCon : AudioController
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClipFirst;
    [SerializeField] private AudioClip _audioClipSecond;

    public void FirstSound() 
    {
        PlaySound(_audioSource, _audioClipFirst);
    }
    public void SecondSound() 
    {
        PlaySound(_audioSource, _audioClipSecond);
    }
}

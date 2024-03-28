using System.Collections;
using UnityEngine;

public class MusicGameCon : AudioController
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClips;
    private int indexClip;
    
    void Start() 
    {
        StartCoroutine(playEngineSound());
    }

    IEnumerator playEngineSound()
    {
        if (_audioSource.clip == null) 
        {
            indexClip = Random.Range(0,_audioClips.Length - 1);
        }

        if (indexClip == _audioClips.Length) {
            indexClip = 0;
        }

        PlaySound(_audioSource,_audioClips[indexClip]);

        indexClip ++;

        yield return new WaitForSeconds(_audioSource.clip.length);       

        StartCoroutine(playEngineSound());
    }
}
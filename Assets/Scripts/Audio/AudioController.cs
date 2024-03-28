using UnityEngine;

public abstract class AudioController : MonoBehaviour
{
    public void PlaySound(AudioSource source, AudioClip clip) 
    {
        source.clip = clip;
        source.Play();
    }
    public void Disconnect()
    {
        this.transform.parent = null;
        Destroy(this.gameObject, 2f);
    }
    public void ChangePitch(AudioSource source, float count) 
    {
        source.pitch = count;
    }
}
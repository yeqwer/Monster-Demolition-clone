using UnityEngine;

public class MusicToggle : MonoBehaviour
{
    public void OnValueChanged(bool value)
    {
        Settings.Audio.MuteMusic = value;
    }
}
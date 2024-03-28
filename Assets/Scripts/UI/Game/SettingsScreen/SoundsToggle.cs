using UnityEngine;

public class SoundsToggle : MonoBehaviour
{
    public void OnValueChanged(bool value)
    {
        Settings.Audio.MuteSounds = value;
    }
}
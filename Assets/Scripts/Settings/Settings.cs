using UnityEngine;
using UnityEngine.Audio;

public static class Settings
{
    public static class Audio
    {
        private const string _soundsKeyword = "SoundsVolume";

        private const string _musicKeyword = "MusicVolume";

        private static AudioMixer _masterMixer;

        private static float _soundsMutedVolume = -80f;

        private static float _musicMutedVolume = -80f;

        private static float _soundsDefaultVolume = 0f;

        private static float _musicDefaultVolume = 0f;

        public static bool MuteSounds
        {
            get
            {
                _masterMixer.GetFloat(_soundsKeyword, out float volume);

                if (volume <= _soundsMutedVolume)
                    return true;

                return false;
            }
            set
            {
                if (value is true)
                    _masterMixer.SetFloat(_soundsKeyword, _soundsMutedVolume);
                else
                    _masterMixer.SetFloat(_soundsKeyword, _soundsDefaultVolume);
            }
        }

        public static bool MuteMusic
        {
            get
            {
                _masterMixer.GetFloat(_musicKeyword, out float volume);

                if (volume <= _musicMutedVolume)
                    return true;

                return false;
            }
            set
            {
                if (value is true)
                    _masterMixer.SetFloat(_musicKeyword, _musicMutedVolume);
                else
                    _masterMixer.SetFloat(_musicKeyword, _musicDefaultVolume);
            }
        }

        static Audio()
        {
            InitializeAudioMixer();
        }

        private static void InitializeAudioMixer()
        {
            _masterMixer = Resources.Load<AudioMixer>("AudioMixers/Mixer");

#if UNITY_EDITOR
            Debug.Log($"Sound mixer is loaded: {_masterMixer != null}");
#endif
        }
    }
}
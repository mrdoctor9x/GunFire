using GenifyStudio.Scripts.Tools;
using System;
using UnityEngine;

namespace GenifyStudio.Scripts.Manager
{
    public enum SoundFX
    {
        FireSilence,
        Running
    }

    public enum MusicType
    {
        Lobby,
        InGame
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource sfxLoopSource;

        [SerializeField] private AudioClip[] musicClips;
        [SerializeField] private AudioClip[] sfxClips;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                Instance.Reset();
            }

            DontDestroyOnLoad(gameObject);
        }

        public void Reset()
        {
            sfxSource.Stop();
            musicSource.Stop();
            musicSource.time = 0;
        }

        public void PlayMusic(MusicType musicType)
        {
            if (!SettingConfig.Instance.IsMusicOn) return;
            musicSource.clip = musicClips[(int)musicType];
            musicSource.Play();
        }

        public void PlayOneHit(SoundFX soundFX)
        {
            if (!SettingConfig.Instance.IsSfxOn) return;
            sfxSource.PlayOneShot(sfxClips[(int)soundFX]);
        }
        public void PlaySfxLoop(SoundFX soundFX)
        {
            if (!SettingConfig.Instance.IsMusicOn) return;
            sfxLoopSource.clip = sfxClips[(int)soundFX];
            sfxLoopSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void Pause()
        {
            musicSource.Pause();
        }public void StopSfxLoop()
        {
            sfxLoopSource.Stop();
        }

        public void UnPause()
        {
            musicSource.UnPause();
        }
    }

    public class SettingConfig
    {
        private static SettingConfig instance;
        private Setting setting;

        public static SettingConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SettingConfig();
                    instance.LoadPreferences();
                }

                return instance;
            }
        }

        #region Properties

        public bool IsMusicOn
        {
            get => setting.isMusicOn;
            set
            {
                setting.isMusicOn = value;
                SavePreferences();
            }
        }

        public bool IsSfxOn
        {
            get => setting.isSfxOn;
            set
            {
                setting.isSfxOn = value;
                SavePreferences();
            }
        }

        public bool IsVibrateOn
        {
            get => setting.isVibrateOn;
            set
            {
                setting.isVibrateOn = value;
                SavePreferences();
            }
        }

        #endregion

        #region preferences

        public void LoadPreferences()
        {
            setting = PrefsManager.GetCurrentSetting();
        }

        private void SavePreferences()
        {
            PrefsManager.UpdateSetting(setting);
        }

        #endregion
    }

    [Serializable]
    public class Setting
    {
        public bool isMusicOn;
        public bool isSfxOn;
        public bool isVibrateOn;

        public Setting()
        {
            isMusicOn = true;
            isSfxOn = true;
            isVibrateOn = true;
        }
    }
}
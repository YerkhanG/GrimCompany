using System;
using Unity.VisualScripting;
using UnityEngine;

public enum SoundType
{
    SWORD,
    BOW,
    HURT,
    DEATH,
    CLICK,
    BOW_HIT,
    BUFF,
    HEAL
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds
    {
        get => sounds;
    }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}

namespace audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [Header("Sound Effects")]
        [SerializeField] private SoundList[] soundList;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;
        
        [Header("Background Music")]
        [SerializeField] private AudioClip bgmClip;
        [SerializeField] [Range(0f, 1f)] private float bgmVolume = 0.5f;
        [SerializeField] private bool playBGMOnStart = true;
        
        private static AudioManager _instance;
        private AudioSource _sfxSource;
        private AudioSource _bgmSource;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                
                _sfxSource = GetComponent<AudioSource>();
                if (_sfxSource == null)
                {
                    Debug.LogError("AudioManager: AudioSource component not found!");
                }
                
                _bgmSource = gameObject.AddComponent<AudioSource>();
                _bgmSource.loop = true;
                _bgmSource.playOnAwake = false;
                _bgmSource.volume = bgmVolume;
                
                LoadVolumes();
            }
            else
            {
                if (_instance.bgmClip != bgmClip)
                {
                    Debug.Log($"Switching BGM from {_instance.bgmClip?.name} to {bgmClip?.name}");
                    _instance.bgmClip = bgmClip;
                    
                    if (playBGMOnStart && bgmClip != null)
                    {
                        _instance.PlayBGMInternal();
                    }
                    else if (bgmClip == null)
                    {
                        _instance.StopBGMInternal();
                    }
                }
                
                _instance.soundList = soundList;
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (_instance == this && playBGMOnStart && bgmClip != null)
            {
                PlayBGMInternal();
            }
        }

        private void LoadVolumes()
        {
            bgmVolume = PlayerPrefs.GetFloat("BGMVolume", bgmVolume);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", sfxVolume);
            
            if (_bgmSource != null)
                _bgmSource.volume = bgmVolume;
        }

        public float getCurrentSFXVolume()
        {
            return sfxVolume;
        }

        public static void PlaySound(SoundType sound, float volume = 1)
        {
            if (_instance == null || _instance._sfxSource == null)
                return;
            
            int soundIndex = (int)sound;
            
            if (_instance.soundList == null || soundIndex >= _instance.soundList.Length)
            {
                Debug.LogError($"Sound {sound} not found in soundList!");
                return;
            }
            
            AudioClip[] clips = _instance.soundList[soundIndex].Sounds;
            AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
            
            _instance._sfxSource.PlayOneShot(randomClip, volume * _instance.sfxVolume);
        }
        
        private void PlayBGMInternal()
        {
            if (_bgmSource == null || bgmClip == null)
                return;
                
            _bgmSource.clip = bgmClip;
            _bgmSource.Play();
        }
        
        public static void PlayBGM()
        {
            _instance?.PlayBGMInternal();
        }
        
        private void StopBGMInternal()
        {
            if (_bgmSource == null)
                return;
                
            _bgmSource.Stop();
        }
        
        public static void StopBGM()
        {
            _instance?.StopBGMInternal();
        }
        
        public static void SetBGMVolume(float volume)
        {
            if (_instance == null || _instance._bgmSource == null)
                return;
            
            _instance.bgmVolume = Mathf.Clamp01(volume);
            _instance._bgmSource.volume = _instance.bgmVolume;
            PlayerPrefs.SetFloat("BGMVolume", _instance.bgmVolume);
            PlayerPrefs.Save();
        }
        
        public static void SetSFXVolume(float volume)
        {
            if (_instance == null)
                return;
            
            _instance.sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("SFXVolume", _instance.sfxVolume);
            PlayerPrefs.Save();
        }
        
        public static float GetBGMVolume()
        {
            return _instance != null ? _instance.bgmVolume : 0.5f;
        }
        
        public static float GetSFXVolume()
        {
            return _instance != null ? _instance.sfxVolume : 1f;
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            string[] names = Enum.GetNames(typeof(SoundType));
            Array.Resize(ref soundList, names.Length);

            for (int i = 0; i < soundList.Length; i++)
            {
                soundList[i].name = names[i];
            }
        }
        #endif
    }
}
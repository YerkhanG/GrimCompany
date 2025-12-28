using audio;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingsController : MonoBehaviour
    {
        [Header("Volume Sliders")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        
        private void Start()
        {
            bgmSlider.value = AudioManager.GetBGMVolume();
            sfxSlider.value = AudioManager.GetSFXVolume();
            
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        private void OnBGMVolumeChanged(float value)
        {
            AudioManager.SetBGMVolume(value);
        }
        
        private void OnSFXVolumeChanged(float value)
        {
            AudioManager.SetSFXVolume(value);
            AudioManager.PlaySound(SoundType.CLICK);
        }
        
        private void OnDestroy()
        {
            bgmSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
            sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }
    }
}
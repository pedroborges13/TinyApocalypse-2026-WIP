using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VolumeUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void OnEnable()
    {
        if (AudioManager.Instance != null)
        {
            //The first GetValue argument is the player's saved value. If none exists, it defaults to 10 (slider range: 0-17)
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 10f); //"MusicVolume" is the exact name of the exposed parameter in the AudioMixer
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 10f); //"SFXVolume" is the exact name of the exposed parameter in the AudioMixer
        }
    }
    void Start()
    {
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }

    void OnMusicChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }
    
    void OnSFXChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
}

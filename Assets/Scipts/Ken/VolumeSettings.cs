using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume") && PlayerPrefs.HasKey("SFXVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    public void SetMusicVolume()
    {
        float volume = Mathf.Clamp(musicSlider.value, 0.0001f, 1f);
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = Mathf.Clamp(SFXSlider.value, 0.0001f, 1f);
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadVolume()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVolume");
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume");

        musicSlider.value = musicVol;
        SFXSlider.value = sfxVol;

        SetMusicVolume();
        SetSFXVolume();
    }
}

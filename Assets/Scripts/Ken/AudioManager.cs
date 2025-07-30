using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ApplySavedVolumes();
    }

    // Public getter for myMixer
    public AudioMixer GetMixer()
    {
        return myMixer;
    }

    public void ApplySavedVolumes()
    {
        float musicVol = PlayerPrefs.GetFloat("musicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        myMixer.SetFloat("Music", Mathf.Log10(Mathf.Clamp(musicVol, 0.0001f, 1f)) * 20);
        myMixer.SetFloat("SFX", Mathf.Log10(Mathf.Clamp(sfxVol, 0.0001f, 1f)) * 20);
    }
}
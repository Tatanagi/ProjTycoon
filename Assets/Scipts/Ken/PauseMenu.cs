using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuUI;
    public Slider sfxSlider;
    public Slider musicSlider;

    [Header("Audio")]
    public AudioMixer audioMixer;

    [Header("Settings")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);

        // Initialize sliders to match mixer values
        float sfxVolume, musicVolume;
        audioMixer.GetFloat("SFXVolume", out sfxVolume);
        audioMixer.GetFloat("MusicVolume", out musicVolume);

        sfxSlider.value = Mathf.Pow(10, sfxVolume / 20);   // Convert from dB to [0,1]
        musicSlider.value = Mathf.Pow(10, musicVolume / 20);
    }

    public void PauseGame()
    {
        if (isPaused) return;

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20); // [0,1] → dB
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }
}

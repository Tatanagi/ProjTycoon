using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button backButton;
    public Button exitButton;

    [Header("Audio")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    // Start is called before the first frame update
    void Start()
    {
        // Assign button listeners
        playButton.onClick.AddListener(PlayGame);
        settingsButton.onClick.AddListener(OpenSettings);
        backButton.onClick.AddListener(CloseSettings);
        exitButton.onClick.AddListener(ExitGame);

        // Assign slider listeners
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            // Initialize volume
            SetMusicVolume(musicSlider.value);
        }
        else
        {
            Debug.LogWarning("Music Slider is not assigned in the Inspector!");
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            // Initialize volume
            SetSFXVolume(sfxSlider.value);
        }
        else
        {
            Debug.LogWarning("SFX Slider is not assigned in the Inspector!");
        }

        // Ensure correct panel states
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings opened");
        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        Debug.Log("Settings closed");
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();

#if UNITY_EDITOR
        // This line only runs in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
        else
        {
            Debug.LogWarning("Music AudioSource is not assigned!");
        }
    }

    void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
        else
        {
            Debug.LogWarning("SFX AudioSource is not assigned!");
        }
    }
}
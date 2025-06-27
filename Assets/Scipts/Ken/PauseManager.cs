using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        // Initialize state
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;

        // Setup button listeners
        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(TogglePause);
        quitButton.onClick.AddListener(() => SceneManager.LoadScene("Main Menu"));
    }

    public void TogglePause()
    {
        bool shouldPause = !pauseCanvas.activeSelf;
        pauseCanvas.SetActive(shouldPause);
        Time.timeScale = shouldPause ? 0f : 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f; // Critical for editor workflow
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;

    private void Start()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;

        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(TogglePause);
    }

    public void TogglePause()
    {
        bool shouldPause = !pauseCanvas.activeSelf;
        pauseCanvas.SetActive(shouldPause);
        Time.timeScale = shouldPause ? 0f : 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f; //Resets time when destroyed
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject creditsCanvas;
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button backToSettingsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        settingsCanvas.SetActive(false);

        settingsButton.onClick.AddListener(ToggleSettings);
        creditsButton.onClick.AddListener(ToggleCredits);
        backToMenuButton.onClick.AddListener(ToggleMainMenu);
        backToSettingsButton.onClick.AddListener(ToggleSettings);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void ToggleSettings()
    {
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
        creditsCanvas.SetActive(false);
    }
    public void ToggleCredits()
    {
        settingsCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
    }

    public void ToggleMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Game has been closed");
        Application.Quit();
    }
}

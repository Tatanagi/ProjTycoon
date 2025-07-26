using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EndSceneResultsDisplay : MonoBehaviour
{
    [Header("Player UI Elements")]
    [SerializeField]
    [Tooltip("TextMeshProUGUI for Player 1's results")]
    private TextMeshProUGUI player1ResultsText;

    [SerializeField]
    [Tooltip("TextMeshProUGUI for Player 2's results")]
    private TextMeshProUGUI player2ResultsText;

    [SerializeField]
    [Tooltip("TextMeshProUGUI for Player 3's results")]
    private TextMeshProUGUI player3ResultsText;

    [SerializeField]
    [Tooltip("TextMeshProUGUI for Player 4's results")]
    private TextMeshProUGUI player4ResultsText;

    [Header("Button Settings")]
    [SerializeField]
    [Tooltip("The UI Button to return to Main Menu")]
    private Button mainMenuButton;

    void Start()
    {
        // Check if all UI elements are assigned
        if (player1ResultsText == null || player2ResultsText == null ||
            player3ResultsText == null || player4ResultsText == null)
        {
            Debug.LogError("One or more Player Results TextMeshProUGUI fields are not assigned in the Inspector!");
        }

        if (mainMenuButton == null)
        {
            Debug.LogError("Main Menu Button is not assigned in the Inspector!");
        }
        else
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        DisplayResults();
    }

    private void OnDestroy()
    {
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }
    }

    private void DisplayResults()
    {
        // Display results for each player
        player1ResultsText.text = GetPlayerResults(1);
        player2ResultsText.text = GetPlayerResults(2);
        player3ResultsText.text = GetPlayerResults(3);
        player4ResultsText.text = GetPlayerResults(4);
    }

    private string GetPlayerResults(int playerNumber)
    {
        int bronze = PlayerPrefs.GetInt($"Player{playerNumber}_Bronze", 0);
        int silver = PlayerPrefs.GetInt($"Player{playerNumber}_Silver", 0);
        int gold = PlayerPrefs.GetInt($"Player{playerNumber}_Gold", 0);
        int turnips = PlayerPrefs.GetInt($"Player{playerNumber}_Turnips", 0);
        int shinyPennies = PlayerPrefs.GetInt($"Player{playerNumber}_ShinyPennies", 0);

        return $"Player{playerNumber} Bronze: {bronze}, Silver: {silver}, Gold: {gold}, Turnips: {turnips}, Shiny Pennies: {shinyPennies}";
    }

    private void GoToMainMenu()
    {
        Debug.Log("Loading Main Menu Workspace scene...");
        SceneManager.LoadScene("Main Menu Workspace");
    }
}
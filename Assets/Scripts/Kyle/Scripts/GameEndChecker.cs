using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndChecker : MonoBehaviour
{
    [Header("End Game Settings")]
    [SerializeField]
    [Tooltip("Number of Shiny Pennies required to win the game")]
    private int shinyPennyQuota = 100;

    private PlayerController[] players;

    void Start()
    {
        // Initialize players array
        players = new PlayerController[4];
        for (int i = 0; i < 4; i++)
        {
            string tag = "Player" + (i + 1);
            GameObject playerObj = GameObject.FindGameObjectWithTag(tag);

            if (playerObj != null)
            {
                players[i] = playerObj.GetComponent<PlayerController>();
                if (players[i] != null && players[i].inventory != null)
                {
                    players[i].inventory.OnChanged += CheckForGameEnd;
                }
                else
                {
                    Debug.LogWarning($"Player{i + 1} is missing PlayerController or Inventory.");
                }
            }
            else
            {
                Debug.LogWarning($"Player{i + 1} not found in scene.");
            }
        }

        CheckForGameEnd();
    }

    private void OnDestroy()
    {
        foreach (var player in players)
        {
            if (player != null && player.inventory != null)
            {
                player.inventory.OnChanged -= CheckForGameEnd;
            }
        }
    }

    private void CheckForGameEnd()
    {
        foreach (var player in players)
        {
            if (player != null && player.inventory != null)
            {
                if (player.inventory.ShinyPenniesValue >= shinyPennyQuota)
                {
                    Debug.Log($"{player.name} has reached the Shiny Penny quota of {shinyPennyQuota}! Ending game.");
                    SavePlayerData();
                    SceneManager.LoadScene("ENDSCENE");
                    return;
                }
            }
        }
    }

    private void SavePlayerData()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null && players[i].inventory != null)
            {
                PlayerPrefs.SetInt($"Player{i + 1}_Bronze", players[i].inventory.BronzeValue);
                PlayerPrefs.SetInt($"Player{i + 1}_Silver", players[i].inventory.SilverValue);
                PlayerPrefs.SetInt($"Player{i + 1}_Gold", players[i].inventory.GoldValue);
                PlayerPrefs.SetInt($"Player{i + 1}_Turnips", players[i].inventory.TurnipsValue);
                PlayerPrefs.SetInt($"Player{i + 1}_ShinyPennies", players[i].inventory.ShinyPenniesValue);
            }
            else
            {
                // Set defaults if player is missing
                PlayerPrefs.SetInt($"Player{i + 1}_Bronze", 0);
                PlayerPrefs.SetInt($"Player{i + 1}_Silver", 0);
                PlayerPrefs.SetInt($"Player{i + 1}_Gold", 0);
                PlayerPrefs.SetInt($"Player{i + 1}_Turnips", 0);
                PlayerPrefs.SetInt($"Player{i + 1}_ShinyPennies", 0);
            }
        }
        PlayerPrefs.Save();
    }
}
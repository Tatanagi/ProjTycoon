/*using TMPro;
using UnityEngine;

public class CoinDisplayByTurn : MonoBehaviour
{
    public TMP_Text goldNumTextCoin;
    public TMP_Text silverNumTextCoin;
    public TMP_Text bronzeNumTextCoin;

    public TurnManager turnManager;

    private PlayerController[] players;

    void Start()
    {
        // Load all players by tag
        players = new PlayerController[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player" + (i + 1));
            if (playerObj != null)
                players[i] = playerObj.GetComponent<PlayerController>();
            else
                Debug.LogWarning($"Player{i + 1} not found!");
        }

        UpdateCoinDisplay();
    }

    void Update()
    {
        // Update in real-time (optional, can use a delegate/event for performance)
        UpdateCoinDisplay();
    }

    public void UpdateCoinDisplay()
    {
        int currentPlayerIndex = turnManager.GetCurrentPlayerIndex();

        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Length)
        {
            PlayerController currentPlayer = players[currentPlayerIndex];

            if (currentPlayer != null)
            {
                goldNumTextCoin.text = currentPlayer.inventory.Gold.ToString();
                silverNumTextCoin.text = currentPlayer.inventory.Silver.ToString();
                bronzeNumTextCoin.text = currentPlayer.inventory.Bronze.ToString();
            }
        }
    }
}
*/

using TMPro;
using UnityEngine;

public class CoinDisplayByTurn : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text goldNumTextCoin;
    public TMP_Text silverNumTextCoin;
    public TMP_Text bronzeNumTextCoin;

    [Header("Turn Manager")]
    public TurnManager turnManager;

    private PlayerController[] players;

    void Start()
    {
        // Initialize player array
        players = new PlayerController[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player" + (i + 1));
            if (playerObj != null)
            {
                players[i] = playerObj.GetComponent<PlayerController>();

                if (players[i] == null)
                {
                    Debug.LogWarning($"Player{i + 1} has no PlayerController attached.");
                }
            }
            else
            {
                Debug.LogWarning($"Player{i + 1} not found in the scene!");
            }
        }

        UpdateCoinDisplay();
    }

    void Update()
    {
        // Optional: real-time coin updates every frame (can be optimized)
        UpdateCoinDisplay();
    }

    public void UpdateCoinDisplay()
    {
        int currentPlayerIndex = turnManager.GetCurrentPlayerIndex();

        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Length)
        {
            PlayerController currentPlayer = players[currentPlayerIndex];

            if (currentPlayer != null && currentPlayer.inventory != null)
            {
                goldNumTextCoin.text = currentPlayer.inventory.Gold.ToString();
                silverNumTextCoin.text = currentPlayer.inventory.Silver.ToString();
                bronzeNumTextCoin.text = currentPlayer.inventory.Bronze.ToString();
            }
            else
            {
                Debug.LogWarning($"Player {currentPlayerIndex + 1} or their inventory is missing!");
            }
        }
        else
        {
            Debug.LogWarning("Invalid player index in TurnManager.");
        }
    }
}

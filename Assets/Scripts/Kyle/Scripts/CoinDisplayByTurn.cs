using TMPro;
using UnityEngine;

public class CoinDisplayByTurn : MonoBehaviour
{
    [Header("Player 1 UI Elements")]
    public TMP_Text goldTextP1;
    public TMP_Text silverTextP1;
    public TMP_Text bronzeTextP1;

    [Header("Player 2 UI Elements")]
    public TMP_Text goldTextP2;
    public TMP_Text silverTextP2;
    public TMP_Text bronzeTextP2;

    [Header("Player 3 UI Elements")]
    public TMP_Text goldTextP3;
    public TMP_Text silverTextP3;
    public TMP_Text bronzeTextP3;

    [Header("Player 4 UI Elements")]
    public TMP_Text goldTextP4;
    public TMP_Text silverTextP4;
    public TMP_Text bronzeTextP4;

    private PlayerController[] players = new PlayerController[4];

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player" + (i + 1));
            if (playerObj != null)
            {
                players[i] = playerObj.GetComponent<PlayerController>();

                if (players[i] != null && players[i].inventory != null)
                {
                    players[i].inventory.OnChanged += UpdateCoinDisplay;
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

        UpdateCoinDisplay(); // Initial update
    }

    private void OnDestroy()
    {
        foreach (var player in players)
        {
            if (player != null && player.inventory != null)
                player.inventory.OnChanged -= UpdateCoinDisplay;
        }
    }

    public void UpdateCoinDisplay()
    {
        if (players[0]?.inventory != null)
        {
            goldTextP1.text = players[0].inventory.Gold.ToString();
            silverTextP1.text = players[0].inventory.Silver.ToString();
            bronzeTextP1.text = players[0].inventory.Bronze.ToString();
        }

        if (players[1]?.inventory != null)
        {
            goldTextP2.text = players[1].inventory.Gold.ToString();
            silverTextP2.text = players[1].inventory.Silver.ToString();
            bronzeTextP2.text = players[1].inventory.Bronze.ToString();
        }

        if (players[2]?.inventory != null)
        {
            goldTextP3.text = players[2].inventory.Gold.ToString();
            silverTextP3.text = players[2].inventory.Silver.ToString();
            bronzeTextP3.text = players[2].inventory.Bronze.ToString();
        }

        if (players[3]?.inventory != null)
        {
            goldTextP4.text = players[3].inventory.Gold.ToString();
            silverTextP4.text = players[3].inventory.Silver.ToString();
            bronzeTextP4.text = players[3].inventory.Bronze.ToString();
        }
    }
}

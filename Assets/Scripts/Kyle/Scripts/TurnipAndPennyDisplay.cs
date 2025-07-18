using TMPro;
using UnityEngine;

public class TurnipAndPennyDisplay : MonoBehaviour
{
    [Header("Player 1 UI Elements")]
    public TMP_Text turnipNumTextP1;
    public TMP_Text shinyPennyNumTextP1;

    [Header("Player 2 UI Elements")]
    public TMP_Text turnipNumTextP2;
    public TMP_Text shinyPennyNumTextP2;

    [Header("Player 3 UI Elements")]
    public TMP_Text turnipNumTextP3;
    public TMP_Text shinyPennyNumTextP3;

    [Header("Player 4 UI Elements")]
    public TMP_Text turnipNumTextP4;
    public TMP_Text shinyPennyNumTextP4;

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
                    players[i].inventory.OnChanged += UpdateDisplay;
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

        UpdateDisplay(); // Initial display update
    }

    private void OnDestroy()
    {
        foreach (var player in players)
        {
            if (player != null && player.inventory != null)
                player.inventory.OnChanged -= UpdateDisplay;
        }
    }

    private void UpdateDisplay()
    {
        if (players[0]?.inventory != null)
        {
            turnipNumTextP1.text = players[0].inventory.TurnipsValue.ToString();      // Use TurnipsValue
            shinyPennyNumTextP1.text = players[0].inventory.ShinyPenniesValue.ToString(); // Use ShinyPenniesValue
        }

        if (players[1]?.inventory != null)
        {
            turnipNumTextP2.text = players[1].inventory.TurnipsValue.ToString();      // Use TurnipsValue
            shinyPennyNumTextP2.text = players[1].inventory.ShinyPenniesValue.ToString(); // Use ShinyPenniesValue
        }

        if (players[2]?.inventory != null)
        {
            turnipNumTextP3.text = players[2].inventory.TurnipsValue.ToString();      // Use TurnipsValue
            shinyPennyNumTextP3.text = players[2].inventory.ShinyPenniesValue.ToString(); // Use ShinyPenniesValue
        }

        if (players[3]?.inventory != null)
        {
            turnipNumTextP4.text = players[3].inventory.TurnipsValue.ToString();      // Use TurnipsValue
            shinyPennyNumTextP4.text = players[3].inventory.ShinyPenniesValue.ToString(); // Use ShinyPenniesValue
        }
    }
}
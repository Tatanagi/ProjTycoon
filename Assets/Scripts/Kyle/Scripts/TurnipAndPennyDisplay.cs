using TMPro;
using UnityEngine;

public class TurnipAndPennyDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text turnipNumText;
    public TMP_Text shinyPennyNumText;

    [Header("Player Settings")]
    [Tooltip("Set this to 0 for Player1, 1 for Player2, etc.")]
    public int playerIndex;

    private PlayerController player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player" + (playerIndex + 1));
        if (playerObj != null)
        {
            player = playerObj.GetComponent<PlayerController>();

            if (player == null)
            {
                Debug.LogWarning($"Player{playerIndex + 1} is missing PlayerController!");
            }
        }
        else
        {
            Debug.LogWarning($"Player{playerIndex + 1} not found in scene!");
        }

        if (player != null && player.inventory != null)
        {
            player.inventory.OnChanged += UpdateDisplay;
            UpdateDisplay();
        }
    }

    void OnDestroy()
    {
        if (player != null && player.inventory != null)
            player.inventory.OnChanged -= UpdateDisplay;
    }

    public void UpdateDisplay()
    {
        if (player != null && player.inventory != null)
        {
            turnipNumText.text = player.inventory.Turnips.ToString();
            shinyPennyNumText.text = player.inventory.ShinyPennies.ToString();
        }
    }
}
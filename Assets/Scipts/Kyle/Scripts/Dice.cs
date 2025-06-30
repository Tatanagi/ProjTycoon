using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dice : MonoBehaviour
{
    public Sprite[] diceSides;               // Dice sprites for 1-6
    public Button rollButton;                // Button to trigger roll
    public TurnManager turnManager;          // ScriptableObject managing turn logic
    public Image diceImageDisplay;           // Image component for showing the result (optional)

    private PlayerController[] players;
    private bool coroutineAllowed = true;
    public TurnUIController turnUIController;

    void Start()
    {
        // Load dice sprites from Resources/DiceSprites folder
        diceSides = Resources.LoadAll<Sprite>("DiceSprites");

        // Auto-load players based on tags Player1–4
        players = new PlayerController[4];
        for (int i = 0; i < 4; i++)
        {
            string tag = "Player" + (i + 1);
            GameObject playerObj = GameObject.FindGameObjectWithTag(tag);

            if (playerObj != null)
            {
                players[i] = playerObj.GetComponent<PlayerController>();
            }
            else
            {
                Debug.LogWarning("No GameObject found with tag: " + tag);
            }
        }

        // Add listener to the Roll button
        if (rollButton != null)
        {
            rollButton.onClick.AddListener(() => StartCoroutine(RollDice()));
        }

        Debug.Log("TurnManager starting at index: " + turnManager.GetCurrentPlayerIndex());
    }

    private IEnumerator RollDice()
    {
        if (!coroutineAllowed || turnManager == null)
            yield break;

        coroutineAllowed = false;

        int currentPlayerIndex = turnManager.GetCurrentPlayerIndex();

        // Safety check
        if (currentPlayerIndex < 0 || currentPlayerIndex >= players.Length)
        {
            Debug.LogError("Invalid player index from TurnManager.");
            coroutineAllowed = true;
            yield break;
        }

        PlayerController currentPlayer = players[currentPlayerIndex];

        if (currentPlayer == null)
        {
            Debug.LogWarning("Player at index " + currentPlayerIndex + " is missing.");
            coroutineAllowed = true;
            yield break;
        }

        // Roll the dice (1 to 6)
        int diceRoll = Random.Range(1, 7);
        Debug.Log("Player " + (currentPlayerIndex + 1) + " rolled: " + diceRoll);

        // Optional: update dice sprite visually
        if (diceSides != null && diceSides.Length >= 6 && diceImageDisplay != null)
        {
            diceImageDisplay.sprite = diceSides[diceRoll - 1];
        }

        // Move the player
        currentPlayer.MovePlayer(diceRoll);

        yield return new WaitUntil(() => currentPlayer.IsFinishedMoving);

        // Move to next player
        turnManager.NextTurn();

        // Update UI for next player's turn
        if (turnUIController != null)
        {
            turnUIController.UpdateTurnUI();
        }

        coroutineAllowed = true;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dice : MonoBehaviour
{
    public Sprite[] diceSides;
    public Button rollButton;
    public TurnManager turnManager;

    private PlayerController[] players;
    private bool coroutineAllowed = true;

    void Start()
    {
        // Load all dice sprites from Resources/DiceSprites folder
        diceSides = Resources.LoadAll<Sprite>("DiceSprites");

        // Automatically find and assign all players based on tags
        players = new PlayerController[4];
        for (int i = 0; i < 4; i++)
        {
            string tag = "Player" + (i + 1); // e.g., "Player1"
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

        // Attach roll button listener
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

        // Index safety check
        if (currentPlayerIndex < 0 || currentPlayerIndex >= players.Length)
        {
            Debug.LogError("Invalid player index from TurnManager: " + currentPlayerIndex);
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

        // Move the player
        currentPlayer.MovePlayer(diceRoll);

        // Wait until the player finishes moving
        yield return new WaitUntil(() => currentPlayer.IsFinishedMoving);

        // Advance to the next turn
        turnManager.NextTurn();
        coroutineAllowed = true;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dice : MonoBehaviour
{
    public GameObject diceObject;            // 3D dice object
    public Button rollButton;                // Button to trigger roll
    public TurnManager turnManager;          // ScriptableObject managing turn logic

    private PlayerController[] players;
    private bool coroutineAllowed = true;

    [Header("Testing Options")]
    public bool testMode = false;            // Toggle for test mode
    [Range(1, 40)]
    public int testDiceNumber = 1;           // Number to use when test mode is enabled

    [Header("Face Transforms")]
    public Transform[] faceTransforms = new Transform[6]; // Index 0 = Face 1, ..., Index 5 = Face 6

    void Start()
    {
        // Validate faceTransforms assignment
        for (int i = 0; i < faceTransforms.Length; i++)
        {
            if (faceTransforms[i] == null)
            {
                Debug.LogError($"Missing transform for dice face value {i + 1}. Please assign all 6 face transforms in the inspector.");
            }
        }

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

        // Choose dice roll
        int diceRoll = testMode ? testDiceNumber : Random.Range(1, 7);
        Debug.Log("Player " + (currentPlayerIndex + 1) + " rolled: " + diceRoll + (testMode ? " (TEST MODE)" : ""));

        // Animate the dice roll
        yield return StartCoroutine(AnimateDiceRoll(diceRoll));

        // Move the player
        currentPlayer.MovePlayer(diceRoll);

        // Wait for movement to complete
        yield return new WaitUntil(() => currentPlayer.IsFinishedMoving);

        // Award resource, then resolve tile
        GameManager.Instance.OnPlayerFinishMove(currentPlayer, diceRoll);
        BoardCell currentCell = currentPlayer.GetCurrentCell().GetComponent<BoardCell>();
        if (currentCell != null)
        {
            switch (currentCell.cellType)
            {
                case CellType.CommunityChest:
                    UIManager.Instance.ShowCommunityChestCard(currentPlayer);
                    break;

                case CellType.LuckyLoanLender:
                    UIManager.Instance.ShowLoanOffer(currentPlayer);
                    break;

                case CellType.RoyalMint:
                    UIManager.Instance.ShowExchange();
                    break;

                case CellType.Stables:
                case CellType.Quarry:
                case CellType.Fishery:
                case CellType.WheatField:
                case CellType.MiningShaft:
                case CellType.Thief:
                    UIManager.Instance.ShowCellAction(GetCellActionTitle(currentCell.cellType), GetCellActionDescription(currentCell.cellType), currentPlayer, () =>
                    {
                        ExecuteCellAction(currentCell.cellType, currentPlayer);
                    });
                    break;

                default:
                    GameManager.Instance.ResolveLanding(currentPlayer);
                    TurnUIController.Instance.UpdateTurnUI();
                    break;
            }
        }
        else
        {
            GameManager.Instance.ResolveLanding(currentPlayer);
            TurnUIController.Instance.UpdateTurnUI();
        }

        // Advance turn
        turnManager.NextTurn();

        coroutineAllowed = true;
    }

    private IEnumerator AnimateDiceRoll(int diceRoll)
    {
        // Reset dice rotation
        diceObject.transform.rotation = Quaternion.identity;

        // Roll animation with random rotations
        float rollDuration = 1f;
        float interval = 0.1f;
        int steps = Mathf.FloorToInt(rollDuration / interval);

        Debug.Log("Starting dice roll animation...");

        for (int i = 0; i < steps; i++)
        {
            Vector3 randomRotation = new Vector3(
                Random.Range(-90, 90),
                Random.Range(-90, 90),
                Random.Range(-90, 90)
            );

            diceObject.transform.Rotate(randomRotation, Space.World);
            Debug.Log($"Roll Step {i + 1}/{steps} - Rotation Applied: {randomRotation} | Current Rotation: {diceObject.transform.rotation.eulerAngles}");

            yield return new WaitForSeconds(interval);
        }

        // Snap the dice to the correct rotation using transform reference
        Quaternion finalRotation;

        if (diceRoll >= 1 && diceRoll <= 6 && faceTransforms[diceRoll - 1] != null)
        {
            finalRotation = faceTransforms[diceRoll - 1].rotation;
        }
        else
        {
            Debug.LogWarning("Missing or invalid transform for dice face " + diceRoll + ". Using identity rotation.");
            finalRotation = Quaternion.identity;
        }

        diceObject.transform.rotation = finalRotation;

        Debug.Log($"Final dice result: {diceRoll} | Final rotation set to: {finalRotation.eulerAngles}");

        yield return new WaitForSeconds(0.5f);
    }

    private string GetCellActionTitle(CellType cellType)
    {
        return cellType.ToString();
    }

    private string GetCellActionDescription(CellType cellType)
    {
        switch (cellType)
        {
            case CellType.Stables: return "Gain 1 shiny penny and 1 random resource!";
            case CellType.Quarry: return "Gain 1 Bronze Token!";
            case CellType.Fishery: return "Gain 1 Silver Token!";
            case CellType.WheatField: return "Gain 5 Turnips! (Double during Turnip Craze)";
            case CellType.MiningShaft: return "Gain 1 Gold Token!";
            case CellType.Thief: return "Lose up to 20% Bronze, 10% Silver, 5% Gold to a thief!";
            default: return "";
        }
    }

    private void ExecuteCellAction(CellType cellType, PlayerController player)
    {
        switch (cellType)
        {
            case CellType.Stables: Stables.Execute(player); break;
            case CellType.Quarry: Quarry.Execute(player); break;
            case CellType.Fishery: Fishery.Execute(player); break;
            case CellType.WheatField: WheatField.Execute(player); break;
            case CellType.MiningShaft: MiningShaft.Execute(player); break;
            case CellType.Thief: Thief.Execute(player); break;
        }
        GameManager.Instance.ResolveLanding(player);
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dice : MonoBehaviour
{
    public static Dice Instance { get; private set; }

    public GameObject diceObject;
    public Button rollButton;
    public TurnManager turnManager;

    private PlayerController[] players;
    private bool coroutineAllowed = true;
    public bool actionConfirmed = false;

    [Header("Testing Options")]
    public bool testMode = false;
    [Range(1, 40)]
    public int testDiceNumber = 1;

    [Header("Face Transforms")]
    public Transform[] faceTransforms = new Transform[6];

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < faceTransforms.Length; i++)
        {
            if (faceTransforms[i] == null)
            {
                Debug.LogError($"Missing transform for dice face value {i + 1}. Please assign all 6 face transforms in the inspector.");
            }
        }

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

        if (rollButton != null)
        {
            rollButton.onClick.AddListener(() => StartCoroutine(RollDice()));
        }

        Debug.Log("TurnManager starting at index: " + turnManager.GetCurrentPlayerIndex());

        StartCoroutine(InitializeGameWithResourceTokens());
    }

    private IEnumerator InitializeGameWithResourceTokens()
    {
        int currentPlayerIndex = turnManager.GetCurrentPlayerIndex();
        PlayerController currentPlayer = players[currentPlayerIndex];

        if (currentPlayer == null)
        {
            Debug.LogError("Current player is null at index " + currentPlayerIndex + ". Ensure all player GameObjects are tagged (Player1–Player4) and have PlayerController.");
            yield break;
        }

        BoardCell startCell = currentPlayer.GetCurrentCell();
        if (startCell == null)
        {
            Debug.LogError("Player's current cell is null. Ensure GetCurrentCell() is implemented and the player starts on a valid cell with a BoardCell component.");
            yield break;
        }

        if (startCell.cellType == CellType.ResourceTokens)
        {
            startCell.OnPlayerLanded(currentPlayer);
            actionConfirmed = false;
            yield return new WaitUntil(() => actionConfirmed);
        }
        else
        {
            Debug.LogWarning("Starting cell is not ResourceTokens. Cell type: " + (startCell != null ? startCell.cellType.ToString() : "null"));
        }

        TurnUIController.Instance.StartFirstTurn();
        TurnUIController.Instance.UpdateTurnUI();
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

        int diceRoll = testMode ? testDiceNumber : Random.Range(1, 7);
        Debug.Log("Player " + (currentPlayerIndex + 1) + " rolled: " + diceRoll + (testMode ? " (TEST MODE)" : ""));

        yield return StartCoroutine(AnimateDiceRoll(diceRoll));

        currentPlayer.MovePlayer(diceRoll);

        yield return new WaitUntil(() => currentPlayer.IsFinishedMoving);

        GameManager.Instance.OnPlayerFinishMove(currentPlayer, diceRoll);
        BoardCell currentCell = currentPlayer.GetCurrentCell();
        if (currentCell != null)
        {
            actionConfirmed = false;
            currentCell.OnPlayerLanded(currentPlayer);
            if (currentCell.cellType != CellType.Normal)
            {
                yield return new WaitUntil(() => actionConfirmed);
            }
            else
            {
                GameManager.Instance.ResolveLanding(currentPlayer);
            }
        }
        else
        {
            GameManager.Instance.ResolveLanding(currentPlayer);
        }

        turnManager.NextTurn();

        // Only update UI if not starting a new round
        if (turnManager.GetCurrentPlayerIndex() != 0)
        {
            TurnUIController.Instance.UpdateTurnUI();
        }

        coroutineAllowed = true;
    }

    private IEnumerator AnimateDiceRoll(int diceRoll)
    {
        diceObject.transform.rotation = Quaternion.identity;

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

    public void OnActionConfirmed()
    {
        actionConfirmed = true;
    }
}

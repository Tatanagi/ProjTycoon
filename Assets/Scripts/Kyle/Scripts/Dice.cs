using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Dice : MonoBehaviour
{
    public static Dice Instance { get; private set; }
    public GameObject diceObject;
    public Button rollButton;
    public TurnManager turnManager;
    public AudioSource diceRollAudioSource; // AudioSource for dice roll and button SFX
    public AudioClip diceRollClip; // Assign dice roll sound clip in Inspector
    [SerializeField] private AudioClip buttonClickClip; // Button click sound clip
    [SerializeField] private AudioClip diceRollingClip; // Rolling sound clip for animation
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 0.5f; // Volume for all dice SFX
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
        // Ensure AudioSource is set up
        if (diceRollAudioSource == null)
        {
            diceRollAudioSource = gameObject.AddComponent<AudioSource>();
            diceRollAudioSource.playOnAwake = false;
            // Assumes AudioMixer has an "SFX" group; set in Inspector or via code
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager != null && audioManager.GetMixer() != null)
            {
                diceRollAudioSource.outputAudioMixerGroup = audioManager.GetMixer().FindMatchingGroups("SFX")[0];
            }
            else
            {
                Debug.LogWarning("AudioManager or AudioMixer not found. Ensure AudioManager is in the scene and myMixer is assigned.");
            }
        }
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
        if (diceRollClip == null)
        {
            Debug.LogWarning("Dice roll AudioClip not assigned in Inspector. Please assign a dice roll sound.");
        }
        if (buttonClickClip == null)
        {
            Debug.LogWarning("Button click AudioClip not assigned in Inspector. Please assign a button click sound.");
        }
        if (diceRollingClip == null)
        {
            Debug.LogWarning("Dice rolling AudioClip not assigned in Inspector. Please assign a dice rolling sound.");
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
            startCell.OnPlayerLanded(currentPlayer, turnManager); // Pass turnManager
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
        // Play button click sound
        if (diceRollAudioSource != null && buttonClickClip != null)
        {
            diceRollAudioSource.PlayOneShot(buttonClickClip, sfxVolume);
        }
        else
        {
            Debug.LogWarning("Cannot play button click sound: AudioSource or buttonClickClip is missing.");
        }
        yield return null; // Allow button sound to play briefly
        // Play dice roll sound
        if (diceRollAudioSource != null && diceRollClip != null)
        {
            diceRollAudioSource.PlayOneShot(diceRollClip, sfxVolume);
        }
        else
        {
            Debug.LogWarning("Cannot play dice roll sound: AudioSource or diceRollClip is missing.");
        }
        yield return StartCoroutine(AnimateDiceRoll(diceRoll));
        currentPlayer.MovePlayer(diceRoll);
        yield return new WaitUntil(() => currentPlayer.IsFinishedMoving);
        GameManager.Instance.OnPlayerFinishMove(currentPlayer, diceRoll);
        BoardCell currentCell = currentPlayer.GetCurrentCell();
        if (currentCell != null)
        {
            actionConfirmed = false;
            currentCell.OnPlayerLanded(currentPlayer, turnManager); // Pass turnManager
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
        // Play rolling sound for the duration
        if (diceRollAudioSource != null && diceRollingClip != null)
        {
            diceRollAudioSource.PlayOneShot(diceRollingClip, sfxVolume);
        }
        else
        {
            Debug.LogWarning("Cannot play dice rolling sound: AudioSource or diceRollingClip is missing.");
        }
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
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Game State")]
    public bool hasLoan = false;
    public int loanAmount = 0;
    public bool isInDebt = false;

    [Header("Inventory")]
    public PlayerInventory inventory { get; private set; }

    [Header("Movement")]
    [SerializeField]
    private BoardCell[] boardCells;
    [SerializeField]
    private int startingCellIndex = 0;
    [SerializeField]
    private float moveSpeed = 2f;
    [SerializeField]
    private int currentCellIndex = -1;
    public bool IsFinishedMoving { get; private set; } = true;
    public BoardCell CurrentCell { get; private set; }

    [Header("Audio")]
    [SerializeField]
    private AudioSource stepAudioSource; // AudioSource for step SFX
    [SerializeField]
    private AudioClip stepSound; // Chess-like step sound
    [SerializeField]
    [Range(0f, 1f)]
    private float stepVolume = 0.5f; // Volume for subtle chess-like sound

    [Header("Turn Management")]
    [SerializeField]
    private bool turnAround;
    [SerializeField]
    private int tempCellIndex;

    private void Awake()
    {
        // Initialize inventory
        inventory = GetComponent<PlayerInventory>();
        if (!inventory)
        {
            Debug.LogError($"{name} is missing PlayerInventory component!");
        }
        else
        {
            inventory.Initialize();
        }

        // Validate board cells
        if (boardCells == null || boardCells.Length == 0)
        {
            Debug.LogError($"{name} has no board cells assigned! Assign the boardCells array in the Inspector.");
        }
        else if (startingCellIndex < 0 || startingCellIndex >= boardCells.Length)
        {
            Debug.LogError($"{name} startingCellIndex ({startingCellIndex}) is out of range. Set to 0.");
            currentCellIndex = 0;
        }
        else
        {
            currentCellIndex = startingCellIndex;
            CurrentCell = boardCells[currentCellIndex];
            transform.position = CurrentCell.transform.position;
            Debug.Log($"{name} initialized at {CurrentCell.cellType} (Index: {currentCellIndex}).");
        }

        // Initialize AudioSource
        if (stepAudioSource == null)
        {
            stepAudioSource = GetComponent<AudioSource>();
            if (stepAudioSource == null)
            {
                stepAudioSource = gameObject.AddComponent<AudioSource>();
                Debug.LogWarning($"{name} had no AudioSource. Added one automatically.");
            }
        }
        stepAudioSource.playOnAwake = false;
        stepAudioSource.loop = false;
        stepAudioSource.spatialBlend = 0f; // 2D sound for board game
    }

    public BoardCell GetCurrentCell()
    {
        if (CurrentCell == null)
        {
            Debug.LogWarning($"{name} current cell is null. This may indicate an initialization issue.");
        }
        return CurrentCell;
    }

    public void MovePlayer(int steps)
    {
        if (!IsFinishedMoving)
        {
            Debug.LogWarning($"{name} is still moving. Cannot start new movement.");
            return;
        }
        if (steps <= 0)
        {
            Debug.LogWarning($"{name} cannot move {steps} steps. Must be positive.");
            IsFinishedMoving = true;
            return;
        }
        StartCoroutine(MoveSteps(steps));
    }

    private IEnumerator MoveSteps(int steps)
    {
        IsFinishedMoving = false;

        for (int i = 0; i < steps; i++)
        {
            if (boardCells == null || boardCells.Length == 0)
            {
                Debug.LogError($"{name} has no board cells to move to!");
                IsFinishedMoving = true;
                yield break;
            }

            int nextIndex = (currentCellIndex + 1) % boardCells.Length;
            tempCellIndex += 1;
            Vector3 nextPos = boardCells[nextIndex].transform.position;

            // Play chess-like step sound
            if (stepSound != null && stepAudioSource != null)
            {
                stepAudioSource.PlayOneShot(stepSound, stepVolume);
            }
            else
            {
                Debug.LogWarning($"{name} cannot play step sound: AudioClip or AudioSource is missing.");
            }

            // Move to next position
            while (Vector3.Distance(transform.position, nextPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentCellIndex = nextIndex;
            CurrentCell = boardCells[currentCellIndex];
            Debug.Log($"{name} moved to: {CurrentCell.cellType} (Index: {currentCellIndex})");
            yield return new WaitForSeconds(0.1f); // Brief pause for sound clarity
        }

        IsFinishedMoving = true;

        // Handle turn-around logic
        if (tempCellIndex >= 39)
        {
            turnAround = true;
            tempCellIndex = currentCellIndex;
            Debug.Log($"{name} completed a board loop. Triggering turn-around.");
        }

        if (turnAround)
        {
            isInDebt = false;
            turnAround = false;
            Debug.Log($"{name} turn-around: Debt cleared.");
        }

        // Trigger cell-specific action
        if (CurrentCell != null && GameManager.Instance != null && GameManager.Instance.turnManager != null)
        {
            CurrentCell.OnPlayerLanded(this, GameManager.Instance.turnManager);
        }
        else
        {
            Debug.LogWarning($"{name} could not trigger OnPlayerLanded: CurrentCell or TurnManager is null.");
        }
    }

    public void StartNewRound()
    {
        if (hasLoan)
        {
            int debtAmount = loanAmount * 2;
            if (inventory.Spend(ResourceType.ShinyPennies, debtAmount))
            {
                hasLoan = false;
                loanAmount = 0;
                isInDebt = false;
                Debug.Log($"{name} repaid loan of {debtAmount} shiny pennies.");
            }
            else
            {
                isInDebt = true;
                hasLoan = false;
                loanAmount = 0;
                Debug.Log($"{name} failed to repay loan → in debt for one round.");
            }
        }
        else
        {
            isInDebt = false;
            Debug.Log($"{name} starting new round with no debt penalty.");
        }
    }

    public void SetCurrentCell(BoardCell newCell)
    {
        if (newCell == null)
        {
            Debug.LogError($"{name} attempted to set null current cell.");
            return;
        }

        CurrentCell = newCell;
        currentCellIndex = System.Array.IndexOf(boardCells, newCell);
        if (currentCellIndex < 0)
        {
            Debug.LogWarning($"{name} set to a cell not in boardCells array. Movement may be affected.");
            currentCellIndex = 0;
            CurrentCell = boardCells[currentCellIndex];
        }
        transform.position = CurrentCell.transform.position;
        Debug.Log($"{name} set to cell: {CurrentCell.cellType} (Index: {currentCellIndex})");
    }

    public void TakeLoan(int amount)
    {
        if (hasLoan)
        {
            Debug.LogWarning($"{name} already has a loan. Cannot take another.");
            return;
        }
        if (amount <= 0)
        {
            Debug.LogWarning($"{name} cannot take a loan of {amount}. Must be positive.");
            return;
        }
        hasLoan = true;
        loanAmount = amount;
        inventory.Add(ResourceType.ShinyPennies, amount);
        Debug.Log($"{name} took a loan of {amount} shiny pennies.");
    }

    public int GetCurrentCellIndex()
    {
        return currentCellIndex;
    }
}
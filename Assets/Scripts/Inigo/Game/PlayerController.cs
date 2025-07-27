using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Game State")]
    public bool hasLoan = false;
    public bool isInJail = false;
    public bool tokenGainBanned = false;

    [Header("Inventory")]
    public PlayerInventory inventory { get; private set; }

    [Header("Movement")]
    [SerializeField]
    private BoardCell[] boardCells; // Array of all board cells, set in Inspector
    [SerializeField]
    private int startingCellIndex = 0; // Index of the starting cell, set in Inspector
    public float moveSpeed = 2f;
    private int currentCellIndex = -1; // Initialize to -1, set in Awake
    public bool IsFinishedMoving { get; private set; } = true;
    public BoardCell currentCell { get; private set; }

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
            inventory.Initialize(); // Initialize with default values
        }

        // Set the starting cell
        if (boardCells == null || boardCells.Length == 0)
        {
            Debug.LogError($"{name} has no board cells assigned! Assign the boardCells array in the Inspector.");
        }
        else if (startingCellIndex < 0 || startingCellIndex >= boardCells.Length)
        {
            Debug.LogError($"{name} startingCellIndex ({startingCellIndex}) is out of range. Set a valid index (0 to {boardCells.Length - 1}).");
            currentCellIndex = 0; // Default to first cell as fallback
        }
        else
        {
            currentCellIndex = startingCellIndex;
            currentCell = boardCells[currentCellIndex];
            transform.position = currentCell.transform.position; // Move player to starting position
            Debug.Log($"{name} initialized at {currentCell.cellType} (Index: {currentCellIndex}).");
        }
    }

    public BoardCell GetCurrentCell()
    {
        if (currentCell == null)
        {
            Debug.LogWarning($"{name} current cell is null. This may indicate an initialization issue.");
        }
        return currentCell;
    }

    public void MovePlayer(int steps)
    {
        if (IsFinishedMoving && !isInJail) // Prevent movement if in jail
            StartCoroutine(MoveSteps(steps));
        else if (isInJail)
            Debug.LogWarning($"{name} is in jail and cannot move.");
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
            Vector3 nextPos = boardCells[nextIndex].transform.position;

            while (Vector3.Distance(transform.position, nextPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentCellIndex = nextIndex;
            yield return new WaitForSeconds(0.1f); // Brief pause between cells
        }

        currentCell = boardCells[currentCellIndex];
        Debug.Log($"{name} landed on: {currentCell.cellType} (Index: {currentCellIndex})");
        currentCell.OnPlayerLanded(this);

        IsFinishedMoving = true;
    }

    public void StartNewRound()
    {
        if (hasLoan)
        {
            int payment = Mathf.RoundToInt(inventory.ShinyPenniesValue * 0.2f);
            if (inventory.Spend(ResourceType.ShinyPennies, payment))
            {
                hasLoan = false;
                Debug.Log($"{name} repaid loan of {payment} shiny pennies.");
            }
            else
            {
                isInJail = true;
                tokenGainBanned = true;
                hasLoan = false;
                Debug.Log($"{name} failed to repay loan → jailed & token-banned.");
            }
        }
        else
        {
            isInJail = false;
            tokenGainBanned = false;
        }
    }

    // Optional: Method to set the current cell (e.g., after movement or initialization)
    public void SetCurrentCell(BoardCell newCell)
    {
        if (newCell != null)
        {
            currentCell = newCell;
            currentCellIndex = System.Array.IndexOf(boardCells, newCell);
            if (currentCellIndex < 0)
            {
                Debug.LogWarning($"{name} set to a cell not in boardCells array. Movement may be affected.");
            }
            transform.position = currentCell.transform.position;
        }
        else
        {
            Debug.LogError($"{name} attempted to set null current cell.");
        }
    }
}

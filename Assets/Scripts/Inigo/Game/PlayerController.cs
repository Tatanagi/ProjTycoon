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
    public float moveSpeed = 2f;
    private int currentCellIndex = -1;
    public bool IsFinishedMoving { get; private set; } = true;
    public BoardCell currentCell { get; private set; }

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        if (!inventory)
        {
            Debug.LogError($"{name} is missing PlayerInventory component!");
        }
        else
        {
            inventory.Initialize();
        }

        if (boardCells == null || boardCells.Length == 0)
        {
            Debug.LogError($"{name} has no board cells assigned! Assign the boardCells array in the Inspector.");
        }
        else if (startingCellIndex < 0 || startingCellIndex >= boardCells.Length)
        {
            Debug.LogError($"{name} startingCellIndex ({startingCellIndex}) is out of range. Set a valid index (0 to {boardCells.Length - 1}).");
            currentCellIndex = 0;
        }
        else
        {
            currentCellIndex = startingCellIndex;
            currentCell = boardCells[currentCellIndex];
            transform.position = currentCell.transform.position;
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
        if (IsFinishedMoving)
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
            Vector3 nextPos = boardCells[nextIndex].transform.position;

            while (Vector3.Distance(transform.position, nextPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentCellIndex = nextIndex;
            yield return new WaitForSeconds(0.1f);
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
            isInDebt = false; // Reset debt status at round start
            Debug.Log($"{name} starting new round with no debt penalty.");
        }
    }

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
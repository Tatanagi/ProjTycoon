using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool hasLoan = false;
    public bool isInJail = false;
    public bool tokenGainBanned = false;

    public PlayerInventory inventory { get; private set; }

    [Header("Movement")]
    public BoardCell[] boardCells;
    public float moveSpeed = 2f;
    private int currentCellIndex = 0;
    public bool IsFinishedMoving { get; private set; } = true;
    public BoardCell currentCell { get; private set; }

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        if (!inventory)
        {
            Debug.LogError($"{name} is missing PlayerInventory!");
        }
    }

    public BoardCell GetCurrentCell()
    {
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
        Debug.Log($"{name} landed on: {currentCell.cellType}");
        currentCell.OnPlayerLanded(this);

        IsFinishedMoving = true;
    }

    public void StartNewRound()
    {
        if (hasLoan)
        {
            int payment = Mathf.RoundToInt(inventory.ShinyPennies * 0.2f);
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
}

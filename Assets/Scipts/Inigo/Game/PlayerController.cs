using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int gold = 0;
    public int silver = 0;
    public int bronze = 0;
    public int shinyPennies = 0;
    public int turnips = 0;
    public int roundTurnips = 0;

    public bool hasLoan = false;
    public bool isInJail = false;
    public bool tokenGainBanned = false;

    public Transform[] boardCells;
    public float moveSpeed = 2f;
    private int currentCellIndex = 0;
    public bool IsFinishedMoving { get; private set; } = true;

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
            int nextIndex = currentCellIndex + 1;
            if (nextIndex >= boardCells.Length)
                break;

            Vector3 nextPos = boardCells[nextIndex].position;

            while (Vector3.Distance(transform.position, nextPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentCellIndex = nextIndex;
            yield return new WaitForSeconds(0.1f);
        }

        IsFinishedMoving = true;
    }

    public void StartNewRound()
    {
        roundTurnips = 0;

        if (hasLoan)
        {
            int payment = shinyPennies * 2;
            if (shinyPennies >= payment)
            {
                shinyPennies -= payment;
                hasLoan = false;
            }
            else
            {
                isInJail = true;
                tokenGainBanned = true;
                hasLoan = false;
                Debug.Log($"{name} failed to repay their loan and is now in jail.");
            }
        }
        else
        {
            isInJail = false;
            tokenGainBanned = false;
        }
    }
}

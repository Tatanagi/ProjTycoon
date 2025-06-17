using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform[] boardCells;  // Set these in the Unity inspector
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
                break;  // Stop if out of bounds

            Vector3 nextPos = boardCells[nextIndex].position;

            while (Vector3.Distance(transform.position, nextPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentCellIndex = nextIndex;
            yield return new WaitForSeconds(0.1f);  // Slight delay per step
        }

        IsFinishedMoving = true;
    }
}

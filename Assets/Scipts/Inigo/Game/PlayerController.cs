using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform[] boardCells;
    public float moveSpeed = 2f;
    private int currentCellIndex = 0;

    public void MovePlayer(int steps)
    {
        StartCoroutine(MoveSteps(steps));
    }

    private IEnumerator MoveSteps(int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            currentCellIndex = (currentCellIndex + 1) % boardCells.Length;
            Vector3 targetPos = boardCells[currentCellIndex].position;

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

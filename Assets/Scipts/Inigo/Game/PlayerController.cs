using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
            currentCellIndex = (currentCellIndex + 1) % boardCells.Length;
            Vector3 targetPos = boardCells[currentCellIndex].position;

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;
            yield return new WaitForSeconds(0.2f);
        }

        string landedTag = boardCells[currentCellIndex].tag;
        Debug.Log("Landed on: " + landedTag);

        HandleCellAction(landedTag);

        IsFinishedMoving = true;
    }

    private void HandleCellAction(string tag)
    {
        switch (tag)
        {
            case "Start":
                Debug.Log("You are at the start!");
                break;
            case "Fish":
            case "Fish 2":
                Debug.Log("You caught some fish!");
                break;
            case "Fishery":
                Debug.Log("You arrived at the fishery!");
                break;
            case "House":
            case "House With Grass":
                Debug.Log("You entered a house.");
                break;
            case "Stone Quarry":
                Debug.Log("You found some stone!");
                break;
            case "Wheat Field":
                Debug.Log("You harvested wheat!");
                break;
            case "Wishing Well With Rocks":
                Debug.Log("You found a wishing well!");
                break;
            case "Lucky Loan Lender":
            case "Loan Lender":
                Debug.Log("You can get a loan here.");
                break;
            case "Silk":
                Debug.Log("You found valuable silk!");
                break;
            case "Bag":
                Debug.Log("You found a mysterious bag.");
                break;
            case "Goats":
                Debug.Log("You encountered some goats.");
                break;
            case "Tree":
                Debug.Log("You found a tree.");
                break;
            case "Royal Mint":
                Debug.Log("You reached the royal mint. Gain coins!");
                break;
            case "Wool":
                Debug.Log("You collected wool.");
                break;
            case "Lettuce":
                Debug.Log("You harvested lettuce.");
                break;
            case "Hat":
                Debug.Log("You found a stylish hat.");
                break;
            case "Tile Branch":
                Debug.Log("A branch blocks your path.");
                break;
            case "Village Fair":
                Debug.Log("Enjoy the fair!");
                break;
            case "Turnips":
                Debug.Log("You found turnips.");
                break;
            case "Stabie":
                Debug.Log("You met Stabie the NPC.");
                break;
            case "Community Chess":
                Debug.Log("Draw a community card!");
                break;
            case "Blank Space":
                Debug.Log("Just passing through.");
                break;
            default:
                Debug.Log("Landed on an unknown tile: " + tag);
                break;
        }
    }
}

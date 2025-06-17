using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dice : MonoBehaviour
{
    public PlayerController player;
    public Image diceImage;            // UI Image to show dice face
    public Sprite[] diceSides;         // Array of dice face sprites (1 to 6)
    public Button rollButton;          // Button to roll the dice

    private bool coroutineAllowed = true;

    void Start()
    {
        // Load dice sprites from Resources/DiceSprites folder
        diceSides = Resources.LoadAll<Sprite>("DiceSprites");

        if (rollButton != null)
            rollButton.onClick.AddListener(() => StartCoroutine(RollDice()));
    }

    private IEnumerator RollDice()
    {
        if (!coroutineAllowed || player == null)
            yield break;

        coroutineAllowed = false;

        int randomDiceNumber = Random.Range(1, 7); // 1–6
        Debug.Log("Rolled: " + randomDiceNumber);

        if (diceImage != null && diceSides.Length >= 6)
        {
            diceImage.sprite = diceSides[randomDiceNumber - 1];
        }

        player.MovePlayer(randomDiceNumber);

        yield return new WaitUntil(() => player.IsFinishedMoving);
        coroutineAllowed = true;
    }
}

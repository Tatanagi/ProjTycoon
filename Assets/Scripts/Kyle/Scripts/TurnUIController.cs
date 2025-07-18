using UnityEngine;
using TMPro;
using System.Collections;

public class TurnUIController : MonoBehaviour
{
    public static TurnUIController Instance { get; private set; }

    public GameObject turnBasedUI;        // UI panel to show/hide
    public TMP_Text turnTMPText;          // TextMeshPro text component
    public TurnManager turnManager;       // Reference to TurnManager ScriptableObject
    public bool firstTurn = true;         // Flag to handle the first turn, now public

    private Coroutine hideCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        // Do not update turn UI on enable for the first turn
        if (!firstTurn && turnBasedUI != null)
        {
            turnBasedUI.SetActive(true);
            UpdateTurnUI();
        }
    }

    public void UpdateTurnUI()
    {
        if (turnBasedUI != null)
            turnBasedUI.SetActive(true);

        if (turnTMPText != null && turnManager != null)
        {
            int playerNumber = turnManager.GetCurrentPlayerIndex() + 1;
            turnTMPText.text = $"Player {playerNumber}'s Turn";
        }

        // Restart the 5-second hide timer
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterDelay(5f));
    }

    public void StartFirstTurn()
    {
        firstTurn = false; // Reset flag after the first turn is confirmed
        UpdateTurnUI();    // Show the turn UI after the first action
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (turnBasedUI != null)
            turnBasedUI.SetActive(false);
    }
}
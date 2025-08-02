using UnityEngine;

[CreateAssetMenu(fileName = "TurnManager", menuName = "Game/Turn Manager")]
public class TurnManager : ScriptableObject
{
    public int currentPlayerIndex = 0;
    public int totalPlayers = 4;
    private int currentRound = 1;

    private void OnEnable()
    {
        currentPlayerIndex = 0;
        currentRound = 1;
    }

    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
        if (currentPlayerIndex == 0)
        {
            currentRound++;
            Debug.Log($"Starting Round {currentRound}");
            // Reset previous round effect and show CA panel
            RandomEffectRounds randomEffects = FindFirstObjectByType<RandomEffectRounds>();
            if (randomEffects != null)
            {
                randomEffects.ResetEffect();
                randomEffects.ApplyRandomEffectWithPanel(currentRound);
            }
            else
            {
                Debug.LogWarning("RandomEffectRounds not found in scene!");
            }
        }
        else
        {
            // Update UI for the next player immediately if not starting a new round
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null && uiManager.turnController != null)
            {
                uiManager.turnController.UpdateTurnUI();
            }
            else
            {
                Debug.LogWarning("UIManager or TurnUIController not found to update turn UI!");
            }
        }
        Debug.Log($"Now it's Player {(currentPlayerIndex + 1)}'s turn.");
    }

    public void UpdateTurnUIAfterEffect()
    {
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null && uiManager.turnController != null)
        {
            uiManager.turnController.UpdateTurnUI();
        }
        else
        {
            Debug.LogWarning("UIManager or TurnUIController not found to update turn UI!");
        }
    }

    public void UpdateTurnUI()
    {
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null && uiManager.turnController != null)
        {
            uiManager.turnController.UpdateTurnUI();
        }
        else
        {
            Debug.LogWarning("UIManager or TurnUIController not found to update turn UI!");
        }
    }

    public int GetCurrentPlayerIndex()
    {
        return currentPlayerIndex;
    }

    public int GetCurrentRound()
    {
        return currentRound;
    }
}

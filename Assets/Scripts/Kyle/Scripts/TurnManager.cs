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
        }
        Debug.Log($"Now it's Player {(currentPlayerIndex + 1)}'s turn.");
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
using UnityEngine;

[CreateAssetMenu(fileName = "TurnManager", menuName = "Game/Turn Manager")]
public class TurnManager : ScriptableObject
{
    public int currentPlayerIndex = 0;
    public int totalPlayers = 4;

    private void OnEnable()
    {
        // Reset the player turn on game start
        currentPlayerIndex = 0;
    }

    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
        Debug.Log("Now it's Player " + (currentPlayerIndex + 1) + "'s turn.");
    }

    public int GetCurrentPlayerIndex()
    {
        return currentPlayerIndex;
    }
}

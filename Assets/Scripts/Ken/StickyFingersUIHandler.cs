using UnityEngine;

public class StickyFingersUIHandler : MonoBehaviour
{
    public TurnManager turnManager;

    // Called by the Confirm button
    public void ConfirmSteal()
    {
        //Gets active player's StickyFingers script
        var currentPlayer = StickyFingers.GetStickyFingersByPlayerIndex(turnManager.GetCurrentPlayerIndex());
        //Call confirm logic
        if (currentPlayer != null)
        {
            currentPlayer.OnStickyFingersConfirmed();
        }
        else
        {
            Debug.LogWarning("Could not find current player's StickyFingers script.");
        }
    }

    // Called by the Cancel button
    public void CancelSteal()
    {
        var currentPlayer = StickyFingers.GetStickyFingersByPlayerIndex(turnManager.GetCurrentPlayerIndex());
        //Closes panel
        if (currentPlayer != null)
        {
            currentPlayer.CloseStickyFingersUI();
        }
        else
        {
            Debug.LogWarning("Could not find current player's StickyFingers script.");
        }
    }
}

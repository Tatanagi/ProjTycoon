using UnityEngine;

//Links the "Use Sticky Fingers" button to the correct player (player on current turn)
public class StickyFingersButtonBridge : MonoBehaviour
{
    public TurnManager turnManager;

    public void OnStickyFingersButtonClicked()
    {
        //Checks whose turn it is
        int currentIndex = turnManager.GetCurrentPlayerIndex();
        //Finds their StickyFingers script
        StickyFingers currentPlayerSF = StickyFingers.GetStickyFingersByPlayerIndex(currentIndex);

        if (currentPlayerSF != null)
        {
            //Opens their UI
            currentPlayerSF.ActivateStickyFingersUI();
        }
        else
        {
            Debug.LogWarning("StickyFingers not found for current player!");
        }
    }
}

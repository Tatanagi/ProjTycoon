using UnityEngine;

public enum CellType
{
    Normal,
    CommunityChest,
    LuckyLoanLender
}

public class BoardCell : MonoBehaviour
{
    public CellType cellType = CellType.Normal;

    public void OnPlayerLanded(PlayerController player)
    {
        switch (cellType)
        {
            case CellType.CommunityChest:
                Debug.Log("Player landed on Community Chest.");
                UIManager.Instance.ShowCommunityChestCard(player);
                break;

            case CellType.LuckyLoanLender:
                Debug.Log("Player landed on LuckyLoanLender.");
                UIManager.Instance.ShowLoanOffer(player);
                break;

            case CellType.Normal:

            default:
                break;
        }
    }
}

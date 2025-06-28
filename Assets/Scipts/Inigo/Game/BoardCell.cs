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
                GameManager.Instance.communityChest.DrawCard(player, GameManager.Instance.GetAllPlayers());
                break;

            case CellType.LuckyLoanLender:
                Debug.Log("Player landed on LuckyLoanLender."); // TEMP TEST
                UIManager.Instance.ShowLoanOffer(player);
                break;

            case CellType.Normal:

            default:
                break;
        }
    }
}

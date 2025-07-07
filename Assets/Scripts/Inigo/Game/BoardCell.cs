using UnityEngine;

public enum CellType
{
    Normal,
    CommunityChest,
    LuckyLoanLender,
    RoyalMint,
    ResourceTokens,
    SuddenShortage
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

            case CellType.RoyalMint:
                Debug.Log("Player landed on Royal Mint.");
                UIManager.Instance.ShowExchange(player);
                break;

            case CellType.ResourceTokens:
                Debug.Log("Player landed on Resource Tokens.");
                GameManager.Instance.GiveStartTileBonus(player);
                break;

            case CellType.SuddenShortage:
                Debug.Log("Player landed on Sudden Shortage!");
                GameManager.Instance.suddenShortage.TryTriggerShortage();
                break;

            case CellType.Normal:
            default:
                break;
        }
    }
}

using UnityEngine;

public enum CellType
{
    Normal,
    CommunityChest,
    LuckyLoanLender,
    RoyalMint,
    ResourceTokens,
    SuddenShortage,
    Stables,
    Quarry,
    Fishery,
    WheatField,
    MiningShaft,
    Thief
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
                UIManager.Instance.ShowExchange();
                break;

            case CellType.ResourceTokens:
                Debug.Log("Player landed on Resource Tokens.");
                GameManager.Instance.GiveStartTileBonus(player);
                break;

            case CellType.SuddenShortage:
                Debug.Log("Player landed on Sudden Shortage!");
                GameManager.Instance.suddenShortage.TryTriggerShortage();
                break;

            case CellType.Stables:
                Debug.Log("Player landed on Stables!");
                Stables.Execute(player);
                break;

            case CellType.Quarry:
                Debug.Log("Player landed on Quarry!");
                Quarry.Execute(player);
                break;

            case CellType.Fishery:
                Debug.Log("Player landed on Fishery!");
                Fishery.Execute(player);
                break;

            case CellType.WheatField:
                Debug.Log("Player landed on Wheat Field!");
                WheatField.Execute(player);
                break;

            case CellType.MiningShaft:
                Debug.Log("Player landed on Mining Shaft!");
                MiningShaft.Execute(player);
                break;

            case CellType.Thief:
                Debug.Log("Player landed on Thief!");
                Thief.Execute(player);
                break;

            case CellType.Normal:
            default:
                break;
        }
    }
}

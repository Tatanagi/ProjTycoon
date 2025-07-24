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
        Debug.Log($"Player {player.name} landed on {cellType}.");
        switch (cellType)
        {
            case CellType.CommunityChest:
                UIManager.Instance.ShowCommunityChestCard(player);
                break;
            case CellType.LuckyLoanLender:
                UIManager.Instance.ShowLoanOffer(player);
                break;
            case CellType.RoyalMint:
                UIManager.Instance.ShowExchange();
                break;
            case CellType.ResourceTokens:
                // At start, show CA panel and let GameManager handle the bonus on confirm
                UIManager.Instance.ShowCellAction(
                    GetCellActionTitle(),
                    GetCellActionDescription(),
                    player,
                    () => GameManager.Instance.GiveStartTileBonus(player) // Trigger bonus on confirm
                );
                break;
            case CellType.SuddenShortage:
            case CellType.Stables:
            case CellType.Quarry:
            case CellType.Fishery:
            case CellType.WheatField:
            case CellType.MiningShaft:
            case CellType.Thief:
                UIManager.Instance.ShowCellAction(
                    GetCellActionTitle(),
                    GetCellActionDescription(),
                    player
                );
                break;
        }
    }

    public string GetCellActionTitle()
    {
        return cellType.ToString();
    }

    public string GetCellActionDescription()
    {
        switch (cellType)
        {
            case CellType.Stables: return "Gain 1 shiny penny and 1 random resource!";
            case CellType.Quarry: return "Gain 1 Bronze Token!";
            case CellType.Fishery: return "Gain 1 Silver Token!";
            case CellType.WheatField: return "Gain 5 Turnips! (Double during Turnip Craze)";
            case CellType.MiningShaft: return "Gain 1 Gold Token!";
            case CellType.Thief: return "Lose up to 20% Bronze, 10% Silver, 5% Gold to a thief!";
            case CellType.ResourceTokens: return "Confirm to receive +5 gold, silver, and bronze!";
            case CellType.SuddenShortage: return "This round will be capped to 20 silver, gold, and bronze";
            default: return "";
        }
    }
}
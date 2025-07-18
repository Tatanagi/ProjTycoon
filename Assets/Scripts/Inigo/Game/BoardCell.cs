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
                Debug.Log("Player landed on Lucky Loan Lender.");
                UIManager.Instance.ShowLoanOffer(player);
                break;

            case CellType.RoyalMint:
                Debug.Log("Player landed on Royal Mint.");
                UIManager.Instance.ShowExchange();
                break;

            case CellType.ResourceTokens:
                Debug.Log("Player landed on Resource Tokens.");
                UIManager.Instance.ShowCellAction(
                    "Start Tile Bonus",
                    "You received +5 gold, silver, and bronze!",
                    player,
                    () =>
                    {
                        GameManager.Instance.GiveStartTileBonus(player);
                        TurnUIController.Instance.UpdateTurnUI(); // Show turn UI only after confirming
                    }
                );
                break;

            case CellType.SuddenShortage:
                Debug.Log("Player landed on Sudden Shortage!");
                SuddenShortage suddenShortage = GameManager.Instance.suddenShortage;
                if (suddenShortage.IsActive ||
                    GameManager.Instance.turnManager.GetCurrentRound() == suddenShortage.TriggeredRound)
                {
                    UIManager.Instance.ShowCellAction(
                        "Sudden Shortage",
                        "You cannot activate sudden shortage it is once per round",
                        player
                    );
                }
                else
                {
                    UIManager.Instance.ShowCellAction(
                        "Sudden Shortage",
                        "This round will be capped to 20 silver, gold, and bronze",
                        player,
                        () => suddenShortage.TryTriggerShortage(player)
                    );
                }
                break;

            case CellType.Stables:
                Debug.Log("Player landed on Stables!");
                UIManager.Instance.ShowCellAction(
                    "Stables",
                    "Gain 1 shiny penny and 1 random resource!",
                    player,
                    () => Stables.Execute(player)
                );
                break;

            case CellType.Quarry:
                Debug.Log("Player landed on Quarry!");
                UIManager.Instance.ShowCellAction(
                    "Quarry",
                    "Gain 1 Bronze Token!",
                    player,
                    () => Quarry.Execute(player)
                );
                break;

            case CellType.Fishery:
                Debug.Log("Player landed on Fishery!");
                UIManager.Instance.ShowCellAction(
                    "Fishery",
                    "Gain 1 Silver Token!",
                    player,
                    () => Fishery.Execute(player)
                );
                break;

            case CellType.WheatField:
                Debug.Log("Player landed on Wheat Field!");
                UIManager.Instance.ShowCellAction(
                    "Wheat Field",
                    "Gain 5 Turnips! (Double during Turnip Craze)",
                    player,
                    () => WheatField.Execute(player)
                );
                break;

            case CellType.MiningShaft:
                Debug.Log("Player landed on Mining Shaft!");
                UIManager.Instance.ShowCellAction(
                    "Mining Shaft",
                    "Gain 1 Gold Token!",
                    player,
                    () => MiningShaft.Execute(player)
                );
                break;

            case CellType.Thief:
                Debug.Log("Player landed on Thief!");
                UIManager.Instance.ShowCellAction(
                    "Thief",
                    "Lose up to 20% Bronze, 10% Silver, 5% Gold to a thief!",
                    player,
                    () => Thief.Execute(player)
                );
                break;

            case CellType.Normal:
            default:
                Debug.Log("Player landed on a Normal cell.");
                TurnUIController.Instance.UpdateTurnUI(); // Show TurnUI for normal cells immediately
                break;
        }
    }
}
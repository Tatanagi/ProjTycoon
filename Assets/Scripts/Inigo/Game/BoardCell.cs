using UnityEngine;
using System;

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
        Action onConfirm = null;

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
                onConfirm = () => GameManager.Instance.GiveStartTileBonus(player);
                break;
            case CellType.SuddenShortage:
                onConfirm = () => SuddenShortage.Execute(player);
                break;
            case CellType.Stables:
                onConfirm = () => Stables.Execute(player);
                break;
            case CellType.Quarry:
                onConfirm = () => Quarry.Execute(player);
                break;
            case CellType.Fishery:
                onConfirm = () => Fishery.Execute(player);
                break;
            case CellType.WheatField:
                onConfirm = () => WheatField.Execute(player);
                break;
            case CellType.MiningShaft:
                onConfirm = () => MiningShaft.Execute(player);
                break;
            case CellType.Thief:
                onConfirm = () => Thief.Execute(player);
                break;
        }

        if (onConfirm != null)
        {
            UIManager.Instance.ShowCellAction(
                GetCellActionTitle(),
                GetCellActionDescription(player),
                player,
                onConfirm
            );
        }
    }

    public string GetCellActionTitle()
    {
        return cellType.ToString();
    }

    public string GetCellActionDescription(PlayerController currentPlayer)
    {
        if (currentPlayer == null) return "";

        switch (cellType)
        {
            case CellType.Stables:
                return "Gain 1 Shiny Penny and 1 random resource!";
            case CellType.Quarry:
                return "Gain 1 Bronze Token!";
            case CellType.Fishery:
                return "Gain 1 Silver Token!";
            case CellType.WheatField:
                bool isTurnipConversionActive = TurnipCraze.Instance != null && TurnipCraze.Instance.isTurnipConversionActive;
                bool isCrazeActive = TurnipCraze.Instance != null && TurnipCraze.Instance.isCrazeActive;
                int turnipsGained = isTurnipConversionActive || isCrazeActive ? 10 : 5;
                string description = $"Gain {turnipsGained} Turnips!";
                if (isTurnipConversionActive)
                    description += " (Increased during Turnip Conversion Effect)";
                else if (isCrazeActive)
                    description += " (Increased during Turnip Craze)";
                return description;
            case CellType.MiningShaft:
                return "Gain 1 Gold Token!";
            case CellType.Thief:
                PlayerController[] allPlayers = GameManager.Instance.GetAllPlayers();
                if (allPlayers == null || allPlayers.Length == 0) return "Thief: No players available!";
                int currentIndex = Array.IndexOf(allPlayers, currentPlayer);
                int targetIndex = (currentIndex + 1) % allPlayers.Length;
                PlayerController targetPlayer = allPlayers[targetIndex];
                return $"{targetPlayer.name}: Lose up to 20% Bronze, 10% Silver, 5% Gold to a thief!";
            case CellType.ResourceTokens:
                return "Confirm to receive +5 gold, +5 silver, and +5 bronze!";
            case CellType.SuddenShortage:
                return "This round will be capped to 20 silver, gold, and bronze for all players!";
            case CellType.LuckyLoanLender:
                return "Would you like a loan worth 10% of your shiny pennies?";
            default:
                return "";
        }
    }
}
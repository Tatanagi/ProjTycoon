using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public PlayerController[] players;
    public ResourceMarket resourceMarket;
    public RoyalDecree royalDecree;
    public CommunityChest communityChest;
    public LuckyLoanLender loanLender;
    public SuddenShortage suddenShortage;
    public TurnManager turnManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
        Debug.Log($"Round {turnManager.GetCurrentRound()} begins!");

        foreach (PlayerController p in players)
        {
            p.StartNewRound();
        }

        royalDecree.GenerateNewDecree();
        resourceMarket.GenerateTokens(turnManager.GetCurrentRound());
    }

    public void OnPlayerFinishMove(PlayerController player, int rollResult)
    {
        if (player == null) return;
        ResolveLanding(player);
    }

    public void GiveStartTileBonus(PlayerController player)
    {
        if (player == null || player.inventory == null) return;

        player.inventory.Add(ResourceType.Gold, 5);
        player.inventory.Add(ResourceType.Silver, 5);
        player.inventory.Add(ResourceType.Bronze, 5);

        UIManager.Instance.ShowCellAction(
            "Start Tile Bonus",
            $"{player.name} received +5 gold, +5 silver, and +5 bronze!",
            player
        );

        Debug.Log($"{player.name} received +5 gold, +5 silver, and +5 bronze.");
    }

    public PlayerController[] GetAllPlayers()
    {
        return players;
    }

    public void ResolveLanding(PlayerController player)
    {
        if (player == null) return;
        BoardCell cell = player.GetCurrentCell();

        if (cell == null)
        {
            Debug.LogWarning("[GameManager] Current cell is null for player " + player.name);
            return;
        }

        switch (cell.cellType)
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
            case CellType.SuddenShortage:
                UIManager.Instance.ShowCellAction(
                    "Sudden Shortage",
                    "This round will be capped to 20 silver, gold, and bronze.",
                    player
                );
                suddenShortage.TryTriggerShortage(player);
                break;
            case CellType.Stables:
            case CellType.Quarry:
            case CellType.Fishery:
            case CellType.WheatField:
            case CellType.MiningShaft:
            case CellType.Thief:
                UIManager.Instance.ShowCellAction(
                    cell.GetCellActionTitle(),
                    cell.GetCellActionDescription(player),
                    player
                );
                break;
            case CellType.ResourceTokens:
                // Handled by CA panel confirm in OnPlayerLanded, no action here
                break;
            case CellType.Normal:
                // Delegate to BoardCell, but check if this is the last player of the round
                cell.OnPlayerLanded(player, turnManager);
                break;
            default:
                Debug.LogWarning("[GameManager] Unhandled cell type: " + cell.cellType);
                break;
        }
    }
}
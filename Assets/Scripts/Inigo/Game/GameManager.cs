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

    public int round = 1;

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
        Debug.Log($"Round {round} begins!");

        foreach (PlayerController p in players)
        {
            p.StartNewRound();
        }

        royalDecree.GenerateNewDecree();
        resourceMarket.GenerateTokens(round);
    }

    public void OnPlayerFinishMove(PlayerController player, int rollResult)
    {
        if (!player.tokenGainBanned)
        {
            resourceMarket.GiveResourceToPlayer(player, rollResult);
        }
    }

    public void GiveStartTileBonus(PlayerController player)
    {
        player.inventory.Add(ResourceType.Gold, 5);
        player.inventory.Add(ResourceType.Silver, 5);
        player.inventory.Add(ResourceType.Bronze, 5);
        Debug.Log($"{player.name} received +5 gold, silver, and bronze.");
    }

    public List<PlayerController> GetAllPlayers()
    {
        return new List<PlayerController>(players);
    }

    public void ResolveLanding(PlayerController player)
    {
        BoardCell cell = player.GetCurrentCell();

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
            case CellType.ResourceTokens:
                GiveStartTileBonus(player);
                break;
            case CellType.SuddenShortage:
                suddenShortage.TryTriggerShortage();
                break;
            case CellType.Normal:
            default:
                TurnUIController.Instance.UpdateTurnUI();
                break;
        }
    }
}
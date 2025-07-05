using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton that coordinates the overall game flow.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public PlayerController[] players;
    public ResourceMarket resourceMarket;
    public RoyalDecreeManager decreeManager;
    public CommunityChest communityChest;
    public LuckyLoanLender loanLender;

    public int round = 1;

    // ---------- Unity Lifecycle ----------

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

    // ---------- Round Management ----------

    public void StartRound()
    {
        Debug.Log($"Round {round} begins!");

        foreach (PlayerController p in players)
        {
            p.StartNewRound();
        }

        decreeManager.GenerateNewDecree();
        resourceMarket.GenerateTokens(round);
    }

    // ---------- Game Events ----------

    /// <summary>
    /// Called when a player finishes movement and is eligible for resources.
    /// </summary>
    public void OnPlayerFinishMove(PlayerController player, int rollResult)
    {
        if (!player.tokenGainBanned)
        {
            resourceMarket.GiveResourceToPlayer(player, rollResult);
        }
    }

    // ---------- Public Utilities ----------

    /// <summary>
    /// Gives 5 gold, silver, and bronze to a player — used for Start tile and ResourceTokens cells.
    /// </summary>
    public void GiveStartTileBonus(PlayerController player)
    {
        player.gold += 5;
        player.silver += 5;
        player.bronze += 5;

        Debug.Log($"{player.name} received +5 gold, silver, and bronze.");
    }

    public List<PlayerController> GetAllPlayers()
    {
        return new List<PlayerController>(players);
    }

    public void ResolveLanding(PlayerController player)
    {
        BoardCell cell = player.GetCurrentCell();

        // EXAMPLE: Replace this with your actual tile type or tag check
        switch (cell.cellType)
        {
            case CellType.CommunityChest:
                UIManager.Instance.ShowDrawCard(player); // TurnUIController will be triggered *after* the panel is hidden
                break;

            case CellType.LuckyLoanLender:
                UIManager.Instance.ShowLoanOffer(player);
                break;

            case CellType.Normal:

            default:
                TurnUIController.Instance.UpdateTurnUI(); // Nothing special — show turn banner immediately
                break;
        }
    }
}

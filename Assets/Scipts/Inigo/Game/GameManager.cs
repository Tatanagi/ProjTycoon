using System.Collections.Generic;
using UnityEngine;

/// <summary>Singleton that coordinates the overall game flow.</summary>
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

    // ---------- Unity ----------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() => StartRound();

    // ---------- Round management ----------

    public void StartRound()
    {
        Debug.Log($"Round {round} begins!");

        foreach (PlayerController p in players) p.StartNewRound();

        decreeManager.GenerateNewDecree();
        resourceMarket.GenerateTokens(round);
    }

    // ---------- Public helpers ----------

    public void OnPlayerFinishMove(PlayerController player, int rollResult)
    {
        if (!player.tokenGainBanned)
            resourceMarket.GiveResourceToPlayer(player, rollResult);
    }

    public List<PlayerController> GetAllPlayers() => new(players);
}

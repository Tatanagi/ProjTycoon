using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerController[] players;
    public ResourceMarket resourceMarket;
    public RoyalDecreeManager decreeManager;
    public CommunityChest communityChest;
    public LuckyLoanLender loanLender;

    public int round = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
        Debug.Log($"Round {round} begins!");

        foreach (var player in players)
        {
            player.StartNewRound();
        }

        decreeManager.GenerateNewDecree();
        resourceMarket.GenerateTokens(round);
    }

    public void OnPlayerFinishMove(PlayerController player, int rollResult)
    {
        if (!player.tokenGainBanned)
        {
            resourceMarket.GiveResourceToPlayer(player, rollResult);
        }
    }

    public List<PlayerController> GetAllPlayers()
    {
        return new List<PlayerController>(players);
    }
}

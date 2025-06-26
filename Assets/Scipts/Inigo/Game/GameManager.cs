using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController[] players;
    public ResourceMarket resourceMarket;
    public RoyalDecreeManager decreeManager;
    public CommunityChest communityChest;
    public LuckyLoanLender loanLender;

    private int currentPlayer = 0;
    private int round = 1;

    void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
        Debug.Log($"Round {round} begins!");

        foreach (var player in players)
            player.StartNewRound();

        decreeManager.GenerateNewDecree();
        resourceMarket.GenerateTokens(round);

        NextTurn();
    }

    public void NextTurn()
    {
        PlayerController player = players[currentPlayer];
        if (player.isInJail)
        {
            Debug.Log($"{player.name} is in jail and misses their turn.");
        }
        else
        {
            int roll = Random.Range(1, 7);
            Debug.Log($"{player.name} rolled a {roll}");
            player.MovePlayer(roll);

            if (!player.tokenGainBanned)
                resourceMarket.GiveResourceToPlayer(player, roll);
        }

        currentPlayer++;
        if (currentPlayer >= players.Length)
        {
            currentPlayer = 0;
            round++;
            StartRound();
        }
    }
}

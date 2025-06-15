using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController[] players;
    public ResourceMarket resourceMarket;
    public RoyalDecreeManager decreeManager;

    private int currentPlayer = 0;
    private int round = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
        Debug.Log($"Round {round} begins!");
        decreeManager.GenerateNewDecree();
        resourceMarket.GenerateTokens(round);
        NextTurn();
    }

    public void NextTurn()
    {
        PlayerController player = players[currentPlayer];
        int roll = Random.Range(1, 7);
        Debug.Log($"Player {currentPlayer + 1} rolled {roll}");
        player.MovePlayer(roll);

        resourceMarket.GiveResourceToPlayer(player, roll);

        currentPlayer++;
        if (currentPlayer >= players.Length)
        {
            currentPlayer = 0;
            round++;
            StartRound();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

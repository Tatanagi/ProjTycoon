using System.Collections.Generic;
using UnityEngine;
using System;

public class CommunityChest : MonoBehaviour
{
    [System.Serializable]
    public class CommunityCard
    {
        public string cardName;
        public string description;
        public int cost;
        public Action<PlayerController, List<PlayerController>> effect;
    }

    public List<CommunityCard> deck = new List<CommunityCard>();

    public void InitializeDeck()
    {
        deck = new List<CommunityCard>()
        {
            new CommunityCard
            {
                cardName = "Steal Gold",
                description = "Pick a player and decrease 2 gold.",
                cost = 8,
                effect = (player, others) =>
                {
                    var target = ChooseTarget(others);
                    target.gold = Mathf.Max(0, target.gold - 2);
                }
            },
            new CommunityCard
            {
                cardName = "Steal Silver",
                description = "Pick a player and decrease 5 silver.",
                cost = 5,
                effect = (player, others) =>
                {
                    var target = ChooseTarget(others);
                    target.silver = Mathf.Max(0, target.silver - 5);
                }
            },
            new CommunityCard
            {
                cardName = "Steal Bronze",
                description = "Pick a player and decrease 10 bronze.",
                cost = 1,
                effect = (player, others) =>
                {
                    var target = ChooseTarget(others);
                    target.bronze = Mathf.Max(0, target.bronze - 10);
                }
            },
            new CommunityCard
            {
                cardName = "Steal Shiny Pennies",
                description = "Pick a player and decrease 5 shiny pennies.",
                cost = 12,
                effect = (player, others) =>
                {
                    var target = ChooseTarget(others);
                    int stolen = Mathf.Min(5, target.shinyPennies);
                    target.shinyPennies -= stolen;
                    player.shinyPennies += stolen;
                }
            },
            new CommunityCard
            {
                cardName = "Gain Extra Pennies",
                description = "Gain 10 shiny pennies.",
                cost = 5,
                effect = (player, others) =>
                {
                    player.shinyPennies += 10;
                }
            },
            new CommunityCard
            {
                cardName = "Gain Turnips",
                description = "All players gain 25 turnips this round only.",
                cost = 4,
                effect = (player, others) =>
                {
                    foreach (var p in others)
                        p.roundTurnips += 25;
                    player.roundTurnips += 25;
                }
            }
        };
    }

    public void DrawCard(PlayerController player, List<PlayerController> allPlayers)
    {
        if (deck.Count == 0) return;

        var card = deck[UnityEngine.Random.Range(0, deck.Count)];

        if (player.shinyPennies < card.cost)
        {
            Debug.Log($"{player.name} doesn't have enough shiny pennies.");
            return;
        }

        player.shinyPennies -= card.cost;
        card.effect.Invoke(player, allPlayers.FindAll(p => p != player));

        Debug.Log($"{player.name} drew a Community Card: {card.cardName} - {card.description}");
    }

    private PlayerController ChooseTarget(List<PlayerController> players)
    {
        return players[UnityEngine.Random.Range(0, players.Count)];
    }
}

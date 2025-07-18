using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Draw-a-card mechanic that lets a player pay a cost to trigger an effect.
/// The deck is created at runtime in <see cref="Awake"/>.
/// </summary>
public class CommunityChest : MonoBehaviour
{
    // ---------- Nested types ----------

    [Serializable]
    public class CommunityCard
    {
        public string cardName;
        public string description;
        public int cost;   // shiny-penny cost to draw
        public Action<PlayerController, List<PlayerController>> effect;
    }

    // ---------- Fields ----------

    private readonly List<CommunityCard> _deck = new();   // run-time deck

    // ---------- Unity ----------

    private void Awake() => InitialiseDeck();

    // ---------- Public API ----------

    /// <summary>Draws (and immediately resolves) a random card, if the player can afford the cost.</summary>
    public void DrawCard(PlayerController player, IReadOnlyList<PlayerController> allPlayers)
    {
        if (_deck.Count == 0) return;

        CommunityCard card = _deck[UnityEngine.Random.Range(0, _deck.Count)];

        if (!player.inventory.CanAfford(ResourceType.ShinyPennies, card.cost))
        {
            Debug.Log($"{player.name} doesn't have enough shiny pennies.");
            return;
        }

        player.inventory.Spend(ResourceType.ShinyPennies, card.cost);
        card.effect.Invoke(player, new List<PlayerController>(allPlayers).FindAll(p => p != player));

        Debug.Log($"{player.name} drew Community Card: {card.cardName} – {card.description}");
    }

    // ---------- Private helpers ----------

    private void InitialiseDeck()
    {
        _deck.Clear();
        _deck.AddRange(new[]
        {
            NewCard("Steal Gold", "Pick a player and decrease 2 gold.", 8,
                (self, others) =>
                {
                    var target = ChooseTarget(others);
                    int lost = Mathf.Min(2, target.inventory.GoldValue); // Use GoldValue
                    target.inventory.Spend(ResourceType.Gold, lost);
                }),

            NewCard("Steal Silver", "Pick a player and decrease 5 silver.", 5,
                (self, others) =>
                {
                    var target = ChooseTarget(others);
                    int lost = Mathf.Min(5, target.inventory.SilverValue); // Use SilverValue
                    target.inventory.Spend(ResourceType.Silver, lost);
                }),

            NewCard("Steal Bronze", "Pick a player and decrease 10 bronze.", 1,
                (self, others) =>
                {
                    var target = ChooseTarget(others);
                    int lost = Mathf.Min(10, target.inventory.BronzeValue); // Use BronzeValue
                    target.inventory.Spend(ResourceType.Bronze, lost);
                }),

            NewCard("Steal Shiny Pennies", "Pick a player and decrease 5 shiny pennies.", 12,
                (self, others) =>
                {
                    var target = ChooseTarget(others);
                    int stolen = Mathf.Min(5, target.inventory.ShinyPenniesValue); // Use ShinyPenniesValue
                    target.inventory.Spend(ResourceType.ShinyPennies, stolen);
                    self.inventory.Add(ResourceType.ShinyPennies, stolen);
                }),

            NewCard("Gain Extra Pennies", "Gain 10 shiny pennies.", 5,
                (self, _) =>
                {
                    self.inventory.Add(ResourceType.ShinyPennies, 10);
                }),

            NewCard("Gain Turnips", "All players gain 25 turnips this round only.", 4,
                (self, others) =>
                {
                    foreach (var p in others) p.inventory.Add(ResourceType.RoundTurnips, 25); // Use Add
                    self.inventory.Add(ResourceType.RoundTurnips, 25); // Use Add
                })
        });
    }

    private static CommunityCard NewCard(string name, string desc, int cost,
        Action<PlayerController, List<PlayerController>> effect) =>
        new() { cardName = name, description = desc, cost = cost, effect = effect };

    private static PlayerController ChooseTarget(IReadOnlyList<PlayerController> players) =>
        players[UnityEngine.Random.Range(0, players.Count)];
}